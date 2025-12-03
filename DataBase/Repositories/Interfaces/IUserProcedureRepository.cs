using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IUserProcedureRepository : IRepository<UserProcedure>
    {
        Task<IEnumerable<UserProcedure>> GetByUserIdAsync(int userId);
    }
}
