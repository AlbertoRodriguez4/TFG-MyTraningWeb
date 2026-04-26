<template>
  <div class="app-root">
    <div v-if="!isPremium && !isLoading" class="premium-overlay">
      <div class="premium-lock">
        <div class="lock-icon">
          <v-icon size="64" color="#FFD700">mdi-lock-outline</v-icon>
        </div>
        <h2 class="lock-title">Chat Premium</h2>
        <p class="lock-description">
          El Chat con Coach AI es una función exclusiva para usuarios Premium
        </p>
        <div class="premium-benefits">
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Entrenador personal AI 24/7</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Rutinas personalizadas</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Consejos de nutrición</span>
          </div>
          <div class="benefit-item">
            <v-icon size="18" color="#00ff88">mdi-check-circle</v-icon>
            <span>Seguimiento de progreso</span>
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

    <div v-else>
      <div class="chat-shell">
        <ChatSidebar :open="sidebarOpen" />
        <div class="main">
          <ChatHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />
          <ChatFeed :messages="messages" @send="handleSend" />
          <ChatComposer :disabled="isLoading" :show-quick-prompts="messages.length === 0" @send="handleSend" />
        </div>
      </div>
      <div class="backdrop" :class="{ 'backdrop--on': sidebarOpen }" @click="sidebarOpen = false" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'
import ChatSidebar from '../../components/CoachAI/ChatSidebar.vue'
import ChatHeader from '../../components/CoachAI/ChatHeader.vue'
import ChatFeed from '../../components/CoachAI/ChatFeed.vue'
import ChatComposer from '../../components/CoachAI/ChatComposer.vue'

const router = useRouter()
const subscriptionStore = useSubscriptionStore()

const messages = ref([])
const isLoading = ref(true)
const sidebarOpen = ref(false)
const isPremium = ref(false)

onMounted(async () => {
  try {
    await subscriptionStore.checkSubscription()
    isPremium.value = subscriptionStore.hasActiveSubscription
  } catch (error) {
    logger.error('Error checking premium status:', error)
    isPremium.value = false
  } finally {
    isLoading.value = false
  }
})

function goToPayment() {
  router.push('/payment')
}

function goBack() {
  router.back()
}

const now = () =>
  new Date().toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })

const buildHistory = () =>
  messages.value
    .filter(m => !m.isTyping)
    .map(m => ({ role: m.role, content: m.text }))

const getUserContext = () => {
  let contextText = "";
  try {
    const rawBMI = localStorage.getItem('lastBMIResult') || '{}';
    const rawHealth = localStorage.getItem('healthCalcResults') || '[]';
    const bmiData = JSON.parse(rawBMI);
    const healthData = JSON.parse(rawHealth);
    const caloriesEntry = Array.isArray(healthData)
      ? healthData.find(item => item.type === 'calories')
      : null;
    contextText = `
    --- INFORMACIÓN FÍSICA DEL USUARIO ---
    Ten en cuenta estos datos actuales del usuario para personalizar tus respuestas:
    - Peso: ${bmiData.weight || 'No especificado'} kg
    - Altura: ${bmiData.height || 'No especificada'} cm
    - IMC: ${bmiData.bmi ? bmiData.bmi.toFixed(2) : 'No especificado'} (${bmiData.category || ''})
    - Calorías de mantenimiento (TDEE): ${caloriesEntry?.tdee || 'No especificadas'} kcal.
    `;
  } catch (error) {
    logger.warn("No se pudieron cargar los datos físicos del usuario para el chatbot", error);
  }
  return contextText;
}

async function handleSend(text) {
  const t = text?.trim()
  if (!t || isLoading.value) return
  messages.value.push({ id: Date.now(), role: 'user', text: t, time: now() })
  const typingId = Date.now() + 1
  messages.value.push({ id: typingId, role: 'assistant', isTyping: true, time: '' })
  try {
    const token = localStorage.getItem('token')
    if (!token) {
      throw new Error('No hay sesión activa para usar CoachAI.')
    }
    const res = await fetch(`${API_BASE_URL}/api/CoachAI/chat`, {
      method: 'POST',
      headers: getAuthHeaders(token),
      body: JSON.stringify({
        mensaje: t,
        historial: buildHistory(),
        contextoUsuario: getUserContext()
      })
    })
    if (!res.ok) {
      const errorData = await res.json().catch(() => ({}))
      throw new Error(errorData?.message || 'No se pudo obtener respuesta del CoachAI.')
    }
    const data = await res.json()
    const reply = data?.reply || 'No pude procesar tu mensaje. Inténtalo de nuevo.'
    messages.value = messages.value.filter(m => m.id !== typingId)
    messages.value.push({ id: Date.now() + 2, role: 'assistant', text: reply, time: now() })
  } catch (err) {
    logger.error(err)
    messages.value = messages.value.filter(m => m.id !== typingId)
    messages.value.push({
      id: Date.now() + 2,
      role: 'assistant',
      text: 'Error de conexión. Verifica tu configuración e inténtalo de nuevo.',
      time: now()
    })
  }
}
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700&family=Syne:wght@700;800&display=swap');

*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
html, body, #app { height: 100%; font-family: 'DM Sans', 'Segoe UI', system-ui, sans-serif; }
body { background: #f0ebe3; display: flex; align-items: center; justify-content: center; min-height: 100vh; }

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
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes blink {
  0%, 80%, 100% { opacity: .25; transform: scale(.75); }
  40% { opacity: 1; transform: scale(1); }
}
</style>

<style scoped>
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  gap: 1.25rem;
}

.loading-text {
  color: rgba(0, 0, 0, 0.7);
  font-size: 1rem;
}

.premium-overlay {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
  background: linear-gradient(135deg, rgba(10, 10, 10, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  padding: 2rem;
}

.premium-lock {
  max-width: 440px;
  width: 100%;
  text-align: center;
  padding: 2.5rem 1.75rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 215, 0, 0.2);
  border-radius: 20px;
  backdrop-filter: blur(10px);
}

.lock-icon {
  margin-bottom: 1.25rem;
  display: inline-flex;
  padding: 1.25rem;
  background: rgba(255, 215, 0, 0.1);
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
  background: rgba(0, 0, 0, 0.3);
  padding: 1.25rem;
  border-radius: 10px;
}

.benefit-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  color: rgba(255, 255, 255, 0.9);
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

.backdrop {
  display: none;
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, .45);
  z-index: 99;
  backdrop-filter: blur(2px);
}

.backdrop--on { display: block; }

@media (min-width: 700px) {
  .backdrop { display: none !important; }
}
</style>
