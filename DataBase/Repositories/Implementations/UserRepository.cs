using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DBContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Airplane)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRoles role)
        {
            return await _dbSet
                .Include(u => u.Airplane)
                .Where(u => u.Roles == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByAirplaneIdAsync(int airplaneId)
        {
            return await _dbSet
                .Include(u => u.Airplane)
                .Where(u => u.AirplaneId == airplaneId)
                .ToListAsync();
        }
    }
}
