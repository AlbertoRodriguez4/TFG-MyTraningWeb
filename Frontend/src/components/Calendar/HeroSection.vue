<template>
  <section class="hero-section">
    <div class="hero-overlay"></div>
    <div class="hero-particles"></div>
    <v-container class="hero-content">
      <v-row align="center" justify="space-between">
        <v-col cols="12" md="7">
          <div class="hero-text">
            <v-chip color="purple" dark class="mb-4 hero-badge elevation-4">
              <v-icon small left>mdi-fire</v-icon>
              {{ $t('hero.streakDays', { streak }) }}
            </v-chip>
            <h1 class="display-2 font-weight-black white--text mb-4 hero-title">
              {{ $t('hero.titleLine1') }}<br>
              <span class="gradient-text-hero">{{ $t('hero.titleLine2') }}</span>
            </h1>
            <p class="text-h6 white--text mb-6 hero-subtitle">
              {{ $t('hero.subtitle') }}
            </p>
            <div class="hero-actions">
              <v-btn
                x-large
                color="white"
                class="hero-btn-primary mr-3"
                elevation="8"
                @click="handleCreateRoutine"
              >
                <v-icon left color="purple">mdi-plus-circle</v-icon>
                <span class="purple--text font-weight-bold">{{ $t('calendar.createRoutineButton') }}</span>
              </v-btn>
              <v-btn
                x-large
                outlined
                dark
                class="hero-btn-secondary"
                @click="showStats = true"
              >
                <v-icon left>mdi-chart-timeline-variant</v-icon>
                {{ $t('hero.viewStats') }}
              </v-btn>
            </div>
          </div>
        </v-col>
        <v-col cols="12" md="5">
          <StatsCards
            :user-level="userLevel"
            :userXP="userXP"
            :xp-to-next-level="xpToNextLevel"
            :xp-progress="xpProgress"
            :coins="coins"
            :completed-routines="completedRoutines"
          />
        </v-col>
      </v-row>
    </v-container>

    <!-- Diálogo de descanso -->
    <v-dialog v-model="showRestDialog" max-width="500">
      <v-card class="rest-dialog">
        <v-card-title class="rest-dialog-title">
          <v-icon large color="white" class="mr-3">mdi-sleep</v-icon>
          <span>{{ $t('restDialog.title') }}</span>
        </v-card-title>
        <v-card-text class="rest-dialog-text">
          <p class="text-h6 mb-3">{{ $t('restDialog.alreadyTrained') }}</p>
          <p class="text-body-1">
            {{ $t('restDialog.restAdvice') }}
          </p>
          <p class="text-body-2 mt-3" style="color: #d8b4fe; font-weight: 600;">
            {{ $t('restDialog.comeBackTomorrow', { streak }) }}
          </p>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="#a78bfa"
            variant="elevated"
            large
            @click="showRestDialog = false"
          >
            {{ $t('common.gotIt') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Diálogo de Estadísticas -->
    <RoutineStats v-model="showStats" />
  </section>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoutineStore } from '@/stores/RoutineStore'
import StatsCards from './StatsCards.vue'
import RoutineStats from './RoutineStats.vue'

defineProps<{
  userLevel: number
  userXP: number
  xpToNextLevel: number
  coins: number
  completedRoutines: number
  streak: number
  xpProgress: number
}>()

const emit = defineEmits<{
  'create-routine': []
}>()

const routineStore = useRoutineStore()
const showRestDialog = ref(false)
const showStats = ref(false)

const hasRoutineToday = computed((): boolean => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return routineStore.routines.some(routine => {
    const routineDate = new Date(routine.createdat)
    routineDate.setHours(0, 0, 0, 0)
    return routineDate.getTime() === today.getTime()
  })
})

function handleCreateRoutine(): void {
  if (hasRoutineToday.value) {
    showRestDialog.value = true
  } else {
    emit('create-routine')
  }
}
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, #0a0a1a 0%, #1a0a2e 50%, #0f0a1a 100%);
  padding: 4rem 0 6rem;
  position: relative;
  overflow: hidden;
}

.hero-overlay {
  position: absolute;
  inset: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320"><path fill="rgba(139,92,246,0.08)" d="M0,96L48,112C96,128,192,160,288,160C384,160,480,128,576,122.7C672,117,768,139,864,144C960,149,1056,139,1152,122.7C1248,107,1344,85,1392,74.7L1440,64L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"></path></svg>');
  background-size: cover;
  background-position: bottom;
  opacity: 0.5;
}

.hero-particles {
  position: absolute;
  inset: 0;
  background-image:
    radial-gradient(circle at 20% 50%, rgba(139, 92, 246, 0.15) 1px, transparent 1px),
    radial-gradient(circle at 80% 80%, rgba(34, 211, 238, 0.12) 1px, transparent 1px),
    radial-gradient(circle at 40% 20%, rgba(167, 139, 250, 0.1) 1px, transparent 1px);
  background-size: 60px 60px, 100px 100px, 120px 120px;
  background-position: 0 0, 50px 70px, 150px 280px;
  animation: particleFloat 20s linear infinite;
}

@keyframes particleFloat {
  from {
    transform: translateY(0);
  }
  to {
    transform: translateY(-15px);
  }
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-title {
  text-shadow: 0 4px 30px rgba(0, 0, 0, 0.5);
  animation: fadeInUp 0.8s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.hero-badge {
  font-weight: 700;
  font-size: 0.95rem;
  border-radius: 50px;
  padding: 10px 20px;
  animation: pulse 2s ease-in-out infinite;
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.3), rgba(34, 211, 238, 0.3)) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 92, 246, 0.5) !important;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
    box-shadow: 0 0 20px rgba(139, 92, 246, 0.3);
  }
  50% {
    transform: scale(1.08);
    box-shadow: 0 0 30px rgba(139, 92, 246, 0.5);
  }
}

.gradient-text-hero {
  background: linear-gradient(135deg, #a78bfa 0%, #22d3ee 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: shimmer 3s linear infinite;
  filter: drop-shadow(0 0 20px rgba(167, 139, 250, 0.5));
}

@keyframes shimmer {
  to {
    background-position: 200% center;
  }
}

.hero-subtitle {
  opacity: 0.9;
  max-width: 600px;
  color: var(--text-muted);
  text-shadow: none;
  animation: fadeInUp 0.8s ease-out 0.2s both;
}

.hero-actions {
  display: flex;
  gap: 1rem;
  animation: fadeInUp 0.8s ease-out 0.4s both;
}

.hero-btn-primary {
  font-weight: 700;
  text-transform: none;
  letter-spacing: 0.5px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border-radius: 16px;
  background: linear-gradient(135deg, #8b5cf6, #22d3ee) !important;
  box-shadow: 0 8px 30px rgba(139, 92, 246, 0.4) !important;
}

.hero-btn-primary:hover {
  transform: translateY(-3px) scale(1.02);
  box-shadow: 0 15px 40px rgba(139, 92, 246, 0.6) !important;
}

.hero-btn-secondary {
  font-weight: 700;
  text-transform: none;
  border: 2px solid rgba(139, 92, 246, 0.4) !important;
  backdrop-filter: blur(10px);
  background: rgba(139, 92, 246, 0.1) !important;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border-radius: 16px;
  color: #a78bfa !important;
}

.hero-btn-secondary:hover {
  background: rgba(139, 92, 246, 0.2) !important;
  border-color: rgba(139, 92, 246, 0.7) !important;
  transform: translateY(-3px);
  box-shadow: 0 10px 30px rgba(139, 92, 246, 0.3);
}

.rest-dialog {
  border-radius: 16px;
  overflow: hidden;
  background-color: var(--card-bg);
}

.rest-dialog-title {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 24px;
  font-size: 1.5rem;
  font-weight: 700;
}

.rest-dialog-text {
  padding: 32px 24px;
  text-align: center;
  color: var(--text-primary);
}

.rest-dialog-text p {
  line-height: 1.6;
  color: var(--text-primary);
}

.rest-dialog-text .text-h6 {
  color: var(--text-primary);
  font-weight: 700;
}

@media (max-width: 960px) {
  .hero-section {
    padding: 3rem 0 4rem;
  }

  .display-2 {
    font-size: 2.5rem !important;
  }

  .hero-actions {
    flex-direction: column;
    gap: 1rem;
  }

  .hero-actions .v-btn {
    width: 100%;
    margin: 0 !important;
    min-height: 48px;
  }
}

@media (max-width: 600px) {
  .hero-section {
    padding: 1.5rem 0 2.5rem;
  }

  .display-2 {
    font-size: 1.75rem !important;
    line-height: 1.2;
  }

  .text-h6 {
    font-size: 0.9rem !important;
  }

  .hero-badge {
    font-size: 0.8rem;
    padding: 8px 16px;
  }

  .hero-actions {
    gap: 0.75rem;
  }

  .hero-actions .v-btn {
    min-height: 56px;
    font-size: 0.9rem !important;
  }

  .hero-btn-primary,
  .hero-btn-secondary {
    border-radius: 12px !important;
  }

  .rest-dialog-title {
    font-size: 1.1rem;
    padding: 16px;
  }

  .rest-dialog-text {
    padding: 20px 12px;
  }

  .rest-dialog-text .text-h6 {
    font-size: 1rem !important;
    color: var(--text-primary);
  }

  .rest-dialog-text .text-body-1 {
    font-size: 0.875rem !important;
  }
}

@media (max-width: 360px) {
  .hero-section {
    padding: 1rem 0 2rem;
  }

  .display-2 {
    font-size: 1.5rem !important;
  }

  .text-h6 {
    font-size: 0.85rem !important;
  }

  .hero-actions .v-btn {
    min-height: 52px;
    font-size: 0.85rem !important;
  }
}
</style>