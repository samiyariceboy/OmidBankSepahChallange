using OmidBankSepah.Order.Domain.Enums;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot
{
    public class Order
    {

        private Order()
        {
        }

        private Order(int customerId)
        {
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        public static Order Create(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentOutOfRangeException
                    (nameof(customerId), "Customer ID must be greater than zero.");

            return new Order(customerId);
        }

        public OrderItem AddItem(int productId, decimal price, int quantity)
        {
            var item = OrderItem.Create(productId, price, quantity);
            item.AttachTo(this);
            _items.Add(item);
            RecalculateTotalAmount();

            return item;
        }

        public void UpdateItemQuantity(int itemId, int quantity)
        {
            var item = GetItem(itemId);
            item.UpdateQuantity(quantity);
            RecalculateTotalAmount();
        }

        public void UpdateItemPrice(int itemId, decimal price)
        {
            var item = GetItem(itemId);
            item.UpdatePrice(price);
            RecalculateTotalAmount();
        }

        public void RemoveItem(int itemId)
        {
            var item = GetItem(itemId);
            _items.Remove(item);
            item.DetachFromOrder();
            RecalculateTotalAmount();
        }

        public void UpdateStatus(OrderStatus status)
        {
            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException(nameof(status), "Order status is invalid.");

            Status = status;
        }

        private OrderItem GetItem(int itemId)
        {
            if (itemId <= 0)
                throw new ArgumentOutOfRangeException(nameof(itemId), "Order item ID must be greater than zero.");

            return _items.FirstOrDefault(item => item.Id == itemId)
                ?? throw new InvalidOperationException($"Order item '{itemId}' was not found.");
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Sum(item => item.TotalPrice);
        }
    }
}