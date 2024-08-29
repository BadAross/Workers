using FluentValidation;
using Workers.DataAccess.Dto.Requests;

namespace Workers.DataAccess.Validators.Worker;

/// <summary>
/// Валидация изменения данных сотрудника
/// </summary>
internal sealed class UpdateWorkerValidator : AbstractValidator<UpdateWorkerRequest>
{
    public UpdateWorkerValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .OverridePropertyName("Имя сотрудника")
            .WithMessage("Поле 'Имя сотрудника'  должно быть заполнено")
            .MaximumLength(100)
            .OverridePropertyName("Имя сотрудника")
            .WithMessage("Превышено допустимое значение символов в поле 'Имя сотрудника'");
        
        RuleFor(r => r.Surname)
            .NotEmpty()
            .OverridePropertyName("Фамилия сотрудника")
            .WithMessage("Поле 'Фамилия сотрудника'  должно быть заполнено")
            .MaximumLength(100)
            .OverridePropertyName("Фамилия сотрудника")
            .WithMessage("Превышено допустимое значение символов в поле 'Фамилия сотрудника'");
        
        RuleFor(r => r.Phone)
            .MaximumLength(16)
            .OverridePropertyName("Номер телефона сотрудника")
            .WithMessage("Превышено допустимое значение символов в поле 'Номер телефона сотрудника'");
        
        RuleFor(r => r.Passport.Number)
            .NotEmpty()
            .OverridePropertyName("Номер паспорта")
            .WithMessage("Поле 'Номер паспорта'  должно быть заполнено")
            .MaximumLength(50)
            .OverridePropertyName("Номер паспорта")
            .WithMessage("Превышено допустимое значение символов в поле 'Номер паспорта'");
    }
}