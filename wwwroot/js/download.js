// PortfolioForge — browser download helper invoked from Blazor via JSInterop
window.portfolioForge = {
    downloadFile: function (filename, base64Data, mimeType) {
        try {
            const binary = atob(base64Data);
            const bytes = new Uint8Array(binary.length);
            for (let i = 0; i < binary.length; i++) {
                bytes[i] = binary.charCodeAt(i);
            }
            const blob = new Blob([bytes], { type: mimeType || 'application/octet-stream' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            // Revoke after a small delay so the download actually starts
            setTimeout(() => URL.revokeObjectURL(url), 500);
        } catch (err) {
            console.error('Download failed:', err);
            alert('Download failed: ' + err.message);
        }
    }
};
