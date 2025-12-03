using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class MedicalExaminationRepository : Repository<MedicalExamination>, IMedicalExaminationRepository
    {
        public MedicalExaminationRepository(DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<MedicalExamination>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.User)
                .Include(m => m.UserRoleProcedure)
                    .ThenInclude(urp => urp.Procedure)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalExamination>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(m => m.User)
                .Include(m => m.UserRoleProcedure)
                    .ThenInclude(urp => urp.Procedure)
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }
    }
}
