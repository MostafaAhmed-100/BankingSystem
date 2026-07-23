using BankingSystem.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.AccountNumber);

            builder.Property(a => a.Balance).HasColumnType("decimal(18,2)");
            builder.Property(a => a.CurrencyCode).HasMaxLength(3);
            builder.Property(a => a.RowVersion).IsRowVersion();

            builder.HasQueryFilter(a => a.IsActive == true);

            builder.HasOne(a => a.Customer)
                   .WithMany(c => c.Accounts)
                   .HasForeignKey(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}