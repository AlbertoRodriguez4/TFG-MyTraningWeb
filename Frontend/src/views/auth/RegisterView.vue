<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import router from '@/router'
import { useUserStore } from '@/stores/userStore'
import type { User } from '@/components/Models/User'
import { useAuthValidation } from '@/composables/useAuthValidation'

const store = useUserStore()
const errorMessage = ref('')
const isLoading = ref(false)

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const name = ref('')

const { t } = useI18n()

// Snackbar states
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('error')

const {
  errors,
  passwordStrength,
  isPasswordValid,
  validateEmail,
  validatePassword,
  validateConfirmPassword,
  validateName,
  updatePasswordStrength,
  clearErrors
} = useAuthValidation()

const getStrengthColor = (strength: number) => {
  if (strength <= 1) return '#ef4444'
  if (strength <= 2) return '#f59e0b'
  if (strength <= 3) return '#eab308'
  if (strength <= 4) return '#84cc16'
  return '#22c55e'
}

const getStrengthLabel = (strength: number) => {
  if (strength <= 1) return t('strength_very_weak')
  if (strength <= 2) return t('strength_weak')
  if (strength <= 3) return t('strength_medium')
  if (strength <= 4) return t('strength_strong')
  return t('strength_very_strong')
}

const showSnackbar = (message: string, color: string = 'error') => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

const register = async () => {
  errorMessage.value = ''
  isLoading.value = true
  clearErrors()

  const emailTrimmed = email.value.trim()
  const passwordTrimmed = password.value.trim()
  const confirmPasswordTrimmed = confirmPassword.value.trim()
  const nameTrimmed = name.value.trim()

  // Validación de campos vacíos
  if (!emailTrimmed || !passwordTrimmed || !confirmPasswordTrimmed || !nameTrimmed) {
    errorMessage.value = t('complete_all_fields')
    showSnackbar(t('please_complete_all'), 'error')
    isLoading.value = false
    return
  }

  // Validaciones usando el composable
  if (!validateName(nameTrimmed)) {
    errorMessage.value = errors.name || ''
    showSnackbar(errors.name || t('name_invalid'), 'warning')
    isLoading.value = false
    return
  }

  // Validación adicional de longitud máxima de nombre
  if (nameTrimmed.length > 50) {
    errorMessage.value = t('name_too_long')
    showSnackbar(t('name_max_chars'), 'warning')
    isLoading.value = false
    return
  }
   // Validar que el email tenga un formato correcto
  if (!validateEmail(emailTrimmed)) {
    errorMessage.value = errors.email || ''
    showSnackbar(errors.email || t('invalid_email'), 'warning')
    isLoading.value = false
    return
  }
  // Validar que la contraseña sea lo suficientemente fuerte
  updatePasswordStrength(passwordTrimmed)
  if (!isPasswordValid.value) {
    const missingRequirements = passwordStrength.value.feedback
    errorMessage.value = `${t('weak_password')} ${missingRequirements.join(', ')}`
    showSnackbar(`${t('weak_password_must')} ${missingRequirements.join(', ')}`, 'warning')
    isLoading.value = false
    return
  }
  // Validar que las contraseñas coincidan
  if (!validateConfirmPassword(passwordTrimmed, confirmPasswordTrimmed)) {
    errorMessage.value = errors.confirmPassword || ''
    showSnackbar(errors.confirmPassword || t('passwords_no_match'), 'error')
    isLoading.value = false
    return
  }

  const user: User = {
    id: 0,
    name: nameTrimmed,
    passwordhash: passwordTrimmed,
    email: emailTrimmed,
    level: 0,
    strength: 0,
    endurance: 0,
    consistencyStreak: 0,
    gold: 0,
    role: 'userNormal',
    experience: 0,
    xpRequired: 100,
    xpRemaining: 100,
    equippedStrengthItemId: undefined,
    equippedEnduranceItemId: undefined,
    avatarUrl: ''
  }

  try {
    const result = await store.registerUser(user)
    //Mensaje de error 
    if (!result.success) {
      errorMessage.value = t('user_not_created')
      showSnackbar(t('email_registered'), 'error')
      isLoading.value = false
      return
    }

    const emailToVerify = result.email || emailTrimmed
    const successMessage = result.emailSent
      ? t('register_success_verify')
      : t('register_success_resend')

    showSnackbar(successMessage, result.emailSent ? 'success' : 'warning')
    setTimeout(() => {
      router.push({ name: 'verifyEmail', query: { email: emailToVerify } })
    }, 1500)
  } catch (error) {
    console.error(error)
    errorMessage.value = t('unexpected_error')
    showSnackbar(t('register_server_error'), 'error')
    isLoading.value = false
  }
}
</script>

<template>
  <div class="register-wrapper">
   
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>

    <!-- Grid lines futuristas -->
    <div class="grid-lines"></div>

    <div class="register-container">
      <!-- Header -->
      <div class="header-banner">
        <div class="logo-container">
          <div class="logo-icon">
            <img src="@/assets/imgs/Logo.png" alt="Training Hub" class="main-logo" />
          </div>
        </div>
        <h1 class="hero-title">{{ $t('slogan') }}</h1>
        <p class="hero-subtitle">{{ $t('advanced_training_system') }}</p>
      </div>

      <!-- Contenedor Principal -->
      <div class="content-grid">
        <!-- Panel de Registro -->
        <div class="form-panel">
          <div class="form-card">
            <div class="form-header">
              <div class="status-badge">
                <span class="badge-dot"></span>
                <span class="badge-text">{{ $t('new_user') }}</span>
              </div>
              <h2 class="form-title">{{ $t('register_title') }}</h2>
              <p class="form-subtitle">{{ $t('init_profile') }}</p>
            </div>

            <v-form @submit.prevent="register" class="register-form">
              <!-- Inputs -->
              <div class="inputs-grid">
                <!-- Nombre -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('username_label') }}</label>
                  <v-text-field v-model="name" :placeholder="$t('username_placeholder')" variant="solo-filled"
                    density="comfortable" color="purple-lighten-2" class="custom-input" hide-details="auto"
                    maxlength="50" counter></v-text-field>
                </div>

                <!-- Email -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('email_label') }}</label>
                  <v-text-field v-model="email" type="email" :placeholder="$t('email_placeholder')"
                    variant="solo-filled" density="comfortable" color="purple-lighten-2" class="custom-input"
                    hide-details="auto"></v-text-field>
                </div>

                <!-- Contraseña -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('password_label') }}</label>
                  <v-text-field v-model="password" type="password" :placeholder="$t('password_placeholder')"
                    variant="solo-filled" density="comfortable" color="purple-lighten-2" class="custom-input"
                    hide-details="auto" @input="updatePasswordStrength"></v-text-field>

                  <!-- Password Strength Indicator -->
                  <div v-if="password" class="password-strength">
                    <div class="strength-bar">
                      <div class="strength-fill" :style="{
                        width: `${(passwordStrength.strength / 5) * 100}%`,
                        background: getStrengthColor(passwordStrength.strength)
                      }"></div>
                    </div>
                    <div class="strength-label" :style="{ color: getStrengthColor(passwordStrength.strength) }">
                      {{ getStrengthLabel(passwordStrength.strength) }}
                    </div>
                    <div v-if="passwordStrength.feedback.length > 0" class="strength-feedback">
                      <span class="feedback-text">{{ $t('password_strength_feedback') }} {{
                        passwordStrength.feedback.join(', ') }}</span>
                    </div>
                  </div>
                </div>

                <!-- Confirmar -->
                <div class="input-wrapper">
                  <label class="input-label">{{ $t('confirm_label') }}</label>
                  <v-text-field v-model="confirmPassword" type="password"
                    :placeholder="$t('confirm_password_placeholder')" variant="solo-filled" density="comfortable"
                    color="purple-lighten-2" class="custom-input" hide-details="auto"></v-text-field>
                </div>
              </div>

              <!-- Stats Preview -->
              <div class="stats-section">
                <div class="stats-label">{{ $t('starting_stats') }}</div>
                <div class="stats-grid">
                  <div class="stat-item">
                    <div class="stat-icon"><v-icon>mdi-arm-flex</v-icon></div>
                    <div class="stat-value">0</div>
                    <div class="stat-name">STR</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-icon"><v-icon>mdi-lightning-bolt</v-icon></div>
                    <div class="stat-value">0</div>
                    <div class="stat-name">END</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-icon"><v-icon>mdi-fire</v-icon></div>
                    <div class="stat-value">0</div>
                    <div class="stat-name">STK</div>
                  </div>
                  <div class="stat-item">
                    <div class="stat-icon"><v-icon>mdi-gold</v-icon></div>
                    <div class="stat-value">100</div>
                    <div class="stat-name">GOLD</div>
                  </div>
                </div>
              </div>

              <!-- Error Alert -->
              <div v-if="errorMessage" class="error-alert">
                <span class="error-icon"><v-icon>mdi-alert</v-icon></span>
                <span class="error-text">{{ errorMessage }}</span>
              </div>

              <!-- Submit Button -->
              <v-btn type="submit" size="x-large" class="submit-btn" :loading="isLoading" :disabled="isLoading" block>
                <span v-if="!isLoading" class="btn-text">
                  <span>{{ $t('register_button') }}</span>
                  <span class="btn-arrow"><v-icon>mdi-arrow-right</v-icon></span>
                </span>
                <span v-else class="btn-loading">
                  <v-progress-circular indeterminate size="20" width="2" color="white"></v-progress-circular>
                  <span>{{ $t('starting_system') }}</span>
                </span>
              </v-btn>

              <!-- Separator -->
              <div class="separator">
                <div class="separator-line"></div>
                <span class="separator-text">{{ $t('or_separator') }}</span>
                <div class="separator-line"></div>
              </div>

              <!-- Login Link -->
              <div class="login-section">
                <p class="login-text">{{ $t('already_have_account') }}</p>
                <v-btn variant="outlined" size="large" class="login-btn" to="/login" block>
                  {{ $t('login_button') }}
                </v-btn>
              </div>
            </v-form>
          </div>
        </div>

        <!-- Info Panel -->
        <div class="info-panel">
          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-controller-classic</v-icon></div>
            <h3 class="info-title">{{ $t('level_system') }}</h3>
            <p class="info-desc">{{ $t('complete_workouts') }}</p>
          </div>

          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-account-group</v-icon></div>
            <h3 class="info-title">{{ $t('train_group') }}</h3>
            <p class="info-desc">{{ $t('join_rooms') }}</p>
          </div>

          <div class="info-card">
            <div class="info-icon"><v-icon>mdi-trophy</v-icon></div>
            <h3 class="info-title">{{ $t('challenges_rewards') }}</h3>
            <p class="info-desc">{{ $t('earn_xp_gold') }}</p>
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

.register-wrapper {
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

.register-container {
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

/* Form Panel */
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
.inputs-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  margin-bottom: 1.5rem;
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

:deep(.custom-input .v-counter) {
  color: #64748b !important;
  font-size: 0.7rem;
  margin-top: 0.25rem;
}

/* Password Strength Indicator */
.password-strength {
  margin-top: 0.75rem;
}

.strength-bar {
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.strength-fill {
  height: 100%;
  transition: all 0.3s ease;
  border-radius: 2px;
}

.strength-label {
  font-size: 0.75rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.strength-feedback {
  font-size: 0.7rem;
  color: #94a3b8;
  margin-top: 0.25rem;
}

.feedback-text {
  display: inline-block;
}

/* Stats Section */
.stats-section {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.05), rgba(34, 211, 238, 0.05));
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.stats-label {
  font-size: 0.7rem;
  color: #64748b;
  font-weight: 600;
  letter-spacing: 2px;
  margin-bottom: 1rem;
  text-align: center;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.stat-item {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(139, 92, 246, 0.1);
  border-radius: 12px;
  padding: 1rem;
  text-align: center;
  transition: all 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-3px);
  border-color: rgba(139, 92, 246, 0.3);
}

.stat-icon {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #f8fafc;
  margin-bottom: 0.25rem;
}

.stat-name {
  font-size: 0.7rem;
  color: #64748b;
  font-weight: 600;
  letter-spacing: 1px;
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

/* Submit Button */
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

/* Separator */
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
  font-size: 0.75rem;
  font-weight: 600;
}

/* Login Section */
.login-section {
  text-align: center;
}

.login-text {
  color: #64748b;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.login-btn {
  border-color: rgba(139, 92, 246, 0.3) !important;
  color: #a78bfa !important;
  font-weight: 600 !important;
  border-radius: 12px !important;
  text-transform: none !important;
  transition: all 0.3s ease !important;
}

.login-btn:hover {
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

.info-icon {
  font-size: 2rem;
  margin-bottom: 0.75rem;
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

/* Responsive */
@media (max-width: 1024px) {
  .content-grid {
    grid-template-columns: 1fr;
  }

  .info-panel {
    order: -1;
  }

  .inputs-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .register-container {
    padding: 2rem 1rem;
  }

  .form-card {
    padding: 2rem 1.5rem;
  }

  .hero-title {
    font-size: 2rem;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
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
}
</style>
