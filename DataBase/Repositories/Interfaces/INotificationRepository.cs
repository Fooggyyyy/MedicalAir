using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<bool> ExistsSimilarAsync(int userId, string messageBody, int daysBack = 1);
    }
}

