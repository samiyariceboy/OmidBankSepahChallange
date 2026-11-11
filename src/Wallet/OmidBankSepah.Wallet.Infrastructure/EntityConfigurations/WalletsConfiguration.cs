using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot;

namespace OmidBankSepah.Wallet.Infrastructure.EntityConfigurations;

public sealed class WalletsConfiguration : IEntityTypeConfiguration<Wallets>
{
    public void Configure(EntityTypeBuilder<Wallets> builder)
    {
        builder.Property(wallet => wallet.CustomerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(wallet => wallet.Balance).HasPrecision(18, 2);
        builder.Property(wallet => wallet.CreditBalance).HasPrecision(18, 2);
        builder.Property(wallet => wallet.CreditBalanceRemaining).HasPrecision(18, 2);
        builder.Property(wallet => wallet.WithdrawableBalance).HasPrecision(18, 2);

        builder.HasIndex(wallet => wallet.CustomerId)
            .HasDatabaseName("IX_Wallets_CustomerId");

        builder.Navigation(wallet => wallet.Transactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(wallet => wallet.Transactions)
            .WithOne(transaction => transaction.Wallets)
            .HasForeignKey(transaction => transaction.WalletsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
