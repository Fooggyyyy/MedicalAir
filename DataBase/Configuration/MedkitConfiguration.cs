using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class MedkitConfiguration : IEntityTypeConfiguration<Medkit>
    {
        public void Configure(EntityTypeBuilder<Medkit> builder)
        {

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.AirplaneId)
                .IsRequired();

            builder.Property(m => m.NameMedkit)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(m => m.IsValid)
                .IsRequired();

            builder.HasOne(m => m.Airplane)
                .WithMany(a => a.Medkits)
                .HasForeignKey(m => m.AirplaneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Medicins)
                .WithMany(med => med.Medkits);
        }
    }
}
