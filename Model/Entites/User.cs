using MedicalAir.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class User
    {
        public int Id { get; set; }

        public int? AirplaneId { get; set; }
        public Airplane? Airplane { get; set; }

        public string? FullName {  get; set; } 
        public string? Email { get; set; }
        public string? HashPassword { get; set; }
        public UserRoles Roles { get; set; }
        public bool IsBlocked { get; set; } = false;

        public List<Certificat>? Certificats { get; set; }
        public List<Notification>? Notifications { get; set; }
        public List<RegistrationUser>? RegistrationUsers { get; set; }
        public List<MedicalExamination>? MedicalExaminations { get; set; }
        public List<UserProcedure>? UsersProcedures { get; set; }

        public User(int? airplaneId, string? fullName, string? email, string? hashPassword, UserRoles roles)
        {
            AirplaneId = airplaneId;
            FullName = fullName;
            Email = email;
            HashPassword = hashPassword;
            Roles = roles;
            IsBlocked = false;
        }

        public User(string? fullName, string? email, string? hashPassword, UserRoles roles)
        {
            FullName = fullName;
            Email = email;
            HashPassword = hashPassword;
            Roles = roles;
            IsBlocked = false;
        }

        public User() { }
    }
}
