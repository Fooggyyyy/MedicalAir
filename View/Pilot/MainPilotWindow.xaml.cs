using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Pilot;

namespace MedicalAir.View.Pilot
{
    
    public partial class MainPilotWindow : Window
    {
        public MainPilotWindow()
        {
            InitializeComponent();
            
            var dbContext = DbContextFactory.Create();
            var unitOfWork = new UnitOfWork(dbContext);
            var viewModel = new MainPilotViewModel(unitOfWork);
            DataContext = viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT; 
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new InfoPilotWindow());
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
    }
}
