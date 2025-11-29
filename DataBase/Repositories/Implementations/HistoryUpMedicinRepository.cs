using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class HistoryUpMedicinRepository : Repository<HistoryUpMedicin>, IHistoryUpMedicinRepository
    {
        public HistoryUpMedicinRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HistoryUpMedicin>> GetValidAsync()
        {
            return await _dbSet
                .Where(h => h.IsValid)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistoryUpMedicin>> GetInvalidAsync()
        {
            return await _dbSet
                .Where(h => !h.IsValid)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistoryUpMedicin>> GetExpiredAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _dbSet
                .Where(h => h.EndData < today)
                .ToListAsync();
        }

        public async Task<HistoryUpMedicin?> GetWithMedicinsAsync(int id)
        {
            return await _dbSet
                .Include(h => h.Medicins)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}

