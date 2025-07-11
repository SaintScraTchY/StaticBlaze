import Editor from 'https://esm.sh/@toast-ui/editor';
import chart from 'https://esm.sh/@toast-ui/editor-plugin-chart';
import codeSyntaxHighlight from 'https://esm.sh/@toast-ui/editor-plugin-code-syntax-highlight';
import Prism from 'https://esm.sh/prismjs';
import mermaidPlugin from 'https://esm.sh/@toast-ui/editor-plugin-mermaid';

export function initEditor(element, dotNetRef, initialValue) {
    const editor = new Editor({
        el: element,
        initialEditType: 'wysiwyg', // Use only rendered view
        previewStyle: 'tab', // hides split view
        height: `${element.parentElement.clientHeight - 40}px`,
        usageStatistics: false,
        language: 'en',
        plugins: [codeSyntaxHighlight, [chart, { usageStatistics: false }], [mermaidPlugin, { mermaid }]],
        hooks: {
            change: () => {
                dotNetRef.invokeMethodAsync('UpdateEditorValue', editor.getMarkdown());
            },
            addImageBlobHook: async (blob, callback) => {
                const compressedBlob = await compressImage(blob, 0.7);
                const arrayBuffer = await compressedBlob.arrayBuffer();
                const url = await dotNetRef.invokeMethodAsync(
                    'HandleImageUpload',
                    Array.from(new Uint8Array(arrayBuffer)),
                    compressedBlob.type
                );
                callback(url, compressedBlob.name);
            }
        }
    });

    // RTL Enhancement
    const direction = document.documentElement.dir || 'ltr';
    if (direction === 'rtl') {
        element.querySelector('.toastui-editor-contents')?.classList.add('rtl');
    }

    return {
        destroy: () => editor.destroy(),
        setMarkdown: content => editor.setMarkdown(content)
    };
}

async function compressImage(blob, quality = 0.7) {
    return new Promise((resolve) => {
        const img = new Image();
        const reader = new FileReader();

        reader.readAsDataURL(blob);
        reader.onload = (event) => {
            img.src = event.target.result;
        };

        img.onload = () => {
            const canvas = document.createElement("canvas");
            const ctx = canvas.getContext("2d");
            canvas.width = img.width;
            canvas.height = img.height;
            ctx.drawImage(img, 0, 0);
            canvas.toBlob((compressedBlob) => {
                resolve(compressedBlob);
            }, "image/jpeg", quality);
        };
    });
}