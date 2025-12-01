using System.Linq;
using System.Windows;
using MedicalAir.Config;
using MedicalAir.DataBase;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.Model.Session;
using MedicalAir.View.General;
using MedicalAir.ViewModel.FlightAttendant;

namespace MedicalAir.View.FlightAttendant
{
    /// <summary>
    /// Логика взаимодействия для NotificationFaWindow.xaml
    /// </summary>
    public partial class NotificationFaWindow : Window
    {
        public NotificationFaWindow()
        {
            InitializeComponent();
            
            var dbContext = DbContextFactory.Create();
            var unitOfWork = new UnitOfWork(dbContext);
            var viewModel = new NotificationFaViewModel(unitOfWork);
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

        private void MedicamentsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MedicamentsFaWindow());
        }

        private void ProcedureButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new ProcedureFaWindow());
        }
    }
}
