using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.EntityConfigurations;

public sealed class ProductInventoryConfiguration : IEntityTypeConfiguration<ProductInventory>
{
    public void Configure(EntityTypeBuilder<ProductInventory> builder)
    {
        builder.Property(inventory => inventory.Quantity)
            .IsRequired();

        builder.Property(inventory => inventory.RowVersion)
            .IsRowVersion();

        builder.HasIndex(inventory => inventory.ProductId)
            .IsUnique()
            .HasDatabaseName("UX_ProductInventories_ProductId");

        builder.HasOne<Products>()
            .WithOne()
            .HasForeignKey<ProductInventory>(inventory => inventory.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
