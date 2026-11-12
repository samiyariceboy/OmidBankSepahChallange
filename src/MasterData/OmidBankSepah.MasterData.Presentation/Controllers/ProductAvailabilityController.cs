using Microsoft.AspNetCore.Mvc;
using OmidBankSepah.MasterData.Application.Interfaces;
using OmidBankSepah.MasterData.Presentation.Contracts.Requests;

namespace OmidBankSepah.MasterData.Presentation.Controllers;

[ApiController]
[Route("api/product-availability")]
public sealed class ProductAvailabilityController(
    IProductAvailabilityApplicationService productAvailabilityApplicationService) : ControllerBase
{
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> Get(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var availability = await productAvailabilityApplicationService.GetProductAvailabilityAsync(
            productId,
            cancellationToken);

        return Ok(availability);
    }

    [HttpPost("{productId:guid}/increase")]
    public async Task<IActionResult> Increase(
        Guid productId,
        [FromBody] IncreaseProductAvailabilityReq request,
        CancellationToken cancellationToken)
    {
        var availability = await productAvailabilityApplicationService.IncreaseProductAvailabilityAsync(
            productId,
            request.Quantity,
            cancellationToken);

        return Ok(availability);
    }
}
