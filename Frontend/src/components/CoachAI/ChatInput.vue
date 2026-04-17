<template>
    <aside class="sidebar">
        <div class="sidebar-logo">
            <div class="logo-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
                    <defs>
                        <linearGradient id="lg" x1="0%" y1="0%" x2="100%" y2="100%">
                            <stop offset="0%" stop-color="#00e5ff" />
                            <stop offset="100%" stop-color="#7c3aed" />
                        </linearGradient>
                    </defs>
                    <path d="M12 2L13.5 8.5L20 7L15.5 12L20 17L13.5 15.5L12 22L10.5 15.5L4 17L8.5 12L4 7L10.5 8.5L12 2Z"
                        fill="url(#lg)" />
                </svg>
            </div>
            <span class="logo-text">COACH<em>AI</em></span>
        </div>

        <nav class="sidebar-nav">
            <a class="nav-item active" href="#">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
                </svg>
                <span>Chat</span>
            </a>
            <a class="nav-item" href="#">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M3 18v-6a9 9 0 0 1 18 0v6" />
                    <path
                        d="M21 19a2 2 0 0 1-2 2h-1a2 2 0 0 1-2-2v-3a2 2 0 0 1 2-2h3zM3 19a2 2 0 0 0 2 2h1a2 2 0 0 0 2-2v-3a2 2 0 0 0-2-2H3z" />
                </svg>
                <span>Rutinas</span>
            </a>
            <a class="nav-item" href="#">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
                </svg>
                <span>Progreso</span>
            </a>
            <a class="nav-item" href="#">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="3" />
                    <path d="M19.07 4.93a10 10 0 0 1 0 14.14M4.93 4.93a10 10 0 0 0 0 14.14" />
                </svg>
                <span>Nutrición</span>
            </a>
        </nav>

        <div class="stats-section">
            <p class="stats-label">Estadísticas del día</p>
            <div class="stat-card">
                <div class="stat-top">
                    <span class="stat-icon">🔥</span>
                    <span class="stat-value">847 <span class="stat-unit">kcal</span></span>
                </div>
                <div class="stat-bar-wrap">
                    <div class="stat-bar orange" style="width: 68%" />
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-top">
                    <span class="stat-icon">💧</span>
                    <span class="stat-value">1.8 <span class="stat-unit">L</span></span>
                </div>
                <div class="stat-bar-wrap">
                    <div class="stat-bar cyan" style="width: 45%" />
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-top">
                    <span class="stat-icon">👟</span>
                    <span class="stat-value">6,240 <span class="stat-unit">pasos</span></span>
                </div>
                <div class="stat-bar-wrap">
                    <div class="stat-bar violet" style="width: 62%" />
                </div>
            </div>
        </div>

        <div class="sidebar-footer">
            <div class="user-chip">
                <div class="user-avatar">A</div>
                <div class="user-info">
                    <span class="user-name">Atleta</span>
                    <span class="user-plan">Plan Gratis</span>
                </div>
                <div class="upgrade-dot" title="Upgrade disponible" />
            </div>
        </div>
    </aside>
</template>

<style scoped>
.sidebar {
    width: 220px;
    flex-shrink: 0;
    background: rgba(9, 9, 22, 0.9);
    border-right: 1px solid var(--border);
    display: flex;
    flex-direction: column;
    padding: 24px 16px;
    gap: 28px;
}

.sidebar-logo {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 0 4px;
}

.logo-icon {
    width: 30px;
    height: 30px;
    border-radius: 8px;
    background: linear-gradient(135deg, #0d0d2a, #191940);
    border: 1px solid rgba(0, 229, 255, 0.25);
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 0 12px rgba(0, 229, 255, 0.15);
}

.logo-text {
    font-family: 'Bebas Neue', sans-serif;
    font-size: 20px;
    letter-spacing: 2px;
    color: var(--text-primary);
}

.logo-text em {
    font-style: normal;
    background: linear-gradient(90deg, #00e5ff, #7c3aed);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
}

.sidebar-nav {
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 10px 12px;
    border-radius: var(--radius-sm);
    color: var(--text-secondary);
    text-decoration: none;
    font-size: 13.5px;
    font-weight: 500;
    transition: all 0.15s ease;
}

.nav-item:hover {
    color: var(--text-primary);
    background: rgba(255, 255, 255, 0.04);
}

.nav-item.active {
    background: linear-gradient(135deg, rgba(0, 229, 255, 0.1), rgba(124, 58, 237, 0.1));
    color: #00e5ff;
    border: 1px solid rgba(0, 229, 255, 0.2);
    box-shadow: inset 0 1px 0 rgba(0, 229, 255, 0.1);
}

.stats-section {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.stats-label {
    font-size: 10.5px;
    text-transform: uppercase;
    letter-spacing: 1.2px;
    color: var(--text-muted);
    padding: 0 4px;
}

.stat-card {
    background: var(--bg-card);
    border: 1px solid var(--border);
    border-radius: 10px;
    padding: 10px 12px;
}

.stat-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 8px;
}

.stat-icon {
    font-size: 15px;
}

.stat-value {
    font-size: 14px;
    font-weight: 600;
    color: var(--text-primary);
}

.stat-unit {
    font-size: 11px;
    color: var(--text-muted);
    font-weight: 400;
}

.stat-bar-wrap {
    width: 100%;
    height: 3px;
    background: rgba(255, 255, 255, 0.06);
    border-radius: 99px;
}

.stat-bar {
    height: 100%;
    border-radius: 99px;
    transition: width 0.8s ease;
}

.stat-bar.cyan {
    background: linear-gradient(90deg, #00e5ff, #0077ff);
    box-shadow: 0 0 8px rgba(0, 229, 255, 0.4);
}

.stat-bar.orange {
    background: linear-gradient(90deg, #ff6b35, #f97316);
    box-shadow: 0 0 8px rgba(249, 115, 22, 0.4);
}

.stat-bar.violet {
    background: linear-gradient(90deg, #7c3aed, #a78bfa);
    box-shadow: 0 0 8px rgba(124, 58, 237, 0.4);
}

.sidebar-footer {
    margin-top: auto;
}

.user-chip {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 10px 12px;
    background: var(--bg-card);
    border: 1px solid var(--border);
    border-radius: 10px;
    position: relative;
}

.user-avatar {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: linear-gradient(135deg, #7c3aed, #00a8cc);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 13px;
    font-weight: 700;
    color: white;
    box-shadow: 0 0 12px rgba(124, 58, 237, 0.4);
}

.user-name {
    display: block;
    font-size: 13px;
    font-weight: 600;
    color: var(--text-primary);
}

.user-plan {
    font-size: 11px;
    color: var(--text-muted);
}

.upgrade-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #f000b8;
    box-shadow: 0 0 8px rgba(240, 0, 184, 0.7);
    margin-left: auto;
    flex-shrink: 0;
}
</style>