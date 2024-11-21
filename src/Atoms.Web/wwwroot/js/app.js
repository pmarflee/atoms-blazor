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
}