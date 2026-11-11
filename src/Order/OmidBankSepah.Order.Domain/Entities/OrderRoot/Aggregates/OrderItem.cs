using Taday.Corelibrary.Domain.Entities;
using OmidBankSepah.Order.Domain.Constraint;
using OmidBankSepah.Order.Domain.Entities.OrderRoot;

namespace OmidBankSepah.Order.Domain.Entities.OrderRoot.Aggregates
{
    public class OrderItem : BaseEntity, ICloneable
    {
        private OrderItem()
        {
        }

        private OrderItem(Guid productId, decimal price, int quantity)
        {
            Price = price;
            Quantity = quantity;
            ProductId = productId;
        }


        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }

        public Guid ProductId { get; private set; }

        public decimal Price { get; private set; }

        public int Quantity { get; private set; }

        public decimal TotalPrice => Price * Quantity;

        protected override string[] IntegrityProperties
            => [];

        internal static OrderItem Create(Guid productId, decimal price, int quantity)
        {
            Validate(productId, price, quantity);
            return new OrderItem(productId, price, quantity);
        }

        internal void AttachTo(Order order)
        {
            OrderAssert.NotNull(order, nameof(AttachTo), OrderErrors.OrderRequired);

            OrderAssert.IsSuccess(
                Order is null || ReferenceEquals(Order, order),
                [OrderErrors.OrderItemAlreadyAttached]
            );

            Order = order;
        }

        internal void DetachFromOrder()
        {
            Order = null;
            OrderId = Guid.Empty;
        }

        internal void UpdateQuantity(int quantity)
        {
            OrderAssert.IsSuccess
            (
                quantity > 0,
                [OrderErrors.QuantityMustBePositive]
            );

            Quantity = quantity;
        }

        internal void UpdatePrice(decimal price)
        {
            OrderAssert.IsSuccess(price >= 0, [OrderErrors.PriceCannotBeNegative]);

            Price = price;
        }

        private static void Validate(Guid productId, decimal price, int quantity)
        {
            OrderAssert.IsSuccess
            (
                productId != Guid.Empty,
                [OrderErrors.ProductIdInvalid]
            );

            OrderAssert.IsSuccess
            (
                price >= 0, 
                [OrderErrors.PriceCannotBeNegative]
            );

            OrderAssert.IsSuccess(quantity > 0, [OrderErrors.QuantityMustBePositive]);
        }

        protected override bool SetHasIntegrityChecksum()
        {
            return true;
        }

        public override object GetId()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
