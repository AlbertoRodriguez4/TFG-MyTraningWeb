<template>
  <div class="app-root">
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
            <span>{{ $t('coach_ai_personal') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('premium_access') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('nutrition_tips') }}</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>{{ $t('advanced_tracking') }}</span>
          </div>
        </div>
        <div class="pricing-info">
          <span class="price-tag">{{ $t('chat.perMonth') }}</span>
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

    <!-- Chat Interface (solo visible si es premium) -->
    <div v-else>
      <div class="chat-shell">

        <!-- Sidebar -->
        <ChatSidebar :open="sidebarOpen" />

        <!-- Main column -->
        <div class="main">
          <ChatHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />

          <ChatFeed :messages="messages" @send="handleSend" />

          <!-- Banner: se necesitan datos de salud -->
          <div v-if="!hasHealthData" class="health-data-banner">
            <div class="health-data-content">
              <v-icon color="#FFD700" size="24">mdi-calculator-variant</v-icon>
              <div class="health-data-text">
                <span class="health-data-title">{{ $t('chatbot_no_health_data') }}</span>
                <span class="health-data-desc">{{ $t('chatbot_complete_calculator_desc') }}</span>
              </div>
              <v-btn
                color="#FFD700"
                variant="flat"
                size="small"
                class="health-data-btn"
                @click="goToCalculator"
              >
                {{ $t('chatbot_go_calculator') }}
              </v-btn>
            </div>
          </div>

          <ChatComposer :disabled="isLoading || !hasHealthData" :show-quick-prompts="messages.length === 0" @send="handleSend" />
        </div>

      </div>

      <!-- Mobile backdrop: closes sidebar on outside tap -->
      <div class="backdrop" :class="{ 'backdrop--on': sidebarOpen }" @click="sidebarOpen = false" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'
import type { ChatMessage } from '@/components/Models/Chat'
import ChatSidebar from '../../components/CoachAI/ChatSidebar.vue'
import ChatHeader from '../../components/CoachAI/ChatHeader.vue'
import ChatFeed from '../../components/CoachAI/ChatFeed.vue'
import ChatComposer from '../../components/CoachAI/ChatComposer.vue'

const router = useRouter()
const subscriptionStore = useSubscriptionStore()
const { t } = useI18n()

const messages = ref<ChatMessage[]>([])
const isLoading = ref(true)
const sidebarOpen = ref(false)
const isPremium = ref(false)
const hasHealthData = ref(false)


onMounted(async () => {
  try {
    await subscriptionStore.checkSubscription()
    isPremium.value = subscriptionStore.hasActiveSubscription
    checkHealthData()
  } catch (error) {
    logger.error('Error checking premium status:', error)
    isPremium.value = false
  } finally {
    isLoading.value = false
  }
})

function checkHealthData() {
  try {
    // Intentar cargar el perfil de salud del localStorage, si no se pueden cargar o no tienen los datos necesarios, se considera que no hay datos de salud.
    const profile = JSON.parse(localStorage.getItem('userHealthProfile') || '{}')
    // Se requiere al menos peso, altura, bmi y tdee para considerar que hay datos
    hasHealthData.value = !!(profile.weight && profile.height && profile.bmi && profile.tdee)
  } catch {
    hasHealthData.value = false
  }
}

function goToCalculator() {
  router.push('/calculator')
}

function goToPayment() {
  router.push('/payment')
}

function goBack() {
  router.back()
}
// Función para obtener el texto de progreso del logro, se muestra el progreso actual y el requisito total, o "Desbloqueado" si ya se ha desbloqueado el logro.
const now = () =>
  new Date().toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })

  // Función para construir el historial de mensajes en el formato que la API espera, se filtran los mensajes de escritura y se mapean a un array de objetos con role y content.
const buildHistory = () =>
  messages.value
    .filter(m => !m.isTyping)
    .map(m => ({ role: m.role, content: m.text }))

const getUserContext = () => {
  let contextText = "";
  try {
    // Obtenemos los datos de salud del localstorage, si no se pueden cargar o no tienen los datos necesarios, se devuelve un string vacío para no enviar contexto a la IA.
    const rawProfile = localStorage.getItem('userHealthProfile') || '{}';
    const profile = JSON.parse(rawProfile);

    if (!profile.weight || !profile.height || !profile.bmi || !profile.tdee) {
      return ""; // Sin datos suficientes, no enviar contexto
    }

    const activityMap: Record<string, string> = {
      sedentary: t('sedentary'),
      light: t('light'),
      moderate: t('moderate'),
      active: t('active_label'),
      veryactive: t('very_active'),
    };
    const goalMap: Record<string, string> = {
      loss: t('loss'),
      maintenance: t('maintenance'),
      gain: t('gain_label'),
    };
    const genderMap: Record<string, string> = {
      male: t('male'),
      female: t('female'),
    };

    //Construir el texto que leerá la IA
    contextText = `

    --- ${t('physical_info')} ---
    ${t('informacion_fisica_context')}
    - ${t('weight_label')}: ${profile.weight} kg
    - ${t('height_label')}: ${profile.height} cm
    - ${t('age_label')}: ${profile.age || t('chat.noSpecified')} años
    - ${t('gender')}: ${genderMap[profile.gender] || t('chat.noSpecified')}
    - ${t('bmi_label')}: ${profile.bmi.toFixed(2)} (${profile.bmiCategory || ''})
    - ${t('activity')}: ${activityMap[profile.activity] || t('chat.noSpecified')}
    - ${t('goal')}: ${goalMap[profile.goal] || t('chat.noSpecified')}
    - ${t('bmr_label')}: ${profile.bmr || t('chat.notCalculated')} kcal
    - ${t('tdee_label')}: ${profile.tdee} kcal
    - ${t('protein')}: ${profile.macros?.protein || t('chat.notCalculated')}g
    - ${t('carbs')}: ${profile.macros?.carbs || t('chat.notCalculated')}g
    - ${t('fats')}: ${profile.macros?.fat || t('chat.notCalculated')}g
    `;
  } catch (error) {
    logger.warn("No se pudieron cargar los datos físicos del usuario para el chatbot", error);
  }
  return contextText;
}

// Enviar el mensaje del usuario al backend, se construye el historial de mensajes y el contexto del usuario, se hace la petición a la API y se 
// maneja la respuesta o cualquier error que pueda ocurrir. También se maneja un estado de "escribiendo" para mostrar un indicador mientras se espera la respuesta.
async function handleSend(text: string) {
  const trimmed = text?.trim()
  if (!trimmed || isLoading.value) return

  // Add user message
  messages.value.push({ id: Date.now(), role: 'user', text: trimmed, time: now() })

    // Verificar si tiene datos de salud antes de continuar
  checkHealthData()
  if (!hasHealthData.value) {
    const replyId = Date.now() + 1
    messages.value.push({
      id: replyId,
      role: 'assistant',
      text: `${t('chatbot_no_health_data')} \n\n${t('chatbot_complete_calculator_desc')}`,
      time: now()
    })
    return
  }

  // Agregar mensaje de "escribiendo" del asistente
  const typingId = Date.now() + 1
  messages.value.push({ id: typingId, role: 'assistant', text: '', isTyping: true, time: '' })

  try {
    // Obtener el token de autenticación del localStorage, si no hay token se lanza un error para evitar hacer la petición a la API sin autenticación.
    const token = localStorage.getItem('token')
    if (!token) {
      throw new Error(t('no_active_session_coachai'))
    }
    // Hacer la petición a la API con el mensaje del usuario, el historial de mensajes y el contexto del usuario, se espera la respuesta y se 
    // maneja cualquier error que pueda ocurrir.
    const res = await fetch(`${API_BASE_URL}/api/CoachAI/chat`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify({
        mensaje: trimmed,
        historial: buildHistory(),
        contextoUsuario: getUserContext()
      })
    })

    if (!res.ok) {
      const errorData = await res.json().catch(() => ({}))
      throw new Error(errorData?.message || t('could_not_process_message'))
    }

    const data = await res.json()
    const reply = data?.reply || t('could_not_process_message')

    messages.value = messages.value.filter(m => m.id !== typingId)
    messages.value.push({ id: Date.now() + 2, role: 'assistant', text: reply, time: now() })

  } catch (err) {
    logger.error(err)
    messages.value = messages.value.filter(m => m.id !== typingId)
    messages.value.push({
      id: Date.now() + 2,
      role: 'assistant',
      text: t('connection_error_retry'),
      time: now()
    })
  }
}
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700&family=Syne:wght@700;800&display=swap');

*,
*::before,
*::after {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

html,
body,
#app {
  height: 100%;
  font-family: 'DM Sans', 'Segoe UI', system-ui, sans-serif;
}

body {
  background: #f0ebe3;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
}

:root {
  --c-bg: #faf8f5;
  --c-surface: #ffffff;
  --c-sidebar: #1c1917;
  --c-border: #e8e2da;
  --c-text: #1c1917;
  --c-muted: #78716c;
  --c-faint: #a8a29e;
  --c-accent: #e85d26;
  --c-accent2: #f97316;
  --c-user-text: #ffffff;
  --r-sm: 8px;
  --r-md: 14px;
  --r-lg: 20px;
  --r-xl: 26px;
}

@keyframes fadeSlideUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes blink {

  0%,
  80%,
  100% {
    opacity: .25;
    transform: scale(.75);
  }

  40% {
    opacity: 1;
    transform: scale(1);
  }
}
</style>

<style scoped>
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
  color: rgba(0, 0, 0, 0.7);
  font-size: 1.1rem;
}

/* Premium Overlay */
.premium-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, rgba(10, 10, 10, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
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

.app-root {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;

}

.chat-shell {
  display: flex;
  width: min(1040px, 100vw);
  height: min(800px, 100vh);
  background: var(--c-bg);
  border-radius: 28px;
  overflow: hidden;
  box-shadow:
    0 0 0 1px rgba(0, 0, 0, .06),
    0 32px 80px rgba(0, 0, 0, .18),
    0 8px 24px rgba(0, 0, 0, .1);
}

.main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  background: var(--c-surface);
}

/* Mobile backdrop */
.backdrop {
  display: none;
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, .45);
  z-index: 99;
  backdrop-filter: blur(2px);
}

.backdrop--on {
  display: block;
}

@media (min-width: 700px) {
  .backdrop {
    display: none !important;
  }
}

/* Health Data Banner */
.health-data-banner {
  background: linear-gradient(135deg, #1c1917 0%, #2d2a26 100%);
  border-top: 1px solid rgba(255, 215, 0, 0.2);
  padding: 12px 20px;
}

.health-data-content {
  display: flex;
  align-items: center;
  gap: 12px;
  max-width: 720px;
  margin: 0 auto;
}

.health-data-text {
  display: flex;
  flex-direction: column;
  flex: 1;
  gap: 2px;
}

.health-data-title {
  color: #FFD700;
  font-weight: 700;
  font-size: 13px;
}

.health-data-desc {
  color: rgba(255, 255, 255, 0.7);
  font-size: 12px;
  line-height: 1.4;
}

.health-data-btn {
  font-weight: 700;
  font-size: 12px;
  text-transform: none;
  letter-spacing: 0;
  flex-shrink: 0;
}

@media (max-width: 600px) {
  .health-data-content {
    flex-direction: column;
    text-align: center;
    gap: 10px;
  }

  .health-data-text {
    align-items: center;
  }

  .health-data-btn {
    width: 100%;
  }
}
</style>
