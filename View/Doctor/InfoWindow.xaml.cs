using System.Windows;
using System.Windows.Controls;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.Doctor
{
    
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.Doctor.InfoViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT;
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MainDoctorWindow());
        }

        private void CertificatButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new CertificatWindow());
        }

        private void MedicalExaminationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MedicalExaminationWindow());
        }

        private void MedicamentsProcedureButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MedicamentsProcedureWindow());
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ReportWindow());
        }

        private void SendNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new SendNotificationWindow());
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is MedicalExamination examination && DataContext is ViewModel.Doctor.InfoViewModel vm)
            {
                e.Row.Tag = vm.GetProceduresForExamination(examination);
            }
        }
    }
}
