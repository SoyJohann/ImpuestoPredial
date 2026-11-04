const reportErrors = () => {
  const app = window.PDFViewerApplication;
  if (!app?.eventBus) {
    window.setTimeout(reportErrors, 50);
    return;
  }
  app.eventBus.on("documenterror", event => {
    window.parent.postMessage({ type: "pdfjs-error", reason: String(event.reason ?? event.message ?? "") }, window.location.origin);
  });
  app.eventBus.on("documentloaded", () => {
    window.parent.postMessage({ type: "pdfjs-loaded" }, window.location.origin);
  });
};
reportErrors();
window.addEventListener("keydown", event => {
  if (event.key === "Escape") {
    window.parent.postMessage({ type: "pdfjs-escape" }, window.location.origin);
  }
});
