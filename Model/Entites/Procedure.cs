using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class Procedure
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string? Units { get; set; }
        public bool MustBeTrue { get; set; }

        public List<UserRoleProcedure>? UsersRolesProcedures { get; set; }
        public List<UserProcedure>? UsersProcedures { get; set; }
        public Procedure(string? name, string? description, int minValue, int maxValue, string? units, bool mustBeTrue)
        {
            Name = name;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            Units = units;
            MustBeTrue = mustBeTrue;
        }

        public Procedure() { }
    }
}
