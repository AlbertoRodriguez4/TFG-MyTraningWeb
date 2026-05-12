<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/userStore'
import UserViewHeader from '@/components/User/UserViewHeader.vue'
import CommunityStats from '@/components/User/CommunityStats.vue'
import PodiumUser from '@/components/User/PodiumUser.vue'
import UserList from '@/components/User/UserList.vue'

const store = useUserStore()
const topThreeUsers = ref<any[]>([])
const allUsers = ref<any[]>([])
const loading = ref(true)

// Stats de la comunidad
const communityStats = ref({
  totalUsers: 0,
  activeToday: 0,
  totalCheckIns: 0
})

onMounted(async () => {
  const [topUsers, stats] = await Promise.all([
    store.getTopThreeUsers(),
    store.getCommunityStats()
  ])
  topThreeUsers.value = topUsers
  communityStats.value = stats
  await store.fetchCommunityUsers()
  allUsers.value = store.user.filter((u: any) =>
    !topThreeUsers.value.some((top: any) => top.name === u.name)
  ).slice(0, 10)
  loading.value = false
})
</script>

<template>
  <v-main class="user-view">
    <!-- Fondo con efecto -->
    <div class="background-gradient"></div>
    <div class="animated-bg">
      <div class="orb orb-1"></div>
      <div class="orb orb-2"></div>
      <div class="orb orb-3"></div>
    </div>
    <div class="particles-bg">
      <div
        class="particle"
        v-for="i in 40"
        :key="i"
        :style="{
          left: Math.random() * 100 + '%',
          animationDelay: Math.random() * 5 + 's',
          animationDuration: (3 + Math.random() * 4) + 's'
        }"
      ></div>
    </div>

    <v-container class="container">
      <!-- Header -->
      <UserViewHeader />

      <!-- Stats -->
      <CommunityStats :stats="communityStats" />

      <!-- Podium Section -->
      <div class="podium-section">
        <div class="section-title">
          <span class="section-icon">🏆</span>
          <h2>{{ $t('home_hall_of_fame') }}</h2>
          <span class="section-icon">🏆</span>
        </div>

        <div class="podium-wrapper">
          <div class="podium" v-if="!loading && topThreeUsers.length > 0">
            <PodiumUser
              v-if="topThreeUsers[1]"
              :user="topThreeUsers[1]"
              :position="2"
            />

            <PodiumUser
              v-if="topThreeUsers[0]"
              :user="topThreeUsers[0]"
              :position="1"
            />

            <PodiumUser
              v-if="topThreeUsers[2]"
              :user="topThreeUsers[2]"
              :position="3"
            />
          </div>

          <!-- Loading -->
          <div class="loading" v-else-if="loading">
            <div class="loading-content">
              <div class="trophy-loader">
                <v-icon size="64" color="#ffcc00">mdi-trophy-variant</v-icon>
                <div class="loader-ring"></div>
              </div>
              <p>Cargando campeones...</p>
            </div>
          </div>

          <!-- Empty state -->
          <div class="empty-state" v-else>
            <v-icon size="80" color="rgba(255, 204, 0, 0.3)">mdi-trophy-outline</v-icon>
            <p>{{ $t('home_no_champions') }}</p>
          </div>
        </div>
      </div>

      <!-- Top Users List -->
      <UserList :users="allUsers" :loading="loading" />
    </v-container>
  </v-main>
</template>

<style scoped>
.user-view {
  min-height: 100vh;
  background: linear-gradient(180deg, #0a0e1a 0%, #0f1419 50%, #0a0e1a 100%);
  position: relative;
  padding: 2rem 0 4rem;
  overflow-x: hidden;
}

.background-gradient {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    radial-gradient(ellipse at 20% 30%, rgba(139, 92, 246, 0.15) 0%, transparent 50%),
    radial-gradient(ellipse at 80% 70%, rgba(59, 130, 246, 0.12) 0%, transparent 50%),
    radial-gradient(ellipse at 50% 50%, rgba(255, 215, 0, 0.05) 0%, transparent 70%);
  pointer-events: none;
}

/* Animated Orbs */
.animated-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
}

.orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.3;
  animation: orb-float 20s ease-in-out infinite;
}

.orb-1 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(139, 92, 246, 0.4), transparent);
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.orb-2 {
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.3), transparent);
  top: 60%;
  right: 5%;
  animation-delay: -7s;
}

.orb-3 {
  width: 250px;
  height: 250px;
  background: radial-gradient(circle, rgba(255, 215, 0, 0.25), transparent);
  bottom: 10%;
  left: 30%;
  animation-delay: -14s;
}

@keyframes orb-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  25% { transform: translate(30px, -30px) scale(1.1); }
  50% { transform: translate(-20px, 20px) scale(0.95); }
  75% { transform: translate(20px, 30px) scale(1.05); }
}

/* Particles Background */
.particles-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
  z-index: 1;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 215, 0, 0.5);
  border-radius: 50%;
  box-shadow: 0 0 10px rgba(255, 215, 0, 0.8);
  animation: float-particle linear infinite;
}

@keyframes float-particle {
  0% {
    transform: translateY(100vh) translateX(0) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) translateX(50px) scale(1);
    opacity: 0;
  }
}

.container {
  position: relative;
  z-index: 2;
  max-width: 1400px;
}

/* Podium Section */
.podium-section {
  margin: 3rem 0;
}

.section-title {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.section-title h2 {
  font-size: 2rem;
  font-weight: 800;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 3px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite;
  margin: 0;
}

.section-icon {
  font-size: 2rem;
  filter: drop-shadow(0 0 10px rgba(255, 215, 0, 0.5));
  animation: trophy-glow 2s ease-in-out infinite;
}

@keyframes trophy-glow {
  0%, 100% { transform: scale(1) rotate(-5deg); }
  50% { transform: scale(1.1) rotate(5deg); }
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

/* Podium Wrapper */
.podium-wrapper {
  margin-top: 2rem;
  padding: 2rem;
  background: linear-gradient(180deg, rgba(255, 215, 0, 0.05) 0%, transparent 100%);
  border-radius: 30px;
  border: 1px solid rgba(255, 215, 0, 0.1);
  position: relative;
}

.podium-wrapper::before {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 60%;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(255, 215, 0, 0.5), transparent);
}

.podium {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 2rem;
  padding: 2rem 0 1rem;
}

/* Loading */
.loading {
  display: flex;
  justify-content: center;
  padding: 4rem 0;
}

.loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.trophy-loader {
  position: relative;
  animation: trophy-bounce 1s ease-in-out infinite;
}

@keyframes trophy-bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-15px); }
}

.loader-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100px;
  height: 100px;
  border: 3px solid rgba(255, 215, 0, 0.2);
  border-top-color: #FFD700;
  border-radius: 50%;
  animation: loader-spin 1s linear infinite;
}

@keyframes loader-spin {
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

.loading-content p {
  color: rgba(255, 204, 0, 0.8);
  font-size: 1.1rem;
  font-weight: 600;
  letter-spacing: 1px;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 0;
  gap: 1.5rem;
}

.empty-state p {
  color: rgba(255, 255, 255, 0.5);
  font-size: 1.2rem;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 968px) {
  .podium {
    flex-direction: column;
    align-items: center;
    gap: 3rem;
  }

  .podium-wrapper {
    padding: 1rem;
  }

  .section-title h2 {
    font-size: 1.5rem;
  }
}

@media (max-width: 480px) {
  .container {
    padding: 0 1rem;
  }

  .section-title {
    gap: 0.5rem;
  }

  .section-title h2 {
    font-size: 1.2rem;
  }

  .section-icon {
    font-size: 1.5rem;
  }
}
</style>
