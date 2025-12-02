using MedicalAir.DataBase.UnitOfWork;
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
    public class InfoViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<HistoryUpMedicin> historyUpMedicins;
        public ObservableCollection<HistoryUpMedicin> HistoryUpMedicins
        {
            get => historyUpMedicins;
            set => Set(ref historyUpMedicins, value);
        }

        private ObservableCollection<MedicalExamination> validMedicalExaminations;
        public ObservableCollection<MedicalExamination> ValidMedicalExaminations
        {
            get => validMedicalExaminations;
            set => Set(ref validMedicalExaminations, value);
        }

        private ObservableCollection<MedicalExamination> invalidMedicalExaminations;
        public ObservableCollection<MedicalExamination> InvalidMedicalExaminations
        {
            get => invalidMedicalExaminations;
            set => Set(ref invalidMedicalExaminations, value);
        }

        private ObservableCollection<MedicalExamination> pilotValidExaminations;
        public ObservableCollection<MedicalExamination> PilotValidExaminations
        {
            get => pilotValidExaminations;
            set => Set(ref pilotValidExaminations, value);
        }

        private ObservableCollection<MedicalExamination> pilotInvalidExaminations;
        public ObservableCollection<MedicalExamination> PilotInvalidExaminations
        {
            get => pilotInvalidExaminations;
            set => Set(ref pilotInvalidExaminations, value);
        }

        private ObservableCollection<MedicalExamination> flightAttendantValidExaminations;
        public ObservableCollection<MedicalExamination> FlightAttendantValidExaminations
        {
            get => flightAttendantValidExaminations;
            set => Set(ref flightAttendantValidExaminations, value);
        }

        private ObservableCollection<MedicalExamination> flightAttendantInvalidExaminations;
        public ObservableCollection<MedicalExamination> FlightAttendantInvalidExaminations
        {
            get => flightAttendantInvalidExaminations;
            set => Set(ref flightAttendantInvalidExaminations, value);
        }

        private Dictionary<int, string> examinationProceduresMap;
        public Dictionary<int, string> ExaminationProceduresMap
        {
            get => examinationProceduresMap;
            set => Set(ref examinationProceduresMap, value);
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

        public ICommand LoadData { get; set; }

        public InfoViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            HistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>();
            ValidMedicalExaminations = new ObservableCollection<MedicalExamination>();
            InvalidMedicalExaminations = new ObservableCollection<MedicalExamination>();
            PilotValidExaminations = new ObservableCollection<MedicalExamination>();
            PilotInvalidExaminations = new ObservableCollection<MedicalExamination>();
            FlightAttendantValidExaminations = new ObservableCollection<MedicalExamination>();
            FlightAttendantInvalidExaminations = new ObservableCollection<MedicalExamination>();
            examinationProceduresMap = new Dictionary<int, string>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Загружаем историю пополнений лекарств
                var historyList = await _unitOfWork.HistoryUpMedicinRepository.GetAllAsync();
                HistoryUpMedicins = new ObservableCollection<HistoryUpMedicin>(historyList.OrderByDescending(h => h.UpData));

                // Загружаем все медосмотры
                var allExaminations = await _unitOfWork.MedicalExaminationRepository.GetAllAsync();
                
                // Разделяем на пройденные и не пройденные
                var valid = allExaminations.Where(e => e.IsValid).ToList();
                var invalid = allExaminations.Where(e => !e.IsValid).ToList();
                
                ValidMedicalExaminations = new ObservableCollection<MedicalExamination>(valid);
                InvalidMedicalExaminations = new ObservableCollection<MedicalExamination>(invalid);

                // Получаем пилотов и бортпроводников
                var pilots = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.PILOT);
                var flightAttendants = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.FLIGHTATTENDAT);

                var pilotIds = pilots.Select(p => p.Id).ToList();
                var flightAttendantIds = flightAttendants.Select(fa => fa.Id).ToList();

                // Медосмотры пилотов
                var pilotValid = allExaminations.Where(e => pilotIds.Contains(e.UserId) && e.IsValid).ToList();
                var pilotInvalid = allExaminations.Where(e => pilotIds.Contains(e.UserId) && !e.IsValid).ToList();
                
                PilotValidExaminations = new ObservableCollection<MedicalExamination>(pilotValid);
                PilotInvalidExaminations = new ObservableCollection<MedicalExamination>(pilotInvalid);

                // Медосмотры бортпроводников
                var faValid = allExaminations.Where(e => flightAttendantIds.Contains(e.UserId) && e.IsValid).ToList();
                var faInvalid = allExaminations.Where(e => flightAttendantIds.Contains(e.UserId) && !e.IsValid).ToList();
                
                FlightAttendantValidExaminations = new ObservableCollection<MedicalExamination>(faValid);
                FlightAttendantInvalidExaminations = new ObservableCollection<MedicalExamination>(faInvalid);

                // Загружаем процедуры для каждого медосмотра
                examinationProceduresMap.Clear();
                foreach (var examination in allExaminations)
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
            }
            catch (Exception ex)
            {
                Helper.Dialogs.ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", Helper.Dialogs.MessageType.Error);
            }
        }
    }
}
