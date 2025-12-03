using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class RegistrationUserConfiguration : IEntityTypeConfiguration<RegistrationUser>
    {
        public void Configure(EntityTypeBuilder<RegistrationUser> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.AirplaneId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.IsRegister)
                .IsRequired();

            builder.Property(r => r.MessageBody)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(r => r.Data)
                .IsRequired();

            builder.HasOne(r => r.Airplane)
                .WithMany(a => a.RegistrationUsers)
                .HasForeignKey(r => r.AirplaneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RegistrationUsers)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
