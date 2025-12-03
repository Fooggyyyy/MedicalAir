using System.Windows;
using System.Windows.Controls;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using MedicalAir.View.General;

namespace MedicalAir.View.Pilot
{
    
    public partial class InfoPilotWindow : Window
    {
        public InfoPilotWindow()
        {
            InitializeComponent();
            var dbContext = Config.DbContextFactory.Create();
            DataContext = new ViewModel.Pilot.InfoPilotViewModel(new DataBase.UnitOfWork.UnitOfWork(dbContext));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT;
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MainPilotWindow());
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new RegPilotWindow());
        }

        private void ProcedureButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ProcedurePilotWindow());
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new NotificationPilotWindow());
        }

        private void MedicalExaminationsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is MedicalExamination examination && DataContext is ViewModel.Pilot.InfoPilotViewModel vm)
            {
                var procedures = vm.GetProceduresForExamination(examination);
                e.Row.Tag = procedures;
                
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
