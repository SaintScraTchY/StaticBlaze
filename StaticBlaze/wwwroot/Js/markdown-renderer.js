window.renderMarkdown = function (markdownText, outputElementId) {
    const md = window.markdownit({
        html: true,
        highlight: function (str, lang) {
            if (lang && hljs.getLanguage(lang)) {
                try {
                    return '<pre class="hljs"><code>' +
                        hljs.highlight(str, { language: lang, ignoreIllegals: true }).value +
                        '</code></pre>';
                } catch (__) {}
            }
            return '<pre class="hljs"><code>' + md.utils.escapeHtml(str) + '</code></pre>';
        }
    })
        .use(window.markdownitContainer, 'warning')
        .use(window.markdownitAbbr)
        .use(window.markdownitFootnote);

    // Replace mermaid blocks
    markdownText = markdownText.replace(/```(mermaid|graph|sequenceDiagram|gantt|classDiagram)[\r\n]+([\s\S]*?)```/g, (_, __, code) => {
        return `<div class="mermaid">${code.trim()}</div>`;
    });


    const html = md.render(markdownText);
    document.getElementById(outputElementId).innerHTML = html;

    // Initialize Mermaid
    if (window.mermaid) {
        window.mermaid.initialize({ startOnLoad: false });
        window.mermaid.init(undefined, document.querySelectorAll(".mermaid"));
    }
};
