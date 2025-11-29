using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IHistoryUpMedicinRepository : IRepository<HistoryUpMedicin>
    {
        Task<IEnumerable<HistoryUpMedicin>> GetValidAsync();
        Task<IEnumerable<HistoryUpMedicin>> GetInvalidAsync();
        Task<IEnumerable<HistoryUpMedicin>> GetExpiredAsync();
        Task<HistoryUpMedicin?> GetWithMedicinsAsync(int id);
    }
}

