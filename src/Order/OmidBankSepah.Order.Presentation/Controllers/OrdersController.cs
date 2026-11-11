using Microsoft.AspNetCore.Mvc;
using OmidBankSepah.Order.Application.Contracts.Requests;
using OmidBankSepah.Order.Application.Services.Orders;

namespace OmidBankSepah.Order.Presentation.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderApplicationService orderApplicationService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderReq request,
        CancellationToken cancellationToken)
    {
        var order = await orderApplicationService.CreateAsync(request, cancellationToken);
        return Ok(order);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetById(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await orderApplicationService.GetByIdAsync(orderId, cancellationToken);
        return Ok(order);
    }
}
