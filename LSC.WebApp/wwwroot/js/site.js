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
