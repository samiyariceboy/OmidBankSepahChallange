using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.EntityConfigurations;
public sealed class ProductsConfiguration : IEntityTypeConfiguration<Products>
{
    public void Configure(EntityTypeBuilder<Products> builder)
    {
        builder.Property(product => product.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.BrandName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(product => product.ModelName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(product => product.ProductDescription)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.HasIndex(product => product.ProductName)
            .HasDatabaseName("IX_Products_ProductName");

        builder.HasIndex(product => new { product.BrandName, product.ModelName })
            .HasDatabaseName("IX_Products_BrandName_ModelName");

        builder.HasIndex(product => product.ProductCategoryId)
            .HasDatabaseName("IX_Products_ProductCategoryId");

        builder.HasIndex(product => product.ProductYear)
            .HasDatabaseName("IX_Products_ProductYear");
    }
}
