using FluentValidation;
using MedicalAir.ViewModel.General;

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
                .Must(BeValidFullName).WithMessage("ФИО должно содержать только буквы и иметь формат: Фамилия Имя Отчество (3 слова, разделенные 2 пробелами)");

            RuleFor(x => x.UserRoles)
                .NotNull().WithMessage("Выберите роль")
                .IsInEnum().WithMessage("Недопустимая роль");
        }

        private bool BeValidFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return false;

            var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length != 3)
                return false;

            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                    return false;

                foreach (var ch in word)
                {
                    if (!char.IsLetter(ch))
                        return false;
                }
            }

            var trimmedName = fullName.Trim();
            var parts = trimmedName.Split(' ');
            if (parts.Length != 3)
                return false;

            return true;
        }
    }
}
