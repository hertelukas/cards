using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cards.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public int? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int? statusCode)
    {
        if (statusCode is 404)
        {
            _logger.LogWarning("Page not found");
            ErrorMessage = "We couldn't find the page you were looking for.";
        }
        else
        {
            _logger.LogError("Error {Code} occurred", statusCode);
        }

        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        ErrorCode = statusCode;
    }
}