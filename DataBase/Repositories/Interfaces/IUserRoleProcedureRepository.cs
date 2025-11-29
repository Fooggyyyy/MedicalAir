using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IUserRoleProcedureRepository : IRepository<UserRoleProcedure>
    {
        Task<IEnumerable<UserRoleProcedure>> GetByProcedureIdAsync(int procedureId);
        Task<IEnumerable<UserRoleProcedure>> GetByRoleAsync(UserRoles role);
    }
}

