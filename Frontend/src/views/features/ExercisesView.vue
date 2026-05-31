<template>
  <v-app>
    <v-main class="exercises-page">
      <v-container fluid class="pa-6">
        <!-- Header -->
        <div class="page-header mb-8">
          <div class="header-icon">
            <v-icon size="48" color="white">mdi-dumbbell</v-icon>
          </div>
          <div>
            <h1 class="text-h3 font-weight-bold text-white">{{ $t('exercises_title') }}</h1>
            <p class="text-subtitle-1 text-grey-light">{{ $t('exercises_subtitle') }}</p>
          </div>
        </div>

        <!-- Filtros -->
        <v-row class="mb-6">
          <v-col cols="12" md="6">
            <v-text-field
              v-model="searchQuery"
              :label="$t('search_exercise')"
              variant="outlined"
              color="primary"
              prepend-inner-icon="mdi-magnify"
              @update:model-value="handleSearch"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-select
              v-model="selectedMuscleGroup"
              :items="store.muscleGroups"
              :label="$t('muscle_group')"
              variant="outlined"
              color="primary"
              clearable
              @update:model-value="handleFilter"
            />
          </v-col>
        </v-row>

        <!-- Grid de ejercicios -->
        <v-row v-if="!store.loading">
          <v-col
            v-for="exercise in store.exercises"
            :key="exercise.id"
            cols="12" sm="6" md="4"
            class="d-flex"
          >
             <v-card class="exercise-card" elevation="8">
              <div class="exercise-image-wrapper">
                <v-img
                  :src="getImageUrl(exercise)"
                  height="200"
                  cover
                  class="exercise-image"
                >
                  <template #error>
                    <div class="image-fallback">
                      <div class="fallback-content">
                        <v-icon size="56" color="#ffffff">mdi-dumbbell</v-icon>
                        <span class="fallback-text">{{ $t('no_image') }}</span>
                      </div>
                    </div>
                  </template>
                  <template #placeholder>
                    <div class="image-fallback">
                      <div class="fallback-content">
                        <v-icon size="56" color="#ffffff">mdi-dumbbell</v-icon>
                        <span class="fallback-text">{{ $t('loading') }}</span>
                      </div>
                    </div>
                  </template>
                </v-img>
                <v-chip
                  class="difficulty-chip"
                  :class="'diff-' + exercise.difficulty"
                  variant="flat"
                  size="small"
                >
                  {{ $t(`difficulty_${exercise.difficulty}`) }}
                </v-chip>
              </div>
              <v-card-text class="pa-5 exercise-content">
                <h3 class="text-h6 font-weight-bold text-white mb-2 exercise-name">{{ exercise.name }}</h3>
                <p class="text-body-2 text-grey-light mb-3 exercise-desc">{{ exercise.description }}</p>

                <div class="exercise-tags">
                  <v-chip x-small color="primary" variant="flat" class="mr-2">
                    <v-icon x-small start>mdi-arm-flex</v-icon>
                    {{ $t(`muscle_${exercise.muscleGroup}`) }}
                  </v-chip>
                  <v-chip x-small color="info" variant="flat" class="mr-2" v-if="exercise.target">
                    <v-icon x-small start>mdi-bullseye</v-icon>
                    {{ exercise.target }}
                  </v-chip>
                  <v-chip x-small variant="flat" class="equipment-chip">
                    <v-icon x-small start>mdi-tools</v-icon>
                    {{ exercise.equipment }}
                  </v-chip>
                </div>

                <div class="secondary-muscles mt-2" v-if="exercise.secondaryMuscles && exercise.secondaryMuscles.length > 0">
                  <v-chip x-small color="secondary" variant="flat" class="mr-1" v-for="(muscle, idx) in exercise.secondaryMuscles" :key="idx">
                    {{ muscle }}
                  </v-chip>
                </div>

                <v-expansion-panels variant="accordion" class="mt-4 dark-panels">
                  <v-expansion-panel v-if="exercise.instructionSteps && exercise.instructionSteps.length > 0" class="dark-panel">
                    <v-expansion-panel-title class="dark-panel-title">{{ $t('instructions') }}</v-expansion-panel-title>
                    <v-expansion-panel-text class="dark-panel-text">
                      <ol class="instruction-list">
                        <li v-for="(step, idx) in exercise.instructionSteps" :key="idx">{{ step }}</li>
                      </ol>
                    </v-expansion-panel-text>
                  </v-expansion-panel>
                  <v-expansion-panel v-else-if="exercise.instructions" class="dark-panel">
                    <v-expansion-panel-title class="dark-panel-title">{{ $t('instructions') }}</v-expansion-panel-title>
                    <v-expansion-panel-text class="dark-panel-text">{{ exercise.instructions }}</v-expansion-panel-text>
                  </v-expansion-panel>
                  <v-expansion-panel v-if="exercise.commonMistakes" class="dark-panel">
                    <v-expansion-panel-title class="dark-panel-title">{{ $t('common_mistakes') }}</v-expansion-panel-title>
                    <v-expansion-panel-text class="dark-panel-text">{{ exercise.commonMistakes }}</v-expansion-panel-text>
                  </v-expansion-panel>
                </v-expansion-panels>

                <v-btn
                  v-if="exercise.videoUrl"
                  :href="exercise.videoUrl"
                  target="_blank"
                  color="error"
                  variant="text"
                  size="small"
                  class="mt-3 video-btn"
                  prepend-icon="mdi-play-circle"
                >
                  {{ $t('watch_video') }}
                </v-btn>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <v-skeleton-loader v-else type="card" :loading="true" />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useExerciseStore } from '@/stores/exerciseStore';

const store = useExerciseStore();
const searchQuery = ref('');
const selectedMuscleGroup = ref<string | null>(null);

onMounted(() => {
  store.fetchExercises();
});

function handleSearch() {
  if (searchQuery.value.length > 2) { // Solo buscar si el término tiene más de 2 caracteres para evitar búsquedas innecesarias
    store.searchExercises(searchQuery.value);
  } else if (searchQuery.value === '') {
    store.fetchExercises();
  }
}

function handleFilter() {
  if (selectedMuscleGroup.value) {
    store.filterByMuscleGroup(selectedMuscleGroup.value);
  } else {
    store.fetchExercises();
  }
}

function getDifficultyColor(difficulty: string) {
  const colors: Record<string, string> = {
    beginner: 'success',
    intermediate: 'warning',
    advanced: 'error'
  };
  return colors[difficulty] || 'grey';
}

function getImageUrl(exercise: any) {
  const url = exercise.gifUrl || exercise.imageUrl; // Priorizar gifUrl si está disponible, sino usar imageUrl para mostrar la imagen del ejercicio. 
  // Si ninguno está disponible, se mostrará el fallback definido en el template.
  if (!url) return '';
  // Solo usar URLs absolutas (http:// o https://) para evitar problemas con URLs relativas o datos corruptos en la base de datos
  if (typeof url === 'string' && (url.startsWith('http://') || url.startsWith('https://'))) {
    return url;
  }
  return '';
}
</script>

<style scoped>
.exercises-page {
  background: linear-gradient(135deg, #0a0a0f 0%, #1a1a2e 100%);
  min-height: 100vh;
}
.page-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}
.header-icon {
  width: 80px;
  height: 80px;
  border-radius: 20px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  display: flex;
  align-items: center;
  justify-content: center;
}
.exercise-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  overflow: hidden;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  height: 100%;
}
.exercise-card:hover {
  transform: translateY(-4px);
  border-color: rgba(102, 126, 234, 0.5);
}
.exercise-image-wrapper {
  position: relative;
  flex-shrink: 0;
}
.exercise-image {
  background: rgba(0, 0, 0, 0.3);
}
.image-fallback {
  width: 100%;
  height: 100%;
  min-height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 50%, #2563eb 100%);
}
.fallback-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}
.fallback-text {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 0.5px;
}
.difficulty-chip {
  position: absolute;
  top: 12px;
  right: 12px;
  font-weight: 700;
  color: #ffffff !important;
  text-shadow: 0 1px 3px rgba(0,0,0,0.5);
}
.diff-beginner {
  background: linear-gradient(135deg, #22c55e, #16a34a) !important;
}
.diff-intermediate {
  background: linear-gradient(135deg, #f59e0b, #d97706) !important;
}
.diff-advanced {
  background: linear-gradient(135deg, #ef4444, #dc2626) !important;
}
.exercise-content {
  display: flex;
  flex-direction: column;
  flex: 1;
}
.exercise-name {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.3;
  min-height: 2.6em;
}
.exercise-desc {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.4;
  min-height: 4.2em;
}
.exercise-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-bottom: auto;
}
.equipment-chip {
  background: rgba(255, 255, 255, 0.1) !important;
  color: rgba(255, 255, 255, 0.85) !important;
}
.video-btn {
  color: #f87171 !important;
}

.instruction-list {
  padding-left: 1.2rem;
  margin: 0;
}
.instruction-list li {
  margin-bottom: 0.5rem;
  line-height: 1.5;
}

/* Colores para fondo oscuro */
.text-white {
  color: #ffffff !important;
}
.text-grey-light {
  color: rgba(255, 255, 255, 0.75) !important;
}

/* Expansion panels oscuros */
:deep(.dark-panels) {
  background: transparent !important;
}
:deep(.dark-panel) {
  background: rgba(0, 0, 0, 0.25) !important;
  color: #ffffff !important;
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
  border-radius: 10px !important;
  margin-bottom: 0.5rem !important;
}
:deep(.dark-panel-title) {
  color: #ffffff !important;
  font-weight: 700 !important;
  font-size: 0.85rem !important;
}
:deep(.dark-panel-title .v-expansion-panel-title__icon) {
  color: rgba(255, 255, 255, 0.7) !important;
}
:deep(.dark-panel-text) {
  color: rgba(255, 255, 255, 0.8) !important;
  font-size: 0.9rem !important;
  line-height: 1.5 !important;
}

/* Inputs con texto claro */
:deep(.v-field__input) {
  color: #ffffff !important;
}
:deep(.v-field__outline) {
  --v-field-border-opacity: 0.35 !important;
}
:deep(.v-label) {
  color: rgba(255, 255, 255, 0.7) !important;
}
:deep(.v-field--focused .v-label) {
  color: rgba(255, 255, 255, 0.9) !important;
}
:deep(.v-select__selection-text) {
  color: #ffffff !important;
}
</style>
