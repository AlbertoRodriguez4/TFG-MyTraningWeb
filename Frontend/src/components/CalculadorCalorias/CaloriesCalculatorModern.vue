<template>
  <div class="calc-section calories-section">
    <div class="section-header">
      <v-icon size="28" color="#ffcc00">mdi-fire</v-icon>
      <h2 class="section-title">Calorías Necesarias</h2>
    </div>

    <div class="inputs-grid">
      <div class="input-group">
        <label>Edad</label>
        <v-text-field
          v-model.number="age"
          type="number"
          variant="outlined"
          density="compact"
          hide-details
          @input="calculateCalories"
          placeholder="28"
        />
      </div>

      <div class="input-group">
        <label>Género</label>
        <v-select
          v-model="gender"
          :items="genderOptions"
          variant="outlined"
          density="compact"
          hide-details
          @update:model-value="calculateCalories"
        />
      </div>

      <div class="input-group">
        <label>Actividad</label>
        <v-select
          v-model="activity"
          :items="activityOptions"
          variant="outlined"
          density="compact"
          hide-details
          @update:model-value="calculateCalories"
        />
      </div>
    </div>

    <div class="input-group">
      <label>Objetivo</label>
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

    <div v-if="result" class="calories-result">
      <div class="calories-main">
        <span class="calories-label">Calorías Diarias</span>
        <span class="calories-number">{{ result.tdee }}</span>
        <span class="calories-unit">kcal</span>
      </div>

      <div class="macros-mini">
        <div class="macro-mini-item">
          <div class="macro-mini-label">Proteína</div>
          <div class="macro-mini-value">{{ result.macros.protein }}g</div>
        </div>
        <div class="macro-mini-item">
          <div class="macro-mini-label">Carbs</div>
          <div class="macro-mini-value">{{ result.macros.carbs }}g</div>
        </div>
        <div class="macro-mini-item">
          <div class="macro-mini-label">Grasas</div>
          <div class="macro-mini-value">{{ result.macros.fat }}g</div>
        </div>
      </div>

      <v-btn
        color="#ffcc00"
        text-color="#000"
        variant="flat"
        size="small"
        class="save-btn"
        @click="handleSave"
        block
      >
        <v-icon start size="18">mdi-content-save</v-icon>
        Guardar Cálculo
      </v-btn>
    </div>

    <div v-else class="empty-placeholder">
      <v-icon size="32" color="rgba(255,204,0,0.2)">mdi-food-apple</v-icon>
      <span>Completa los datos</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface CaloriesResult {
  bmr: number
  tdee: number
  macros: {
    protein: number
    carbs: number
    fat: number
  }
}

interface Props {
  bmiWeight?: number | null
  bmiHeight?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  bmiWeight: null,
  bmiHeight: null,
})

const emit = defineEmits<{
  'result-changed': [value: CaloriesResult]
  'save': []
}>()

const age = ref<number | null>(null)
const gender = ref<'male' | 'female'>('male')
const activity = ref('moderate')
const currentGoal = ref('maintenance')
const result = ref<CaloriesResult | null>(null)

const genderOptions = [
  { title: 'Hombre', value: 'male' },
  { title: 'Mujer', value: 'female' },
]

const activityOptions = [
  { title: 'Sedentario', value: 'sedentary' },
  { title: 'Ligero', value: 'light' },
  { title: 'Moderado', value: 'moderate' },
  { title: 'Activo', value: 'active' },
  { title: 'Muy Activo', value: 'veryactive' },
]

const goalOptions = [
  { value: 'loss', label: 'Pérdida', icon: '📉' },
  { value: 'maintenance', label: 'Mantener', icon: '⚖️' },
  { value: 'gain', label: 'Ganancia', icon: '📈' },
]

const calculateCalories = () => {
  if (!age.value || !props.bmiWeight || !props.bmiHeight) {
    result.value = null
    return
  }
  let bmr: number
  if (gender.value === 'male') {
    bmr = 88.362 + (13.397 * props.bmiWeight) + (4.799 * props.bmiHeight) - (5.677 * age.value)
  } else {
    bmr = 447.593 + (9.247 * props.bmiWeight) + (3.098 * props.bmiHeight) - (4.33 * age.value)
  }
  const activityMultipliers: Record<string, number> = {
    sedentary: 1.2, light: 1.375, moderate: 1.55, active: 1.725, veryactive: 1.9,
  }
  const tdeeBase = bmr * (activityMultipliers[activity.value] || 1.55)
  const goalAdjustments: Record<string, number> = { loss: -500, maintenance: 0, gain: 500 }
  const tdee = Math.round(tdeeBase + (goalAdjustments[currentGoal.value] || 0))
  const protein = Math.round(props.bmiWeight * 2)
  const fat = Math.round((tdee * 0.3) / 9)
  const carbs = Math.round((tdee - protein * 4 - fat * 9) / 4)
  result.value = { bmr: Math.round(bmr), tdee, macros: { protein, carbs, fat } }
  emit('result-changed', result.value)
}

const handleSave = () => {
  emit('save')
}
</script>

<style scoped>
.calc-section {
  padding: 1.5rem;
  border-radius: 14px;
  border: 1.5px solid rgba(255, 204, 0, 0.15);
  background: rgba(20, 20, 20, 0.55);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.calc-section:hover {
  border-color: rgba(255, 204, 0, 0.3);
  background: rgba(20, 20, 20, 0.7);
}

.section-header {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin-bottom: 1.25rem;
}

.section-title {
  font-size: 1.15rem;
  font-weight: 700;
  color: #ffcc00;
  margin: 0;
}

.inputs-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.input-group {
  margin-bottom: 1rem;
}

.input-group label {
  display: block;
  margin-bottom: 0.4rem;
  color: rgba(255, 255, 255, 0.85);
  font-weight: 600;
  font-size: 0.85rem;
}

:deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.12) !important;
}

:deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.06) !important;
  border-color: rgba(255, 204, 0, 0.35) !important;
}

:deep(.v-field__input) {
  color: rgba(255, 255, 255, 0.9) !important;
  font-size: 0.9rem;
}

.goal-selector {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.6rem;
}

.goal-btn {
  padding: 0.6rem;
  border-radius: 8px;
  border: 1.5px solid rgba(255, 204, 0, 0.12);
  background: rgba(255, 204, 0, 0.02);
  color: rgba(255, 255, 255, 0.65);
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.3rem;
  font-weight: 600;
}

.goal-btn:hover {
  border-color: rgba(255, 204, 0, 0.25);
  background: rgba(255, 204, 0, 0.04);
}

.goal-btn.active {
  border-color: #ffcc00;
  background: rgba(255, 204, 0, 0.1);
  color: #ffcc00;
  transform: scale(1.03);
}

.goal-icon { font-size: 1.1rem; }
.goal-text { font-size: 0.75rem; }

.calories-result {
  background: rgba(255, 204, 0, 0.04);
  border: 1px solid rgba(255, 204, 0, 0.12);
  border-radius: 10px;
  padding: 1.25rem;
}

.calories-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.3rem;
  margin-bottom: 1rem;
}

.calories-label {
  color: rgba(255, 255, 255, 0.65);
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.calories-number {
  font-size: 2.2rem;
  font-weight: 900;
  color: #ffcc00;
  line-height: 1;
}

.calories-unit {
  color: rgba(255, 255, 255, 0.55);
  font-size: 0.8rem;
}

.macros-mini {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.6rem;
  margin-bottom: 1rem;
}

.macro-mini-item {
  background: rgba(255, 204, 0, 0.06);
  padding: 0.65rem;
  border-radius: 8px;
  text-align: center;
  border: 1px solid rgba(255, 204, 0, 0.08);
}

.macro-mini-label {
  font-size: 0.7rem;
  color: rgba(255, 255, 255, 0.55);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.25rem;
}

.macro-mini-value {
  font-size: 0.95rem;
  font-weight: 700;
  color: #ffcc00;
}

.empty-placeholder {
  height: 130px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  color: rgba(255, 255, 255, 0.35);
  font-size: 0.85rem;
  text-align: center;
  border: 1.5px dashed rgba(255, 204, 0, 0.08);
  border-radius: 10px;
}

.save-btn {
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-size: 0.8rem;
  transition: all 0.2s ease;
}

.save-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(255, 204, 0, 0.25);
}

@media (max-width: 600px) {
  .calc-section { padding: 1.25rem; }
  .inputs-grid { grid-template-columns: 1fr; }
  .goal-selector { gap: 0.5rem; }
  .calories-number { font-size: 1.8rem; }
}

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
