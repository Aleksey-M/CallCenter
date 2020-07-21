let appFilters = {
    pageSize: 25,
    pageNo: 1,
    nameFilter: '',
    gender: 0,
    minAge: 0,
    maxAge: 0,
    minDaysAfterLastCall: 0,
    isFiltersNotChanged: function () {
        let _pageSize = parseInt($('#iPerPage').val());
        let _nameFilter = $('#nameFilter').val();
        let _gender = parseInt($('#gender').val());
        let _minAge = parseInt($('#minAge').val()) || 0;
        let _maxAge = parseInt($('#maxAge').val()) || 0;
        let _minDaysAfterLastCall = parseInt($('#minDaysAfterLastCall').val()) || 0;

        return this.pageSize === _pageSize &&
            this.nameFilter === _nameFilter &&
            this.gender === _gender &&
            this.minAge === _minAge &&
            this.maxAge === _maxAge &&
            this.minDaysAfterLastCall === _minDaysAfterLastCall;
    },
    updateFilters: function () {
        appFilters.pageSize = parseInt($('#iPerPage').val());
        appFilters.nameFilter = $('#nameFilter').val();
        appFilters.gender = parseInt($('#gender').val());
        appFilters.minAge = parseInt($('#minAge').val()) || 0;
        appFilters.maxAge = parseInt($('#maxAge').val()) || 0;
        appFilters.minDaysAfterLastCall = parseInt($('#minDaysAfterLastCall').val()) || 0;
    },
    getFiltersParamString: function () {
        let parameters = '?';
        parameters += 'PageNo=' + this.pageNo + '&PageSize=' + this.pageSize;

        if (this.nameFilter) {
            parameters += '&NameFilter=' + encodeURIComponent(this.nameFilter);
        }
        if (this.gender !== 0) {
            parameters += '&Gender=' + this.gender;
        }
        if (this.minAge !== 0) {
            parameters += '&MinAge=' + this.minAge;
        }
        if (this.maxAge !== 0) {
            parameters += '&MaxAge=' + this.maxAge;
        }
        if (this.minDaysAfterLastCall !== 0) {
            parameters += '&MinDaysAfterLastCall=' + this.minDaysAfterLastCall;
        }
        return parameters;
    }
};

let appState = {
    currentPersonId: null,
    currentCallId: null,
    countOfRecords: 0,
    getPagesCount: function (pageSize) {
        return this.countOfRecords <= pageSize ? 1 : this.countOfRecords % pageSize === 0 ? Math.floor(this.countOfRecords / pageSize) : Math.floor(this.countOfRecords / pageSize) + 1;
    }
};

let UIMap = {
    personDetailTable: $('#tableUserDetail'),
    personCallsTable: $('#tableCallsHistory tbody'),
    personsListTable: $('#tableUsersList tbody'),
    pagesLabel: $('#pageLabel'),
    personsCountLabel: $('#personsCountLabel')
};

let appQuery = {
    loadPersonsDataList: function (callback) {
        $.ajax({
            url: '/api/persons' + appFilters.getFiltersParamString(),
            type: 'GET',
            dataType: 'json',
            success: function (persons) {
                callback(persons);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    },
    updateRecordsCount: function (callback) {
        $.ajax({
            url: '/api/persons/count' + appFilters.getFiltersParamString(),
            type: 'GET',
            dataType: 'json',
            success: function (count) {
                callback(count);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    },
    getPersonData: function (selPersonId, callback) {
        $.ajax({
            url: '/api/persons/' + selPersonId,
            type: 'GET',
            dataType: 'json',
            success: function (person) {
                callback(person);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    },
    getPersonColls: function (personId, callback) {
        $.ajax({
            url: '/api/persons/' + personId + '/calls',
            type: 'GET',
            dataType: 'json',
            success: function (calls) {
                callback(calls);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    },
    deletePerson: function (personId, callback) {
        $.ajax({
            url: '/api/persons/' + personId,
            type: "DELETE",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                callback(data);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    }
}

let app = {
    updatePagesLabel: function () {
        UIMap.pagesLabel.text('[' + appFilters.pageNo + ' стр. из ' + appState.getPagesCount(appFilters.pageSize) + ']');
        $('#iPerPage').val(appFilters.pageSize);
    },
    loadPersonsList: function () {
        $('#loadingMsg').show();
        appState.currentPersonId = '';

        if (!appFilters.isFiltersNotChanged()) {
            appFilters.updateFilters();
        }
        appQuery.updateRecordsCount(function (count) {
            appState.countOfRecords = count;
            UIMap.personsCountLabel.text('Всего найдено: ' + count);
        });
        appQuery.loadPersonsDataList(function (persons) {
            UIMap.personsListTable.empty();
            $.each(persons, function (index, person) {
                let row = '<tr id="' + person.id + '" onclick="app.selPerson(this);"> <td>' + (person.lastName === null ? '' : person.lastName) +
                    '</td><td>' + person.firstName +
                    '</td><td>' + (person.patronymic === null ? '' : person.patronymic) + '</td></tr>';
                UIMap.personsListTable.append(row);
            });
            app.updatePagesLabel();
            $('#loadingMsg').hide();
        });
    },
    deletePersonClick: function () {
        let lst = $('.table-info');
        if (lst.length === 0) {
            alert('Ничего не выделено');
            return;
        }
        if (!confirm('Удалить данные о клиенте?')) return;
        let selPersonId = $(lst[0]).prop('id');

        appQuery.deletePerson(selPersonId, function () {
            $('#tableCallsHistory tbody').empty();
            app.loadPersonsList();
        });

    },
    pageSizeChange: function (elem) {
        appFilters.pageSize = elem.value;
        appFilters.pageNo = 1;
        app.loadPersonsList();
    },
    nextPage: function () {
        if (appFilters.pageNo >= appState.getPagesCount(appFilters.pageSize)) return;
        appFilters.pageNo++;
        app.loadPersonsList();
    },
    prevPage: function () {

        if (appFilters.pageNo === 1) return;
        appFilters.pageNo--;
        app.loadPersonsList();
    },
    selPerson: function (el) {
        $.each($('.table-info'), function (ndex, elem) { $(elem).removeClass('table-info'); });
        $(el).addClass('table-info');

        let selPersonId = $(el).prop('id');
        appState.currentPersonId = selPersonId;
        appState.currentCallId = '';

        appQuery.getPersonData(selPersonId, function (person) {
            let ln = person.lastName === null ? "не указано" : person.lastName;
            let pt = person.patronymic === null ? "не указано" : person.patronymic;
            let ger = person.gender === 1 ? "М" : person.gender === 2 ? "Ж" : "не указано";

            let bd = "не указано";
            if (person.birthDate !== null) {
                bd = person.birthDate.split('T')[0];
            }

            $('#detSurname').text(ln);
            $('#detName').text(person.firstName);
            $('#detPatronymic').text(pt);
            $('#detGender').text(ger);
            $('#detBirthDate').text(bd);
            $('#detPhone').text(person.phoneNumber);
        });

        app.updateCalls(selPersonId);
    },
    updateCalls: function (personId) {

        UIMap.personCallsTable.empty();
        appQuery.getPersonColls(personId, function (calls) {
            $.each(calls, function (index, call) {
                let d = call.callDate.split('T');
                let co = call.orderCost === null ? 0 : call.orderCost;

                let str = "<tr><td>" + d[0]/*.replace(/-/g, '.') */+
                    "</td><td>" + call.callReport +
                    "</td><td>" + co +
                    "</td><td><button type='button' class='btn btn-default btn-sm' data-toggle='modal' data-target='#editCall' id='" + call.callId + "' onclick='app.editCallClick(this)' >Изменить</button></td><td>" +
                    "<button type='button' class='btn btn-danger btn-sm' id='" + call.callId + "' onclick='app.deleteCallClick(this)' >Удалить</button></td></tr>";

                UIMap.personCallsTable.append(str);
            });
        });
    },
    addPersonClick: function () {
        app.clearForm();
    },
    clearForm: function () {
        document.getElementById("frmEditPerson").reset();
        $('#cMessage').hide();
        $('#cValidation').hide();
        $('#currentPersonId').val('');
    },
    savePerson: function () {
        if (!app.formIsValid()) {
            $('#cValidation').show();
            return;
        }
        $('#cValidation').hide();

        let firstName = $('#cName').val();
        if (!firstName) firstName = null;
        let lastName = $('#cSurname').val();
        if (!lastName) lastName = null;
        let patronymic = $('#cPatronymic').val();
        if (!patronymic) patronymic = null;
        let birthDate = $('#cBirthDate').val();
        let gender = $('#cGender').val();
        let phoneNumber = $('#cPhoneNumber').val();

        let personId = $('#currentPersonId').val();
        if (personId) {
            $.ajax({
                url: '/api/persons',
                type: "PUT",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    Id: personId,
                    FirstName: firstName,
                    LastName: lastName,
                    Patronymic: patronymic,
                    BirthDate: birthDate,
                    PhoneNumber: phoneNumber,
                    Gender: gender
                }),
                success: function (data) {
                    $('#cMessage').show();
                    app.loadPersonsList();
                },
                error: function (x, y, z) {
                    alert(x + '\n' + y + '\n' + z);
                }
            });
        }
        else {
            $.ajax({
                url: '/api/persons',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    FirstName: firstName,
                    LastName: lastName,
                    Patronymic: patronymic,
                    BirthDate: birthDate,
                    PhoneNumber: phoneNumber,
                    Gender: gender
                }),
                success: function (data) {
                    $('#cMessage').show();
                    app.loadPersonsList();
                },
                error: function (x, y, z) {
                    alert(x + '\n' + y + '\n' + z);
                }
            });
        }
    },
    formIsValid: function () {
        let cName = $('#cName').val();
        let cBirthDate = $('#cBirthDate').val();
        let cPhone = $('#cPhoneNumber').val();

        let reqs = cName && cBirthDate && cPhone;
        let lens = cName.length >= 3 &&
            cName.length <= 30 &&
            (cPhone.length >= 5 && cPhone.length <= 20) &&
            $('#cSurname').val().length <= 30 && $('#cPatronymic').val().length <= 30;

        return reqs && lens;
    },
    editPersonClick: function () {
        $('#cMessage').hide();
        $('#cValidation').hide();
        if (!appState.currentPersonId) {
            alert('Запись не выбрана. При сохранении будет создана новая');
            $('#btnSavePerson').prop('disabled', true);
            return;
        }
        $('#btnSavePerson').prop('disabled', false);
        $('#cName').val($('#detName').text());
        $('#cSurname').val($('#detSurname').text());
        $('#cPatronymic').val($('#detPatronymic').text());
        $('#cBirthDate').val($('#detBirthDate').text());
        $('#cGender').val($('#detGender').text() === 'не указано' ? 0 : $('#detGender').text() === 'М' ? 1 : 2);
        $('#cPhoneNumber').val($('#detPhone').text());
    },
    callFormIsValid: function () {
        let cDate = $('#crCallDate').val();
        let cReport = $('#crCallReport').val();
        let cOrderCost = $('#crOrderCost').val();
        let reqs = cReport && cDate && appState.currentPersonId;
        let lens = cReport.length <= 500 && cReport.length >= 5;
        return reqs && lens;
    },
    clearCallForm: function () {
        document.getElementById("frmEditCall").reset();
        $('#crMessage').hide();
        $('#crValidation').hide();
        $('#btnSaveCall').prop('disabled', false);
    },
    addCallClick: function (elem) {
        if (!appState.currentPersonId) {
            alert('Запись не выбрана. Сохранение не возможно');    
            $('#btnSaveCall').prop('disabled', true);
            return;
        }
        app.clearCallForm();
    },
    deleteCallClick: function (btn) {
        if (!confirm('Удалить отчет о звонке?')) {
            return;
        }

        let personId = appState.currentPersonId;
        let callId = $(btn).attr('id');
        $('#currentCallId').val('');
        
        $.ajax({
            url: '/api/persons/' + personId + '/calls/' + callId,
            type: "DELETE",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                app.updateCalls(personId);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    },
    editCallClick: function (btn) {
        let personId = appState.currentPersonId;
        let callId = $(btn).attr('id');
        appState.currentCallId = callId;

        let rowList = $(btn).parent().parent().children();
        let cDate = $(rowList[0]).text();
        let cCallReport = $(rowList[1]).text();
        let cOrderCost = $(rowList[2]).text();
        
        $('#crCallDate').val(cDate);
        $('#crCallReport').text(cCallReport);
        $('#crOrderCost').val(cOrderCost);
        $('#crMessage').hide();
        $('#crValidation').hide();
    },
    saveCall: function () {
        if (!app.callFormIsValid()) {
            $('#crValidation').show();
            return;
        }
        $('#crValidation').hide();

        let cDate = $('#crCallDate').val();
        let cReport = $('#crCallReport').val();
        let cOrderCost = $('#crOrderCost').val();
        cOrderCost = isNaN(parseFloat(cOrderCost)) ? 0 : parseFloat(cOrderCost);

        let personId = appState.currentPersonId;
        let callId = appState.currentCallId;

        if (callId) {
            $.ajax({
                url: '/api/persons/' + personId + '/calls',
                type: "PUT",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    CallId: callId,
                    PersonId: personId,
                    CallReport: cReport,
                    CallDate: cDate,
                    OrderCost: cOrderCost
                }),
                success: function (data) {
                    $('#crMessage').show();
                    app.updateCalls(personId);
                    app.currentCallId = '';
                },
                error: function (x, y, z) {
                    alert(x + '\n' + y + '\n' + z);
                }
            });
        }
        else {
            $.ajax({
                url: '/api/persons/' + personId + '/calls',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    PersonId: personId,
                    CallReport: cReport,
                    CallDate: cDate,
                    OrderCost: cOrderCost
                }),
                success: function (data) {
                    $('#crMessage').show();
                    app.updateCalls(personId);
                    app.currentCallId = '';
                },
                error: function (x, y, z) {
                    alert(x + '\n' + y + '\n' + z);
                }
            });
        }
    }
}

function createTestData() {
    $.ajax({
        url: '/api/createtestdata',
        type: 'POST',
        dataType: 'text',
        success: function () {
            location.reload();
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

$(document).ready(function () {
    app.loadPersonsList();    
});