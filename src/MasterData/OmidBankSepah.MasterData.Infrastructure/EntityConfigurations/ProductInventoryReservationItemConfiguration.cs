using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.EntityConfigurations;

public sealed class ProductInventoryReservationItemConfiguration
    : IEntityTypeConfiguration<ProductInventoryReservationItem>
{
    public void Configure(EntityTypeBuilder<ProductInventoryReservationItem> builder)
    {
        builder.HasIndex(item => new { item.ProductInventoryReservationId, item.ProductId })
            .IsUnique()
            .HasDatabaseName("UX_ProductInventoryReservationItems_ReservationId_ProductId");

        builder.HasIndex(item => item.ProductId)
            .HasDatabaseName("IX_ProductInventoryReservationItems_ProductId");
    }
}
