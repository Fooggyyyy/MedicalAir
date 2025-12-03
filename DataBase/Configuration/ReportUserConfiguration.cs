using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class ReportUserConfiguration : IEntityTypeConfiguration<ReportUser>
    {
        public void Configure(EntityTypeBuilder<ReportUser> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.DataStart)
                .IsRequired();

            builder.Property(r => r.DataEnd)
                .IsRequired();

            builder.Property(r => r.TotalUsers)
                .IsRequired();

            builder.Property(r => r.TotalUsersME)
                .IsRequired();

            builder.Property(r => r.Passed)
                .IsRequired();

            builder.Property(r => r.NotPassed)
                .IsRequired();

            builder.Property(r => r.PassedPercent)
                .IsRequired();

            builder.Property(r => r.NotPassedPercent)
                .IsRequired();
        }
    }
}
