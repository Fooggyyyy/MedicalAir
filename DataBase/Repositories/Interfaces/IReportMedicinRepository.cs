using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IReportMedicinRepository : IRepository<ReportMedicin>
    {
        Task<IEnumerable<ReportMedicin>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}

