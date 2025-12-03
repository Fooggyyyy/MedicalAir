using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface ICertificatRepository : IRepository<Certificat>
    {
        Task<IEnumerable<Certificat>> GetByUserIdAsync(int userId);
    }
}
