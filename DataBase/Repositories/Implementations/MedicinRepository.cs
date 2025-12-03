using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class MedicinRepository : Repository<Medicin>, IMedicinRepository
    {
        public MedicinRepository(DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Medicin>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.HistoryUpMedicin)
                .ToListAsync();
        }

        public async Task<Medicin?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(m => m.HistoryUpMedicin)
                .FirstOrDefaultAsync(m => m.Name == name);
        }
    }
}
