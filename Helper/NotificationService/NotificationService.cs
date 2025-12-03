using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;

namespace MedicalAir.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GenerateAutomaticNotificationsAsync(int userId)
        {
            try
            {
                
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null) return;

                if (user.Roles == UserRoles.PILOT || user.Roles == UserRoles.FLIGHTATTENDAT)
                {
                    await CheckCertificatExpirationAsync(userId);
                    await CheckMedicalExaminationExpirationAsync(userId);
                }

                if (user.Roles == UserRoles.FLIGHTATTENDAT)
                {
                    await CheckMedicinesExpirationAsync(userId);
                }
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Ошибка при генерации уведомлений: {ex.Message}");
            }
        }

        private async Task CheckCertificatExpirationAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekFromNow = today.AddDays(7);

            var certificats = await _unitOfWork.CertificatRepository.GetByUserIdAsync(userId);
            
            foreach (var certificat in certificats)
            {
                
                if (certificat.DataEnd >= today && certificat.DataEnd <= weekFromNow && certificat.Status != CertificatStatus.EXPIRED)
                {
                    var message = $"⚠️ Ваш сертификат истекает {certificat.DataEnd:dd.MM.yyyy}. Пожалуйста, обновите его.";
                    
                    if (!await _unitOfWork.NotificationRepository.ExistsSimilarAsync(userId, message, 1))
                    {
                        var notification = new Notification(userId, message);
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                    }
                }
            }
        }

        private async Task CheckMedicalExaminationExpirationAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekFromNow = today.AddDays(7);

            var examinations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(userId);
            
            foreach (var examination in examinations)
            {
                
                if (examination.DataEnd >= today && examination.DataEnd <= weekFromNow)
                {
                    var procedureName = examination.UserRoleProcedure?.Procedure?.Name ?? "медосмотр";
                    var message = $"🏥 Напоминание: необходимо пройти медосмотр '{procedureName}' до {examination.DataEnd:dd.MM.yyyy}.";
                    
                    if (!await _unitOfWork.NotificationRepository.ExistsSimilarAsync(userId, message, 1))
                    {
                        var notification = new Notification(userId, message);
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                    }
                }
            }
        }

        private async Task CheckMedicinesExpirationAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekFromNow = today.AddDays(7);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user?.AirplaneId == null) return;

            var medkits = await _unitOfWork.MedkitRepository.GetByAirplaneIdAsync(user.AirplaneId.Value);
            
            foreach (var medkit in medkits)
            {
                if (medkit.Medicins == null) continue;

                foreach (var medicin in medkit.Medicins)
                {
                    if (medicin.HistoryUpMedicin == null) continue;

                    var endDate = medicin.HistoryUpMedicin.EndData;

                    if (endDate >= today && endDate <= weekFromNow)
                    {
                        var message = $"💊 Внимание! Лекарство '{medicin.Name}' в аптечке '{medkit.NameMedkit ?? "Аптечка"}' истекает {endDate:dd.MM.yyyy}. Необходимо заменить.";
                        
                        if (!await _unitOfWork.NotificationRepository.ExistsSimilarAsync(userId, message, 1))
                        {
                            var notification = new Notification(userId, message);
                            await _unitOfWork.NotificationRepository.AddAsync(notification);
                        }
                    }
                }
            }
        }
    }
}
