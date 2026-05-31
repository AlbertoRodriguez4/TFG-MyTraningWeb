<template>
  <v-card class="weekly-summary-card mt-8" elevation="4" rounded="xl">
    <v-card-text class="pa-6">
      <h3 class="text-h5 font-weight-bold mb-4 text-white">
  <v-icon color="purple" class="mr-2">mdi-calendar-week</v-icon>
  {{ $t('weeklySummary.title') }}
</h3>
      <v-row>
        <v-col cols="6" md="3">
          <div class="summary-item">
            <v-icon size="40" color="purple">mdi-calendar-check</v-icon>
            <div class="summary-value">{{ completedRoutines }}</div>
            <div class="summary-label">{{ $t('weeklySummary.trainings') }}</div>
          </div>
        </v-col>
        <v-col cols="6" md="3">
          <div class="summary-item">
            <v-icon size="40" color="amber">mdi-lightning-bolt</v-icon>
            <div class="summary-value">{{ userXP }}</div>
            <div class="summary-label">{{ $t('weeklySummary.xpEarned') }}</div>
          </div>
        </v-col>
        <v-col cols="6" md="3">
          <div class="summary-item">
            <v-icon size="40" color="orange">mdi-fire</v-icon>
            <div class="summary-value">{{ streak }}</div>
            <div class="summary-label">{{ $t('weeklySummary.streakDays') }}</div>
          </div>
        </v-col>
        <v-col cols="6" md="3">
          <div class="summary-item">
            <v-icon size="40" color="green">mdi-target</v-icon>
            <div class="summary-value">{{ Math.round(xpProgress) }}%</div>
            <div class="summary-label">{{ $t('weeklySummary.progress') }}</div>
          </div>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
defineProps<{
  completedRoutines: number
  userXP: number
  streak: number
  xpProgress: number
}>()
</script>

<style scoped>
.weekly-summary-card {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.9) 0%, rgba(15, 10, 26, 0.95) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  backdrop-filter: blur(10px);
}

.summary-item {
  text-align: center;
  padding: 1rem;
}

.summary-value {
  font-size: 2rem;
  font-weight: 900;
  color: #ffffff;
  margin: 0.5rem 0 0.25rem;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.3);
}

.summary-label {
  font-size: 0.9rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.7);
}

@media (max-width: 960px) {
  .summary-item {
    padding: 0.75rem;
  }

  .summary-value {
    font-size: 1.75rem;
  }

  .summary-label {
    font-size: 0.85rem;
  }
}

@media (max-width: 600px) {
  .weekly-summary-card {
    margin-top: 1.5rem;
  }
  
  .summary-item {
    padding: 0.75rem 0.5rem;
  }
  
  .summary-item .v-icon {
    font-size: 30px !important;
  }
  
  .summary-value {
    font-size: 1.5rem;
  }
  
  .summary-label {
    font-size: 0.75rem;
  }
}
</style>