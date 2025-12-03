using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.HashPassword;
using MedicalAir.Helper.WindowManager;
using MedicalAir.ViewModel.General;

namespace MedicalAir.View.General
{
    
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new LoginViewModel(new UnitOfWork(dbContext), new HashPasswordService());
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        private void RegisterLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new RegisterWindow());
        }
    }
}
