<template>
  <header class="topbar">

    <button class="menu-btn" @click="$emit('toggle-sidebar')"       :aria-label="t('chat.menuAria')">
      <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
        <line x1="3" y1="6"  x2="21" y2="6"/>
        <line x1="3" y1="12" x2="21" y2="12"/>
        <line x1="3" y1="18" x2="21" y2="18"/>
      </svg>
    </button>

    <div class="topbar-id">
      <div class="topbar-ava">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
          <path d="M12 2L14 9H21L15.5 13.5L17.5 21L12 16.5L6.5 21L8.5 13.5L3 9H10L12 2Z" fill="currentColor"/>
        </svg>
        <span class="online-dot"/>
      </div>
      <div>
        <p class="topbar-name">CoachAI</p>
        <p class="topbar-status">
          <span class="pulse-dot"/>
          {{ t('chat.onlineStatus') }}
        </p>
      </div>
    </div>

    <div class="topbar-badge">
      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
      </svg>
      Fitness AI
    </div>

  </header>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
// Emitimos un evento para que el componente padre controle la apertura del sidebar en móviles
defineEmits<{
  'toggle-sidebar': []
}>()
</script>

<style scoped>
.topbar {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 16px 22px;
  border-bottom: 1px solid var(--c-border);
  background: var(--c-surface);
  flex-shrink: 0;
}

.menu-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 34px;
  height: 34px;
  border: 1px solid var(--c-border);
  border-radius: var(--r-sm);
  background: transparent;
  color: var(--c-muted);
  cursor: pointer;
  transition: all .15s;
  flex-shrink: 0;
}

.menu-btn:hover {
  background: var(--c-bg);
  color: var(--c-text);
}

@media (min-width: 700px) {
  .menu-btn { display: none; }
}

.topbar-id {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.topbar-ava {
  position: relative;
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background: linear-gradient(135deg, #fff1eb, #ffe0cc);
  border: 2px solid var(--c-accent);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--c-accent);
  flex-shrink: 0;
}

.online-dot {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 11px;
  height: 11px;
  background: #22c55e;
  border: 2px solid var(--c-surface);
  border-radius: 50%;
}

.topbar-name {
  font-family: 'Syne', sans-serif;
  font-size: 17px;
  font-weight: 800;
  color: var(--c-text);
  letter-spacing: .3px;
  line-height: 1;
  margin-bottom: 3px;
}

.topbar-status {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 12px;
  color: var(--c-muted);
}

.pulse-dot {
  display: inline-block;
  width: 6px;
  height: 6px;
  background: #22c55e;
  border-radius: 50%;
  flex-shrink: 0;
}

.topbar-badge {
  display: flex;
  align-items: center;
  gap: 5px;
  padding: 6px 13px;
  border-radius: 99px;
  background: #fff1eb;
  border: 1px solid #fbd5bb;
  color: var(--c-accent);
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
  flex-shrink: 0;
}
</style>