<script setup lang="ts">
import { computed } from 'vue'
import type { Stats } from '@/components/Models/User'

interface Props {
  stats: Stats
}

const props = defineProps<Props>()
// Configuración de estadísticas para la comunidad, que incluye el número total de usuarios, usuarios activos hoy y total de check-ins, 
// junto con sus respectivos íconos, etiquetas y estilos de color y gradiente para cada estadística.
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
    color: '#ffcc00',
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
      <div class="stat-shine"></div>
      <div  :style="{ background: `radial-gradient(ellipse, ${stat.color}40, transparent 70%)` }"></div>

      <div class="stat-content">
        <div class="icon-badge" :style="{ background: stat.gradient }">
          <v-icon size="24" color="#fff">
            {{ stat.icon }}
          </v-icon>
          <div class="icon-shine"></div>
        </div>

        <div class="stat-value-wrapper">
          <span class="stat-value">{{ stat.value.toLocaleString() }}</span>
        </div>

        <div class="stat-label">{{ stat.label }}</div>

        <div class="stat-progress">
          <div class="progress-fill" :style="{ background: stat.gradient }"></div>
        </div>
      </div>

      <div class="corner corner-tl"></div>
      <div class="corner corner-br"></div>
    </div>
  </div>
</template>

<style scoped>
.community-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  max-width: 900px;
  margin: 2.5rem auto 3rem;
  padding: 0 1rem;
}

.stat-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.95) 0%, rgba(15, 23, 42, 0.98) 100%);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 24px;
  padding: 1.5rem;
  text-align: center;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  cursor: pointer;
  overflow: hidden;

}

.stat-card:nth-child(1) { animation-delay: 0.1s; }
.stat-card:nth-child(2) { animation-delay: 0.2s; }
.stat-card:nth-child(3) { animation-delay: 0.3s; }

/* Background Effects */
.stat-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0.05;
  transition: opacity 0.4s ease;
}

.stat-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.1),
    transparent
  );
  transition: left 0.6s ease;
}

/* Content */
.stat-content {
  position: relative;
  z-index: 2;
}

/* Icon Badge */
.icon-badge {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 56px;
  height: 56px;
  border-radius: 16px;
  margin-bottom: 1rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  transition: transform 0.3s ease;
}

.icon-shine {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.3) 0%, transparent 50%);
  border-radius: 16px;
  pointer-events: none;
}

/* Value */
.stat-value-wrapper {
  position: relative;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 2.2rem;
  font-weight: 900;
  color: #fff;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
  line-height: 1;
  letter-spacing: -1px;
}

/* Label */
.stat-label {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.5);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  margin-bottom: 1rem;
}

/* Progress Bar */
.stat-progress {
  position: relative;
  width: 100%;
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 2px;
  overflow: hidden;
}

.progress-fill {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  width: 70%;
  border-radius: 2px;
  box-shadow: 0 0 10px currentColor;
}

/* Corner Decorations */
.corner {
  position: absolute;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.corner-tl {
  top: 12px;
  left: 12px;
  border-right: none;
  border-bottom: none;
  border-radius: 4px 0 0 0;
}

.corner-br {
  bottom: 12px;
  right: 12px;
  border-left: none;
  border-top: none;
  border-radius: 0 0 4px 0;
}

/* Responsive */
@media (max-width: 768px) {
  .community-stats {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }

  .stat-card {
    padding: 1.25rem;
  }

  .stat-value {
    font-size: 1.8rem;
  }

  .icon-badge {
    width: 48px;
    height: 48px;
  }
}

@media (max-width: 480px) {
  .community-stats {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }

  .stat-card {
    padding: 1.25rem;
  }

  .stat-value {
    font-size: 1.6rem;
  }

  .stat-label {
    font-size: 0.75rem;
  }

  .icon-badge {
    width: 44px;
    height: 44px;
  }
}
</style>
