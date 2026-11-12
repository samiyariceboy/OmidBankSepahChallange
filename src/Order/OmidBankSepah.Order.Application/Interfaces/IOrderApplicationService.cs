using OmidBankSepah.Order.Application.Contracts.Requests;
using OmidBankSepah.Order.Domain.Contracts.Responses;

namespace OmidBankSepah.Order.Application.Interfaces;

public interface IOrderApplicationService
{
    Task<OrderRes> CreateAsync(
        CreateOrderReq request,
        CancellationToken cancellationToken);

    Task<OrderRes?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}
