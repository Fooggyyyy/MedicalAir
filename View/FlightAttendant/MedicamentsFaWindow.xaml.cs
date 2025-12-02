using System.Linq;
using System.Windows;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.FlightAttendant
{
    /// <summary>
    /// Логика взаимодействия для MedicamentsFaWindow.xaml
    /// </summary>
    public partial class MedicamentsFaWindow : Window
    {
        public MedicamentsFaWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.FlightAttendant.MedicamentsFaViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
        }

        private void MedkitDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Обновление данных при выборе аптечки происходит автоматически через привязку
        }

        private void MedicinsInMedkitDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.FlightAttendant.MedicamentsFaViewModel vm && sender is System.Windows.Controls.DataGrid dg)
            {
                vm.SelectedMedicin = dg.SelectedItem as MedicalAir.Model.Entites.Medicin;
            }
        }

        private void AvailableMedicinsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.FlightAttendant.MedicamentsFaViewModel vm && sender is System.Windows.Controls.DataGrid dg)
            {
                vm.SelectedMedicin = dg.SelectedItem as MedicalAir.Model.Entites.Medicin;
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
            WindowManager.ShowAndCloseCurrent(new MainFaWindow());
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new InfoFaWindow());
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new RegFaWindow());
        }

        private void ProcedureButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ProcedureFaWindow());
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new NotificationFaWindow());
        }
    }
}
