/* Search-as-you-type for staff lists (Appointments, Patients, Doctor's Appointments).
   A search box with data-live-search="<postback target>" searches 0.6 s after the last key, or at once on Enter.
   The lists sit in an UpdatePanel, which redraws the search box too: without help the cursor is lost and
   letters typed while the list reloads disappear. After each reload the box gets back what was typed
   (and searches again if it changed) and the cursor where it was. */
(function () {
    var DELAY = 600;
    var s = { timer: null, sent: null, typed: null, caret: null, id: null, focused: false, busy: false, again: false };

    function boxOf(el) { return el && el.closest ? el.closest('[data-live-search]') : null; }

    function remember(box) {
        s.id = box.id;
        s.typed = box.value;
        s.focused = true;
        try { s.caret = box.selectionStart; } catch (e) { s.caret = null; }
    }

    function schedule(box) {
        clearTimeout(s.timer);
        s.timer = setTimeout(function () { run(box.id); }, DELAY);
    }

    function run(id) {
        var box = document.getElementById(id);
        if (!box) return;
        if (s.busy) { s.again = true; return; }            // search again with the latest text when this one is back
        if (box.value === s.sent) return;                   // nothing new to search for
        s.sent = box.value;
        s.busy = true;
        __doPostBack(box.getAttribute('data-live-search'), '');
    }

    document.addEventListener('input', function (e) {
        var box = boxOf(e.target);
        if (!box) return;
        remember(box);
        schedule(box);
    });

    document.addEventListener('keydown', function (e) {
        var box = boxOf(e.target);
        if (!box || e.key !== 'Enter') return;
        e.preventDefault();                                 // Enter must not submit the page with another button
        remember(box);
        clearTimeout(s.timer);
        run(box.id);
    });

    document.addEventListener('keyup', function (e) {       // arrow keys / Home / End move the cursor
        var box = boxOf(e.target);
        if (box) try { s.caret = box.selectionStart; } catch (err) { }
    });

    // Clicking somewhere else while the list reloads: do not pull the cursor back into the box
    document.addEventListener('focusin', function (e) {
        if (!boxOf(e.target)) s.focused = false;
    });

    function afterReload() {
        s.busy = false;
        var box = s.id && document.getElementById(s.id);
        if (!box) return;
        if (s.typed !== null && box.value !== s.typed) box.value = s.typed;   // keep letters typed meanwhile
        if (s.focused && document.activeElement !== box) {
            box.focus();
            var at = s.caret === null || s.caret > box.value.length ? box.value.length : s.caret;
            try { box.setSelectionRange(at, at); } catch (e) { }
        }
        if (s.again || box.value !== s.sent) { s.again = false; schedule(box); }
    }

    window.addEventListener('load', function () {
        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager)
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(afterReload);
        // after a full page load (no UpdatePanel), start from what the server searched for
        var box = document.querySelector('[data-live-search]');
        if (box) s.sent = box.value;
    });
})();
