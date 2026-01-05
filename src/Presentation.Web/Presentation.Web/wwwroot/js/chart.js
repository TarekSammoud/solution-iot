// Chart.js module for Blazor
let charts = {};

export function createChart(canvasId, config) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error(`Canvas with id '${canvasId}' not found`);
        return null;
    }
    
    const chart = new Chart(ctx, config);
    charts[canvasId] = chart;
    return chart;
}

export function updateChart(canvasId, data) {
    const chart = charts[canvasId];
    if (chart) {
        chart.data = data;
        chart.update();
    }
}

export function destroyChart(canvasId) {
    const chart = charts[canvasId];
    if (chart) {
        chart.destroy();
        delete charts[canvasId];
    }
}

// Load Chart.js from CDN if not already loaded
if (typeof Chart === 'undefined') {
    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.js';
    script.onload = function() {
        console.log('Chart.js loaded successfully');
    };
    document.head.appendChild(script);
}