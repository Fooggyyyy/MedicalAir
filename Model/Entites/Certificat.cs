using MedicalAir.Model.Enums;

namespace MedicalAir.Model.Entites
{
    public class Certificat
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateOnly DataStart { get; set; }
        public DateOnly DataEnd { get; set; }
        public CertificatStatus Status { get; set; }

        public Certificat(int userId, DateOnly dataStart, DateOnly dataEnd, CertificatStatus status)
        {
            UserId = userId;
            DataStart = dataStart;
            DataEnd = dataEnd;
            Status = status;
        }

        public Certificat() { }
    }
}
