using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Admin
{
    public class MainAdminViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        //On registers
        private int newId;
        public int NewId
        {
            get => newId;
            set => Set(ref newId, value);
        }

        private int newAirplaneId;
        public int NewAirplaneId
        {
            get => newAirplaneId;
            set => Set(ref newAirplaneId, value);
        }

        private int newUserId;
        public int NewUserId
        {
            get => newUserId;
            set => Set(ref newUserId, value);
        }

        private string newMessageBody;
        public string NewMessageBody
        {
            get => newMessageBody;
            set => Set(ref newMessageBody, value);
        }

        private DateOnly newData;
        public DateOnly NewData
        {
            get => newData;
            set => Set(ref newData, value);
        }

        //

        private ObservableCollection<RegistrationUser> registrationUsers;
        public ObservableCollection<RegistrationUser> RegistrationUsers
        {
            get => registrationUsers;
            set => Set(ref registrationUsers, value);

        }

        private ObservableCollection<Airplane> airplanes;
        public ObservableCollection<Airplane> Airplanes
        {
            get => airplanes;
            set => Set(ref airplanes, value);
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set => Set(ref users, value);
        }

        private RegistrationUser selectedRegistr;

        public RegistrationUser SelectedRegistr
        {
            get => selectedRegistr;
            set => Set(ref selectedRegistr, value);
        }

        private Airplane selectedAirplane;
        public Airplane SelectedAirplane
        {
            get => selectedAirplane;
            set => Set(ref selectedAirplane, value);
        }

        private User selectedUser;
        public User SelectedUser
        { 
            get => selectedUser;
            set => Set(ref selectedUser, value);
        }

        public ICommand UpdateAirplaneToUser { get; set; }

        public ICommand AddAirplane { get; set; }
        public ICommand DeleteAirplane { get; set; }
        public ICommand EditAirplane { get; set; }

        public ICommand AddRegisterToUser { get; set; }
        public ICommand DeleteRegisterToUser { get; set; }
        public ICommand EditRegisterToUser { get; set; }

        public ICommand ToggleBlockUser { get; set; }

        public ICommand LoadData { get; set; }
        public MainAdminViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Airplanes = new ObservableCollection<Airplane>();
            Users = new ObservableCollection<User>();

            LoadData = CreateAsyncCommand(LoadDataAsync);

            AddAirplane = CreateAsyncCommand(AddAirplaneAsync);
            EditAirplane = CreateAsyncCommand(EditAirplaneAsync, () => SelectedAirplane != null);
            DeleteAirplane = CreateAsyncCommand(DeleteAirplaneAsync, () => SelectedAirplane != null);

            UpdateAirplaneToUser = CreateAsyncCommand(UpdateAirplaneToUserAsync, () => SelectedUser != null && SelectedAirplane != null);

            AddRegisterToUser = CreateAsyncCommand(AddRegisterToUserAsync);
            DeleteRegisterToUser = CreateAsyncCommand(RemoveRegisterToUserAsync, () => SelectedRegistr != null);
            EditRegisterToUser = CreateAsyncCommand(EditRegisterToUserAsync, () => SelectedRegistr != null);

            ToggleBlockUser = CreateAsyncCommand(ToggleBlockUserAsync, () => SelectedUser != null);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var airplanesList = await _unitOfWork.AirplaneRepository.GetAllAsync();
                var usersList = await _unitOfWork.UserRepository.GetAllAsync();
                var registersList = await _unitOfWork.RegistrationUserRepository.GetAllAsync();

                // Фильтруем админов из списка пользователей
                var filteredUsers = usersList.Where(u => u.Roles != Model.Enums.UserRoles.ADMIN).ToList();

                RegistrationUsers = new ObservableCollection<RegistrationUser>(registersList);
                Airplanes = new ObservableCollection<Airplane>(airplanesList);
                Users = new ObservableCollection<User>(filteredUsers);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddAirplaneAsync()
        {
            try
            {
                var dialog = new Helper.Dialogs.AddAirplaneDialog("Добавление самолета", "");
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.AirplaneName))
                {
                    var airplane = new Airplane(dialog.AirplaneName);
                    await _unitOfWork.AirplaneRepository.AddAsync(airplane);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show($"Самолет '{dialog.AirplaneName}' успешно добавлен", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при добавлении самолета: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task EditAirplaneAsync()
        {
            try
            {
                if (SelectedAirplane == null)
                {
                    ModernMessageDialog.Show("Выберите самолет для редактирования", "Предупреждение", MessageType.Warning);
                    return;
                }

                var dialog = new Helper.Dialogs.AddAirplaneDialog("Редактирование самолета", SelectedAirplane.Name);
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.AirplaneName))
                {
                    SelectedAirplane.Name = dialog.AirplaneName;
                    await _unitOfWork.AirplaneRepository.UpdateAsync(SelectedAirplane);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Самолет успешно обновлен", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при редактировании самолета: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteAirplaneAsync()
        {
            try
            {
                if (SelectedAirplane == null)
                {
                    ModernMessageDialog.Show("Выберите самолет для удаления", "Предупреждение", MessageType.Warning);
                    return;
                }

                var result = ModernMessageDialog.ShowQuestion(
                    $"Вы уверены, что хотите удалить самолет '{SelectedAirplane.Name}'?",
                    "Подтверждение удаления");

                if (result == true)
                {
                    await _unitOfWork.AirplaneRepository.DeleteAsync(SelectedAirplane.Id);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Самолет успешно удален", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении самолета: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateAirplaneToUserAsync()
        {
            try
            {
                if (SelectedUser == null)
                {
                    ModernMessageDialog.Show("Выберите пользователя", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (SelectedAirplane == null)
                {
                    ModernMessageDialog.Show("Выберите самолет", "Предупреждение", MessageType.Warning);
                    return;
                }

                SelectedUser.AirplaneId = SelectedAirplane.Id;
                await _unitOfWork.UserRepository.UpdateAsync(SelectedUser);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show($"Самолет '{SelectedAirplane.Name}' успешно назначен пользователю '{SelectedUser.FullName}'", 
                    "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при назначении самолета пользователю: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddRegisterToUserAsync()
        {
            try
            {
                var dialog = new Helper.Dialogs.AddRegistrationDialog(Users, Airplanes);
                if (dialog.ShowDialog() == true)
                {
                    var selectedDate = DateOnly.FromDateTime(dialog.SelectedDate);
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    var minDate = today.AddDays(7); // Минимум на неделю позже текущей даты
                    
                    // Проверка: регистрация должна быть минимум на неделю позже текущей даты
                    if (selectedDate < minDate)
                    {
                        ModernMessageDialog.Show(
                            $"Дата регистрации должна быть минимум на неделю позже текущей даты (не ранее {minDate:dd.MM.yyyy})",
                            "Ошибка", MessageType.Warning);
                        return;
                    }

                    // Проверка: не может быть две регистрации для пользователя с периодичностью меньше недели
                    var existingRegistrations = await _unitOfWork.RegistrationUserRepository.GetByUserIdAsync(dialog.SelectedUserId);
                    var existingRegistrationsList = existingRegistrations.ToList();
                    
                    foreach (var existingReg in existingRegistrationsList)
                    {
                        var daysDifference = Math.Abs((selectedDate.ToDateTime(TimeOnly.MinValue) - existingReg.Data.ToDateTime(TimeOnly.MinValue)).Days);
                        if (daysDifference < 7)
                        {
                            ModernMessageDialog.Show(
                                $"Нельзя создать регистрацию: у пользователя уже есть регистрация на {existingReg.Data:dd.MM.yyyy}. " +
                                $"Минимальный интервал между регистрациями - 7 дней.",
                                "Ошибка", MessageType.Warning);
                            return;
                        }
                    }

                    var newRegister = new RegistrationUser(
                        dialog.SelectedAirplaneId, 
                        dialog.SelectedUserId, 
                        false, 
                        dialog.MessageBody, 
                        selectedDate);
                    
                    await _unitOfWork.RegistrationUserRepository.AddAsync(newRegister);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Регистрация успешно добавлена!", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка создания регистрации: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task RemoveRegisterToUserAsync()
        {
            try
            {
                if (SelectedRegistr == null)
                {
                    ModernMessageDialog.Show("Выберите регистрацию для удаления!", "Ошибка", MessageType.Warning);
                    return;
                }

                var result = ModernMessageDialog.ShowQuestion(
                    $"Вы уверены, что хотите удалить регистрацию пользователя '{SelectedRegistr.User?.FullName}' от {SelectedRegistr.Data:dd.MM.yyyy}?",
                    "Подтверждение удаления");

                if (result == true)
                {
                    await _unitOfWork.RegistrationUserRepository.DeleteAsync(SelectedRegistr.Id);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Регистрация успешно удалена!", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка удаления регистрации: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }


        private async Task EditRegisterToUserAsync()
        {
            try
            {
                if (SelectedRegistr == null)
                {
                    ModernMessageDialog.Show("Выберите регистрацию для изменения!", "Ошибка", MessageType.Warning);
                    return;
                }

                // Используем диалог для редактирования
                var dialog = new Helper.Dialogs.AddRegistrationDialog(Users, Airplanes);
                
                // Устанавливаем текущие значения в диалог
                dialog.SetValues(
                    SelectedRegistr.UserId,
                    SelectedRegistr.AirplaneId,
                    SelectedRegistr.MessageBody,
                    SelectedRegistr.Data.ToDateTime(TimeOnly.MinValue));
                
                // Переключаем в режим редактирования
                dialog.SetEditMode();

                if (dialog.ShowDialog() == true)
                {
                    var selectedDate = DateOnly.FromDateTime(dialog.SelectedDate);
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    var minDate = today.AddDays(7); // Минимум на неделю позже текущей даты
                    
                    // Проверка: регистрация должна быть минимум на неделю позже текущей даты
                    if (selectedDate < minDate)
                    {
                        ModernMessageDialog.Show(
                            $"Дата регистрации должна быть минимум на неделю позже текущей даты (не ранее {minDate:dd.MM.yyyy})",
                            "Ошибка", MessageType.Warning);
                        return;
                    }

                    // Проверка: не может быть две регистрации для пользователя с периодичностью меньше недели
                    var existingRegistrations = await _unitOfWork.RegistrationUserRepository.GetByUserIdAsync(dialog.SelectedUserId);
                    var existingRegistrationsList = existingRegistrations.Where(r => r.Id != SelectedRegistr.Id).ToList(); // Исключаем текущую регистрацию
                    
                    foreach (var existingReg in existingRegistrationsList)
                    {
                        var daysDifference = Math.Abs((selectedDate.ToDateTime(TimeOnly.MinValue) - existingReg.Data.ToDateTime(TimeOnly.MinValue)).Days);
                        if (daysDifference < 7)
                        {
                            ModernMessageDialog.Show(
                                $"Нельзя изменить регистрацию: у пользователя уже есть регистрация на {existingReg.Data:dd.MM.yyyy}. " +
                                $"Минимальный интервал между регистрациями - 7 дней.",
                                "Ошибка", MessageType.Warning);
                            return;
                        }
                    }

                    SelectedRegistr.AirplaneId = dialog.SelectedAirplaneId;
                    SelectedRegistr.UserId = dialog.SelectedUserId;
                    SelectedRegistr.MessageBody = dialog.MessageBody;
                    SelectedRegistr.Data = selectedDate;

                    await _unitOfWork.RegistrationUserRepository.UpdateAsync(SelectedRegistr);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Регистрация успешно обновлена!", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка обновления регистрации: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task ToggleBlockUserAsync()
        {
            try
            {
                if (SelectedUser == null)
                {
                    ModernMessageDialog.Show("Выберите пользователя для блокировки/разблокировки", "Предупреждение", MessageType.Warning);
                    return;
                }

                var action = SelectedUser.IsBlocked ? "разблокировать" : "заблокировать";
                var result = ModernMessageDialog.ShowQuestion(
                    $"Вы уверены, что хотите {action} пользователя '{SelectedUser.FullName}'?",
                    $"Подтверждение {action}");

                if (result == true)
                {
                    var userId = SelectedUser.Id;
                    var userName = SelectedUser.FullName;
                    var newBlockStatus = !SelectedUser.IsBlocked;
                    
                    SelectedUser.IsBlocked = newBlockStatus;
                    await _unitOfWork.UserRepository.UpdateAsync(SelectedUser);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    
                    // Восстанавливаем выбранного пользователя после перезагрузки
                    SelectedUser = Users.FirstOrDefault(u => u.Id == userId);
                    
                    var status = newBlockStatus ? "заблокирован" : "разблокирован";
                    ModernMessageDialog.Show($"Пользователь '{userName}' успешно {status}", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при изменении статуса блокировки: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

    }
}
