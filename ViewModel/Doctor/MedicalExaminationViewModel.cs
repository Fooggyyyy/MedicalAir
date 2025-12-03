using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class MedicalExaminationViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<MedicalExamination> medicalExaminations;
        private ObservableCollection<MedicalExamination> allMedicalExaminations;
        private Dictionary<int, string> examinationProceduresMap; 
        public ObservableCollection<MedicalExamination> MedicalExaminations
        {
            get => medicalExaminations;
            set => Set(ref medicalExaminations, value);
        }

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
                
                var filteredExaminations = examinationsList
                    .Where(e => e.User != null && 
                               (e.User.Roles == UserRoles.PILOT || e.User.Roles == UserRoles.FLIGHTATTENDAT))
                    .OrderByDescending(e => e.DataEnd)
                    .ToList();
                
                allMedicalExaminations = new ObservableCollection<MedicalExamination>(filteredExaminations);
                
                examinationProceduresMap.Clear();
                foreach (var examination in allMedicalExaminations)
                {
                    if (examination.User != null)
                    {
                        
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

                var allUsersFromDb = await _unitOfWork.UserRepository.GetAllAsync();
                
                var filteredUsers = allUsersFromDb
                    .Where(u => u.Roles == UserRoles.PILOT || u.Roles == UserRoles.FLIGHTATTENDAT)
                    .Where(u => u.Roles != UserRoles.ADMIN && u.Roles != UserRoles.DOCTOR)
                    .ToList();
                
                PilotsAndAttendants = new ObservableCollection<User>(filteredUsers);
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

                var userRoleProcedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(SelectedUser.Roles);
                
                if (!userRoleProcedures.Any())
                {
                    ModernMessageDialog.Show("Для данной роли не назначены процедуры", "Предупреждение", MessageType.Warning);
                    return;
                }

                var startDate = DateOnly.FromDateTime(DataStart.Value.Date);
                var endDate = DateOnly.FromDateTime(DataEnd.Value.Date);

                var existingExaminations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(SelectedUser.Id);
                var exists = existingExaminations.Any(e => e.DataStart == startDate && e.DataEnd == endDate);
                
                if (exists)
                {
                    ModernMessageDialog.Show("Медосмотр для данного пользователя с такими датами уже существует", "Информация", MessageType.Info);
                    return;
                }

                var firstProcedure = userRoleProcedures.First();
                var examination = new MedicalExamination(SelectedUser.Id, firstProcedure.Id, startDate, endDate, false, Message);
                await _unitOfWork.MedicalExaminationRepository.AddAsync(examination);
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

                var dialog = new Helper.Dialogs.EditMedicalExaminationDialog(SelectedMedicalExamination);
                if (dialog.ShowDialog() == true)
                {
                    SelectedMedicalExamination.DataStart = DateOnly.FromDateTime(dialog.DataStart.Value.Date);
                    SelectedMedicalExamination.DataEnd = DateOnly.FromDateTime(dialog.DataEnd.Value.Date);
                    
                    SelectedMedicalExamination.Message = dialog.Message;
                    
                    await _unitOfWork.MedicalExaminationRepository.UpdateAsync(SelectedMedicalExamination);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Медосмотр успешно обновлен", "Успех", MessageType.Success);
                }
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

                var userProcedures = await _unitOfWork.UserProcedureRepository.GetByUserIdAsync(SelectedMedicalExamination.UserId);
                var examinationProcedures = userProcedures
                    .Where(up => up.StartData == SelectedMedicalExamination.DataStart && 
                                up.EndData == SelectedMedicalExamination.DataEnd)
                    .ToList();

                foreach (var procedure in examinationProcedures)
                {
                    await _unitOfWork.UserProcedureRepository.DeleteAsync(procedure);
                }

                await _unitOfWork.MedicalExaminationRepository.DeleteAsync(SelectedMedicalExamination);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Медосмотр и все связанные процедуры успешно удалены", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении медосмотра: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
