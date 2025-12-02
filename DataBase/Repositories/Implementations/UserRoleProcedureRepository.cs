using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class UserRoleProcedureRepository : Repository<UserRoleProcedure>, IUserRoleProcedureRepository
    {
        public UserRoleProcedureRepository(DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<UserRoleProcedure>> GetAllAsync()
        {
            return await _dbSet
                .Include(urp => urp.Procedure)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRoleProcedure>> GetByProcedureIdAsync(int procedureId)
        {
            return await _dbSet
                .Include(urp => urp.Procedure)
                .Where(urp => urp.ProcedureId == procedureId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRoleProcedure>> GetByRoleAsync(UserRoles role)
        {
            return await _dbSet
                .Include(urp => urp.Procedure)
                .Where(urp => urp.Roles == role)
                .ToListAsync();
        }
    }
}

