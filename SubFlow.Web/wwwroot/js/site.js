(function () {
    const STORAGE_KEY = "sf-theme";
    const root = document.documentElement;
    const btn = document.getElementById("themeToggle");
    const icon = document.getElementById("themeIcon");

    function setTheme(theme) {
        root.setAttribute("data-theme", theme);
        localStorage.setItem(STORAGE_KEY, theme);

        // Показываем ИКОНКУ ТЕКУЩЕЙ темы (как ты просил)
        if (icon) icon.textContent = theme === "dark" ? "🌙" : "☀️";
    }

    function getInitialTheme() {
        const saved = localStorage.getItem(STORAGE_KEY);
        if (saved === "dark" || saved === "light") return saved;

        // Если нет сохранённого — берём системную
        const prefersDark = window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches;
        return prefersDark ? "dark" : "light";
    }

    // init
    setTheme(getInitialTheme());

    if (btn) {
        btn.addEventListener("click", () => {
            const current = root.getAttribute("data-theme") || "light";
            setTheme(current === "dark" ? "light" : "dark");
        });
    }
})();
