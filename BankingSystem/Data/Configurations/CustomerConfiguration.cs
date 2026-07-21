using BankingSystem.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).HasMaxLength(100);
            builder.Property(c => c.Street).HasMaxLength(150);
            builder.Property(c => c.City).HasMaxLength(100);

            builder.HasOne(c => c.user)
                   .WithOne(u => u.Customer)
                   .HasForeignKey<Customer>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Bankers)
                   .WithMany(b => b.customers)
                   .UsingEntity(j => j.ToTable("CustomerBankers"));
        }
    }
}