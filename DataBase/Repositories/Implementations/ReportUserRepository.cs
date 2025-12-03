using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class ReportUserRepository : Repository<ReportUser>, IReportUserRepository
    {
        public ReportUserRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReportUser>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet
                .Where(r => r.DataStart >= startDate && r.DataEnd <= endDate)
                .ToListAsync();
        }
    }
}
