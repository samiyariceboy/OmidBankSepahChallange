namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot
{
    public class ProductCategory
    {
        private ProductCategory()
        {
        }

        private ProductCategory(string name, string description, int? parentProductCategoryId, string code)
        {
            Name = name;
            Description = description;
            ParentProductCategoryId = parentProductCategoryId;
            Code = code;
            IsActive = true;
        }

        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public int? ParentProductCategoryId { get; private set; }
        public bool IsActive { get; private set; }

        public static ProductCategory Create(
            string name,
            string description,
            int? parentProductCategoryId,
            string code)
        {
            Validate(name, description, parentProductCategoryId, code);

            return new ProductCategory(
                name.Trim(),
                description.Trim(),
                parentProductCategoryId,
                code.Trim());
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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));

            if (description is null)
                throw new ArgumentNullException(nameof(description));

            if (parentProductCategoryId is <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentProductCategoryId), "Parent category ID must be greater than zero.");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Category code is required.", nameof(code));
        }
    }
}