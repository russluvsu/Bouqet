/* Jslogin.js | Bouqet sign-in page
   Plain JavaScript, no libraries. This only changes wording and the
   look of the controls. The server decides the real role at sign-in. */
(function () {
    'use strict';

    var COPY = {
        client: {
            heading: 'Welcome back',
            subtext: 'Sign in to follow your wedding plans.',
            note: 'Your planner will give you your login details.',
            label: 'Client',
            forgot: 'Ask your planner to make you a new password.'
        },
        planner: {
            heading: 'Planner sign in',
            subtext: 'Manage your clients and weddings.',
            note: 'Add clients and issue their login details from your dashboard.',
            label: 'Planner',
            forgot: 'Planner password resets are not available yet. Please contact the system owner.'
        }
    };

    function el(id) { return document.getElementById(id); }

    var mode = el('hfMode');
    var heading = el('heading');
    var subtext = el('subtext');
    var footNote = el('footNote');
    var toggle = el('roleToggle');
    var roleLabel = el('roleLabel');
    var forgotButton = el('forgotButton');
    var forgotNote = el('forgotNote');
    var password = el('txtPassword');
    var showButton = el('togglePassword');

    if (!mode || !toggle) { return; }

    function currentMode() {
        return mode.value === 'planner' ? 'planner' : 'client';
    }

    function applyMode(next) {
        var copy = COPY[next];
        mode.value = next;
        heading.textContent = copy.heading;
        subtext.textContent = copy.subtext;
        footNote.textContent = copy.note;
        roleLabel.textContent = copy.label;
        forgotNote.textContent = copy.forgot;
        toggle.classList.toggle('is-planner', next === 'planner');
        toggle.setAttribute('aria-checked', next === 'planner' ? 'true' : 'false');
    }

    toggle.addEventListener('click', function () {
        applyMode(currentMode() === 'planner' ? 'client' : 'planner');
    });

    forgotButton.addEventListener('click', function () {
        var open = forgotNote.hidden;
        forgotNote.hidden = !open;
        forgotButton.setAttribute('aria-expanded', open ? 'true' : 'false');
    });

    showButton.addEventListener('click', function () {
        var reveal = password.type === 'password';
        password.type = reveal ? 'text' : 'password';
        showButton.textContent = reveal ? 'Hide' : 'Show';
        showButton.setAttribute('aria-pressed', reveal ? 'true' : 'false');
    });

    applyMode(currentMode());
})();