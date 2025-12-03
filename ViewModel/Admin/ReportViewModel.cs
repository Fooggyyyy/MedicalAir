using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Admin
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private DateTime? dateStart;
        public DateTime? DateStart
        {
            get => dateStart;
            set => Set(ref dateStart, value);
        }

        private DateTime? dateEnd;
        public DateTime? DateEnd
        {
            get => dateEnd;
            set => Set(ref dateEnd, value);
        }

        private ObservableCollection<ReportUser> reportUsers;
        public ObservableCollection<ReportUser> ReportUsers
        {
            get => reportUsers;
            set => Set(ref reportUsers, value);
        }

        private ObservableCollection<ReportMedicin> reportMedicins;
        public ObservableCollection<ReportMedicin> ReportMedicins
        {
            get => reportMedicins;
            set => Set(ref reportMedicins, value);
        }

        private ReportUser selectedReportUser;
        public ReportUser SelectedReportUser
        {
            get => selectedReportUser;
            set => Set(ref selectedReportUser, value);
        }

        private ReportMedicin selectedReportMedicin;
        public ReportMedicin SelectedReportMedicin
        {
            get => selectedReportMedicin;
            set => Set(ref selectedReportMedicin, value);
        }

        public ICommand LoadReports { get; set; }

        public ReportViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ReportUsers = new ObservableCollection<ReportUser>();
            ReportMedicins = new ObservableCollection<ReportMedicin>();

            DateStart = DateTime.Now.AddDays(-30);
            DateEnd = DateTime.Now;

            LoadReports = CreateAsyncCommand(LoadReportsAsync, () => DateStart.HasValue && DateEnd.HasValue && DateStart <= DateEnd);
        }

        private async Task LoadReportsAsync()
        {
            try
            {
                if (!DateStart.HasValue || !DateEnd.HasValue)
                {
                    ModernMessageDialog.Show("Пожалуйста, выберите обе даты (от и до)", "Предупреждение", MessageType.Warning);
                    return;
                }

                if (DateStart.Value > DateEnd.Value)
                {
                    ModernMessageDialog.Show("Дата начала не может быть позже даты окончания", "Ошибка", MessageType.Error);
                    return;
                }

                var startDate = DateOnly.FromDateTime(DateStart.Value.Date);
                var endDate = DateOnly.FromDateTime(DateEnd.Value.Date);

                await GenerateUserReportAsync(startDate, endDate);
                
                var medicinsReports = await _unitOfWork.ReportMedicinRepository.GetByDateRangeAsync(startDate, endDate);
                ReportMedicins = new ObservableCollection<ReportMedicin>(medicinsReports);

                if (ReportUsers.Any() || ReportMedicins.Any())
                {
                    var message = $"Отчеты загружены за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}:\n";
                    message += $"• Отчетов по пользователям: {ReportUsers.Count()}\n";
                    message += $"• Отчетов по медикаментам: {ReportMedicins.Count()}";
                    ModernMessageDialog.Show(message, "Успех", MessageType.Success);
                }
                else
                {
                    ModernMessageDialog.Show($"За период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy} отчеты не найдены", 
                        "Информация", MessageType.Info);
                }
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке отчетов: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task GenerateUserReportAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var allUsers = await _unitOfWork.UserRepository.GetAllAsync();
                var filteredUsers = allUsers
                    .Where(u => u.Roles == UserRoles.PILOT || u.Roles == UserRoles.FLIGHTATTENDAT)
                    .Where(u => u.Roles != UserRoles.ADMIN && u.Roles != UserRoles.DOCTOR)
                    .ToList();

                if (!filteredUsers.Any())
                {
                    ModernMessageDialog.Show($"За период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy} пользователи не найдены", 
                        "Информация", MessageType.Info);
                    ReportUsers = new ObservableCollection<ReportUser>();
                    return;
                }

                var allExaminations = await _unitOfWork.MedicalExaminationRepository.GetAllAsync();
                
                var examinationsInPeriod = allExaminations
                    .Where(e => e.User != null && 
                               (e.User.Roles == UserRoles.PILOT || e.User.Roles == UserRoles.FLIGHTATTENDAT) &&
                               (e.DataStart <= endDate && e.DataEnd >= startDate))
                    .ToList();

                var totalUsers = filteredUsers.Count;
                
                var usersWithExaminations = examinationsInPeriod
                    .Select(e => e.UserId)
                    .Distinct()
                    .ToList();
                var totalUsersME = usersWithExaminations.Count;
                
                var passed = examinationsInPeriod.Count(e => e.IsValid);
                var notPassed = examinationsInPeriod.Count(e => !e.IsValid);
                
                var passedPercent = examinationsInPeriod.Any() ? (passed * 100) / examinationsInPeriod.Count : 0;
                var notPassedPercent = examinationsInPeriod.Any() ? (notPassed * 100) / examinationsInPeriod.Count : 0;

                var report = new ReportUser(
                    startDate,
                    endDate,
                    totalUsers,
                    totalUsersME,
                    passed,
                    notPassed,
                    passedPercent,
                    notPassedPercent
                );

                await _unitOfWork.ReportUserRepository.AddAsync(report);
                await _unitOfWork.SaveAsync();

                var reports = await _unitOfWork.ReportUserRepository.GetByDateRangeAsync(startDate, endDate);
                ReportUsers = new ObservableCollection<ReportUser>(reports.OrderByDescending(r => r.DataEnd));
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при генерации отчета по пользователям: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
