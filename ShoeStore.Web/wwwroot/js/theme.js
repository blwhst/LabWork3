document.addEventListener('DOMContentLoaded', () => {
    const toggle = document.getElementById('themeToggle');
    const html = document.documentElement;

    if (!toggle) return;

    const updateButton = () => {
        const current = html.getAttribute('data-theme') || 'light';
        toggle.textContent = current === 'dark' ? 'Светлая тема' : 'Тёмная тема';
    };

    updateButton();

    toggle.addEventListener('click', () => {
        const current = html.getAttribute('data-theme') || 'light';
        const next = current === 'light' ? 'dark' : 'light';

        html.setAttribute('data-theme', next);
        localStorage.setItem('theme', next);

        updateButton();
    });
});