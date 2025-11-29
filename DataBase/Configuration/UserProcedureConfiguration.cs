using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class UserProcedureConfiguration : IEntityTypeConfiguration<UserProcedure>
    {
        public void Configure(EntityTypeBuilder<UserProcedure> builder)
        {
            builder.HasKey(up => up.Id);

            builder.Property(up => up.Id)
                .ValueGeneratedOnAdd();

            builder.Property(up => up.UserId)
                .IsRequired();

            builder.Property(up => up.ProcedureId)
                .IsRequired();

            builder.Property(up => up.StartData)
                .IsRequired();

            builder.Property(up => up.EndData)
                .IsRequired();

            builder.Property(up => up.IsValid)
                .IsRequired();

            builder.HasOne(up => up.User)
                .WithMany(u => u.UsersProcedures)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Procedure)
                .WithMany(p => p.UsersProcedures)
                .HasForeignKey(up => up.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

