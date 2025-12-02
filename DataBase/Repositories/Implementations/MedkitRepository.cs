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
                .Include(m => m.Medicins)
                    .ThenInclude(med => med.HistoryUpMedicin)
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
                    .ThenInclude(med => med.HistoryUpMedicin)
                .Include(m => m.Airplane)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override Task UpdateAsync(Medkit entity)
        {
            // Для many-to-many связей нужно явно отслеживать изменения
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entry.State = EntityState.Modified;
            
            // Отслеживаем изменения в коллекции Medicins для many-to-many связи
            if (entity.Medicins != null)
            {
                foreach (var medicin in entity.Medicins)
                {
                    var medicinEntry = _context.Entry(medicin);
                    if (medicinEntry.State == EntityState.Detached)
                    {
                        _context.Medicins.Attach(medicin);
                    }
                    // Помечаем связь как добавленную
                    medicinEntry.State = EntityState.Unchanged;
                }
            }
            
            return Task.CompletedTask;
        }
    }
}

