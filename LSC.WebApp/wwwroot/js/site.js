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

// Automatically bind updates on page load
window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('select, input[type="radio"]').forEach(input => {
        input.addEventListener('change', updateLaceScore);
    });

    // Initial render
    updateLaceScore();
});