using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages;

public class DisclaimerModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public DisclaimerModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

