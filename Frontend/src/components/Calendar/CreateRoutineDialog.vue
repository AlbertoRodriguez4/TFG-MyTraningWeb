<template>
  <!-- ✅ CORRECTO -->
  <v-dialog :value="value" @input="(val: boolean) => $emit('input', val)" max-width="1100" persistent
    transition="dialog-transition" scrollable>
    <v-card rounded="xl" class="create-dialog" elevation="24">
      <!-- Header épico con animación -->
      <div class="dialog-header">
        <div class="header-bg-animation"></div>
        <div class="header-content">
          <div class="header-icon-wrapper">
            <v-icon size="56" color="white" class="pulse-icon">mdi-dumbbell</v-icon>
            <div class="icon-glow"></div>
          </div>
          <div class="header-text">
            <div class="level-badge">
              <v-icon small color="white">mdi-star</v-icon>
              <span>{{ t('calendar.createRoutineTitle') }}</span>
            </div>
            <h2 class="text-h3 font-weight-black white--text mb-2 title-glow">
              {{ t('calendar.newEpicRoutine') }}
            </h2>
            <div class="d-flex align-center">
              <div class="date-chip">
                <v-icon small color="white" class="mr-2">mdi-calendar-star</v-icon>
                <span class="white--text font-weight-bold">{{ formatDate }}</span>
              </div>
            </div>
          </div>
        </div>
        <v-btn icon x-large dark @click="handleClose" class="close-btn">
          <v-icon size="32">mdi-close-thick</v-icon>
        </v-btn>
      </div>

      <!-- Contenido del formulario -->
      <v-card-text class="pa-8 dialog-body">
        <!-- Advertencia si la fecha es pasada -->
        <v-alert
          v-if="isPastDate"
          type="warning"
          colored-border
          border="start"
          elevation="2"
          class="mb-6"
        >
          <div class="d-flex align-center">
            <v-icon color="amber" class="mr-3">mdi-calendar-remove</v-icon>
            <div>
              <div class="font-weight-bold">{{ t('calendar.cannotCreatePast') }}</div>
              <div class="text-caption">{{ t('calendar.selectFutureDate') }}</div>
            </div>
          </div>
        </v-alert>

        <!-- Nombre de la rutina con efecto gaming -->
        <div class="form-section mb-8">
          <div class="section-header mb-4">
            <div class="section-icon-wrapper">
              <v-icon color="white" size="24">mdi-format-title</v-icon>
            </div>
            <div>
              <div class="section-title">{{ t('calendar.routineNameTitle') }}</div>
              <div class="section-subtitle">{{ t('calendar.routineNameSubtitle') }}</div>
            </div>
            <v-chip x-small color="error" class="ml-auto chip-required">
              <v-icon x-small left>mdi-alert-circle</v-icon>
              {{ t('common.required') }}
            </v-chip>
          </div>
          <div class="input-wrapper">
            <v-text-field v-model="localRoutineName" :placeholder="t('calendar.routineNamePlaceholder')" outlined
              dense color="purple" hide-details="auto" class="custom-textfield-epic">
              <template v-slot:prepend-inner>
                <div class="input-icon-wrapper">
                  <v-icon color="purple lighten-2">mdi-weight-lifter</v-icon>
                </div>
              </template>
              <template v-slot:append>
                <v-fade-transition>
                  <div v-if="localRoutineName" class="success-indicator">
                    <v-icon color="success">mdi-check-circle</v-icon>
                  </div>
                </v-fade-transition>
              </template>
            </v-text-field>
            <div class="char-counter" v-if="localRoutineName">
              {{ localRoutineName.length }}/50
            </div>
          </div>
        </div>

        <!-- NUEVO: Tipo de Entrenamiento -->
        <div class="form-section mb-8">
          <div class="section-header mb-4">
            <div class="section-icon-wrapper gradient-success">
              <v-icon color="white" size="24">mdi-target</v-icon>
            </div>
            <div>
              <div class="section-title">{{ t('calendar.trainingFocusTitle') }}</div>
              <div class="section-subtitle">{{ t('calendar.trainingFocusSubtitle') }}</div>
            </div>
            <v-chip x-small color="error" class="ml-auto chip-required">
              <v-icon x-small left>mdi-alert-circle</v-icon>
              {{ t('common.required') }}
            </v-chip>
          </div>
          <div class="training-type-selector">
            <div v-for="type in trainingTypes" :key="type.value" @click="trainingFocus = type.value"
              :class="['training-type-card', { 'active': trainingFocus === type.value }]"
              :style="{ '--type-color': type.color }">
              <div class="training-type-icon">
                <v-icon :color="trainingFocus === type.value ? 'white' : type.color" size="36">
                  {{ type.icon }}
                </v-icon>
              </div>
              <div class="training-type-label">{{ type.label }}</div>
              <div class="training-type-description">{{ type.description }}</div>
              <v-fade-transition>
                <div v-if="trainingFocus === type.value" class="check-indicator">
                  <v-icon color="white" size="20">mdi-check-circle</v-icon>
                </div>
              </v-fade-transition>
            </div>
          </div>
        </div>

        <!-- Ejercicios con diseño mejorado -->
        <div class="form-section mb-8">
          <div class="section-header mb-4">
            <div class="section-icon-wrapper gradient-secondary">
              <v-icon color="white" size="24">mdi-clipboard-list-outline</v-icon>
            </div>
            <div>
              <div class="section-title">{{ t('calendar.mainExercisesTitle') }}</div>
              <div class="section-subtitle">{{ t('calendar.mainExercisesSubtitle') }}</div>
            </div>
            <v-chip x-small color="error" class="ml-auto chip-required">
              <v-icon x-small left>mdi-alert-circle</v-icon>
              {{ t('common.required') }}
            </v-chip>
          </div>
          <div class="input-wrapper">
            <v-textarea v-model="localRoutineExercises"
              :placeholder="t('calendar.exercisesPlaceholder')"
              outlined color="purple" hide-details="auto" rows="5" class="custom-textfield-epic" auto-grow>
              <template v-slot:prepend-inner>
                <div class="input-icon-wrapper">
                  <v-icon color="purple lighten-2">mdi-format-list-checks</v-icon>
                </div>
              </template>
              <template v-slot:append>
                <v-fade-transition>
                  <div v-if="localRoutineExercises" class="success-indicator">
                    <v-icon color="success">mdi-check-circle</v-icon>
                  </div>
                </v-fade-transition>
              </template>
            </v-textarea>
          </div>
          <div class="tips-box mt-3">
            <v-icon small color="amber" class="mr-2">mdi-lightbulb-on</v-icon>
            <span class="text-caption font-weight-medium">
              {{ t('calendar.proTip') }}
            </span>
          </div>
        </div>

        <!-- Grid de opciones -->
        <v-row class="mb-8">
          <!-- Nivel de dificultad rediseñado -->
          <v-col cols="12" md="6">
            <div class="form-section">
              <div class="section-header mb-4">
                <div class="section-icon-wrapper gradient-warning">
                  <v-icon color="white" size="24">mdi-speedometer</v-icon>
                </div>
                <div>
                  <div class="section-title">{{ t('calendar.challengeLevelTitle') }}</div>
                  <div class="section-subtitle">{{ t('calendar.challengeLevelSubtitle') }}</div>
                </div>
              </div>
              <div class="difficulty-selector">
                <div v-for="level in difficultyLevels" :key="level.value" @click="selectedDifficulty = level.value"
                  :class="['difficulty-card', { 'active': selectedDifficulty === level.value }]"
                  :style="{ '--card-color': level.color }">
                  <div class="difficulty-icon">
                    <v-icon :color="selectedDifficulty === level.value ? 'white' : level.color" size="32">
                      {{ level.icon }}
                    </v-icon>
                  </div>
                  <div class="difficulty-label">{{ level.label }}</div>
                  <div class="difficulty-xp">+{{ level.xpBonus }} XP</div>
                </div>
              </div>
            </div>
          </v-col>

          <!-- Duración con visualización mejorada -->
          <v-col cols="12" md="6">
            <div class="form-section">
              <div class="section-header mb-4">
                <div class="section-icon-wrapper gradient-info">
                  <v-icon color="white" size="24">mdi-clock-fast</v-icon>
                </div>
                <div>
                  <div class="section-title">{{ t('calendar.estimatedDurationTitle') }}</div>
                  <div class="section-subtitle">{{ t('calendar.estimatedDurationSubtitle') }}</div>
                </div>
              </div>
              <div class="duration-display">
                <div class="duration-value">{{ estimatedDuration }}</div>
                <div class="duration-unit">{{ t('common.minutes') }}</div>
              </div>
              <v-slider v-model="estimatedDuration" :min="15" :max="120" :step="15" color="purple"
                track-color="grey lighten-3" class="custom-slider-epic mt-4" hide-details>
                <template v-slot:prepend>
                  <div class="slider-label">15m</div>
                </template>
                <template v-slot:append>
                  <div class="slider-label">120m</div>
                </template>
              </v-slider>
              <div class="duration-bonus mt-2">
                <v-icon small color="success">mdi-plus-circle</v-icon>
                <span>{{ t('calendar.durationBonus', { bonus: durationBonus }) }}</span>
              </div>
            </div>
          </v-col>
        </v-row>

        <!-- Recompensas ÉPICAS - DINÁMICAS -->
        <div class="rewards-epic-section">
          <div class="rewards-epic-header">
            <div class="rewards-title-wrapper">
              <div class="rewards-icon-mega">
                <v-icon size="48" color="white">mdi-trophy-variant</v-icon>
              </div>
              <div>
                <h3 class="text-h4 font-weight-black mb-1">
                  {{ t('calendar.epicRewardsTitle') }}
                </h3>
                <p class="rewards-subtitle mb-0">
                  {{ t('calendar.epicRewardsSubtitle') }}
                </p>
              </div>
            </div>
          </div>

          <v-row class="rewards-grid">
            <!-- XP Card -->
            <v-col cols="12" sm="6" :md="rewardColumnSize">
              <div class="reward-card-epic reward-xp">
                <div class="reward-glow"></div>
                <div class="reward-icon-container">
                  <v-icon size="48" color="white">mdi-star-circle</v-icon>
                </div>
                <div class="reward-amount">{{ calculatedXP }}</div>
                <div class="reward-label">{{ t('calendar.rewardXP') }}</div>
                <div class="reward-progress">
                  <v-progress-linear :value="75" height="6" color="white" background-color="rgba(255,255,255,0.2)"
                    rounded></v-progress-linear>
                  <div class="progress-text">Nivel +{{ calculatedRewards.level }}</div>
                </div>
              </div>
            </v-col>

            <!-- Monedas Card -->
            <v-col cols="12" sm="6" :md="rewardColumnSize">
              <div class="reward-card-epic reward-coins">
                <div class="reward-glow"></div>
                <div class="reward-icon-container">
                  <v-icon size="48" color="white">mdi-cash-multiple</v-icon>
                </div>
                <div class="reward-amount">{{ calculatedRewards.gold }}</div>
                <div class="reward-label">{{ t('calendar.rewardCoins') }}</div>
                <div class="reward-info">
                  <v-icon x-small color="white">mdi-store</v-icon>
                  <span>{{ t('calendar.useInShop') }}</span>
                </div>
              </div>
            </v-col>

            <!-- Fuerza Card (solo si aplica) -->
            <v-col v-if="calculatedRewards.strength > 0" cols="12" sm="6" :md="rewardColumnSize">
              <div class="reward-card-epic reward-strength">
                <div class="reward-glow"></div>
                <div class="reward-icon-container">
                  <v-icon size="48" color="white">mdi-arm-flex</v-icon>
                </div>
                <div class="reward-amount">+{{ calculatedRewards.strength }}</div>
                <div class="reward-label">{{ t('calendar.rewardStrength') }}</div>
                <div class="reward-info">
                  <v-icon x-small color="white">mdi-trending-up</v-icon>
                  <span>{{ t('calendar.improvePower') }}</span>
                </div>
              </div>
            </v-col>

            <!-- Resistencia Card (solo si aplica) -->
            <v-col v-if="calculatedRewards.endurance > 0" cols="12" sm="6" :md="rewardColumnSize">
              <div class="reward-card-epic reward-endurance">
                <div class="reward-glow"></div>
                <div class="reward-icon-container">
                  <v-icon size="48" color="white">mdi-run-fast</v-icon>
                </div>
                <div class="reward-amount">+{{ calculatedRewards.endurance }}</div>
                <div class="reward-label">{{ t('calendar.rewardEndurance') }}</div>
                <div class="reward-info">
                  <v-icon x-small color="white">mdi-lightning-bolt</v-icon>
                  <span>{{ t('calendar.increaseEnergy') }}</span>
                </div>
              </div>
            </v-col>

            <!-- Racha Card -->
            <v-col cols="12" sm="6" :md="rewardColumnSize">
              <div class="reward-card-epic reward-streak">
                <div class="reward-glow"></div>
                <div class="reward-icon-container">
                  <v-icon size="48" color="white">mdi-fire</v-icon>
                </div>
                <div class="reward-amount">+1</div>
                <div class="reward-label">{{ t('calendar.rewardStreak') }}</div>
                <div class="reward-info">
                  <v-icon x-small color="white">mdi-trending-up</v-icon>
                  <span>{{ t('calendar.keepMomentum') }}</span>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Barra de logros adicional -->
          <div class="achievements-preview">
            <div class="achievement-item">
              <v-icon color="amber">mdi-medal</v-icon>
              <span>{{ t('calendar.achievementFirstRoutine') }}</span>
            </div>
            <div class="achievement-item">
              <v-icon color="purple">mdi-crown</v-icon>
              <span>{{ t('calendar.achievementPremiumRooms') }}</span>
            </div>
            <div class="achievement-item">
              <v-icon color="blue">mdi-account-group</v-icon>
              <span>{{ t('calendar.achievementCommunityPoints') }}</span>
            </div>
          </div>
        </div>
      </v-card-text>

      <!-- Footer épico -->
      <v-divider></v-divider>
      <v-card-actions class="pa-6 dialog-footer">
        <div class="footer-stats">
          <div class="stat-item">
            <v-icon small color="grey darken-1">mdi-calendar-check</v-icon>
            <span>{{ t('calendar.footerDate', { date: formatDate }) }}</span>
          </div>
          <div class="stat-item">
            <v-icon small color="grey darken-1">mdi-account</v-icon>
            <span>{{ t('calendar.footerPersonal') }}</span>
          </div>
        </div>
        <v-spacer></v-spacer>
        <v-btn x-large text @click="handleClose" class="px-8 cancel-btn">
          <v-icon left>mdi-close-circle-outline</v-icon>
          {{ t('common.cancel') }}
        </v-btn>
        <v-btn x-large color="purple" dark depressed @click="handleCreate" :disabled="!isFormValid" :loading="creating"
          class="px-10 ml-4 create-btn-epic" elevation="8">
          <v-icon left size="28">mdi-rocket-launch</v-icon>
          <span class="button-text">
            <span class="main-text">{{ t('calendar.createRoutineButton') }}</span>
            <span class="sub-text">+{{ calculatedXP }} XP</span>
          </span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import type { Routines, DifficultyLevel, TrainingType } from '../Models/Routines';

const { t } = useI18n();

interface Props {
  value?: boolean;
  selectedDay?: number | null;
  selectedDate?: Date | null;
  monthName: string;
  routineName?: string;
  routineExercises?: string;
  userId: number;
}

const props = withDefaults(defineProps<Props>(), {
  value: false,
  selectedDay: null,
  selectedDate: null,
  routineName: '',
  routineExercises: ''
});

interface Emits {
  (e: 'input', value: boolean): void;
  (e: 'close'): void;
  (e: 'create', routine: Routines): void;
}

const emit = defineEmits<Emits>();

// Data
const creating = ref(false);
const selectedDifficulty = ref(1);
const estimatedDuration = ref(60);
const localRoutineName = ref('');
const localRoutineExercises = ref('');
const trainingFocus = ref('strength'); // NUEVO
// Miveles de dificultad con bonificaciones de XP (cuanto más difícil, más XP)
const difficultyLevels = computed<DifficultyLevel[]>(() => [
  {
    value: 0,
    label: t('difficulty.beginner'),
    icon: 'mdi-leaf',
    color: '#4caf50',
    xpBonus: 0
  },
  {
    value: 1,
    label: t('difficulty.intermediate'),
    icon: 'mdi-lightning-bolt',
    color: '#ff9800',
    xpBonus: 50
  },
  {
    value: 2,
    label: t('difficulty.advanced'),
    icon: 'mdi-fire',
    color: '#f44336',
    xpBonus: 100
  }
]);

// Tipos de entrenamiento con descripciones
const trainingTypes = computed<TrainingType[]>(() => [
  {
    value: 'strength',
    label: t('calendar.strength'),
    icon: 'mdi-dumbbell',
    color: '#e74c3c',
    description: t('calendar.strengthDesc')
  },
  {
    value: 'endurance',
    label: t('calendar.endurance'),
    icon: 'mdi-run-fast',
    color: '#3498db',
    description: t('calendar.enduranceDesc')
  },
  {
    value: 'ambas',
    label: t('calendar.hybrid'),
    icon: 'mdi-star-four-points',
    color: '#9b59b6',
    description: t('calendar.hybridDesc')
  }
]);

// Devuelve un string formateado con el día y el mes para mostrar en el header del diálogo
const formatDate = computed(() => {
  return `${props.selectedDay} ${t('common.of')} ${props.monthName}`;
});
// Comprobar si la fecha seleccionada es pasada para mostrar una advertencia y deshabilitar la creación de la rutina
const isPastDate = computed(() => {
  if (!props.selectedDate) return false;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const selected = new Date(props.selectedDate);
  selected.setHours(0, 0, 0, 0);
  return selected.getTime() < today.getTime();
});
// El formulario es válido si el nombre, los ejercicios, el enfoque de entrenamiento están completos y la fecha no es pasada
const isFormValid = computed(() => {
  return localRoutineName.value && localRoutineExercises.value && trainingFocus.value && !isPastDate.value;
});
// Calcular bonificación de XP basada en la duración estimada (cuanto más larga, más XP, con un máximo de +40 XP a 120 minutos)
const durationBonus = computed(() => {
  return Math.floor(estimatedDuration.value / 15) * 10;
});
// Calcular XP total basado en la dificultad y la duración
const calculatedXP = computed(() => {
  const baseXP = 100;
  const difficultyBonus = difficultyLevels.value[selectedDifficulty.value]?.xpBonus ?? 0;
  return baseXP + difficultyBonus + durationBonus.value;
});

// Calcular recompensas adicionales basadas en el enfoque de entrenamiento y el XP total
const calculatedRewards = computed(() => {
  const rewards = {
    strength: 0,
    endurance: 0,
    level: 0,
    gold: calculatedXP.value,
    streak: 1
  };

  if (trainingFocus.value === 'strength') { // Si el enfoque es fuerza, otorga más puntos de fuerza
    rewards.strength = Math.floor(calculatedXP.value / 10);
  } else if (trainingFocus.value === 'endurance') {
    rewards.endurance = Math.floor(calculatedXP.value / 10); // Si el enfoque es resistencia, otorga más puntos de resistencia
  } else if (trainingFocus.value === 'ambas') {
    rewards.strength = Math.floor(calculatedXP.value / 20); // Si el enfoque es híbrido, otorga puntos de fuerza y resistencia pero en menor cantidad
    rewards.endurance = Math.floor(calculatedXP.value / 20); // Si el enfoque es híbrido, otorga puntos de fuerza y resistencia pero en menor cantidad
  }

  rewards.level = Math.floor(calculatedXP.value / 100);

  return rewards;
});

// Calcular el tamaño de las columnas para las recompensas dinámicamente según cuántas recompensas se muestren (XP, Monedas, Fuerza, Resistencia, Racha)
const rewardColumnSize = computed(() => {
  const visibleRewards = 3 + // XP, Monedas, Racha (siempre visibles)
    (calculatedRewards.value.strength > 0 ? 1 : 0) + // Fuerza (solo si se otorgan puntos de fuerza)
    (calculatedRewards.value.endurance > 0 ? 1 : 0); // Resistencia (solo si se otorgan puntos de resistencia)

  // Si hay 4 recompensas, usar md="3", si hay 5, usar md="2.4" (aproximado a 3)
  return visibleRewards === 5 ? 3 : 3;
});

// Sincronizar los props con los datos locales cuando se abre el diálogo para editar una rutina existente, esto espera a cuando el diálogo se abra (props.value cambie a true) 
// y solo actualiza los campos si están vacíos para no sobrescribir lo que el usuario ya haya escrito
watch(() => props.value, (newVal) => {
  if (newVal && !localRoutineName.value && !localRoutineExercises.value) {
    localRoutineName.value = props.routineName || '';
    localRoutineExercises.value = props.routineExercises || '';
  }
});

// Cerrar diálogo y emitir evento de cierre para que el padre pueda manejarlo (por ejemplo, para limpiar campos o actualizar la vista)
const handleClose = () => {
  emit('input', false);
  emit('close');
};
// Crear la rutina, primero verifica que el formulario sea válido, luego simula un proceso de creación (con un pequeño delay para mostrar el estado de carga) y 
// finalmente emite el evento 'create' con el nuevo objeto de rutina que cumple con la interfaz Routines, incluyendo la fecha seleccionada o la fecha actual si no se 
// seleccionó ninguna fecha. Después de crear la rutina, resetea los campos del formulario para que estén listos para una nueva creación.
const handleCreate = async () => {
  if (!isFormValid.value) return;

  creating.value = true;

  await new Promise(resolve => setTimeout(resolve, 500));

  // Crear objeto que cumple con la interfaz Routines
  // Usar la fecha seleccionada en el calendario, u hoy si no hay fecha
  const routineDate = props.selectedDate ? new Date(props.selectedDate) : new Date();
  const newRoutine: Routines = {
    id: 0,
    name: localRoutineName.value,
    description: localRoutineExercises.value,
    difficulty: selectedDifficulty.value,
    reward: calculatedXP.value,
    trainingfocus: trainingFocus.value, // NUEVO CAMPO
    iscompleted: false,
    createdat: routineDate,
    userId: props.userId
  };

  emit('create', newRoutine);

  creating.value = false;
  localRoutineName.value = '';
  localRoutineExercises.value = '';
  trainingFocus.value = 'strength'; // Reset
};
</script>

<style scoped>
.create-dialog {
  max-height: 95vh;
  overflow: hidden;
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(139, 92, 246, 0.3);
}

/* ===== HEADER ÉPICO - Tema Oscuro ===== */
.dialog-header {
  background: linear-gradient(135deg, rgba(10, 10, 26, 0.95) 0%, rgba(26, 10, 46, 0.9) 50%, rgba(15, 10, 26, 0.95) 100%);
  padding: 2.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  position: relative;
  overflow: hidden;
  border-bottom: 2px solid rgba(139, 92, 246, 0.3);
}

.header-bg-animation {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.1) 0%, transparent 50%),
    radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.08) 0%, transparent 50%);
  animation: headerPulse 4s ease-in-out infinite;
}

@keyframes headerPulse {

  0%,
  100% {
    opacity: 0.5;
  }

  50% {
    opacity: 1;
  }
}

.header-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  position: relative;
  z-index: 1;
}

.header-icon-wrapper {
  width: 90px;
  height: 90px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(20px);
  border: 3px solid rgba(255, 255, 255, 0.3);
  position: relative;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.icon-glow {
  position: absolute;
  inset: -10px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.4) 0%, transparent 70%);
  border-radius: 28px;
  animation: glowPulse 2s ease-in-out infinite;
}

@keyframes glowPulse {

  0%,
  100% {
    opacity: 0.5;
    transform: scale(1);
  }

  50% {
    opacity: 1;
    transform: scale(1.1);
  }
}

.pulse-icon {
  animation: iconBounce 2s ease-in-out infinite;
}

@keyframes iconBounce {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-5px);
  }
}

.level-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 0.4rem 1rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 1px;
  color: white;
  margin-bottom: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.title-glow {
  text-shadow: 0 0 20px rgba(255, 255, 255, 0.5), 0 4px 12px rgba(0, 0, 0, 0.3);
  animation: titleFloat 3s ease-in-out infinite;
}

@keyframes titleFloat {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-3px);
  }
}

.date-chip {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 0.5rem 1rem;
  border-radius: 12px;
  display: flex;
  align-items: center;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.close-btn {
  position: relative;
  z-index: 2;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  background: rgba(255, 255, 255, 0.1) !important;
  backdrop-filter: blur(10px);
}

.close-btn:hover {
  transform: rotate(90deg) scale(1.15);
  background: rgba(255, 255, 255, 0.25) !important;
}

/* ===== BODY - Tema Oscuro ===== */
.dialog-body {
  max-height: calc(95vh - 320px);
  overflow-y: auto;
  background: linear-gradient(to bottom, rgba(26, 10, 46, 0.5) 0%, rgba(15, 10, 26, 0.5) 100%);
}

/* ===== SECCIONES ===== */
.form-section {
  animation: slideInUp 0.6s cubic-bezier(0.4, 0, 0.2, 1) backwards;
}

.form-section:nth-child(1) {
  animation-delay: 0.1s;
}

.form-section:nth-child(2) {
  animation-delay: 0.15s;
}

.form-section:nth-child(3) {
  animation-delay: 0.2s;
}

.form-section:nth-child(4) {
  animation-delay: 0.25s;
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.section-icon-wrapper {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #a78bfa, #22d3ee);
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 16px rgba(167, 139, 250, 0.4);
  flex-shrink: 0;
}

.gradient-secondary {
  background: linear-gradient(135deg, #f472b6, #fb7185);
}

.gradient-warning {
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
}

.gradient-info {
  background: linear-gradient(135deg, #a78bfa, #22d3ee);
}

.section-title {
  font-size: 1.25rem;
  font-weight: 800;
  color: #ffffff;
  line-height: 1.2;
}

.section-subtitle {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 500;
}

.chip-required {
  font-weight: 700;
  height: 24px;
}

/* ===== INPUTS ÉPICOS ===== */
.input-wrapper {
  position: relative;
}

.custom-textfield-epic>>>.v-input__slot {
  background: rgba(15, 10, 26, 0.6) !important;
  border-radius: 16px !important;
  border: 2px solid rgba(139, 92, 246, 0.4) !important;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  padding: 0.5rem 1rem !important;
}

.custom-textfield-epic>>>.v-input__slot:hover {
  border-color: rgba(139, 92, 246, 0.7) !important;
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(139, 92, 246, 0.2);
}

.custom-textfield-epic.v-input--is-focused>>>.v-input__slot {
  border-color: rgba(167, 139, 250, 0.9) !important;
  background: rgba(15, 10, 26, 0.8) !important;
  box-shadow: 0 0 0 4px rgba(139, 92, 246, 0.15), 0 8px 24px rgba(139, 92, 246, 0.25);
  transform: translateY(-2px);
}

.custom-textfield-epic>>>input,
.custom-textfield-epic>>>textarea {
  font-weight: 600;
  font-size: 1.05rem;
  color: #ffffff;
}

.custom-textfield-epic>>>label {
  color: rgba(255, 255, 255, 0.7);
}

.custom-textfield-epic>>>input::placeholder,
.custom-textfield-epic>>>textarea::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.input-icon-wrapper {
  background: rgba(139, 92, 246, 0.15);
  border-radius: 10px;
  padding: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}

.success-indicator {
  animation: successPop 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes successPop {
  0% {
    transform: scale(0);
  }

  50% {
    transform: scale(1.2);
  }

  100% {
    transform: scale(1);
  }
}

.char-counter {
  position: absolute;
  bottom: -20px;
  right: 8px;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  font-weight: 600;
}

.tips-box {
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.15) 0%, rgba(245, 158, 11, 0.1) 100%);
  border-left: 3px solid rgba(251, 191, 36, 0.6);
  padding: 0.75rem 1rem;
  border-radius: 12px;
  display: flex;
  align-items: center;
  color: rgba(255, 255, 255, 0.9);
}

/* ===== SELECTOR DE DIFICULTAD - Tema Oscuro ===== */
.difficulty-selector {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.difficulty-card {
  background: rgba(26, 10, 46, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 20px;
  padding: 1.5rem 1rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

.difficulty-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--card-color), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.difficulty-card:hover {
  transform: translateY(-8px) scale(1.05);
  box-shadow: 0 12px 32px rgba(139, 92, 246, 0.3);
  border-color: rgba(139, 92, 246, 0.6);
}

.difficulty-card.active {
  border-color: var(--card-color);
  background: rgba(26, 10, 46, 0.9);
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
}

.difficulty-card.active::before {
  opacity: 1;
}

.difficulty-card.active .difficulty-label,
.difficulty-card.active .difficulty-xp {
  color: white;
}

.difficulty-icon {
  width: 64px;
  height: 64px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  position: relative;
  z-index: 1;
}

.difficulty-card.active .difficulty-icon {
  background: rgba(255, 255, 255, 0.15);
}

.difficulty-label {
  font-size: 1rem;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 0.5rem;
  position: relative;
  z-index: 1;
}

.difficulty-xp {
  font-size: 0.875rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.6);
  position: relative;
  z-index: 1;
}

/* ===== DURACIÓN - Tema Oscuro ===== */
.duration-display {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.3) 0%, rgba(167, 139, 250, 0.2) 100%);
  border-radius: 20px;
  padding: 1.5rem;
  text-align: center;
  position: relative;
  overflow: hidden;
  border: 2px solid rgba(139, 92, 246, 0.4);
}

.duration-display::before {
  content: '';
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at 30% 50%, rgba(167, 139, 250, 0.15) 0%, transparent 60%);
}

.duration-value {
  font-size: 3.5rem;
  font-weight: 900;
  color: white;
  line-height: 1;
  text-shadow: 0 4px 12px rgba(139, 92, 246, 0.5);
  position: relative;
  z-index: 1;
}

.duration-unit {
  font-size: 1.1rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.8);
  text-transform: uppercase;
  letter-spacing: 1px;
  position: relative;
  z-index: 1;
}

.custom-slider-epic>>>.v-slider__thumb {
  width: 28px;
  height: 28px;
  background: linear-gradient(135deg, #a78bfa, #22d3ee);
  border: 4px solid white;
  box-shadow: 0 4px 12px rgba(167, 139, 250, 0.5);
}

.custom-slider-epic>>>.v-slider__track-fill {
  background: linear-gradient(90deg, #a78bfa, #22d3ee);
}

.custom-slider-epic>>>.v-slider__track {
  background: rgba(255, 255, 255, 0.1);
}

.slider-label {
  font-size: 0.875rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.7);
}

.duration-bonus {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 700;
  color: rgba(52, 211, 153, 0.9);
}

/* ===== RECOMPENSAS ÉPICAS ===== */
.rewards-epic-section {
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  border-radius: 28px;
  padding: 2.5rem;
  position: relative;
  overflow: hidden;
  animation: slideInUp 0.6s cubic-bezier(0.4, 0, 0.2, 1) 0.4s backwards;
}

.rewards-epic-section::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(102, 126, 234, 0.1) 0%, transparent 70%);
  animation: rotateGradient 20s linear infinite;
}

@keyframes rotateGradient {
  from {
    transform: rotate(0deg);
  }

  to {
    transform: rotate(360deg);
  }
}

.rewards-epic-header {
  position: relative;
  z-index: 1;
  margin-bottom: 2rem;
}

.rewards-title-wrapper {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.rewards-icon-mega {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 32px rgba(255, 215, 0, 0.4);
  animation: trophyFloat 3s ease-in-out infinite;
}

@keyframes trophyFloat {

  0%,
  100% {
    transform: translateY(0) rotate(0deg);
  }

  50% {
    transform: translateY(-10px) rotate(5deg);
  }
}

.rewards-epic-header h3 {
  color: white;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.rewards-subtitle {
  color: rgba(255, 255, 255, 0.8);
  font-size: 1rem;
  font-weight: 600;
}

.rewards-grid {
  position: relative;
  z-index: 1;
}

.reward-card-epic {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 24px;
  padding: 2rem 1.5rem;
  text-align: center;
  position: relative;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
}

.reward-card-epic::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1) 0%, transparent 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.reward-card-epic:hover {
  transform: translateY(-12px) scale(1.05);
  border-color: rgba(255, 255, 255, 0.3);
}

.reward-card-epic:hover::before {
  opacity: 1;
}

.reward-glow {
  position: absolute;
  inset: -20px;
  background: radial-gradient(circle, var(--glow-color) 0%, transparent 70%);
  opacity: 0.3;
  animation: glowPulse 2s ease-in-out infinite;
}

.reward-xp {
  --glow-color: rgba(102, 126, 234, 0.6);
}

.reward-xp .reward-icon-container {
  background: linear-gradient(135deg, #667eea, #764ba2);
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.5);
}

.reward-coins {
  --glow-color: rgba(255, 193, 7, 0.6);
}

.reward-coins .reward-icon-container {
  background: linear-gradient(135deg, #ffc107, #ff9800);
  box-shadow: 0 8px 32px rgba(255, 193, 7, 0.5);
}

.reward-streak {
  --glow-color: rgba(255, 87, 34, 0.6);
}

.reward-streak .reward-icon-container {
  background: linear-gradient(135deg, #ff6b35, #f7931e);
  box-shadow: 0 8px 32px rgba(255, 87, 34, 0.5);
}

.reward-stats {
  --glow-color: rgba(76, 175, 80, 0.6);
}

.reward-stats .reward-icon-container {
  background: linear-gradient(135deg, #4caf50, #2e7d32);
  box-shadow: 0 8px 32px rgba(76, 175, 80, 0.5);
}

.reward-icon-container {
  width: 80px;
  height: 80px;
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
  position: relative;
  z-index: 1;
  animation: iconFloat 3s ease-in-out infinite;
}

@keyframes iconFloat {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-8px);
  }
}

.reward-amount {
  font-size: 3rem;
  font-weight: 900;
  color: white;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
  margin-bottom: 0.5rem;
  position: relative;
  z-index: 1;
}

.reward-label {
  font-size: 1rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.9);
  text-transform: uppercase;
  letter-spacing: 1px;
  margin-bottom: 1rem;
  position: relative;
  z-index: 1;
}

.reward-progress {
  position: relative;
  z-index: 1;
}

.progress-text {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.7);
  margin-top: 0.5rem;
  font-weight: 600;
}

.reward-info {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.8);
  font-weight: 600;
  position: relative;
  z-index: 1;
}

/* ===== LOGROS ADICIONALES ===== */
.achievements-preview {
  display: flex;
  justify-content: center;
  gap: 2rem;
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 2px solid rgba(255, 255, 255, 0.1);
  position: relative;
  z-index: 1;
  flex-wrap: wrap;
}

.achievement-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  padding: 0.75rem 1.5rem;
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.achievement-item:hover {
  transform: translateY(-4px);
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
}

/* ===== FOOTER - Tema Oscuro ===== */
.dialog-footer {
  background: linear-gradient(to bottom, rgba(26, 10, 46, 0.8) 0%, rgba(15, 10, 26, 0.9) 100%);
  border-top: 2px solid rgba(139, 92, 246, 0.3);
}

.footer-stats {
  display: flex;
  gap: 2rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

.cancel-btn {
  font-weight: 700;
  text-transform: none;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
  border-radius: 12px;
  color: rgba(255, 255, 255, 0.8) !important;
}

.cancel-btn:hover {
  background: rgba(255, 255, 255, 0.1) !important;
  transform: translateY(-2px);
}

.create-btn-epic {
  font-weight: 700;
  text-transform: none;
  letter-spacing: 0.5px;
  background: linear-gradient(135deg, #a78bfa 0%, #22d3ee 100%) !important;
  border-radius: 14px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.create-btn-epic::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2) 0%, transparent 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.create-btn-epic:hover::before {
  opacity: 1;
}

.create-btn-epic:not(:disabled):hover {
  transform: translateY(-4px) scale(1.05);
  box-shadow: 0 12px 32px rgba(167, 139, 250, 0.5) !important;
}

.button-text {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 2px;
}

.main-text {
  font-size: 1.05rem;
  line-height: 1;
  color: white;
}

.sub-text {
  font-size: 0.75rem;
  opacity: 0.9;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.9);
}

/* ===== SCROLLBAR ===== */
.dialog-body::-webkit-scrollbar {
  width: 10px;
}

.dialog-body::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
  margin: 8px 0;
}

.dialog-body::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 10px;
  border: 2px solid #f1f1f1;
}

.dialog-body::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #764ba2, #667eea);
}

/* ===== SELECTOR DE TIPO DE ENTRENAMIENTO - Tema Oscuro ===== */
.training-type-selector {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.training-type-card {
  background: rgba(26, 10, 46, 0.6);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 20px;
  padding: 1.5rem 1rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

.training-type-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--type-color), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.training-type-card:hover {
  transform: translateY(-8px) scale(1.05);
  box-shadow: 0 12px 32px rgba(139, 92, 246, 0.3);
  border-color: rgba(139, 92, 246, 0.6);
}

.training-type-card.active {
  border-color: var(--type-color);
  background: rgba(26, 10, 46, 0.9);
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
}

.training-type-card.active::before {
  opacity: 1;
}

.training-type-card.active .training-type-label,
.training-type-card.active .training-type-description {
  color: white;
}

.training-type-icon {
  width: 64px;
  height: 64px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  position: relative;
  z-index: 1;
  transition: all 0.3s ease;
}

.training-type-card.active .training-type-icon {
  background: rgba(255, 255, 255, 0.15);
}

.training-type-label {
  font-size: 1.1rem;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 0.5rem;
  position: relative;
  z-index: 1;
}

.training-type-description {
  font-size: 0.875rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.6);
  position: relative;
  z-index: 1;
}

.check-indicator {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 32px;
  height: 32px;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2;
  animation: checkPop 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes checkPop {
  0% {
    transform: scale(0);
  }

  50% {
    transform: scale(1.2);
  }

  100% {
    transform: scale(1);
  }
}

/* ===== RESPONSIVE ===== */
@media (max-width: 960px) {
  .dialog-header {
    padding: 2rem 1.5rem;
  }

  .header-icon-wrapper {
    width: 70px;
    height: 70px;
  }

  .pulse-icon {
    font-size: 44px !important;
  }

  .title-glow {
    font-size: 1.75rem !important;
  }

  .difficulty-selector {
    grid-template-columns: 1fr;
  }

  .difficulty-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    text-align: left;
    padding: 1rem 1.5rem;
  }

  .difficulty-icon {
    margin: 0;
    width: 56px;
    height: 56px;
  }

  .rewards-title-wrapper {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .achievements-preview {
    flex-direction: column;
    gap: 1rem;
  }

  .training-type-selector {
    grid-template-columns: 1fr;
  }

  .training-type-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    text-align: left;
    padding: 1rem 1.5rem;
  }

  .training-type-icon {
    margin: 0;
    width: 56px;
    height: 56px;
  }
}

@media (max-width: 600px) {
  .create-dialog {
    max-height: 100vh !important;
    height: 100vh !important;
    border-radius: 0 !important;
    display: flex;
    flex-direction: column;
  }

  .dialog-header {
    padding: 1.5rem 1rem 1rem;
    border-bottom-width: 1px;
    flex-shrink: 0;
  }

  .header-content {
    gap: 0.75rem;
  }

  .header-icon-wrapper {
    width: 52px;
    height: 52px;
    border-radius: 16px;
    border-width: 2px;
  }

  .pulse-icon {
    font-size: 28px !important;
  }

  .icon-glow {
    inset: -6px;
    border-radius: 20px;
  }

  .level-badge {
    font-size: 0.65rem;
    padding: 0.3rem 0.75rem;
    margin-bottom: 0.25rem;
  }

  .title-glow {
    font-size: 1.25rem !important;
    margin-bottom: 0.5rem !important;
  }

  .date-chip {
    padding: 0.35rem 0.75rem;
    border-radius: 10px;
    font-size: 0.8rem;
  }

  .close-btn {
    position: absolute;
    top: 12px;
    right: 12px;
    width: 40px !important;
    height: 40px !important;
  }

  .close-btn .v-icon {
    font-size: 24px !important;
  }

  .dialog-body {
    padding: 1.25rem 1rem !important;
    flex: 1;
    overflow-y: auto;
    max-height: calc(100vh - 200px);
  }

  .form-section {
    margin-bottom: 1.75rem !important;
  }

  .section-header {
    gap: 0.75rem;
    margin-bottom: 1rem !important;
  }

  .section-icon-wrapper {
    width: 40px;
    height: 40px;
    border-radius: 10px;
  }

  .section-icon-wrapper .v-icon {
    font-size: 20px !important;
  }

  .section-title {
    font-size: 1rem;
  }

  .section-subtitle {
    font-size: 0.8rem;
  }

  .chip-required {
    font-size: 0.65rem;
    height: 20px;
    padding: 0 8px;
  }

  .input-wrapper {
    margin-top: 0.5rem;
  }

  .custom-textfield-epic>>>.v-input__slot {
    border-radius: 12px !important;
    padding: 0.5rem 0.75rem !important;
  }

  .custom-textfield-epic>>>input,
  .custom-textfield-epic>>>textarea {
    font-size: 1rem !important;
  }

  .input-icon-wrapper {
    border-radius: 8px;
    padding: 4px;
    margin-right: 6px;
  }

  .input-icon-wrapper .v-icon {
    font-size: 20px !important;
  }

  .tips-box {
    padding: 0.75rem 1rem;
    border-radius: 10px;
    font-size: 0.85rem;
    margin-top: 0.75rem;
  }

  .tips-box .v-icon {
    font-size: 18px !important;
  }

  /* Training Type - Mobile Compact */
  .training-type-selector {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }

  .training-type-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    text-align: left;
    padding: 1rem 1.25rem;
    border-radius: 14px;
    border-width: 1.5px;
  }

  .training-type-icon {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    margin: 0;
    flex-shrink: 0;
  }

  .training-type-icon .v-icon {
    font-size: 26px !important;
  }

  .training-type-label {
    font-size: 1rem;
    margin-bottom: 0.2rem;
  }

  .training-type-description {
    font-size: 0.8rem;
    line-height: 1.3;
  }

  .check-indicator {
    width: 28px;
    height: 28px;
    top: 10px;
    right: 10px;
  }

  .check-indicator .v-icon {
    font-size: 18px !important;
  }

  /* Difficulty - Mobile Compact */
  .difficulty-selector {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }

  .difficulty-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    text-align: left;
    padding: 1rem 1.25rem;
    border-radius: 14px;
    border-width: 1.5px;
  }

  .difficulty-icon {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    margin: 0;
    flex-shrink: 0;
  }

  .difficulty-icon .v-icon {
    font-size: 26px !important;
  }

  .difficulty-label {
    font-size: 1rem;
    margin-bottom: 0.2rem;
  }

  .difficulty-xp {
    font-size: 0.85rem;
  }

  /* Duration - Mobile */
  .duration-display {
    padding: 1.5rem 1rem;
    border-radius: 16px;
  }

  .duration-value {
    font-size: 3rem;
  }

  .duration-unit {
    font-size: 1rem;
  }

  .custom-slider-epic {
    margin-top: 1rem !important;
  }

  .slider-label {
    font-size: 0.8rem;
  }

  .duration-bonus {
    font-size: 0.85rem;
    margin-top: 0.75rem;
  }

  /* Rewards - Mobile */
  .rewards-epic-section {
    padding: 1.5rem 1rem;
    border-radius: 20px;
    margin-top: 1.5rem;
  }

  .rewards-icon-mega {
    width: 56px;
    height: 56px;
    border-radius: 14px;
  }

  .rewards-icon-mega .v-icon {
    font-size: 32px !important;
  }

  .rewards-epic-header {
    margin-bottom: 1.5rem;
  }

  .rewards-epic-header h3 {
    font-size: 1.35rem !important;
    margin-bottom: 0.25rem !important;
  }

  .rewards-subtitle {
    font-size: 0.9rem;
  }

  .rewards-grid {
    gap: 0.75rem !important;
  }

  .reward-card-epic {
    padding: 1.5rem 1rem;
    border-radius: 18px;
  }

  .reward-icon-container {
    width: 60px;
    height: 60px;
    border-radius: 14px;
    margin-bottom: 1rem;
  }

  .reward-icon-container .v-icon {
    font-size: 36px !important;
  }

  .reward-amount {
    font-size: 2.25rem;
    margin-bottom: 0.35rem;
  }

  .reward-label {
    font-size: 0.85rem;
    margin-bottom: 0.75rem;
  }

  .reward-info {
    font-size: 0.8rem;
    gap: 0.4rem;
  }

  .achievements-preview {
    flex-direction: column;
    gap: 0.75rem;
    padding-top: 1.5rem;
    margin-top: 1.5rem;
  }

  .achievement-item {
    padding: 0.75rem 1rem;
    border-radius: 10px;
    font-size: 0.85rem;
    gap: 0.6rem;
  }

  .achievement-item .v-icon {
    font-size: 20px !important;
  }

  /* Footer - Mobile */
  .dialog-footer {
    padding: 1rem !important;
    flex-direction: column;
    gap: 0.75rem;
    flex-shrink: 0;
    border-top-width: 1px;
  }

  .footer-stats {
    flex-direction: row;
    gap: 1rem;
    width: 100%;
    justify-content: center;
  }

  .stat-item {
    font-size: 0.8rem;
    gap: 0.4rem;
  }

  .stat-item .v-icon {
    font-size: 16px !important;
  }

  .dialog-footer .v-spacer {
    display: none;
  }

  .dialog-footer .cancel-btn {
    width: 100%;
    order: 2;
    min-height: 48px;
  }

  .create-btn-epic {
    width: 100%;
    margin-left: 0 !important;
    margin-top: 0 !important;
    order: 1;
    min-height: 56px;
    border-radius: 14px !important;
  }

  .create-btn-epic .v-icon {
    font-size: 24px !important;
  }

  .button-text {
    align-items: center;
  }

  .main-text {
    font-size: 1rem;
  }

  .sub-text {
    font-size: 0.75rem;
  }
}

@media (max-width: 360px) {
  .dialog-header {
    padding: 1rem 0.75rem;
  }

  .header-icon-wrapper {
    width: 44px;
    height: 44px;
  }

  .pulse-icon {
    font-size: 24px !important;
  }

  .title-glow {
    font-size: 1.1rem !important;
  }

  .dialog-body {
    padding: 0.75rem !important;
  }

  .section-icon-wrapper {
    width: 32px;
    height: 32px;
  }

  .section-title {
    font-size: 0.9rem;
  }

  .section-subtitle {
    font-size: 0.75rem;
  }

  .training-type-card,
  .difficulty-card {
    padding: 0.75rem;
  }

  .training-type-icon,
  .difficulty-icon {
    width: 38px;
    height: 38px;
  }

  .training-type-label,
  .difficulty-label {
    font-size: 0.85rem;
  }

  .rewards-epic-section {
    padding: 1rem 0.75rem;
    border-radius: 16px;
  }

  .reward-card-epic {
    padding: 1rem 0.6rem;
    border-radius: 14px;
  }

  .reward-amount {
    font-size: 1.75rem;
  }

  .reward-label {
    font-size: 0.7rem;
  }

  .duration-value {
    font-size: 2rem;
  }
}
</style>