using FluentValidation;

namespace CallCenter.Back.Data
{
    public class PersonsFilterFieldsValidator : AbstractValidator<PersonsFilterFields>
    {
        public PersonsFilterFieldsValidator()
        {
            RuleFor(f => f.Gender).IsInEnum().WithMessage("Некорректное числовое значение Gender");
            RuleFor(f => f.MaxAge).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MaxAge");
            RuleFor(f => f.MinAge).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MinAge");
            //RuleFor(f => f.MinAge).LessThanOrEqualTo(f => f.MaxAge).WithMessage("MinAge не должен быть больше MaxAge");
            RuleFor(f => f.MinDaysAfterLastCall).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MinDaysAfterLastCall");
            RuleFor(f => f.PageNo).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение номера страницы");
            RuleFor(f => f.PageSize).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение количества записей для страницы");            
        }
    }

    public class CallValidator : AbstractValidator<Call>
    {
        public CallValidator()
        {
            RuleFor(c => c.OrderCost).GreaterThanOrEqualTo(0);
            RuleFor(c => c.CallReport).Length(5, 500).WithMessage("Длина строки должна быть от 5 до 500 символов");
        }
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(p => p.FirstName).NotNull().WithMessage("Обязательное поле");
            RuleFor(p => p.FirstName).Length(3, 30).WithMessage("Должно быть от 3 до 30 символов");
            RuleFor(p => p.PhoneNumber).NotNull().WithMessage("Обязательное поле");
            RuleFor(p => p.PhoneNumber).Length(5, 20).WithMessage("Должно быть от 5 до 20 символов");
            RuleFor(p => p.LastName).Length(0, 30).WithMessage("Не должно быть больше 30 символов");
            RuleFor(p => p.Patronymic).Length(0, 30).WithMessage("Не должно быть больше 30 символов");
            RuleFor(p => p.Gender).IsInEnum();
        }
    }
}
