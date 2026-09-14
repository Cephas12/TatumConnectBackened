using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace TatumConnectBackened.DTOs;

public class BillerQueryParameters : PaginationParameters
{
    [FromQuery(Name = "category")]
    public BillerCategory? Category { get; set; }
}
