using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class CertificatRepository : Repository<Certificat>, ICertificatRepository
    {
        public CertificatRepository(DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Certificat>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificat>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }
}
