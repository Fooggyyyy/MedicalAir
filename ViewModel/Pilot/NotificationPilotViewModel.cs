using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Pilot
{
    public class NotificationPilotViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<Notification> notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => notifications;
            set => Set(ref notifications, value);
        }

        private Notification selectedNotification;
        public Notification SelectedNotification
        {
            get => selectedNotification;
            set => Set(ref selectedNotification, value);
        }

        public ICommand LoadNotifications { get; set; }
        public ICommand RefreshNotifications { get; set; }

        public NotificationPilotViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Notifications = new ObservableCollection<Notification>();

            LoadNotifications = CreateAsyncCommand(LoadNotificationsAsync);
            RefreshNotifications = CreateAsyncCommand(LoadNotificationsAsync);

            LoadNotificationsAsync();
        }

        private async Task LoadNotificationsAsync()
        {
            try
            {
                if (Session.UserId == 0)
                {
                    ModernMessageDialog.Show("Пользователь не авторизован", "Ошибка", MessageType.Error);
                    return;
                }

                var notificationsList = await _unitOfWork.NotificationRepository.GetByUserIdAsync(Session.UserId);
                Notifications = new ObservableCollection<Notification>(notificationsList.OrderByDescending(n => n.Id));

                if (!Notifications.Any())
                {
                    // Не показываем сообщение, просто пустой список
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке уведомлений: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
