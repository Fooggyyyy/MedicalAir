using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IReportUserRepository : IRepository<ReportUser>
    {
        Task<IEnumerable<ReportUser>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}

