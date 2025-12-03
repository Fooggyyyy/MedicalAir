using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IRegistrationUserRepository : IRepository<RegistrationUser>
    {
        Task<IEnumerable<RegistrationUser>> GetByUserIdAsync(int userId);
        Task<IEnumerable<RegistrationUser>> GetByAirplaneIdAsync(int airplaneId);
        Task<IEnumerable<RegistrationUser>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}
