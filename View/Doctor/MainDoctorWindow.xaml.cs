using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Doctor;

namespace MedicalAir.View.Doctor
{
    
    public partial class MainDoctorWindow : Window
    {
        public MainDoctorWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new MainDoctorViewModel(new UnitOfWork(dbContext));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT; 
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new InfoWindow());
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
    }
}
