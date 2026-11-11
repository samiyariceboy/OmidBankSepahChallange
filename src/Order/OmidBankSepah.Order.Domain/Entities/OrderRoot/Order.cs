using OmidBankSepah.Order.Domain.Enums;
using Taday.Corelibrary.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using OmidBankSepah.Order.Domain.Constraint;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot
{
    public class Order : AggregateRoot, ICloneable, IValidatableObject
    {

        private Order()
        {
        }

        private Order(Guid customerId)
        {
            Id = ImmediateFillId();
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        protected override string[] IntegrityProperties => throw new NotImplementedException();

        public static Order Create(Guid customerId)
        {
            OrderAssert.IsSuccess
            (
                customerId != Guid.Empty, 
                [OrderErrors.CustomerIdInvalid]
            );

            return new Order(customerId);
        }

        public OrderItem AddItem(Guid productId, decimal price, int quantity)
        {
            var item = OrderItem.Create(productId, price, quantity);
            item.AttachTo(this);
            _items.Add(item);
            RecalculateTotalAmount();

            return item;
        }

        public void UpdateItemQuantity(Guid itemId, int quantity)
        {
            var item = GetItem(itemId, nameof(UpdateItemQuantity));
            item.UpdateQuantity(quantity);
            RecalculateTotalAmount();
        }

        public void UpdateItemPrice(Guid itemId, decimal price)
        {
            var item = GetItem(itemId, nameof(UpdateItemPrice));
            item.UpdatePrice(price);
            RecalculateTotalAmount();
        }

        public void RemoveItem(Guid itemId)
        {
            var item = GetItem(itemId, nameof(RemoveItem));
            _items.Remove(item);
            item.DetachFromOrder();
            RecalculateTotalAmount();
        }

        public void UpdateStatus(OrderStatus status)
        {
            OrderAssert.IsSuccess(Enum.IsDefined(status), [OrderErrors.OrderStatusInvalid]);

            Status = status;
        }

        private OrderItem GetItem(Guid itemId, string methodName)
        {
            OrderAssert.IsSuccess(itemId != Guid.Empty, [OrderErrors.OrderItemIdInvalid]);

            var item = _items.FirstOrDefault(item => item.Id == itemId);
            OrderAssert.NotNull(item, methodName, OrderErrors.OrderItemNotFound);
            return item!;
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Sum(item => item.TotalPrice);
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
    }
}