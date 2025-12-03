using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.HashPassword;
using MedicalAir.Helper.Validators.General;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Enums;
using System.Windows.Input;

namespace MedicalAir.ViewModel.General
{
    public class RegisterViewModel : ViewModelBase<RegisterViewModel>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashPassword hashPassword;

        private string email;
        public string Email
        {
            get => email;
            set => Set(ref email, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        private string fullName;
        public string FullName
        {
            get => fullName;
            set => Set(ref fullName, value);
        }

        private UserRoles userRoles;
        public UserRoles UserRoles
        {
            get => userRoles;
            set => Set(ref userRoles, value);
        }

        public ICommand RegisterCommand { get; }
        public RegisterViewModel(IUnitOfWork unitOfWork, IHashPassword hashPassword)
        {
            this.unitOfWork = unitOfWork;
            this.hashPassword = hashPassword;
            Validator = new RegisterValidator(); 
            RegisterCommand = CreateAsyncCommand(RegisterAsync, CanRegister);
        }

        private bool CanRegister()
        => !HasErrors &&
           !string.IsNullOrWhiteSpace(Email) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !string.IsNullOrWhiteSpace(FullName);

        private async Task RegisterAsync()
        {
            var result = Validator.Validate(this);
            if (!result.IsValid)
            {
                foreach (var err in result.Errors)
                    ValidateProperty(err.PropertyName); 
                return; 
            }

            var existingUser = await unitOfWork.UserRepository.GetByEmailAsync(Email);
            if (existingUser != null)
            {
                ModernMessageDialog.Show("Пользователь с данным email уже существует", "Ошибка", MessageType.Error);
                return;
            }

            var HashPassword = hashPassword.HashPassword(Password);
            await unitOfWork.UserRepository.AddAsync(new Model.Entites.User(FullName, Email, HashPassword, UserRoles));
            await unitOfWork.SaveAsync();

            ModernMessageDialog.Show("Пользователь " + FullName + " успешно зарегистрирован", "Успех", MessageType.Success);
        }
    }
}
