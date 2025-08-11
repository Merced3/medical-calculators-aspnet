using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages.Mortality;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

    // ===== Inputs =====
    [BindProperty] public bool CHF_Comorbid { get; set; }
    [BindProperty] public bool CreatinineHigh { get; set; }

    [BindProperty, Required(ErrorMessage = "Select sex")]
    public int? Sex { get; set; }   // 0=Female, 1=Male

    [BindProperty, Required(ErrorMessage = "Select ADL status")]
    public int? ADLDependency { get; set; } // 2 or 5

    [BindProperty, Required(ErrorMessage = "Select cancer status")]
    public int? CancerStatus { get; set; }  // 3 or 8

    [BindProperty, Required(ErrorMessage = "Select albumin level")]
    public int? AlbuminLevel { get; set; }  // 1 or 2

    // ===== Outputs =====
    public int TotalScore { get; set; } = -1;
    public string RiskCategory { get; set; } = "";

    public void OnGet() { }

    public void OnPost()
    {
        if (!ModelState.IsValid)
        {
            // Do not compute; Razor will show validation errors.
            return;
        }

        var chf = CHF_Comorbid ? 2 : 0;
        var cr  = CreatinineHigh ? 2 : 0;

        TotalScore = chf + cr + Sex!.Value + ADLDependency!.Value + CancerStatus!.Value + AlbuminLevel!.Value;

        if (TotalScore <= 1)       RiskCategory = "4–13%";
        else if (TotalScore <= 3)  RiskCategory = "19–20%";
        else if (TotalScore <= 6)  RiskCategory = "34–37%";
        else                       RiskCategory = "64–68%";
    }
}
