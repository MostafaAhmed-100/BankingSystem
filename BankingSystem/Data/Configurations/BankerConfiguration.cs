using BankingSystem.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Data.Configurations
{
    public class BankerConfiguration : IEntityTypeConfiguration<Banker>
    {
        public void Configure(EntityTypeBuilder<Banker> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).HasMaxLength(100);

            builder.HasOne(b => b.user)
                   .WithOne(u => u.Banker)
                   .HasForeignKey<Banker>(b => b.UserId);

            builder.HasQueryFilter(a => a.IsActive == true);

            builder.HasOne(b => b.Branch)
                   .WithMany()
                   .HasForeignKey(b => b.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}