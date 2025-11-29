using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Interfaces;

namespace MedicalAir.DataBase.Repositories.Implementations
{
    public class ProcedureRepository : Repository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(DBContext context) : base(context)
        {
        }

        public async Task<Procedure?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}

