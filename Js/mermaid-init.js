import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
export function initMermaid() {
    if (window.mermaid) {
        window.mermaid.initialize({ startOnLoad: false });
        window.mermaid.init(undefined, document.querySelectorAll(".mermaid"));
    }
}