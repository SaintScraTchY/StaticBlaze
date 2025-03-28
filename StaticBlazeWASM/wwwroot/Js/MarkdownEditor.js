import Editor from 'https://esm.sh/@toast-ui/editor';

export function initEditor(element, dotNetRef, initialValue) {
    // Calculate height minus headers/margins
    const containerHeight = `${element.parentElement.clientHeight - 40}px`;

    const editor = new Editor({
        el: element,
        initialValue: initialValue || '',
        previewStyle: 'vertical',
        height: containerHeight, // Use calculated height
        usageStatistics: false,
        hooks: {
            change: () => {
                const markdown = editor.getMarkdown();
                dotNetRef.invokeMethodAsync('UpdateEditorValue', markdown);
            },
            addImageBlobHook: async (blob, callback) => {
                const compressedBlob = await compressImage(blob, 0.7); // 70% Quality
                const reader = new FileReader();
                reader.onload = () => {
                    callback(reader.result, compressedBlob.name || "image");
                };
                reader.readAsDataURL(compressedBlob);
            }
        }
    });

    // Handle window resize
    const resizeHandler = () => {
        const newHeight = `${element.parentElement.clientHeight - 40}px`;
        editor.setHeight(newHeight);
    };

    window.addEventListener('resize', resizeHandler);

    // Return cleanup function
    return {
        destroy: () => {
            window.removeEventListener('resize', resizeHandler);
            editor.destroy();
        },
        setMarkdown: (content) => editor.setMarkdown(content)
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
