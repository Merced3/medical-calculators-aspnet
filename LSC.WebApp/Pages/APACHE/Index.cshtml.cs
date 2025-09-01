using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages.APACHE;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger) { _logger = logger; }

    // ===== Form Inputs (nullable so placeholders show) =====
    [BindProperty]
    [Display(Name = "Admit Diagnosis")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public double? AdmitDx { get; set; }

    [BindProperty]
    [Display(Name = "Rectal Temperature")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? RectalTemp { get; set; }

    [BindProperty]
    [Display(Name = "Mean Arterial Pressure (MAP)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? MAP { get; set; }

    [BindProperty]
    [Display(Name = "Heart Rate (HR)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? HR { get; set; }

    [BindProperty]
    [Display(Name = "Respiratory Rate (RR)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? RR { get; set; }

    [BindProperty]
    [Display(Name = "Oxygenation – A–a Gradient or PaO₂")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Oxygenation { get; set; }

    [BindProperty]
    [Display(Name = "Acid-Base Status – pH or HCO₃")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? AcidBase { get; set; }

    [BindProperty]
    [Display(Name = "Sodium (Na⁺)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Sodium { get; set; }

    [BindProperty]
    [Display(Name = "Potassium (K⁺)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Potassium { get; set; }

    [BindProperty]
    [Display(Name = "Creatinine (mg/dL)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Creatinine { get; set; }

    [BindProperty]
    [Display(Name = "Hematocrit (Hct)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Hematocrit { get; set; }

    [BindProperty]
    [Display(Name = "White Blood Cell Count (WBC)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? WBC { get; set; }

    [BindProperty]
    [Display(Name = "Glasgow Coma Scale (GCS)")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Glasgow { get; set; }

    [BindProperty]
    [Display(Name = "Age Modifier")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? Age { get; set; }

    [BindProperty]
    [Display(Name = "Chronic Health Conditions")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public int? ChronicDx { get; set; }

    [BindProperty]
    [Display(Name = "Post-Emergency Surgery")]
    [Required(ErrorMessage = "Please Select an Option.")]
    public double? PostEmergSurg { get; set; }

    // ===== Form Outputs =====

    public bool ShowResults { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Raw computed outputs (numeric)
    public int? Score { get; private set; }
    public double? LogOR { get; private set; }
    public double? OddsRatio { get; private set; }
    public double? Mortality { get; private set; }

    // Enums for display formatting
    public enum OddsFormat { Ratio, Percent, Fraction, Rate }
    public enum MortalityFormat { Percent, Fraction, Ratio, Rate }

    // User-selected display options
    [BindProperty] public OddsFormat OddsDisplay { get; set; } = OddsFormat.Ratio;
    [BindProperty] public MortalityFormat MortalityDisplay { get; set; } = MortalityFormat.Percent;
    [BindProperty] public int DecimalPrecision { get; set; } = 2;

    // Formatted outputs (strings) for rendering in the UI
    public string? ScoreText { get; private set; }
    public string? LogORText { get; private set; }
    public string? OddsText { get; private set; }
    public string? MortalityText { get; private set; }

    // ===== Form Submission Handler =====
    public IActionResult OnPostCalculate()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please complete all fields before calculating.";
            ShowResults = false;
            return Page();
        }

        // 1) Compute base numbers
        Score = (RectalTemp ?? 0) + (MAP ?? 0) + (HR ?? 0) + (RR ?? 0) +
                (Oxygenation ?? 0) + (AcidBase ?? 0) + (Sodium ?? 0) +
                (Potassium ?? 0) + (Creatinine ?? 0) + (Hematocrit ?? 0) +
                (WBC ?? 0) + (Glasgow ?? 0) + (Age ?? 0) + (ChronicDx ?? 0);

        // 1.2 Add AdmitDx and PostEmergSurg into log odds
        double admitDxVal = AdmitDx ?? 0;
        double postSurgVal = PostEmergSurg ?? 0;

        LogOR = -3.517 + ((Score ?? 0) * 0.146) + postSurgVal + admitDxVal;

        // 1.3 Odds ratio & Mortality
        OddsRatio = Math.Exp(LogOR.Value);
        Mortality = 100 * OddsRatio / (1 + OddsRatio);

        // 2) Format according to user selections
        var dp = Math.Clamp(DecimalPrecision, 0, 6);

        ScoreText = Score?.ToString();
        LogORText = LogOR?.ToString($"F{dp}");

        if (OddsRatio.HasValue)
        {
            var or = OddsRatio.Value;
            OddsText = OddsDisplay switch
            {
                OddsFormat.Ratio    => or.ToString($"F{dp}"),
                OddsFormat.Percent  => (or * 100).ToString($"F{dp}") + "%",
                OddsFormat.Fraction => $"{Math.Round(or, dp)}:1",
                OddsFormat.Rate     => (or / (1 + or)).ToString($"F{dp}"),
                _ => or.ToString($"F{dp}")
            };
        }

        if (Mortality.HasValue)
        {
            var mPct = Mortality.Value;     // 0..100
            var p    = mPct / 100.0;        // 0..1
            var odds = p / (1 - p);         // odds = p/(1-p)

            MortalityText = MortalityDisplay switch
            {
                MortalityFormat.Percent  => mPct.ToString($"F{dp}") + "%",
                MortalityFormat.Fraction => p.ToString($"F{dp}"),
                MortalityFormat.Ratio    => $"{Math.Round(odds, dp)}:1",
                MortalityFormat.Rate     => p.ToString($"F{dp}"),
                _ => mPct.ToString($"F{dp}") + "%"
            };
        }

        ShowResults = true;
        return Page();
    }
}
