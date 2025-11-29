using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IMedicinRepository : IRepository<Medicin>
    {
        Task<Medicin?> GetByNameAsync(string name);
        Task<IEnumerable<Medicin>> GetByHistoryUpMedicinIdAsync(int historyUpMId);
        Task<IEnumerable<Medicin>> GetWithMedkitsAsync(int id);
    }
}

