<template>
  <div class="calc-section bmi-section">
    <div class="section-icon">📊</div>
    <h2 class="section-title">Índice de Masa Corporal</h2>

    <div class="input-group">
      <label>Peso</label>
      <div class="input-with-unit">
        <v-text-field
          v-model.number="weight"
          type="number"
          variant="outlined"
          density="compact"
          @input="calculateBMI"
          placeholder="75"
        />
        <span class="unit">kg</span>
      </div>
    </div>

    <div class="input-group">
      <label>Altura</label>
      <div class="input-with-unit">
        <v-text-field
          v-model.number="height"
          type="number"
          variant="outlined"
          density="compact"
          @input="calculateBMI"
          placeholder="180"
        />
        <span class="unit">cm</span>
      </div>
    </div>

    <!-- BMI Result Circle -->
    <div v-if="result" class="result-circle">
      <svg viewBox="0 0 100 100" class="circle-svg">
        <circle cx="50" cy="50" r="45" class="circle-bg" />
        <circle
          cx="50"
          cy="50"
          r="45"
          class="circle-progress"
          :class="getStatusClass(result.bmi)"
          :style="{ strokeDashoffset: getCircleOffset(result.bmi) }"
        />
      </svg>
      <div class="circle-content">
        <div class="bmi-number">{{ result.bmi.toFixed(1) }}</div>
        <div class="bmi-label">{{ result.category }}</div>
      </div>
    </div>

    <div v-else class="empty-placeholder">
      Ingresa peso y altura
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
      Guardar IMC
    </v-btn>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'

interface BMIResult {
  bmi: number
  category: string
}

// Props and Emits
const emit = defineEmits<{
  'result-changed': [value: { weight: number | null; height: number | null; bmi?: number; category?: string }]
  'save': []
}>()

// State
const weight = ref<number | null>(null)
const height = ref<number | null>(null)
const result = ref<BMIResult | null>(null)

// Calculate BMI
const calculateBMI = () => {
  if (!weight.value || !height.value) {
    result.value = null
    emit('result-changed', { weight: weight.value, height: height.value })
    return
  }

  const heightInMeters = height.value / 100
  const bmiValue = weight.value / (heightInMeters * heightInMeters)

  let category = ''
  if (bmiValue < 18.5) category = 'Bajo peso'
  else if (bmiValue < 25) category = 'Peso normal'
  else if (bmiValue < 30) category = 'Sobrepeso'
  else category = 'Obesidad'

  result.value = { bmi: bmiValue, category }

  emit('result-changed', {
    weight: weight.value,
    height: height.value,
    bmi: bmiValue,
    category,
  })
}

// Helpers
const getStatusClass = (bmiValue: number): string => {
  if (bmiValue < 18.5) return 'underweight'
  if (bmiValue < 25) return 'normal'
  if (bmiValue < 30) return 'overweight'
  return 'obese'
}

const getCircleOffset = (bmiValue: number): number => {
  const circumference = 2 * Math.PI * 45
  const maxBMI = 40
  const percentage = Math.min((bmiValue / maxBMI) * 100, 100)
  return circumference - (circumference * percentage) / 100
}

const handleSave = () => {
  // Guardar perfil de salud en localStorage para el chatbot
  try {
    const existing = JSON.parse(localStorage.getItem('userHealthProfile') || '{}')
    const updated = {
      ...existing,
      weight: weight.value,
      height: height.value,
      bmi: result.value?.bmi,
      bmiCategory: result.value?.category,
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

.input-with-unit {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.input-with-unit :deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.15) !important;
}

.input-with-unit :deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.08) !important;
  border-color: rgba(255, 204, 0, 0.4) !important;
}

.input-with-unit :deep(.v-field__input) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.unit {
  color: rgba(255, 255, 255, 0.6);
  font-weight: 600;
  min-width: 2rem;
  text-align: center;
}

/* Result Circle */
.result-circle {
  position: relative;
  width: 150px;
  height: 150px;
  margin: 2rem auto;
  display: flex;
  align-items: center;
  justify-content: center;
}

.circle-svg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  transform: rotate(-90deg);
}

.circle-bg {
  fill: none;
  stroke: rgba(255, 204, 0, 0.1);
  stroke-width: 3;
}

.circle-progress {
  fill: none;
  stroke: #ffcc00;
  stroke-width: 3;
  stroke-linecap: round;
  transition: stroke-dashoffset 0.3s ease;
}

.circle-progress.underweight {
  stroke: #6496ff;
}

.circle-progress.normal {
  stroke: #4ade80;
}

.circle-progress.overweight {
  stroke: #ffc107;
}

.circle-progress.obese {
  stroke: #ff4444;
}

.circle-content {
  text-align: center;
  z-index: 1;
}

.bmi-number {
  font-size: 2rem;
  font-weight: 900;
  color: #ffcc00;
}

.bmi-label {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
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

  .result-circle {
    width: 120px;
    height: 120px;
  }

  .bmi-number {
    font-size: 1.5rem;
  }
}
</style>