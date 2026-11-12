using MediatR;
using OmidBankSepah.IntegrationEvents.ProductInventory;

namespace OmidBankSepah.MasterData.Application.UseCases.ProductInventoryChanged;

public sealed record ProductInventoryChangedCommand(ProductInventoryChangedMessage Message) : IRequest;
