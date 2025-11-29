using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class Medkit
    {
        public int Id { get; set; }

        public int AirplaneId { get; set; }
        public Airplane? Airplane { get; set; }

        public string? NameMedkit { get; set; }
        public bool IsValid { get; set; }

        public List<Medicin>? Medicins { get; set; }

        public Medkit(int airplaneId, string? nameMedkit, bool isValid)
        {
            AirplaneId = airplaneId;
            NameMedkit = nameMedkit;
            IsValid = isValid;
        }

        public Medkit() { }
    }
}
