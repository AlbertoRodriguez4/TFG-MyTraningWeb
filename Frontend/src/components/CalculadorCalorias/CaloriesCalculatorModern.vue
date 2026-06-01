<template>
  <div class="calc-section calories-section">
    <v-icon class="section-icon" color="orange">mdi-fire</v-icon>
    <h2 class="section-title">{{ $t('calories_needed') }}</h2>

    <div class="input-group">
      <label>{{ $t('age_label') }}</label>
      <v-text-field
        v-model.number="age"
        type="number"
        variant="outlined"
        density="compact"
        @input="calculateCalories"
        placeholder="28"
      />
    </div>

    <div class="input-group">
      <label>{{ $t('gender') }}</label>
      <v-select
        v-model="gender"
        :items="genderOptions"
        variant="outlined"
        density="compact"
        @update:model-value="calculateCalories"
      />
    </div>

    <div class="input-group">
      <label>{{ $t('activity') }}</label>
      <v-select
        v-model="activity"
        :items="activityOptions"
        variant="outlined"
        density="compact"
        @update:model-value="calculateCalories"
      />
    </div>

    <div class="input-group">
      <label>{{ $t('goal') }}</label>
      <div class="goal-selector">
        <button
          v-for="goal in goalOptions"
          :key="goal.value"
          class="goal-btn"
          :class="{ active: currentGoal === goal.value }"
          @click="currentGoal = goal.value; calculateCalories()"
        >
          <span class="goal-icon">{{ goal.icon }}</span>
          <span class="goal-text">{{ goal.label }}</span>
        </button>
      </div>
    </div>

    <!-- Calories Result Card -->
    <div v-if="result" class="calories-result">
      <div class="calories-main">
        <span class="calories-label">{{ $t('daily_calories') }}</span>
        <span class="calories-number">{{ result.tdee }}</span>
        <span class="calories-unit">kcal</span>
      </div>

      <!-- Macros Mini -->
      <div class="macros-mini">
        <div class="macro-mini-item">
          <div class="macro-mini-label">{{ $t('protein') }}</div>
          <div class="macro-mini-value">{{ result.macros.protein }}g</div>
        </div>
        <div class="macro-mini-item">
          <div class="macro-mini-label">{{ $t('carbs') }}</div>
          <div class="macro-mini-value">{{ result.macros.carbs }}g</div>
        </div>
        <div class="macro-mini-item">
          <div class="macro-mini-label">{{ $t('fats') }}</div>
          <div class="macro-mini-value">{{ result.macros.fat }}g</div>
        </div>
      </div>
    </div>

    <div v-else class="empty-placeholder">
      {{ $t('fill_data') }}
    </div>

    <v-btn
      v-if="result"
      color="#ffcc00"
      text-color="#000"
      variant="flat"
      size="large"
      class="save-btn"
      @click="handleSave"
      block
    >
      <v-icon start>mdi-content-save</v-icon>
      {{ $t('save_calculation') }}
    </v-btn>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { CaloriesResult } from '@/components/Models/Health'

const { t } = useI18n()

// Props
interface Props {
  bmiWeight?: number | null
  bmiHeight?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  bmiWeight: null,
  bmiHeight: null,
})

// Emits
const emit = defineEmits<{
  'result-changed': [value: CaloriesResult]
  'save': []
}>()

// State
const age = ref<number | null>(null)
const gender = ref<'male' | 'female'>('male')
const activity = ref('moderate')
const currentGoal = ref('maintenance')
const result = ref<CaloriesResult | null>(null)

// Options
const genderOptions = computed(() => [
  { title: t('male'), value: 'male' },
  { title: t('female'), value: 'female' },
])

const activityOptions = computed(() => [ // Definir opciones de actividad 
  { title: t('sedentary'), value: 'sedentary' },
  { title: t('light'), value: 'light' },
  { title: t('moderate'), value: 'moderate' },
  { title: t('active_label'), value: 'active' },
  { title: t('very_active'), value: 'veryactive' },
])

const goalOptions = computed(() => [ // Definir opciones de objetivos con iconos
  { value: 'loss', label: t('loss'), icon: 'mdi mdi-trending-down' },
  { value: 'maintenance', label: t('maintenance'), icon: 'mdi mdi-trending-neutral' },
  { value: 'gain', label: t('gain_label'), icon: 'mdi mdi-trending-up' },
])

// Calcular las calorías en base a los parámetos ingresados y el peso/altura del BMI
const calculateCalories = () => {
  if (!age.value || !props.bmiWeight || !props.bmiHeight) {
    result.value = null
    return
  }

  // Calcular el BMI con la fórmula estándar y luego usarlo para calcular el BMR con la fórmula de Harris-Benedict
  let bmr: number
  if (gender.value === 'male') {
    bmr = 88.362 + (13.397 * props.bmiWeight) + (4.799 * props.bmiHeight) - (5.677 * age.value)
  } else {
    bmr = 447.593 + (9.247 * props.bmiWeight) + (3.098 * props.bmiHeight) - (4.33 * age.value)
  }

  // Agregar variable de cantidad de actividad física para calcular el TDEE
  const activityMultipliers: Record<string, number> = {
    sedentary: 1.2,
    light: 1.375,
    moderate: 1.55,
    active: 1.725,
    veryactive: 1.9,
  }

  const tdeeBase = bmr * (activityMultipliers[activity.value] || 1.55) // Calcular TDEE base sin ajustes de objetivo

  // Ajustar objetivo de calorías según la meta seleccionada (pérdida, mantenimiento o ganancia)
  const goalAdjustments: Record<string, number> = {
    loss: -500,
    maintenance: 0,
    gain: 500,
  }

  const tdee = Math.round(tdeeBase + (goalAdjustments[currentGoal.value] || 0)) // Calcular TDEE final con ajuste de objetivo, y redondear el resutlado final 

  // Calcular los macros en base a los resultados obtenidos 
  const protein = Math.round(props.bmiWeight * 2)
  const fat = Math.round((tdee * 0.3) / 9)
  const carbs = Math.round((tdee - protein * 4 - fat * 9) / 4)

  result.value = {
    bmr: Math.round(bmr),
    tdee,
    macros: { protein, carbs, fat },
  }

  emit('result-changed', result.value)
}

const handleSave = () => {
  // Guardar perfil de salud en localStorage para el chatbot
  try {
    const existing = JSON.parse(localStorage.getItem('userHealthProfile') || '{}')
    const updated = {
      ...existing,
      age: age.value,
      gender: gender.value,
      activity: activity.value,
      goal: currentGoal.value,
      bmr: result.value?.bmr,
      tdee: result.value?.tdee,
      macros: result.value?.macros,
      lastUpdated: new Date().toISOString(),
    }
    localStorage.setItem('userHealthProfile', JSON.stringify(updated))
  } catch (e) {
    console.error('Error guardando perfil de salud:', e)
  }
  emit('save')
}
</script>

<style scoped>
.calc-section {
  padding: 2rem;
  border-radius: 16px;
  border: 2px solid rgba(255, 204, 0, 0.2);
  background: rgba(20, 20, 20, 0.6);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.calc-section:hover {
  border-color: rgba(255, 204, 0, 0.4);
  background: rgba(20, 20, 20, 0.8);
  transform: translateY(-2px);
}

.section-icon {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #ffcc00;
  margin: 0 0 1.5rem 0;
}

/* Inputs */
.input-group {
  margin-bottom: 1.5rem;
}

.input-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  font-size: 0.95rem;
}

:deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.15) !important;
}

:deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.08) !important;
  border-color: rgba(255, 204, 0, 0.4) !important;
}

:deep(.v-field__input) {
  color: rgba(255, 255, 255, 0.9) !important;
}

/* Goal Selector */
.goal-selector {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.8rem;
}

.goal-btn {
  padding: 0.8rem;
  border-radius: 8px;
  border: 2px solid rgba(255, 204, 0, 0.15);
  background: rgba(255, 204, 0, 0.02);
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.4rem;
  font-weight: 600;
}

.goal-btn:hover {
  border-color: rgba(255, 204, 0, 0.3);
  background: rgba(255, 204, 0, 0.05);
}

.goal-btn.active {
  border-color: #ffcc00;
  background: rgba(255, 204, 0, 0.1);
  color: #ffcc00;
  transform: scale(1.05);
}

.goal-icon {
  font-size: 1.3rem;
}

.goal-text {
  font-size: 0.8rem;
}

/* Calories Result */
.calories-result {
  background: rgba(255, 204, 0, 0.05);
  border: 1px solid rgba(255, 204, 0, 0.15);
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.calories-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.calories-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.calories-number {
  font-size: 2.5rem;
  font-weight: 900;
  color: #ffcc00;
}

.calories-unit {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.85rem;
}

/* Macros Mini */
.macros-mini {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.8rem;
}

.macro-mini-item {
  background: rgba(255, 204, 0, 0.08);
  padding: 0.8rem;
  border-radius: 8px;
  text-align: center;
  border: 1px solid rgba(255, 204, 0, 0.1);
}

.macro-mini-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.6);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.3rem;
}

.macro-mini-value {
  font-size: 1rem;
  font-weight: 700;
  color: #ffcc00;
}

/* Empty Placeholder */
.empty-placeholder {
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.95rem;
  text-align: center;
  border: 2px dashed rgba(255, 204, 0, 0.1);
  border-radius: 8px;
  margin-bottom: 1.5rem;
}

/* Save Button */
.save-btn {
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.2s ease;
}

.save-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(255, 204, 0, 0.3);
}

/* Responsive */
@media (max-width: 600px) {
  .calc-section {
    padding: 1.5rem;
  }

  .goal-selector {
    grid-template-columns: repeat(3, 1fr);
  }

  .calories-number {
    font-size: 2rem;
  }
}

/* Select Styling */
:deep(.v-select__content .v-list) {
  background: rgba(20, 20, 20, 0.95) !important;
}

:deep(.v-list-item) {
  color: rgba(255, 255, 255, 0.9) !important;
}

:deep(.v-list-item.v-list-item--active) {
  background: rgba(255, 204, 0, 0.1) !important;
  color: #ffcc00 !important;
}
</style>