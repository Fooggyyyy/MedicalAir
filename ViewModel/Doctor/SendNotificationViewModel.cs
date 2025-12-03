using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class SendNotificationViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<User> _pilots;
        public ObservableCollection<User> Pilots
        {
            get => _pilots;
            set => Set(ref _pilots, value);
        }

        private ObservableCollection<User> _flightAttendants;
        public ObservableCollection<User> FlightAttendants
        {
            get => _flightAttendants;
            set => Set(ref _flightAttendants, value);
        }

        private User? _selectedPilot;
        public User? SelectedPilot
        {
            get => _selectedPilot;
            set => Set(ref _selectedPilot, value);
        }

        private User? _selectedFlightAttendant;
        public User? SelectedFlightAttendant
        {
            get => _selectedFlightAttendant;
            set => Set(ref _selectedFlightAttendant, value);
        }

        private string _messageBody;
        public string MessageBody
        {
            get => _messageBody;
            set => Set(ref _messageBody, value);
        }

        private bool _sendToAllPilots;
        public bool SendToAllPilots
        {
            get => _sendToAllPilots;
            set => Set(ref _sendToAllPilots, value);
        }

        private bool _sendToAllFlightAttendants;
        public bool SendToAllFlightAttendants
        {
            get => _sendToAllFlightAttendants;
            set => Set(ref _sendToAllFlightAttendants, value);
        }

        public ICommand SendNotification { get; set; }
        public ICommand LoadData { get; set; }

        public SendNotificationViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Pilots = new ObservableCollection<User>();
            FlightAttendants = new ObservableCollection<User>();
            MessageBody = "";

            LoadData = CreateAsyncCommand(LoadDataAsync);
            SendNotification = CreateAsyncCommand(SendNotificationAsync);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var pilots = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.PILOT);
                var flightAttendants = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.FLIGHTATTENDAT);

                Pilots = new ObservableCollection<User>(pilots.Where(u => !u.IsBlocked));
                FlightAttendants = new ObservableCollection<User>(flightAttendants.Where(u => !u.IsBlocked));
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task SendNotificationAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MessageBody))
                {
                    ModernMessageDialog.Show("Введите текст уведомления", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (!SendToAllPilots && !SendToAllFlightAttendants && SelectedPilot == null && SelectedFlightAttendant == null)
                {
                    ModernMessageDialog.Show("Выберите получателя или отправьте всем", "Предупреждение", MessageType.Warning);
                    return;
                }

                int sentCount = 0;

                if (SendToAllPilots)
                {
                    foreach (var pilot in Pilots)
                    {
                        var notification = new Notification(pilot.Id, MessageBody);
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                        sentCount++;
                    }
                }
                else if (SelectedPilot != null)
                {
                    var notification = new Notification(SelectedPilot.Id, MessageBody);
                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                    sentCount++;
                }

                if (SendToAllFlightAttendants)
                {
                    foreach (var fa in FlightAttendants)
                    {
                        var notification = new Notification(fa.Id, MessageBody);
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                        sentCount++;
                    }
                }
                else if (SelectedFlightAttendant != null)
                {
                    var notification = new Notification(SelectedFlightAttendant.Id, MessageBody);
                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                    sentCount++;
                }

                await _unitOfWork.SaveAsync();

                ModernMessageDialog.Show($"Уведомление успешно отправлено {sentCount} получателю(ям)", "Успех", MessageType.Success);
                
                MessageBody = "";
                SelectedPilot = null;
                SelectedFlightAttendant = null;
                SendToAllPilots = false;
                SendToAllFlightAttendants = false;
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при отправке уведомления: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
