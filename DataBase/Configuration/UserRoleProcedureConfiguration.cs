using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;

namespace MedicalAir.DataBase.Configuration
{
    public class UserRoleProcedureConfiguration : IEntityTypeConfiguration<UserRoleProcedure>
    {
        public void Configure(EntityTypeBuilder<UserRoleProcedure> builder)
        {
            builder.HasKey(urp => urp.Id);

            builder.Property(urp => urp.Id)
                .ValueGeneratedOnAdd();

            builder.Property(urp => urp.ProcedureId)
                .IsRequired();

            builder.Property(urp => urp.Roles)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(urp => urp.Procedure)
                .WithMany(p => p.UsersRolesProcedures)
                .HasForeignKey(urp => urp.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(urp => urp.MedicalExaminations)
                .WithOne(me => me.UserRoleProcedure)
                .HasForeignKey(me => me.URPId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

