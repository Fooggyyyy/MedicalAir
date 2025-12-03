using MedicalAir.Model.Enums;

namespace MedicalAir.Model.Entites
{
    public class UserRoleProcedure
    {
        public int Id { get; set; }

        public int ProcedureId { get; set; }
        public Procedure? Procedure { get; set; }

        public UserRoles Roles { get; set; }

        public List<MedicalExamination>? MedicalExaminations { get; set; }
        public UserRoleProcedure(int procedureId, UserRoles roles)
        {
            ProcedureId = procedureId;
            Roles = roles;
        }

        public UserRoleProcedure() { }
    }
}
