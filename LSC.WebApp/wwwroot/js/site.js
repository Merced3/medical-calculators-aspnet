// --- Modal: safe on pages that don't include it ---
const infoModal = document.getElementById('infoModal');
if (infoModal) {
  infoModal.addEventListener('show.bs.modal', function (event) {
    const trigger = event.relatedTarget;
    const title = trigger?.getAttribute('data-title') || '';
    const body = trigger?.getAttribute('data-body') || '';

    const modalTitle = infoModal.querySelector('.modal-title');
    const modalBody = infoModal.querySelector('.modal-body');

    if (modalTitle) modalTitle.textContent = title;
    if (modalBody) modalBody.innerHTML = body;
  });
}

window.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname;

    // --- APACHE bindings: small safety net ---
    if (path.includes("/APACHE")) {
        document.querySelectorAll('select').forEach(select => {
            select.addEventListener('change', updateApacheScore);
        });
        document.getElementById("DecPrecisionSelect").addEventListener("change", updateApacheScore);

        document.querySelectorAll('input[name="PostEmergSurg"]').forEach(radio => {
            radio.addEventListener("change", updateApacheScore);
        });

        // Watch for unit dropdowns
        ["ScoreSelect", "LogORSelect", "OddsRatioSelect", "MortalitySelect"].forEach(id => {
            document.getElementById(id)?.addEventListener("change", updateApacheScore);
        });
    }
});

function updateApacheScore() {
    const fields = [
        "RectalTemp", "MAP", "HR", "RR", "Oxygenation", "AcidBase",
        "Sodium", "Potassium", "Creatinine", "Hematocrit", "WBC",
        "Glasgow", "Age", "ChronicDx", "AdmitDx"
    ];

    let allSelected = true;
    let totalScore = 0;
    let postEmergSurg = parseFloat(document.querySelector('input[name="PostEmergSurg"]:checked')?.value || "0");
    let admitDx = 0;

    for (let field of fields) {
        const el = document.getElementById(field);
        if (!el || el.disabled) continue;

        const selected = el.value;
        if (selected === "" || el.selectedIndex === 0) {
            allSelected = false;
            break;
        }

        const value = parseFloat(selected);
        if (field === "AdmitDx") admitDx = value;
        else if (field === "ChronicDx" && el.options[el.selectedIndex].text.includes("Emergent")) postEmergSurg = 1;
        else totalScore += value;
    }

    const dp = parseInt(document.getElementById("DecPrecisionSelect").value || "2");

    const scoreEl = document.getElementById("ScoreResult");
    const logOREl = document.getElementById("LogOR");
    const oddsEl = document.getElementById("OddsRatio");
    const mortalityEl = document.getElementById("Mortality");
    
    if (!allSelected) {
        scoreEl.value = "";
        logOREl.value = "";
        oddsEl.value = "";
        mortalityEl.value = "";
        return;
    }

    const logOR = -3.517 + (totalScore * 0.146) + postEmergSurg + admitDx;
    const oddsRatio = Math.exp(logOR);
    const mortality = 100 * oddsRatio / (1 + oddsRatio);

    // Respect selected label units
    const scoreUnit = document.getElementById("ScoreSelect").value;         // we don't use it but we will keep it just incase
    const logORUnit = document.getElementById("LogORSelect").value;         // we don't use it but we will keep it just incase
    const oddsUnit = document.getElementById("OddsRatioSelect").value;
    const mortalityUnit = document.getElementById("MortalitySelect").value;

    scoreEl.value = totalScore.toFixed(0);  // Score is unaffected
    logOREl.value = logOR.toFixed(dp);      // Log OR is unaffected
    
    oddsEl.value =
        (["%", "Percent"].includes(oddsUnit)
            ? (oddsRatio * 100).toFixed(dp)
            : oddsRatio.toFixed(dp));
    
    mortalityEl.value =
        (["%", "Percent"].includes(mortalityUnit)
            ? mortality.toFixed(dp)
            : (mortality / 100).toFixed(dp)) +
        (["%", "Percent"].includes(mortalityUnit) ? "%" : "");
}
