document.addEventListener('DOMContentLoaded', () => {
    const sizeSelect = document.getElementById('sizeSelect');
    const quantityInput = document.getElementById('quantityInput');

    if (sizeSelect && quantityInput) {
        const syncMax = () => {
            const opt = sizeSelect.options[sizeSelect.selectedIndex];
            const max = parseInt(opt?.dataset.max || '1', 10) || 1;

            quantityInput.dataset.max = max;

            const stockInfo = document.getElementById('stockInfo');
            if (stockInfo) stockInfo.textContent = max;

            let v = parseInt(quantityInput.value, 10);
            if (!v || v < 1) quantityInput.value = 1;
            if (v > max) quantityInput.value = max;
        };

        sizeSelect.addEventListener('change', syncMax);

        quantityInput.addEventListener('input', () => {
            let raw = quantityInput.value;
            raw = raw.replace(/[^0-9]/g, '');
            if (raw.length > 1 && raw.startsWith('0')) {
                raw = raw.replace(/^0+/, '');
            }
            quantityInput.value = raw;
        });

        quantityInput.addEventListener('blur', () => {
            const max = parseInt(quantityInput.dataset.max || '1', 10) || 1;
            let v = parseInt(quantityInput.value, 10);

            if (isNaN(v) || v < 1) {
                quantityInput.value = 1;
            } else if (v > max) {
                quantityInput.value = max;
            } else {
                quantityInput.value = v;
            }
        });

        syncMax();
    }

    document.querySelectorAll('input.quantity-input[data-max]').forEach(inp => {
        const max = parseInt(inp.dataset.max, 10) || 1;
        inp.dataset.max = max;

        inp.addEventListener('input', () => {
            inp.value = inp.value.replace(/[^0-9]/g, '');
        });

        inp.addEventListener('blur', () => {
            let v = parseInt(inp.value, 10);
            if (isNaN(v) || v < 1) inp.value = 1;
            else if (v > max) inp.value = max;
            else inp.value = v;
        });

        const form = inp.closest('form');
        if (form) {
            inp.addEventListener('change', () => {
                form.submit();
            });
        }
    });
});