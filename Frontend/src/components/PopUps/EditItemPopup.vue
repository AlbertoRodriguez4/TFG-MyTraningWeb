<script setup lang="ts">
import { watch, reactive, ref } from 'vue'
import type { Item } from '@/components/Models/Item'
import { useItemStore } from '@/stores/itemStore'
import { logger } from '@/utils/logger'

const store = useItemStore()

const props = defineProps<{
  item: Item
  visible: boolean
}>()

const emit = defineEmits(['close', 'edit', 'delete'])

const dialogVisible = ref(props.visible)
watch(() => props.visible, val => dialogVisible.value = val)
watch(dialogVisible, val => { if (!val) emit('close') })

const errorMessage = ref('')
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')

const editedItem = reactive<Item>({
  id: 0,
  name: '',
  type: 'Strength',
  bonus: 0,
  price: 0
})

watch(() => props.item, (newItem) => {
  if (newItem) Object.assign(editedItem, { ...newItem })
}, { immediate: true })

function closePopup() {
  dialogVisible.value = false
}

function showSnackbar(text: string, color: string = 'success') {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}

const handleEdit = async () => {
  errorMessage.value = ''

  if (!editedItem.name || editedItem.name.trim().length < 3) {
    errorMessage.value = 'El nombre debe tener al menos 3 caracteres.'
    return
  }

  const validTypes = ['Strength', 'Endurance']
  if (!validTypes.includes(editedItem.type)) {
    errorMessage.value = 'El tipo seleccionado no es válido.'
    return
  }

  if (editedItem.bonus < 0 || editedItem.price < 0) {
    errorMessage.value = 'Bono y precio no pueden ser negativos.'
    return
  }

  try {
    const updatedItem = {
      ...editedItem,
      name: editedItem.name.trim()
    }
    const result = await store.editItem(updatedItem.id, updatedItem)
    if (result != null) {
      showSnackbar('Ítem editado correctamente', 'success')
      emit('edit')
      closePopup()
    } else {
      errorMessage.value = "Hubo un problema al editar el ítem."
    }
  } catch (error: any) {
    const message = error?.data?.message
    errorMessage.value = message || "Hubo un problema al editar el ítem."
    logger.error("Error al editar el ítem:", error)
  }
}

const handleDelete = async () => {
  const result = await store.deleteItem(editedItem.id)
  if (result !== null) {
    showSnackbar('Ítem eliminado correctamente', 'success')
    emit('delete')
    closePopup()
  } else {
    showSnackbar('Hubo un problema al eliminar el ítem', 'error')
  }
}
</script>

<template>
  <v-dialog v-model="dialogVisible" max-width="600" class="item-dialog">
    <v-card class="item-card">
      <!-- Header personalizado -->
      <div class="item-header">
        <div class="header-content">
          <v-icon class="header-icon">mdi-shopping</v-icon>
          <h2 class="header-title">Editar Ítem</h2>
        </div>
        <v-btn 
          icon 
          class="close-button" 
          @click="closePopup"
          size="small"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <v-card-text class="item-content">
        <!-- Error message -->
        <v-alert 
          v-if="errorMessage" 
          type="error" 
          variant="tonal"
          class="error-alert"
        >
          {{ errorMessage }}
        </v-alert>

        <!-- Nombre del ítem -->
        <div class="form-group">
          <label class="field-label">
            <v-icon size="small" class="label-icon">mdi-dumbbell</v-icon>
            Nombre del Ítem
          </label>
          <v-text-field
            v-model="editedItem.name"
            variant="outlined"
            density="comfortable"
            hide-details
            class="custom-input"
          />
        </div>

        <!-- Selector de tipo con botones -->
        <div class="form-group">
          <label class="field-label">Tipo de Bonus</label>
          <div class="type-selector">
            <button
              type="button"
              :class="['type-button', { active: editedItem.type === 'Strength' }]"
              @click="editedItem.type = 'Strength'"
            >
              <v-icon size="large" class="type-icon">mdi-lightning-bolt</v-icon>
              <span class="type-name">Fuerza</span>
            </button>
            <button
              type="button"
              :class="['type-button', { active: editedItem.type === 'Endurance' }]"
              @click="editedItem.type = 'Endurance'"
            >
              <v-icon size="large" class="type-icon">mdi-heart-pulse</v-icon>
              <span class="type-name">Resistencia</span>
            </button>
          </div>
        </div>

        <!-- Stats grid -->
        <div class="stats-grid">
          <div class="stat-card bonus-card">
            <div class="stat-header">
              <v-icon class="stat-icon">mdi-trending-up</v-icon>
              <span class="stat-label">Bonus</span>
            </div>
            <div class="stat-input-wrapper">
              <input
                v-model.number="editedItem.bonus"
                type="number"
                min="0"
                class="stat-input"
              />
              <span class="stat-suffix">+</span>
            </div>
          </div>

          <div class="stat-card price-card">
            <div class="stat-header">
              <v-icon class="stat-icon">mdi-currency-usd</v-icon>
              <span class="stat-label">Precio</span>
            </div>
            <div class="stat-input-wrapper">
              <input
                v-model.number="editedItem.price"
                type="number"
                min="0"
                class="stat-input"
              />
              <v-icon class="stat-suffix-icon">mdi-coin</v-icon>
            </div>
          </div>
        </div>

        <!-- Vista previa -->
        <div class="preview-card">
          <div class="preview-label">Vista Previa</div>
          <div class="preview-content">
            <div class="preview-icon-wrapper" :class="editedItem.type === 'Strength' ? 'strength-preview' : 'endurance-preview'">
              <v-icon size="large">
                {{ editedItem.type === 'Strength' ? 'mdi-lightning-bolt' : 'mdi-heart-pulse' }}
              </v-icon>
            </div>
            <div class="preview-info">
              <div class="preview-name">{{ editedItem.name || 'Nombre del ítem' }}</div>
              <div class="preview-stats">+{{ editedItem.bonus }} {{ editedItem.type === 'Strength' ? 'Fuerza' : 'Resistencia' }}</div>
            </div>
            <div class="preview-price">
              <v-icon size="small">mdi-coin</v-icon>
              {{ editedItem.price }}
            </div>
          </div>
        </div>
      </v-card-text>

      <!-- Footer con acciones -->
      <v-card-actions class="item-actions">
        <v-btn
          color="error"
          variant="outlined"
          @click="handleDelete"
          class="action-btn delete-btn"
        >
          <v-icon left>mdi-delete</v-icon>
          Eliminar
        </v-btn>
        <v-spacer />
        <v-btn
          color="grey"
          variant="outlined"
          @click="closePopup"
          class="action-btn cancel-btn"
        >
          Cancelar
        </v-btn>
        <v-btn
          color="primary"
          variant="elevated"
          @click="handleEdit"
          class="action-btn save-btn"
        >
          <v-icon left>mdi-content-save</v-icon>
          Guardar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Snackbar -->
  <v-snackbar
    v-model="snackbar"
    :color="snackbarColor"
    :timeout="3000"
    location="top"
    rounded="pill"
  >
    <div class="d-flex align-center">
      <v-icon class="mr-2">
        {{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}
      </v-icon>
      {{ snackbarText }}
    </div>
  </v-snackbar>
</template>

<style scoped>
/* Dialog y Card base */
.item-card {
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  border-radius: 16px !important;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5) !important;
}

/* Header */
.item-header {
  background: linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%);
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
}

.item-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.2);
  pointer-events: none;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
  z-index: 1;
}

.header-icon {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 8px;
  border-radius: 8px;
  color: white !important;
}

.header-title {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  color: white;
  letter-spacing: -0.5px;
}

.close-button {
  background: rgba(255, 255, 255, 0.2) !important;
  backdrop-filter: blur(10px);
  color: white !important;
  position: relative;
  z-index: 1;
}

.close-button:hover {
  background: rgba(255, 255, 255, 0.3) !important;
}

/* Content */
.item-content {
  padding: 24px !important;
  max-height: 70vh;
  overflow-y: auto;
}

.error-alert {
  margin-bottom: 20px;
  border-radius: 12px !important;
}

/* Form groups */
.form-group {
  margin-bottom: 24px;
}

.field-label {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #94a3b8;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.label-icon {
  color: #3b82f6 !important;
}

.custom-input {
  background: rgba(30, 41, 59, 0.5);
  border-radius: 12px;
}

.custom-input :deep(.v-field) {
  border-radius: 12px;
  background: rgba(30, 41, 59, 0.5);
}

.custom-input :deep(.v-field__outline) {
  color: rgba(148, 163, 184, 0.3);
}

.custom-input :deep(.v-field--focused .v-field__outline) {
  color: #3b82f6;
}

.custom-input :deep(input) {
  color: white;
}

/* Type selector */
.type-selector {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.type-button {
  background: rgba(30, 41, 59, 0.5);
  border: 2px solid rgba(148, 163, 184, 0.3);
  border-radius: 12px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  color: #94a3b8;
}

.type-button:hover {
  border-color: rgba(148, 163, 184, 0.5);
  transform: translateY(-2px);
}

.type-button.active {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.2) 0%, rgba(239, 68, 68, 0.1) 100%);
  border-color: #ef4444;
  box-shadow: 0 8px 20px rgba(239, 68, 68, 0.3);
}

.type-button.active .type-icon {
  color: #ef4444 !important;
}

.type-button.active .type-name {
  color: #fca5a5;
}

.type-button:nth-child(2).active {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.2) 0%, rgba(34, 197, 94, 0.1) 100%);
  border-color: #22c55e;
  box-shadow: 0 8px 20px rgba(34, 197, 94, 0.3);
}

.type-button:nth-child(2).active .type-icon {
  color: #22c55e !important;
}

.type-button:nth-child(2).active .type-name {
  color: #86efac;
}

.type-icon {
  color: #64748b !important;
}

.type-name {
  font-weight: 600;
  font-size: 14px;
}

/* Stats grid */
.stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 24px;
}

.stat-card {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.15) 0%, rgba(139, 92, 246, 0.05) 100%);
  border: 1px solid rgba(139, 92, 246, 0.3);
  border-radius: 12px;
  padding: 16px;
}

.bonus-card {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.15) 0%, rgba(168, 85, 247, 0.05) 100%);
  border-color: rgba(139, 92, 246, 0.3);
}

.price-card {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.15) 0%, rgba(251, 191, 36, 0.05) 100%);
  border-color: rgba(245, 158, 11, 0.3);
}

.stat-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.stat-icon {
  color: #8b5cf6 !important;
  font-size: 20px !important;
}

.price-card .stat-icon {
  color: #f59e0b !important;
}

.stat-label {
  color: #cbd5e1;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.stat-input-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
}

.stat-input {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(139, 92, 246, 0.2);
  border-radius: 8px;
  padding: 12px;
  color: white;
  font-size: 24px;
  font-weight: 700;
  width: 100%;
  outline: none;
  transition: all 0.2s ease;
}

.stat-input:focus {
  border-color: #8b5cf6;
  box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1);
}

.price-card .stat-input {
  border-color: rgba(245, 158, 11, 0.2);
}

.price-card .stat-input:focus {
  border-color: #f59e0b;
  box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1);
}

.stat-suffix {
  color: #8b5cf6;
  font-size: 20px;
  font-weight: 700;
}

.stat-suffix-icon {
  color: #f59e0b !important;
  font-size: 24px !important;
}

/* Preview card */
.preview-card {
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(30, 41, 59, 0.4) 100%);
  border: 1px solid rgba(148, 163, 184, 0.2);
  border-radius: 12px;
  padding: 16px;
}

.preview-label {
  color: #64748b;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  margin-bottom: 12px;
}

.preview-content {
  display: flex;
  align-items: center;
  gap: 12px;
}

.preview-icon-wrapper {
  width: 48px;
  height: 48px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.strength-preview {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}

.endurance-preview {
  background: rgba(34, 197, 94, 0.2);
  color: #22c55e;
}

.preview-info {
  flex: 1;
}

.preview-name {
  color: white;
  font-weight: 600;
  font-size: 15px;
  margin-bottom: 4px;
}

.preview-stats {
  color: #94a3b8;
  font-size: 13px;
}

.preview-price {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #f59e0b;
  font-weight: 700;
  font-size: 16px;
}

/* Actions */
.item-actions {
  background: rgba(15, 23, 42, 0.5);
  border-top: 1px solid rgba(148, 163, 184, 0.1);
  padding: 16px 24px !important;
}

.action-btn {
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  border-radius: 10px !important;
  padding: 0 24px !important;
  height: 44px !important;
}

.delete-btn {
  border-color: #dc2626 !important;
  color: #dc2626 !important;
}

.delete-btn:hover {
  background: rgba(220, 38, 38, 0.1) !important;
}

.cancel-btn {
  border-color: #64748b !important;
  color: #94a3b8 !important;
}

.cancel-btn:hover {
  background: rgba(100, 116, 139, 0.1) !important;
}

.save-btn {
  background: linear-gradient(135deg, #3b82f6 0%, #06b6d4 100%) !important;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3) !important;
}

.save-btn:hover {
  box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4) !important;
  transform: translateY(-1px);
}

/* Scrollbar */
.item-content::-webkit-scrollbar {
  width: 8px;
}

.item-content::-webkit-scrollbar-track {
  background: rgba(30, 41, 59, 0.3);
  border-radius: 4px;
}

.item-content::-webkit-scrollbar-thumb {
  background: rgba(148, 163, 184, 0.3);
  border-radius: 4px;
}

.item-content::-webkit-scrollbar-thumb:hover {
  background: rgba(148, 163, 184, 0.5);
}

/* Animaciones */
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

.item-card {
  animation: fadeIn 0.3s ease-out;
}
</style>