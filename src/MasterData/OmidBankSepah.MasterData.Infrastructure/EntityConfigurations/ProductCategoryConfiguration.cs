using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.EntityConfigurations;
public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.Property(category => category.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(category => category.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(category => category.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.HasIndex(category => category.Name)
            .HasDatabaseName("IX_ProductCategories_Name");

        builder.HasIndex(category => category.Code)
            .HasDatabaseName("IX_ProductCategories_Code");

        builder.HasIndex(category => category.ParentProductCategoryId)
            .HasDatabaseName("IX_ProductCategories_ParentProductCategoryId");

        builder.Navigation(category => category.Products)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(category => category.Products)
            .WithOne(product => product.ProductCategory)
            .HasForeignKey(product => product.ProductCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
