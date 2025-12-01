using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

            // Устанавливаем значения по умолчанию (последние 30 дней)
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

                var usersReports = await _unitOfWork.ReportUserRepository.GetByDateRangeAsync(startDate, endDate);
                var medicinsReports = await _unitOfWork.ReportMedicinRepository.GetByDateRangeAsync(startDate, endDate);

                ReportUsers = new ObservableCollection<ReportUser>(usersReports);
                ReportMedicins = new ObservableCollection<ReportMedicin>(medicinsReports);

                if (!ReportUsers.Any() && !ReportMedicins.Any())
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
    }
}
