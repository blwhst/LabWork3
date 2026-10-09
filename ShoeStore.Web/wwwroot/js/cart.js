// Ограничение ввода количества остатком на складе.

document.addEventListener('DOMContentLoaded', () => {
    // ---- Страница товара ----
    const sizeSelect = document.getElementById('sizeSelect');
    const qtyInput = document.getElementById('qtyInput');

    if (sizeSelect && qtyInput) {
        const syncMax = () => {
            const opt = sizeSelect.options[sizeSelect.selectedIndex];
            const max = parseInt(opt?.dataset.max || '1', 10) || 1;
            qtyInput.max = max;
            const v = parseInt(qtyInput.value, 10);
            if (!v || v < 1) qtyInput.value = 1;
            if (v > max) qtyInput.value = max;
        };

        sizeSelect.addEventListener('change', syncMax);

        qtyInput.addEventListener('input', () => {
            const max = parseInt(qtyInput.max, 10) || 1;
            let v = parseInt(qtyInput.value, 10);
            if (isNaN(v) || v < 1) { qtyInput.value = 1; return; }
            if (v > max) qtyInput.value = max;
        });

        qtyInput.addEventListener('blur', () => {
            const max = parseInt(qtyInput.max, 10) || 1;
            let v = parseInt(qtyInput.value, 10);
            if (isNaN(v) || v < 1) qtyInput.value = 1;
            else if (v > max) qtyInput.value = max;
        });

        syncMax();
    }

    // ---- Корзина: любые input с data-max ----
    document.querySelectorAll('input.qty-input[data-max]').forEach(inp => {
        const max = parseInt(inp.dataset.max, 10) || 1;
        inp.max = max;

        const clamp = () => {
            let v = parseInt(inp.value, 10);
            if (isNaN(v) || v < 1) inp.value = 1;
            else if (v > max) inp.value = max;
        };
        inp.addEventListener('input', clamp);
        inp.addEventListener('blur', clamp);
    });
});