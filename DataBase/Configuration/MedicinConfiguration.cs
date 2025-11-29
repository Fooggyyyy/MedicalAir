using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class MedicinConfiguration : IEntityTypeConfiguration<Medicin>
    {
        public void Configure(EntityTypeBuilder<Medicin> builder)
        {  
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.HistoryUpMId)
                .IsRequired();

            builder.Property(m => m.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(m => m.Composition)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(m => m.HistoryUpMedicin)
                .WithMany(h => h.Medicins)
                .HasForeignKey(m => m.HistoryUpMId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

