using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
    {
        public void Configure(EntityTypeBuilder<Procedure> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(p => p.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.MinValue)
                .IsRequired();

            builder.Property(p => p.MaxValue)
                .IsRequired();

            builder.Property(p => p.Units)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(p => p.MustBeTrue)
                .IsRequired();

            builder.HasMany(p => p.UsersRolesProcedures)
                .WithOne(urp => urp.Procedure)
                .HasForeignKey(urp => urp.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.UsersProcedures)
                .WithOne(up => up.Procedure)
                .HasForeignKey(up => up.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
