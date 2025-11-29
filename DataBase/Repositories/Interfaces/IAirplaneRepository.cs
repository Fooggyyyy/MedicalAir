using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IAirplaneRepository : IRepository<Airplane>
    {
        Task<Airplane?> GetByNameAsync(string name);
        Task<Airplane?> GetWithUsersAsync(int id);
        Task<Airplane?> GetWithMedkitsAsync(int id);
    }
}

