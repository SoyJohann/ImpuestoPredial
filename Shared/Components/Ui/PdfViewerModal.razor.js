let keyListener;
let messageListener;

export function attach(dotnet) {
    keyListener = event => {
        if (event.key === "Escape") {
            event.preventDefault();
            dotnet.invokeMethodAsync("ViewerClose");
        }
    };
    messageListener = event => {
        if (event.origin !== window.location.origin) return;
        if (event.data?.type === "pdfjs-error") dotnet.invokeMethodAsync("ViewerFailed", String(event.data.reason ?? ""));
        if (event.data?.type === "pdfjs-loaded") dotnet.invokeMethodAsync("ViewerLoaded");
        if (event.data?.type === "pdfjs-escape") dotnet.invokeMethodAsync("ViewerClose");
    };
    window.addEventListener("keydown", keyListener);
    window.addEventListener("message", messageListener);
}

export function detach() {
    if (keyListener) window.removeEventListener("keydown", keyListener);
    if (messageListener) window.removeEventListener("message", messageListener);
}
