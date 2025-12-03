using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using MedicalAir.Services;
using System.Windows.Input;

namespace MedicalAir.ViewModel.FlightAttendant
{
    public class MainFaViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;

        private User currentUser;
        public User CurrentUser
        {
            get => currentUser;
            set => Set(ref currentUser, value);
        }

        private string airplaneName;
        public string AirplaneName
        {
            get => airplaneName;
            set => Set(ref airplaneName, value);
        }

        private string registrationDate;
        public string RegistrationDate
        {
            get => registrationDate;
            set => Set(ref registrationDate, value);
        }

        private bool isRegistered;
        public bool IsRegistered
        {
            get => isRegistered;
            set => Set(ref isRegistered, value);
        }

        private string registrationStatus;
        public string RegistrationStatus
        {
            get => registrationStatus;
            set => Set(ref registrationStatus, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand RefreshData { get; set; }

        public MainFaViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notificationService = new NotificationService(unitOfWork);
            AirplaneName = "Не назначен";
            RegistrationDate = "Нет данных";
            RegistrationStatus = "Не зарегистрирован";

            LoadData = CreateAsyncCommand(LoadDataAsync);
            RefreshData = CreateAsyncCommand(LoadDataAsync);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (Session.UserId == 0)
                {
                    
                    ModernMessageDialog.Show("Пользователь не авторизован", "Ошибка", MessageType.Error);
                    return;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                if (user == null)
                {
                    ModernMessageDialog.Show("Пользователь не найден", "Ошибка", MessageType.Error);
                    return;
                }

                CurrentUser = user;

                if (user.AirplaneId.HasValue)
                {
                    var airplane = await _unitOfWork.AirplaneRepository.GetByIdAsync(user.AirplaneId.Value);
                    if (airplane != null)
                    {
                        AirplaneName = airplane.Name ?? "Неизвестный самолет";
                    }
                    else
                    {
                        AirplaneName = "Самолет не найден";
                    }
                }
                else
                {
                    AirplaneName = "Не назначен";
                }

                var registrations = await _unitOfWork.RegistrationUserRepository.GetByUserIdAsync(Session.UserId);
                var registrationsList = registrations.ToList();

                if (registrationsList.Any())
                {
                    
                    var latestRegistration = registrationsList
                        .OrderByDescending(r => r.Data)
                        .ThenByDescending(r => r.Id)
                        .FirstOrDefault();

                    if (latestRegistration != null)
                    {
                        RegistrationDate = latestRegistration.Data.ToString("dd.MM.yyyy");
                        IsRegistered = latestRegistration.IsRegister;
                        RegistrationStatus = latestRegistration.IsRegister 
                            ? "✅ Подтверждена" 
                            : "⏳ Ожидает подтверждения";
                    }
                }
                else
                {
                    RegistrationDate = "Нет данных";
                    IsRegistered = false;
                    RegistrationStatus = "Не зарегистрирован";
                }

                await _notificationService.GenerateAutomaticNotificationsAsync(Session.UserId);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
