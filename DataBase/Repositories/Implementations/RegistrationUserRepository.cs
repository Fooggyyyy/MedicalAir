using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class RegistrationUserRepository : Repository<RegistrationUser>, IRegistrationUserRepository
    {
        public RegistrationUserRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RegistrationUser>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Airplane)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RegistrationUser>> GetByAirplaneIdAsync(int airplaneId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Airplane)
                .Where(r => r.AirplaneId == airplaneId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RegistrationUser>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Airplane)
                .Where(r => r.Data >= startDate && r.Data <= endDate)
                .ToListAsync();
        }
    }
}
