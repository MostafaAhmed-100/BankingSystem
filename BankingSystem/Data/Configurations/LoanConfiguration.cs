using BankingSystem.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Data.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.HasKey(l => l.LoanNumber);

            builder.Property(l => l.Amount).HasColumnType("decimal(18,2)");

            builder.HasOne(l => l.Customer)
                   .WithMany(c => c.Loans)
                   .HasForeignKey(l => l.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Branch)
                   .WithMany(b => b.Loans)
                   .HasForeignKey(l => l.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}