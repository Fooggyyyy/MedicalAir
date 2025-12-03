using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicalAir.Model.Entites;

namespace MedicalAir.DataBase.Configuration
{
    public class CertificatConfiguration : IEntityTypeConfiguration<Certificat>
    {
        public void Configure(EntityTypeBuilder<Certificat> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.DataStart)
                .IsRequired();

            builder.Property(c => c.DataEnd)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(c => c.User)
                .WithMany(u => u.Certificats)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
