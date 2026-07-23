using BankingSystem.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Data.Configurations
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(c => c.CardNumber);
            builder.Property(c => c.CardNumber).HasMaxLength(16);
            builder.Property(c => c.CardType).HasMaxLength(50);
            builder.HasQueryFilter(a => a.IsActive == true);

            builder.Property(c => c.Limit).HasColumnType("decimal(18,2)");

            builder.HasOne(c => c.Account)
                   .WithMany(a => a.CreditCards)
                   .HasForeignKey(c => c.AccountNumber)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Customer)
                   .WithMany()
                   .HasForeignKey(c => c.CustomerId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}