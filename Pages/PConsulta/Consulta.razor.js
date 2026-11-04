function downloadFileFromStream(fileName, contentStreamReference) {
    contentStreamReference.arrayBuffer().then(arrayBuffer => {
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);
        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? '';
        anchorElement.click();
        anchorElement.remove();
        URL.revokeObjectURL(url);
    });
}

window.downloadFileFromStream = downloadFileFromStream;

