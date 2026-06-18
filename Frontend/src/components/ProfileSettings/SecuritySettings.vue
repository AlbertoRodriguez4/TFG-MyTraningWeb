<template>
  <div class="security-settings">
    <v-card class="settings-card" elevation="0" border>
      <v-card-title class="card-title">
        <v-icon>mdi-shield-lock</v-icon>
        {{ $t('security_password') }}
      </v-card-title>

      <v-divider class="card-divider" />

      <v-card-text>
        <v-form ref="passwordForm" class="settings-form">
          <div class="form-group">
            <label class="form-label">{{ $t('security.currentPassword') }}</label>
            <v-text-field v-model="passwordData.current" type="password" variant="outlined" density="comfortable"
              :placeholder="$t('security.currentPasswordPlaceholder')" class="form-input" :rules="[rules.required]" />
          </div>

          <div class="form-group">
            <label class="form-label">{{ $t('security.newPassword') }}</label>
            <v-text-field v-model="passwordData.new" type="password" variant="outlined" density="comfortable"
              :placeholder="$t('security.newPasswordPlaceholder')" class="form-input" :rules="[rules.required, rules.minLength]" counter
              maxlength="32" />
            <p class="password-hint">{{ $t('security.passwordRequirements') }}</p>
          </div>

          <div class="form-group">
            <label class="form-label">{{ $t('security.confirmNewPassword') }}</label>
            <v-text-field v-model="passwordData.confirm" type="password" variant="outlined" density="comfortable"
              :placeholder="$t('security.confirmNewPasswordPlaceholder')" class="form-input" :rules="[rules.required, rules.passwordMatch]" />
          </div>

          <div class="form-actions">
            <v-btn color="#ffcc00" text-color="#000" variant="flat" size="large" class="save-btn" :loading="isUpdating"
              @click="handleUpdatePassword">
              <v-icon start>mdi-check</v-icon>
              {{ $t('save_changes') }}
            </v-btn>
          </div>
        </v-form>
      </v-card-text>
    </v-card>

    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      :timeout="snackbar.timeout"
      location="bottom right"
      rounded="lg"
      elevation="4"
    >
      <div class="snackbar-content">
        <v-icon :icon="snackbar.icon" class="mr-2" />
        {{ snackbar.message }}
      </div>

      <template #actions>
        <v-btn
          variant="text"
          icon="mdi-close"
          size="small"
          @click="snackbar.show = false"
        />
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/userStore'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const userStore = useUserStore()
const passwordForm = ref()
const isUpdating = ref(false)
const twoFAEnabled = ref(false)
// Estado reactuvo para los campos del formulario de cambio de contraseña
const passwordData = reactive({
  current: '',
  new: '',
  confirm: '',
})

// Estado reactivo para el snackbar de notificaciones
const snackbar = reactive({
  show: false,
  message: '',
  color: 'success',
  icon: 'mdi-check-circle',
  timeout: 3000,
})
// Mostrar el snackbar dependiendo de si la acción fue exitosa o hubo un error
const showSnackbar = (message: string, type: 'success' | 'error') => {
  snackbar.message = message
  snackbar.color = type === 'success' ? '#2e7d32' : '#c62828'
  snackbar.icon = type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'
  snackbar.timeout = type === 'success' ? 3000 : 5000
  snackbar.show = true
}
// Reglas de validación para los campos del formulario
const rules = {
  required: (value: string) => !!value || 'This field is required',
  minLength: (value: string) => value?.length >= 8 || 'Password must be at least 8 characters',
  passwordMatch: (value: string) => value === passwordData.new || 'Passwords do not match',
}

async function handleUpdatePassword() {
  const { valid } = await passwordForm.value.validate()
  // Se comprueba la validación del formulario, si no es válida se detiene la ejecutión, y si sí que es válida se procede a actualizar la contraseña
  if (!valid) return

  isUpdating.value = true

  try {
    // Ejercutar la funcióin de cambio de contraseña del store, y mostrar el snackbar dependiendo del resultado
    const success = await userStore.changePassword(
      passwordData.current,
      passwordData.new
    )

    if (success) {
      showSnackbar(t('password_updated'), 'success')
      passwordForm.value.reset()
    } else {
      showSnackbar(t('current_password_incorrect'), 'error')
    }
  } catch (error) {
    logger.error(error)
    showSnackbar(t('unexpected_error_retry'), 'error')
  } finally {
    isUpdating.value = false
  }
}
</script>

<style scoped>
.security-settings {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.settings-card {
  background: rgba(20, 20, 20, 0.5) !important;
  border: 1px solid rgba(255, 204, 0, 0.15) !important;
  border-radius: 12px;
}

.card-title {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  color: #ffcc00;
  font-weight: 700;
  font-size: 1.2rem;
  padding: 1.5rem;
  padding-bottom: 0;
}

.card-divider {
  margin: 1rem 0;
  border-color: rgba(255, 204, 0, 0.1) !important;
}

.settings-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  font-size: 0.95rem;
}

.password-hint {
  margin: 0;
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.5);
  font-style: italic;
}

.form-input :deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.15) !important;
}

.form-input :deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.08) !important;
  border-color: rgba(255, 204, 0, 0.4) !important;
}

.form-input :deep(.v-field__input) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.form-actions {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 204, 0, 0.1);
}

.save-btn {
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Snackbar */
.snackbar-content {
  display: flex;
  align-items: center;
  font-weight: 500;
  font-size: 0.95rem;
}

/* Two-Factor Authentication */
.twofa-status {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.15);
  background: rgba(255, 204, 0, 0.03);
}

.twofa-status.enabled {
  background: rgba(74, 222, 128, 0.08);
  border-color: rgba(74, 222, 128, 0.2);
}

.status-info {
  flex: 1;
}

.status-title {
  margin: 0 0 0.3rem 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 700;
  font-size: 1rem;
}

.status-desc {
  margin: 0;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.9rem;
}

/* Sessions */
.sessions-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.session-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.1);
  background: rgba(255, 204, 0, 0.02);
}

.session-info {
  display: flex;
  gap: 2rem;
  flex: 1;
}

.session-device {
  display: flex;
  gap: 0.8rem;
  align-items: center;
}

.session-device :deep(.v-icon) {
  color: #ffcc00;
  font-size: 1.5rem;
}

.device-name {
  margin: 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
}

.device-location {
  margin: 0.2rem 0 0 0;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.85rem;
}

.session-time {
  text-align: right;
}

.time-label {
  margin: 0;
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.time-value {
  margin: 0.2rem 0 0 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
}

.logout-btn {
  color: rgba(255, 255, 255, 0.5) !important;
}

.current-chip {
  background: rgba(74, 222, 128, 0.15) !important;
  color: #4ade80 !important;
}

/* Login History */
.login-item {
  padding: 0.5rem 0;
}

.login-time {
  margin: 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
}

.login-location {
  margin: 0.2rem 0 0 0;
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
}

.login-ip {
  margin: 0.2rem 0 0 0;
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.85rem;
}

/* Responsive */
@media (max-width: 600px) {
  .twofa-status {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .session-info {
    flex-direction: column;
    gap: 0.8rem;
  }

  .session-time {
    text-align: left;
  }
}
</style>