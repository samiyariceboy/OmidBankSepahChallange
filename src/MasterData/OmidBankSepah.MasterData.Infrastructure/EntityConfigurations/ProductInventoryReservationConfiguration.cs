using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.EntityConfigurations;

public sealed class ProductInventoryReservationConfiguration
    : IEntityTypeConfiguration<ProductInventoryReservation>
{
    public void Configure(EntityTypeBuilder<ProductInventoryReservation> builder)
    {
        builder.Property(reservation => reservation.OrderId)
            .IsRequired();

        builder.HasIndex(reservation => reservation.OrderId)
            .IsUnique()
            .HasDatabaseName("UX_ProductInventoryReservations_OrderId");

        builder.Navigation(reservation => reservation.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(reservation => reservation.Items)
            .WithOne(item => item.ProductInventoryReservation)
            .HasForeignKey(item => item.ProductInventoryReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
