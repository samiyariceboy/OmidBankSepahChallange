using MediatR;
using OmidBankSepah.IntegrationEvents.ProductInventory;

namespace OmidBankSepah.Identity.Application.UseCases.ProductInventoryChanged;

public sealed record ProductInventoryChangedCommand(ProductInventoryChangedMessage Message) : IRequest;
