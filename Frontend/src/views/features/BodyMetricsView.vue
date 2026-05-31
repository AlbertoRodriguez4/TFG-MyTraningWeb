<template>
  <v-app>
    <v-main class="body-metrics-page">
      <v-container fluid class="pa-6">
        <!-- Header -->
        <div class="page-header mb-8">
          <div class="header-icon">
            <v-icon size="48" color="white">mdi-chart-line</v-icon>
          </div>
          <div>
            <h1 class="text-h3 font-weight-bold text-white">{{ $t('body_metrics_title') }}</h1>
            <p class="text-subtitle-1 text-grey-light">{{ $t('body_metrics_subtitle') }}</p>
          </div>
        </div>

        <v-row>
          <!-- Formulario -->
          <v-col cols="12" md="4">
            <v-card class="metric-form-card" elevation="8">
              <v-card-title class="text-h6 text-white">
                <v-icon class="mr-2">mdi-plus-circle</v-icon>
                {{ $t('add_measurement') }}
              </v-card-title>
              <v-card-text>
                <v-form @submit.prevent="submitMetric">
                  <v-text-field v-model="form.weight" :label="$t('weight_kg')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.height" :label="$t('height_cm')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.bodyFat" :label="$t('body_fat_percent')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.waist" :label="$t('waist_cm')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.chest" :label="$t('chest_cm')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.arms" :label="$t('arms_cm')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-text-field v-model="form.thighs" :label="$t('thighs_cm')" type="number" variant="outlined"
                    color="primary" class="mb-3" />
                  <v-textarea v-model="form.notes" :label="$t('notes')" variant="outlined" color="primary" rows="2"
                    class="mb-4" />
                  <v-btn type="submit" color="primary" block size="large" :loading="store.loading">
                    {{ $t('save_measurement') }}
                  </v-btn>
                </v-form>
              </v-card-text>
            </v-card>

            <!-- BMI Card -->
            <v-card v-if="latestBMI" class="mt-4 bmi-card" elevation="8">
              <v-card-text class="text-center pa-6">
                <div class="text-overline text-grey-light">{{ $t('bmi_current') }}</div>
                <div class="text-h2 font-weight-bold" :class="bmiColorClass">{{ latestBMI }}</div>
                <div class="text-subtitle-1" :class="bmiColorClass">{{ bmiCategory }}</div>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Gráficos y tabla -->
          <v-col cols="12" md="8">
            <v-card class="chart-card mb-6" elevation="8">
              <v-card-title class="text-white">
                <v-icon class="mr-2">mdi-weight-kilogram</v-icon>
                {{ $t('weight_evolution') }}
              </v-card-title>
              <v-card-text>
                <canvas ref="weightChart" height="250"></canvas>
              </v-card-text>
            </v-card>

            <v-card class="data-table-card" elevation="8">
              <v-card-title class="text-white">
                <v-icon class="mr-2">mdi-history</v-icon>
                {{ $t('measurement_history') }}
              </v-card-title>
              <v-card-text>
                <v-table>
                  <thead>
                    <tr>
                      <th class="text-left">{{ $t('date') }}</th>
                      <th class="text-left">{{ $t('weight') }}</th>
                      <th class="text-left">{{ $t('body_fat') }}</th>
                      <th class="text-left">{{ $t('waist') }}</th>
                      <th class="text-left">{{ $t('actions') }}</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="metric in store.metrics" :key="metric.id">
                      <td>{{ formatDate(metric.date) }}</td>
                      <td>{{ metric.weight }} kg</td>
                      <td>{{ metric.bodyFat }}%</td>
                      <td>{{ metric.waist }} cm</td>
                      <td>
                        <v-btn icon size="small" color="error" @click="deleteMetric(metric.id!)">
                          <v-icon>mdi-delete</v-icon>
                        </v-btn>
                      </td>
                    </tr>
                  </tbody>
                </v-table>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue';
import { useBodyMetricStore } from '@/stores/bodyMetricStore';
import { useI18n } from 'vue-i18n';
import Chart from 'chart.js/auto';

const { t } = useI18n();
const store = useBodyMetricStore();

const form = ref({
  weight: '',
  height: '',
  bodyFat: '',
  waist: '',
  chest: '',
  arms: '',
  thighs: '',
  notes: ''
});

const weightChart = ref<HTMLCanvasElement | null>(null);
let chartInstance: Chart | null = null;

onMounted(async () => {
  await store.fetchMyMetrics();
  await nextTick(); // Esperar a que el DOM se actualice con los datos antes de renderizar el gráfico
  renderChart();
});

watch(() => store.metrics, () => {
  nextTick(() => renderChart());
}, { deep: true });

const latestBMI = computed(() => {
  // Calcular el BMI usando la última medición registrada, para mostrar un resumen rápido al usuario de su estado actual
  const latest = store.metrics[0];
  if (!latest || !latest.weight || !latest.height) return null;
  return store.calculateBMI(latest.weight, latest.height);
});

const bmiCategory = computed(() => {
  // Determinar la categoría de BMI según el valor calculado, para mostrar un texto descriptivo al usuario
  const bmi = latestBMI.value;
  if (!bmi) return '';
  if (bmi < 18.5) return t('bmi.underweight');
  if (bmi < 25) return t('bmi.normalWeight');
  if (bmi < 30) return t('bmi.overweight');
  return t('bmi.obesity');
});

const bmiColorClass = computed(() => {
  // Asignar una clase de color según la categoría de BMI para resaltar visualmente el estado del usuario
  const bmi = latestBMI.value;
  if (!bmi) return '';
  if (bmi < 18.5) return 'bmi-blue';
  if (bmi < 25) return 'bmi-success';
  if (bmi < 30) return 'bmi-orange';
  return 'bmi-error';
});

async function submitMetric() {
  // Al enviar el formulario, se crea un nuevo objeto con los datos enviados
  const metric = {
    date: new Date().toISOString(),
    weight: form.value.weight ? parseFloat(form.value.weight) : undefined,
    height: form.value.height ? parseFloat(form.value.height) : undefined,
    bodyFat: form.value.bodyFat ? parseFloat(form.value.bodyFat) : undefined,
    waist: form.value.waist ? parseFloat(form.value.waist) : undefined,
    chest: form.value.chest ? parseFloat(form.value.chest) : undefined,
    arms: form.value.arms ? parseFloat(form.value.arms) : undefined,
    thighs: form.value.thighs ? parseFloat(form.value.thighs) : undefined,
    notes: form.value.notes || undefined
  };
  await store.addMetric(metric);
  // Al agregar una nueva medición, se limpia el formulario para facilitar la entrada de nuevos datos por parte del usuario
  form.value = { weight: '', height: '', bodyFat: '', waist: '', chest: '', arms: '', thighs: '', notes: '' };
}

async function deleteMetric(id: number) {
  await store.deleteMetric(id);
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString();
}

function renderChart() {
  // Para renderizar el gráfico de evolución del peso, se ordenan las métricas por fecha y se extraen las etiquetas y datos necesarios para construir el gráfico
  if (!weightChart.value) return;
  if (chartInstance) chartInstance.destroy();

  const sorted = [...store.metrics].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
  const labels = sorted.map(m => formatDate(m.date));
  const data = sorted.map(m => m.weight ?? null);
  // Se crea una nueva instancia de Chart.js para mostrar la evolución del peso a lo largo del tiempo, con un diseño personalizado acorde al tema oscuro de la aplicación
  chartInstance = new Chart(weightChart.value, {
    type: 'line',
    data: {
      labels,
      datasets: [{
        label: t('chart.weightKg'),
        data,
        borderColor: '#667eea',
        backgroundColor: 'rgba(102, 126, 234, 0.1)',
        fill: true,
        tension: 0.4,
        pointBackgroundColor: '#667eea',
        pointBorderColor: '#fff',
        pointRadius: 5
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: false }
      },
      scales: {
        x: {
          ticks: { color: '#aaa' },
          grid: { color: 'rgba(255,255,255,0.05)' }
        },
        y: {
          ticks: { color: '#aaa' },
          grid: { color: 'rgba(255,255,255,0.05)' }
        }
      }
    }
  });
}
</script>

<style scoped>
.body-metrics-page {
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

.metric-form-card,
.chart-card,
.data-table-card,
.bmi-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
}

.bmi-card .text-h2 {
  font-size: 3rem;
}

/* Colores para fondo oscuro */
.text-white {
  color: #ffffff !important;
}

.text-grey-light {
  color: rgba(255, 255, 255, 0.75) !important;
}

/* Tabla con texto claro */
:deep(.v-table) {
  background: transparent !important;
}

:deep(.v-table th) {
  color: rgba(255, 255, 255, 0.9) !important;
  font-weight: 700 !important;
  font-size: 0.85rem !important;
}

:deep(.v-table td) {
  color: rgba(255, 255, 255, 0.8) !important;
  font-size: 0.9rem !important;
}

:deep(.v-table tr:hover td) {
  color: #ffffff !important;
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

/* Clases BMI personalizadas */
.bmi-blue {
  color: #60a5fa !important;
}

.bmi-success {
  color: #4ade80 !important;
}

.bmi-orange {
  color: #fb923c !important;
}

.bmi-error {
  color: #f87171 !important;
}
</style>
