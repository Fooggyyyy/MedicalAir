using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface ICertificatRepository : IRepository<Certificat>
    {
        Task<IEnumerable<Certificat>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Certificat>> GetByStatusAsync(CertificatStatus status);
        Task<IEnumerable<Certificat>> GetExpiredAsync();
        Task<IEnumerable<Certificat>> GetExpiringSoonAsync(int days);
    }
}

