using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace yyLibWeb.Pages
{
    [IgnoreAntiforgeryToken]
    [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel: PageModel
    {
        public string? RequestId { get; set; }

        public bool HasRequestId => string.IsNullOrWhiteSpace (RequestId) == false;

        private readonly ILogger <ErrorModel> _logger;

        public ErrorModel (ILogger <ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet ()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
