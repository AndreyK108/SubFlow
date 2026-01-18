// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// =======================
// SubFlow theme (light/dark)
// =======================
(function () {
    const STORAGE_KEY = "sf.theme"; // "light" | "dark"
    const root = document.documentElement;

    function applyTheme(theme) {
        if (theme === "dark") root.setAttribute("data-theme", "dark");
        else root.removeAttribute("data-theme");
        updateToggleUI(theme);
    }

    function getSystemTheme() {
        return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches
            ? "dark"
            : "light";
    }

    function getSavedTheme() {
        const v = localStorage.getItem(STORAGE_KEY);
        return (v === "dark" || v === "light") ? v : null;
    }

    function saveTheme(theme) {
        localStorage.setItem(STORAGE_KEY, theme);
    }

    function updateToggleUI(theme) {
        const btn = document.getElementById("sfThemeToggle");
        if (!btn) return;

        const isDark = theme === "dark";
        btn.setAttribute("aria-pressed", isDark ? "true" : "false");
        btn.setAttribute("title", isDark ? "Светлая тема" : "Тёмная тема");

        const icon = btn.querySelector("[data-sf-icon]");
        const text = btn.querySelector("[data-sf-text]");

        if (icon) icon.textContent = isDark ? "☀️" : "🌙";
        if (text) text.textContent = isDark ? "Light" : "Dark";
    }

    // init
    const initialTheme = getSavedTheme() ?? getSystemTheme();
    applyTheme(initialTheme);

    // handle click
    document.addEventListener("click", function (e) {
        const btn = e.target.closest("#sfThemeToggle");
        if (!btn) return;

        const current = root.getAttribute("data-theme") === "dark" ? "dark" : "light";
        const next = current === "dark" ? "light" : "dark";

        saveTheme(next);
        applyTheme(next);
    });

    // react to system changes if user didn't choose explicitly
    if (window.matchMedia) {
        const mq = window.matchMedia("(prefers-color-scheme: dark)");
        mq.addEventListener?.("change", function () {
            if (getSavedTheme() != null) return; // user choice overrides system
            applyTheme(getSystemTheme());
        });
    }
})();
