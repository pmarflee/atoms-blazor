vNotify.options.position = vNotify.positionOption.bottomRight;

window.App = class {
    static setDefaultColourScheme() {
        document.documentElement.classList.remove('high-contrast');
    }
    static setAlternateColourScheme() {
        document.documentElement.classList.add('high-contrast');
    }
    static setDefaultAtomShape() {
        document.documentElement.classList.remove('shaped-atoms');
    }
    static setVariedAtomShape() {
        document.documentElement.classList.add('shaped-atoms');
    }
    static setCursor(playerId) {
        const highContrast = document.documentElement.classList.contains('high-contrast') ? '-hc' : '';
        document.documentElement.style.cursor = 'url("images/cursor-player' + playerId + highContrast + '.svg") 18 3, default';
    }
    static setDefaultCursor() {
        document.documentElement.style.cursor = 'url("images/cursor.svg") 18 3, default';
    }
    static startMusic() {
        if (document.getElementById('music-intro')) {
            document.getElementById('music-intro').play();
            document.getElementById('music-intro').addEventListener('ended', App.#loopMusic);
        }
    }
    static #loopMusic() {
        if (document.getElementById('music')) {
            document.getElementById('music').play();
        }
    }
    static stopMusic() {
        if (document.getElementById('music-intro')) {
            document.getElementById('music-intro').removeEventListener('ended', App.#loopMusic);
        }
        if (document.getElementById('music')) {
            document.getElementById('music').pause();
            document.getElementById('music').load();
        }
    }
    static copyToClipboard(text) {
        navigator.clipboard.writeText(text)
            .then(function () {
                vNotify.success({ text: "Link Copied to Clipboard!", title: "Send Invite" });
            })
            .catch(function (error) {
                vNotify.error({ text: error, title: "Send Invite" });
            });
    }
    static notify(message) {
        vNotify.info({ text: message })
    }
}