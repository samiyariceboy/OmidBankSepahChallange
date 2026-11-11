using OmidBankSepah.Order.Domain.Enums;
using Taday.Corelibrary.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using OmidBankSepah.Order.Domain.Constraint;
using OmidBankSepah.Order.Domain.Contracts.Responses;
using OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot
{
    public class Order : AggregateRoot, ICloneable, IValidatableObject
    {

        private Order()
        {
        }

        private Order(Guid customerId, string idempotencyKey)
        {
            Id = ImmediateFillId();
            CustomerId = customerId;
            IdempotencyKey = idempotencyKey;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid CustomerId { get; private set; }
        public string IdempotencyKey { get; private set; } = string.Empty;
        public byte[] RowVersion { get; private set; } = [];
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? RejectionReason { get; private set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        protected override string[] IntegrityProperties => [];

        public static Order Create(Guid customerId, string idempotencyKey)
        {
            OrderAssert.IsSuccess
            (
                customerId != Guid.Empty, 
                [OrderErrors.CustomerIdInvalid]
            );

            OrderAssert.IsSuccess(
                !string.IsNullOrWhiteSpace(idempotencyKey),
                [OrderErrors.IdempotencyKeyRequired]
            );

            OrderAssert.IsSuccess(
                idempotencyKey.Length <= 100,
                [OrderErrors.IdempotencyKeyTooLong]
            );

            return new Order(customerId, idempotencyKey.Trim());
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

        public void Confirm()
        {
            EnsurePending();
            OrderAssert.IsSuccess(_items.Count > 0, [OrderErrors.OrderItemsRequired]);

            Status = OrderStatus.Confirmed;
            RejectionReason = null;
        }

        public void Reject(string reason)
        {
            EnsurePending();
            OrderAssert.IsSuccess(!string.IsNullOrWhiteSpace(reason), [OrderErrors.OrderStatusInvalid]);

            Status = OrderStatus.Rejected;
            RejectionReason = reason.Trim();
        }

        public bool MatchesRequest(Guid customerId, IEnumerable<(Guid ProductId, decimal Price, int Quantity)> items)
        {
            if (CustomerId != customerId)
            {
                return false;
            }

            var requested = items.OrderBy(item => item.ProductId)
                .ThenBy(item => item.Price)
                .ThenBy(item => item.Quantity)
                .ToArray();

            var current = _items
                .Select(item => (item.ProductId, item.Price, item.Quantity))
                .OrderBy(item => item.ProductId)
                .ThenBy(item => item.Price)
                .ThenBy(item => item.Quantity)
                .ToArray();

            return requested.SequenceEqual(current);
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

        private void EnsurePending()
        {
            OrderAssert.IsSuccess(Status == OrderStatus.Pending, [OrderErrors.OrderMustBePending]);
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
            return new OrderRes(
                Id,
                CustomerId,
                Status,
                TotalAmount,
                CreatedAt,
                RejectionReason,
                _items.Select(item => (OrderItemRes)item.Clone()).ToArray()
            );
        }
    }
}