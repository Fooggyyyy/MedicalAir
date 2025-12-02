using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Helper.ViewModelBase;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using MedicalAir.Model.Session;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalAir.ViewModel.Doctor
{
    public class MainDoctorViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private User currentUser;
        public User CurrentUser
        {
            get => currentUser;
            set => Set(ref currentUser, value);
        }

        private int activeCertificatsCount;
        public int ActiveCertificatsCount
        {
            get => activeCertificatsCount;
            set => Set(ref activeCertificatsCount, value);
        }

        private int expiredCertificatsCount;
        public int ExpiredCertificatsCount
        {
            get => expiredCertificatsCount;
            set => Set(ref expiredCertificatsCount, value);
        }

        private int almostExpiredCertificatsCount;
        public int AlmostExpiredCertificatsCount
        {
            get => almostExpiredCertificatsCount;
            set => Set(ref almostExpiredCertificatsCount, value);
        }

        private int validExaminationsCount;
        public int ValidExaminationsCount
        {
            get => validExaminationsCount;
            set => Set(ref validExaminationsCount, value);
        }

        private int invalidExaminationsCount;
        public int InvalidExaminationsCount
        {
            get => invalidExaminationsCount;
            set => Set(ref invalidExaminationsCount, value);
        }

        private int proceduresCount;
        public int ProceduresCount
        {
            get => proceduresCount;
            set => Set(ref proceduresCount, value);
        }

        private int medicinesCount;
        public int MedicinesCount
        {
            get => medicinesCount;
            set => Set(ref medicinesCount, value);
        }

        private int pilotsCount;
        public int PilotsCount
        {
            get => pilotsCount;
            set => Set(ref pilotsCount, value);
        }

        private int flightAttendantsCount;
        public int FlightAttendantsCount
        {
            get => flightAttendantsCount;
            set => Set(ref flightAttendantsCount, value);
        }

        public ICommand LoadData { get; set; }
        public ICommand RefreshData { get; set; }

        public MainDoctorViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            LoadData = CreateAsyncCommand(LoadDataAsync);
            RefreshData = CreateAsyncCommand(LoadDataAsync);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                if (Session.UserId == 0)
                {
                    ModernMessageDialog.Show("Пользователь не авторизован", "Ошибка", MessageType.Error);
                    return;
                }

                // Загружаем информацию о враче
                var user = await _unitOfWork.UserRepository.GetByIdAsync(Session.UserId);
                if (user == null)
                {
                    ModernMessageDialog.Show("Пользователь не найден", "Ошибка", MessageType.Error);
                    return;
                }

                CurrentUser = user;

                // Загружаем статистику по сертификатам
                var allCertificats = await _unitOfWork.CertificatRepository.GetAllAsync();
                ActiveCertificatsCount = allCertificats.Count(c => c.Status == CertificatStatus.ACTIVE);
                ExpiredCertificatsCount = allCertificats.Count(c => c.Status == CertificatStatus.EXPIRED);
                AlmostExpiredCertificatsCount = allCertificats.Count(c => c.Status == CertificatStatus.ALMOSTEXPIRED);

                // Загружаем статистику по медосмотрам
                var allExaminations = await _unitOfWork.MedicalExaminationRepository.GetAllAsync();
                ValidExaminationsCount = allExaminations.Count(e => e.IsValid);
                InvalidExaminationsCount = allExaminations.Count(e => !e.IsValid);

                // Загружаем статистику по процедурам
                var allProcedures = await _unitOfWork.ProcedureRepository.GetAllAsync();
                ProceduresCount = allProcedures.Count();

                // Загружаем статистику по лекарствам
                var allMedicines = await _unitOfWork.HistoryUpMedicinRepository.GetAllAsync();
                MedicinesCount = allMedicines.Count();

                // Загружаем статистику по пользователям
                var pilots = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.PILOT);
                PilotsCount = pilots.Count();

                var flightAttendants = await _unitOfWork.UserRepository.GetByRoleAsync(UserRoles.FLIGHTATTENDAT);
                FlightAttendantsCount = flightAttendants.Count();
            }
            catch (Exception ex)
            {
                ModernMessageDialog.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageType.Error);
            }
        }
    }
}
