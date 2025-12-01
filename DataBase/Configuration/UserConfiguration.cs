using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;
using MedicalAir.Model.Enums;

namespace MedicalAir.DataBase.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.FullName)
                .HasMaxLength(255);

            builder.Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(u => u.HashPassword)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(u => u.Roles)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(u => u.AirplaneId)
                .IsRequired(false);

            builder.Property(u => u.IsBlocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(u => u.Airplane)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AirplaneId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.Certificats)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RegistrationUsers)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.MedicalExaminations)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UsersProcedures)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

