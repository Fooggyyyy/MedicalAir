using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IMedkitRepository : IRepository<Medkit>
    {
        Task<IEnumerable<Medkit>> GetByAirplaneIdAsync(int airplaneId);
        Task<Medkit?> GetWithMedicinsAsync(int id);
    }
}
