using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // ========== Form Inputs ==========
    [BindProperty]
    public int LengthOfStay { get; set; }

    [BindProperty]
    public bool AcuteAdmission { get; set; }

    [BindProperty]
    public int ComorbidityScore { get; set; }

    [BindProperty]
    public int ERVisits { get; set; }

    // ========== Output ==========
    public int TotalScore { get; set; } = -1;

    public string RiskCategory { get; set; } = "";

    public void OnGet()
    {
        
    }

    public void OnPost()
    {
        TotalScore = LengthOfStay
                   + (AcuteAdmission ? 3 : 0)
                   + ComorbidityScore
                   + ERVisits;

        RiskCategory = TotalScore switch
        {
            <= 3 => "Up to 3.5%",
            <= 7 => "4.3% – 7.3%",
            <= 11 => "8.7% – 14.4%",
            <= 15 => "17% – 26.6%",
            <= 19 => "30.4% – 43.7%",
            _ => "Unknown"
        };
    }
}
