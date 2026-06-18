<template>
  <v-card class="settings-card" elevation="0" border>
    <v-card-title class="card-title">
      <v-icon>mdi-account-circle</v-icon>
      {{ $t('personal_info') }}
    </v-card-title>

    <v-divider class="card-divider" />

    <v-card-text>
      <v-form ref="form" class="settings-form">
        <div class="form-group">
          <label class="form-label">{{ $t('profile.fullName') }}</label>
          <v-text-field v-model="formData.name" :placeholder="$t('profile.fullNamePlaceholder')" variant="outlined"
            density="comfortable" class="form-input" :rules="[rules.required, rules.minLength]" />
        </div>

        <div class="form-group">
          <label class="form-label">{{ $t('email_label') }}</label>
          <v-text-field v-model="formData.email" :placeholder="$t('profile.emailPlaceholder')" type="email" variant="outlined"
            density="comfortable" class="form-input" disabled hint="Contacta con soporte para cambiar tu correo" />
        </div>

        <div class="form-group" v-if="store.loggedUser?.role != 'userNormal'">
          <label class="form-label">{{ $t('profile.role') }}</label>
          <v-text-field :model-value="store.loggedUser?.role || 'User'" variant="outlined" density="comfortable"
            class="form-input" disabled />
        </div>

        <div class="stats-info">
          <div class="info-group">
            <label class="form-label">{{ $t('profile.level') }}</label>
            <p class="info-value">{{ store.loggedUser?.level || 0 }}</p>
          </div>
          <div class="info-group">
            <label class="form-label">{{ $t('profile.experience') }}</label>
            <p class="info-value">{{ store.loggedUser?.experience || 0 }} / {{ store.loggedUser?.xpRequired || 100 }} XP
            </p>
          </div>
          <div class="info-group">
            <label class="form-label">{{ $t('profile.consistencyStreak') }}</label>
            <p class="info-value">{{ store.loggedUser?.consistencyStreak || 0 }} {{ $t('days_count') }}</p>
          </div>
        </div>

        <div class="form-actions">
          <v-btn color="#ffcc00" text-color="#000" variant="flat" size="large" @click="saveChanges" class="save-btn"
            :loading="saving">
            <v-icon start>mdi-check</v-icon>
            {{ $t('save_changes') }}
          </v-btn>
          <v-btn variant="outlined" size="large" @click="resetForm" class="cancel-btn">
            {{ $t('common.cancel') }}
          </v-btn>
        </div>
      </v-form>
    </v-card-text>
  </v-card>

  <v-snackbar v-model="snackbar.show" :color="snackbar.color" :timeout="snackbar.timeout" location="bottom right"
    rounded="lg" elevation="4">
    <div class="snackbar-content">
      <v-icon :icon="snackbar.icon" class="mr-2" />
      {{ snackbar.message }}
    </div>

    <template #actions>
      <v-btn variant="text" icon="mdi-close" size="small" @click="snackbar.show = false" />
    </template>
  </v-snackbar>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/userStore'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const store = useUserStore()
const form = ref()
const saving = ref(false)
// Form data reactivo para los campos editables del perfil
const formData = reactive({
  name: store.loggedUser?.name || '',
  email: store.loggedUser?.email || '',
  avatarUrl: store.loggedUser?.avatarUrl || '',
})


const snackbar = reactive({
  show: false,
  message: '',
  color: 'success',
  icon: 'mdi-check-circle',
  timeout: 3000,
})

const showSnackbar = (message: string, type: 'success' | 'error') => {
  snackbar.message = message
  snackbar.color = type === 'success' ? '#2e7d32' : '#c62828'
  snackbar.icon = type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'
  snackbar.timeout = type === 'success' ? 3000 : 5000
  snackbar.show = true
}
// Reglas de validación para el formulario
const rules = {
  required: (value: string) => !!value || 'Este campo es requerido',
  minLength: (value: string) => value?.length >= 2 || t('min_2_chars'),
}
// Función para guardar los cambios del perfil, muestra un mensaje de éxito o error dependiendo del resultado
const saveChanges = async () => {
  // Comprobar que el formulario es válido antes de intentar guardar los cambios
  const { valid } = await form.value.validate()
  // Si es válido y hay un usuario logueado, se intenta guardar los cambios, mostrando un mensaje de éxito o error dependiendo del resultado
  if (valid && store.loggedUser) {
    saving.value = true
    try {
      await store.editUser(store.loggedUser.id, {
        ...store.loggedUser,
        name: formData.name,
        avatarUrl: formData.avatarUrl,
      })
      showSnackbar('¡Perfil actualizado exitosamente!', 'success')
    } catch (error) {
      logger.error('Error al guardar:', error)
      showSnackbar(t('error_saving'), 'error')
    } finally {
      saving.value = false
    }
  }
}

const resetForm = () => {
  formData.name = store.loggedUser?.name || ''
  formData.email = store.loggedUser?.email || ''
  formData.avatarUrl = store.loggedUser?.avatarUrl || ''
}
</script>

<style scoped>
.settings-card {
  background: rgba(20, 20, 20, 0.5) !important;
  border: 1px solid rgba(255, 204, 0, 0.15) !important;
  border-radius: 12px;
  margin-bottom: 2rem;
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

.form-input :deep(.v-field) {
  background: rgba(255, 204, 0, 0.03) !important;
  border-color: rgba(255, 204, 0, 0.15) !important;
}

.form-input :deep(.v-field--focused) {
  background: rgba(255, 204, 0, 0.08) !important;
  border-color: rgba(255, 204, 0, 0.4) !important;
}

.form-input :deep(.v-field__input),
.form-input :deep(.v-select__content),
.form-input :deep(textarea) {
  color: rgba(255, 255, 255, 0.9) !important;
}

.form-input :deep(.v-field__input::placeholder),
.form-input :deep(textarea::placeholder) {
  color: rgba(255, 255, 255, 0.5) !important;
}

/* Avatar Preview */
.avatar-preview {
  margin-top: 1rem;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.2);
  background: rgba(255, 204, 0, 0.05);
  display: flex;
  align-items: center;
  justify-content: center;
}

.avatar-preview img {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  object-fit: cover;
  border: 2px solid rgba(255, 204, 0, 0.3);
}

/* Stats Info (Read Only) */
.stats-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.1);
  background: rgba(255, 204, 0, 0.02);
  margin: 1rem 0;
}

.info-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.info-group .form-label {
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: rgba(255, 255, 255, 0.6);
}

.info-value {
  margin: 0;
  font-size: 1.3rem;
  font-weight: 900;
  color: #ffcc00;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 204, 0, 0.1);
}

.save-btn {
  flex: 1;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.cancel-btn {
  flex: 1;
  color: rgba(255, 255, 255, 0.7) !important;
  border-color: rgba(255, 204, 0, 0.3) !important;
  font-weight: 600;
}

/* Snackbar */
.snackbar-content {
  display: flex;
  align-items: center;
  font-weight: 500;
  font-size: 0.95rem;
}

/* Responsive */
@media (max-width: 600px) {
  .card-title {
    font-size: 1.1rem;
    padding: 1.2rem;
  }

  .stats-info {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
    gap: 0.8rem;
  }

  .save-btn,
  .cancel-btn {
    flex: 1;
  }
}
</style>