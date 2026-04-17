<script setup lang="ts">
import { useRoomStore } from '@/stores/RoomStore';
import { useUserRoomStore } from '@/stores/UsersRoomStore';
import { useUserStore } from '@/stores/userStore';
import { computed, onMounted, onUnmounted, ref } from 'vue';
import RoomRender from '../../components/Renders/RoomRender.vue';

const store = useRoomStore()
const userRoomStore = useUserRoomStore()
const userStore = useUserStore()

const loggedUser = ref(userStore.loggedUser)
const totalMembers = ref(0)
const joinedRoomsCount = ref(0)

// Estadísticas reales calculadas
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

  // Escuchar evento de cambio de membresía para actualizar stats
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
    // Contar miembros totales en todas las salas
    let membersCount = 0
    let myRooms = 0

    for (const room of store.room) {
      await userRoomStore.fetchMembersByRoomId(room.id)
      const currentMembers = userRoomStore.currentRoomMembers.length
      membersCount += currentMembers

      // Contar salas a las que estoy unido
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
      <!-- Background Effects -->
      <div class="background-overlay"></div>
      <div class="gradient-orb orb-1"></div>
      <div class="gradient-orb orb-2"></div>

      <v-container fluid class="content-container">
        <!-- Welcome Banner -->
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

        <!-- Stats Section -->
        <section class="stats-section">
          <div class="section-header">
            <div class="section-icon">📊</div>
            <h2 class="section-title">{{ $t('estadisticas_salas') || 'Estadísticas de Salas' }}</h2>
            <div class="section-line"></div>
          </div>
          <div class="stats-grid">
            <!-- Total Salas Card -->
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon total-rooms">
                  🏟️
                </div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ totalRoomsCount || '...' }}</span>
                  <span class="stat-label">Salas totales</span>
                </div>
              </div>
            </div>

            <!-- Mis Salas Card -->
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon my-rooms">
                  ✅
                </div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ joinedRoomsCount || '...' }}</span>
                  <span class="stat-label">Mis salas</span>
                </div>
              </div>
            </div>

            <!-- Total Miembros Card -->
            <div class="stat-card">
              <div class="stat-card-header">
                <div class="stat-card-icon total-members">
                  👥
                </div>
                <div class="stat-card-body">
                  <span class="stat-value">{{ totalMembers || '...' }}</span>
                  <span class="stat-label">Entrenadores</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- Content Section -->
        <section class="content-section" id="inventarios">
          <RoomRender />
        </section>

        <!-- Floating Action Button -->
        <div class="fab-container">
          <button class="fab" @click="$emit('create-room')">
            <span class="fab-icon">↑</span>
          </button>
        </div>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Main Wrapper - Consistente con HomeLoggedView */
.main {
  position: relative;
  min-height: 100vh;
  width: 100%;
  background: linear-gradient(135deg, #0a0e27 0%, #1a1a2e 50%, #16213e 100%);
  overflow-x: hidden;
  overflow-y: auto;
}

/* Background Effects - Consistente con el resto de la app */
.background-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url('../assets/imgs/gimansio-fondo.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  opacity: 0.15;
  z-index: 0;
  pointer-events: none;
}

/* Gradient Orbs - Efecto visual consistente */
.gradient-orb {
  position: fixed;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.12;
  z-index: 1;
  pointer-events: none;
  animation: float-orb 20s ease-in-out infinite;
}

.orb-1 {
  width: 500px;
  height: 500px;
  background: radial-gradient(circle, #fbbf24, transparent);
  top: -250px;
  right: -250px;
  animation-delay: 0s;
}

.orb-2 {
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, #f59e0b, transparent);
  bottom: -200px;
  left: -200px;
  animation-delay: 5s;
}

@keyframes float-orb {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  25% {
    transform: translate(50px, -50px) scale(1.1);
  }
  50% {
    transform: translate(-30px, 30px) scale(0.9);
  }
  75% {
    transform: translate(40px, 40px) scale(1.05);
  }
}

/* Content Container */
.content-container {
  position: relative;
  z-index: 2;
  width: 100% !important;
  max-width: 100% !important;
  padding: 2rem 1rem;
}

/* Welcome Banner */
.welcome-banner {
  position: relative;
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.08) 0%, rgba(245, 158, 11, 0.05) 100%);
  border: 2px solid rgba(251, 191, 36, 0.35);
  border-radius: 24px;
  padding: 2.5rem;
  margin-bottom: 3rem;
  overflow: hidden;
  backdrop-filter: blur(10px);
  box-shadow:
    0 8px 32px rgba(251, 191, 36, 0.15),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.banner-glow {
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 24px;
  opacity: 0;
  filter: blur(20px);
  transition: opacity 0.3s;
  z-index: -1;
}

.welcome-banner:hover .banner-glow {
  opacity: 0.25;
}

.banner-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.welcome-icon {
  font-size: 4rem;
  filter: drop-shadow(0 4px 12px rgba(251, 191, 36, 0.5));
  animation: pulse-icon 2s ease-in-out infinite;
}

@keyframes pulse-icon {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

.welcome-text {
  flex: 1;
}

.welcome-title {
  font-size: clamp(1.75rem, 4vw, 2.5rem);
  font-weight: 800;
  color: #ffffff;
  margin: 0;
  line-height: 1.2;
  text-shadow: 0 2px 20px rgba(251, 191, 36, 0.4);
}

.room-title-highlight {
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 900;
}

.welcome-subtitle {
  font-size: 1.125rem;
  color: rgba(255, 255, 255, 0.7);
  margin: 0.5rem 0 0 0;
  font-weight: 500;
}

.banner-decoration {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  opacity: 0.5;
}

.decoration-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, transparent, #fbbf24, transparent);
}

.decoration-dot {
  width: 8px;
  height: 8px;
  background: #fbbf24;
  border-radius: 50%;
  box-shadow: 0 0 10px #fbbf24;
}

/* Stats Section - Panel consistente */
.stats-section {
  margin-bottom: 3rem;
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.section-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 8px rgba(251, 191, 36, 0.5));
}

.section-title {
  font-size: clamp(1.5rem, 3vw, 2rem);
  font-weight: 800;
  color: #ffffff;
  margin: 0;
  text-shadow: 0 2px 10px rgba(251, 191, 36, 0.3);
}

.section-line {
  flex: 1;
  height: 3px;
  background: linear-gradient(90deg, #fbbf24, transparent);
  border-radius: 2px;
  margin-left: 1rem;
}

/* Stats Cards Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  position: relative;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 1.5rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.2);
}

.stat-card:hover {
  border-color: rgba(251, 191, 36, 0.5);
  box-shadow: 0 8px 32px rgba(251, 191, 36, 0.25);
  transform: translateY(-2px);
}

.stat-card::before {
  content: '';
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 20px;
  opacity: 0;
  filter: blur(10px);
  transition: opacity 0.3s;
  z-index: -1;
}

.stat-card:hover::before {
  opacity: 0.2;
}

.stat-card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.stat-card-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  background: rgba(251, 191, 36, 0.1);
  border: 1px solid rgba(251, 191, 36, 0.3);
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.15), inset 0 0 12px rgba(251, 191, 36, 0.08);
}

.stat-card-icon.total-rooms {
  background: rgba(251, 191, 36, 0.12);
  border-color: rgba(251, 191, 36, 0.4);
}

.stat-card-icon.my-rooms {
  background: rgba(56, 189, 248, 0.12);
  border-color: rgba(56, 189, 248, 0.4);
}

.stat-card-icon.total-members {
  background: rgba(52, 211, 153, 0.12);
  border-color: rgba(52, 211, 153, 0.4);
}

.stat-card-body {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.stat-value {
  font-size: 2rem;
  font-weight: 900;
  color: #ffffff;
  text-shadow: 0 2px 10px rgba(251, 191, 36, 0.3);
}

.stat-label {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Content Section */
.content-section {
  position: relative;
  z-index: 2;
  margin-top: 0;
  animation: fadeIn 0.8s ease-out 0.2s both;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Floating Action Button - Estilo consistente con HomeLoggedView */
.fab-container {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  z-index: 1000;
}

.fab {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border: none;
  box-shadow:
    0 4px 12px rgba(251, 191, 36, 0.5),
    0 0 0 0 rgba(251, 191, 36, 0.5);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  animation: fab-pulse 2s ease-in-out infinite;
}

.fab:hover {
  transform: translateY(-4px) scale(1.1);
  box-shadow:
    0 8px 24px rgba(251, 191, 36, 0.7),
    0 0 0 8px rgba(251, 191, 36, 0.15);
}

.fab:active {
  transform: translateY(-2px) scale(1.05);
}

.fab-icon {
  font-size: 1.5rem;
  color: white;
  font-weight: bold;
}

@keyframes fab-pulse {
  0%, 100% {
    box-shadow:
      0 4px 12px rgba(251, 191, 36, 0.5),
      0 0 0 0 rgba(251, 191, 36, 0.5);
  }
  50% {
    box-shadow:
      0 4px 12px rgba(251, 191, 36, 0.5),
      0 0 0 10px rgba(251, 191, 36, 0);
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .content-container {
    padding: 1rem 0.5rem;
  }

  .welcome-banner {
    padding: 1.5rem;
    margin-bottom: 2rem;
  }

  .banner-content {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .welcome-icon {
    font-size: 3rem;
  }

  .section-header {
    flex-wrap: wrap;
  }

  .section-line {
    width: 100%;
    margin-left: 0;
    margin-top: 0.5rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }

  .fab-container {
    bottom: 1rem;
    right: 1rem;
  }

  .fab {
    width: 48px;
    height: 48px;
  }

  .gradient-orb {
    filter: blur(80px);
  }
}

@media (max-width: 480px) {
  .welcome-title {
    font-size: 1.5rem;
  }

  .welcome-subtitle {
    font-size: 0.875rem;
  }

  .section-title {
    font-size: 1.25rem;
  }

  .section-icon {
    font-size: 1.5rem;
  }

  .stat-value {
    font-size: 1.5rem;
  }
}

/* Accessibility */
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
</style>