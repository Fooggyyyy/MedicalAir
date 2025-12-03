using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.FlightAttendant
{
    public class MedicamentsFaViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<Medkit> medkits;
        public ObservableCollection<Medkit> Medkits
        {
            get => medkits;
            set => Set(ref medkits, value);
        }

        private ObservableCollection<Medicin> availableMedicins;
        public ObservableCollection<Medicin> AvailableMedicins
        {
            get => availableMedicins;
            set => Set(ref availableMedicins, value);
        }

        private Medkit selectedMedkit;
        public Medkit SelectedMedkit
        {
            get => selectedMedkit;
            set
            {
                if (Set(ref selectedMedkit, value))
                {
                    
                    LoadMedkitMedicinsAsync();
                }
            }
        }

        private async Task LoadMedkitMedicinsAsync()
        {
            try
            {
                if (SelectedMedkit != null)
                {
                    
                    var medkitWithMedicins = await _unitOfWork.MedkitRepository.GetWithMedicinsAsync(SelectedMedkit.Id);
                    if (medkitWithMedicins != null)
                    {
                        
                        selectedMedkit = medkitWithMedicins;
                        OnPropertyChanged(nameof(SelectedMedkit));
                    }
                    
                    await RefreshAvailableMedicinsAsync();
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке медикаментов: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task RefreshAvailableMedicinsAsync()
        {
            try
            {
                var allMedicins = await _unitOfWork.MedicinRepository.GetAllAsync();
                var today = DateOnly.FromDateTime(DateTime.Today);
                
                var medkitMedicinIds = new HashSet<int>();
                if (SelectedMedkit != null)
                {
                    var medkitWithMedicins = await _unitOfWork.MedkitRepository.GetWithMedicinsAsync(SelectedMedkit.Id);
                    if (medkitWithMedicins?.Medicins != null)
                    {
                        medkitMedicinIds = new HashSet<int>(medkitWithMedicins.Medicins.Select(m => m.Id));
                    }
                }
                
                var validMedicins = allMedicins
                    .Where(m => m.HistoryUpMedicin != null && 
                                m.HistoryUpMedicin.IsValid && 
                                m.HistoryUpMedicin.EndData >= today &&
                                !medkitMedicinIds.Contains(m.Id))
                    .GroupBy(m => m.Name) 
                    .Select(g => g.First()) 
                    .ToList();

                AvailableMedicins = new ObservableCollection<Medicin>(validMedicins);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении списка лекарств: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private Medicin selectedMedicin;
        public Medicin SelectedMedicin
        {
            get => selectedMedicin;
            set => Set(ref selectedMedicin, value);
        }

        private string medkitName;
        public string MedkitName
        {
            get => medkitName;
            set => Set(ref medkitName, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand AddMedkit { get; set; }
        public ICommand DeleteMedkit { get; set; }
        public ICommand AddMedicinToMedkit { get; set; }
        public ICommand RemoveMedicinFromMedkit { get; set; }
        public ICommand DeleteExpiredMedicins { get; set; }
        public ICommand UpdateMedkitValidity { get; set; }

        public MedicamentsFaViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Medkits = new ObservableCollection<Medkit>();
            AvailableMedicins = new ObservableCollection<Medicin>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            AddMedkit = CreateAsyncCommand(AddMedkitAsync);
            DeleteMedkit = CreateAsyncCommand(DeleteMedkitAsync, () => SelectedMedkit != null);
            AddMedicinToMedkit = CreateAsyncCommand(AddMedicinToMedkitAsync, () => SelectedMedkit != null && SelectedMedicin != null);
            RemoveMedicinFromMedkit = CreateAsyncCommand(RemoveMedicinFromMedkitAsync, () => SelectedMedkit != null && SelectedMedicin != null);
            DeleteExpiredMedicins = CreateAsyncCommand(DeleteExpiredMedicinsAsync);
            UpdateMedkitValidity = CreateAsyncCommand(UpdateMedkitValidityAsync);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (Session.UserId == 0)
                {
                    return;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                if (user == null || !user.AirplaneId.HasValue)
                {
                    return;
                }

                var medkitsList = await _unitOfWork.MedkitRepository.GetByAirplaneIdAsync(user.AirplaneId.Value);
                Medkits = new ObservableCollection<Medkit>(medkitsList);

                await RefreshAvailableMedicinsAsync();

                await UpdateMedkitValidityAsync();
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddMedkitAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MedkitName))
                {
                    ModernMessageDialog.Show("Введите название аптечки", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (Session.UserId == 0)
                {
                    return;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                if (user == null || !user.AirplaneId.HasValue)
                {
                    ModernMessageDialog.Show("Пользователь не назначен на самолет", "Ошибка", MessageType.Error);
                    return;
                }

                var medkit = new Medkit(user.AirplaneId.Value, MedkitName, false);
                await _unitOfWork.MedkitRepository.AddAsync(medkit);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                MedkitName = "";
                ModernMessageDialog.Show("Аптечка успешно создана", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании аптечки: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteMedkitAsync()
        {
            try
            {
                if (SelectedMedkit == null)
                {
                    ModernMessageDialog.Show("Выберите аптечку", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.MedkitRepository.DeleteAsync(SelectedMedkit);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Аптечка успешно удалена", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении аптечки: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddMedicinToMedkitAsync()
        {
            try
            {
                if (SelectedMedkit == null || SelectedMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите аптечку и лекарство", "Предупреждение", MessageType.Warning);
                    return;
                }

                var medkitWithMedicins = await _unitOfWork.MedkitRepository.GetWithMedicinsAsync(SelectedMedkit.Id);
                if (medkitWithMedicins == null)
                {
                    ModernMessageDialog.Show("Аптечка не найдена", "Ошибка", MessageType.Error);
                    return;
                }

                if (medkitWithMedicins.Medicins != null && medkitWithMedicins.Medicins.Any(m => m.Id == SelectedMedicin.Id))
                {
                    ModernMessageDialog.Show("Это лекарство уже добавлено в аптечку", "Предупреждение", MessageType.Warning);
                    return;
                }

                var medicin = await _unitOfWork.MedicinRepository.GetByIdAsync(SelectedMedicin.Id);
                if (medicin == null)
                {
                    ModernMessageDialog.Show("Лекарство не найдено", "Ошибка", MessageType.Error);
                    return;
                }

                var historyUpMedicin = await _unitOfWork.HistoryUpMedicinRepository.GetByIdAsync(medicin.HistoryUpMId);
                if (historyUpMedicin == null)
                {
                    ModernMessageDialog.Show("История пополнения лекарства не найдена", "Ошибка", MessageType.Error);
                    return;
                }

                if (historyUpMedicin.Count <= 0)
                {
                    ModernMessageDialog.Show("Нельзя взять лекарство: количество равно 0", "Ошибка", MessageType.Error);
                    return;
                }

                historyUpMedicin.Count -= 1;
                await _unitOfWork.HistoryUpMedicinRepository.UpdateAsync(historyUpMedicin);

                if (medkitWithMedicins.Medicins == null)
                {
                    medkitWithMedicins.Medicins = new List<Medicin>();
                }
                
                medkitWithMedicins.Medicins.Add(medicin);

                await _unitOfWork.MedkitRepository.UpdateAsync(medkitWithMedicins);
                await _unitOfWork.SaveAsync();

                await LoadMedkitMedicinsAsync();
                await UpdateMedkitValidityAsync();
                ModernMessageDialog.Show("Лекарство успешно добавлено в аптечку", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при добавлении лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task RemoveMedicinFromMedkitAsync()
        {
            try
            {
                if (SelectedMedkit == null || SelectedMedicin == null)
                {
                    ModernMessageDialog.Show("Выберите аптечку и лекарство", "Предупреждение", MessageType.Warning);
                    return;
                }

                var medkitWithMedicins = await _unitOfWork.MedkitRepository.GetWithMedicinsAsync(SelectedMedkit.Id);
                if (medkitWithMedicins == null || medkitWithMedicins.Medicins == null)
                {
                    ModernMessageDialog.Show("Аптечка не найдена или пуста", "Ошибка", MessageType.Error);
                    return;
                }

                var medicinToRemove = medkitWithMedicins.Medicins.FirstOrDefault(m => m.Id == SelectedMedicin.Id);
                if (medicinToRemove == null)
                {
                    ModernMessageDialog.Show("Лекарство не найдено в аптечке", "Предупреждение", MessageType.Warning);
                    return;
                }

                medkitWithMedicins.Medicins.Remove(medicinToRemove);
                await _unitOfWork.MedkitRepository.UpdateAsync(medkitWithMedicins);
                await _unitOfWork.SaveAsync();

                await LoadMedkitMedicinsAsync();
                await UpdateMedkitValidityAsync();
                ModernMessageDialog.Show("Лекарство успешно удалено из аптечки", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении лекарства: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteExpiredMedicinsAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var allMedicins = await _unitOfWork.MedicinRepository.GetAllAsync();
                
                var expiredMedicins = allMedicins
                    .Where(m => m.HistoryUpMedicin != null && m.HistoryUpMedicin.EndData < today)
                    .ToList();

                if (!expiredMedicins.Any())
                {
                    ModernMessageDialog.Show("Просроченных лекарств не найдено", "Информация", MessageType.Info);
                    return;
                }

                foreach (var medicin in expiredMedicins)
                {
                    await _unitOfWork.MedicinRepository.DeleteAsync(medicin);
                }

                await _unitOfWork.SaveAsync();
                await LoadDataAsync();
                await UpdateMedkitValidityAsync();
                ModernMessageDialog.Show($"Удалено {expiredMedicins.Count} просроченных лекарств", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении просроченных лекарств: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateMedkitValidityAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                bool hasChanges = false;

                foreach (var medkit in Medkits)
                {
                    var medkitWithMedicins = await _unitOfWork.MedkitRepository.GetWithMedicinsAsync(medkit.Id);
                    if (medkitWithMedicins == null || medkitWithMedicins.Medicins == null || !medkitWithMedicins.Medicins.Any())
                    {
                        if (medkit.IsValid != false)
                        {
                            medkit.IsValid = false;
                            await _unitOfWork.MedkitRepository.UpdateAsync(medkit);
                            hasChanges = true;
                        }
                        continue;
                    }

                    var validMedicins = medkitWithMedicins.Medicins
                        .Where(m => m.HistoryUpMedicin != null && 
                                   m.HistoryUpMedicin.EndData >= today && 
                                   m.HistoryUpMedicin.IsValid)
                        .ToList();

                    var uniqueMedicinsCount = validMedicins
                        .Select(m => m.HistoryUpMId)
                        .Distinct()
                        .Count();

                    var isValid = validMedicins.Count == medkitWithMedicins.Medicins.Count && 
                                 uniqueMedicinsCount >= 5;

                    if (medkit.IsValid != isValid)
                    {
                        medkit.IsValid = isValid;
                        await _unitOfWork.MedkitRepository.UpdateAsync(medkit);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    await _unitOfWork.SaveAsync();
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении валидности аптечек: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
