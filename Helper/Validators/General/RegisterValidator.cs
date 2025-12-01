using FluentValidation;
using MedicalAir.ViewModel.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Helper.Validators.General
{
    public class RegisterValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Неверный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Минимум 6 символов");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Введите ваше имя")
                .MinimumLength(5).WithMessage("Имя слишком короткое");

            RuleFor(x => x.UserRoles)
                .NotNull().WithMessage("Выберите роль")
                .IsInEnum().WithMessage("Недопустимая роль");
        }
    }
}
