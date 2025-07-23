const infoModal = document.getElementById('infoModal');
infoModal.addEventListener('show.bs.modal', function (event) {
    const trigger = event.relatedTarget;
    const title = trigger.getAttribute('data-title');
    const body = trigger.getAttribute('data-body');

    const modalTitle = infoModal.querySelector('.modal-title');
    const modalBody = infoModal.querySelector('.modal-body');

    modalTitle.textContent = title;
    modalBody.textContent = body;
});