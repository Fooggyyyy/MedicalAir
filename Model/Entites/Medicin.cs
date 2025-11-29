using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class Medicin
    {
        public int Id { get; set; }

        public int HistoryUpMId { get; set; }
        public HistoryUpMedicin? HistoryUpMedicin { get; set; }

        public string? Name { get; set; }
        public string? Composition { get; set; }

        public List<Medkit>? Medkits { get; set; }

        public Medicin(int historyUpMId, string? name, string? composition)
        {
            HistoryUpMId = historyUpMId;
            Name = name;
            Composition = composition;
        }

        public Medicin() { }
    }
}
