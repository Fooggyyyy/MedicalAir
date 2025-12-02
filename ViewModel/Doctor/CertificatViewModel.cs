using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class CertificatViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<Certificat> certificats;
        private ObservableCollection<Certificat> allCertificats;
        public ObservableCollection<Certificat> Certificats
        {
            get => certificats;
            set => Set(ref certificats, value);
        }

        private string searchCertificats;
        public string SearchCertificats
        {
            get => searchCertificats;
            set
            {
                Set(ref searchCertificats, value);
                FilterCertificats();
            }
        }

        private ObservableCollection<User> pilotsAndAttendants;
        public ObservableCollection<User> PilotsAndAttendants
        {
            get => pilotsAndAttendants;
            set => Set(ref pilotsAndAttendants, value);
        }

        private Certificat selectedCertificat;
        public Certificat SelectedCertificat
        {
            get => selectedCertificat;
            set => Set(ref selectedCertificat, value);
        }

        private User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set => Set(ref selectedUser, value);
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

        public ICommand LoadData { get; set; }
        public ICommand AddCertificat { get; set; }
        public ICommand UpdateCertificat { get; set; }
        public ICommand DeleteCertificat { get; set; }
        public ICommand UpdateStatuses { get; set; }
        public ICommand ClearSearchCertificats { get; set; }

        public CertificatViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Certificats = new ObservableCollection<Certificat>();
            PilotsAndAttendants = new ObservableCollection<User>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            AddCertificat = CreateAsyncCommand(AddCertificatAsync, () => SelectedUser != null && DataStart.HasValue && DataEnd.HasValue && DataStart <= DataEnd);
            UpdateCertificat = CreateAsyncCommand(UpdateCertificatAsync, () => SelectedCertificat != null && DataStart.HasValue && DataEnd.HasValue && DataStart <= DataEnd);
            DeleteCertificat = CreateAsyncCommand(DeleteCertificatAsync, () => SelectedCertificat != null);
            UpdateStatuses = CreateAsyncCommand(UpdateStatusesAsync);
            ClearSearchCertificats = CreateCommand(_ => { SearchCertificats = ""; });

            InitializeAsync();
        }

        private void FilterCertificats()
        {
            if (allCertificats == null) return;

            if (string.IsNullOrWhiteSpace(SearchCertificats))
            {
                Certificats = new ObservableCollection<Certificat>(allCertificats);
            }
            else
            {
                var searchLower = SearchCertificats.ToLower();
                var filtered = allCertificats.Where(c => 
                    (c.User?.FullName?.ToLower().Contains(searchLower) ?? false) ||
                    c.DataStart.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    c.DataEnd.ToString("dd.MM.yyyy").Contains(searchLower) ||
                    c.Status.ToString().ToLower().Contains(searchLower) ||
                    c.Id.ToString().Contains(searchLower)
                ).ToList();
                Certificats = new ObservableCollection<Certificat>(filtered);
            }
        }

        private async void InitializeAsync()
        {
            await UpdateStatusesAsync();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var certificatsList = await _unitOfWork.CertificatRepository.GetAllAsync();
                allCertificats = new ObservableCollection<Certificat>(certificatsList.OrderByDescending(c => c.DataEnd));
                FilterCertificats();

                var pilots = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.PILOT);
                var flightAttendants = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.FLIGHTATTENDAT);
                
                var allUsers = pilots.Concat(flightAttendants).ToList();
                PilotsAndAttendants = new ObservableCollection<User>(allUsers);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task AddCertificatAsync()
        {
            try
            {
                if (SelectedUser == null)
                {
                    ModernMessageDialog.Show("Выберите пользователя", "Предупреждение", MessageType.Warning);
                    return;
                }

                // Проверяем, прошел ли пользователь медосмотр
                var userExaminations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(SelectedUser.Id);
                var hasValidExamination = userExaminations.Any(e => e.IsValid);
                
                if (!hasValidExamination)
                {
                    ModernMessageDialog.Show("Нельзя создать сертификат для пользователя, который не прошел медосмотр", "Ошибка", MessageType.Error);
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

                var startDate = DateOnly.FromDateTime(DataStart.Value.Date);
                var endDate = DateOnly.FromDateTime(DataEnd.Value.Date);

                var certificat = new Certificat(SelectedUser.Id, startDate, endDate, CertificatStatus.ACTIVE);
                await _unitOfWork.CertificatRepository.AddAsync(certificat);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Сертификат успешно создан", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при создании сертификата: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateCertificatAsync()
        {
            try
            {
                if (SelectedCertificat == null)
                {
                    ModernMessageDialog.Show("Выберите сертификат", "Предупреждение", MessageType.Warning);
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

                SelectedCertificat.DataStart = DateOnly.FromDateTime(DataStart.Value.Date);
                SelectedCertificat.DataEnd = DateOnly.FromDateTime(DataEnd.Value.Date);
                
                // Обновляем статус
                await UpdateCertificatStatus(SelectedCertificat);
                
                await _unitOfWork.CertificatRepository.UpdateAsync(SelectedCertificat);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Сертификат успешно обновлен", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении сертификата: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task DeleteCertificatAsync()
        {
            try
            {
                if (SelectedCertificat == null)
                {
                    ModernMessageDialog.Show("Выберите сертификат", "Предупреждение", MessageType.Warning);
                    return;
                }

                await _unitOfWork.CertificatRepository.DeleteAsync(SelectedCertificat);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                ModernMessageDialog.Show("Сертификат успешно удален", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при удалении сертификата: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateStatusesAsync()
        {
            try
            {
                var allCertificats = await _unitOfWork.CertificatRepository.GetAllAsync();
                var today = DateOnly.FromDateTime(DateTime.Today);
                bool hasChanges = false;

                foreach (var certificat in allCertificats)
                {
                    var oldStatus = certificat.Status;
                    
                    if (certificat.DataEnd < today)
                    {
                        certificat.Status = CertificatStatus.EXPIRED;
                    }
                    else if (certificat.DataEnd <= today.AddDays(7))
                    {
                        certificat.Status = CertificatStatus.ALMOSTEXPIRED;
                    }
                    else
                    {
                        certificat.Status = CertificatStatus.ACTIVE;
                    }

                    if (oldStatus != certificat.Status)
                    {
                        await _unitOfWork.CertificatRepository.UpdateAsync(certificat);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении статусов: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateCertificatStatus(Certificat certificat)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            if (certificat.DataEnd < today)
            {
                certificat.Status = CertificatStatus.EXPIRED;
            }
            else if (certificat.DataEnd <= today.AddDays(7))
            {
                certificat.Status = CertificatStatus.ALMOSTEXPIRED;
            }
            else
            {
                certificat.Status = CertificatStatus.ACTIVE;
            }
        }
    }
}
