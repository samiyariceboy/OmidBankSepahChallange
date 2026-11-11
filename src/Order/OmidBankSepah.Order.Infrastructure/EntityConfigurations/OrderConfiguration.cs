using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Infrastructure.EntityConfigurations;
public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder
            .Property(order => order.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(order => order.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(order => order.RowVersion)
            .IsRowVersion();

        builder.HasIndex(order => order.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("UX_Orders_IdempotencyKey");

        builder
            .HasIndex(order => new { order.CustomerId, order.CreatedAt })
            .HasDatabaseName("IX_Orders_CustomerId_CreatedAt");

        builder
            .HasIndex(order => new { order.Status, order.CreatedAt })
            .HasDatabaseName("IX_Orders_Status_CreatedAt");

        builder
            .HasIndex(order => order.CreatedAt)
            .HasDatabaseName("IX_Orders_CreatedAt");

        builder
            .Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
