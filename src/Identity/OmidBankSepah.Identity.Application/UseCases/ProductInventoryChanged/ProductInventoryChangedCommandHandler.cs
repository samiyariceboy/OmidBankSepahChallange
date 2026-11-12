using MediatR;

namespace OmidBankSepah.Identity.Application.UseCases.ProductInventoryChanged;

public sealed class ProductInventoryChangedCommandHandler
    : IRequestHandler<ProductInventoryChangedCommand>
{
    public Task Handle(
        ProductInventoryChangedCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: Forward the product inventory change to the Notification Service.
        return Task.CompletedTask;
    }
}
