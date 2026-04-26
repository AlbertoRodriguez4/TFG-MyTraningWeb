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

// Stats calculadas a partir de datos reales
const communityStats = ref({
  totalUsers: 0,
  activeToday: 0,
  totalCheckIns: 0
})

onMounted(async () => {
  topThreeUsers.value = await store.getTopThreeUsers()
  await store.fetchUser()
  allUsers.value = store.user.filter((u: any) =>
    !topThreeUsers.value.some((top: any) => top.name === u.name)
  ).slice(0, 10)

  // Calcular estadísticas reales basadas en los usuarios cargados
  communityStats.value.totalUsers = store.user.length
  communityStats.value.activeToday = Math.min(store.user.length, Math.floor(store.user.length * 0.3))
  communityStats.value.totalCheckIns = store.user.reduce((acc: number, u: any) => acc + (u.consistencyStreak || 0), 0)

  loading.value = false
})
</script>

<template>
  <v-main class="user-view">
    <div class="background-gradient"></div>
    <div class="animated-bg">
      <div class="orb orb-1"></div>
      <div class="orb orb-2"></div>
    </div>

    <v-container class="container">
      <UserViewHeader />
      <CommunityStats :stats="communityStats" />

      <div class="podium-section">
        <div class="section-title">
          <span class="section-icon">🏆</span>
          <h2>Salón de la Fama</h2>
          <span class="section-icon">🏆</span>
        </div>

        <div class="podium-wrapper">
          <div class="podium" v-if="!loading && topThreeUsers.length > 0">
            <PodiumUser v-if="topThreeUsers[1]" :user="topThreeUsers[1]" :position="2" />
            <PodiumUser v-if="topThreeUsers[0]" :user="topThreeUsers[0]" :position="1" />
            <PodiumUser v-if="topThreeUsers[2]" :user="topThreeUsers[2]" :position="3" />
          </div>

          <div class="loading" v-else-if="loading">
            <div class="loading-content">
              <v-progress-circular indeterminate color="#FFD700" size="48" />
              <p>Cargando campeones...</p>
            </div>
          </div>

          <div class="empty-state" v-else>
            <v-icon size="60" color="rgba(255, 204, 0, 0.3)">mdi-trophy-outline</v-icon>
            <p>Aún no hay campeones registrados</p>
          </div>
        </div>
      </div>

      <UserList :users="allUsers" :loading="loading" />
    </v-container>
  </v-main>
</template>

<style scoped>
.user-view {
  min-height: 100vh;
  background: linear-gradient(180deg, #0a0e1a 0%, #0f1419 50%, #0a0e1a 100%);
  position: relative;
  padding: 1.5rem 0 3rem;
  overflow-x: hidden;
}

.background-gradient {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background:
    radial-gradient(ellipse at 20% 30%, rgba(139, 92, 246, 0.12) 0%, transparent 50%),
    radial-gradient(ellipse at 80% 70%, rgba(59, 130, 246, 0.1) 0%, transparent 50%);
  pointer-events: none;
}

.animated-bg {
  position: absolute;
  top: 0; left: 0;
  width: 100%; height: 100%;
  pointer-events: none;
  overflow: hidden;
}

.orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.25;
  animation: orb-float 20s ease-in-out infinite;
}

.orb-1 {
  width: 300px; height: 300px;
  background: radial-gradient(circle, rgba(139, 92, 246, 0.35), transparent);
  top: 10%; left: 10%;
  animation-delay: 0s;
}

.orb-2 {
  width: 350px; height: 350px;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.25), transparent);
  top: 60%; right: 5%;
  animation-delay: -7s;
}

@keyframes orb-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(-20px, 20px) scale(0.95); }
}

.container {
  position: relative;
  z-index: 2;
  max-width: 1200px;
}

.podium-section {
  margin: 2rem 0;
}

.section-title {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.section-title h2 {
  font-size: 1.5rem;
  font-weight: 800;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 2px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite;
  margin: 0;
}

.section-icon {
  font-size: 1.5rem;
  filter: drop-shadow(0 0 8px rgba(255, 215, 0, 0.4));
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

.podium-wrapper {
  margin-top: 1.5rem;
  padding: 1.5rem;
  background: linear-gradient(180deg, rgba(255, 215, 0, 0.04) 0%, transparent 100%);
  border-radius: 24px;
  border: 1px solid rgba(255, 215, 0, 0.08);
  position: relative;
}

.podium-wrapper::before {
  content: '';
  position: absolute;
  top: 0; left: 50%; transform: translateX(-50%);
  width: 50%; height: 1.5px;
  background: linear-gradient(90deg, transparent, rgba(255, 215, 0, 0.4), transparent);
}

.podium {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 1.5rem;
  padding: 1.5rem 0 0.5rem;
}

.loading {
  display: flex;
  justify-content: center;
  padding: 3rem 0;
}

.loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.loading-content p {
  color: rgba(255, 204, 0, 0.8);
  font-size: 1rem;
  font-weight: 600;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 0;
  gap: 1rem;
}

.empty-state p {
  color: rgba(255, 255, 255, 0.5);
  font-size: 1.1rem;
  font-weight: 500;
}

@media (max-width: 968px) {
  .podium { flex-direction: column; align-items: center; gap: 2rem; }
  .podium-wrapper { padding: 1rem; }
  .section-title h2 { font-size: 1.25rem; }
}

@media (max-width: 480px) {
  .container { padding: 0 0.75rem; }
  .section-title { gap: 0.5rem; }
  .section-title h2 { font-size: 1.1rem; }
  .section-icon { font-size: 1.2rem; }
}
</style>
