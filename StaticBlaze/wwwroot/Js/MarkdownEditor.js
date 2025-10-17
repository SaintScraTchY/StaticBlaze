// wwwroot/js/MarkdownEditor.js
let editorInstance = null;

// Load external script dynamically
async function loadScript(src) {
    if (document.querySelector(`script[src="${src}"]`)) return;
    await new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = src;
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Load external stylesheet dynamically
function loadCSS(href) {
    if (document.querySelector(`link[href="${href}"]`)) return;
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = href;
    document.head.appendChild(link);
}

// Wait until ToastUI is ready
async function ensureToastUI() {
    if (window.toastui?.Editor) return;
    loadCSS('https://uicdn.toast.com/editor/latest/toastui-editor.min.css');

    await loadScript('https://uicdn.toast.com/editor/latest/toastui-editor-all.min.js');
    await loadScript('https://uicdn.toast.com/editor-plugin-code-syntax-highlight/latest/toastui-editor-plugin-code-syntax-highlight-all.min.js');
    await loadScript('https://cdn.jsdelivr.net/npm/prismjs@1.29.0/prism.min.js');
    await loadScript('https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js');

    // Configure Mermaid globally
    window.mermaid.initialize({
        startOnLoad: false,
        theme: 'default',
        securityLevel: 'loose'
    });
}

// Render Mermaid diagrams
function renderMermaidDiagrams(container) {
    const mermaidBlocks = container.querySelectorAll('pre code.language-mermaid, pre.mermaid');
    mermaidBlocks.forEach(block => {
        const pre = block.tagName === 'CODE' ? block.parentElement : block;
        if (pre.tagName === 'PRE') {
            pre.className = 'mermaid';
            pre.textContent = block.textContent || pre.textContent;
            try {
                window.mermaid.run({ nodes: [pre] });
            } catch (e) {
                console.error('Mermaid render failed:', e);
                pre.textContent = '❌ Mermaid render error';
                pre.style.color = 'red';
            }
        }
    });
}

// Initialize editor
export async function initEditor(element, dotNetRef, initialValue) {
    await ensureToastUI();
    const { Editor } = window.toastui;
    const codeSyntaxHighlight = window.toastui.Editor.plugin.codeSyntaxHighlight;

    element.innerHTML = '';

    const editor = new Editor({
        el: element,
        initialValue: initialValue || '',
        initialEditType: 'wysiwyg',
        previewStyle: 'tab',
        height: '500px',
        usageStatistics: false,
        language: 'en',
        plugins: [codeSyntaxHighlight],
        hooks: {
            change: () => dotNetRef.invokeMethodAsync('UpdateEditorValue', editor.getMarkdown()),
            addImageBlobHook: async (blob, callback) => {
                const compressedBlob = await compressImage(blob, 0.7);
                const arrayBuffer = await compressedBlob.arrayBuffer();
                const url = await dotNetRef.invokeMethodAsync(
                    'HandleImageUpload',
                    Array.from(new Uint8Array(arrayBuffer)),
                    compressedBlob.type
                );
                callback(url, 'image.jpg');
            },
            beforePreviewRender: () => renderMermaidDiagrams(element)
        }
    });

    setTimeout(() => renderMermaidDiagrams(element), 100);
    editorInstance = editor;

    return {
        destroy: () => editor.destroy(),
        setMarkdown: (content) => {
            editor.setMarkdown(content);
            setTimeout(() => renderMermaidDiagrams(element), 50);
        }
    };
}

// Helper: Compress uploaded image
async function compressImage(blob, quality = 0.7) {
    return new Promise((resolve) => {
        const img = new Image();
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        const reader = new FileReader();
        reader.onload = e => img.src = e.target.result;
        reader.readAsDataURL(blob);

        img.onload = () => {
            canvas.width = img.width;
            canvas.height = img.height;
            ctx.drawImage(img, 0, 0);
            canvas.toBlob(resolve, 'image/jpeg', quality);
        };
    });
}
