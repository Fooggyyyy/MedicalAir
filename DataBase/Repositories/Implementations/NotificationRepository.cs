using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            // Фильтруем уведомления старше недели
            var weekAgo = DateTime.Now.AddDays(-7);
            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.UserId == userId && n.CreatedDate >= weekAgo)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
    }
}

