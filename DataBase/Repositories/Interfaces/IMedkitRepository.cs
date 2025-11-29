using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IMedkitRepository : IRepository<Medkit>
    {
        Task<IEnumerable<Medkit>> GetByAirplaneIdAsync(int airplaneId);
        Task<IEnumerable<Medkit>> GetValidAsync();
        Task<IEnumerable<Medkit>> GetInvalidAsync();
        Task<Medkit?> GetWithMedicinsAsync(int id);
    }
}

