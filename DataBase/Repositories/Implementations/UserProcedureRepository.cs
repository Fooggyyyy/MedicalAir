using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class UserProcedureRepository : Repository<UserProcedure>, IUserProcedureRepository
    {
        public UserProcedureRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserProcedure>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(up => up.User)
                .Include(up => up.Procedure)
                .Where(up => up.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserProcedure>> GetByProcedureIdAsync(int procedureId)
        {
            return await _dbSet
                .Include(up => up.User)
                .Include(up => up.Procedure)
                .Where(up => up.ProcedureId == procedureId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserProcedure>> GetValidAsync()
        {
            return await _dbSet
                .Include(up => up.User)
                .Include(up => up.Procedure)
                .Where(up => up.IsValid)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserProcedure>> GetInvalidAsync()
        {
            return await _dbSet
                .Include(up => up.User)
                .Include(up => up.Procedure)
                .Where(up => !up.IsValid)
                .ToListAsync();
        }
    }
}

