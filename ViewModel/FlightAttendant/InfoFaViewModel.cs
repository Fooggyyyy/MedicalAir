using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalAir.ViewModel.FlightAttendant
{
    public class InfoFaViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private ObservableCollection<MedicalExamination> medicalExaminations;
        public ObservableCollection<MedicalExamination> MedicalExaminations
        {
            get => medicalExaminations;
            set => Set(ref medicalExaminations, value);
        }

        private ObservableCollection<Certificat> certificats;
        public ObservableCollection<Certificat> Certificats
        {
            get => certificats;
            set => Set(ref certificats, value);
        }

        private MedicalExamination selectedMedicalExamination;
        public MedicalExamination SelectedMedicalExamination
        {
            get => selectedMedicalExamination;
            set => Set(ref selectedMedicalExamination, value);
        }

        private ObservableCollection<UserProcedure> allUserProcedures;
        public ObservableCollection<UserProcedure> AllUserProcedures
        {
            get => allUserProcedures;
            set => Set(ref allUserProcedures, value);
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

        public InfoFaViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            MedicalExaminations = new ObservableCollection<MedicalExamination>();
            Certificats = new ObservableCollection<Certificat>();
            AllUserProcedures = new ObservableCollection<UserProcedure>();
            examinationProceduresMap = new Dictionary<int, string>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
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

                // Загружаем медосмотры пользователя
                var examinations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(Session.UserId);
                
                // Загружаем процедуры для каждого медосмотра ПЕРЕД установкой MedicalExaminations
                var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                if (user != null)
                {
                    examinationProceduresMap.Clear();
                    var roleProcedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(user.Roles);
                    
                    foreach (var examination in examinations)
                    {
                        var procedureNames = roleProcedures
                            .Where(rp => rp.Procedure != null)
                            .Select(rp => rp.Procedure.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .ToList();
                        
                        examinationProceduresMap[examination.Id] = string.Join(", ", procedureNames);
                    }
                }
                
                MedicalExaminations = new ObservableCollection<MedicalExamination>(examinations.OrderByDescending(e => e.DataEnd));

                // Загружаем все процедуры пользователя
                var userProcedures = await _unitOfWork.UserProcedureRepository.GetByUserIdAsync(Session.UserId);
                AllUserProcedures = new ObservableCollection<UserProcedure>(userProcedures.OrderByDescending(up => up.EndData));

                // Загружаем сертификаты пользователя
                var certificatsList = await _unitOfWork.CertificatRepository.GetByUserIdAsync(Session.UserId);
                Certificats = new ObservableCollection<Certificat>(certificatsList.OrderByDescending(c => c.DataEnd));
            }
            catch (Exception ex)
            {
                Helper.Dialogs.ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", Helper.Dialogs.MessageType.Error);
            }
        }

    }
}
