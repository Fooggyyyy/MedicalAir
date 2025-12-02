using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Doctor;

namespace MedicalAir.View.Doctor
{
    /// <summary>
    /// Логика взаимодействия для CertificatWindow.xaml
    /// </summary>
    public partial class CertificatWindow : Window
    {
        public CertificatWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new CertificatViewModel(new UnitOfWork(dbContext));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CertificatViewModel vm && vm.SelectedCertificat != null)
            {
                vm.DataStart = vm.SelectedCertificat.DataStart.ToDateTime(TimeOnly.MinValue);
                vm.DataEnd = vm.SelectedCertificat.DataEnd.ToDateTime(TimeOnly.MinValue);
            }
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

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new InfoWindow());
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
