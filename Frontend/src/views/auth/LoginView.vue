<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/userStore'
import { useRouter } from 'vue-router'
import { useAuthValidation } from '@/composables/useAuthValidation'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const REMEMBER_EMAIL_KEY = 'remember_email'
const REMEMBER_FLAG_KEY = 'remember_me'

const email = ref('')
const password = ref('')
const errorMessage = ref('')
const isLoading = ref(false)
const showPassword = ref(false)
const rememberMe = ref(false)

const store = useUserStore()
const router = useRouter()
const { validateEmail, validatePassword, clearErrors, errors } = useAuthValidation()

// Snackbar
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('error')

const showSnackbar = (message: string, color = 'error') => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

//  Al montar: recuperar email guardado 
onMounted(() => {
  const savedFlag = localStorage.getItem(REMEMBER_FLAG_KEY)
  const savedEmail = localStorage.getItem(REMEMBER_EMAIL_KEY)

  if (savedFlag === 'true' && savedEmail) {
    rememberMe.value = true
    email.value = savedEmail
  }
})

const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value
}

//  Guardar / limpiar email según la preferencia 
function applyRememberMe(emailValue: string) {
  if (rememberMe.value) {
    localStorage.setItem(REMEMBER_FLAG_KEY, 'true')
    localStorage.setItem(REMEMBER_EMAIL_KEY, emailValue)
  } else {
    localStorage.removeItem(REMEMBER_FLAG_KEY)
    localStorage.removeItem(REMEMBER_EMAIL_KEY)
  }
}

// Función principal de login
async function handleLogin() {
  errorMessage.value = ''
  isLoading.value = true
  clearErrors()

  const emailTrimmed = email.value.trim()
  const passwordTrimmed = password.value.trim()
  // Validaciones básicas antes de llamar al store
  if (!validateEmail(emailTrimmed)) {
    showSnackbar(errors.email || t('invalid_email'), 'warning')
    isLoading.value = false
    return
  }

  if (!validatePassword(passwordTrimmed)) {
    showSnackbar(errors.password || t('invalid_password'), 'warning')
    isLoading.value = false
    return
  }

  try {
    const loginResult = await store.loginUser(emailTrimmed, passwordTrimmed)

    if (loginResult.success && store.loggedUser?.email === emailTrimmed) {
      applyRememberMe(emailTrimmed)

      showSnackbar(t('welcome_back_train'), 'success')
      setTimeout(() => { router.push('/homeLogged') }, 1500)
    } else if (loginResult.requiresEmailVerification) {
      showSnackbar(loginResult.message || t('must_verify_email'), 'warning')
      setTimeout(() => {
        router.push({ name: 'verifyEmail', query: { email: emailTrimmed } })
      }, 1200)
    } else {
      errorMessage.value = t('invalid_credentials')
      showSnackbar(t('invalid_credentials_check'), 'error')
      isLoading.value = false
    }
  } catch (error) {
    console.error('Login failed:', error)
    errorMessage.value = t('unexpected_error')
    showSnackbar(t('server_error'), 'error')
    isLoading.value = false
  }
}
</script>

<template>
  <div class="login-wrapper">
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>
    <div class="grid-lines"></div>

    <div class="login-container">
      <!-- Header -->
      <div class="header-banner">
        <div class="logo-container">
          <div class="logo-icon">
            <img src="@/assets/imgs/Logo.png" :alt="$t('title')" class="main-logo" />
          </div>
        </div>
        <h1 class="hero-title">{{ $t('slogan') }}</h1>
        <p class="hero-subtitle">{{ $t('hero_subtitle_login') }}</p>
      </div>

      <!-- Content Grid -->
      <div class="content-grid">
        <!-- Form Panel -->
        <div class="form-panel">
          <div class="form-card">
            <div class="form-header">
              <div class="status-badge">
                <span class="badge-dot"></span>
                <span class="badge-text">{{ $t('system_access') }}</span>
              </div>
              <h2 class="form-title">{{ $t('title_login') }}</h2>
              <p class="form-subtitle">{{ $t('resume_training') }}</p>
            </div>

            <v-form @submit.prevent="handleLogin" class="login-form">
              <div class="inputs-container">
                <!-- Email -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('email_label') }}</label>
                  <v-text-field v-model="email" type="email" :placeholder="$t('placeholder_email')"
                    variant="solo-filled" density="comfortable" color="purple-lighten-2" class="custom-input"
                    hide-details="auto" autocomplete="email" />
                </div>

                <!-- Contraseña -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('password_label') }}</label>
                  <v-text-field v-model="password" :type="showPassword ? 'text' : 'password'"
                    :placeholder="$t('placeholder_password')" variant="solo-filled" density="comfortable"
                    color="purple-lighten-2" class="custom-input password-input" hide-details="auto"
                    autocomplete="current-password">
                    <template #append-inner>
                      <v-btn @click="togglePasswordVisibility" icon size="small" variant="text"
                        class="password-toggle-btn">
                        <span class="toggle-icon"><v-icon>{{ showPassword ? 'mdi-eye' : 'mdi-eye-off' }}</v-icon></span>
                      </v-btn>
                    </template>
                  </v-text-field>
                </div>
              </div>

              <!-- ── Recordar sesión ────────────────────────────── -->
              <div class="remember-row">
                <button type="button" class="remember-toggle" :class="{ checked: rememberMe }"
                  @click="rememberMe = !rememberMe" :aria-checked="rememberMe" role="checkbox">
                  <span class="remember-box">
                    <span v-if="rememberMe" class="remember-check"><v-icon>mdi-check</v-icon></span>
                  </span>
                  <span class="remember-label">{{ $t('remember_email') }}</span>
                </button>

                <!-- Indicador visual cuando está activo -->
                <div v-if="rememberMe" class="remember-active">
                  <span class="ra-dot"></span>
                  <span class="ra-text">{{ $t('will_save_on_login') }}</span>
                </div>
              </div>
              <!-- ───────────────────────────────────────────────── -->

              <!-- Quick Info -->
              <div class="quick-info">
                <div class="info-item">
                  <span class="info-icon"><v-icon>mdi-lightning-bolt</v-icon></span>
                  <span class="info-text">{{ $t('instant_stats_access') }}</span>
                </div>
                <div class="info-item">
                  <span class="info-icon"><v-icon>mdi-target</v-icon></span>
                  <span class="info-text">{{ $t('resume_workouts') }}</span>
                </div>
              </div>

              <!-- Error -->
              <div v-if="errorMessage" class="error-alert">
                <span class="error-icon"><v-icon>mdi-alert</v-icon></span>
                <span class="error-text">{{ errorMessage }}</span>
              </div>

              <!-- Submit -->
              <v-btn type="submit" size="x-large" class="submit-btn" :loading="isLoading" :disabled="isLoading" block>
                <span v-if="!isLoading" class="btn-text">
                  <span>{{ $t('login_button') }}</span>
                  <span class="btn-arrow"><v-icon>mdi-arrow-right</v-icon></span>
                </span>
                <span v-else class="btn-loading">
                  <v-progress-circular indeterminate size="20" width="2" color="white" />
                  <span>{{ $t('verifying') }}</span>
                </span>
              </v-btn>

              <!-- Separator -->
              <div class="separator">
                <div class="separator-line"></div>
                <span class="separator-text">{{ $t('first_time') }}</span>
                <div class="separator-line"></div>
              </div>

              <!-- Register -->
              <div class="register-section">
                <p class="register-text">{{ $t('no_account') }}</p>
                <v-btn variant="outlined" size="large" class="register-btn" to="/register" block>
                  {{ $t('register_button') }}
                </v-btn>
              </div>
            </v-form>
          </div>
        </div>

        <!-- Info Panel -->
        <div class="info-panel">
          <div class="welcome-card">
            <div class="welcome-icon"><v-icon>mdi-hand-wave</v-icon></div>
            <h3 class="welcome-title">{{ $t('welcome_back') }}</h3>
            <p class="welcome-desc">{{ $t('continue_journey') }}</p>
          </div>

          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-controller-classic</v-icon></div>
            <h3 class="info-title">{{ $t('gamified_system') }}</h3>
            <p class="info-desc">{{ $t('gain_xp') }}</p>
          </div>

          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-account-group</v-icon></div>
            <h3 class="info-title">{{ $t('train_group') }}</h3>
            <p class="info-desc">{{ $t('join_rooms') }}</p>
          </div>

          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-trophy</v-icon></div>
            <h3 class="info-title">{{ $t('epic_challenges') }}</h3>
            <p class="info-desc">{{ $t('daily_challenges') }}</p>
          </div>

          <div class="progress-card">
            <div class="progress-label">{{ $t('progression_system') }}</div>
            <div class="progress-items">
              <div class="progress-item">
                <span class="progress-icon"><v-icon>mdi-lightning-bolt</v-icon></span>
                <span class="progress-name">{{ $t('levels') }}</span>
              </div>
              <div class="progress-item">
                <span class="progress-icon"><v-icon>mdi-coin</v-icon></span>
                <span class="progress-name">{{ $t('coins') }}</span>
              </div>
              <div class="progress-item">
                <span class="progress-icon"><v-icon>mdi-arm-flex</v-icon></span>
                <span class="progress-name">{{ $t('attributes') }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Snackbar -->
    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4000" location="top" rounded="pill">
      <div class="snackbar-content">
        <span class="snackbar-icon">
          <v-icon>{{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}</v-icon>
        </span>
        <span>{{ snackbarMessage }}</span>
      </div>
      <template #actions>
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

.login-wrapper {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a0a1a 0%, #1a0a2e 50%, #0f0a1a 100%);
  position: relative;
  overflow-x: hidden;
}

.orb {
  position: fixed;
  border-radius: 50%;
  filter: blur(80px);
  pointer-events: none;
  z-index: 1;
  animation: pulse 4s ease-in-out infinite;
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

@keyframes pulse {

  0%,
  100% {
    opacity: 0.5;
    transform: scale(1);
  }

  50% {
    opacity: 0.8;
    transform: scale(1.1);
  }
}

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

.login-container {
  position: relative;
  z-index: 2;
  max-width: 1400px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

/* Header */
.header-banner {
  text-align: center;
  margin-bottom: 3rem;
  animation: fadeIn 0.8s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
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
  height: 80px;
  filter: drop-shadow(0 0 20px rgba(139, 92, 246, 0.5));
}

.hero-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: linear-gradient(135deg, #a78bfa, #22d3ee);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 0.5rem;
  letter-spacing: -0.5px;
}

.hero-subtitle {
  font-size: 0.75rem;
  color: #64748b;
  letter-spacing: 3px;
  font-weight: 600;
}

/* Content Grid */
.content-grid {
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  gap: 2rem;
  align-items: start;
}

.form-panel {
  animation: slideUp 0.8s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.form-card {
  background: rgba(15, 15, 30, 0.6);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 24px;
  padding: 3rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
}

.form-header {
  margin-bottom: 2rem;
  text-align: center;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(139, 92, 246, 0.1);
  border: 1px solid rgba(139, 92, 246, 0.3);
  padding: 0.5rem 1rem;
  border-radius: 50px;
  margin-bottom: 1.5rem;
}

.badge-dot {
  width: 8px;
  height: 8px;
  background: #22d3ee;
  border-radius: 50%;
  animation: blink 2s ease-in-out infinite;
}

@keyframes blink {

  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.3;
  }
}

.badge-text {
  font-size: 0.7rem;
  color: #a78bfa;
  font-weight: 600;
  letter-spacing: 2px;
}

.form-title {
  font-size: 2rem;
  color: #f8fafc;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.form-subtitle {
  color: #64748b;
  font-size: 0.95rem;
}

/* Inputs */
.inputs-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.input-wrapper {
  position: relative;
}

.input-label {
  display: block;
  font-size: 0.7rem;
  color: #64748b;
  font-weight: 600;
  letter-spacing: 1.5px;
  margin-bottom: 0.5rem;
  padding-left: 0.25rem;
}

:deep(.custom-input .v-field) {
  background: rgba(0, 0, 0, 0.3) !important;
  border: 1px solid rgba(139, 92, 246, 0.2) !important;
  border-radius: 12px !important;
  color: #f8fafc !important;
  transition: all 0.3s ease !important;
}

:deep(.custom-input .v-field:hover) {
  border-color: rgba(139, 92, 246, 0.4) !important;
  background: rgba(0, 0, 0, 0.4) !important;
}

:deep(.custom-input .v-field--focused) {
  border-color: #a78bfa !important;
  box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1) !important;
  background: rgba(0, 0, 0, 0.4) !important;
}

:deep(.custom-input input) {
  color: #f8fafc !important;
}

:deep(.custom-input input::placeholder) {
  color: #475569 !important;
}

.password-toggle-btn {
  color: #64748b !important;
  transition: all 0.3s ease !important;
}

.password-toggle-btn:hover {
  color: #a78bfa !important;
  background: rgba(139, 92, 246, 0.1) !important;
}

.toggle-icon {
  font-size: 1.2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.3s ease;
}

.password-toggle-btn:hover .toggle-icon {
  transform: scale(1.1);
}

:deep(.password-input .v-field__append-inner) {
  padding-top: 0 !important;
  align-items: center !important;
}

/* ═══════════════════════════════════════════
   RECORDAR SESIÓN
   ═══════════════════════════════════════════ */
.remember-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1.25rem;
  padding: 0 0.25rem;
}

.remember-toggle {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
  color: inherit;
  transition: opacity 0.2s;
}

.remember-toggle:hover {
  opacity: 0.85;
}

.remember-box {
  width: 18px;
  height: 18px;
  border-radius: 5px;
  border: 1.5px solid rgba(139, 92, 246, 0.35);
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.22s ease;
  flex-shrink: 0;
}

/* Estado activo del checkbox */
.remember-toggle.checked .remember-box {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.8), rgba(34, 211, 238, 0.6));
  border-color: #a78bfa;
  box-shadow: 0 0 8px rgba(139, 92, 246, 0.4);
}

.remember-check {
  font-size: 0.625rem;
  font-weight: 900;
  color: #fff;
  line-height: 1;
}

.remember-label {
  font-size: 0.78rem;
  font-weight: 600;
  color: #94a3b8;
  letter-spacing: 0.2px;
  user-select: none;
}

/* Badge "se guardará" */
.remember-active {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.6rem;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #64748b;
  font-weight: 700;
}

.ra-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: #22d3ee;
  box-shadow: 0 0 5px #22d3ee;
  animation: blink 1.8s ease-in-out infinite;
}

.ra-text {
  color: #4ade80;
  font-size: 0.6rem;
}

/* ─────────────────────────────────────────── */

.quick-info {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.05), rgba(34, 211, 238, 0.05));
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.info-icon {
  font-size: 1.25rem;
}

.info-text {
  color: #94a3b8;
  font-size: 0.9rem;
}

.error-alert {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1.5rem;
  animation: shake 0.3s ease;
}

@keyframes shake {

  0%,
  100% {
    transform: translateX(0);
  }

  25% {
    transform: translateX(-10px);
  }

  75% {
    transform: translateX(10px);
  }
}

.error-icon {
  font-size: 1.5rem;
}

.error-text {
  color: #fca5a5;
  font-size: 0.9rem;
}

.submit-btn {
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

.submit-btn:hover {
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

.submit-btn:hover .btn-arrow {
  transform: translateX(5px);
}

.btn-loading {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.separator {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin: 1.5rem 0;
}

.separator-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(139, 92, 246, 0.3), transparent);
}

.separator-text {
  color: #64748b;
  font-size: 0.7rem;
  font-weight: 600;
  letter-spacing: 1.5px;
}

.register-section {
  text-align: center;
}

.register-text {
  color: #64748b;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.register-btn {
  border-color: rgba(139, 92, 246, 0.3) !important;
  color: #a78bfa !important;
  font-weight: 600 !important;
  border-radius: 12px !important;
  text-transform: none !important;
  transition: all 0.3s ease !important;
}

.register-btn:hover {
  border-color: #a78bfa !important;
  background: rgba(139, 92, 246, 0.1) !important;
  transform: translateY(-2px);
}

/* Info Panel */
.info-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  animation: fadeIn 0.8s ease-out 0.2s both;
}

.welcome-card {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.15), rgba(34, 211, 238, 0.15));
  border: 1px solid rgba(139, 92, 246, 0.3);
  border-radius: 20px;
  padding: 2rem;
  text-align: center;
  backdrop-filter: blur(10px);
}

.welcome-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  animation: wave 2s ease-in-out infinite;
}

@keyframes wave {

  0%,
  100% {
    transform: rotate(0deg);
  }

  25% {
    transform: rotate(20deg);
  }

  75% {
    transform: rotate(-20deg);
  }
}

.welcome-title {
  color: #a78bfa;
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.welcome-desc {
  color: #94a3b8;
  font-size: 1rem;
}

.info-card {
  background: rgba(15, 15, 30, 0.4);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(139, 92, 246, 0.1);
  border-radius: 16px;
  padding: 1.5rem;
  transition: all 0.3s ease;
}

.info-card:hover {
  transform: translateY(-3px);
  border-color: rgba(139, 92, 246, 0.3);
  background: rgba(15, 15, 30, 0.6);
}

.info-title {
  color: #a78bfa;
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.info-desc {
  color: #94a3b8;
  font-size: 0.9rem;
  line-height: 1.5;
}

.progress-card {
  background: rgba(139, 92, 246, 0.1);
  border: 1px solid rgba(139, 92, 246, 0.3);
  border-radius: 16px;
  padding: 1.5rem;
  backdrop-filter: blur(10px);
}

.progress-label {
  font-size: 0.7rem;
  color: #a78bfa;
  font-weight: 600;
  letter-spacing: 2px;
  margin-bottom: 1rem;
  text-align: center;
}

.progress-items {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.progress-item {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 12px;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  transition: transform 0.3s ease;
}

.progress-item:hover {
  transform: scale(1.05);
}

.progress-icon {
  font-size: 1.5rem;
}

.progress-name {
  color: #94a3b8;
  font-size: 0.8rem;
  font-weight: 600;
}

/* Snackbar */
.snackbar-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-weight: 500;
}

.snackbar-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  font-weight: 700;
}

/* Responsive */
@media (max-width: 1024px) {
  .content-grid {
    grid-template-columns: 1fr;
  }

  .info-panel {
    order: -1;
  }
}

@media (max-width: 768px) {
  .login-container {
    padding: 2rem 1rem;
  }

  .form-card {
    padding: 2rem 1.5rem;
  }

  .hero-title {
    font-size: 2rem;
  }

  .welcome-icon {
    font-size: 2.5rem;
  }

  .welcome-title {
    font-size: 1.3rem;
  }

  .progress-items {
    gap: 0.75rem;
  }
}

@media (max-width: 480px) {
  .form-card {
    padding: 1.5rem 1rem;
  }

  .hero-title {
    font-size: 1.5rem;
  }

  .main-logo {
    height: 60px;
  }

  .welcome-card {
    padding: 1.2rem;
  }

  .welcome-icon {
    font-size: 2rem;
  }

  .progress-items {
    grid-template-columns: 1fr;
  }

  .remember-row {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
