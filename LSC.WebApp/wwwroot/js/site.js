const infoModal = document.getElementById('infoModal');
infoModal.addEventListener('show.bs.modal', function (event) {
    const trigger = event.relatedTarget;
    const title = trigger.getAttribute('data-title');
    const body = trigger.getAttribute('data-body');

    const modalTitle = infoModal.querySelector('.modal-title');
    const modalBody = infoModal.querySelector('.modal-body');

    modalTitle.textContent = title;
    modalBody.innerHTML = body;
});

function updateLaceScore() {
    const length = parseInt(document.getElementById("LengthOfStay").value);
    const acute = document.querySelector('input[name="AcuteAdmission"]:checked')?.value === "true";
    const comorb = parseInt(document.getElementById("ComorbidityScore").value);
    const er = parseInt(document.getElementById("ERVisits").value);

    const total = length + (acute ? 3 : 0) + comorb + er;
    let risk = "Unknown";

    if (total <= 3) risk = "Up to 3.5%";
    else if (total <= 7) risk = "4.3% – 7.3%";
    else if (total <= 11) risk = "8.7% – 14.4%";
    else if (total <= 15) risk = "17% – 26.6%";
    else if (total <= 19) risk = "30.4% – 43.7%";

    document.getElementById("scoreOutput").innerHTML =
        `<h4>LACE Score: ${total}</h4><p>Estimated Risk: <strong>${risk}</strong></p>`;

    document.getElementById("scoreOutput").style.display = "block";
}

function updateMortalityScore() {
    const chf = document.getElementById("CHF_Comorbid")?.checked ? 2 : 0;
    const creatinine = document.getElementById("CreatinineHigh")?.checked ? 2 : 0;
    const sex = parseInt(document.getElementById("Sex")?.value || 0);
    const adl = parseInt(document.getElementById("ADLDependency")?.value || 0);
    const cancer = parseInt(document.getElementById("CancerStatus")?.value || 0);
    const albumin = parseInt(document.getElementById("AlbuminLevel")?.value || 0);

    const total = chf + creatinine + sex + adl + cancer + albumin;

    let risk = "Unknown";
    if (total <= 1) risk = "4–13%";
    else if (total <= 3) risk = "19–20%";
    else if (total <= 6) risk = "34–37%";
    else if (total <= 20) risk = "64–68%";

    const output = document.getElementById("scoreOutput");
    output.innerHTML = `<h4>Total Score: ${total}</h4><p>1-Year Mortality Risk: <strong>${risk}</strong></p>`;
    output.style.display = "block";
}

window.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname;

    if (path.includes("/LACE")) {
        document.querySelectorAll('select, input[type="radio"]').forEach(input => {
            input.addEventListener('change', updateLaceScore);
        });
        updateLaceScore();
    }

    if (path.includes("/Mortality")) {
        document.querySelectorAll('select, input[type="radio"], input[type="checkbox"]').forEach(input => {
            input.addEventListener('change', updateMortalityScore);
        });
        updateMortalityScore();
    }

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
