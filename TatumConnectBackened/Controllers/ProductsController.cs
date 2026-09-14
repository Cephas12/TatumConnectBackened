using Microsoft.AspNetCore.Mvc;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Responses;
using TatumConnectBackened.Services;

namespace TatumConnectBackened.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IBillerService _service;

    public ProductsController(IBillerService service)
    {
        _service = service;
    }

    [HttpGet("billers")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BillerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<BillerDto>>>> GetBillers(
        [FromQuery] BillerQueryParameters query,
        CancellationToken ct)
    {
        return Ok(await _service.GetAsync(query, ct));
    }

    [HttpGet("billers/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BillerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BillerDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BillerDto>>> GetBiller(
        Guid id,
        CancellationToken ct)
    {
        var response = await _service.GetByIdAsync(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
