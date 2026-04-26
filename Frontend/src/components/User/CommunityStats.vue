<script setup lang="ts">
import { computed } from 'vue'

interface Stats {
  totalUsers: number
  activeToday: number
  totalCheckIns: number
}

interface Props {
  stats: Stats
}

const props = defineProps<Props>()

const statsConfig = computed(() => [
  {
    key: 'totalUsers',
    value: props.stats.totalUsers,
    icon: 'mdi-account-multiple',
    label: 'Usuarios Totales',
    color: '#ffcc00',
    gradient: 'linear-gradient(135deg, #FFD700 0%, #FFA500 100%)'
  },
  {
    key: 'activeToday',
    value: props.stats.activeToday,
    icon: 'mdi-account-check',
    label: 'Activos Hoy',
    color: '#4ade80',
    gradient: 'linear-gradient(135deg, #4ade80 0%, #22c55e 100%)'
  },
  {
    key: 'totalCheckIns',
    value: props.stats.totalCheckIns,
    icon: 'mdi-check-circle',
    label: 'Check-ins',
    color: '#6366f1',
    gradient: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)'
  }
])
</script>

<template>
  <div class="community-stats">
    <div
      v-for="stat in statsConfig"
      :key="stat.key"
      class="stat-card"
      :style="{ '--stat-color': stat.color }"
    >
      <div class="stat-bg" :style="{ background: stat.gradient }"></div>
      <div class="stat-content">
        <div class="icon-badge" :style="{ background: stat.gradient }">
          <v-icon size="20" color="#fff">{{ stat.icon }}</v-icon>
        </div>
        <span class="stat-value">{{ stat.value.toLocaleString() }}</span>
        <span class="stat-label">{{ stat.label }}</span>
        <div class="stat-progress">
          <div class="progress-fill" :style="{ background: stat.gradient }"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.community-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1rem;
  max-width: 800px;
  margin: 2rem auto 2.5rem;
  padding: 0 1rem;
}

.stat-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.95) 0%, rgba(15, 23, 42, 0.98) 100%);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 18px;
  padding: 1.25rem;
  text-align: center;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  cursor: pointer;
  overflow: hidden;
  animation: stat-entrance 0.5s ease-out backwards;
}

.stat-card:nth-child(1) { animation-delay: 0.1s; }
.stat-card:nth-child(2) { animation-delay: 0.2s; }
.stat-card:nth-child(3) { animation-delay: 0.3s; }

@keyframes stat-entrance {
  0% { opacity: 0; transform: translateY(20px) scale(0.95); }
  100% { opacity: 1; transform: translateY(0) scale(1); }
}

.stat-bg {
  position: absolute;
  top: 0; left: 0;
  width: 100%; height: 100%;
  opacity: 0.04;
  transition: opacity 0.3s ease;
}

.stat-card:hover .stat-bg {
  opacity: 0.1;
}

.stat-card:hover {
  transform: translateY(-5px) scale(1.02);
  border-color: var(--stat-color);
  box-shadow:
    0 10px 30px rgba(0, 0, 0, 0.3),
    0 0 30px color-mix(in srgb, var(--stat-color) 20%, transparent);
}

.stat-content {
  position: relative;
  z-index: 2;
}

.icon-badge {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
  border-radius: 12px;
  margin-bottom: 0.75rem;
  box-shadow: 0 3px 12px rgba(0, 0, 0, 0.3);
}

.stat-value {
  font-size: 1.8rem;
  font-weight: 900;
  color: #fff;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.4);
  line-height: 1;
  letter-spacing: -1px;
  display: block;
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  display: block;
  margin-bottom: 0.75rem;
}

.stat-progress {
  position: relative;
  width: 100%;
  height: 3px;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 2px;
  overflow: hidden;
}

.progress-fill {
  position: absolute;
  top: 0; left: 0;
  height: 100%;
  width: 60%;
  border-radius: 2px;
}

@media (max-width: 768px) {
  .community-stats {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.875rem;
  }
  .stat-card { padding: 1rem; }
  .stat-value { font-size: 1.5rem; }
  .icon-badge { width: 40px; height: 40px; }
}

@media (max-width: 480px) {
  .community-stats {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }
}
</style>
