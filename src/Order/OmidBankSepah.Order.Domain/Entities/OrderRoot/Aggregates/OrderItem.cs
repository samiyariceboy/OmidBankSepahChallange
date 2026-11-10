using Taday.Corelibrary.Domain.Entities;
using OmidBankSepah.Order.Domain.Entities.OrderRoot;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates
{
    public class OrderItem 
    {
        private OrderItem()
        {
        }

        private OrderItem(int productId, decimal price, int quantity)
        {
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

        internal static OrderItem Create(int productId, decimal price, int quantity)
        {
            Validate(productId, price, quantity);
            return new OrderItem(productId, price, quantity);
        }

        internal void AttachTo(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (Order is not null && !ReferenceEquals(Order, order))
                throw new InvalidOperationException("An order item cannot belong to more than one order.");

            Order = order;
        }

        internal void DetachFromOrder()
        {
            Order = null;
            OrderId = 0;
        }

        internal void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            Quantity = quantity;
        }

        internal void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

            Price = price;
        }

        private static void Validate(int productId, decimal price, int quantity)
        {
            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");

            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
