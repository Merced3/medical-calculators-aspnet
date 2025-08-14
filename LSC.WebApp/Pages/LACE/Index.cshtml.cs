using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LSC.WebApp.Pages.LACE
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

        // ===== Form Inputs (nullable so placeholders show) =====
        [BindProperty]
        [Display(Name = "Length of Stay")]
        [Required(ErrorMessage = "Please select the patient's length of stay.")]
        public int? LengthOfStay { get; set; }

        [BindProperty]
        [Display(Name = "Acute or Emergent Admission")]
        [Required(ErrorMessage = "Please indicate whether the admission was acute or emergent.")]
        public bool? AcuteAdmission { get; set; }

        [BindProperty]
        [Display(Name = "Comorbidity Category")]
        [Required(ErrorMessage = "Please select the patient's comorbidity category.")]
        public int? ComorbidityScore { get; set; }

        [BindProperty]
        [Display(Name = "ER Visits (Past 6 Months)")]
        [Required(ErrorMessage = "Please choose the number of ER visits in the past 6 months.")]
        public int? ERVisits { get; set; }

        // ========== Output ==========
        public int TotalScore { get; set; } = -1;
        public string RiskCategory { get; set; } = "";

        public void OnGet() { }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                TotalScore = -1;
                RiskCategory = "";
                return;
            }

            var total =
                    LengthOfStay!.Value
                    + (AcuteAdmission == true ? 3 : 0)
                    + ComorbidityScore!.Value
                    + ERVisits!.Value;

            TotalScore = total;

            RiskCategory = total switch
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
}
