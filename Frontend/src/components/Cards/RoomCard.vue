<script setup lang="ts">
const props = defineProps<{
  room: {
    id: number
    name: string
    minlevel: number
    minstats: number
    minconsistency: number
    description: string
    date: string
    localization: string
    creatorRole?: string
  }
  memberCount: number
  isStaff: boolean
  isAdmin?: boolean
  isJoined?: boolean
  creatorRole?: string
}>()

const emit = defineEmits<{
  (e: 'view', id: number): void
  (e: 'edit', room: any): void
}>()

// Configuración de dificultades para las salas de entrenamiento, dependiendo del nivel mínimo requerido
const difficulties: Record<string, { labelKey: string; color: string; rgb: string; gradient: string; glow: string }> = {
  common: {
    labelKey: 'common',
    color: '#94a3b8',
    rgb: '148,163,184',
    gradient: 'linear-gradient(135deg, rgba(148,163,184,0.2), rgba(71,85,105,0.1))',
    glow: 'rgba(148,163,184,0.3)'
  },
  rare: {
    labelKey: 'rare',
    color: '#38bdf8',
    rgb: '56,189,248',
    gradient: 'linear-gradient(135deg, rgba(56,189,248,0.2), rgba(14,165,233,0.1))',
    glow: 'rgba(56,189,248,0.4)'
  },
  epic: {
    labelKey: 'epic',
    color: '#f472b6',
    rgb: '244,114,182',
    gradient: 'linear-gradient(135deg, rgba(244,114,182,0.2), rgba(236,72,153,0.1))',
    glow: 'rgba(244,114,182,0.4)'
  },
  legendary: {
    labelKey: 'legendary',
    color: '#fbbf24',
    rgb: '251,191,36',
    gradient: 'linear-gradient(135deg, rgba(251,191,36,0.25), rgba(245,158,11,0.1))',
    glow: 'rgba(251,191,36,0.5)'
  },
}
// Funciones para determinar la dificultad y el icono de la sala según el nivel mínimo requerido
const getRoomDifficulty = (level: number) => {
  if (level >= 50) return 'legendary'
  if (level >= 30) return 'epic'
  if (level >= 15) return 'rare'
  return 'common'
}
// Función para determinar el icono de la sala según el nivel mínimo requerido
const getRoomIcon = (level: number) => {
  if (level >= 50) return 'mdi-dragon'
  if (level >= 30) return 'mdi-star'
  if (level >= 15) return 'mdi-diamond-stone'
  return 'mdi-target'
}

const diff = getRoomDifficulty(props.room.minlevel)
const diffConfig = difficulties[diff]
const isTrainer = props.creatorRole === 'userStaff'
const canEdit = props.isAdmin || (props.isStaff && props.isJoined)
</script>

<template>
  <div class="room-card-wrapper" :class="{ 'trainer-card': isTrainer }">
    <!-- Efecto de borde brillante -->
    <div class="card-glow-border"></div>

    <div class="room-card" :style="{ '--diff-color': diffConfig.color, '--diff-rgb': diffConfig.rgb }">
      <!-- Header con gradiente -->
      <div class="card-header" :style="{ background: diffConfig.gradient }">
        <div class="header-left">
          <div class="room-icon"><v-icon>{{ getRoomIcon(room.minlevel) }}</v-icon></div>
          <div class="header-info">
            <div class="room-name-row">
              <span class="room-id">#{{ room.id.toString().padStart(3, '0') }}</span>
              <h3 class="room-name">{{ room.name }}</h3>
            </div>
            <div class="badges-row">
              <span class="diff-badge" :style="{ color: diffConfig.color, borderColor: diffConfig.color + '40', background: diffConfig.color + '15' }">
                <span class="diff-dot" :style="{ background: diffConfig.color, boxShadow: `0 0 8px ${diffConfig.color}` }"></span>
                {{ $t(diffConfig.labelKey) }}
              </span>
              <span v-if="isTrainer" class="trainer-badge">
                <span class="trainer-icon"><v-icon>mdi-medal</v-icon></span>
                {{ $t('trainer_room') }}
              </span>
            </div>
          </div>
        </div>
        <div class="member-badge">
          <span class="member-count">{{ memberCount }}</span>
          <span class="member-label">{{ $t('members_label') }}</span>
        </div>
      </div>

      <!-- Descripción -->
      <p v-if="room.description" class="room-description">{{ room.description }}</p>

      <!-- Stats Grid -->
      <div class="stats-grid">
        <div class="stat-item">
          <div class="stat-icon"><v-icon>mdi-chart-bar</v-icon></div>
          <div class="stat-info">
            <span class="stat-label-card">{{ $t('level_label') }}</span>
            <span class="stat-value-card" :style="{ color: diffConfig.color }">{{ room.minlevel }}</span>
          </div>
        </div>
        <div class="stat-item">
          <div class="stat-icon"><v-icon>mdi-arm-flex</v-icon></div>
          <div class="stat-info">
            <span class="stat-label-card">{{ $t('stats_label') }}</span>
            <span class="stat-value-card" :style="{ color: diffConfig.color }">{{ room.minstats }}</span>
          </div>
        </div>
        <div class="stat-item">
          <div class="stat-icon"><v-icon>mdi-fire</v-icon></div>
          <div class="stat-info">
            <span class="stat-label-card">{{ $t('consistency_short') }}</span>
            <span class="stat-value-card" :style="{ color: diffConfig.color }">{{ room.minconsistency }}%</span>
          </div>
        </div>
        <div v-if="room.localization" class="stat-item">
          <div class="stat-icon"><v-icon>mdi-map-marker</v-icon></div>
          <div class="stat-info">
            <span class="stat-label-card">{{ $t('localization') }}</span>
            <span class="stat-value-card muted">{{ room.localization }}</span>
          </div>
        </div>
        <div v-if="room.date" class="stat-item">
          <div class="stat-icon"><v-icon>mdi-calendar</v-icon></div>
          <div class="stat-info">
            <span class="stat-label-card">{{ $t('date') }}</span>
            <span class="stat-value-card muted">{{ room.date }}</span>
          </div>
        </div>
      </div>

      <!-- Barra de nivel -->
      <div class="level-bar-container">
        <div class="level-bar-label">
          <span>{{ $t('difficulty') }}</span>
          <span :style="{ color: diffConfig.color }">{{ room.minlevel }}/100</span>
        </div>
        <div class="level-bar-track">
          <div
            class="level-bar-fill"
            :style="{
              width: `${Math.min(room.minlevel, 100)}%`,
              background: `linear-gradient(90deg, ${diffConfig.color}, ${diffConfig.glow})`,
              boxShadow: `0 0 12px ${diffConfig.glow}`
            }"
          ></div>
        </div>
      </div>

      <!-- Acciones -->
      <div class="card-actions">
        <button v-if="canEdit" class="action-btn edit-btn" @click="emit('edit', room)">
          <span class="btn-icon"><v-icon>mdi-pencil</v-icon></span>
          <span>{{ $t('edit_label') }}</span>
        </button>
        <button class="action-btn view-btn" @click="emit('view', room.id)" :style="{ background: diffConfig.gradient, borderColor: diffConfig.color + '50', color: diffConfig.color }">
          <span>{{ $t('view_room') }}</span>
          <span class="btn-arrow">→</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.room-card-wrapper {
  position: relative;
  border-radius: 20px;
  transition: transform 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.room-card-wrapper:hover {
  transform: translateY(-8px);
}

/* Efecto de borde brillante */
.card-glow-border {
  position: absolute;
  inset: -1px;
  border-radius: 21px;
  background: linear-gradient(135deg, var(--diff-color, rgba(255,255,255,0.1)), transparent, var(--diff-color, rgba(255,255,255,0.1)));
  opacity: 0;
  transition: opacity 0.4s;
  z-index: 0;
  pointer-events: none;
}

.room-card-wrapper:hover .card-glow-border {
  opacity: 0.6;
}

/* Tarjeta principal */
.room-card {
  position: relative;
  z-index: 1;
  background: linear-gradient(145deg, rgba(15, 23, 42, 0.95), rgba(30, 41, 59, 0.9));
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  overflow: hidden;
  backdrop-filter: blur(20px);
  box-shadow:
    0 4px 24px rgba(0, 0, 0, 0.4),
    0 0 0 1px rgba(255, 255, 255, 0.03);
  transition: box-shadow 0.4s ease;
  display: flex;
  flex-direction: column;
}

.room-card-wrapper:hover .room-card {
  box-shadow:
    0 20px 60px rgba(0, 0, 0, 0.5),
    0 0 40px rgba(var(--diff-rgb), 0.15),
    0 0 0 1px rgba(var(--diff-rgb), 0.2);
}

/* Header */
.card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  padding: 1.25rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
  gap: 1rem;
}

.header-left {
  display: flex;
  align-items: flex-start;
  gap: 0.875rem;
  flex: 1;
  min-width: 0;
}

.room-icon {
  font-size: 2.25rem;
  line-height: 1;
  filter: drop-shadow(0 2px 8px rgba(0, 0, 0, 0.4));
  flex-shrink: 0;
  margin-top: 2px;
}

.header-info {
  flex: 1;
  min-width: 0;
}

.room-name-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.room-id {
  font-size: 0.625rem;
  font-family: 'Courier New', monospace;
  font-weight: 800;
  color: var(--diff-color);
  background: rgba(var(--diff-rgb), 0.15);
  border: 1px solid rgba(var(--diff-rgb), 0.3);
  border-radius: 6px;
  padding: 0.15rem 0.4rem;
  flex-shrink: 0;
}

.room-name {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 800;
  color: #ffffff;
  letter-spacing: -0.3px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.4);
}

.badges-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.4rem;
}

.diff-badge {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  font-size: 0.625rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  border-radius: 6px;
  padding: 0.25rem 0.6rem;
  border: 1px solid;
}

.diff-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  animation: pulse-dot 2s ease-in-out infinite;
}

@keyframes pulse-dot {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.4; transform: scale(0.7); }
}

.trainer-badge {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.625rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.15);
  border: 1px solid rgba(251, 191, 36, 0.4);
  border-radius: 6px;
  padding: 0.25rem 0.6rem;
  animation: trainer-glow 2.5s ease-in-out infinite;
}

.trainer-icon {
  font-size: 0.875rem;
}

@keyframes trainer-glow {
  0%, 100% { box-shadow: 0 0 8px rgba(251, 191, 36, 0.2); }
  50% { box-shadow: 0 0 16px rgba(251, 191, 36, 0.45); }
}

/* Member badge */
.member-badge {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.15rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 0.5rem 0.75rem;
  flex-shrink: 0;
}

.member-count {
  font-size: 1.25rem;
  font-weight: 900;
  color: var(--diff-color);
  font-family: 'Courier New', monospace;
  line-height: 1;
  text-shadow: 0 0 10px rgba(var(--diff-rgb), 0.5);
}

.member-label {
  font-size: 0.5625rem;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  color: #64748b;
  font-weight: 700;
}

/* Descripción */
.room-description {
  margin: 0;
  padding: 0.875rem 1.25rem;
  font-size: 0.8125rem;
  color: #94a3b8;
  line-height: 1.6;
  border-bottom: 1px solid rgba(255, 255, 255, 0.04);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.5rem;
  padding: 0.875rem 1.25rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.04);
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 10px;
  padding: 0.5rem 0.625rem;
  transition: all 0.25s ease;
}

.stat-item:hover {
  background: rgba(var(--diff-rgb), 0.08);
  border-color: rgba(var(--diff-rgb), 0.2);
  transform: translateY(-2px);
}

.stat-icon {
  font-size: 1rem;
  line-height: 1;
}

.stat-info {
  display: flex;
  flex-direction: column;
  gap: 0.05rem;
  min-width: 0;
}

.stat-label-card {
  font-size: 0.5625rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #64748b;
  font-weight: 700;
}

.stat-value-card {
  font-size: 0.875rem;
  font-weight: 800;
  font-family: 'Courier New', monospace;
  line-height: 1.2;
}

.stat-value-card.muted {
  color: #cbd5e1;
  font-family: inherit;
  font-weight: 600;
  font-size: 0.75rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Level Bar */
.level-bar-container {
  padding: 0.875rem 1.25rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.04);
}

.level-bar-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.6875rem;
  font-weight: 700;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  margin-bottom: 0.4rem;
}

.level-bar-track {
  width: 100%;
  height: 6px;
  background: rgba(255, 255, 255, 0.06);
  border-radius: 3px;
  overflow: hidden;
}

.level-bar-fill {
  height: 100%;
  border-radius: 3px;
  transition: width 1s cubic-bezier(0.34, 1.56, 0.64, 1);
}

/* Acciones */
.card-actions {
  display: flex;
  gap: 0.5rem;
  padding: 0.875rem 1.25rem;
}

.action-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 0.625rem 1rem;
  border-radius: 10px;
  border: 1px solid;
  font-size: 0.75rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  cursor: pointer;
  transition: all 0.25s ease;
  white-space: nowrap;
}

.edit-btn {
  background: rgba(255, 255, 255, 0.04);
  border-color: rgba(255, 255, 255, 0.12);
  color: #94a3b8;
}

.edit-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.25);
  color: #ffffff;
  transform: translateY(-2px);
}

.view-btn {
  position: relative;
  overflow: hidden;
}

.view-btn:hover {
  transform: translateY(-2px);
  filter: brightness(1.2);
  box-shadow: 0 4px 16px rgba(var(--diff-rgb), 0.3);
}

.btn-arrow {
  display: inline-block;
  transition: transform 0.2s;
}

.view-btn:hover .btn-arrow {
  transform: translateX(4px);
}

/* Trainer Card - Estilos especiales */
.trainer-card .room-card {
  border-color: rgba(251, 191, 36, 0.35);
}

.trainer-card .card-glow-border {
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.6), transparent, rgba(251, 191, 36, 0.4));
}

.trainer-card:hover .card-glow-border {
  opacity: 0.8;
}

.trainer-card .room-card {
  box-shadow:
    0 4px 24px rgba(0, 0, 0, 0.4),
    0 0 30px rgba(251, 191, 36, 0.08),
    0 0 0 1px rgba(251, 191, 36, 0.15);
}

.trainer-card:hover .room-card {
  box-shadow:
    0 20px 60px rgba(0, 0, 0, 0.5),
    0 0 60px rgba(251, 191, 36, 0.18),
    0 0 0 1px rgba(251, 191, 36, 0.3);
}

/* Responsive */
@media (max-width: 480px) {
  .card-header {
    flex-direction: column;
    gap: 0.75rem;
  }

  .member-badge {
    flex-direction: row;
    align-self: flex-start;
    gap: 0.4rem;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
