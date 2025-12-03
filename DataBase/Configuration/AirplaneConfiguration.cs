using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class AirplaneConfiguration : IEntityTypeConfiguration<Airplane>
    {
        public void Configure(EntityTypeBuilder<Airplane> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasMany(a => a.Medkits)
                .WithOne(m => m.Airplane)
                .HasForeignKey(m => m.AirplaneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Users)
                .WithOne(u => u.Airplane)
                .HasForeignKey(u => u.AirplaneId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(a => a.RegistrationUsers)
                .WithOne(r => r.Airplane)
                .HasForeignKey(r => r.AirplaneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
