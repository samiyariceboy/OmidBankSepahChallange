using MediatR;
using OmidBankSepah.IntegrationEvents.ProductInventory;

namespace OmidBankSepah.Order.Application.UseCases.ProductInventoryChanged;

public sealed record ProductInventoryChangedCommand(ProductInventoryChangedMessage Message) : IRequest;
