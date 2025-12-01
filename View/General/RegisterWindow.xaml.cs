using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.HashPassword;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Enums;
using MedicalAir.ViewModel.General;

namespace MedicalAir.View.General
{
    /// <summary>
    /// Класс для отображения ролей с красивыми названиями
    /// </summary>
    public class UserRoleDisplay
    {
        public UserRoles Value { get; set; }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new RegisterViewModel(new UnitOfWork(dbContext), new HashPasswordService());
            
            // Создаем список ролей с красивыми названиями
            var roles = new List<UserRoleDisplay>
            {
                new UserRoleDisplay { Value = UserRoles.PILOT, DisplayName = "Пилот" },
                new UserRoleDisplay { Value = UserRoles.DOCTOR, DisplayName = "Врач" },
                new UserRoleDisplay { Value = UserRoles.FLIGHTATTENDAT, DisplayName = "Бортпроводник" },
               //new UserRoleDisplay { Value = UserRoles.ADMIN, DisplayName = "Администратор" }
            };
            
            UserRolesComboBox.ItemsSource = roles;
            UserRolesComboBox.DisplayMemberPath = "DisplayName";
            UserRolesComboBox.SelectedValuePath = "Value";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        private void LoginLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }
    }
}
