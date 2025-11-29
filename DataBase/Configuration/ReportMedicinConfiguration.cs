using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class ReportMedicinConfiguration : IEntityTypeConfiguration<ReportMedicin>
    {
        public void Configure(EntityTypeBuilder<ReportMedicin> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.DataStart)
                .IsRequired();

            builder.Property(r => r.DataEnd)
                .IsRequired();

            builder.Property(r => r.TotalMedicines)
                .IsRequired();

            builder.Property(r => r.TotalUniqueMedicines)
                .IsRequired();

            builder.Property(r => r.TotalMedkit)
                .IsRequired();

            builder.Property(r => r.ExpiredCount)
                .IsRequired();

            builder.Property(r => r.AlmostExpiredCount)
                .IsRequired();

            builder.Property(r => r.ExpiredCountPercent)
                .IsRequired();

            builder.Property(r => r.AlmostExpiredCountPercent)
                .IsRequired();
        }
    }
}

