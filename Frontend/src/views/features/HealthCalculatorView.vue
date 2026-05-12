<template>
  <div class="health-calc-container">
    <!-- Background Decoration -->
    <div class="bg-decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
    </div>

    <!-- Premium Check Overlay -->
    <div v-if="!isPremium && !isLoading" class="premium-overlay">
      <div class="premium-lock">
        <div class="lock-icon">
          <v-icon size="80" color="#FFD700">mdi-lock-outline</v-icon>
        </div>
        <h2 class="lock-title">{{ $t('premium_feature') }}</h2>
        <p class="lock-description">
          {{ $t('premium_calculators_desc') }}
        </p>
        <div class="premium-benefits">
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('unlimited_imc_calculator') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('custom_calorie_calculator') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('calculation_history') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('chat_coach_ai') }}</span>
          </div>
        </div>
        <div class="pricing-info">
          <span class="price-tag">10€<small>/{{ $t('per_month_short') }}</small></span>
          <p class="price-desc">{{ $t('cancel_anytime') }}</p>
        </div>
        <v-btn
          class="unlock-btn"
          color="#FFD700"
          size="large"
          @click="goToPayment"
        >
          <v-icon start>mdi-lock-check</v-icon>
          {{ $t('unlock_now') }}
        </v-btn>
        <v-btn
          variant="text"
          class="later-btn"
          @click="goBack"
        >
          {{ $t('later') }}
        </v-btn>
      </div>
    </div>

    <!-- Loading State -->
    <div v-else-if="isLoading" class="loading-container">
      <v-progress-circular
        size="64"
        color="#FFD700"
        indeterminate
      />
      <p class="loading-text">{{ $t('verifying_subscription') }}</p>
    </div>

    <!-- Main Content (solo visible si es premium) -->
    <v-container v-else fluid class="main-content px-0">
      <!-- Header -->
      <div class="calc-header">
        <h1 class="main-title">{{ $t('calculadora_salud') }}</h1>
        <p class="main-subtitle">{{ $t('calculadora_salud_subtitle') }}</p>
      </div>

      <!-- Calculator Container -->
      <div class="calculator-wrapper">
        <!-- BMI Section -->
        <BmiCalculatorModern @result-changed="bmiData = $event" @save="saveBMI" />

        <!-- Calories Section -->
        <CaloriesCalculatorModern :bmi-weight="bmiData.weight" :bmi-height="bmiData.height"
          @result-changed="caloriesData = $event" @save="saveCalories" />
      </div>

      <!-- History Section -->
      <HealthHistoryModern :results="allResults" @delete="deleteResult" />
    </v-container>

    <!-- Success Notification -->
    <v-snackbar v-model="showNotification" color="success" timeout="2000" location="top">
      {{ $t(notificationMessage) }}
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

interface UserHealthProfile {
  weight?: number
  height?: number
  bmi?: number
  bmiCategory?: string
  age?: number
  gender?: 'male' | 'female'
  activity?: string
  goal?: string
  bmr?: number
  tdee?: number
  macros?: {
    protein: number
    carbs: number
    fat: number
  }
  lastUpdated?: string
}

// State
const bmiData = ref<BMIData>({ weight: null, height: null })
const caloriesData = ref<CaloriesData>({})
const allResults = ref<HistoryItem[]>([])
const showNotification = ref(false)
const notificationMessage = ref('')

// Datos adicionales del perfil para el chatbot
const userAge = ref<number | null>(null)
const userGender = ref<'male' | 'female'>('male')
const userActivity = ref('moderate')
const userGoal = ref('maintenance')

// Check premium status on mount
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

// Save functions
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
  notificationMessage.value = 'imc_guardado'
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
  notificationMessage.value = 'calculo_guardado'
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
  padding: 2rem 0;
}

/* Background Circles */
.bg-decoration {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
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
  width: 400px;
  height: 400px;
  top: -100px;
  right: -100px;
  animation: float 20s infinite ease-in-out;
}

.circle-2 {
  width: 300px;
  height: 300px;
  bottom: -50px;
  left: -50px;
  animation: float 25s infinite ease-in-out reverse;
}

.circle-3 {
  width: 200px;
  height: 200px;
  top: 50%;
  left: 10%;
  animation: float 30s infinite ease-in-out;
}

@keyframes float {

  0%,
  100% {
    transform: translate(0, 0);
  }

  50% {
    transform: translate(30px, 30px);
  }
}

/* Loading Container */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  gap: 1.5rem;
}

.loading-text {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1.1rem;
}

/* Premium Overlay */
.premium-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  padding: 2rem;
}

.premium-lock {
  max-width: 500px;
  width: 100%;
  text-align: center;
  padding: 3rem 2rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 215, 0, 0.2);
  border-radius: 24px;
  backdrop-filter: blur(10px);
}

.lock-icon {
  margin-bottom: 1.5rem;
  display: inline-flex;
  padding: 1.5rem;
  background: rgba(255, 215, 0, 0.1);
  border-radius: 50%;
}

.lock-title {
  font-size: 2rem;
  font-weight: 900;
  color: #FFD700;
  margin: 0 0 1rem;
  text-transform: uppercase;
  letter-spacing: 2px;
}

.lock-description {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1rem;
  margin: 0 0 2rem;
  line-height: 1.6;
}

.premium-benefits {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
  text-align: left;
  background: rgba(0, 0, 0, 0.3);
  padding: 1.5rem;
  border-radius: 12px;
}

.benefit-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.95rem;
}

.pricing-info {
  margin-bottom: 2rem;
}

.price-tag {
  display: block;
  font-size: 3rem;
  font-weight: 900;
  color: #FFD700;
  letter-spacing: -2px;
}

.price-tag small {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: normal;
}

.price-desc {
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.9rem;
  margin: 0.5rem 0 0;
}

.unlock-btn {
  width: 100%;
  font-weight: 700;
  font-size: 1.1rem;
  padding: 1.25rem;
  margin-bottom: 1rem;
  text-transform: none;
}

.later-btn {
  color: rgba(255, 255, 255, 0.5);
  text-transform: none;
}

.later-btn:hover {
  color: rgba(255, 255, 255, 0.8);
}

/* Main Content */
.main-content {
  position: relative;
  z-index: 1;
  width: 100%;
}

.calc-header {
  text-align: center;
  margin-bottom: 3rem;
}

.main-title {
  font-size: 2.5rem;
  font-weight: 900;
  color: #ffcc00;
  margin: 0 0 0.5rem 0;
  text-shadow: 0 2px 10px rgba(255, 204, 0, 0.3);
}

.main-subtitle {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1.1rem;
  margin: 0;
}

/* Calculator Wrapper */
.calculator-wrapper {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 2rem;
  margin-bottom: 3rem;
}

/* Responsive */
@media (max-width: 1024px) {
  .calculator-wrapper {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .main-title {
    font-size: 1.8rem;
  }
}

@media (max-width: 600px) {
  .calc-header {
    margin-bottom: 2rem;
  }

  .main-title {
    font-size: 1.5rem;
  }

  .main-subtitle {
    font-size: 0.95rem;
  }

  .premium-lock {
    padding: 2rem 1.5rem;
  }

  .lock-title {
    font-size: 1.5rem;
  }

  .price-tag {
    font-size: 2.5rem;
  }
}

/* Snackbar Styling */
:deep(.v-snackbar__content) {
  font-weight: 700;
  font-size: 0.95rem;
}
</style>
