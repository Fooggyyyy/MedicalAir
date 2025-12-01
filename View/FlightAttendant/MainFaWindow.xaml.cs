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
    /// Логика взаимодействия для MainFaWindow.xaml
    /// </summary>
    public partial class MainFaWindow : Window
    {
        public MainFaWindow()
        {
            InitializeComponent();
            
            var dbContext = DbContextFactory.Create();
            var unitOfWork = new UnitOfWork(dbContext);
            var viewModel = new MainFaViewModel(unitOfWork);
            DataContext = viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.UserId = 0;
            Session.UserRole = Model.Enums.UserRoles.FLIGHTATTENDAT; // Сброс роли
            WindowManager.ShowAndCloseCurrent(new LoginWindow());
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

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new NotificationFaWindow());
        }
    }
}
