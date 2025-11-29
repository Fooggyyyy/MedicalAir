using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class AirplaneRepository : Repository<Airplane>, IAirplaneRepository
    {
        public AirplaneRepository(DBContext context) : base(context)
        {
        }

        public async Task<Airplane?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<Airplane?> GetWithUsersAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Users)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Airplane?> GetWithMedkitsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Medkits)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}

