using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class ReportMedicinRepository : Repository<ReportMedicin>, IReportMedicinRepository
    {
        public ReportMedicinRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReportMedicin>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet
                .Where(r => r.DataStart >= startDate && r.DataEnd <= endDate)
                .ToListAsync();
        }
    }
}
