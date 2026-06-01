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
        <div
          v-for="day in mobileVisibleDays"
          :key="'list-' + day"
          class="mobile-day-row"
          :class="{ 'row-completed': getRoutineForDay(day)?.iscompleted, 'row-pending': getRoutineForDay(day) && !getRoutineForDay(day)?.iscompleted, 'row-today': isToday(day) }"
          @click="$emit('day-click', day)"
        >
          <div class="mobile-day-number">
            <span class="day-big-num">{{ day }}</span>
            <span v-if="isToday(day)" class="mobile-today-badge">HOY</span>
          </div>

          <div class="mobile-day-content">
            <template v-if="getRoutineForDay(day)">
              <div class="mobile-routine-info">
                <v-icon :color="getRoutineForDay(day)?.iscompleted ? 'green' : 'orange'" size="20">
                  {{ getRoutineForDay(day)?.iscompleted ? 'mdi-check-circle' : 'mdi-dumbbell' }}
                </v-icon>
                <div class="mobile-routine-text">
                  <span class="mobile-routine-name">{{ getRoutineForDay(day)?.name }}</span>
                  <span class="mobile-routine-xp">+{{ getRoutineForDay(day)?.reward }} XP</span>
                </div>
              </div>
            </template>
            <template v-else>
              <div class="mobile-empty-day">
                <v-icon size="18" color="grey">mdi-plus-circle-outline</v-icon>
                <span>Sin rutina</span>
              </div>
            </template>
          </div>

          <div class="mobile-day-status">
            <v-chip v-if="getRoutineForDay(day)?.iscompleted" x-small color="success" dark>
              <v-icon x-small left>mdi-check</v-icon>
              Hecho
            </v-chip>
            <v-chip v-else-if="getRoutineForDay(day)" x-small color="warning" dark>
              <v-icon x-small left>mdi-clock-outline</v-icon>
              Pendiente
            </v-chip>
            <v-icon v-else size="20" color="grey">mdi-chevron-right</v-icon>
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
const isMobile = computed(() => windowWidth.value < 600);

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
  gap: 8px;
}

.mobile-day-row {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.04);
  border: 1.5px solid rgba(139, 92, 246, 0.2);
  cursor: pointer;
  transition: all 0.2s ease;
}

.mobile-day-row:active {
  transform: scale(0.98);
  background: rgba(139, 92, 246, 0.1);
}

.mobile-day-row.row-today {
  border-color: rgba(167, 139, 250, 0.6);
  background: rgba(167, 139, 250, 0.08);
}

.mobile-day-row.row-completed {
  border-color: rgba(52, 211, 153, 0.4);
  background: rgba(52, 211, 153, 0.06);
}

.mobile-day-row.row-pending {
  border-color: rgba(251, 191, 36, 0.4);
  background: rgba(251, 191, 36, 0.06);
}

.mobile-day-number {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 44px;
}

.day-big-num {
  font-size: 1.4rem;
  font-weight: 900;
  color: #ffffff;
  line-height: 1;
}

.mobile-today-badge {
  font-size: 0.55rem;
  font-weight: 800;
  color: #c4b5fd;
  letter-spacing: 1px;
  margin-top: 2px;
}

.mobile-day-content {
  flex: 1;
  min-width: 0;
}

.mobile-routine-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.mobile-routine-text {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.mobile-routine-name {
  font-size: 0.9rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.9);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.mobile-routine-xp {
  font-size: 0.75rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.5);
}

.mobile-empty-day {
  display: flex;
  align-items: center;
  gap: 8px;
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.85rem;
  font-weight: 600;
}

.mobile-day-status {
  flex-shrink: 0;
}

/* ===== DESKTOP RESPONSIVE ===== */

@media (max-width: 960px) {
  .calendar-container {
    padding: 1rem;
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
}

@media (max-width: 360px) {
  .calendar-container {
    padding: 0.75rem;
    border-radius: 12px;
  }

  .mobile-mini-cell {
    border-radius: 8px;
  }

  .mini-day-num {
    font-size: 0.7rem;
  }

  .mobile-day-row {
    padding: 12px;
    gap: 10px;
  }

  .day-big-num {
    font-size: 1.2rem;
  }

  .mobile-routine-name {
    font-size: 0.8rem;
  }
}
</style>