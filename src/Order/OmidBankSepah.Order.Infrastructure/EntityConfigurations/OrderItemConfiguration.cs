using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates;

namespace OmidBankSepah.Order.Infrastructure.EntityConfigurations;
public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .Property(item => item.Price)
            .HasPrecision(18, 2);

        builder
            .HasIndex(item => new { item.OrderId, item.ProductId })
            .HasDatabaseName("IX_OrderItems_OrderId_ProductId");

        builder
            .HasIndex(item => item.ProductId)
            .HasDatabaseName("IX_OrderItems_ProductId");
    }
}
