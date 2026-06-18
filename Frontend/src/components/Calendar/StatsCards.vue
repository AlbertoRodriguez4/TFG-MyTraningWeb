<template>
  <div class="stats-cards-floating">
    <v-card class="floating-stat-card card-1" elevation="12" rounded="xl">
      <v-card-text class="pa-4">
        <div class="d-flex align-center">
          <div class="mini-icon-wrapper purple">
            <v-icon color="white" size="24">mdi-star</v-icon>
          </div>
          
          <div class="ml-3">
            <div class="text-caption text-uppercase stat-label">
              Nivel Actual
            </div>
            <div class="text-h5 font-weight-black stat-value">
              {{ userLevel }}
            </div>
          </div>
        </div>
        
        <v-progress-linear
          :value="xpProgress"
          height="6"
          rounded
          color="purple"
          background-color="grey lighten-3"
          class="mt-3 xp-progress"
        ></v-progress-linear>
        
        <div class="text-caption mt-1 xp-text">
          {{ userXP }} / {{ xpToNextLevel }} XP
        </div> 
      </v-card-text>
    </v-card>

    <v-card class="floating-stat-card card-2" elevation="12" rounded="xl">
      <v-card-text class="pa-4">
        <div class="d-flex align-center justify-space-between">
          <div class="d-flex align-center">
            <div class="mini-icon-wrapper amber">
              <v-icon color="white" size="24">mdi-gold</v-icon>
            </div>
            
            <div class="ml-3">
              <div class="text-caption text-uppercase stat-label">
                Monedas
              </div>
              <div class="text-h5 font-weight-black stat-value">
                {{ coins.toLocaleString() }}
              </div>
            </div>
          </div>
          
          <v-icon color="white" size="40" class="treasure-icon">
            mdi-treasure-chest
          </v-icon>
        </div>
      </v-card-text>
    </v-card>

    <v-card class="floating-stat-card card-3" elevation="12" rounded="xl">
      <v-card-text class="pa-4">
        <div class="d-flex align-center justify-space-between">
          <div class="d-flex align-center">
            <div class="mini-icon-wrapper green">
              <v-icon color="white" size="24">mdi-check-all</v-icon>
            </div>
            
            <div class="ml-3">
              <div class="text-caption text-uppercase stat-label">
                Completadas
              </div>
              <div class="text-h5 font-weight-black stat-value">
                {{ completedRoutines }}
              </div>
            </div>
          </div>
          
          <v-chip
            v-if="completedRoutines >= 5"
            small
            color="green"
            dark
            class="motivation-chip"
          >
            <v-icon small left>mdi-fire</v-icon>
            ¡En racha!
          </v-chip>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  userLevel: number;
  userXP: number;
  xpToNextLevel: number;
  xpProgress: number;
  coins: number;
  completedRoutines: number;
}

const props = defineProps<Props>();

// Computed para mostrar el chip de motivación si el usuario ha completado 5 o más rutinas
const showMotivation = computed((): boolean => {
  return props.completedRoutines >= 5;
});

// Computed para determinar el color de la barra de progreso según el porcentaje de XP
const progressColor = computed((): string => {
  if (props.xpProgress >= 80) return 'success';
  if (props.xpProgress >= 50) return 'purple';
  return 'orange';
});
</script>

<style scoped>
/* Contenedor principal de tarjetas */
.stats-cards-floating {
  position: relative;
  width: 100%;
}

/* Tarjeta estática - Tema oscuro futurista */
.floating-stat-card {
  margin-bottom: 1rem;
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.9) 0%, rgba(15, 10, 26, 0.95) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.card-2 {
  margin-left: 2rem;
}

/* Wrapper del icono circular */
.mini-icon-wrapper {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Colores de los iconos - Tema oscuro */
.mini-icon-wrapper.purple {
  background: linear-gradient(135deg, #a78bfa 0%, #22d3ee 100%);
  box-shadow: 0 4px 16px rgba(167, 139, 250, 0.4);
}

.mini-icon-wrapper.amber {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  box-shadow: 0 4px 16px rgba(251, 191, 36, 0.4);
}

.mini-icon-wrapper.green {
  background: linear-gradient(135deg, #34d399 0%, #10b981 100%);
  box-shadow: 0 4px 16px rgba(52, 211, 153, 0.4);
}

/* Labels y valores - Tema oscuro */
.stat-label {
  font-weight: 700;
  letter-spacing: 0.5px;
  font-size: 0.7rem;
  line-height: 1;
  margin-bottom: 4px;
  color: rgba(255, 255, 255, 0.6);
}

.stat-value {
  line-height: 1;
  color: #ffffff;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.3);
}

/* Barra de progreso XP - Tema oscuro */
.xp-progress {
  border-radius: 10px;
  overflow: hidden;
  background: rgba(255, 255, 255, 0.1) !important;
}

.xp-progress >>> .v-progress-linear__determinate {
  transition: width 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Texto de XP - Tema oscuro */
.xp-text {
  font-weight: 600;
  font-size: 0.75rem;
  text-align: center;
  letter-spacing: 0.3px;
  color: rgba(255, 255, 255, 0.7);
}

/* Icono decorativo del cofre */
.treasure-icon {
  opacity: 0.7;
}

/* Chip de motivación */
.motivation-chip {
  font-weight: 700;
  letter-spacing: 0.5px;
}

/* Responsive Design */
@media (max-width: 960px) {
  .stats-cards-floating {
    margin-top: 2rem;
  }
  
  .floating-stat-card {
    margin-left: 0 !important;
    margin-bottom: 1rem;
  }
  
  .card-2,
  .card-3 {
    margin-left: 0 !important;
  }
}

@media (max-width: 600px) {
  .floating-stat-card {
    margin-bottom: 0.75rem;
  }
  
  .floating-stat-card .v-card__text {
    padding: 0.75rem !important;
  }
  
  .mini-icon-wrapper {
    width: 45px;
    height: 45px;
  }
  
  .mini-icon-wrapper .v-icon {
    font-size: 20px !important;
  }
  
  .stat-label {
    font-size: 0.65rem;
  }
  
  .stat-value {
    font-size: 1.3rem !important;
  }
  
  .treasure-icon {
    font-size: 32px !important;
  }
  
  .xp-progress {
    height: 5px !important;
  }
  
  .xp-text {
    font-size: 0.7rem;
  }
  
  .motivation-chip {
    height: 20px !important;
    font-size: 0.65rem;
  }
  
  .motivation-chip .v-icon {
    font-size: 14px !important;
  }
}

@media (max-width: 400px) {
  .mini-icon-wrapper {
    width: 40px;
    height: 40px;
  }
  
  .mini-icon-wrapper .v-icon {
    font-size: 18px !important;
  }
  
  .stat-value {
    font-size: 1.1rem !important;
  }
  
  .treasure-icon {
    font-size: 28px !important;
  }
}


/* Accesibilidad */
@media (prefers-reduced-motion: reduce) {
  .floating-stat-card,
  .mini-icon-wrapper,
  .treasure-icon,
  .motivation-chip {

    transition: none !important;
  }
}
</style>