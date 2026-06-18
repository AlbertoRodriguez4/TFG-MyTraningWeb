<template>
  <v-dialog v-model="showDialog" max-width="800" scrollable>
    <v-card v-if="monthData" class="month-detail-card">
      <div class="detail-header">
        <div class="header-content">
          <v-icon large color="white" class="mr-3">mdi-calendar-month</v-icon>
          <div>
            <h2 class="detail-title">{{ monthData.name }} {{ monthData.year }}</h2>
            <p class="detail-subtitle">
              {{ t('monthDetail.summary', { total: monthData.total, completed: monthData.completed }) }}
            </p>
          </div>
        </div>
        <v-btn icon dark @click="closeDialog">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <v-card-text class="pa-6">
        <div class="mini-stats">
          <div class="mini-stat success">
            <v-icon color="white">mdi-check-circle</v-icon>
            <div class="mini-stat-value">{{ monthData.completed }}</div>
            <div class="mini-stat-label">{{ t('common.completed') }}</div>
          </div>
          <div class="mini-stat warning">
            <v-icon color="white">mdi-clock-outline</v-icon>
            <div class="mini-stat-value">{{ monthData.pending }}</div>
            <div class="mini-stat-label">{{ t('common.pending') }}</div>
          </div>
          <div class="mini-stat info">
            <v-icon color="white">mdi-star</v-icon>
            <div class="mini-stat-value">{{ monthData.totalXP }}</div>
            <div class="mini-stat-label">{{ t('common.totalXP') }}</div>
          </div>
        </div>

        <div class="routines-list">
          <h3 class="list-title">
            <v-icon color="white" class="mr-2">mdi-dumbbell</v-icon>
            {{ t('monthDetail.monthRoutines') }}
          </h3>

          <div v-for="routine in sortedRoutines" :key="routine.id" class="routine-item"
            :class="{ 'completed': routine.iscompleted }">
            <div class="routine-date">
              <div class="date-circle" :class="{ 'completed': routine.iscompleted }">
                {{ new Date(routine.createdat).getDate() }}
              </div>
              <div class="date-info">
                <div class="weekday">{{ (routine.createdat) }}</div>
                <div class="full-date">{{ (routine.createdat) }}</div>
              </div>
            </div>

            <div class="routine-info">
              <div class="routine-name">
                <v-icon :color="white" class="mr-2">
                  {{ routine.iscompleted ? 'mdi-check-circle' : 'mdi-clock-outline' }}
                </v-icon>
                {{ routine.name }}
              </div>
              <div class="routine-description">{{ routine.description }}</div>
            </div>

            <div class="routine-stats">
              <v-chip small :color="getDifficultyColor(routine.difficulty)" dark class="mb-2">
                {{ getDifficultyText(routine.difficulty) }}
              </v-chip>
              <div class="reward-badge">
                <v-icon small color="white">mdi-star</v-icon>
                <span>+{{ routine.reward }} XP</span>
              </div>
            </div>
          </div>
        </div>
      </v-card-text>

      <v-card-actions class="detail-footer">
        <v-spacer></v-spacer>
        <v-btn color="purple" text large @click="closeDialog">
          {{ t('common.close') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import type { MonthlyStats } from '@/components/Models/Calendar';

const { t, locale } = useI18n();

interface Props {
  modelValue: boolean;
  monthData: MonthlyStats | null;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'close': [];
}>();

const showDialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
});

/**
 * Rutinas ordenadas por fecha (más reciente primero)
 */
const sortedRoutines = computed(() => {
  if (!props.monthData) return [];
  return [...props.monthData.routines].sort((a, b) => {
    return new Date(b.createdat).getTime() - new Date(a.createdat).getTime();
  });
});

/**
 * Formatea el día del mes
 */
const formatDay = (dateString: string): string => {
  const date = new Date(dateString);
  return date.getDate().toString();
};

/**
 * Formatea el día de la semana
 */
const formatWeekday = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString(locale.value, { weekday: 'short' });
};

/**
 * Formatea la fecha completa
 */
const formatFullDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString(locale.value, { day: 'numeric', month: 'short' });
};

/**
 * Obtiene el texto de la dificultad
 */
const getDifficultyText = (difficulty: number): string => {
  const map: Record<number, string> = {
    1: t('difficulty.easy'),
    2: t('difficulty.medium'),
    3: t('difficulty.hard'),
    4: t('difficulty.extreme')
  };
  return map[difficulty] || t('difficulty.medium');
};

/**
 * Obtiene el color de la dificultad
 */
const getDifficultyColor = (difficulty: number): string => {
  const colors: Record<number, string> = {
    1: 'success',
    2: 'info',
    3: 'warning',
    4: 'error'
  };
  return colors[difficulty] || 'grey';
};

/**
 * Cierra el diálogo
 */
const closeDialog = (): void => {
  emit('close');
};
</script>

<style scoped>
.month-detail-card {
  border-radius: 16px;
  overflow: hidden;
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
}

.detail-header {
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

.detail-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0;
}

.detail-subtitle {
  font-size: 0.95rem;
  opacity: 0.9;
  margin: 0;
}

.mini-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.mini-stat {
  background: rgba(26, 10, 46, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 1rem;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  border-left: 4px solid;
  backdrop-filter: blur(10px);
}

.mini-stat.success {
  border-left-color: rgba(52, 211, 153, 0.8);
}

.mini-stat.warning {
  border-left-color: rgba(251, 191, 36, 0.8);
}

.mini-stat.info {
  border-left-color: rgba(251, 191, 36, 0.8);
}

.mini-stat-value {
  font-size: 2rem;
  font-weight: 800;
  color: #ffffff;
  margin: 0.5rem 0;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.4);
}

.mini-stat-label {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
  text-transform: uppercase;
}

.list-title {
  font-size: 1.3rem;
  font-weight: 700;
  color: #ffffff;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
}

.routines-list {
  max-height: 500px;
  overflow-y: auto;
  overflow-x: hidden;
}

.routine-item {
  background: rgba(26, 10, 46, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 1rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  display: grid;
  grid-template-columns: auto 1fr auto;
  gap: 1.5rem;
  align-items: center;
  transition: all 0.3s ease;
  border-left: 4px solid rgba(251, 191, 36, 0.6);
  backdrop-filter: blur(10px);
}

.routine-item.completed {
  border-left-color: rgba(52, 211, 153, 0.8);
  background: linear-gradient(135deg, rgba(52, 211, 153, 0.1) 0%, rgba(16, 185, 129, 0.05) 100%);
}

.routine-date {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.date-circle {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.8), rgba(245, 158, 11, 0.6));
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 800;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
}

.date-circle.completed {
  background: linear-gradient(135deg, rgba(52, 211, 153, 0.8), rgba(16, 185, 129, 0.6));
  box-shadow: 0 4px 12px rgba(52, 211, 153, 0.4);
}

.date-info {
  display: flex;
  flex-direction: column;
}

.weekday {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 600;
  text-transform: uppercase;
}

.full-date {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.8);
  font-weight: 700;
}

.routine-info {
  flex: 1;
}

.routine-name {
  font-size: 1.1rem;
  font-weight: 700;
  color: #ffffff;
  margin-bottom: 0.5rem;
  display: flex;
  align-items: center;
}

.routine-description {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.4;
}

.routine-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
}

.reward-badge {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  background: linear-gradient(135deg, #ffc107, #ff9800);
  color: white;
  padding: 0.4rem 0.8rem;
  border-radius: 16px;
  font-weight: 700;
  font-size: 0.9rem;
}

.detail-footer {
  background: rgba(15, 10, 26, 0.8);
  border-top: 1px solid rgba(139, 92, 246, 0.3);
  padding: 16px 24px;
}

@media (max-width: 768px) {
  .routine-item {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .routine-stats {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
  }

  .date-circle {
    width: 50px;
    height: 50px;
    font-size: 1.25rem;
  }

  .mini-stats {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .detail-header {
    padding: 20px;
  }

  .detail-title {
    font-size: 1.25rem;
  }

  .detail-subtitle {
    font-size: 0.85rem;
  }

  .routine-item {
    padding: 1rem;
  }

  .routine-name {
    font-size: 1rem;
  }

  .routine-description {
    font-size: 0.85rem;
  }
}
</style>