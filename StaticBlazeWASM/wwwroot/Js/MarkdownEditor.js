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