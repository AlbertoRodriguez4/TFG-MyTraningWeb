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
    <div class="stack-layer layer-2"></div>
    <div class="stack-layer layer-1"></div>

    <div class="room-card">
      <div class="card-bar">
        <div class="bar-pulse"></div>
      </div>

      <div class="card-avatar">
        <div class="avatar-box">
          <span>{{ getRoomIcon(room.minlevel) }}</span>
        </div>
      </div>

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
            Editar
          </button>
          <button class="btn btn-join" @click="$emit('view', room.id)">
            Ver sala <span class="arrow">→</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.card-stack {
  position: relative;
  width: 100%;
  padding-bottom: 8px;
}

.stack-layer {
  position: absolute;
  left: 0; right: 0;
  border-radius: 14px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.layer-1 {
  bottom: 0; top: 4px;
  transform: scaleX(0.97);
  opacity: 0.45;
  background: rgba(var(--accent-rgb), 0.035);
  border: 1px solid rgba(var(--accent-rgb), 0.12);
}

.layer-2 {
  bottom: -3px; top: 8px;
  transform: scaleX(0.94);
  opacity: 0.25;
  background: rgba(var(--accent-rgb), 0.02);
  border: 1px solid rgba(var(--accent-rgb), 0.07);
}

.card-stack:hover .layer-1 {
  transform: scaleX(0.97) translateY(3px);
  opacity: 0.55;
}

.card-stack:hover .layer-2 {
  transform: scaleX(0.93) translateY(6px);
  opacity: 0.3;
}

.room-card {
  position: relative;
  z-index: 4;
  display: flex;
  align-items: center;
  gap: 1.25rem;
  width: 100%;
  padding: 1.25rem 1.5rem;
  border-radius: 16px;
  border: 1px solid rgba(var(--accent-rgb), 0.2);
  background: linear-gradient(135deg,
      rgba(15, 23, 42, 0.95) 0%,
      rgba(var(--accent-rgb), 0.05) 50%,
      rgba(15, 23, 42, 0.92) 100%);
  backdrop-filter: blur(16px);
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow:
    0 0 0 1px rgba(255, 255, 255, 0.04),
    0 6px 24px rgba(0, 0, 0, 0.5),
    inset 0 1px 0 rgba(255, 255, 255, 0.06),
    0 0 30px rgba(var(--accent-rgb), 0.03);
}

.room-card:hover {
  border-color: rgba(var(--accent-rgb), 0.5);
  transform: translateY(-4px) scale(1.005);
  box-shadow:
    0 0 0 1px rgba(var(--accent-rgb), 0.25),
    0 16px 48px rgba(0, 0, 0, 0.6),
    0 0 80px rgba(var(--accent-rgb), 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.card-bar {
  position: absolute;
  left: 0; top: 0; bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, transparent, var(--accent) 25%, rgba(var(--accent-rgb), 0.8) 75%, transparent);
  box-shadow: 0 0 10px rgba(var(--accent-rgb), 0.6);
}

.bar-pulse {
  position: absolute;
  left: 50%; top: 50%;
  transform: translate(-50%, -50%);
  width: 6px; height: 6px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 8px var(--accent);
  animation: blink 2.2s ease-in-out infinite;
}

@keyframes blink {
  0%, 100% { opacity: 1; transform: translate(-50%, -50%) scale(1); }
  50% { opacity: 0.3; transform: translate(-50%, -50%) scale(0.6); }
}

.card-avatar {
  position: relative;
  flex-shrink: 0;
  width: 56px; height: 56px;
}

.avatar-box {
  width: 56px; height: 56px;
  border-radius: 14px;
  border: 1.5px solid rgba(var(--accent-rgb), 0.35);
  background: radial-gradient(circle at 30% 30%, rgba(var(--accent-rgb), 0.2), rgba(15, 23, 42, 0.8));
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  position: relative;
  z-index: 1;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow:
    inset 0 0 20px rgba(var(--accent-rgb), 0.1),
    0 6px 18px rgba(0, 0, 0, 0.4);
}

.room-card:hover .avatar-box {
  transform: scale(1.1) rotate(6deg);
  border-color: rgba(var(--accent-rgb), 0.7);
}

.card-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
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
  gap: 0.5rem;
  min-width: 0;
}

.id-tag {
  flex-shrink: 0;
  font-size: 0.6rem;
  font-family: 'Courier New', monospace;
  font-weight: 800;
  letter-spacing: 1px;
  color: var(--accent);
  background: rgba(var(--accent-rgb), 0.12);
  border: 1px solid rgba(var(--accent-rgb), 0.3);
  border-radius: 5px;
  padding: 0.15rem 0.4rem;
}

.room-name {
  margin: 0;
  font-size: 1.1rem;
  font-weight: 900;
  color: #ffffff;
  letter-spacing: -0.3px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.diff-badge {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.6rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--accent);
  background: rgba(var(--accent-rgb), 0.12);
  border: 1px solid rgba(var(--accent-rgb), 0.35);
  border-radius: 6px;
  padding: 0.25rem 0.55rem;
}

.diff-dot {
  width: 4px; height: 4px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 5px var(--accent);
}

.room-desc {
  margin: 0;
  font-size: 0.78rem;
  color: #cbd5e1;
  line-height: 1.5;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.chips-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.4rem;
}

.chip {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  background: rgba(15, 23, 42, 0.55);
  border: 1px solid rgba(var(--accent-rgb), 0.15);
  transition: all 0.2s ease;
}

.chip:hover {
  background: rgba(var(--accent-rgb), 0.12);
  border-color: rgba(var(--accent-rgb), 0.4);
  transform: translateY(-1px);
}

.chip-lbl {
  font-size: 0.6rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #94a3b8;
  font-weight: 700;
}

.chip-val {
  font-size: 0.75rem;
  font-weight: 900;
  color: var(--accent);
  font-family: 'Courier New', monospace;
}

.chip-val--muted {
  color: #cbd5e1;
  font-family: inherit;
  font-weight: 700;
  font-size: 0.7rem;
  max-width: 90px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.card-right {
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding-left: 1.25rem;
  border-left: 1px solid rgba(255, 255, 255, 0.06);
}

.members-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.15rem;
}

.members-ring-wrap {
  position: relative;
  width: 46px; height: 46px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.members-svg {
  position: absolute;
  inset: 0;
  width: 100%; height: 100%;
}

.members-num {
  font-size: 0.95rem;
  font-weight: 900;
  color: var(--accent);
  font-family: 'Courier New', monospace;
  position: relative;
  z-index: 1;
}

.members-lbl {
  font-size: 0.6rem;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  color: #64748b;
  font-weight: 700;
}

.btns {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  width: 100%;
  min-width: 90px;
}

.btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 0.45rem 0.9rem;
  border-radius: 8px;
  border: none;
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.btn-edit {
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.12);
  color: #94a3b8;
}

.btn-edit:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.25);
  color: #ffffff;
}

.btn-join {
  background: linear-gradient(135deg, var(--accent) 0%, rgba(var(--accent2-rgb), 0.85) 100%);
  color: #0f172a;
  font-weight: 900;
  box-shadow: 0 3px 12px rgba(var(--accent-rgb), 0.35);
}

.btn-join:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: 0 6px 20px rgba(var(--accent-rgb), 0.5);
  filter: brightness(1.1);
}

.arrow {
  display: inline-block;
  transition: transform 0.2s;
}

.btn-join:hover .arrow {
  transform: translateX(3px);
}

@media (max-width: 640px) {
  .room-card {
    flex-wrap: wrap;
    padding: 1rem 1.25rem;
    gap: 0.875rem;
  }
  .card-right {
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    padding-left: 0;
    padding-top: 0.75rem;
    border-left: none;
    border-top: 1px solid rgba(255, 255, 255, 0.06);
  }
  .btns {
    flex-direction: row;
    min-width: auto;
  }
  .diff-badge { display: none; }
}
</style>
