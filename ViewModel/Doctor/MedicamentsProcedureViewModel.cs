using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class MedicamentsProcedureViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<Procedure> procedures;
        private ObservableCollection<Procedure> allProcedures;
        public ObservableCollection<Procedure> Procedures
        {
            get => procedures;
            set => Set(ref procedures, value);
        }

        private string searchProcedures;
        public string SearchProcedures
        {
            get => searchProcedures;
            set
            {
                Set(ref searchProcedures, value);
                FilterProcedures();
            }
        }

        private Procedure selectedProcedure;
        public Procedure SelectedProcedure
        {
            get => selectedProcedure;
            set => Set(ref selectedProcedure, value);
        }

        private string procedureName;
        public string ProcedureName
        {
            get => procedureName;
            set => Set(ref procedureName, value);
        }

        private string procedureDescription;
        public string ProcedureDescription
        {
            get => procedureDescription;
            set => Set(ref procedureDescription, value);
        }

        private int minValue;
        public int MinValue
        {
            get => minValue;
            set => Set(ref minValue, value);
        }

        private int maxValue;
        public int MaxValue
        {
            get => maxValue;
            set => Set(ref maxValue, value);
        }

        private string units;
        public string Units
        {
            get => units;
            set => Set(ref units, value);
        }

        private bool mustBeTrue;
        public bool MustBeTrue
        {
            get => mustBeTrue;
            set
            {
                Set(ref mustBeTrue, value);
                if (value)
                {
                    
                    MinValue = 0;
                    MaxValue = 1;
                }
            }
        }

        private ObservableCollection<UserRoleProcedure> userRoleProcedures;
        public ObservableCollection<UserRoleProcedure> UserRoleProcedures
        {
            get => userRoleProcedures;
            set => Set(ref userRoleProcedures, value);
        }

        private UserRoleProcedure selectedUserRoleProcedure;
        public UserRoleProcedure SelectedUserRoleProcedure
        {
            get => selectedUserRoleProcedure;
            set => Set(ref selectedUserRoleProcedure, value);
        }

        private ObservableCollection<UserRoles> availableRoles;
        public ObservableCollection<UserRoles> AvailableRoles
        {
            get => availableRoles;
            set => Set(ref availableRoles, value);
        }

        private UserRoles selectedRole;
        public UserRoles SelectedRole
        {
            get => selectedRole;
            set
            {
                if (Set(ref selectedRole, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private Procedure selectedProcedureForRole;
        public Procedure SelectedProcedureForRole
        {
            get => selectedProcedureForRole;
            set
            {
                if (Set(ref selectedProcedureForRole, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private ObservableCollection<Medicin> medicins;
        private ObservableCollection<Medicin> allMedicins;
        public ObservableCollection<Medicin> Medicins
        {
            get => medicins;
            set => Set(ref medicins, value);
        }

        private string searchMedicins;
        public string SearchMedicins
        {
            get => searchMedicins;
            set
            {
                Set(ref searchMedicins, value);
                FilterMedicins();
            }
        }

        private Medicin selectedMedicin;
        public Medicin SelectedMedicin
        {
            get => selectedMedicin;
            set => Set(ref selectedMedicin, value);
        }

        private string medicinName;
        public string MedicinName
        {
            get => medicinName;
            set => Set(ref medicinName, value);
        }

        private string medicinComposition;
        public string MedicinComposition
        {
            get => medicinComposition;
            set => Set(ref medicinComposition, value);
        }

        private ObservableCollection<HistoryUpMedicin> historyUpMedicins;
        private ObservableCollection<HistoryUpMedicin> allHistoryUpMedicins;
        public ObservableCollection<HistoryUpMedicin> HistoryUpMedicins
        {
            get => historyUpMedicins;
            set => Set(ref historyUpMedicins, value);
        }

        private string searchHistoryMedicines;
        public string SearchHistoryMedicines
        {
            get => searchHistoryMedicines;
            set
            {
                Set(ref searchHistoryMedicines, value);
                FilterHistoryMedicines();
            }
        }

        private HistoryUpMedicin selectedHistoryUpMedicin;
        public HistoryUpMedicin SelectedHistoryUpMedicin
        {
            get => selectedHistoryUpMedicin;
            set => Set(ref selectedHistoryUpMedicin, value);
        }

        private Medicin selectedMedicinForReplenishment;
        public Medicin SelectedMedicinForReplenishment
        {
            get => selectedMedicinForReplenishment;
            set => Set(ref selectedMedicinForReplenishment, value);
        }

        private int count;
        public int Count
        {
            get => count;
            set => Set(ref count, value);
        }

        private DateTime? upData;
        public DateTime? UpData
        {
            get => upData;
            set => Set(ref upData, value);
        }

        private DateTime? endData;
        public DateTime? EndData
        {
            get => endData;
            set => Set(ref endData, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand AddProcedure { get; set; }
        public ICommand UpdateProcedure { get; set; }
        public ICommand DeleteProcedure { get; set; }
        public ICommand AddUserRoleProcedure { get; set; }
        public ICommand DeleteUserRoleProcedure { get; set; }
        public ICommand AddMedicin { get; set; }
        public ICommand UpdateMedicin { get; set; }
        public ICommand DeleteMedicin { get; set; }
        public ICommand AddHistoryUpMedicin { get; set; }
        public ICommand UpdateHistoryUpMedicin { get; set; }
        public ICommand DeleteHistoryUpMedicin { get; set; }
        public ICommand ClearSearchProcedures { get; set; }
        public ICommand ClearSearchMedicins { get; set; }
        public ICommand ClearSearchHistoryMedicines { get; set; }
        public ICommand AddSingleUserRoleProcedure { get; set; }

        public MedicamentsProcedureViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Procedures = new ObservableCollection<Procedure>();
            UserRoleProcedures = new ObservableCollection<UserRoleProcedure>();
            Medicins = new ObservableCollection<Medicin>();
            HistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>();
            AvailableRoles = new ObservableCollection<UserRoles> { UserRoles.PILOT, UserRoles.FLIGHTATTENDAT };

            LoadData = CreateAsyncCommand(LoadDataAsync);
            AddProcedure = CreateAsyncCommand(AddProcedureAsync, () => !string.IsNullOrWhiteSpace(ProcedureName));
            UpdateProcedure = CreateAsyncCommand(UpdateProcedureAsync, () => SelectedProcedure != null && !string.IsNullOrWhiteSpace(ProcedureName));
            DeleteProcedure = CreateAsyncCommand(DeleteProcedureAsync, () => SelectedProcedure != null);
            AddSingleUserRoleProcedure = CreateAsyncCommand(AddSingleUserRoleProcedureAsync, () => SelectedProcedureForRole != null && (SelectedRole == UserRoles.PILOT || SelectedRole == UserRoles.FLIGHTATTENDAT));
            AddUserRoleProcedure = CreateAsyncCommand(AddUserRoleProcedureAsync, () => SelectedRole == UserRoles.PILOT || SelectedRole == UserRoles.FLIGHTATTENDAT);
            DeleteUserRoleProcedure = CreateAsyncCommand(DeleteUserRoleProcedureAsync, () => SelectedUserRoleProcedure != null);
            AddMedicin = CreateAsyncCommand(AddMedicinAsync, () => !string.IsNullOrWhiteSpace(MedicinName) && !string.IsNullOrWhiteSpace(MedicinComposition));
            UpdateMedicin = CreateAsyncCommand(UpdateMedicinAsync, () => SelectedMedicin != null);
            DeleteMedicin = CreateAsyncCommand(DeleteMedicinAsync, () => SelectedMedicin != null);
            AddHistoryUpMedicin = CreateAsyncCommand(AddHistoryUpMedicinAsync, () => SelectedMedicinForReplenishment != null && UpData.HasValue && EndData.HasValue && Count > 0);
            UpdateHistoryUpMedicin = CreateAsyncCommand(UpdateHistoryUpMedicinAsync, () => SelectedHistoryUpMedicin != null);
            DeleteHistoryUpMedicin = CreateAsyncCommand(DeleteHistoryUpMedicinAsync, () => SelectedHistoryUpMedicin != null);
            ClearSearchProcedures = CreateCommand(_ => { SearchProcedures = ""; });
            ClearSearchMedicins = CreateCommand(_ => { SearchMedicins = ""; });
            ClearSearchHistoryMedicines = CreateCommand(_ => { SearchHistoryMedicines = ""; });

            LoadDataAsync();
        }

        private void FilterProcedures()
        {
            if (allProcedures == null) return;

            if (string.IsNullOrWhiteSpace(SearchProcedures))
            {
                Procedures = new ObservableCollection<Procedure>(allProcedures);
            }
            else
            {
                var searchLower = SearchProcedures.ToLower();
                var filtered = allProcedures.Where(p => 
                    (p.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (p.Description?.ToLower().Contains(searchLower) ?? false) ||
                    (p.Units?.ToLower().Contains(searchLower) ?? false) ||
                    p.MinValue.ToString().Contains(searchLower) ||
                    p.MaxValue.ToString().Contains(searchLower) ||
                    p.Id.ToString().Contains(searchLower)
                ).ToList();
                Procedures = new ObservableCollection<Procedure>(filtered);
            }
        }

        private void FilterMedicins()
        {
            if (allMedicins == null) return;

            if (string.IsNullOrWhiteSpace(SearchMedicins))
            {
                Medicins = new ObservableCollection<Medicin>(allMedicins);
            }
            else
            {
                var searchLower = SearchMedicins.ToLower();
                var filtered = allMedicins.Where(m => 
                    (m.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (m.Composition?.ToLower().Contains(searchLower) ?? false) ||
                    m.Id.ToString().Contains(searchLower)
                ).ToList();
                Medicins = new ObservableCollection<Medicin>(filtered);
            }
        }

        private void FilterHistoryMedicines()
        {
            if (allHistoryUpMedicins == null) return;

            if (string.IsNullOrWhiteSpace(SearchHistoryMedicines))
            {
                HistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>(allHistoryUpMedicins);
            }
            else
            {
                var searchLower = SearchHistoryMedicines.ToLower();
                var filtered = allHistoryUpMedicins.Where(m => 
                    (m.Name?.ToLower().Contains(searchLower) ?? false) ||
                    m.Count.ToString().Contains(searchLower) ||
                    m.UpData.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    m.EndData.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    m.Id.ToString().Contains(searchLower)
                ).ToList();
                HistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>(filtered);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var proceduresList = await _unitOfWork.ProcedureRepository.GetAllAsync();
                allProcedures = new ObservableCollection<Procedure>(proceduresList);
                FilterProcedures();

                var userRoleProceduresList = await _unitOfWork.RoleProcedureRepository.GetAllAsync();
                UserRoleProcedures = new ObservableCollection<UserRoleProcedure>(userRoleProceduresList);

                var medicinsList = await _unitOfWork.MedicinRepository.GetAllAsync();
                
                var uniqueMedicins = medicinsList
                    .GroupBy(m => m.Name)
                    .Select(g => g.First())
                    .OrderBy(m => m.Name)
                    .ToList();
                allMedicins = new ObservableCollection<Medicin>(uniqueMedicins);
                FilterMedicins();

                var historyList = await _unitOfWork.HistoryUpMedicinRepository.GetAllAsync();
                allHistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>(historyList.OrderByDescending(h => h.UpData));
                FilterHistoryMedicines();
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddProcedureAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProcedureName))
                {
                    ModernMessageDialog.Show("Введите название процедуры", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (!MustBeTrue && MinValue >= MaxValue)
                {
                    ModernMessageDialog.Show("Минимальное значение должно быть меньше максимального", "Ошибка", MessageType.Error);
                    return;
                }

                var procedure = new Procedure(ProcedureName, ProcedureDescription, MinValue, MaxValue, Units, MustBeTrue);
                await _unitOfWork.ProcedureRepository.AddAsync(procedure);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Процедура успешно создана", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании процедуры: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateProcedureAsync()
        {
            try
            {
                if (SelectedProcedure == null)
                {
                    ModernMessageDialog.Show("Выберите процедуру для редактирования", "Предупреждение", MessageType.Warning);
                    return;
                }

                var dialog = new Helper.Dialogs.EditProcedureDialog(SelectedProcedure);
                if (dialog.ShowDialog() == true)
                {
                    SelectedProcedure.Name = dialog.ProcedureName;
                    SelectedProcedure.Description = dialog.ProcedureDescription;
                    SelectedProcedure.MinValue = (int)dialog.MinValue;
                    SelectedProcedure.MaxValue = (int)dialog.MaxValue;
                    SelectedProcedure.Units = dialog.Units;
                    SelectedProcedure.MustBeTrue = dialog.MustBeTrue;

                    await _unitOfWork.ProcedureRepository.UpdateAsync(SelectedProcedure);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Процедура успешно обновлена", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении процедуры: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteProcedureAsync()
        {
            try
            {
                if (SelectedProcedure == null)
                {
                    ModernMessageDialog.Show("Выберите процедуру", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.ProcedureRepository.DeleteAsync(SelectedProcedure);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Процедура успешно удалена", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении процедуры: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddSingleUserRoleProcedureAsync()
        {
            try
            {
                if (SelectedProcedureForRole == null)
                {
                    ModernMessageDialog.Show("Выберите процедуру", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (SelectedRole != UserRoles.PILOT && SelectedRole != UserRoles.FLIGHTATTENDAT)
                {
                    ModernMessageDialog.Show("Выберите роль (Пилот или Бортпроводник)", "Предупреждение", MessageType.Warning);
                    return;
                }

                var existingProceduresForRole = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(SelectedRole);
                var exists = existingProceduresForRole.Any(urp => urp.ProcedureId == SelectedProcedureForRole.Id);

                if (exists)
                {
                    ModernMessageDialog.Show("Эта процедура уже назначена для данной роли", "Информация", MessageType.Info);
                    return;
                }

                var userRoleProcedure = new UserRoleProcedure(SelectedProcedureForRole.Id, SelectedRole);
                await _unitOfWork.RoleProcedureRepository.AddAsync(userRoleProcedure);
                await _unitOfWork.SaveAsync();
                await LoadDataAsync();
                ModernMessageDialog.Show($"Процедура '{SelectedProcedureForRole.Name}' успешно назначена для роли", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при назначении процедуры: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddUserRoleProcedureAsync()
        {
            try
            {
                if (SelectedRole != UserRoles.PILOT && SelectedRole != UserRoles.FLIGHTATTENDAT)
                {
                    ModernMessageDialog.Show("Выберите роль (Пилот или Бортпроводник)", "Предупреждение", MessageType.Warning);
                    return;
                }

                var allProcedures = await _unitOfWork.ProcedureRepository.GetAllAsync();
                
                var existingProceduresForRole = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(SelectedRole);
                var existingProcedureIds = existingProceduresForRole.Select(urp => urp.ProcedureId).ToList();

                int addedCount = 0;
                foreach (var procedure in allProcedures)
                {
                    if (!existingProcedureIds.Contains(procedure.Id))
                    {
                        var userRoleProcedure = new UserRoleProcedure(procedure.Id, SelectedRole);
                        await _unitOfWork.RoleProcedureRepository.AddAsync(userRoleProcedure);
                        addedCount++;
                    }
                }

                if (addedCount == 0)
                {
                    ModernMessageDialog.Show("Все процедуры уже назначены для данной роли", "Информация", MessageType.Info);
                    return;
                }

                await _unitOfWork.SaveAsync();
                await LoadDataAsync();
                ModernMessageDialog.Show($"Успешно назначено {addedCount} процедур(ы) для роли", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании связи: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteUserRoleProcedureAsync()
        {
            try
            {
                if (SelectedUserRoleProcedure == null)
                {
                    ModernMessageDialog.Show("Выберите связь", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.RoleProcedureRepository.DeleteAsync(SelectedUserRoleProcedure);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Связь успешно удалена", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении связи: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddMedicinAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MedicinName))
                {
                    ModernMessageDialog.Show("Введите название лекарства", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(MedicinComposition))
                {
                    ModernMessageDialog.Show("Введите состав лекарства", "Предупреждение", MessageType.Warning);
                    return;
                }

                var existingMedicin = await _unitOfWork.MedicinRepository.GetByNameAsync(MedicinName);
                if (existingMedicin != null)
                {
                    ModernMessageDialog.Show("Лекарство с таким названием уже существует", "Ошибка", MessageType.Error);
                    return;
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                var tempHistory = new HistoryUpMedicin(0, MedicinName, today, today, true);
                await _unitOfWork.HistoryUpMedicinRepository.AddAsync(tempHistory);
                await _unitOfWork.SaveAsync();

                var medicin = new Medicin(tempHistory.Id, MedicinName, MedicinComposition);
                await _unitOfWork.MedicinRepository.AddAsync(medicin);
                await _unitOfWork.SaveAsync();

                MedicinName = "";
                MedicinComposition = "";

                await LoadDataAsync();
                ModernMessageDialog.Show("Лекарство успешно создано", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateMedicinAsync()
        {
            try
            {
                if (SelectedMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите лекарство для редактирования", "Предупреждение", MessageType.Warning);
                    return;
                }

                var dialog = new Helper.Dialogs.EditMedicinDialog(SelectedMedicin);
                if (dialog.ShowDialog() == true)
                {
                    
                    var existingMedicin = await _unitOfWork.MedicinRepository.GetByNameAsync(dialog.MedicineName);
                    if (existingMedicin != null && existingMedicin.Id != SelectedMedicin.Id)
                    {
                        ModernMessageDialog.Show("Лекарство с таким названием уже существует", "Ошибка", MessageType.Error);
                        return;
                    }

                    SelectedMedicin.Name = dialog.MedicineName;
                    SelectedMedicin.Composition = dialog.MedicineComposition;

                    await _unitOfWork.MedicinRepository.UpdateAsync(SelectedMedicin);
                    await _unitOfWork.SaveAsync();

                    MedicinName = "";
                    MedicinComposition = "";
                    SelectedMedicin = null;

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Лекарство успешно обновлено", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteMedicinAsync()
        {
            try
            {
                if (SelectedMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите лекарство для удаления", "Предупреждение", MessageType.Warning);
                    return;
                }

                var result = ModernMessageDialog.ShowQuestion($"Вы уверены, что хотите удалить лекарство '{SelectedMedicin.Name}'? Все связанные записи истории пополнений также будут удалены.", "Подтверждение");
                if (result != true)
                {
                    return;
                }

                await _unitOfWork.MedicinRepository.DeleteAsync(SelectedMedicin);
                await _unitOfWork.SaveAsync();

                SelectedMedicin = null;
                MedicinName = "";
                MedicinComposition = "";

                await LoadDataAsync();
                ModernMessageDialog.Show("Лекарство успешно удалено", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddHistoryUpMedicinAsync()
        {
            try
            {
                if (SelectedMedicinForReplenishment == null)
                {
                    ModernMessageDialog.Show("Выберите лекарство для пополнения", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (!UpData.HasValue || !EndData.HasValue)
                {
                    ModernMessageDialog.Show("Выберите даты пополнения и окончания", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (Count <= 0)
                {
                    ModernMessageDialog.Show("Количество должно быть больше нуля", "Ошибка", MessageType.Error);
                    return;
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                var upDate = DateOnly.FromDateTime(UpData.Value.Date);
                var endDate = DateOnly.FromDateTime(EndData.Value.Date);
                
                var maxUpDate = today.AddDays(7);
                if (upDate > maxUpDate)
                {
                    ModernMessageDialog.Show($"Дата добавления не может быть позже {maxUpDate:dd.MM.yyyy} (7 дней от сегодня)", "Ошибка", MessageType.Error);
                    return;
                }

                var isValid = endDate >= today;

                var historyUpMedicin = new HistoryUpMedicin(Count, SelectedMedicinForReplenishment.Name, upDate, endDate, isValid);
                await _unitOfWork.HistoryUpMedicinRepository.AddAsync(historyUpMedicin);
                await _unitOfWork.SaveAsync();

                var newMedicin = new Medicin(historyUpMedicin.Id, SelectedMedicinForReplenishment.Name, SelectedMedicinForReplenishment.Composition);
                await _unitOfWork.MedicinRepository.AddAsync(newMedicin);
                await _unitOfWork.SaveAsync();

                SelectedMedicinForReplenishment = null;
                Count = 0;
                UpData = null;
                EndData = null;

                await LoadDataAsync();
                ModernMessageDialog.Show("Лекарство успешно пополнено", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при пополнении лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateHistoryUpMedicinAsync()
        {
            try
            {
                if (SelectedHistoryUpMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите запись", "Предупреждение", MessageType.Warning);
                    return;
                }

                var dialog = new Helper.Dialogs.EditMedicineDialog(SelectedHistoryUpMedicin);
                if (dialog.ShowDialog() == true)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var upDate = DateOnly.FromDateTime(dialog.UpData.Value.Date);
                    var endDate = DateOnly.FromDateTime(dialog.EndData.Value.Date);
                    var isValid = endDate >= today;

                    SelectedHistoryUpMedicin.Name = dialog.MedicineName;
                    SelectedHistoryUpMedicin.Count = dialog.Count;
                    SelectedHistoryUpMedicin.UpData = upDate;
                    SelectedHistoryUpMedicin.EndData = endDate;
                    SelectedHistoryUpMedicin.IsValid = isValid;

                    await _unitOfWork.HistoryUpMedicinRepository.UpdateAsync(SelectedHistoryUpMedicin);
                    await _unitOfWork.SaveAsync();

                    await LoadDataAsync();
                    ModernMessageDialog.Show("Лекарство успешно обновлено", "Успех", MessageType.Success);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении записи: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteHistoryUpMedicinAsync()
        {
            try
            {
                if (SelectedHistoryUpMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите запись", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.HistoryUpMedicinRepository.DeleteAsync(SelectedHistoryUpMedicin);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Запись успешно удалена", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
