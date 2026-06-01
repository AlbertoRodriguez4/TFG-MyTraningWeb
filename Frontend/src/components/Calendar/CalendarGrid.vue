<template>
  <div class="calendar-container mt-8">
    <!-- Desktop: Grid View -->
    <template v-if="!isMobile">
      <!-- Week Headers -->
      <div class="week-headers">
        <div
          v-for="day in daysOfWeek"
          :key="day"
          class="week-day-header"
        >
          <div class="week-day-circle">
            <span>{{ day.charAt(0) }}</span>
          </div>
          <span class="week-day-full">{{ day }}</span>
        </div>
      </div>

      <!-- Days Grid -->
      <div class="days-grid">
        <!-- Empty days -->
        <div
          v-for="i in startingDayOfWeek"
          :key="'empty-' + i"
          class="day-item empty-day"
        ></div>

        <!-- Active days -->
        <div
          v-for="day in daysInMonth"
          :key="day"
          class="day-item-wrapper"
        >
          <DayCard
            :day="day"
            :routine="getRoutineForDay(day)"
            :is-today="isToday(day)"
            :current-date="currentDate"
            @click="$emit('day-click', day)"
            @complete="handleCompleteRoutine(day)"
          />
        </div>
      </div>
    </template>

    <!-- Mobile: List View -->
    <template v-else>
      <!-- Compact Week Strip -->
      <div class="mobile-week-strip">
        <div
          v-for="day in daysOfWeek"
          :key="'mob-' + day"
          class="mobile-week-day"
        >
          {{ day.charAt(0) }}
        </div>
      </div>

      <!-- Compact Day Numbers Row (mini calendar) -->
      <div class="mobile-mini-grid">
        <div
          v-for="i in startingDayOfWeek"
          :key="'mob-empty-' + i"
          class="mobile-mini-cell empty"
        ></div>
        <div
          v-for="day in daysInMonth"
          :key="'mob-day-' + day"
          :class="['mobile-mini-cell', { 'has-routine': getRoutineForDay(day), 'is-today': isToday(day), 'is-completed': getRoutineForDay(day)?.iscompleted, 'is-pending': getRoutineForDay(day) && !getRoutineForDay(day)?.iscompleted }]"
          @click="$emit('day-click', day)"
        >
          <span class="mini-day-num">{{ day }}</span>
          <span v-if="getRoutineForDay(day)" class="mini-dot" :class="getRoutineForDay(day)?.iscompleted ? 'dot-completed' : 'dot-pending'"></span>
        </div>
      </div>

      <!-- Day List: show days with routines + today -->
      <div class="mobile-day-list">
        <div v-if="mobileVisibleDays.length === 0" class="mobile-empty-state">
          <v-icon size="64" color="rgba(255,255,255,0.2)">mdi-calendar-blank</v-icon>
          <p class="mobile-empty-text">No hay rutinas este mes</p>
          <p class="mobile-empty-subtext">Toca un día en el calendario para crear una</p>
        </div>
        
        <div
          v-for="day in mobileVisibleDays"
          :key="'list-' + day"
          class="mobile-day-card"
          :class="{ 'card-completed': getRoutineForDay(day)?.iscompleted, 'card-pending': getRoutineForDay(day) && !getRoutineForDay(day)?.iscompleted, 'card-today': isToday(day) && !getRoutineForDay(day) }"
          @click="$emit('day-click', day)"
        >
          <!-- Left: Day Number -->
          <div class="mobile-card-day">
            <span class="card-day-number">{{ day }}</span>
            <span v-if="isToday(day)" class="card-today-label">HOY</span>
          </div>

          <!-- Center: Routine Info -->
          <div class="mobile-card-content">
            <template v-if="getRoutineForDay(day)">
              <div class="card-routine-header">
                <v-icon :color="getRoutineForDay(day)?.iscompleted ? '#34d399' : '#fbbf24'" size="24">
                  {{ getRoutineForDay(day)?.iscompleted ? 'mdi-check-circle' : 'mdi-dumbbell' }}
                </v-icon>
                <span class="card-routine-name">{{ getRoutineForDay(day)?.name }}</span>
              </div>
              <div class="card-routine-meta">
                <span class="card-meta-item">
                  <v-icon size="14" color="rgba(255,255,255,0.5)">mdi-star</v-icon>
                  +{{ getRoutineForDay(day)?.reward }} XP
                </span>
                <span v-if="getRoutineForDay(day)?.iscompleted" class="card-status-completed">
                  <v-icon size="14">mdi-check</v-icon>
                  Completada
                </span>
                <span v-else class="card-status-pending">
                  <v-icon size="14">mdi-clock-outline</v-icon>
                  Pendiente
                </span>
              </div>
            </template>
            <template v-else>
              <div class="card-empty-state">
                <v-icon size="20" color="rgba(255,255,255,0.3)">mdi-plus-circle-outline</v-icon>
                <span>Día libre - Toca para añadir rutina</span>
              </div>
            </template>
          </div>

          <!-- Right: Action Indicator -->
          <div class="mobile-card-action">
            <v-icon size="20" color="rgba(255,255,255,0.3)">mdi-chevron-right</v-icon>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue';
import DayCard from './DayCard.vue';
import type { Routines } from '../Models/Routines';


interface Props {
  daysOfWeek: string[];
  startingDayOfWeek: number;
  daysInMonth: number;
  currentDate: Date;
  routines: Routines[];
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'day-click': [day: number];
  'complete-routine': [day: number, routineId: number];
}>();

const windowWidth = ref(typeof window !== 'undefined' ? window.innerWidth : 1024);
const isMobile = computed(() => windowWidth.value < 960);

function onResize() {
  windowWidth.value = window.innerWidth;
}

onMounted(() => {
  window.addEventListener('resize', onResize);
});

onUnmounted(() => {
  window.removeEventListener('resize', onResize);
});

// Obtener la rutina para un día específico, si existe
const getRoutineForDay = (day: number): Routines | null => {
  const targetDate = new Date(
    props.currentDate.getFullYear(),
    props.currentDate.getMonth(),
    day
  );

  targetDate.setHours(0, 0, 0, 0);

  return props.routines.find(routine => {
    const routineDate = new Date(routine.createdat);
    routineDate.setHours(0, 0, 0, 0);
    
    return routineDate.getTime() === targetDate.getTime();
  }) || null;
};

// Comprobar que el día actual es el correcto para marcarlo como hoy
const isToday = (day: number): boolean => {
  const today = new Date();
  return (
    day === today.getDate() &&
    props.currentDate.getMonth() === today.getMonth() &&
    props.currentDate.getFullYear() === today.getFullYear()
  );
};

// Días visibles en la lista móvil: días con rutina + hoy (si no tiene rutina)
const mobileVisibleDays = computed(() => {
  const days: number[] = [];
  for (let day = 1; day <= props.daysInMonth; day++) {
    const routine = getRoutineForDay(day);
    if (routine || isToday(day)) {
      days.push(day);
    }
  }
  return days;
});

// Manejar la accion de completar una ruta 
const handleCompleteRoutine = (day: number) => {
  const routine = getRoutineForDay(day);
  if (routine) {
    emit('complete-routine', day, routine.id);
  }
};
</script>

<style scoped>
.calendar-container {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.9) 0%, rgba(15, 10, 26, 0.95) 100%);
  border-radius: 20px;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
  border: 2px solid rgba(139, 92, 246, 0.3);
  backdrop-filter: blur(10px);
  max-width: 100%;
  width: 100%;
}

.week-headers {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.week-day-header {
  text-align: center;
}

.week-day-circle {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: linear-gradient(135deg, #a78bfa 0%, #22d3ee 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
  font-size: 1.2rem;
  margin: 0 auto 0.5rem;
  box-shadow: 0 4px 20px rgba(167, 139, 250, 0.5);
}

.week-day-full {
  font-weight: 700;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.8);
  display: block;
}

.days-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1rem;
  grid-auto-rows: 1fr;
}

.day-item-wrapper {
  position: relative;
  width: 100%;
  min-height: 140px;
  height: 100%;
}

.day-item-wrapper >>> .day-card {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.empty-day {
  background: transparent;
  min-height: 140px;
}

/* ===== MOBILE STYLES ===== */

.mobile-week-strip {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 4px;
  margin-bottom: 12px;
}

.mobile-week-day {
  text-align: center;
  font-size: 0.7rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.5);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 4px 0;
}

.mobile-mini-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 4px;
  margin-bottom: 20px;
}

.mobile-mini-cell {
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.05);
  border: 1.5px solid transparent;
  position: relative;
  cursor: pointer;
  transition: all 0.2s ease;
}

.mobile-mini-cell.empty {
  background: transparent;
  cursor: default;
}

.mobile-mini-cell:active:not(.empty) {
  transform: scale(0.92);
}

.mobile-mini-cell.is-today {
  border-color: rgba(167, 139, 250, 0.8);
  background: rgba(167, 139, 250, 0.15);
}

.mobile-mini-cell.is-completed {
  background: rgba(52, 211, 153, 0.12);
  border-color: rgba(52, 211, 153, 0.4);
}

.mobile-mini-cell.is-pending {
  background: rgba(251, 191, 36, 0.1);
  border-color: rgba(251, 191, 36, 0.4);
}

.mini-day-num {
  font-size: 0.8rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.8);
}

.mobile-mini-cell.empty .mini-day-num {
  color: transparent;
}

.mobile-mini-cell.is-today .mini-day-num {
  color: #c4b5fd;
  font-weight: 900;
}

.mini-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  position: absolute;
  bottom: 4px;
}

.dot-completed {
  background: #34d399;
}

.dot-pending {
  background: #fbbf24;
}

/* Day List */
.mobile-day-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 20px;
}

.mobile-empty-state {
  text-align: center;
  padding: 3rem 1rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}

.mobile-empty-text {
  font-size: 1.1rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.6);
  margin: 0;
}

.mobile-empty-subtext {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.4);
  margin: 0;
}

.mobile-day-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px 20px;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.04);
  border: 1.5px solid rgba(139, 92, 246, 0.2);
  cursor: pointer;
  transition: all 0.25s ease;
}

.mobile-day-card:active {
  transform: scale(0.98);
  background: rgba(139, 92, 246, 0.12);
}

.mobile-day-card.card-today {
  border-color: rgba(167, 139, 250, 0.6);
  background: linear-gradient(135deg, rgba(167, 139, 250, 0.08) 0%, rgba(139, 92, 246, 0.05) 100%);
}

.mobile-day-card.card-completed {
  border-color: rgba(52, 211, 153, 0.4);
  background: linear-gradient(135deg, rgba(52, 211, 153, 0.08) 0%, rgba(16, 185, 129, 0.05) 100%);
}

.mobile-day-card.card-pending {
  border-color: rgba(251, 191, 36, 0.4);
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.08) 0%, rgba(245, 158, 11, 0.05) 100%);
}

.mobile-card-day {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-width: 56px;
  height: 56px;
  border-radius: 12px;
  background: rgba(139, 92, 246, 0.15);
  border: 1px solid rgba(139, 92, 246, 0.3);
}

.card-day-number {
  font-size: 1.5rem;
  font-weight: 900;
  color: #ffffff;
  line-height: 1;
}

.card-today-label {
  font-size: 0.6rem;
  font-weight: 800;
  color: #c4b5fd;
  letter-spacing: 1px;
  margin-top: 2px;
}

.mobile-card-content {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.card-routine-header {
  display: flex;
  align-items: center;
  gap: 10px;
}

.card-routine-name {
  font-size: 1rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.95);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-routine-meta {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.card-meta-item {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.6);
}

.card-status-completed {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.75rem;
  font-weight: 700;
  color: #34d399;
}

.card-status-pending {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.75rem;
  font-weight: 700;
  color: #fbbf24;
}

.card-empty-state {
  display: flex;
  align-items: center;
  gap: 10px;
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.9rem;
  font-weight: 600;
}

.mobile-card-action {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ===== DESKTOP RESPONSIVE ===== */

@media (max-width: 960px) {
  .calendar-container {
    padding: 1.25rem;
  }

  .week-day-circle {
    width: 40px;
    height: 40px;
    font-size: 1rem;
  }

  .week-day-full {
    font-size: 0.75rem;
  }

  .days-grid {
    gap: 0.5rem;
  }

  .day-item-wrapper {
    min-height: 120px;
  }

  .empty-day {
    min-height: 120px;
  }
}

@media (max-width: 600px) {
  .calendar-container {
    padding: 1rem;
    border-radius: 16px;
  }

  .mobile-mini-grid {
    gap: 3px;
    margin-bottom: 16px;
  }

  .mobile-mini-cell {
    border-radius: 8px;
  }

  .mini-day-num {
    font-size: 0.75rem;
  }

  .mobile-day-card {
    padding: 14px 16px;
    gap: 12px;
  }

  .mobile-card-day {
    min-width: 48px;
    height: 48px;
  }

  .card-day-number {
    font-size: 1.3rem;
  }

  .card-routine-name {
    font-size: 0.9rem;
  }
}

@media (max-width: 360px) {
  .calendar-container {
    padding: 0.75rem;
    border-radius: 12px;
  }

  .mobile-mini-cell {
    border-radius: 6px;
  }

  .mini-day-num {
    font-size: 0.7rem;
  }

  .mobile-day-card {
    padding: 12px;
    gap: 10px;
  }

  .mobile-card-day {
    min-width: 44px;
    height: 44px;
  }

  .card-day-number {
    font-size: 1.2rem;
  }

  .card-routine-name {
    font-size: 0.85rem;
  }

  .card-meta-item {
    font-size: 0.75rem;
  }
}
</style>