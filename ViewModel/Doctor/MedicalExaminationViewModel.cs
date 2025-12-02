using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class MedicalExaminationViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<MedicalExamination> medicalExaminations;
        private ObservableCollection<MedicalExamination> allMedicalExaminations;
        private Dictionary<int, string> examinationProceduresMap; // Словарь для хранения процедур каждого медосмотра
        public ObservableCollection<MedicalExamination> MedicalExaminations
        {
            get => medicalExaminations;
            set => Set(ref medicalExaminations, value);
        }

        // Метод для получения строки процедур для медосмотра
        public string GetProceduresForExamination(MedicalExamination examination)
        {
            if (examination == null || examinationProceduresMap == null)
                return "";

            if (examinationProceduresMap.TryGetValue(examination.Id, out string procedures))
            {
                return procedures;
            }
            return "";
        }

        private string searchExaminations;
        public string SearchExaminations
        {
            get => searchExaminations;
            set
            {
                Set(ref searchExaminations, value);
                FilterExaminations();
            }
        }

        private ObservableCollection<User> pilotsAndAttendants;
        public ObservableCollection<User> PilotsAndAttendants
        {
            get => pilotsAndAttendants;
            set => Set(ref pilotsAndAttendants, value);
        }

        private ObservableCollection<UserRoleProcedure> userRoleProcedures;
        public ObservableCollection<UserRoleProcedure> UserRoleProcedures
        {
            get => userRoleProcedures;
            set => Set(ref userRoleProcedures, value);
        }

        private MedicalExamination selectedMedicalExamination;
        public MedicalExamination SelectedMedicalExamination
        {
            get => selectedMedicalExamination;
            set => Set(ref selectedMedicalExamination, value);
        }

        private User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                Set(ref selectedUser, value);
                UpdateUserRoleProcedures();
            }
        }

        private UserRoleProcedure selectedUserRoleProcedure;
        public UserRoleProcedure SelectedUserRoleProcedure
        {
            get => selectedUserRoleProcedure;
            set => Set(ref selectedUserRoleProcedure, value);
        }

        private DateTime? dataStart;
        public DateTime? DataStart
        {
            get => dataStart;
            set => Set(ref dataStart, value);
        }

        private DateTime? dataEnd;
        public DateTime? DataEnd
        {
            get => dataEnd;
            set => Set(ref dataEnd, value);
        }


        private string message;
        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand AddMedicalExamination { get; set; }
        public ICommand UpdateMedicalExamination { get; set; }
        public ICommand DeleteMedicalExamination { get; set; }
        public ICommand ClearSearchExaminations { get; set; }

        public MedicalExaminationViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            MedicalExaminations = new ObservableCollection<MedicalExamination>();
            PilotsAndAttendants = new ObservableCollection<User>();
            UserRoleProcedures = new ObservableCollection<UserRoleProcedure>();
            examinationProceduresMap = new Dictionary<int, string>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            AddMedicalExamination = CreateAsyncCommand(AddMedicalExaminationAsync, () => SelectedUser != null && DataStart.HasValue && DataEnd.HasValue);
            UpdateMedicalExamination = CreateAsyncCommand(UpdateMedicalExaminationAsync, () => SelectedMedicalExamination != null && DataStart.HasValue && DataEnd.HasValue);
            DeleteMedicalExamination = CreateAsyncCommand(DeleteMedicalExaminationAsync, () => SelectedMedicalExamination != null);
            ClearSearchExaminations = CreateCommand(_ => { SearchExaminations = ""; });

            LoadDataAsync();
        }

        private void FilterExaminations()
        {
            if (allMedicalExaminations == null) return;

            if (string.IsNullOrWhiteSpace(SearchExaminations))
            {
                MedicalExaminations = new ObservableCollection<MedicalExamination>(allMedicalExaminations);
            }
            else
            {
                var searchLower = SearchExaminations.ToLower();
                var filtered = allMedicalExaminations.Where(e => 
                    (e.User?.FullName?.ToLower().Contains(searchLower) ?? false) ||
                    (e.UserRoleProcedure?.Procedure?.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (e.Message?.ToLower().Contains(searchLower) ?? false) ||
                    e.DataStart.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    e.DataEnd.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    e.Id.ToString().Contains(searchLower)
                ).ToList();
                MedicalExaminations = new ObservableCollection<MedicalExamination>(filtered);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var examinationsList = await _unitOfWork.MedicalExaminationRepository.GetAllAsync();
                allMedicalExaminations = new ObservableCollection<MedicalExamination>(examinationsList.OrderByDescending(e => e.DataEnd));
                
                // Загружаем процедуры для каждого медосмотра
                examinationProceduresMap.Clear();
                foreach (var examination in allMedicalExaminations)
                {
                    if (examination.User != null)
                    {
                        // Получаем все процедуры роли пользователя
                        var roleProcedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(examination.User.Roles);
                        var procedureNames = roleProcedures
                            .Where(rp => rp.Procedure != null)
                            .Select(rp => rp.Procedure.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .ToList();
                        
                        examinationProceduresMap[examination.Id] = string.Join(", ", procedureNames);
                    }
                }
                
                FilterExaminations();

                var pilots = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.PILOT);
                var flightAttendants = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.FLIGHTATTENDAT);
                
                // Фильтруем только пилотов и бортпроводников (исключаем админов и врачей)
                var allUsers = pilots.Concat(flightAttendants)
                    .Where(u => u.Roles == UserRoles.PILOT || u.Roles == UserRoles.FLIGHTATTENDAT)
                    .ToList();
                PilotsAndAttendants = new ObservableCollection<User>(allUsers);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateUserRoleProcedures()
        {
            try
            {
                if (SelectedUser == null)
                {
                    UserRoleProcedures = new ObservableCollection<UserRoleProcedure>();
                    SelectedUserRoleProcedure = null;
                    return;
                }

                var procedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(SelectedUser.Roles);
                UserRoleProcedures = new ObservableCollection<UserRoleProcedure>(procedures);
                
                // Автоматически выбираем первую процедуру
                if (UserRoleProcedures.Any())
                {
                    SelectedUserRoleProcedure = UserRoleProcedures.First();
                }
                else
                {
                    SelectedUserRoleProcedure = null;
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке процедур: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddMedicalExaminationAsync()
        {
            try
            {
                if (SelectedUser == null)
                {
                    ModernMessageDialog.Show("Выберите пользователя", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (!DataStart.HasValue || !DataEnd.HasValue)
                {
                    ModernMessageDialog.Show("Выберите даты начала и окончания", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (DataStart.Value > DataEnd.Value)
                {
                    ModernMessageDialog.Show("Дата начала не может быть позже даты окончания", "Ошибка", MessageType.Error);
                    return;
                }

                // Получаем все процедуры для роли пользователя
                var userRoleProcedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(SelectedUser.Roles);
                
                if (!userRoleProcedures.Any())
                {
                    ModernMessageDialog.Show("Для данной роли не назначены процедуры", "Предупреждение", MessageType.Warning);
                    return;
                }

                var startDate = DateOnly.FromDateTime(DataStart.Value.Date);
                var endDate = DateOnly.FromDateTime(DataEnd.Value.Date);

                // Проверяем, не существует ли уже медосмотр для этого пользователя с такими же датами
                var existingExaminations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(SelectedUser.Id);
                var exists = existingExaminations.Any(e => e.DataStart == startDate && e.DataEnd == endDate);
                
                if (exists)
                {
                    ModernMessageDialog.Show("Медосмотр для данного пользователя с такими датами уже существует", "Информация", MessageType.Info);
                    return;
                }

                // Создаем один медосмотр для первой процедуры роли (как основной)
                var firstProcedure = userRoleProcedures.First();
                var examination = new MedicalExamination(SelectedUser.Id, firstProcedure.Id, startDate, endDate, false, Message);
                await _unitOfWork.MedicalExaminationRepository.AddAsync(examination);
                await _unitOfWork.SaveAsync();

                // Создаем UserProcedure для всех процедур роли с теми же датами
                // Медосмотр будет считаться пройденным, если все UserProcedure валидны
                foreach (var userRoleProcedure in userRoleProcedures)
                {
                    // Проверяем, не существует ли уже UserProcedure для этой процедуры и пользователя с такими датами
                    var existingUserProcedures = await _unitOfWork.UserProcedureRepository.GetByUserIdAsync(SelectedUser.Id);
                    var userProcedureExists = existingUserProcedures.Any(up => up.ProcedureId == userRoleProcedure.ProcedureId && 
                                                                              up.StartData == startDate && 
                                                                              up.EndData == endDate);
                    
                    if (!userProcedureExists)
                    {
                        var userProcedure = new UserProcedure(SelectedUser.Id, userRoleProcedure.ProcedureId, startDate, endDate, false);
                        await _unitOfWork.UserProcedureRepository.AddAsync(userProcedure);
                    }
                }

                await _unitOfWork.SaveAsync();
                await LoadDataAsync();
                ModernMessageDialog.Show("Медосмотр успешно создан со всеми процедурами роли", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании медосмотра: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateMedicalExaminationAsync()
        {
            try
            {
                if (SelectedMedicalExamination == null)
                {
                    ModernMessageDialog.Show("Выберите медосмотр", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (!DataStart.HasValue || !DataEnd.HasValue)
                {
                    ModernMessageDialog.Show("Выберите даты начала и окончания", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (DataStart.Value > DataEnd.Value)
                {
                    ModernMessageDialog.Show("Дата начала не может быть позже даты окончания", "Ошибка", MessageType.Error);
                    return;
                }

                SelectedMedicalExamination.DataStart = DateOnly.FromDateTime(DataStart.Value.Date);
                SelectedMedicalExamination.DataEnd = DateOnly.FromDateTime(DataEnd.Value.Date);
                // IsValid не устанавливается вручную - он обновляется автоматически на основе UserProcedure
                SelectedMedicalExamination.Message = Message;
                
                await _unitOfWork.MedicalExaminationRepository.UpdateAsync(SelectedMedicalExamination);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Медосмотр успешно обновлен", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении медосмотра: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteMedicalExaminationAsync()
        {
            try
            {
                if (SelectedMedicalExamination == null)
                {
                    ModernMessageDialog.Show("Выберите медосмотр", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.MedicalExaminationRepository.DeleteAsync(SelectedMedicalExamination);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Медосмотр успешно удален", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении медосмотра: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
