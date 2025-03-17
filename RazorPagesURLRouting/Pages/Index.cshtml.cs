using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesURLRouting.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public String CurrencyUrl { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        String? url = Url.Page("Currency/View", new { Code = "USD" });
        CurrencyUrl = url ?? "";
    }
}
