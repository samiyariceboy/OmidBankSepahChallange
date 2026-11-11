using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot.Aggregates;

namespace OmidBankSepah.Wallet.Infrastructure.EntityConfigurations;

public sealed class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.Property(transaction => transaction.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(transaction => transaction.ReferenceId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(transaction => transaction.EndorsementId)
            .HasMaxLength(200);

        builder.Property(transaction => transaction.IdempotencyKey)
            .HasMaxLength(WalletTransaction.IdempotencyKeyMaxLength);

        builder.Property(transaction => transaction.Creditor).HasPrecision(18, 2);
        builder.Property(transaction => transaction.Debtor).HasPrecision(18, 2);
        builder.Property(transaction => transaction.PayableAmount).HasPrecision(18, 2);
        builder.Property(transaction => transaction.BalanceAfter).HasPrecision(18, 2);

        builder.HasIndex(transaction => transaction.WalletsId)
            .HasDatabaseName("IX_WalletTransactions_WalletsId");

        builder.HasIndex(transaction => new { transaction.WalletsId, transaction.ReferenceId })
            .HasDatabaseName("IX_WalletTransactions_WalletsId_ReferenceId");

        builder.HasIndex(transaction => transaction.InstallmentBookId)
            .HasDatabaseName("IX_WalletTransactions_InstallmentBookId");

        builder.HasIndex(transaction => transaction.IdempotencyKey)
            .IsUnique()
            .HasFilter("[IdempotencyKey] IS NOT NULL")
            .HasDatabaseName(WalletTransaction.IdempotencyKeyUniqueIndexName);
    }
}
