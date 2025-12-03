using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Session;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.FlightAttendant
{
    public class ProcedureFaViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public class ProcedureItem
        {
            public MedicalExamination Examination { get; set; }
            public UserRoleProcedure UserRoleProcedure { get; set; }
            public Procedure Procedure { get; set; }
            public UserProcedure? UserProcedure { get; set; }
            public bool CanPass { get; set; }
        }

        private ObservableCollection<ProcedureItem> procedures;
        public ObservableCollection<ProcedureItem> Procedures
        {
            get => procedures;
            set => Set(ref procedures, value);
        }

        private ProcedureItem selectedProcedure;
        public ProcedureItem SelectedProcedure
        {
            get => selectedProcedure;
            set => Set(ref selectedProcedure, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand PassProcedure { get; set; }

        public ProcedureFaViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Procedures = new ObservableCollection<ProcedureItem>();

            LoadData = CreateAsyncCommand(LoadDataAsync);
            PassProcedure = CreateAsyncCommand<ProcedureItem>(PassProcedureAsync, (item) => item != null && item.CanPass);

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

                var today = DateOnly.FromDateTime(DateTime.Today);

                var examinations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(Session.UserId);
                
                var userProcedures = await _unitOfWork.UserProcedureRepository.GetByUserIdAsync(Session.UserId);

                var procedureItems = new List<ProcedureItem>();

                foreach (var examination in examinations)
                {
                    
                    if (examination.DataEnd < today)
                    {
                        continue;
                    }

                    var examinationUserProcedures = userProcedures
                        .Where(up => up.StartData == examination.DataStart && up.EndData == examination.DataEnd)
                        .ToList();

                    var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                    if (user == null) continue;

                    var roleProcedures = await _unitOfWork.RoleProcedureRepository.GetByRoleAsync(user.Roles);

                    foreach (var roleProcedure in roleProcedures)
                    {
                        
                        var userProcedure = examinationUserProcedures
                            .FirstOrDefault(up => up.ProcedureId == roleProcedure.ProcedureId && 
                                                  up.StartData == examination.DataStart && 
                                                  up.EndData == examination.DataEnd);

                        var procedureItem = new ProcedureItem
                        {
                            Examination = examination,
                            UserRoleProcedure = roleProcedure,
                            Procedure = roleProcedure.Procedure,
                            UserProcedure = userProcedure,
                            
                            CanPass = userProcedure == null
                        };

                        procedureItems.Add(procedureItem);
                    }
                }

                Procedures = new ObservableCollection<ProcedureItem>(procedureItems);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task PassProcedureAsync(ProcedureItem procedureItem)
        {
            try
            {
                if (procedureItem == null)
                {
                    return;
                }

                var procedure = procedureItem.Procedure;
                bool isValid = false;
                double? value = null;

                if (procedure.MustBeTrue)
                {
                    
                    isValid = true;
                }
                else
                {
                    
                    var dialog = new Helper.Dialogs.ProcedureInputDialog(
                        procedure.Name ?? "",
                        procedure.Description ?? "",
                        procedure.MinValue,
                        procedure.MaxValue,
                        procedure.Units ?? "");

                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Result.HasValue)
                        {
                            value = dialog.Result.Value;
                            
                            isValid = value >= procedure.MinValue && value <= procedure.MaxValue;
                        }
                        else
                        {
                            
                            isValid = false;
                        }
                        
                    }
                    else
                    {
                        return; 
                    }
                }

                var startDate = procedureItem.Examination.DataStart;
                var endDate = procedureItem.Examination.DataEnd;

                if (procedureItem.UserProcedure == null)
                {
                    
                    var userProcedure = new UserProcedure(
                        Session.UserId,
                        procedure.Id,
                        startDate,
                        endDate,
                        isValid);

                    await _unitOfWork.UserProcedureRepository.AddAsync(userProcedure);
                }
                else
                {
                    
                    procedureItem.UserProcedure.StartData = startDate;
                    procedureItem.UserProcedure.EndData = endDate;
                    procedureItem.UserProcedure.IsValid = isValid;

                    await _unitOfWork.UserProcedureRepository.UpdateAsync(procedureItem.UserProcedure);
                }

                await _unitOfWork.SaveAsync();

                await UpdateMedicalExaminationValidity(procedureItem.Examination);

                await LoadDataAsync();

                if (isValid)
                {
                    ModernMessageDialog.Show("Процедура успешно пройдена", "Успех", MessageType.Success);
                }
                
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при прохождении процедуры: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task UpdateMedicalExaminationValidity(MedicalExamination examination)
        {
            try
            {
                
                var userProcedures = await _unitOfWork.UserProcedureRepository.GetByUserIdAsync(Session.UserId);
                var examinationProcedures = userProcedures
                    .Where(up => up.StartData == examination.DataStart && up.EndData == examination.DataEnd)
                    .ToList();

                var allValid = examinationProcedures.Any() && examinationProcedures.All(up => up.IsValid);

                if (examination.IsValid != allValid)
                {
                    examination.IsValid = allValid;
                    await _unitOfWork.MedicalExaminationRepository.UpdateAsync(examination);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при обновлении статуса медосмотра: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
