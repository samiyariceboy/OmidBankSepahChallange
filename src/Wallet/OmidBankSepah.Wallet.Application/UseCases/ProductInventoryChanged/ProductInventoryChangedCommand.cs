using MediatR;
using OmidBankSepah.IntegrationEvents.ProductInventory;

namespace OmidBankSepah.Wallet.Application.UseCases.ProductInventoryChanged;

public sealed record ProductInventoryChangedCommand(ProductInventoryChangedMessage Message) : IRequest;
