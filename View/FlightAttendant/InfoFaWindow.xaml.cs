using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.FlightAttendant
{
    /// <summary>
    /// Логика взаимодействия для InfoFaWindow.xaml
    /// </summary>
    public partial class InfoFaWindow : Window
    {
        public InfoFaWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.FlightAttendant.InfoFaViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
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

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new RegFaWindow());
        }

        private void MedicamentsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MedicamentsFaWindow());
        }

        private void ProcedureButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ProcedureFaWindow());
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new NotificationFaWindow());
        }

        private void MedicalExaminationsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is MedicalExamination examination && DataContext is ViewModel.FlightAttendant.InfoFaViewModel vm)
            {
                var procedures = vm.GetProceduresForExamination(examination);
                e.Row.Tag = procedures;
                
                // Обновляем Tag при изменении данных
                e.Row.DataContextChanged += (s, args) =>
                {
                    if (e.Row.Item is MedicalExamination exam)
                    {
                        e.Row.Tag = vm.GetProceduresForExamination(exam);
                    }
                };
            }
        }
    }
}
