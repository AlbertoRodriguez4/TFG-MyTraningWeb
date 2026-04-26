<script setup lang="ts">
import { useRoomStore } from '@/stores/RoomStore'
import { useUserRoomStore } from '@/stores/UsersRoomStore'
import { useUserStore } from '@/stores/userStore'
import { logger } from '@/utils/logger'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import RoomRender from '../../components/Renders/RoomRender.vue'

const store = useRoomStore()
const userRoomStore = useUserRoomStore()
const userStore = useUserStore()

const loggedUser = ref(userStore.loggedUser)
const totalMembers = ref(0)
const joinedRoomsCount = ref(0)

const statsLoaded = ref(false)

const totalRoomsCount = computed(() => store.room.length)

const averageLevel = computed(() => {
  if (store.room.length === 0) return 0
  const sum = store.room.reduce((acc, room) => acc + room.minlevel, 0)
  return Math.round(sum / store.room.length)
})

const myJoinedRooms = computed(() => {
  if (!loggedUser.value?.id) return 0
  let count = 0
  store.room.forEach(room => {
    const members = userRoomStore.currentRoomMembers
    if (members.some(m => m.userid === loggedUser.value?.id && m.roomid === room.id)) {
      count++
    }
  })
  return count
})

const handleMembershipChange = async () => {
  await calculateStats()
}

onMounted(async () => {
  await store.fetchRoom()
  await calculateStats()
  window.addEventListener('room-membership-changed', handleMembershipChange)
})

onUnmounted(() => {
  window.removeEventListener('room-membership-changed', handleMembershipChange)
})

async function calculateStats() {
  if (!loggedUser.value?.id) {
    statsLoaded.value = true
    return
  }
  try {
    let membersCount = 0
    let myRooms = 0
    for (const room of store.room) {
      await userRoomStore.fetchMembersByRoomId(room.id)
      const currentMembers = userRoomStore.currentRoomMembers.length
      membersCount += currentMembers
      if (currentMembers > 0 && userRoomStore.currentRoomMembers.some(m => m.userid === loggedUser.value?.id)) {
        myRooms++
      }
    }
    totalMembers.value = membersCount
    joinedRoomsCount.value = myRooms
  } catch (error) {
    logger.error('Error calculating stats:', error)
  } finally {
    statsLoaded.value = true
  }
}
</script>

<template>
  <v-app>
    <v-main class="main">
      <div class="background-overlay"></div>
      <div class="gradient-orb orb-1"></div>
      <div class="gradient-orb orb-2"></div>

      <v-container fluid class="content-container">
        <div class="welcome-banner">
          <div class="banner-glow"></div>
          <div class="banner-content">
            <div class="welcome-icon">🏛️</div>
            <div class="welcome-text">
              <h1 class="welcome-title">{{ $t('rooms') || 'Salas de Entrenamiento' }}</h1>
              <p class="welcome-subtitle">Entrena en grupo, supera retos, alcanza la grandeza</p>
            </div>
          </div>
          <div class="banner-decoration">
            <div class="decoration-line"></div>
            <div class="decoration-dot"></div>
            <div class="decoration-line"></div>
          </div>
        </div>

        <section class="stats-section">
          <div class="section-header">
            <div class="section-icon">📊</div>
            <h2 class="section-title">{{ $t('estadisticas_salas') || 'Estadísticas' }}</h2>
            <div class="section-line"></div>
          </div>
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon total-rooms">🏟️</div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ totalRoomsCount || '...' }}</span>
                  <span class="stat-label">Salas totales</span>
                </div>
              </div>
            </div>
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon my-rooms">✅</div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ joinedRoomsCount || '...' }}</span>
                  <span class="stat-label">Mis salas</span>
                </div>
              </div>
            </div>
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon total-members">👥</div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ totalMembers || '...' }}</span>
                  <span class="stat-label">Entrenadores</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="content-section" id="inventarios">
          <RoomRender />
        </section>

        <div class="fab-container">
          <button class="fab" @click="$emit('create-room')">
            <span class="fab-icon">+</span>
          </button>
        </div>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
* { margin: 0; padding: 0; box-sizing: border-box; }

.main {
  position: relative;
  min-height: 100vh;
  width: 100%;
  background: linear-gradient(135deg, #0a0e27 0%, #1a1a2e 50%, #16213e 100%);
  overflow-x: hidden;
  overflow-y: auto;
}

.background-overlay {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
  background-image: url('@/assets/imgs/gimansio-fondo.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  opacity: 0.12;
  z-index: 0;
  pointer-events: none;
}

.gradient-orb {
  position: fixed;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.1;
  z-index: 1;
  pointer-events: none;
  animation: float-orb 20s ease-in-out infinite;
}

.orb-1 {
  width: 450px; height: 450px;
  background: radial-gradient(circle, #fbbf24, transparent);
  top: -200px; right: -200px;
  animation-delay: 0s;
}

.orb-2 {
  width: 350px; height: 350px;
  background: radial-gradient(circle, #f59e0b, transparent);
  bottom: -150px; left: -150px;
  animation-delay: 5s;
}

@keyframes float-orb {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(-20px, 20px) scale(0.95); }
}

.content-container {
  position: relative;
  z-index: 2;
  width: 100% !important;
  max-width: 100% !important;
  padding: 1.5rem 1rem;
}

.welcome-banner {
  position: relative;
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.07) 0%, rgba(245, 158, 11, 0.04) 100%);
  border: 1.5px solid rgba(251, 191, 36, 0.3);
  border-radius: 20px;
  padding: 2rem;
  margin-bottom: 2rem;
  overflow: hidden;
  backdrop-filter: blur(10px);
  box-shadow:
    0 6px 24px rgba(251, 191, 36, 0.12),
    inset 0 1px 0 rgba(255, 255, 255, 0.08);
}

.banner-glow {
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 20px;
  opacity: 0;
  filter: blur(16px);
  transition: opacity 0.3s;
  z-index: -1;
}

.welcome-banner:hover .banner-glow { opacity: 0.2; }

.banner-content {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  margin-bottom: 0.75rem;
}

.welcome-icon {
  font-size: 3rem;
  filter: drop-shadow(0 3px 10px rgba(251, 191, 36, 0.4));
}

.welcome-text { flex: 1; }

.welcome-title {
  font-size: clamp(1.5rem, 3.5vw, 2rem);
  font-weight: 800;
  color: #ffffff;
  margin: 0;
  line-height: 1.2;
  text-shadow: 0 2px 16px rgba(251, 191, 36, 0.35);
}

.room-title-highlight {
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 900;
}

.welcome-subtitle {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.65);
  margin: 0.4rem 0 0 0;
  font-weight: 500;
}

.banner-decoration {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  opacity: 0.45;
}

.decoration-line {
  flex: 1;
  height: 1.5px;
  background: linear-gradient(90deg, transparent, #fbbf24, transparent);
}

.decoration-dot {
  width: 6px; height: 6px;
  background: #fbbf24;
  border-radius: 50%;
  box-shadow: 0 0 8px #fbbf24;
}

.stats-section { margin-bottom: 2rem; }

.section-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.25rem;
}

.section-icon { font-size: 1.5rem; }

.section-title {
  font-size: clamp(1.25rem, 2.5vw, 1.6rem);
  font-weight: 800;
  color: #ffffff;
  margin: 0;
}

.section-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, #fbbf24, transparent);
  border-radius: 2px;
  margin-left: 0.75rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 1rem;
}

.stat-card {
  position: relative;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 1.25rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-shadow: 0 3px 16px rgba(0, 0, 0, 0.18);
}

.stat-card:hover {
  border-color: rgba(251, 191, 36, 0.4);
  box-shadow: 0 6px 24px rgba(251, 191, 36, 0.18);
  transform: translateY(-2px);
}

.stat-card-header {
  display: flex;
  align-items: center;
  gap: 0.875rem;
}

.stat-card-icon {
  width: 42px; height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  background: rgba(251, 191, 36, 0.1);
  border: 1px solid rgba(251, 191, 36, 0.25);
}

.stat-card-icon.my-rooms {
  background: rgba(56, 189, 248, 0.1);
  border-color: rgba(56, 189, 248, 0.25);
}

.stat-card-icon.total-members {
  background: rgba(52, 211, 153, 0.1);
  border-color: rgba(52, 211, 153, 0.25);
}

.stat-card-body {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
}

.stat-value {
  font-size: 1.6rem;
  font-weight: 900;
  color: #ffffff;
}

.stat-label {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.55);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.content-section {
  position: relative;
  z-index: 2;
  margin-top: 0;
}

.fab-container {
  position: fixed;
  bottom: 1.5rem;
  right: 1.5rem;
  z-index: 1000;
}

.fab {
  width: 50px; height: 50px;
  border-radius: 50%;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border: none;
  box-shadow: 0 3px 10px rgba(251, 191, 36, 0.45);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  font-size: 1.5rem;
  color: white;
  font-weight: 300;
}

.fab:hover {
  transform: translateY(-3px) scale(1.08);
  box-shadow: 0 6px 20px rgba(251, 191, 36, 0.6);
}

@media (max-width: 768px) {
  .content-container { padding: 1rem 0.5rem; }
  .welcome-banner { padding: 1.5rem; margin-bottom: 1.5rem; }
  .welcome-icon { font-size: 2.5rem; }
  .banner-content { flex-direction: column; text-align: center; gap: 1rem; }
  .section-header { flex-wrap: wrap; }
  .section-line { width: 100%; margin-left: 0; margin-top: 0.4rem; }
  .stats-grid { grid-template-columns: 1fr; }
  .fab-container { bottom: 1rem; right: 1rem; }
  .fab { width: 44px; height: 44px; }
}

@media (max-width: 480px) {
  .welcome-title { font-size: 1.35rem; }
  .welcome-subtitle { font-size: 0.875rem; }
  .section-title { font-size: 1.1rem; }
  .stat-value { font-size: 1.35rem; }
}

@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
</style>
