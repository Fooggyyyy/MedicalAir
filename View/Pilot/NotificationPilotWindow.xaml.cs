using System.Linq;
using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Pilot;

namespace MedicalAir.View.Pilot
{
    /// <summary>
    /// Логика взаимодействия для NotificationPilotWindow.xaml
    /// </summary>
    public partial class NotificationPilotWindow : Window
    {
        public NotificationPilotWindow()
        {
            InitializeComponent();
            
            var dbContext = DbContextFactory.Create();
            var unitOfWork = new UnitOfWork(dbContext);
            var viewModel = new NotificationPilotViewModel(unitOfWork);
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
            WindowManager.ShowAndCloseCurrent(new MainPilotWindow());
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
    }
}
