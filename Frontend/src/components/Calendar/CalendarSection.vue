<template>
  <section class="calendar-section">
    <v-container>
      <MonthNavigation
        :month-name="monthName"
        :year="currentDate.getFullYear()"
        @previous="$emit('previous-month')"
        @next="$emit('next-month')"
      />

      <CalendarGrid
        :days-of-week="daysOfWeek"
        :starting-day-of-week="startingDayOfWeek"
        :days-in-month="daysInMonth"
        :current-date="currentDate"
        :routines="routines"
        @day-click="$emit('day-click', $event)"
        @complete-routine="handleCompleteRoutine"
      />

      <WeeklySummary
        :completed-routines="completedRoutines"
        :userXP="userXP"
        :streak="streak"
        :xp-progress="xpProgress"
      />
    </v-container>
  </section>
</template>

<script setup lang="ts">
import MonthNavigation from './MonthNavigation.vue';
import CalendarGrid from './CalendarGrid.vue';
import WeeklySummary from './WeeklySummary.vue';
import type { Routines } from '../Models/Routines';



interface Props {
  currentDate: Date;
  monthName: string;
  daysOfWeek: string[];
  daysInMonth: number;
  startingDayOfWeek: number;
  routines: Routines[];
  completedRoutines: number;
  userXP: number;
  xpProgress: number;
  streak: number;
}

const props = defineProps<Props>();
// Emitir eventos para navegación y acciones del calendario
const emit = defineEmits<{
  'previous-month': [];
  'next-month': [];
  'day-click': [day: number];
  'complete-routine': [day: number, routineId: number];
}>();

// Obtener la rutina para un día específico, si existe
const handleCompleteRoutine = (day: number, routineId: number) => {
  emit('complete-routine', day, routineId);
};
</script>

<style scoped>
/* Sección del calendario */
.calendar-section {
  margin-top: -3rem; /* Se superpone ligeramente con el hero */
  padding: 0 0 4rem;
  position: relative;
  z-index: 2;
  background: transparent;
}

/* Responsive Design */
@media (max-width: 600px) {
  .calendar-section {
    margin-top: -2rem;
    padding: 0 0 3rem;
  }
}
</style>