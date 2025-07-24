using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages.Mortality;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // ========== Form Inputs ==========
    [BindProperty] public bool CHF_Comorbid { get; set; }
    [BindProperty] public bool CreatinineHigh { get; set; }
    [BindProperty] public int Sex { get; set; }
    [BindProperty] public int ADLDependency { get; set; }
    [BindProperty] public int CancerStatus { get; set; }
    [BindProperty] public int AlbuminLevel { get; set; }

    // ========== Output ==========
    public int TotalScore { get; set; } = -1;

    public string RiskCategory { get; set; } = "";

    public void OnGet()
    {
        
    }

    public void OnPost()
    {
        // I removed the LACE switch calculation logic, we probably won't need it since we want the live update right?
    }
}
