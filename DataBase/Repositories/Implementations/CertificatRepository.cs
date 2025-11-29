using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class CertificatRepository : Repository<Certificat>, ICertificatRepository
    {
        public CertificatRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Certificat>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificat>> GetByStatusAsync(CertificatStatus status)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificat>> GetExpiredAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.DataEnd < today)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificat>> GetExpiringSoonAsync(int days)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var futureDate = today.AddDays(days);
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.DataEnd >= today && c.DataEnd <= futureDate)
                .ToListAsync();
        }
    }
}

