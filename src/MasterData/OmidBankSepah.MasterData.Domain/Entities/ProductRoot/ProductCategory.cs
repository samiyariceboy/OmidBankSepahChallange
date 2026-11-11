using Taday.Corelibrary.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using OmidBankSepah.MasterData.Domain.Constraint;
using OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductResponses;

namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot
{
    public class ProductCategory : AggregateRoot, ICloneable, IValidatableObject
    {
        private ProductCategory()
        {
        }

        private ProductCategory(
            string name, 
            string description,
            int? parentProductCategoryId,
            string code
        )
        {
            Id = ImmediateFillId();

            Code = code;
            Name = name;
            IsActive = true;
            Description = description;
            ParentProductCategoryId = parentProductCategoryId;
        }

        public bool IsActive { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public int? ParentProductCategoryId { get; private set; }
        public string Description { get; private set; } = string.Empty;

        private readonly List<Products> _products = [];
        public IReadOnlyList<Products> Products => _products.AsReadOnly();

        protected override string[] IntegrityProperties => [];

        public static ProductCategory Create
        (
            string name,
            string description,
            int? parentProductCategoryId,
            string code
        )
        {
            Validate(name, description, parentProductCategoryId, code);

            return new ProductCategory(
                name.Trim(),
                description.Trim(),
                parentProductCategoryId,
                code.Trim()
            );
        }

        public void Update(
            string name,
            string description,
            int? parentProductCategoryId,
            string code,
            bool isActive)
        {
            Validate(name, description, parentProductCategoryId, code);

            Name = name.Trim();
            Description = description.Trim();
            ParentProductCategoryId = parentProductCategoryId;
            Code = code.Trim();
            IsActive = isActive;
        }

        private static void Validate(
            string name,
            string description,
            int? parentProductCategoryId,
            string code)
        {
            MasterDataAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(name),
                [MasterDataErrors.CategoryNameRequired]
            );

            MasterDataAssert.NotNull(
                description,
                nameof(Validate),
                MasterDataErrors.CategoryDescriptionRequired
            );

            MasterDataAssert.IsSuccess(
                parentProductCategoryId is null or > 0,
                [MasterDataErrors.ParentCategoryIdInvalid]
            );

            MasterDataAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(code),
                [MasterDataErrors.CategoryCodeRequired]
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

        public object Clone()
        {
            return new ProductCategoryRes 
            {
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return [];
        }
    }
}