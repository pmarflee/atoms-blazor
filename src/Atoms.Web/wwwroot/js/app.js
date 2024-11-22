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
}