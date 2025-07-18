let mermaidInitialized = false;

async function initializeMermaid() {
    if (!mermaidInitialized) {
        const { default: mermaid } = await import('https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs');
        mermaid.initialize({
            startOnLoad: false,
            theme: document.documentElement.classList.contains('dark') ? 'dark' : 'default',
            fontSize: 16
        });
        mermaidInitialized = true;
        return mermaid;
    }
}

export async function renderMarkdown(element, content) {
    try {
        // Initialize markdown-it with plugins (already loaded in index.html)
        const md = window.markdownit({
            html: true,
            linkify: true,
            typographer: true,
            highlight: function (str, lang) {
                if (lang && window.hljs.getLanguage(lang)) {
                    try {
                        return `<pre class="!p-0"><code class="hljs language-${lang}">${window.hljs.highlight(str, { language: lang }).value}</code></pre>`;
                    } catch (_) {}
                }
                return `<pre class="!p-0"><code class="hljs">${md.utils.escapeHtml(str)}</code></pre>`;
            }
        })
        .use(window.markdownitFootnote)
        .use(window.markdownitAbbr);

        // Replace mermaid code blocks with mermaid divs
        const processedContent = content.replace(/```mermaid([\s\S]*?)```/g, (_, code) => {
            return `<div class="mermaid">${code.trim()}</div>`;
        });

        // Render markdown
        const html = md.render(processedContent);
        element.innerHTML = html;

        // Initialize syntax highlighting
        element.querySelectorAll('pre code').forEach((block) => {
            window.hljs.highlightElement(block);
        });

        // Initialize mermaid diagrams
        const mermaidDiagrams = element.querySelectorAll('.mermaid');
        if (mermaidDiagrams.length > 0) {
            const mermaid = await initializeMermaid();
            await mermaid.run({ nodes: Array.from(mermaidDiagrams) });
        }

        // Add copy button to code blocks
        element.querySelectorAll('pre').forEach(pre => {
            const wrapper = document.createElement('div');
            wrapper.className = 'relative group';
            pre.parentNode.insertBefore(wrapper, pre);
            wrapper.appendChild(pre);

            const button = document.createElement('button');
            button.className = 'absolute top-2 right-2 p-2 rounded bg-gray-800/30 hover:bg-gray-800/50 invisible group-hover:visible transition-all';
            button.innerHTML = `<svg class="w-5 h-5 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7v8a2 2 0 002 2h6M8 7V5a2 2 0 012-2h4.586a1 1 0 01.707.293l4.414 4.414a1 1 0 01.293.707V15a2 2 0 01-2 2h-2M8 7H6a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2v-2"/>
            </svg>`;
            
            button.addEventListener('click', async () => {
                const code = pre.querySelector('code').innerText;
                await navigator.clipboard.writeText(code);
                
                const originalHTML = button.innerHTML;
                button.innerHTML = `<svg class="w-5 h-5 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>`;
                
                setTimeout(() => {
                    button.innerHTML = originalHTML;
                }, 2000);
            });
            
            wrapper.appendChild(button);
        });
    } catch (error) {
        console.error('Error rendering markdown:', error);
        element.innerHTML = `<div class="text-red-500">Error rendering content: ${error.message}</div>`;
    }
}
