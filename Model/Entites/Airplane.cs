using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class Airplane
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Medkit>? Medkits { get; set; }
        public List<User>? Users { get; set; }
        public List<RegistrationUser>? RegistrationUsers { get; set; }

        public Airplane(string? Name)
        {
            this.Name = Name;
        }

        public Airplane() { }
    }
}
