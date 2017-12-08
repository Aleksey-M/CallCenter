import { Injectable } from '@angular/core';

@Injectable()
export class ApiService {

  constructor() { }

}

export class Person {
  constructor(
    personId?: string,
    firstName?: string,
    lastName?: string,
    patronymic?: string,
    birthDate?: Date,
    gender?: Number,
    phoneNumber?: string
  ) { }
}

export class PersonListItem {
  constructor(
    id?: string,
    firstName?: string,
    lastName?: string,
    patronymic?: string
  ) { }
}

export class Call {
  constructor(
    callId?: string,
    callDate?: Date,
    orderCost?: Number,
    callReport?: string,
    personId?: string
  ) { }
}

export class PersonsFilterFields {
  constructor(
    pageNo?: Number,
    pageSize?: Number,
    gender?: Number,
    nameFilter?: string,
    maxAge?: Number,
    minAge?: Number,
    minDaysAfterLastCall?: Number
  ) { }
}
