<template>
  <div class="calc-section bmi-section">
    <div class="section-header">
      <v-icon size="28" color="#ffcc00">mdi-chart-bar</v-icon>
      <h2 class="section-title">Índice de Masa Corporal</h2>
    </div>

    <div class="inputs-row">
      <div class="input-group">
        <label>Peso</label>
        <div class="input-with-unit">
          <v-text-field
            v-model.number="weight"
            type="number"
            variant="outlined"
            density="compact"
            hide-details
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
            hide-details
            @input="calculateBMI"
            placeholder="180"
          />
          <span class="unit">cm</span>
        </div>
      </div>
    </div>

    <div v-if="result" class="result-area">
      <div class="result-circle">
        <svg viewBox="0 0 100 100" class="circle-svg">
          <circle cx="50" cy="50" r="45" class="circle-bg" />
          <circle
            cx="50" cy="50" r="45"
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
        Guardar IMC
      </v-btn>
    </div>

    <div v-else class="empty-placeholder">
      <v-icon size="32" color="rgba(255,204,0,0.2)">mdi-scale-bathroom</v-icon>
      <span>Ingresa peso y altura</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface BMIResult {
  bmi: number
  category: string
}

const emit = defineEmits<{
  'result-changed': [value: { weight: number | null; height: number | null; bmi?: number; category?: string }]
  'save': []
}>()

const weight = ref<number | null>(null)
const height = ref<number | null>(null)
const result = ref<BMIResult | null>(null)

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
  emit('result-changed', { weight: weight.value, height: height.value, bmi: bmiValue, category })
}

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

.inputs-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.25rem;
}

.input-group label {
  display: block;
  margin-bottom: 0.4rem;
  color: rgba(255, 255, 255, 0.85);
  font-weight: 600;
  font-size: 0.85rem;
}

.input-with-unit {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.input-with-unit :deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.12) !important;
}

.input-with-unit :deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.06) !important;
  border-color: rgba(255, 204, 0, 0.35) !important;
}

.input-with-unit :deep(.v-field__input) {
  color: rgba(255, 255, 255, 0.9) !important;
  font-size: 0.9rem;
}

.unit {
  color: rgba(255, 255, 255, 0.55);
  font-weight: 600;
  min-width: 1.8rem;
  text-align: center;
  font-size: 0.85rem;
}

.result-area {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.result-circle {
  position: relative;
  width: 130px;
  height: 130px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.circle-svg {
  position: absolute;
  top: 0; left: 0;
  width: 100%; height: 100%;
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
  transition: stroke-dashoffset 0.4s ease;
}

.circle-progress.underweight { stroke: #6496ff; }
.circle-progress.normal { stroke: #4ade80; }
.circle-progress.overweight { stroke: #ffc107; }
.circle-progress.obese { stroke: #ff4444; }

.circle-content {
  text-align: center;
  z-index: 1;
}

.bmi-number {
  font-size: 1.75rem;
  font-weight: 900;
  color: #ffcc00;
  line-height: 1;
}

.bmi-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.65);
  font-weight: 600;
  margin-top: 0.2rem;
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
  .inputs-row { grid-template-columns: 1fr; gap: 0.75rem; }
  .result-circle { width: 110px; height: 110px; }
  .bmi-number { font-size: 1.5rem; }
}
</style>
