namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates
{
    public class OrderItem
    {
        private OrderItem()
        {
        }

        private OrderItem(int orderId, int productId, decimal price, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            Price = price;
            Quantity = quantity;
        }

        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public Order? Order { get; private set; }
        public int ProductId { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice => Price * Quantity;

        public static OrderItem Create(int orderId, int productId, decimal price, int quantity)
        {
            Validate(orderId, productId, price, quantity);
            return new OrderItem(orderId, productId, price, quantity);
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            Quantity = quantity;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

            Price = price;
        }

        private static void Validate(int orderId, int productId, decimal price, int quantity)
        {
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId), "Order ID must be greater than zero.");

            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");

            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
