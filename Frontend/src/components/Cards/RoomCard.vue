<script setup lang="ts">
defineProps<{
  room: {
    id: number
    name: string
    minlevel: number
    minstats: number
    minconsistency: number
    description: string
    date: string
    localization: string
  }
  memberCount: number
  isStaff: boolean
}>()

defineEmits<{
  (e: 'view', id: number): void
  (e: 'edit', room: any): void
}>()

const difficulties: Record<string, { label: string; color: string; rgb: string; dim: string; secondRgb: string }> = {
  common: { label: 'Común', color: '#94a3b8', rgb: '148,163,184', secondRgb: '71,85,105', dim: 'rgba(148,163,184,0.08)' },
  rare: { label: 'Raro', color: '#38bdf8', rgb: '56,189,248', secondRgb: '14,165,233', dim: 'rgba(56,189,248,0.08)' },
  epic: { label: 'Épico', color: '#f472b6', rgb: '244,114,182', secondRgb: '236,72,153', dim: 'rgba(244,114,182,0.08)' },
  legendary: { label: 'Legendario', color: '#fbbf24', rgb: '251,191,36', secondRgb: '245,158,11', dim: 'rgba(251,191,36,0.08)' },
}

const getRoomDifficulty = (level: number) => {
  if (level >= 50) return 'legendary'
  if (level >= 30) return 'epic'
  if (level >= 15) return 'rare'
  return 'common'
}

const getRoomIcon = (level: number) => {
  if (level >= 50) return '🐉'
  if (level >= 30) return '⭐'
  if (level >= 15) return '💎'
  return '🎯'
}
</script>

<template>
  <div class="card-stack" :style="{
    '--accent': difficulties[getRoomDifficulty(room.minlevel)].color,
    '--accent-rgb': difficulties[getRoomDifficulty(room.minlevel)].rgb,
    '--accent2-rgb': difficulties[getRoomDifficulty(room.minlevel)].secondRgb,
    '--accent-dim': difficulties[getRoomDifficulty(room.minlevel)].dim,
  }">
    <!-- Capas de profundidad -->
    <div class="stack-layer layer-3"></div>
    <div class="stack-layer layer-2"></div>
    <div class="stack-layer layer-1"></div>

    <!-- Carta principal -->
    <div class="room-card">
      <div class="card-scanline"></div>

      <div class="card-bar">
        <div class="bar-pulse"></div>
      </div>

      <!-- Avatar -->
      <div class="card-avatar">
        <div class="avatar-box">
          <span>{{ getRoomIcon(room.minlevel) }}</span>
        </div>
        <div class="avatar-ring"></div>
      </div>

      <!-- Contenido principal -->
      <div class="card-main">
        <div class="top-row">
          <div class="name-block">
            <span class="id-tag">#{{ room.id.toString().padStart(3, '0') }}</span>
            <h3 class="room-name">{{ room.name }}</h3>
          </div>
          <div class="diff-badge">
            <span class="diff-dot"></span>
            {{ difficulties[getRoomDifficulty(room.minlevel)].label }}
          </div>
        </div>

        <p v-if="room.description" class="room-desc">{{ room.description }}</p>

        <div class="chips-row">
          <div class="chip">
            <span class="chip-lbl">Nivel</span>
            <span class="chip-val">{{ room.minlevel }}</span>
          </div>
          <div class="chip">
            <span class="chip-lbl">Stats</span>
            <span class="chip-val">{{ room.minstats }}</span>
          </div>
          <div class="chip">
            <span class="chip-lbl">Consist.</span>
            <span class="chip-val">{{ room.minconsistency }}%</span>
          </div>
          <div class="chip" v-if="room.localization">
            <span class="chip-lbl">📍</span>
            <span class="chip-val chip-val--muted">{{ room.localization }}</span>
          </div>
          <div class="chip" v-if="room.date">
            <span class="chip-lbl">📅</span>
            <span class="chip-val chip-val--muted">{{ room.date }}</span>
          </div>
        </div>
      </div>

      <!-- Panel derecho -->
      <div class="card-right">
        <div class="members-wrap">
          <div class="members-ring-wrap">
            <svg viewBox="0 0 40 40" class="members-svg">
              <circle cx="20" cy="20" r="17" fill="none" stroke="rgba(255,255,255,0.08)" stroke-width="2.5" />
              <circle cx="20" cy="20" r="17" fill="none" stroke="var(--accent)" stroke-width="2.5"
                stroke-dasharray="107 107" stroke-linecap="round" transform="rotate(-90 20 20)" />
            </svg>
            <span class="members-num">{{ memberCount }}</span>
          </div>
          <span class="members-lbl">miembros</span>
        </div>

        <div class="btns">
          <button v-if="isStaff" class="btn btn-edit" @click="$emit('edit', room)">
            ✏️ Editar
          </button>
          <button class="btn btn-join" @click="$emit('view', room.id)">
            Ver sala <span class="arrow">→</span>
          </button>
        </div>
      </div>

      <span class="corner tl"></span>
      <span class="corner br"></span>
    </div>
  </div>
</template>

<style scoped>
/* ── Stack ── */
.card-stack {
  position: relative;
  width: 100%;
  padding-bottom: 10px;
}

.stack-layer {
  position: absolute;
  left: 0;
  right: 0;
  border-radius: 16px;
  transition: all 0.32s cubic-bezier(0.4, 0, 0.2, 1);
}

.layer-1 {
  bottom: 0;
  top: 6px;
  transform: scaleX(0.96);
  opacity: 0.5;
  background: rgba(var(--accent-rgb), 0.04);
  border: 1px solid rgba(var(--accent-rgb), 0.15);
}

.layer-2 {
  bottom: -4px;
  top: 10px;
  transform: scaleX(0.92);
  opacity: 0.3;
  background: rgba(var(--accent-rgb), 0.025);
  border: 1px solid rgba(var(--accent-rgb), 0.09);
}

.layer-3 {
  bottom: -8px;
  top: 14px;
  transform: scaleX(0.88);
  opacity: 0.18;
  background: rgba(var(--accent-rgb), 0.015);
  border: 1px solid rgba(var(--accent-rgb), 0.06);
}

.card-stack:hover .layer-1 {
  transform: scaleX(0.96) translateY(5px);
  opacity: 0.6;
}

.card-stack:hover .layer-2 {
  transform: scaleX(0.91) translateY(10px);
  opacity: 0.35;
}

.card-stack:hover .layer-3 {
  transform: scaleX(0.86) translateY(15px);
  opacity: 0.22;
}

/* ── Carta principal ── */
.room-card {
  position: relative;
  z-index: 4;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  width: 100%;
  padding: 1.5rem 1.875rem;
  border-radius: 20px;
  border: 1px solid rgba(var(--accent-rgb), 0.25);
  background: linear-gradient(135deg,
      rgba(15, 23, 42, 0.95) 0%,
      rgba(var(--accent-rgb), 0.06) 50%,
      rgba(15, 23, 42, 0.92) 100%);
  backdrop-filter: blur(20px);
  overflow: hidden;
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow:
    0 0 0 1px rgba(255, 255, 255, 0.05),
    0 8px 32px rgba(0, 0, 0, 0.6),
    inset 0 1px 0 rgba(255, 255, 255, 0.08),
    0 0 40px rgba(var(--accent-rgb), 0.04);
}

.room-card:hover {
  border-color: rgba(var(--accent-rgb), 0.6);
  transform: translateY(-6px) scale(1.01);
  box-shadow:
    0 0 0 1px rgba(var(--accent-rgb), 0.3),
    0 20px 60px rgba(0, 0, 0, 0.7),
    0 0 120px rgba(var(--accent-rgb), 0.12),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

/* Scanline */
.card-scanline {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1.5px;
  background: linear-gradient(90deg, transparent, rgba(var(--accent-rgb), 0.7) 20%, #fff 50%, rgba(var(--accent-rgb), 0.7) 80%, transparent);
  opacity: 0;
  transition: opacity 0.25s;
}

.room-card:hover .card-scanline {
  opacity: 1;
  animation: scanline 2s ease-in-out infinite;
}

@keyframes scanline {
  0% {
    clip-path: inset(0 100% 0 0);
  }

  50% {
    clip-path: inset(0 0% 0 0);
  }

  100% {
    clip-path: inset(0 0 0 100%);
  }
}

/* Barra lateral */
.card-bar {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, transparent, var(--accent) 25%, rgba(var(--accent-rgb), 0.8) 75%, transparent);
  box-shadow: 0 0 14px rgba(var(--accent-rgb), 0.8);
}

.bar-pulse {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 10px var(--accent), 0 0 20px rgba(var(--accent-rgb), 0.4);
  animation: blink 2.2s ease-in-out infinite;
}

@keyframes blink {

  0%,
  100% {
    opacity: 1;
    transform: translate(-50%, -50%) scale(1);
  }

  50% {
    opacity: 0.2;
    transform: translate(-50%, -50%) scale(0.5);
  }
}

/* Avatar */
.card-avatar {
  position: relative;
  flex-shrink: 0;
  width: 68px;
  height: 68px;
}

.avatar-box {
  width: 72px;
  height: 72px;
  border-radius: 18px;
  border: 2px solid rgba(var(--accent-rgb), 0.4);
  background: radial-gradient(circle at 30% 30%, rgba(var(--accent-rgb), 0.25), rgba(15, 23, 42, 0.8));
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.2rem;
  position: relative;
  z-index: 1;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow:
    inset 0 0 30px rgba(var(--accent-rgb), 0.15),
    0 8px 24px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(var(--accent-rgb), 0.2);
}

.room-card:hover .avatar-box {
  transform: scale(1.15) rotate(10deg);
  border-color: rgba(var(--accent-rgb), 0.8);
  box-shadow:
    inset 0 0 40px rgba(var(--accent-rgb), 0.25),
    0 0 40px rgba(var(--accent-rgb), 0.4),
    0 12px 32px rgba(0, 0, 0, 0.6);
}

.avatar-ring {
  position: absolute;
  inset: -6px;
  border-radius: 20px;
  border: 1px solid rgba(var(--accent-rgb), 0.18);
  animation: rpulse 3s ease-in-out infinite;
}

@keyframes rpulse {

  0%,
  100% {
    opacity: 0.5;
    transform: scale(1);
  }

  50% {
    opacity: 0.1;
    transform: scale(1.14);
  }
}

/* Main */
.card-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
}

.top-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.name-block {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  min-width: 0;
}

.id-tag {
  flex-shrink: 0;
  font-size: 0.625rem;
  font-family: 'Courier New', monospace;
  font-weight: 800;
  letter-spacing: 1px;
  color: var(--accent);
  background: rgba(var(--accent-rgb), 0.15);
  border: 1px solid rgba(var(--accent-rgb), 0.35);
  border-radius: 6px;
  padding: 0.2rem 0.5rem;
  box-shadow: 0 0 10px rgba(var(--accent-rgb), 0.2);
}

.room-name {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 900;
  color: #ffffff;
  letter-spacing: -0.5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
}

.diff-badge {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.625rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  color: var(--accent);
  background: rgba(var(--accent-rgb), 0.15);
  border: 1px solid rgba(var(--accent-rgb), 0.4);
  border-radius: 8px;
  padding: 0.3rem 0.7rem;
  box-shadow: 0 0 15px rgba(var(--accent-rgb), 0.2);
}

.diff-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 6px var(--accent);
  animation: dblink 1.8s ease-in-out infinite;
}

@keyframes dblink {

  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.15;
  }
}

.room-desc {
  margin: 0;
  font-size: 0.8125rem;
  color: #cbd5e1;
  line-height: 1.6;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  text-shadow: 0 1px 5px rgba(0, 0, 0, 0.3);
}

/* Chips */
.chips-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.chip {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.35rem 0.65rem;
  border-radius: 8px;
  background: rgba(15, 23, 42, 0.6);
  border: 1px solid rgba(var(--accent-rgb), 0.2);
  transition: all 0.25s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.chip:hover {
  background: rgba(var(--accent-rgb), 0.15);
  border-color: rgba(var(--accent-rgb), 0.5);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(var(--accent-rgb), 0.25);
}

.chip-lbl {
  font-size: 0.625rem;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  color: #94a3b8;
  font-weight: 700;
}

.chip-val {
  font-size: 0.8125rem;
  font-weight: 900;
  color: var(--accent);
  font-family: 'Courier New', monospace;
  text-shadow: 0 0 8px rgba(var(--accent-rgb), 0.4);
}

.chip-val--muted {
  color: #cbd5e1;
  font-family: inherit;
  font-weight: 700;
  font-size: 0.75rem;
  max-width: 100px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Panel derecho */
.card-right {
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding-left: 1.5rem;
  border-left: 1px solid rgba(255, 255, 255, 0.07);
}

.members-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.2rem;
}

.members-ring-wrap {
  position: relative;
  width: 52px;
  height: 52px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.members-svg {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
}

.members-num {
  font-size: 1.0625rem;
  font-weight: 900;
  color: var(--accent);
  font-family: 'Courier New', monospace;
  position: relative;
  z-index: 1;
  text-shadow: 0 0 12px rgba(var(--accent-rgb), 0.5);
}

/* ← label miembros visible */
.members-lbl {
  font-size: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #64748b;
  font-weight: 700;
}

.btns {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  width: 100%;
  min-width: 100px;
}

.btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.55rem 1.1rem;
  border-radius: 10px;
  border: none;
  font-size: 0.75rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  cursor: pointer;
  transition: all 0.25s ease;
  white-space: nowrap;
}

.btn-edit {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.15);
  color: #94a3b8;
}

.btn-edit:hover {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.3);
  color: #ffffff;
  box-shadow: 0 4px 16px rgba(255, 255, 255, 0.1);
}

.btn-join {
  background: linear-gradient(135deg, var(--accent) 0%, rgba(var(--accent2-rgb), 0.85) 100%);
  color: #0f172a;
  font-weight: 900;
  box-shadow: 0 4px 16px rgba(var(--accent-rgb), 0.4), 0 0 20px rgba(var(--accent-rgb), 0.15);
}

.btn-join:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: 0 8px 28px rgba(var(--accent-rgb), 0.6), 0 0 30px rgba(var(--accent-rgb), 0.25);
  filter: brightness(1.1);
}

.arrow {
  display: inline-block;
  transition: transform 0.2s;
}

.btn-join:hover .arrow {
  transform: translateX(4px);
}

/* Esquinas decorativas */
.corner {
  position: absolute;
  width: 20px;
  height: 20px;
  opacity: 0;
  transition: opacity 0.3s;
  pointer-events: none;
}

.corner::before,
.corner::after {
  content: '';
  position: absolute;
  background: var(--accent);
}

.corner::before {
  width: 100%;
  height: 1px;
  top: 0;
  left: 0;
}

.corner::after {
  width: 1px;
  height: 100%;
  top: 0;
  left: 0;
}

.corner.tl {
  top: 10px;
  left: 14px;
}

.corner.br {
  bottom: 10px;
  right: 14px;
  transform: rotate(180deg);
}

.room-card:hover .corner {
  opacity: 0.5;
}

/* Responsive */
@media (max-width: 640px) {
  .room-card {
    flex-wrap: wrap;
    padding: 1rem 1.25rem;
    gap: 1rem;
  }

  .card-right {
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    padding-left: 0;
    padding-top: 0.875rem;
    border-left: none;
    border-top: 1px solid rgba(255, 255, 255, 0.07);
  }

  .btns {
    flex-direction: row;
    min-width: auto;
  }

  .diff-badge {
    display: none;
  }
}
</style>