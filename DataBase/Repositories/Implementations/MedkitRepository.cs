using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class MedkitRepository : Repository<Medkit>, IMedkitRepository
    {
        public MedkitRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Medkit>> GetByAirplaneIdAsync(int airplaneId)
        {
            return await _dbSet
                .Include(m => m.Airplane)
                .Where(m => m.AirplaneId == airplaneId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medkit>> GetValidAsync()
        {
            return await _dbSet
                .Include(m => m.Airplane)
                .Where(m => m.IsValid)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medkit>> GetInvalidAsync()
        {
            return await _dbSet
                .Include(m => m.Airplane)
                .Where(m => !m.IsValid)
                .ToListAsync();
        }

        public async Task<Medkit?> GetWithMedicinsAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Medicins)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}

