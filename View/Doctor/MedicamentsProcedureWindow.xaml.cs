using System.Windows;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.Doctor
{
    
    public partial class MedicamentsProcedureWindow : Window
    {
        public MedicamentsProcedureWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.Doctor.MedicamentsProcedureViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
        }

        private void ProcedureDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.Doctor.MedicamentsProcedureViewModel vm && vm.SelectedProcedure != null)
            {
                vm.ProcedureName = vm.SelectedProcedure.Name;
                vm.ProcedureDescription = vm.SelectedProcedure.Description ?? "";
                vm.MinValue = vm.SelectedProcedure.MinValue;
                vm.MaxValue = vm.SelectedProcedure.MaxValue;
                vm.Units = vm.SelectedProcedure.Units ?? "";
                vm.MustBeTrue = vm.SelectedProcedure.MustBeTrue;
            }
        }

        private void MedicinsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.Doctor.MedicamentsProcedureViewModel vm && vm.SelectedMedicin != null)
            {
                vm.MedicinName = vm.SelectedMedicin.Name ?? "";
                vm.MedicinComposition = vm.SelectedMedicin.Composition ?? "";
            }
        }

        private void HistoryDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.Doctor.MedicamentsProcedureViewModel vm && vm.SelectedHistoryUpMedicin != null)
            {
                vm.Count = vm.SelectedHistoryUpMedicin.Count;
                vm.UpData = vm.SelectedHistoryUpMedicin.UpData.ToDateTime(TimeOnly.MinValue);
                vm.EndData = vm.SelectedHistoryUpMedicin.EndData.ToDateTime(TimeOnly.MinValue);
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

        private void MedicalExaminationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MedicalExaminationWindow());
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
