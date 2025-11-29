using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.Property(n => n.UserId)
                .IsRequired();

            builder.Property(n => n.MessageBody)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

