using Microsoft.AspNetCore.Mvc;
using TatumConnectBackened.DTOs;

namespace TatumConnectBackened.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([
            FromBody] ProductPurchaseRequestDto request, CancellationToken ct)
        {
            var response = await _service.PurchaseAsync(request, ct);

            if (!response.Success)
            {
                if (response.Errors?.Any(x => x.Code == "AccountNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(x => x.Code == "ProductNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(x => x.Code == "ProductItemNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(x => x.Code == "InsufficientBalance") == true)
                    return BadRequest(response);

                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TatumConnectBackened.Common.Models.PaginationParameters pagination, [FromQuery] TransactionFilterDto? filter)
        {
            var response = await _service.GetTransactionsAsync(pagination, filter);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _service.GetTransactionByIdAsync(id);
            if (!response.Success)
            {
                if (response.Errors?.Any(e => e.Code == "NotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(e => e.Code == "Forbidden") == true)
                    return Forbid();

                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary([FromQuery] TransactionSummaryFilterDto filter)
        {
            var response = await _service.GetAdminTransactionSummaryAsync(filter);
            if (!response.Success)
            {
                if (response.Errors?.Any(e => e.Code == "Forbidden") == true)
                    return Forbid();

                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
