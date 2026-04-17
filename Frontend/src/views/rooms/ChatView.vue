<template>
  <div class="app-root">
    <!-- Premium Check Overlay -->
    <div v-if="!isPremium && !isLoading" class="premium-overlay">
      <div class="premium-lock">
        <div class="lock-icon">
          <v-icon size="80" color="#FFD700">mdi-lock-outline</v-icon>
        </div>
        <h2 class="lock-title">Chat Premium</h2>
        <p class="lock-description">
          El Chat con Coach AI es una función exclusiva para usuarios Premium
        </p>
        <div class="premium-benefits">
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>Entrenador personal AI 24/7</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>Rutinas personalizadas</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>Consejos de nutrición</span>
          </div>
          <div class="benefit-item">
            <v-icon size="20" color="#00ff88">mdi-check-circle</v-icon>
            <span>Seguimiento de progreso</span>
          </div>
        </div>
        <div class="pricing-info">
          <span class="price-tag">10€<small>/mes</small></span>
          <p class="price-desc">Cancela cuando quieras</p>
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

    <!-- Loading State -->
    <div v-else-if="isLoading" class="loading-container">
      <v-progress-circular
        size="64"
        color="#FFD700"
        indeterminate
      />
      <p class="loading-text">Verificando suscripción...</p>
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

          <ChatComposer :disabled="isLoading" :show-quick-prompts="messages.length === 0" @send="handleSend" />
        </div>

      </div>

      <!-- Mobile backdrop: closes sidebar on outside tap -->
      <div class="backdrop" :class="{ 'backdrop--on': sidebarOpen }" @click="sidebarOpen = false" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { logger } from '@/utils/logger'
import ChatSidebar from '../../components/CoachAI/ChatSidebar.vue'
import ChatHeader from '../../components/CoachAI/ChatHeader.vue'
import ChatFeed from '../../components/CoachAI/ChatFeed.vue'
import ChatComposer from '../../components/CoachAI/ChatComposer.vue'

const router = useRouter()
const subscriptionStore = useSubscriptionStore()

/* ── State ── */
const messages = ref([])
const isLoading = ref(true)
const sidebarOpen = ref(false)
const isPremium = ref(false)

/* ── Check Premium Status ── */
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

/* ── Helpers ── */
const now = () =>
  new Date().toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })

const buildHistory = () =>
  messages.value
    .filter(m => !m.isTyping)
    .map(m => ({ role: m.role, content: m.text }))

/* ── Extraer Contexto del Usuario ── */
const getUserContext = () => {
  let contextText = "";
  try {
    // 1. Recuperar los datos de memoria (ajusta 'localStorage' si usas Pinia/Vuex)
    const rawBMI = localStorage.getItem('lastBMIResult') || '{}';
    const rawHealth = localStorage.getItem('healthCalcResults') || '[]';

    // 2. Parsear a objetos JS
    const bmiData = JSON.parse(rawBMI);
    const healthData = JSON.parse(rawHealth);

    // 3. Buscar las calorías (TDEE) en el array de resultados
    const caloriesEntry = Array.isArray(healthData)
      ? healthData.find(item => item.type === 'calories')
      : null;

    // 4. Construir el texto que leerá la IA
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

/* ── Send message ── */
async function handleSend(text) {
  const t = text?.trim()
  if (!t || isLoading.value) return

  // Add user message
  messages.value.push({ id: Date.now(), role: 'user', text: t, time: now() })

  // Add typing placeholder
  const typingId = Date.now() + 1
  messages.value.push({ id: typingId, role: 'assistant', isTyping: true, time: '' })

  // Juntar tu prompt original con los datos físicos extraídos
  const systemPrompt = `Eres CoachAI, un entrenador personal y nutricionista virtual experto, amigable y motivador.
Respondes ÚNICAMENTE preguntas sobre actividad física, entrenamiento, nutrición deportiva, recuperación muscular y bienestar físico.
Si alguien pregunta algo fuera de estos temas, redirige amablemente al fitness.
Tono: energético, motivador, cercano. Usa **negritas** para puntos clave.
Listas con "• ". Respuestas concisas pero completas. Siempre en español.` + getUserContext();

  try {
    // Ejemplo usando la API gratuita de Groq (formato estándar OpenAI)
    const res = await fetch('https://api.groq.com/openai/v1/chat/completions', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${import.meta.env.VITE_GROQ_API_KEY}`
      },
      body: JSON.stringify({
        model: 'llama-3.3-70b-versatile', messages: [
          { role: 'system', content: systemPrompt },
          ...buildHistory()
        ],
        max_tokens: 1000,
        temperature: 0.7
      })
    })

    const data = await res.json()
    // La respuesta en Groq/OpenAI viene en choices[0].message.content
    const reply = data.choices?.[0]?.message?.content || 'No pude procesar tu mensaje. Inténtalo de nuevo.'

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
</style>
