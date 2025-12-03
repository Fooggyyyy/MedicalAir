using System.Windows;
using System.Windows.Controls;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.Doctor
{
    
    public partial class MedicalExaminationWindow : Window
    {
        public MedicalExaminationWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.Doctor.MedicalExaminationViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.Doctor.MedicalExaminationViewModel vm && vm.SelectedMedicalExamination != null)
            {
                vm.DataStart = vm.SelectedMedicalExamination.DataStart.ToDateTime(TimeOnly.MinValue);
                vm.DataEnd = vm.SelectedMedicalExamination.DataEnd.ToDateTime(TimeOnly.MinValue);
                vm.Message = vm.SelectedMedicalExamination.Message;
            }
        }

        private void MedicalExaminationsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is MedicalExamination examination && DataContext is ViewModel.Doctor.MedicalExaminationViewModel vm)
            {
                e.Row.Tag = vm.GetProceduresForExamination(examination);
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

        private void CertificatButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new CertificatWindow());
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
