<template>
  <v-card class="settings-card" elevation="0" border>
    <v-card-title class="card-title">
      <v-icon>mdi-bell</v-icon>
      {{ $t('Preferencias de Notificaciones') }}
    </v-card-title>

    <v-divider class="card-divider" />

    <v-card-text>
      <div class="notifications-container">

        <div class="notification-section">
          <h3 class="section-title">{{ $t('Alertas de la App') }}</h3>
          <p class="section-subtitle mb-4 text-white">{{ $t('Elige qué notificaciones quieres recibir para mantenerte informado sin distracciones.') }}</p>

          <div class="notification-item">
            <div class="item-content">
              <p class="item-title">{{ $t('Recordatorio de Inactividad') }}</p>
              <p class="item-description d-flex align-center flex-wrap gap-2">
                {{ $t('Notifícame si no he registrado un entrenamiento en') }}
                <v-text-field
                  v-model.number="store.preferences.inactivityDays"
                  type="number"
                  variant="outlined"
                  density="compact"
                  hide-details
                  min="1"
                  max="30"
                  class="inline-input"
                  :disabled="!store.preferences.inactivityEnabled"
                />
                {{ $t('días') }}
              </p>
            </div>
            <v-switch
              v-model="store.preferences.inactivityEnabled"
              color="#ffcc00"
              class="toggle-switch"
              hide-details
            />
          </div>

          <div class="notification-item">
            <div class="item-content">
              <p class="item-title">{{ $t('Actividad de Salas') }}</p>
              <p class="item-description">{{ $t('Notifícame al crear o unirme a una sala de entrenamiento') }}</p>
            </div>
            <v-switch
              v-model="store.preferences.roomsEnabled"
              color="#ffcc00"
              class="toggle-switch"
              hide-details
            />
          </div>

          <div class="notification-item">
            <div class="item-content">
              <p class="item-title">{{ $t('Recibos de Compra') }}</p>
              <p class="item-description">{{ $t('Confirmaciones al comprar un objeto en la tienda') }}</p>
            </div>
            <v-switch
              v-model="store.preferences.purchasesEnabled"
              color="#ffcc00"
              class="toggle-switch"
              hide-details
            />
          </div>

          <div class="notification-item">
            <div class="item-content">
              <p class="item-title">{{ $t('Expiración de Suscripción') }}</p>
              <p class="item-description">{{ $t('Alertarme antes de que expire mi Plan Premium') }}</p>
            </div>
            <v-switch
              v-model="store.preferences.subscriptionExpiryEnabled"
              color="#ffcc00"
              class="toggle-switch"
              hide-details
            />
          </div>

        </div>

        <v-alert v-if="store.error" type="error" variant="tonal" class="mt-2" closable @click:close="store.error = null">
          {{ store.error }}
        </v-alert>

        <v-snackbar v-model="showSuccessSnackbar" :timeout="3000" color="success" location="top">
          {{ successMessage }}
          <template #actions>
            <v-btn variant="text" size="small" @click="showSuccessSnackbar = false">{{ $t('close') }}</v-btn>
          </template>
        </v-snackbar>

        <div class="form-actions">
          <v-btn
            color="#ffcc00"
            text-color="#000"
            variant="flat"
            size="large"
            class="save-btn"
            :loading="store.isLoading"
            @click="handleSave"
          >
            <v-icon start>mdi-check</v-icon>
            {{ $t('Guardar Preferencias') }}
          </v-btn>
          <v-btn
            variant="outlined"
            size="large"
            class="reset-btn"
            @click="handleReset"
            :disabled="store.isLoading"
          >
            {{ $t('Restablecer por Defecto') }}
          </v-btn>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useNotificationPreferencesStore } from '@/stores/NotificationPreferencesStore'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const store = useNotificationPreferencesStore()
const showSuccessSnackbar = ref(false)
const successMessage = ref('')

onMounted(() => {
  store.fetchPreferences()
})
// Función para guardar las preferencias, muestra un mensaje de éxito si se guardan correctamente
const handleSave = async () => {
  const success = await store.savePreferences()
  if (success) {
    successMessage.value = t('preferences_saved')
    showSuccessSnackbar.value = true
    logger.log('Preferencias guardadas correctamente')
  }
}
// Función para restablecer las preferencias a los valores por defecto, muestra un mensaje de éxito si se restablecen correctamente
const handleReset = async () => {
  const success = await store.resetDefaults()
  if (success) {
    successMessage.value = t('preferences_reset')
    showSuccessSnackbar.value = true
    logger.log('Preferencias restablecidas a valores por defecto')
  }
}
</script>

<style scoped>
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

.notifications-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.notification-section {
  padding: 1rem 0;
}

.section-title {
  margin: 0 0 0.5rem 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 700;
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.notification-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 204, 0, 0.1);
  background: rgba(255, 204, 0, 0.02);
  margin-bottom: 0.8rem;
  transition: all 0.2s ease;
}

.item-content {
  flex: 1;
}

.item-title {
  margin: 0 0 0.3rem 0;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  font-size: 0.95rem;
}

.item-description {
  margin: 0;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.85rem;
}

/* Estilo especial para el input de días integrado en el texto */
.inline-input {
  max-width: 70px;
  margin: 0 4px;
}
.inline-input :deep(.v-field__input) {
  padding-top: 4px !important;
  padding-bottom: 4px !important;
  min-height: 32px !important;
  color: #ffcc00 !important;
  font-weight: bold;
  text-align: center;
}
.inline-input :deep(.v-field__outline) {
  --v-field-border-opacity: 0.3;
}

.toggle-switch {
  flex-shrink: 0;
  margin-left: 1rem;
}

.toggle-switch :deep(.v-switch__track) {
  background: rgba(255, 255, 255, 0.2) !important;
}

/* Action Buttons */
.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 204, 0, 0.1);
}

.save-btn {
  flex: 1;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.reset-btn {
  flex: 1;
  color: rgba(255, 255, 255, 0.7) !important;
  border-color: rgba(255, 204, 0, 0.3) !important;
  font-weight: 600;
}

/* Responsive */
@media (max-width: 600px) {
  .notification-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .toggle-switch {
    width: 100%;
    margin-left: 0;
  }

  .form-actions {
    flex-direction: column;
  }

  .save-btn,
  .reset-btn {
    flex: 1;
  }
}
</style>