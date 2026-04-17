<template>
  <v-dialog v-model="showDialog" max-width="1200" persistent scrollable>
    <v-card class="routine-stats-card">
      <!-- Header -->
      <div class="stats-header">
        <div class="header-content">
          <v-icon large color="white" class="mr-3">mdi-chart-timeline-variant</v-icon>
          <div>
            <h2 class="stats-title">Estadísticas de Entrenamiento</h2>
            <p class="stats-subtitle">Resumen de tu progreso por mes</p>
          </div>
        </div>
        <v-btn icon dark @click="closeDialog">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <!-- Resumen General -->
      <v-card-text class="pa-6">
        <div class="summary-cards">
          <div class="summary-card">
            <v-icon color="purple" size="40">mdi-dumbbell</v-icon>
            <div class="summary-value">{{ totalRoutines }}</div>
            <div class="summary-label">Rutinas Totales</div>
          </div>
          <div class="summary-card">
            <v-icon color="success" size="40">mdi-check-circle</v-icon>
            <div class="summary-value">{{ completedRoutines }}</div>
            <div class="summary-label">Completadas</div>
          </div>
          <div class="summary-card">
            <v-icon color="warning" size="40">mdi-clock-outline</v-icon>
            <div class="summary-value">{{ pendingRoutines }}</div>
            <div class="summary-label">Pendientes</div>
          </div>
          <div class="summary-card">
            <v-icon color="info" size="40">mdi-percent</v-icon>
            <div class="summary-value">{{ completionRate }}%</div>
            <div class="summary-label">Tasa de Éxito</div>
          </div>
        </div>

        <!-- Estadísticas por Mes -->
        <div class="monthly-stats">
          <h3 class="section-title">
            <v-icon color="purple" class="mr-2">mdi-calendar-month</v-icon>
            Estadísticas Mensuales
          </h3>

          <!-- Si no hay datos -->
          <v-alert v-if="monthlyData.length === 0" type="info" outlined class="mt-4">
            <div class="text-center">
              <v-icon large color="info" class="mb-2">mdi-information</v-icon>
              <p class="text-h6 mb-0">No hay rutinas registradas aún</p>
              <p class="text-body-2">Empieza a crear rutinas para ver tus estadísticas</p>
            </div>
          </v-alert>

          <!-- Grid de meses -->
          <div v-else class="months-grid">
            <div
              v-for="month in monthlyData"
              :key="month.key"
              class="month-card"
              @click="selectMonth(month)"
            >
              <div class="month-header">
                <div class="month-name">{{ month.name }}</div>
                <div class="month-year">{{ month.year }}</div>
              </div>

              <div class="month-stats">
                <div class="stat-row">
                  <span class="stat-label">
                    <v-icon small color="success">mdi-check</v-icon>
                    Completadas
                  </span>
                  <span class="stat-value success--text">{{ month.completed }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">
                    <v-icon small color="warning">mdi-clock</v-icon>
                    Pendientes
                  </span>
                  <span class="stat-value warning--text">{{ month.pending }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">
                    <v-icon small color="purple">mdi-sigma</v-icon>
                    Total
                  </span>
                  <span class="stat-value purple--text">{{ month.total }}</span>
                </div>
              </div>

              <!-- Barra de progreso -->
              <div class="month-progress">
                <div class="progress-bar">
                  <div
                    class="progress-fill"
                    :style="{ width: `${month.completionRate}%` }"
                  ></div>
                </div>
                <span class="progress-label">{{ month.completionRate }}% completado</span>
              </div>

              <!-- Badge de XP ganado -->
              <div class="xp-earned">
                <v-icon small color="amber">mdi-star</v-icon>
                <span>+{{ month.totalXP }} XP</span>
              </div>
            </div>
          </div>
        </div>
      </v-card-text>

      <!-- Footer -->
      <v-card-actions class="stats-footer">
        <v-spacer></v-spacer>
        <v-btn color="purple" text large @click="closeDialog">
          Cerrar
        </v-btn>
      </v-card-actions>
    </v-card>

    <!-- Diálogo de detalle del mes -->
    <MonthDetailDialog
      v-model="showMonthDetail"
      :month-data="selectedMonth"
      @close="closeMonthDetail"
    />
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoutineStore } from '@/stores/RoutineStore';
import type { Routines } from '@/components/Models/Routines';
import MonthDetailDialog from './MonthDetailDialog.vue';

interface MonthlyStats {
  key: string;
  name: string;
  year: number;
  month: number;
  total: number;
  completed: number;
  pending: number;
  completionRate: number;
  totalXP: number;
  routines: Routines[];
}

interface Props {
  modelValue: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
}>();

const routineStore = useRoutineStore();
const showMonthDetail = ref(false);
const selectedMonth = ref<MonthlyStats | null>(null);

const showDialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
});

const monthNames = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
];

/**
 * Total de rutinas creadas
 */
const totalRoutines = computed(() => {
  return routineStore.routines.length;
});

/**
 * Rutinas completadas
 */
const completedRoutines = computed(() => {
  return routineStore.routines.filter(r => r.iscompleted).length;
});

/**
 * Rutinas pendientes
 */
const pendingRoutines = computed(() => {
  return routineStore.routines.filter(r => !r.iscompleted).length;
});

/**
 * Tasa de éxito (porcentaje de completadas)
 */
const completionRate = computed(() => {
  if (totalRoutines.value === 0) return 0;
  return Math.round((completedRoutines.value / totalRoutines.value) * 100);
});

/**
 * Agrupa las rutinas por mes y año
 */
const monthlyData = computed((): MonthlyStats[] => {
  const grouped = new Map<string, MonthlyStats>();

  routineStore.routines.forEach(routine => {
    const date = new Date(routine.createdat);
    const year = date.getFullYear();
    const month = date.getMonth();
    const key = `${year}-${month}`;

    if (!grouped.has(key)) {
      grouped.set(key, {
        key,
        name: monthNames[month],
        year,
        month,
        total: 0,
        completed: 0,
        pending: 0,
        completionRate: 0,
        totalXP: 0,
        routines: []
      });
    }

    const monthData = grouped.get(key)!;
    monthData.total++;
    monthData.routines.push(routine);

    if (routine.iscompleted) {
      monthData.completed++;
      monthData.totalXP += routine.reward;
    } else {
      monthData.pending++;
    }
  });

  // Calcular porcentaje de completado para cada mes
  grouped.forEach(monthData => {
    monthData.completionRate = monthData.total > 0
      ? Math.round((monthData.completed / monthData.total) * 100)
      : 0;
  });

  // Ordenar por año y mes (más reciente primero)
  return Array.from(grouped.values()).sort((a, b) => {
    if (a.year !== b.year) return b.year - a.year;
    return b.month - a.month;
  });
});

/**
 * Selecciona un mes para ver detalles
 */
const selectMonth = (month: MonthlyStats): void => {
  selectedMonth.value = month;
  showMonthDetail.value = true;
};

/**
 * Cierra el diálogo de detalle del mes
 */
const closeMonthDetail = (): void => {
  showMonthDetail.value = false;
  selectedMonth.value = null;
};

/**
 * Cierra el diálogo principal
 */
const closeDialog = (): void => {
  emit('update:modelValue', false);
};
</script>

<style scoped>
.routine-stats-card {
  border-radius: 16px;
  overflow: hidden;
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
}

.stats-header {
  background: linear-gradient(135deg, rgba(167, 139, 250, 0.3) 0%, rgba(34, 211, 238, 0.2) 100%);
  color: white;
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  border-bottom: 2px solid rgba(139, 92, 246, 0.3);
}

.header-content {
  display: flex;
  align-items: center;
}

.stats-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0;
}

.stats-subtitle {
  font-size: 0.95rem;
  opacity: 0.9;
  margin: 0;
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.summary-card {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.6) 0%, rgba(15, 10, 26, 0.8) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 16px;
  padding: 1.5rem;
  text-align: center;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.summary-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(139, 92, 246, 0.3);
  border-color: rgba(167, 139, 250, 0.6);
}

.summary-value {
  font-size: 2.5rem;
  font-weight: 800;
  color: #ffffff;
  margin: 0.5rem 0;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.4);
}

.summary-label {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #ffffff;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
}

.months-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.month-card {
  background: rgba(26, 10, 46, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
  backdrop-filter: blur(10px);
}

.month-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(139, 92, 246, 0.3);
  border-color: rgba(167, 139, 250, 0.6);
}

.month-header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: 1rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid rgba(139, 92, 246, 0.2);
}

.month-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: rgba(167, 139, 250, 0.9);
}

.month-year {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

.month-stats {
  margin-bottom: 1rem;
}

.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  color: white;
}

.stat-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.7);
}

.stat-value {
  font-size: 1.2rem;
  font-weight: 700;
}

.month-progress {
  margin: 1rem 0;
}

.progress-bar {
  height: 8px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #a78bfa, #22d3ee);
  border-radius: 4px;
  transition: width 0.5s ease;
}

.progress-label {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

.xp-earned {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.3) 0%, rgba(245, 158, 11, 0.2) 100%);
  border: 2px solid rgba(251, 191, 36, 0.4);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: 700;
  font-size: 0.95rem;
  width: fit-content;
  margin-top: 1rem;
}

.stats-footer {
  background: rgba(15, 10, 26, 0.8);
  border-top: 1px solid rgba(139, 92, 246, 0.3);
  padding: 16px 24px;
}

@media (max-width: 960px) {
  .summary-cards {
    grid-template-columns: repeat(2, 1fr);
  }

  .months-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .stats-header {
    padding: 20px;
  }

  .stats-title {
    font-size: 1.25rem;
  }

  .stats-subtitle {
    font-size: 0.85rem;
  }

  .summary-cards {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .summary-value {
    font-size: 2rem;
  }

  .month-name {
    font-size: 1.25rem;
  }
}
</style>