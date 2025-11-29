using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class UserProcedure
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        
        public int ProcedureId { get; set; }
        public Procedure? Procedure { get; set; }

        public DateOnly StartData { get; set; }
        public DateOnly EndData { get; set; }

        public bool IsValid { get; set; }

        public UserProcedure(int userId, int procedureId, DateOnly startData, DateOnly endData, bool isValid)
        {
            UserId = userId;
            ProcedureId = procedureId;
            StartData = startData;
            EndData = endData;
            IsValid = isValid;
        }
        
        public UserProcedure() { }
    }
}
