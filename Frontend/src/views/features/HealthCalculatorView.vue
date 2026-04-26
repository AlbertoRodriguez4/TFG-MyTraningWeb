<template>
  <div class="health-calc-container">
    <div class="bg-decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
    </div>

    <div v-if="!isPremium && !isLoading" class="premium-overlay">
      <div class="premium-lock">
        <div class="lock-icon">
          <v-icon size="64" color="#FFD700">mdi-lock-outline</v-icon>
        </div>
        <h2 class="lock-title">Función Premium</h2>
        <p class="lock-description">
          Las calculadoras de salud son una función exclusiva para usuarios Premium
        </p>
        <div class="premium-benefits">
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Calculadora de IMC ilimitada</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Calculadora de calorías personalizada</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Historial de tus cálculos</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Acceso al Chat con Coach AI</span>
          </div>
        </div>
        <v-btn
          class="unlock-btn"
          color="#FFD700"
          size="large"
          @click="goToPayment"
        >
          <v-icon start>mdi-lock-check</v-icon>
          Desbloquear ahora
        </v-btn>
        <v-btn
          variant="text"
          class="later-btn"
          @click="goBack"
        >
          Más tarde
        </v-btn>
      </div>
    </div>

    <div v-else-if="isLoading" class="loading-container">
      <v-progress-circular size="48" color="#FFD700" indeterminate />
      <p class="loading-text">Verificando suscripción...</p>
    </div>

    <v-container v-else fluid class="main-content px-0">
      <div class="calc-header">
        <h1 class="main-title">Calculadora de Salud</h1>
        <p class="main-subtitle">Descubre tu IMC y calorías personalizadas</p>
      </div>

      <div class="calculator-wrapper">
        <BmiCalculatorModern @result-changed="bmiData = $event" @save="saveBMI" />
        <CaloriesCalculatorModern :bmi-weight="bmiData.weight" :bmi-height="bmiData.height"
          @result-changed="caloriesData = $event" @save="saveCalories" />
      </div>

      <HealthHistoryModern :results="allResults" @delete="deleteResult" />
    </v-container>

    <v-snackbar v-model="showNotification" color="success" timeout="2000" location="top">
      {{ notificationMessage }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { logger } from '@/utils/logger'
import BmiCalculatorModern from '../../components/CalculadorCalorias/BmiCalculatorModern.vue'
import CaloriesCalculatorModern from '../../components/CalculadorCalorias/CaloriesCalculatorModern.vue'
import HealthHistoryModern from '../../components/CalculadorCalorias/HealthHistoryModern.vue'

const router = useRouter()
const subscriptionStore = useSubscriptionStore()
const isPremium = ref(false)
const isLoading = ref(true)

interface HistoryItem {
  type: 'bmi' | 'calories'
  date: string
  bmi?: number
  tdee?: number
  weight?: number
  height?: number
}

interface BMIData {
  weight: number | null
  height: number | null
  bmi?: number
  category?: string
}

interface CaloriesData {
  bmr?: number
  tdee?: number
  macros?: {
    protein: number
    carbs: number
    fat: number
  }
}

const bmiData = ref<BMIData>({ weight: null, height: null })
const caloriesData = ref<CaloriesData>({})
const allResults = ref<HistoryItem[]>([])
const showNotification = ref(false)
const notificationMessage = ref('')

onMounted(async () => {
  await checkPremiumStatus()
  loadAllResults()
})

async function checkPremiumStatus() {
  isLoading.value = true
  try {
    await subscriptionStore.checkSubscription()
    isPremium.value = subscriptionStore.hasActiveSubscription
  } catch (error) {
    logger.error('Error checking premium status:', error)
    isPremium.value = false
  } finally {
    isLoading.value = false
  }
}

function goToPayment() {
  router.push('/payment')
}

function goBack() {
  router.back()
}

const saveBMI = () => {
  if (!bmiData.value.bmi) return
  const result: HistoryItem = {
    type: 'bmi',
    date: new Date().toISOString(),
    bmi: bmiData.value.bmi,
    weight: bmiData.value.weight ?? undefined,
    height: bmiData.value.height ?? undefined,
  }
  allResults.value.unshift(result)
  saveToLocalStorage()
  notificationMessage.value = 'IMC guardado'
  showNotification.value = true
}

const saveCalories = () => {
  if (!caloriesData.value.tdee) return
  const result: HistoryItem = {
    type: 'calories',
    date: new Date().toISOString(),
    tdee: caloriesData.value.tdee,
  }
  allResults.value.unshift(result)
  saveToLocalStorage()
  notificationMessage.value = 'Cálculo guardado'
  showNotification.value = true
}

const deleteResult = (index: number) => {
  allResults.value.splice(index, 1)
  saveToLocalStorage()
}

const saveToLocalStorage = () => {
  localStorage.setItem('healthCalcResults', JSON.stringify(allResults.value))
}

const loadAllResults = () => {
  const stored = localStorage.getItem('healthCalcResults')
  if (stored) {
    try {
      allResults.value = JSON.parse(stored)
      allResults.value.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
    } catch (e) {
      logger.error('Error loading results:', e)
    }
  }
}
</script>

<style scoped>
.health-calc-container {
  min-height: 100vh;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
  position: relative;
  overflow: hidden;
  padding: 1.5rem 0;
}

.bg-decoration {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
  pointer-events: none;
  z-index: 0;
}

.circle {
  position: absolute;
  border-radius: 50%;
  opacity: 0.03;
  border: 2px solid #ffcc00;
}

.circle-1 {
  width: 350px; height: 350px;
  top: -80px; right: -80px;
}

.circle-2 {
  width: 250px; height: 250px;
  bottom: -40px; left: -40px;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  gap: 1.25rem;
}

.loading-text {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1rem;
}

.premium-overlay {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  padding: 2rem;
}

.premium-lock {
  max-width: 440px;
  width: 100%;
  text-align: center;
  padding: 2.5rem 1.75rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 215, 0, 0.18);
  border-radius: 20px;
  backdrop-filter: blur(10px);
}

.lock-icon {
  margin-bottom: 1.25rem;
  display: inline-flex;
  padding: 1.25rem;
  background: rgba(255, 215, 0, 0.08);
  border-radius: 50%;
}

.lock-title {
  font-size: 1.75rem;
  font-weight: 900;
  color: #FFD700;
  margin: 0 0 0.75rem;
  text-transform: uppercase;
  letter-spacing: 2px;
}

.lock-description {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.95rem;
  margin: 0 0 1.5rem;
  line-height: 1.5;
}

.premium-benefits {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
  text-align: left;
  background: rgba(0, 0, 0, 0.25);
  padding: 1.25rem;
  border-radius: 10px;
}

.benefit-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  color: rgba(255, 255, 255, 0.85);
  font-size: 0.9rem;
}

.unlock-btn {
  width: 100%;
  font-weight: 700;
  font-size: 1rem;
  padding: 1rem;
  margin-bottom: 0.75rem;
  text-transform: none;
}

.later-btn {
  color: rgba(255, 255, 255, 0.5);
  text-transform: none;
}

.later-btn:hover {
  color: rgba(255, 255, 255, 0.8);
}

.main-content {
  position: relative;
  z-index: 1;
  width: 100%;
}

.calc-header {
  text-align: center;
  margin-bottom: 2rem;
}

.main-title {
  font-size: 2rem;
  font-weight: 900;
  color: #ffcc00;
  margin: 0 0 0.4rem 0;
  text-shadow: 0 2px 8px rgba(255, 204, 0, 0.25);
}

.main-subtitle {
  color: rgba(255, 255, 255, 0.6);
  font-size: 1rem;
  margin: 0;
}

.calculator-wrapper {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (max-width: 1024px) {
  .calculator-wrapper {
    grid-template-columns: 1fr;
    gap: 1.25rem;
  }
  .main-title { font-size: 1.6rem; }
}

@media (max-width: 600px) {
  .calc-header { margin-bottom: 1.5rem; }
  .main-title { font-size: 1.35rem; }
  .main-subtitle { font-size: 0.9rem; }
  .premium-lock { padding: 2rem 1.25rem; }
  .lock-title { font-size: 1.4rem; }
}

:deep(.v-snackbar__content) {
  font-weight: 700;
  font-size: 0.9rem;
}
</style>
