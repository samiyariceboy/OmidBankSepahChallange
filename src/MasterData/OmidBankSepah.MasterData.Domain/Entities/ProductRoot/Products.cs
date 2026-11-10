namespace OmidBankSepah.MasterData.Domain.Entities.ProductRoot
{
    public class Products
    {
        private Products()
        {
        }

        private Products(
            int productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            ProductCategoryId = productCategoryId;
            ProductName = productName;
            BrandName = brandName;
            ModelName = modelName;
            ProductYear = productYear;
            ProductDescription = productDescription;
        }

        public int Id { get; private set; }
        public int ProductCategoryId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string BrandName { get; private set; } = string.Empty;
        public string ModelName { get; private set; } = string.Empty;
        public int ProductYear { get; private set; }
        public string ProductDescription { get; private set; } = string.Empty;

        public static Products Create(
            int productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            Validate(productCategoryId, productName, brandName, modelName, productYear, productDescription);

            return new Products(
                productCategoryId,
                productName.Trim(),
                brandName.Trim(),
                modelName.Trim(),
                productYear,
                productDescription.Trim());
        }

        public void Update(
            int productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            Validate(productCategoryId, productName, brandName, modelName, productYear, productDescription);

            ProductCategoryId = productCategoryId;
            ProductName = productName.Trim();
            BrandName = brandName.Trim();
            ModelName = modelName.Trim();
            ProductYear = productYear;
            ProductDescription = productDescription.Trim();
        }

        private static void Validate(
            int productCategoryId,
            string productName,
            string brandName,
            string modelName,
            int productYear,
            string productDescription)
        {
            if (productCategoryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productCategoryId), "Product category ID must be greater than zero.");

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required.", nameof(productName));

            if (string.IsNullOrWhiteSpace(brandName))
                throw new ArgumentException("Brand name is required.", nameof(brandName));

            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("Model name is required.", nameof(modelName));

            if (productYear <= 0)
                throw new ArgumentOutOfRangeException(nameof(productYear), "Product year must be greater than zero.");

            if (productDescription is null)
                throw new ArgumentNullException(nameof(productDescription));
        }
    }
}