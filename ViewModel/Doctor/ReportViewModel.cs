using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
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

        private ObservableCollection<ReportMedicin> reportMedicins;
        public ObservableCollection<ReportMedicin> ReportMedicins
        {
            get => reportMedicins;
            set => Set(ref reportMedicins, value);
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

                await GenerateReportAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при генерации отчетов: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }

        private async Task GenerateReportAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                
                var allMedicins = await _unitOfWork.MedicinRepository.GetAllAsync();
                var allHistory = await _unitOfWork.HistoryUpMedicinRepository.GetAllAsync();
                var allMedkits = await _unitOfWork.MedkitRepository.GetAllAsync();

                var medicinsInRange = allHistory
                    .Where(h => h.UpData >= startDate && h.UpData <= endDate)
                    .ToList();

                var medicinsActiveInRange = allHistory
                    .Where(h => (h.UpData <= endDate && h.EndData >= startDate))
                    .ToList();

                var allMedicinsInPeriod = medicinsActiveInRange
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    .ToList();

                if (!allMedicinsInPeriod.Any())
                {
                    ModernMessageDialog.Show($"За период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy} лекарства не найдены", 
                        "Информация", MessageType.Info);
                    ReportMedicins = new ObservableCollection<ReportMedicin>();
                    return;
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                var weekFromNow = today.AddDays(7);

                var totalMedicines = allMedicinsInPeriod.Sum(m => m.Count);
                var totalUniqueMedicines = allMedicinsInPeriod.Select(m => m.Name).Distinct().Count();
                var totalMedkit = allMedkits.Count();
                
                var expiredCount = allMedicinsInPeriod.Count(m => m.EndData < today);
                var almostExpiredCount = allMedicinsInPeriod.Count(m => m.EndData >= today && m.EndData <= weekFromNow);
                
                var expiredCountPercent = allMedicinsInPeriod.Count > 0 ? (expiredCount * 100) / allMedicinsInPeriod.Count : 0;
                var almostExpiredCountPercent = allMedicinsInPeriod.Count > 0 ? (almostExpiredCount * 100) / allMedicinsInPeriod.Count : 0;

                var report = new ReportMedicin(
                    startDate,
                    endDate,
                    totalMedicines,
                    totalUniqueMedicines,
                    totalMedkit,
                    expiredCount,
                    almostExpiredCount,
                    expiredCountPercent,
                    almostExpiredCountPercent
                );

                await _unitOfWork.ReportMedicinRepository.AddAsync(report);
                await _unitOfWork.SaveAsync();

                var reports = await _unitOfWork.ReportMedicinRepository.GetByDateRangeAsync(startDate, endDate);
                ReportMedicins = new ObservableCollection<ReportMedicin>(reports.OrderByDescending(r => r.DataEnd));

                ModernMessageDialog.Show("Отчет успешно создан", "Успех", MessageType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при генерации отчета: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
