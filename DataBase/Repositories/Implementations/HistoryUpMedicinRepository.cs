using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class HistoryUpMedicinRepository : Repository<HistoryUpMedicin>, IHistoryUpMedicinRepository
    {
        public HistoryUpMedicinRepository(DBContext context) : base(context)
        {
        }
    }
}
