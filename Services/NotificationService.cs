using MedicalAir.DataBase.UnitOfWork;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                // Получаем информацию о пользователе
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null) return;

                // Генерируем уведомления для всех типов пользователей (пилот и бортпроводник)
                if (user.Roles == UserRoles.PILOT || user.Roles == UserRoles.FLIGHTATTENDAT)
                {
                    await CheckCertificatExpirationAsync(userId);
                    await CheckMedicalExaminationExpirationAsync(userId);
                }

                // Дополнительные уведомления только для бортпроводников
                if (user.Roles == UserRoles.FLIGHTATTENDAT)
                {
                    await CheckMedicinesExpirationAsync(userId);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                System.Diagnostics.Debug.WriteLine($"Ошибка при генерации уведомлений: {ex.Message}");
            }
        }

        private async Task CheckCertificatExpirationAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekFromNow = today.AddDays(7);

            // Получаем все сертификаты пользователя
            var certificats = await _unitOfWork.CertificatRepository.GetByUserIdAsync(userId);
            
            foreach (var certificat in certificats)
            {
                // Проверяем, истекает ли сертификат в течение недели
                if (certificat.DataEnd >= today && certificat.DataEnd <= weekFromNow && certificat.Status != CertificatStatus.EXPIRED)
                {
                    var message = $"⚠️ Ваш сертификат истекает {certificat.DataEnd:dd.MM.yyyy}. Пожалуйста, обновите его.";
                    
                    // Проверяем, не создано ли уже такое уведомление за последние 24 часа
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

            // Получаем все медосмотры пользователя
            var examinations = await _unitOfWork.MedicalExaminationRepository.GetByUserIdAsync(userId);
            
            foreach (var examination in examinations)
            {
                // Проверяем, истекает ли медосмотр в течение недели
                if (examination.DataEnd >= today && examination.DataEnd <= weekFromNow)
                {
                    var procedureName = examination.UserRoleProcedure?.Procedure?.Name ?? "медосмотр";
                    var message = $"🏥 Напоминание: необходимо пройти медосмотр '{procedureName}' до {examination.DataEnd:dd.MM.yyyy}.";
                    
                    // Проверяем, не создано ли уже такое уведомление за последние 24 часа
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

            // Получаем информацию о пользователе с самолетом
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user?.AirplaneId == null) return;

            // Получаем все аптечки на самолете пользователя
            var medkits = await _unitOfWork.MedkitRepository.GetByAirplaneIdAsync(user.AirplaneId.Value);
            
            foreach (var medkit in medkits)
            {
                if (medkit.Medicins == null) continue;

                foreach (var medicin in medkit.Medicins)
                {
                    if (medicin.HistoryUpMedicin == null) continue;

                    var endDate = medicin.HistoryUpMedicin.EndData;

                    // Проверяем, истекает ли срок годности лекарства в течение недели
                    if (endDate >= today && endDate <= weekFromNow)
                    {
                        var message = $"💊 Внимание! Лекарство '{medicin.Name}' в аптечке '{medkit.NameMedkit ?? "Аптечка"}' истекает {endDate:dd.MM.yyyy}. Необходимо заменить.";
                        
                        // Проверяем, не создано ли уже такое уведомление за последние 24 часа
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

