using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(Model.Enums.UserRoles role);
        Task<IEnumerable<User>> GetByAirplaneIdAsync(int airplaneId);
    }
}
