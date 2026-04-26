<template>
  <div class="history-section">
    <div class="history-header">
      <div class="header-left">
        <v-icon size="22" color="#ffcc00">mdi-history</v-icon>
        <h2>Mis Registros</h2>
      </div>
      <v-chip
        v-if="results.length > 0"
        size="small"
        color="#ffcc00"
        text-color="#000"
        class="count-chip"
      >
        {{ results.length }}
      </v-chip>
    </div>

    <div v-if="results.length > 0" class="history-grid">
      <div v-for="(result, index) in results" :key="index" class="history-card">
        <div class="history-card-header">
          <span class="history-type" :class="result.type">
            <v-icon size="14">{{ result.type === 'bmi' ? 'mdi-scale-bathroom' : 'mdi-fire' }}</v-icon>
            {{ result.type === 'bmi' ? 'IMC' : 'Calorías' }}
          </span>
          <v-btn
            icon="mdi-delete"
            size="x-small"
            variant="text"
            density="compact"
            @click="handleDelete(index)"
          />
        </div>

        <div class="history-content">
          <div v-if="result.type === 'bmi'" class="history-item">
            <span class="history-value" :class="`status-${getBMIStatus(result.bmi)}`">
              {{ result.bmi?.toFixed(1) }}
            </span>
            <span class="history-meta">{{ getBMILabel(result.bmi) }}</span>
          </div>
          <div v-if="result.type === 'calories'" class="history-item">
            <span class="history-value">{{ result.tdee }}</span>
            <span class="history-meta">kcal / día</span>
          </div>
        </div>

        <div class="history-footer">
          <span class="history-date">{{ formatDate(result.date) }}</span>
        </div>
      </div>
    </div>

    <div v-else class="history-empty">
      <v-icon size="40" color="rgba(255, 204, 0, 0.2)">mdi-history</v-icon>
      <p>Sin registros aún</p>
    </div>
  </div>
</template>

<script setup lang="ts">
interface HistoryItem {
  type: 'bmi' | 'calories'
  date: string
  bmi?: number
  tdee?: number
  weight?: number
  height?: number
}

interface Props {
  results: HistoryItem[]
}

withDefaults(defineProps<Props>(), {
  results: () => [],
})

const emit = defineEmits<{
  'delete': [index: number]
}>()

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toLocaleDateString('es-ES', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const getBMIStatus = (bmiValue?: number): string => {
  if (!bmiValue) return 'normal'
  if (bmiValue < 18.5) return 'underweight'
  if (bmiValue < 25) return 'normal'
  if (bmiValue < 30) return 'overweight'
  return 'obese'
}

const getBMILabel = (bmiValue?: number): string => {
  if (!bmiValue) return ''
  if (bmiValue < 18.5) return 'Bajo peso'
  if (bmiValue < 25) return 'Normal'
  if (bmiValue < 30) return 'Sobrepeso'
  return 'Obesidad'
}

const handleDelete = (index: number) => {
  emit('delete', index)
}
</script>

<style scoped>
.history-section {
  padding: 1.5rem;
  border-radius: 14px;
  border: 1.5px solid rgba(255, 204, 0, 0.15);
  background: rgba(20, 20, 20, 0.55);
  backdrop-filter: blur(10px);
}

.history-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.25rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.history-header h2 {
  margin: 0;
  color: #ffcc00;
  font-size: 1.1rem;
  font-weight: 700;
}

.count-chip {
  font-weight: 700;
}

.history-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  gap: 0.875rem;
}

.history-card {
  padding: 0.875rem;
  border-radius: 10px;
  border: 1px solid rgba(255, 204, 0, 0.1);
  background: rgba(255, 204, 0, 0.02);
  transition: all 0.2s ease;
  display: flex;
  flex-direction: column;
}

.history-card:hover {
  background: rgba(255, 204, 0, 0.06);
  border-color: rgba(255, 204, 0, 0.25);
  transform: translateY(-2px);
}

.history-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.6rem;
}

.history-type {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  padding: 0.2rem 0.5rem;
  border-radius: 6px;
}

.history-type.bmi {
  color: #ffcc00;
  background: rgba(255, 204, 0, 0.1);
}

.history-type.calories {
  color: #ff8c00;
  background: rgba(255, 140, 0, 0.1);
}

.history-card :deep(.v-btn) {
  color: rgba(255, 255, 255, 0.4) !important;
}

.history-card :deep(.v-btn:hover) {
  color: #ff4444 !important;
}

.history-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  margin-bottom: 0.5rem;
}

.history-item {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
}

.history-value {
  font-weight: 900;
  color: #ffcc00;
  font-size: 1.3rem;
  line-height: 1;
}

.history-meta {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.45);
  font-weight: 500;
}

.history-footer {
  padding-top: 0.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.history-date {
  font-size: 0.7rem;
  color: rgba(255, 255, 255, 0.4);
  font-weight: 500;
}

.status-underweight { color: #6496ff !important; }
.status-normal { color: #4ade80 !important; }
.status-overweight { color: #ffc107 !important; }
.status-obese { color: #ff4444 !important; }

.history-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2.5rem;
  color: rgba(255, 255, 255, 0.35);
  text-align: center;
  min-height: 160px;
  gap: 0.5rem;
}

.history-empty p {
  margin: 0;
  font-size: 0.9rem;
}

@media (max-width: 768px) {
  .history-grid {
    grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  }
}

@media (max-width: 600px) {
  .history-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
