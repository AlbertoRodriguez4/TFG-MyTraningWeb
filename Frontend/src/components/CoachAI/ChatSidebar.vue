<template>
  <aside class="sidebar" :class="{ 'sidebar--open': open }">
    <div class="sidebar-inner">

      <!-- Brand -->
      <div class="brand">
        <div class="brand-mark">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
            <path d="M12 2L14 9H21L15.5 13.5L17.5 21L12 16.5L6.5 21L8.5 13.5L3 9H10L12 2Z" fill="currentColor" />
          </svg>
        </div>
        <span class="brand-name">Coach<strong>AI</strong></span>
      </div>

      <!-- Nav -->
      <nav class="nav">
        <a class="nav-link nav-link--active" href="#">
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
          </svg>
          {{ t('chat.navChat') }}
        </a>  
      </nav>

      <!-- Stats -->
      <div class="stats-block">
        <p class="stats-title">{{ t('common.today') }}</p>
        <div class="stat-row" v-for="s in stats" :key="s.label">
          <span class="stat-emoji">{{ s.emoji }}</span>
          <div class="stat-info">
            <div class="stat-meta">
              <span class="stat-label">{{ s.label }}</span>
              <span class="stat-val">{{ s.value }}</span>
            </div>
            <div class="stat-track">
              <div class="stat-fill" :class="s.color" :style="{ width: s.pct + '%' }" />
            </div>
          </div>
        </div>
      </div>

      <!-- User -->
      <div class="user-row">
        <div class="user-ava">A</div>
        <div class="user-data">
          <span class="user-name">{{ loggedUser?.name || t('common.user') }}</span>
          <span class="user-plan">{{ t('chat.premiumPlan') }}</span>
        </div>
        <div class="upgrade-pip" :title="t('chat.upgradePlan')" />
      </div>

    </div>
  </aside>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '../../stores/userStore'
import { useRoutineStore } from '../../stores/RoutineStore'
import { logger } from '@/utils/logger'
import type { Stat } from '@/components/Models/Chat'

const { t } = useI18n()

defineProps({
  open: { type: Boolean, default: false }
})

const userStore = useUserStore()
const routineStore = useRoutineStore()
const loggedUser = ref(userStore.loggedUser)

const stats = computed<Stat[]>(() => {
  const user = loggedUser.value
  if (!user) return []
  // Calcular rutinas completadas y progreso de XP
  const completedRoutines = routineStore.routines.filter(r => r.iscompleted).length // Obtenemos el número de rutinas completadas
  const xpProgress = user.xpRequired ? Math.round(((user.experience ?? 0) / user.xpRequired) * 100) : 0 // De esta forma, si xpRequired es 0 o undefined, evitamos la división por cero y mostramos 0% de progreso.

    // Retornar las estadísticas formateadas para la sidebar
  return [
    {
      emoji: '⚡',
      label: t('common.level'),
      value: `${user.level}`,
      pct: xpProgress,
      color: 'fill-violet'
    },
    {
      emoji: '🔥',
      label: t('common.streak'),
      value: `${user.consistencyStreak || 0} ${t('common.days')}`,
      pct: Math.min((user.consistencyStreak || 0) * 10, 100),
      color: 'fill-orange'
    },
    {
      emoji: '✅',
      label: t('common.routines'),
      value: `${completedRoutines}`,
      pct: routineStore.routines.length ? Math.round((completedRoutines / routineStore.routines.length) * 100) : 0,
      color: 'fill-blue'
    },
    {
      emoji: '🎯',
      label: t('common.nextLevel'),
      value: `${user.xpRemaining || 0} XP`,
      pct: 100 - xpProgress,
      color: 'fill-violet'
    }
  ]
})
// Cargar los datos 
onMounted(async () => {
  try {
    if (loggedUser.value?.id) {
      await routineStore.getRoutineByUserId(loggedUser.value.id)
    }
  } catch (error) {
    logger.error('No se pudieron cargar las rutinas:', error)
  }
})
</script>

<style scoped>
.sidebar {
  width: 230px;
  flex-shrink: 0;
  background: var(--c-sidebar);
  display: none;
  flex-direction: column;
}

@media (min-width: 700px) {
  .sidebar {
    display: flex;
  }
}

@media (max-width: 699px) {
  .sidebar {
    display: flex;
    position: fixed;
    inset: 0;
    width: 260px;
    z-index: 100;
    border-radius: 0;
    transform: translateX(-100%);
    transition: transform .28s cubic-bezier(.4, 0, .2, 1);
  }

  .sidebar--open {
    transform: translateX(0);
  }
}

.sidebar-inner {
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 28px 18px 22px;
  gap: 28px;
  overflow-y: auto;
}

/* Brand */
.brand {
  display: flex;
  align-items: center;
  gap: 11px;
}

.brand-mark {
  width: 34px;
  height: 34px;
  background: var(--c-accent);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  flex-shrink: 0;
  box-shadow: 0 4px 16px rgba(232, 93, 38, .4);
}

.brand-name {
  font-family: 'Syne', sans-serif;
  font-size: 18px;
  font-weight: 700;
  color: rgba(255, 255, 255, .55);
  letter-spacing: .5px;
  text-transform: uppercase;
}

.brand-name strong {
  color: #fff;
  font-weight: 800;
}

/* Nav */
.nav {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: var(--r-sm);
  color: rgba(255, 255, 255, .45);
  text-decoration: none;
  font-size: 13.5px;
  font-weight: 500;
  transition: all .15s ease;
}

.nav-link:hover {
  color: rgba(255, 255, 255, .85);
  background: rgba(255, 255, 255, .06);
}

.nav-link--active {
  color: #fff;
  background: var(--c-accent);
  box-shadow: 0 4px 14px rgba(232, 93, 38, .35);
}

.nav-link--active:hover {
  color: #fff;
  background: var(--c-accent);
}

/* Stats */
.stats-block {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.stats-title {
  font-size: 10px;
  font-weight: 700;
  letter-spacing: 2px;
  color: rgba(255, 255, 255, .25);
  padding: 0 2px;
}

.stat-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.stat-emoji {
  font-size: 16px;
  flex-shrink: 0;
}

.stat-info {
  flex: 1;
  min-width: 0;
}

.stat-meta {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 11.5px;
  color: rgba(255, 255, 255, .4);
  font-weight: 400;
}

.stat-val {
  font-size: 12px;
  color: rgba(255, 255, 255, .75);
  font-weight: 600;
}

.stat-track {
  height: 3px;
  background: rgba(255, 255, 255, .08);
  border-radius: 99px;
  overflow: hidden;
}

.stat-fill {
  height: 100%;
  border-radius: 99px;
  transition: width 1s ease;
}

.fill-orange {
  background: linear-gradient(90deg, #e85d26, #f97316);
}

.fill-blue {
  background: linear-gradient(90deg, #3b82f6, #60a5fa);
}

.fill-violet {
  background: linear-gradient(90deg, #7c3aed, #a78bfa);
}

/* User */
.user-row {
  margin-top: auto;
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  background: rgba(255, 255, 255, .05);
  border: 1px solid rgba(255, 255, 255, .08);
  border-radius: var(--r-sm);
}

.user-ava {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--c-accent), #f97316);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 700;
  color: #fff;
  flex-shrink: 0;
}

.user-name {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: rgba(255, 255, 255, .85);
}

.user-plan {
  font-size: 11px;
  color: rgba(255, 255, 255, .35);
}

.upgrade-pip {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: var(--c-accent);
  box-shadow: 0 0 8px rgba(232, 93, 38, .8);
  margin-left: auto;
  flex-shrink: 0;
}
</style>