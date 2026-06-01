<script setup lang="ts">
import { useUserStore } from "@/stores/userStore"
import { useRoomStore } from "@/stores/RoomStore"
import { ref, watch } from "vue"
import { useI18n } from 'vue-i18n'
import { logger } from '@/utils/logger'

const { t } = useI18n()

const props = defineProps({
  isVisible: Boolean,
})
const emit = defineEmits(['close'])

const dialogVisible = ref(props.isVisible)

//Sincroniza visibilidad con la prop
watch(() => props.isVisible, (val) => {
  dialogVisible.value = val
})

// Notifica al padre si el popup se cierra desde dentro
watch(dialogVisible, (val) => {
  if (!val) emit('close')
})

const store = useUserStore()
const roomStore = useRoomStore()
const loggedUser = ref(store.loggedUser)

// Estado del snackbar para mostrar mensajes de éxito o error
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')

function showSnackbar(text: string, color: string = 'success') {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}

const roomName = ref('')
const roomDescription = ref('')
const roomDate = ref('')
const roomLocalization = ref('')
const minLevel = ref<number | null>(1)
const minStats = ref<number | null>(10)
const minConsistency = ref<number | null>(0)
const error = ref<string>('')

// Obtener fecha mínima (hoy)
const getMinDate = () => {
  const today = new Date()
  return today.toISOString().split('T')[0]
}

function closePopup() {
  dialogVisible.value = false
  resetForm()
}
// Reiniciar el formulario a su estado inicial
function resetForm() {
  roomName.value = ''
  roomDescription.value = ''
  roomDate.value = ''
  roomLocalization.value = ''
  minLevel.value = 1
  minStats.value = 10
  minConsistency.value = 0
  error.value = ''
}
// Validar los campos del formulario antes de enviar la solicitud de creación de sala
function validateInputs(): boolean {
  // Validar que el nombre de la sala no esté vacío
  if (!roomName.value.trim()) {
    error.value = t('room_name_required')
    return false
  }
  // Validar que la fecha del evento esté seleccionada
  if (!roomDate.value) {
    error.value = t('event_date_required')
    return false
  }
  // Validar que la ubicación no esté vacía
  if (!roomLocalization.value) {
    error.value = t('location_required')
    return false
  }
  // Validar que el nivel mínimo sea un número válido y mayor o igual a 1
  if (minLevel.value === null || isNaN(minLevel.value) || minLevel.value < 1) {
    error.value = t('min_level_error')
    return false
  }
  // Validar que las stats mínimas sean un número válido y mayor o igual a 0
  if (minStats.value === null || isNaN(minStats.value) || minStats.value < 0) {
    error.value = t('min_stats_error')
    return false
  }
  return true
}

async function createRoom() {
  error.value = ''
  // Si no pasan las validaciones, no se intenta crear la sala y se muestra el error correspondiente
  if (!validateInputs()) return
  // Verificar que el usuario esté logueado antes de intentar crear la sala
  if (!loggedUser.value?.id) {
    error.value = t('user_not_logged_in')
    logger.error(error.value)
    return
  }

  // Usar la fecha directamente en formato YYYY-MM-DD
  const dateString = roomDate.value // Formato: "2026-01-30"
  // Crear el objeto de la nueva sala con los datos del formulario
  const newRoom = {
    name: roomName.value.trim(),
    description: roomDescription.value.trim() || t('room_no_description'),
    date: dateString,
    localization: roomLocalization.value,
    minlevel: minLevel.value as number,
    minstats: minStats.value as number,
    minconsistency: minConsistency.value as number,
  }

  try { 
  // Se crea la sala y se da mensaje de exito, si hay error se muestra mensaje de error y se loguea el error                      
    await roomStore.createRoom(newRoom, loggedUser.value.id)
    showSnackbar(t('room_created_success'), 'success')
    closePopup()
  } catch (e) {
    error.value = t('room_create_error')
    showSnackbar(t('room_create_error'), 'error')
    logger.error(e)
  }
}
</script>

<template>
  <v-dialog v-model="dialogVisible" max-width="680" persistent transition="dialog-bottom-transition"
    :fullscreen="$vuetify.display.mobile" scrollable>
    <v-card class="create-room-card">
      <!-- Header con gradiente -->
      <div class="card-header">
        <div class="header-content">
          <div class="header-icon">
            <v-icon size="large" color="#00ff88">mdi-plus-circle</v-icon>
          </div>
          <div class="header-text">
            <h2 class="card-title">{{ $t('crear') }}</h2>
            <p class="card-subtitle">{{ $t('configure_new_room') }}</p>
          </div>
        </div>

        <v-btn icon class="close-btn" @click="closePopup" size="small">
          <v-icon size="20">mdi-close</v-icon>
        </v-btn>
      </div>

      <v-card-text class="card-content">
        <v-form @submit.prevent="createRoom" ref="formRef">
          <!-- Error Alert -->
          <transition name="slide-fade">
            <v-alert v-if="error" type="error" class="error-alert" closable @click:close="error = ''">
              <div class="d-flex align-center">
                <v-icon class="mr-2" size="20">mdi-alert-circle</v-icon>
                <span class="error-text">{{ error }}</span>
              </div>
            </v-alert>
          </transition>

          <!-- Room Name -->
          <div class="form-field">
            <label class="field-label">
              <v-icon size="18" class="label-icon">mdi-door-open</v-icon>
              {{ $t('nombre de la sala') }}
            </label>
            <v-text-field v-model="roomName" :placeholder="$t('insertar nombre de la sala')" required variant="outlined"
              density="comfortable" class="custom-field" bg-color="rgba(255, 255, 255, 0.05)" hide-details>
              <template v-slot:prepend-inner>
                <v-icon color="#00ff88" size="20">mdi-format-text</v-icon>
              </template>
            </v-text-field>
          </div>

          <!-- Room Description -->
          <div class="form-field">
            <label class="field-label">
              <v-icon size="18" class="label-icon">mdi-text-box</v-icon>
              {{ $t('descripcion') }}
            </label>
            <v-textarea v-model="roomDescription" :placeholder="$t('descripcion de la sala')" variant="outlined"
              density="comfortable" class="custom-field custom-textarea" bg-color="rgba(255, 255, 255, 0.05)"
              hide-details rows="3" auto-grow max-rows="5">
              <template v-slot:prepend-inner>
                <v-icon color="#6366f1" size="20">mdi-pencil</v-icon>
              </template>
            </v-textarea>
          </div>

          <!-- Event Date -->
          <div class="form-field">
            <label class="field-label">
              <v-icon size="18" class="label-icon">mdi-calendar-clock</v-icon>
              {{ $t('fecha del evento') }}
            </label>
            <v-text-field v-model="roomDate" type="date" :min="getMinDate()" required variant="outlined"
              density="comfortable" class="custom-field custom-date-field" bg-color="rgba(255, 255, 255, 0.05)"
              hide-details>
              <template v-slot:prepend-inner>
                <v-icon color="#f59e0b" size="20">mdi-calendar</v-icon>
              </template>
            </v-text-field>
          </div>

          <!-- Location -->
          <div class="form-field">
            <label class="field-label">
              <v-icon size="18" class="label-icon">mdi-map-marker</v-icon>
              {{ $t('location_label') }}
            </label>
            <v-text-field v-model="roomLocalization" :placeholder="$t('location_placeholder')" required
              variant="outlined" density="comfortable" class="custom-field custom-location-field"
              bg-color="rgba(255, 255, 255, 0.05)" hide-details>
              <template v-slot:prepend-inner>
                <v-icon color="#10b981" size="20">mdi-map-marker-radius</v-icon>
              </template>
            </v-text-field>
          </div>

          <!-- Requirements Section -->
          <div class="requirements-section">
            <div class="section-header">
              <v-icon color="#ffcc00" size="20">mdi-shield-lock</v-icon>
              <span class="section-title">{{ $t('access_requirements') }}</span>
            </div>

            <div class="requirements-grid">
              <!-- Min Level -->
              <div class="form-field">
                <label class="field-label">
                  <v-icon size="16" class="label-icon">mdi-chevron-triple-up</v-icon>
                  {{ $t('nivel minimo') }}
                </label>
                <v-text-field v-model.number="minLevel" type="number" min="1" required variant="outlined"
                  density="comfortable" class="custom-field" bg-color="rgba(255, 255, 255, 0.05)" hide-details>
                  <template v-slot:prepend-inner>
                    <v-icon color="#00d9ff" size="20">mdi-numeric</v-icon>
                  </template>
                </v-text-field>
              </div>

              <!-- Min Stats -->
              <div class="form-field">
                <label class="field-label">
                  <v-icon size="16" class="label-icon">mdi-trending-up</v-icon>
                  {{ $t('Stats Minimas') }}
                </label>
                <v-text-field v-model.number="minStats" type="number" min="0" required variant="outlined"
                  density="comfortable" class="custom-field" bg-color="rgba(255, 255, 255, 0.05)" hide-details>
                  <template v-slot:prepend-inner>
                    <v-icon color="#ff6b9d" size="20">mdi-numeric</v-icon>
                  </template>
                </v-text-field>
              </div>

              <!-- Min Consistency -->
              <div class="form-field full-width">
                <label class="field-label">
                  <v-icon size="16" class="label-icon">mdi-calendar-check</v-icon>
                  {{ $t('min_consistency') }}
                </label>
                <v-text-field v-model.number="minConsistency" type="number" min="0" variant="outlined"
                  density="comfortable" class="custom-field" bg-color="rgba(255, 255, 255, 0.05)" hide-details>
                  <template v-slot:prepend-inner>
                    <v-icon color="#ffcc00" size="20">mdi-numeric</v-icon>
                  </template>
                </v-text-field>
              </div>
            </div>
          </div>
        </v-form>
      </v-card-text>

      <!-- Action Buttons -->
      <v-card-actions class="card-actions">
        <v-btn color="secondary" @click="closePopup" variant="outlined" size="large" class="action-btn cancel-button">
          <v-icon class="btn-icon">mdi-close-circle</v-icon>
          <span class="btn-text">{{ $t('cancelar') }}</span>
        </v-btn>

        <v-btn @click="createRoom" size="large" class="action-btn create-button">
          <v-icon class="btn-icon">mdi-check-circle</v-icon>
          <span class="btn-text">{{ $t('creacion') }}</span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Snackbar -->
  <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="3000" location="top" rounded="pill">
    <div class="d-flex align-center">
      <v-icon class="mr-2">
        {{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}
      </v-icon>
      {{ snackbarText }}
    </div>
  </v-snackbar>
</template>

<style scoped>
/* Base Card */
.create-room-card {
  background: linear-gradient(180deg, #1a1a2e 0%, #0f0f1e 100%) !important;
  border: 1px solid rgba(0, 255, 136, 0.2);
  border-radius: 24px !important;
  overflow: hidden;
  box-shadow:
    0 20px 60px rgba(0, 0, 0, 0.5),
    0 0 0 1px rgba(255, 255, 255, 0.1) inset;
  animation: fadeIn 0.3s ease-out;

}

/* Header */
.card-header {
  position: relative;
  padding: clamp(1.5rem, 4vw, 2.5rem) clamp(1.25rem, 4vw, 2rem) clamp(1.25rem, 3vw, 2rem);
  background: linear-gradient(135deg, rgba(0, 255, 136, 0.1) 0%, rgba(0, 217, 255, 0.1) 100%);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.header-content {
  display: flex;
  align-items: center;
  gap: clamp(0.75rem, 2vw, 1.25rem);
}

.header-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: clamp(3rem, 10vw, 4.5rem);
  height: clamp(3rem, 10vw, 4.5rem);
  background: rgba(0, 255, 136, 0.1);
  border-radius: 50%;
  border: 2px solid rgba(0, 255, 136, 0.3);
  flex-shrink: 0;
  animation: iconPulse 2s ease-in-out infinite;
}

.header-icon .v-icon {
  font-size: clamp(1.5rem, 5vw, 2.5rem) !important;
}

.header-text {
  flex: 1;
  min-width: 0;
}

.card-title {
  font-size: clamp(1.25rem, 4vw, 2rem);
  font-weight: 800;
  background: linear-gradient(135deg, #00ff88 0%, #00d9ff 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0 0 clamp(0.25rem, 1vw, 0.5rem) 0;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  line-height: 1.2;
}

.card-subtitle {
  color: rgba(255, 255, 255, 0.6);
  font-size: clamp(0.8rem, 2vw, 0.95rem);
  margin: 0;
  font-weight: 300;
  line-height: 1.4;
}

.close-btn {
  position: absolute !important;
  top: clamp(0.75rem, 2vw, 1rem);
  right: clamp(0.75rem, 2vw, 1rem);
  background: rgba(255, 255, 255, 0.05) !important;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
  width: clamp(2rem, 5vw, 2.5rem) !important;
  height: clamp(2rem, 5vw, 2.5rem) !important;
}

.close-btn:hover {
  background: rgba(255, 107, 157, 0.2) !important;
  border-color: rgba(255, 107, 157, 0.5);
  transform: rotate(90deg);
}

/* Content */
.card-content {
  padding: clamp(1.25rem, 4vw, 2rem) !important;
  overflow-x: hidden;
}

/* Error Alert */
.error-alert {
  border-left: 4px solid #ff6b9d !important;
  background: rgba(255, 107, 157, 0.1) !important;
  border-radius: 12px !important;
  margin-bottom: clamp(1rem, 3vw, 1.5rem);
}

.error-text {
  font-size: clamp(0.8rem, 2vw, 0.9rem);
  line-height: 1.4;
}

/* Form Fields */
.form-field {
  margin-bottom: clamp(1rem, 3vw, 1.5rem);
}

.field-label {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  font-size: clamp(0.75rem, 2vw, 0.9rem);
  margin-bottom: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.label-icon {
  flex-shrink: 0;
}

.custom-field :deep(.v-field) {
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
  font-size: clamp(0.875rem, 2vw, 1rem);
}

.custom-field :deep(.v-field__input) {
  padding: clamp(0.5rem, 2vw, 0.75rem) clamp(0.75rem, 2vw, 1rem);
  min-height: clamp(2.5rem, 8vw, 3.5rem);
}

.custom-field :deep(.v-field:hover) {
  border-color: rgba(0, 255, 136, 0.4);
  background: rgba(255, 255, 255, 0.08) !important;
}

.custom-field :deep(.v-field--focused) {
  border-color: #00ff88;
  box-shadow: 0 0 0 3px rgba(0, 255, 136, 0.1);
  background: rgba(255, 255, 255, 0.08) !important;
}

.custom-field :deep(input),
.custom-field :deep(textarea) {
  color: #fff;
  font-weight: 500;
}

.custom-field :deep(.v-field__outline) {
  display: none;
}

.custom-field :deep(.v-field__prepend-inner) {
  padding-right: clamp(0.5rem, 1.5vw, 0.75rem);
}

/* Textarea específico */
.custom-textarea :deep(.v-field__input) {
  min-height: auto;
  padding-top: clamp(0.75rem, 2vw, 1rem);
  padding-bottom: clamp(0.75rem, 2vw, 1rem);
}

.custom-textarea :deep(textarea) {
  line-height: 1.5;
}

.custom-textarea :deep(.v-field--focused) {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1);
}

.custom-textarea :deep(.v-field:hover) {
  border-color: rgba(99, 102, 241, 0.4);
}

/* Date field específico */
.custom-date-field :deep(.v-field--focused) {
  border-color: #f59e0b;
  box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1);
}

.custom-date-field :deep(.v-field:hover) {
  border-color: rgba(245, 158, 11, 0.4);
}

.custom-date-field :deep(input[type="date"]) {
  color: #fff;
  cursor: pointer;
}

.custom-date-field :deep(input[type="date"]::-webkit-calendar-picker-indicator) {
  filter: invert(1);
  cursor: pointer;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.custom-date-field :deep(input[type="date"]::-webkit-calendar-picker-indicator:hover) {
  opacity: 1;
}

/* Location field específico */
.custom-location-field :deep(.v-field--focused) {
  border-color: #10b981;
  box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1);
}

.custom-location-field :deep(.v-field:hover) {
  border-color: rgba(16, 185, 129, 0.4);
}

/* Requirements Section */
.requirements-section {
  background: rgba(255, 204, 0, 0.05);
  border: 1px solid rgba(255, 204, 0, 0.2);
  border-radius: 16px;
  padding: clamp(1rem, 3vw, 1.5rem);
  margin-top: clamp(1rem, 3vw, 1.5rem);
}

.section-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: clamp(1rem, 3vw, 1.5rem);
}

.section-title {
  font-size: clamp(0.85rem, 2vw, 1rem);
  font-weight: 700;
  color: #ffcc00;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  line-height: 1.2;
}

.requirements-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(min(100%, 200px), 1fr));
  gap: clamp(0.75rem, 2vw, 1rem);
}

.requirements-grid .full-width {
  grid-column: 1 / -1;
}

/* Action Buttons */
.card-actions {
  padding: clamp(1rem, 3vw, 1.5rem) clamp(1.25rem, 4vw, 2rem) !important;
  gap: clamp(0.75rem, 2vw, 1rem);
  background: rgba(0, 0, 0, 0.3);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  flex-wrap: wrap;
}

.action-btn {
  flex: 1 1 auto;
  min-width: min(100%, 140px);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  transition: all 0.3s ease;
  font-size: clamp(0.8rem, 2vw, 0.9rem) !important;
  padding: clamp(0.75rem, 2vw, 1rem) clamp(1rem, 3vw, 1.5rem) !important;
  height: auto !important;
}

.btn-icon {
  font-size: clamp(1rem, 3vw, 1.25rem) !important;
  margin-right: clamp(0.35rem, 1vw, 0.5rem);
}

.btn-text {
  font-size: clamp(0.8rem, 2vw, 0.9rem);
  white-space: nowrap;
}

.cancel-button {
  border: 2px solid rgba(255, 255, 255, 0.2) !important;
  color: rgba(255, 255, 255, 0.8) !important;
}

.cancel-button:hover {
  border-color: rgba(255, 107, 157, 0.5) !important;
  background: rgba(255, 107, 157, 0.1) !important;
  color: #ff6b9d !important;
}

.create-button {
  background: linear-gradient(135deg, #00ff88 0%, #00d9ff 100%) !important;
  color: #000 !important;
  font-weight: 700;
  box-shadow: 0 4px 20px rgba(0, 255, 136, 0.4);
}

.create-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(0, 255, 136, 0.6);
}

.create-button:active {
  transform: translateY(0);
}

/* Animations */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes iconPulse {

  0%,
  100% {
    transform: scale(1);
    box-shadow: 0 0 0 0 rgba(0, 255, 136, 0.5);
  }

  50% {
    transform: scale(1.05);
    box-shadow: 0 0 0 10px rgba(0, 255, 136, 0);
  }
}

.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.2s ease-in;
}

.slide-fade-enter-from {
  transform: translateY(-10px);
  opacity: 0;
}

.slide-fade-leave-to {
  transform: translateY(-10px);
  opacity: 0;
}

/* Breakpoints específicos */

/* Tablets pequeñas y móviles grandes (landscape) */
@media (max-width: 768px) {
  .requirements-grid {
    grid-template-columns: 1fr;
  }

  .header-content {
    flex-direction: row;
  }
}

/* Móviles */
@media (max-width: 600px) {
  .card-actions {
    flex-direction: column;
  }

  .action-btn {
    width: 100%;
    min-width: 100%;
  }
}

/* Móviles muy pequeños */
@media (max-width: 360px) {
  .card-title {
    font-size: 1.1rem;
  }

  .card-subtitle {
    font-size: 0.75rem;
  }

  .header-icon {
    width: 2.5rem;
    height: 2.5rem;
  }

  .header-icon .v-icon {
    font-size: 1.25rem !important;
  }
}

/* Pantallas muy grandes */
@media (min-width: 1920px) {
  .create-room-card {
    max-width: 720px;
  }
}

/* Modo fullscreen (móviles) */
@media (max-width: 600px) {
  .v-dialog--fullscreen .create-room-card {
    border-radius: 0 !important;
    height: 100vh;
    display: flex;
    flex-direction: column;
  }

  .v-dialog--fullscreen .card-content {
    flex: 1;
    overflow-y: hidden;
  }

  .v-dialog--fullscreen .card-actions {
    position: sticky;
    bottom: 0;
    background: linear-gradient(180deg, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.5) 100%);
    backdrop-filter: blur(10px);
  }
}

/* Mejoras de accesibilidad y touch */
@media (hover: none) and (pointer: coarse) {
  .custom-field :deep(.v-field__input) {
    min-height: 3rem;
  }

  .action-btn {
    min-height: 3rem;
  }

  .close-btn {
    width: 2.75rem !important;
    height: 2.75rem !important;
  }
}

/* Orientación landscape en móviles */
@media (max-height: 500px) and (orientation: landscape) {
  .card-header {
    padding: 1rem 1.5rem;
  }

  .card-content {
    padding: 1rem !important;
    overflow-y: hidden;
  }

  .form-field {
    margin-bottom: 0.75rem;
  }

  .requirements-section {
    padding: 1rem;
    margin-top: 1rem;
  }

  .section-header {
    margin-bottom: 1rem;
  }
}
</style>