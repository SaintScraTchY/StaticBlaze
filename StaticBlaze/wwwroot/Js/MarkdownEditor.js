import Editor from 'https://esm.sh/@toast-ui/editor';
export function initEditor(element, dotNetRef, initialValue) {
    const editor = new Editor({
        el: element,
        initialValue: initialValue || '',
        previewStyle: 'vertical',
        height: `${element.parentElement.clientHeight - 40}px`,
        usageStatistics: false,
        hooks: {
            change: () => {
                dotNetRef.invokeMethodAsync('UpdateEditorValue', editor.getMarkdown());
            },
            addImageBlobHook: async (blob, callback) => {
                // compress first
                const compressedBlob = await compressImage(blob, 0.7);
                // send to .NET
                const arrayBuffer = await compressedBlob.arrayBuffer();
                // call .NET, get URL
                const url = await dotNetRef.invokeMethodAsync(
                    'HandleImageUpload',
                    Array.from(new Uint8Array(arrayBuffer)),
                    compressedBlob.type
                );
                callback(url, compressedBlob.name);
            }
        }
    });

    // resize logic omitted for brevity…
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
