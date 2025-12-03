using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
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
                .Include(m => m.Medicins)
                    .ThenInclude(med => med.HistoryUpMedicin)
                .Where(m => m.AirplaneId == airplaneId)
                .ToListAsync();
        }

        public async Task<Medkit?> GetWithMedicinsAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Medicins)
                    .ThenInclude(med => med.HistoryUpMedicin)
                .Include(m => m.Airplane)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override Task UpdateAsync(Medkit entity)
        {
            
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entry.State = EntityState.Modified;
            
            if (entity.Medicins != null)
            {
                foreach (var medicin in entity.Medicins)
                {
                    var medicinEntry = _context.Entry(medicin);
                    if (medicinEntry.State == EntityState.Detached)
                    {
                        _context.Medicins.Attach(medicin);
                    }
                    
                    medicinEntry.State = EntityState.Unchanged;
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
