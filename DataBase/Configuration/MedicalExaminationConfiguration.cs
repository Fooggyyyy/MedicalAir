using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class MedicalExaminationConfiguration : IEntityTypeConfiguration<MedicalExamination>
    {
        public void Configure(EntityTypeBuilder<MedicalExamination> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.URPId)
                .IsRequired();

            builder.Property(m => m.DataStart)
                .IsRequired();

            builder.Property(m => m.DataEnd)
                .IsRequired();

            builder.Property(m => m.IsValid)
                .IsRequired();

            builder.Property(m => m.Message)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.HasOne(m => m.User)
                .WithMany(u => u.MedicalExaminations)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.UserRoleProcedure)
                .WithMany(urp => urp.MedicalExaminations)
                .HasForeignKey(m => m.URPId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
