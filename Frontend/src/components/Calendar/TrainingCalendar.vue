<template>
  <v-app>
    <v-main class="workout-hub">
      <!-- Hero Header Section -->
      <HeroSection
        :user-level="userLevel"
        :userXp="userXP"
        :xp-to-next-level="xpToNextLevel"
        :coins="coins"
        :completed-routines="completedRoutines"
        :streak="streak"
        :xp-progress="xpProgress"
        @create-routine="openCreateModal"
      />

      <!-- Calendar Section -->
      <CalendarSection
        :current-date="currentDate"
        :month-name="monthName"
        :days-of-week="daysOfWeek"
        :days-in-month="daysInMonth"
        :starting-day-of-week="startingDayOfWeek"
        :routines="routines"
        :completed-routines="completedRoutines"
        :userXP="userXP"
        :xp-progress="xpProgress"
        :streak="streak"
        @previous-month="previousMonth"
        @next-month="nextMonth"
        @day-click="handleDayClick"
        @complete-routine="completeRoutine"
      />

      <!-- Create Routine Dialog -->
      <CreateRoutineDialog
        v-model="showCreateModal"
        :selected-day="selectedDay"
        :month-name="monthName"
        :routine-name.sync="routineName"
        :routine-exercises.sync="routineExercises"
        @create="createRoutine"
        @close="closeModal"
      />

      <!-- Level Up Dialog -->
      <LevelUpDialog
        v-model="showLevelUp"
        :user-level="userLevel"
      />

      <!-- Success Snackbar -->
      <SuccessSnackbar v-model="showCompleted" />
    </v-main>
  </v-app>
</template>

<script>
import HeroSection from './components/HeroSection.vue';
import CalendarSection from './components/CalendarSection.vue';
import CreateRoutineDialog from './components/CreateRoutineDialog.vue';
import LevelUpDialog from './components/LevelUpDialog.vue';
import SuccessSnackbar from './components/SuccessSnackbar.vue';

export default {
  name: 'TrainingCalendar',
  components: {
    HeroSection,
    CalendarSection,
    CreateRoutineDialog,
    LevelUpDialog,
    SuccessSnackbar
  },
  data() {
    return {
      currentDate: new Date(2025, 9, 11),
      userXP: 3450,
      userLevel: 12,
      coins: 2850,
      streak: 7,
      showCreateModal: false,
      showLevelUp: false,
      showCompleted: false,
      selectedDay: null,
      routineName: '',
      routineExercises: '',
      xpToNextLevel: 4000,
      routines: {
        
      }
    };
  },
  computed: {
    monthNames() {
      return [
        this.$t('january'), this.$t('february'), this.$t('march'), this.$t('april'),
        this.$t('may'), this.$t('june'), this.$t('july'), this.$t('august'),
        this.$t('september'), this.$t('october'), this.$t('november'), this.$t('december')
      ];
    },
    daysOfWeek() {
      return [
        this.$t('sunday'), this.$t('monday'), this.$t('tuesday'), this.$t('wednesday'),
        this.$t('thursday'), this.$t('friday'), this.$t('saturday')
      ];
    },
    monthName() {
      return this.monthNames[this.currentDate.getMonth()];
    },
    daysInMonth() {
      return new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 0).getDate();
    },
    startingDayOfWeek() {
      return new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), 1).getDay();
    },
    xpProgress() {
      return (this.userXP / this.xpToNextLevel) * 100;
    },
    completedRoutines() {
      return Object.values(this.routines).filter(r => r.completed).length;
    }
  },
  methods: {
    previousMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
    },
    nextMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
    },
    getRoutineKey(day) {
      return `${this.currentDate.getFullYear()}-${this.currentDate.getMonth() + 1}-${day}`;
    },
    handleDayClick(day) {
      const routine = this.routines[this.getRoutineKey(day)];
      if (!routine) {
        this.selectedDay = day;
        this.showCreateModal = true;
      }
    },
    openCreateModal() {
      this.selectedDay = new Date().getDate();
      this.showCreateModal = true;
    },
    createRoutine() {
      if (this.routineName && this.routineExercises && this.selectedDay) {
        const key = this.getRoutineKey(this.selectedDay);
        this.$set(this.routines, key, {
          name: this.routineName,
          exercises: this.routineExercises,
          completed: false,
          xp: 150
        });
        this.closeModal();
      }
    },
    completeRoutine(day) {
      const key = this.getRoutineKey(day);
      const routine = this.routines[key];
      
      if (routine && !routine.completed) {
        routine.completed = true;
        
        const newXP = this.userXP + routine.xp;
        this.coins += 50;
        this.streak += 1;
        
        if (newXP >= this.xpToNextLevel) {
          this.userLevel += 1;
          this.userXP = newXP - this.xpToNextLevel;
          this.showLevelUp = true;
        } else {
          this.userXP = newXP;
        }
        
        this.showCompleted = true;
      }
    },
    closeModal() {
      this.showCreateModal = false;
      this.selectedDay = null;
      this.routineName = '';
      this.routineExercises = '';
    }
  }
};
</script>

<style scoped>
.workout-hub {
  background: #f8f9fa;
  min-height: 100vh;
}
</style>