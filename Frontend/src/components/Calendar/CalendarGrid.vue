<template>
  <div class="calendar-container mt-8">
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
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
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

/**
 * Obtiene la rutina asignada a un día específico
 * Compara las fechas de creación de las rutinas con el día del calendario
 */
const getRoutineForDay = (day: number): Routines | null => {
  const targetDate = new Date(
    props.currentDate.getFullYear(),
    props.currentDate.getMonth(),
    day
  );

  // Normalizar a medianoche para comparar solo fechas
  targetDate.setHours(0, 0, 0, 0);

  return props.routines.find(routine => {
    const routineDate = new Date(routine.createdat);
    routineDate.setHours(0, 0, 0, 0);
    
    return routineDate.getTime() === targetDate.getTime();
  }) || null;
};

/**
 * Verifica si un día es el día actual
 */
const isToday = (day: number): boolean => {
  const today = new Date();
  return (
    day === today.getDate() &&
    props.currentDate.getMonth() === today.getMonth() &&
    props.currentDate.getFullYear() === today.getFullYear()
  );
};

/**
 * Maneja la completación de una rutina
 */
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
  /* CRÍTICO: Forzar todas las filas a la misma altura */
  grid-auto-rows: 1fr;
}

/* Wrapper para mantener la proporción y altura consistente */
.day-item-wrapper {
  position: relative;
  width: 100%;
  /* Altura mínima fija para evitar descuadres */
  min-height: 140px;
  height: 100%;
}

/* Asegurar que DayCard ocupe todo el espacio disponible */
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
    padding: 0.5rem;
    border-radius: 12px;
  }

  .week-headers {
    gap: 0.25rem;
    margin-bottom: 0.75rem;
  }

  .week-day-header {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.25rem;
  }

  .week-day-circle {
    width: 32px;
    height: 32px;
    font-size: 0.85rem;
    margin: 0;
  }

  .week-day-full {
    font-size: 0.6rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .days-grid {
    gap: 0.3rem;
  }

  .day-item-wrapper {
    min-height: 95px;
  }

  .empty-day {
    min-height: 95px;
  }
}

@media (max-width: 360px) {
  .calendar-container {
    padding: 0.35rem;
    border-radius: 10px;
  }

  .week-day-circle {
    width: 28px;
    height: 28px;
    font-size: 0.75rem;
  }

  .week-day-full {
    font-size: 0.55rem;
  }

  .day-item-wrapper {
    min-height: 85px;
  }

  .empty-day {
    min-height: 85px;
  }
}
</style>