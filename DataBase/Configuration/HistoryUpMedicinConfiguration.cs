using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class HistoryUpMedicinConfiguration : IEntityTypeConfiguration<HistoryUpMedicin>
    {
        public void Configure(EntityTypeBuilder<HistoryUpMedicin> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .ValueGeneratedOnAdd();

            builder.Property(h => h.Count)
                .IsRequired();

            builder.Property(h => h.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(h => h.UpData)
                .IsRequired();

            builder.Property(h => h.EndData)
                .IsRequired();

            builder.Property(h => h.IsValid)
                .IsRequired();

            builder.HasMany(h => h.Medicins)
                .WithOne(m => m.HistoryUpMedicin)
                .HasForeignKey(m => m.HistoryUpMId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
