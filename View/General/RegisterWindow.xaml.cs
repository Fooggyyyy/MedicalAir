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
    
    public class UserRoleDisplay
    {
        public UserRoles Value { get; set; }
        public string DisplayName { get; set; }
    }

    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new RegisterViewModel(new UnitOfWork(dbContext), new HashPasswordService());
            
            var roles = new List<UserRoleDisplay>
            {
                new UserRoleDisplay { Value = UserRoles.PILOT, DisplayName = "Пилот" },
                new UserRoleDisplay { Value = UserRoles.DOCTOR, DisplayName = "Врач" },
                new UserRoleDisplay { Value = UserRoles.FLIGHTATTENDAT, DisplayName = "Бортпроводник" },
               
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

        private void FullNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[\p{L}\s]+$");
        }
    }
}
