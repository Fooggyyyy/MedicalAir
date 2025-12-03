using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IMedicinRepository : IRepository<Medicin>
    {
        Task<Medicin?> GetByNameAsync(string name);
    }
}
