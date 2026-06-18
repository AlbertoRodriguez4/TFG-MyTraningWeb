<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useUserStore } from '@/stores/userStore'
import { logger } from '@/utils/logger'
import { useI18n } from 'vue-i18n'

const router = useRouter()
const route = useRoute()
const store = useUserStore()
const { t } = useI18n()

const verificationCode = ref('')
const verificationEmail = ref('')
const errorMessage = ref('')
const successMessage = ref('')
const isLoading = ref(false)
const isResending = ref(false)
const countdown = ref(0)

// Referencia al intervalo para limpiarlo en onBeforeUnmount
let countdownInterval: number | null = null

// Snackbar states
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('error')

const showSnackbar = (message: string, color: string = 'error') => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

const formatCode = (event: Event) => {
  const value = (event.target as HTMLInputElement | null)?.value || ''
  // Solo permitir números y limitar a 6 dígitos
  const numeric = value.replace(/[^0-9]/g, '')
  if (numeric.length <= 6) {
    verificationCode.value = numeric
  }
}

const verifyEmail = async () => {
  errorMessage.value = ''
  successMessage.value = ''
  // Validar que el código tenga exactamente 6 dígitos antes de llamar al store
  if (verificationCode.value.length !== 6) {
    errorMessage.value = t('code_6_digits')
    showSnackbar(t('please_enter_6'), 'warning')
    return
  }
  // Validar que el email esté presente antes de llamar al store
  if (!verificationEmail.value) {
    errorMessage.value = t('email_not_found_verify')
    showSnackbar(t('email_verif_not_found'), 'error')
    return
  }

  isLoading.value = true

  try {
    const result = await store.verifyEmail(verificationEmail.value, verificationCode.value)

    if (result) {
      successMessage.value = t('email_verified_success')
      showSnackbar(t('email_verified_redirect'), 'success')

      if (store.loggedUser) {
        await store.refreshLoggedUser()
      }
      //Redireccionar después de un breve retraso para mostrar el mensaje y permitir que el store actualice el estado del usuario
      setTimeout(() => {
        router.push({ name: store.loggedUser ? 'homeLogged' : 'login' })
      }, 1500)
    } else {
      errorMessage.value = t('code_invalid')
      showSnackbar(t('code_invalid_retry'), 'error')
    }
  } catch (error) {
    logger.error(error)
    errorMessage.value = t('verify_error')
    showSnackbar(t('server_error'), 'error')
  } finally {
    isLoading.value = false
  }
}

const resendCode = async () => {
  if (countdown.value > 0) return

  isResending.value = true
  errorMessage.value = ''
  // Validar que el email esté presente antes de llamar al store
  try {
    if (!verificationEmail.value) {
      errorMessage.value = t('email_not_found_resend')
      showSnackbar(t('email_verif_not_found'), 'error')
      return
    }

    const result = await store.resendVerificationCode(verificationEmail.value)
    // Mostrar mensaje de éxito o error según el resultado de la solicitud, y en caso de éxito iniciar el contador para limitar reenvíos
    if (result) {
      successMessage.value = t('code_resent')
      showSnackbar(t('code_resent_success'), 'success')
      startCountdown()
    } else {
      errorMessage.value = t('could_not_resend')
      showSnackbar(t('resend_error'), 'error')
    }
  } catch (error) {
    logger.error(error)
    errorMessage.value = t('resend_error_later')
    showSnackbar(t('server_error'), 'error')
  } finally {
    isResending.value = false
  }
}
// Función para iniciar el contador de reenvío, que se llama después de un reenvío exitoso
const startCountdown = () => {
  // Limpiar intervalo anterior si existe
  if (countdownInterval) {
    clearInterval(countdownInterval)
  }

  countdown.value = 60
  countdownInterval = window.setInterval(() => {
    countdown.value--
    if (countdown.value <= 0 && countdownInterval) {
      clearInterval(countdownInterval)
      countdownInterval = null
    }
  }, 1000)
}

onMounted(() => {
  // Obtener el email de la query o del usuario logueado para mostrarlo en la interfaz y usarlo en las llamadas al store
  const emailFromQuery = typeof route.query.email === 'string' ? route.query.email.trim() : ''
  verificationEmail.value = emailFromQuery || store.loggedUser?.email || ''

  if (!verificationEmail.value) {
    showSnackbar(t('indicate_email'), 'warning')
    setTimeout(() => {
      router.push({ name: 'login' })
    }, 1500)
    return
  }

  // Si ya está verificado, redirigir
  if (store.loggedUser && (store.loggedUser as any).isEmailVerified && store.loggedUser.email === verificationEmail.value) {
    showSnackbar(t('email_already_verified'), 'success')
    setTimeout(() => {
      router.push({ name: 'homeLogged' })
    }, 1500)
  }
})

onBeforeUnmount(() => {
  // Limpiar el intervalo si el componente se desmonta
  if (countdownInterval) {
    clearInterval(countdownInterval)
    countdownInterval = null
  }
})
</script>

<template>
  <div class="verify-wrapper">
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>

    <div class="grid-lines"></div>

    <div class="verify-container">
      <div class="header-banner">
        <div class="logo-container">
          <div class="logo-icon">
            <img src="@/assets/imgs/Logo.png" alt="Training Hub" class="main-logo" />
          </div>
        </div>
        <h1 class="hero-title">{{ $t('slogan') }}</h1>
        <p class="hero-subtitle">{{ $t('account_verification') }}</p>
      </div>

      <div class="verify-card">
        <div class="verify-header">
          <div class="verify-icon"><v-icon>mdi-email</v-icon></div>
          <h2 class="verify-title">{{ $t('verify_your_email') }}</h2>
          <p class="verify-subtitle">
            {{ $t('sent_6_digit') }}
            <span class="email-highlight">{{ verificationEmail }}</span>
          </p>
        </div>

        <div v-if="successMessage" class="success-alert">
          <span class="success-icon"><v-icon>mdi-check</v-icon></span>
          <span class="success-text">{{ successMessage }}</span>
        </div>

        <div v-if="errorMessage" class="error-alert">
          <span class="error-icon"><v-icon>mdi-alert</v-icon></span>
          <span class="error-text">{{ errorMessage }}</span>
        </div>

        <div class="code-section">
          <label class="code-label">{{ $t('verification_code_label') }}</label>
          <div class="code-input-container">
            <input
              v-model="verificationCode"
              @input="formatCode"
              type="text"
              class="code-input"
              placeholder="000000"
              maxlength="6"
              :disabled="isLoading"
              autocomplete="one-time-code"
            />
          </div>
          <p class="code-hint">{{ $t('enter_6_digits') }}</p>
        </div>

        <v-btn
          @click="verifyEmail"
          size="x-large"
          class="verify-btn"
          :loading="isLoading"
          :disabled="isLoading || verificationCode.length !== 6"
          block
        >
          <span v-if="!isLoading" class="btn-text">
            <span>{{ $t('verify_email_btn') }}</span>
            <span class="btn-arrow"><v-icon>mdi-arrow-right</v-icon></span>
          </span>
          <span v-else class="btn-loading">
            <v-progress-circular indeterminate size="20" width="2" color="white"></v-progress-circular>
            <span>{{ $t('verifying') }}</span>
          </span>
        </v-btn>

        <div class="resend-section">
          <p class="resend-text">{{ $t('didnt_receive') }}</p>
          <v-btn
            @click="resendCode"
            variant="text"
            size="small"
            class="resend-btn"
            :disabled="isResending || countdown > 0"
            :loading="isResending"
          >
            <span v-if="countdown > 0">
              {{ $t('resend_in') }} {{ countdown }}s
            </span>
            <span v-else>
              {{ $t('resend_code') }}
            </span>
          </v-btn>
        </div>

        <div class="separator">
          <div class="separator-line"></div>
        </div>

        <v-btn
          variant="outlined"
          size="large"
          class="back-btn"
          to="/login"
          block
        >
          {{ $t('back_home') }}
        </v-btn>
      </div>

      <div class="info-grid">
        <div class="info-card">
          <div class="info-icon"><v-icon>mdi-timer</v-icon></div>
          <h3 class="info-title">{{ $t('expires_15min') }}</h3>
          <p class="info-desc">{{ $t('code_valid_15') }}</p>
        </div>

        <div class="info-card">
          <div class="info-icon"><v-icon>mdi-lock</v-icon></div>
          <h3 class="info-title">{{ $t('secure') }}</h3>
          <p class="info-desc">{{ $t('info_protected') }}</p>
        </div>
      </div>
    </div>

    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4000" location="top" rounded="pill">
      <div class="snackbar-content">
        <span class="snackbar-icon">
          <v-icon>{{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}</v-icon>
        </span>
        <span>{{ snackbarMessage }}</span>
      </div>
      <template v-slot:actions>
        <v-btn variant="text" @click="snackbar = false" icon="mdi-close" size="small"></v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.verify-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a0a1a 0%, #1a0a2e 50%, #0f0a1a 100%);
  position: relative;
  overflow-x: hidden;
}

/* Orbes de luz */
.orb {
  position: fixed;
  border-radius: 50%;
  filter: blur(80px);
  pointer-events: none;
  z-index: 1;

}

.orb-1 {
  top: 10%;
  left: 20%;
  width: 400px;
  height: 400px;
  background: rgba(139, 92, 246, 0.15);
}

.orb-2 {
  bottom: 10%;
  right: 20%;
  width: 400px;
  height: 400px;
  background: rgba(34, 211, 238, 0.15);
  animation-delay: 2s;
}

/* Grid lines */
.grid-lines {
  position: fixed;
  inset: 0;
  background-image:
    linear-gradient(rgba(139, 92, 246, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(139, 92, 246, 0.03) 1px, transparent 1px);
  background-size: 50px 50px;
  pointer-events: none;
  z-index: 1;
}

.verify-container {
  position: relative;
  z-index: 2;
  max-width: 600px;
  margin: 0 auto;
  padding: 2rem;
}

/* Header */
.header-banner {
  text-align: center;
  margin-bottom: 2rem;

}

.logo-container {
  margin-bottom: 1.5rem;
}

.logo-icon {
  display: inline-block;
  padding: 1rem;
  background: rgba(139, 92, 246, 0.1);
  border: 1px solid rgba(139, 92, 246, 0.3);
  border-radius: 24px;
  backdrop-filter: blur(10px);
}

.main-logo {
  height: 60px;
  filter: drop-shadow(0 0 20px rgba(139, 92, 246, 0.5));
}

.hero-title {
  font-size: 2rem;
  font-weight: 700;
  background: linear-gradient(135deg, #a78bfa, #22d3ee);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 0.5rem;
}

.hero-subtitle {
  font-size: 0.75rem;
  color: #64748b;
  letter-spacing: 3px;
  font-weight: 600;
}

/* Verify Card */
.verify-card {
  background: rgba(15, 15, 30, 0.6);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 24px;
  padding: 2.5rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);

}

.verify-header {
  text-align: center;
  margin-bottom: 2rem;
}

.verify-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.verify-title {
  font-size: 1.75rem;
  color: #f8fafc;
  font-weight: 700;
  margin-bottom: 0.75rem;
}

.verify-subtitle {
  color: #94a3b8;
  font-size: 0.95rem;
  line-height: 1.6;
}

.email-highlight {
  color: #a78bfa;
  font-weight: 600;
  display: block;
  margin-top: 0.5rem;
  word-break: break-all;
}

/* Success Alert */
.success-alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(34, 197, 94, 0.1);
  border: 1px solid rgba(34, 197, 94, 0.3);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1.5rem;

}

.success-icon {
  font-size: 1.5rem;
  color: #22c55e;
}

.success-text {
  color: #86efac;
  font-size: 0.9rem;
}

/* Error Alert */
.error-alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1.5rem;

}

.error-icon {
  font-size: 1.5rem;
}

.error-text {
  color: #fca5a5;
  font-size: 0.9rem;
}

/* Code Section */
.code-section {
  margin-bottom: 1.5rem;
}

.code-label {
  display: block;
  font-size: 0.7rem;
  color: #64748b;
  font-weight: 600;
  letter-spacing: 2px;
  margin-bottom: 0.75rem;
  text-align: center;
}

.code-input-container {
  display: flex;
  justify-content: center;
  margin-bottom: 0.75rem;
}

.code-input {
  width: 100%;
  max-width: 280px;
  padding: 1rem;
  font-size: 2rem;
  font-weight: 700;
  text-align: center;
  letter-spacing: 1rem;
  background: rgba(0, 0, 0, 0.3);
  border: 2px solid rgba(139, 92, 246, 0.3);
  border-radius: 16px;
  color: #f8fafc;
  transition: all 0.3s ease;
  outline: none;
}

.code-input:focus {
  border-color: #a78bfa;
  box-shadow: 0 0 0 4px rgba(139, 92, 246, 0.15);
  background: rgba(0, 0, 0, 0.4);
}

.code-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.code-input::placeholder {
  letter-spacing: 0.5rem;
  color: #475569;
}

.code-hint {
  text-align: center;
  color: #64748b;
  font-size: 0.85rem;
}

/* Verify Button */
.verify-btn {
  background: linear-gradient(135deg, #8b5cf6, #22d3ee) !important;
  color: white !important;
  font-weight: 600 !important;
  border-radius: 12px !important;
  text-transform: none !important;
  letter-spacing: 0.5px !important;
  margin-bottom: 1.5rem;
  transition: all 0.3s ease !important;
  box-shadow: 0 8px 24px rgba(139, 92, 246, 0.4) !important;
}

.verify-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 12px 32px rgba(139, 92, 246, 0.6) !important;
}

.btn-text {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.btn-arrow {
  font-size: 1.2rem;
  transition: transform 0.3s ease;
}

.verify-btn:hover:not(:disabled) .btn-arrow {
  transform: translateX(5px);
}

.btn-loading {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* Resend Section */
.resend-section {
  text-align: center;
  margin-bottom: 1.5rem;
}

.resend-text {
  color: #64748b;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.resend-btn {
  color: #a78bfa !important;
  font-weight: 600 !important;
  text-transform: none !important;
}

.resend-btn:disabled {
  opacity: 0.5;
}

/* Separator */
.separator {
  margin: 1.5rem 0;
}

.separator-line {
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(139, 92, 246, 0.3), transparent);
}

/* Back Button */
.back-btn {
  border-color: rgba(139, 92, 246, 0.3) !important;
  color: #a78bfa !important;
  font-weight: 600 !important;
  border-radius: 12px !important;
  text-transform: none !important;
  transition: all 0.3s ease !important;
}

.back-btn:hover {
  border-color: #a78bfa !important;
  background: rgba(139, 92, 246, 0.1) !important;
  transform: translateY(-2px);
}

/* Info Grid */
.info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
  margin-top: 2rem;

}

.info-card {
  background: rgba(15, 15, 30, 0.4);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 92, 246, 0.1);
  border-radius: 16px;
  padding: 1.5rem;
  text-align: center;
  transition: all 0.3s ease;
}

.info-card .info-icon {
  font-size: 2rem;
  margin-bottom: 0.75rem;
}

.info-title {
  color: #a78bfa;
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.info-desc {
  color: #94a3b8;
  font-size: 0.85rem;
  line-height: 1.5;
}

/* Responsive */
@media (max-width: 640px) {
  .verify-container {
    padding: 1rem;
  }

  .verify-card {
    padding: 2rem 1.5rem;
  }

  .hero-title {
    font-size: 1.5rem;
  }

  .verify-title {
    font-size: 1.5rem;
  }

  .code-input {
    font-size: 1.5rem;
    letter-spacing: 0.5rem;
    padding: 0.75rem;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }
}
</style>
