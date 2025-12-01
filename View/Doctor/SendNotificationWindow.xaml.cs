using MedicalAir.Config;
using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.WindowManager;
using MedicalAir.View.General;
using MedicalAir.ViewModel.Doctor;
using System.Windows;

namespace MedicalAir.View.Doctor
{
    public partial class SendNotificationWindow : Window
    {
        public SendNotificationWindow()
        {
            InitializeComponent();
            var dbContext = DbContextFactory.Create();
            DataContext = new SendNotificationViewModel(new UnitOfWork(dbContext));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MainDoctorWindow());
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowAndCloseCurrent(new MainDoctorWindow());
        }
    }
}

