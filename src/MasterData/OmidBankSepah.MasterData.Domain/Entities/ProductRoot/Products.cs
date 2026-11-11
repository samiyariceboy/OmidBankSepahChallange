using Taday.Corelibrary.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using OmidBankSepah.MasterData.Domain.Constraint;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot
{
    public class Products : AggregateRoot, ICloneable, IValidatableObject
    {
        private Products()
        {
        }

        private Products(
            Guid productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            Id = ImmediateFillId();

            ProductCategoryId = productCategoryId;
            ProductName = productName;
            BrandName = brandName;
            ModelName = modelName;
            ProductYear = productYear;
            ProductDescription = productDescription;
        }

        public int ProductYear { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string BrandName { get; private set; } = string.Empty;
        public string ModelName { get; private set; } = string.Empty;
        public string ProductDescription { get; private set; } = string.Empty;

        public Guid ProductCategoryId { get; private set; }
        public ProductCategory? ProductCategory { get; private set; }


        public static Products Create(
            Guid productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            Validate(productCategoryId, 
                productName, brandName,
                modelName, productYear,
                productDescription);

            return new Products(
                productCategoryId,
                productName.Trim(),
                brandName.Trim(),
                modelName.Trim(),
                productYear,
                productDescription.Trim());
        }

        public void Update(
            Guid productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            Validate(productCategoryId, 
                productName, brandName, 
                modelName, productYear, 
                productDescription);

            ProductYear = productYear;
            BrandName = brandName.Trim();
            ModelName = modelName.Trim();
            ProductName = productName.Trim();
            ProductCategoryId = productCategoryId;
            ProductDescription = productDescription.Trim();
        }

        private static void Validate(
            Guid productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            MasterDataAssert.IsSuccess(
                productCategoryId != Guid.Empty,
                [MasterDataErrors.ProductCategoryIdInvalid]
            );

            MasterDataAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(productName),
                [MasterDataErrors.ProductNameRequired]
            );

            MasterDataAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(brandName),
                [MasterDataErrors.BrandNameRequired]
            );

            MasterDataAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(modelName),
                [MasterDataErrors.ModelNameRequired]
            );

            MasterDataAssert.IsSuccess(
                productYear > 0,
                [MasterDataErrors.ProductYearInvalid]
            );

            MasterDataAssert.NotNull(
                productDescription,
                nameof(Validate),
                MasterDataErrors.ProductDescriptionRequired
            );
        }

        protected override bool SetHasIntegrityChecksum()
        {
            return false;
        }

        public override object GetId()
        {
            return Id;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return [];
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        protected override string[] IntegrityProperties => [];

    }
}