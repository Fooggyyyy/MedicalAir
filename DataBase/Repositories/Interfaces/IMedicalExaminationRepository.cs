using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Repositories.Interfaces
{
    public interface IMedicalExaminationRepository : IRepository<MedicalExamination>
    {
        Task<IEnumerable<MedicalExamination>> GetByUserIdAsync(int userId);
    }
}
