using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Admin;

namespace MedicalAir.View.Admin
{
    
    public partial class MainAdminWindow : Window
    {
        public MainAdminWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new MainAdminViewModel(new UnitOfWork(dbContext));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT; 
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ReportWindow());
        }
    }
}
