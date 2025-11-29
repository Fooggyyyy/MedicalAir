using Microsoft.EntityFrameworkCore;
using MedicalAir.Model.Entites;
using MedicalAir.DataBase.Configuration;

namespace MedicalAir.DataBase
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<RegistrationUser> RegistrationUsers { get; set; }
        public DbSet<Certificat> Certificats { get; set; }
        public DbSet<Medicin> Medicins { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MedicalExamination> MedicalExaminations { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ReportUser> ReportUsers { get; set; }
        public DbSet<Medkit> Medkits { get; set; }
        public DbSet<ReportMedicin> ReportMedicins { get; set; }
        public DbSet<HistoryUpMedicin> HistoryUpMedicins { get; set; }
        public DbSet<UserProcedure> UserProcedures { get; set; }
        public DbSet<UserRoleProcedure> UserRoleProcedures { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AirplaneConfiguration());
            modelBuilder.ApplyConfiguration(new RegistrationUserConfiguration());
            modelBuilder.ApplyConfiguration(new CertificatConfiguration());
            modelBuilder.ApplyConfiguration(new MedicinConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalExaminationConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new ReportUserConfiguration());
            modelBuilder.ApplyConfiguration(new MedkitConfiguration());
            modelBuilder.ApplyConfiguration(new ReportMedicinConfiguration());
            modelBuilder.ApplyConfiguration(new HistoryUpMedicinConfiguration());
            modelBuilder.ApplyConfiguration(new UserProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleProcedureConfiguration());
        }
    }
}
