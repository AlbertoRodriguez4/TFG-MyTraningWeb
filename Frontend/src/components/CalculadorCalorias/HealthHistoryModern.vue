<template>
  <div class="history-section">
    <div class="history-header">
      <h2>{{ $t('my_records') }}</h2>
      <v-chip
        v-if="results.length > 0"
        size="small"
        color="#ffcc00"
        text-color="#000"
      >
        {{ results.length }}
      </v-chip>
    </div>

    <div v-if="results.length > 0" class="history-grid">
      <div v-for="(result, index) in results" :key="index" class="history-card">
        <div class="history-card-header">
          <span class="history-date">{{ formatDate(result.date) }}</span>
          <v-btn
            icon="mdi-delete"
            size="x-small"
            variant="text"
            @click="handleDelete(index)"
          />
        </div>

        <div class="history-content">
          <div v-if="result.type === 'bmi'" class="history-item">
            <span class="history-label">{{ $t('bmi_label') }}</span>
            <span class="history-value" :class="`status-${getBMIStatus(result.bmi)}`">
              {{ result.bmi?.toFixed(1) }}
            </span>
          </div>
          <div v-if="result.type === 'calories'" class="history-item">
            <span class="history-label">{{ $t('calories_label') }}</span>
            <span class="history-value">{{ result.tdee }}</span>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="history-empty">
      <v-icon size="48" color="rgba(255, 204, 0, 0.3)">mdi-history</v-icon>
      <p>{{ $t('no_records_yet') }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { locale } = useI18n()

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

// Helpers
const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toLocaleDateString(locale.value, {
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

const handleDelete = (index: number) => {
  emit('delete', index)
}
</script>

<style scoped>
.history-section {
  padding: 2rem;
  border-radius: 16px;
  border: 2px solid rgba(255, 204, 0, 0.2);
  background: rgba(20, 20, 20, 0.6);
  backdrop-filter: blur(10px);
}

.history-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.history-header h2 {
  margin: 0;
  color: #ffcc00;
  font-size: 1.3rem;
  font-weight: 700;
}

.history-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.history-card {
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.15);
  background: rgba(255, 204, 0, 0.03);
  transition: all 0.2s ease;
}

.history-card:hover {
  background: rgba(255, 204, 0, 0.08);
  border-color: rgba(255, 204, 0, 0.3);
}

.history-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.8rem;
  padding-bottom: 0.8rem;
  border-bottom: 1px solid rgba(255, 204, 0, 0.1);
}

.history-date {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 600;
}

.history-card :deep(.v-btn) {
  color: rgba(255, 255, 255, 0.5) !important;
}

.history-card :deep(.v-btn:hover) {
  color: #ff4444 !important;
}

.history-content {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.history-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.history-label {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.8rem;
  font-weight: 600;
}

.history-value {
  font-weight: 700;
  color: #ffcc00;
  font-size: 1.1rem;
}

.status-underweight {
  color: #6496ff !important;
}

.status-normal {
  color: #4ade80 !important;
}

.status-overweight {
  color: #ffc107 !important;
}

.status-obese {
  color: #ff4444 !important;
}

.history-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: rgba(255, 255, 255, 0.4);
  text-align: center;
  min-height: 200px;
}

.history-empty p {
  margin: 1rem 0 0 0;
  font-size: 0.95rem;
}

/* Responsive */
@media (max-width: 768px) {
  .history-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  }
}

@media (max-width: 600px) {
  .history-grid {
    grid-template-columns: 1fr;
  }
}
</style>