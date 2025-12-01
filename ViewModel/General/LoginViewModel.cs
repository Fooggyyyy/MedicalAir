using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.HashPassword;
using MedicalAir.Helper.Validators.General;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Enums;
using MedicalAir.Model.Session;
using MedicalAir.View.Admin;
using MedicalAir.View.Doctor;
using MedicalAir.View.FlightAttendant;
using MedicalAir.View.Pilot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicalAir.ViewModel.General
{
    public class LoginViewModel : ViewModelBase<LoginViewModel>
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

        public ICommand LoginCommand { get; set; }

        public LoginViewModel(IUnitOfWork unitOfWork, IHashPassword hashPassword)
        {
            this.unitOfWork = unitOfWork;
            this.hashPassword = hashPassword;
            Validator = new LoginValidator();
            LoginCommand = CreateAsyncCommand(LoginAsync, CanLogin);
        }

        private bool CanLogin()
        => !HasErrors &&
           !string.IsNullOrWhiteSpace(Email) &&
           !string.IsNullOrWhiteSpace(Password);

        public async Task LoginAsync()
        {
            var result = Validator.Validate(this);
            if (!result.IsValid)
            {
                foreach (var err in result.Errors)
                    ValidateProperty(err.PropertyName);
                return;
            }

            var user = await unitOfWork.UserRepository.GetByEmailAsync(Email);
            if (user == null)
            {
                ModernMessageDialog.Show("Пользователь с почтой " + Email + " не найден", "Ошибка", MessageType.Error);
                return;
            }

            // Проверка на блокировку
            if (user.IsBlocked)
            {
                ModernMessageDialog.Show("Ваш аккаунт заблокирован. Обратитесь к администратору.", "Аккаунт заблокирован", MessageType.Error);
                return;
            }

            if (hashPassword.Verify(Password, user.HashPassword))
            {
                ModernMessageDialog.Show("Добро пожаловать " + user.FullName + "!!!", "Успешный вход", MessageType.Success);
                Session.UserId = user.Id;
                Session.UserRole = user.Roles;

                // Навигация в зависимости от роли пользователя
                NavigateToRoleWindow(user.Roles);
            }
            else
            {
                ModernMessageDialog.Show("Пароль не верный", "Ошибка", MessageType.Error);
                return;
            }
        }

        private void NavigateToRoleWindow(UserRoles role)
        {
            Window targetWindow = null;

            switch (role)
            {
                case UserRoles.ADMIN:
                    targetWindow = new MainAdminWindow();
                    break;
                case UserRoles.DOCTOR:
                    targetWindow = new MainDoctorWindow();
                    break;
                case UserRoles.PILOT:
                    targetWindow = new MainPilotWindow();
                    break;
                case UserRoles.FLIGHTATTENDAT:
                    targetWindow = new MainFaWindow();
                    break;
                default:
                    ModernMessageDialog.Show("Неизвестная роль пользователя", "Ошибка", MessageType.Error);
                    return;
            }

            if (targetWindow != null)
            {
                // Сначала показываем новое окно, затем закрываем старые
                targetWindow.Show();
                
                // Закрываем все окна приложения после открытия главного окна роли
                var windowsToClose = Application.Current.Windows.OfType<Window>()
                    .Where(w => w != targetWindow && w.IsLoaded)
                    .ToList();
                foreach (var window in windowsToClose)
                {
                    window.Close();
                }
            }
        }
    }
}
