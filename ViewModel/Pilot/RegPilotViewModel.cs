using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Pilot
{
    public class RegPilotViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<RegistrationUser> registrations;
        public ObservableCollection<RegistrationUser> Registrations
        {
            get => registrations;
            set => Set(ref registrations, value);
        }

        private RegistrationUser selectedRegistration;
        public RegistrationUser SelectedRegistration
        {
            get => selectedRegistration;
            set => Set(ref selectedRegistration, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand PassRegistration { get; set; }

        public RegPilotViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Registrations = new ObservableCollection<RegistrationUser>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            PassRegistration = CreateAsyncCommand(PassRegistrationAsync, () => SelectedRegistration != null && !SelectedRegistration.IsRegister);
            LoadDataAsync();
        }

        private async Task PassRegistrationAsync()
        {
            try
            {
                if (SelectedRegistration == null)
                {
                    Helper.Dialogs.ModernMessageDialog.Show("Выберите регистрацию", "Предупреждение", Helper.Dialogs.MessageType.Warning);
                    return;
                }

                if (SelectedRegistration.IsRegister)
                {
                    Helper.Dialogs.ModernMessageDialog.Show("Эта регистрация уже пройдена", "Информация", Helper.Dialogs.MessageType.Info);
                    return;
                }

                var examinations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(Session.UserId);
                var today = DateOnly.FromDateTime(DateTime.Today);
                
                var actualExaminations = examinations
                    .Where(e => e.DataEnd >= today)
                    .ToList();

                if (actualExaminations.Any(e => !e.IsValid))
                {
                    Helper.Dialogs.ModernMessageDialog.Show(
                        "Нельзя пройти регистрацию: не все актуальные медосмотры пройдены",
                        "Ошибка",
                        Helper.Dialogs.MessageType.Error);
                    return;
                }

                SelectedRegistration.IsRegister = true;
                await _unitOfWork.RegistrationUserRepository.UpdateAsync(SelectedRegistration);
                await _unitOfWork.SaveAsync();

                await LoadDataAsync();
                Helper.Dialogs.ModernMessageDialog.Show("Регистрация успешно пройдена", "Успех", Helper.Dialogs.MessageType.Success);
            }
            catch (Exception ex)
            {
                Helper.Dialogs.ModernMessageDialog.Show($"Ошибка при прохождении регистрации: {ex.Message}", "Ошибка", Helper.Dialogs.MessageType.Error);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (Session.UserId == 0)
                {
                    return;
                }

                var registrationsList = await _unitOfWork.RegistrationUserRepository.GetByUserIdAsync(Session.UserId);
                Registrations = new ObservableCollection<RegistrationUser>(registrationsList.OrderByDescending(r => r.Data));
            }
            catch (Exception ex)
            {
                Helper.Dialogs.ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", Helper.Dialogs.MessageType.Error);
            }
        }
    }
}
