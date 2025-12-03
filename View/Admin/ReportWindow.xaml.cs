using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Admin;

namespace MedicalAir.View.Admin
{
    
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            
            var dbContext = DbContextFactory.Create();
            var unitOfWork = new UnitOfWork(dbContext);
            var viewModel = new ReportViewModel(unitOfWork);
            DataContext = viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT;
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MainAdminWindow());
        }
    }
}
