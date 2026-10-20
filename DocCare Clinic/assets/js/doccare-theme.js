/* Light / dark mode for the staff panels (admin, reception, doctor).
   The choice is kept in this browser (localStorage). The masters set data-theme on <html> in <head>
   before anything is drawn, so a dark page never flashes white; the colours are in doccare-dark.css. */
(function () {
    var KEY = 'doccare-theme';

    function isDark() { return document.documentElement.getAttribute('data-theme') === 'dark'; }

    function store(theme) {
        try { localStorage.setItem(KEY, theme); } catch (e) { /* private window: just this page */ }
    }

    // Text and grid colours for charts, matching the theme
    function ink() { return isDark() ? '#c9ccd8' : '#52514e'; }
    function grid() { return isDark() ? '#2c3142' : '#e8e7e3'; }

    function syncButtons() {
        var dark = isDark();
        var buttons = document.querySelectorAll('.theme-toggle');
        for (var i = 0; i < buttons.length; i++) {
            var b = buttons[i];
            var label = dark ? 'Switch to light mode' : 'Switch to dark mode';
            b.setAttribute('aria-label', label);
            b.setAttribute('title', label);
            b.setAttribute('aria-pressed', dark ? 'true' : 'false');
            var icon = b.querySelector('i');
            if (icon) icon.className = dark ? 'fa-solid fa-sun' : 'fa-solid fa-moon';
            var text = b.querySelector('.theme-toggle-text');
            if (text) text.textContent = dark ? 'Light mode' : 'Dark mode';
        }
    }

    // Chart.js charts already on the page take the new colours (axes, grid, legend)
    function recolorCharts() {
        if (!window.Chart) return;
        var c = ink(), g = grid();
        Chart.defaults.color = c;
        Chart.defaults.borderColor = g;
        var all = Chart.instances || {};
        Object.keys(all).forEach(function (k) {
            var chart = all[k];
            try {
                // The chart's own settings object: chart.options is a computed view that cannot be written to
                var o = chart.config.options = chart.config.options || {};
                Object.keys(o.scales || {}).forEach(function (axis) {
                    var s = o.scales[axis];
                    s.ticks = s.ticks || {};
                    s.ticks.color = c;
                    if (!s.grid || s.grid.display !== false) { s.grid = s.grid || {}; s.grid.color = g; }
                });
                o.plugins = o.plugins || {};
                o.plugins.legend = o.plugins.legend || {};
                o.plugins.legend.labels = o.plugins.legend.labels || {};
                o.plugins.legend.labels.color = c;
                // Doughnut / pie: the gap between slices takes the card colour
                var type = chart.config.type;
                if (type === 'doughnut' || type === 'pie')
                    (chart.data.datasets || []).forEach(function (ds) { ds.borderColor = isDark() ? '#181b25' : '#ffffff'; });
                chart.update('none');
            } catch (e) { /* a chart we cannot restyle keeps its colours */ }
        });
    }

    function apply(theme) {
        if (theme === 'dark') document.documentElement.setAttribute('data-theme', 'dark');
        else document.documentElement.removeAttribute('data-theme');
        syncButtons();
        recolorCharts();
    }

    window.DocCareTheme = {
        isDark: isDark,
        recolorCharts: recolorCharts,
        ink: ink,
        grid: grid,
        toggle: function () {
            var theme = isDark() ? 'light' : 'dark';
            store(theme);
            apply(theme);
        }
    };

    // Paper is white: print (Appointment Report "Print / Save as PDF") in the light theme, then switch back
    var printedDark = false;
    window.addEventListener('beforeprint', function () { printedDark = isDark(); if (printedDark) apply('light'); });
    window.addEventListener('afterprint', function () { if (printedDark) apply('dark'); printedDark = false; });

    document.addEventListener('DOMContentLoaded', syncButtons);
    // Charts are drawn by page scripts as the page loads; give them the theme's colours afterwards
    window.addEventListener('load', function () { setTimeout(recolorCharts, 0); });
})();
