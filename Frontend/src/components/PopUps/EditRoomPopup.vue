<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { useRoomStore } from '@/stores/RoomStore';
import { logger } from '@/utils/logger';

const store = useRoomStore()

const props = defineProps({
  isVisible: Boolean,
  room: {
    type: Object as () => {
      id: number
      name: string
      minlevel: number
      minstats: number
      minconsistency: number
      description?: string
      date?: string
    },
    required: true
  }
})

const emit = defineEmits(['close', 'edit'])

const dialogVisible = ref(props.isVisible)

// Snackbar state
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarType = ref<'success' | 'error'>('success')

watch(() => props.isVisible, (val) => {
  dialogVisible.value = val
})

watch(dialogVisible, (val) => {
  if (!val) emit('close')
})

function closePopup() {
  dialogVisible.value = false
}

function showSnackbar(text: string, type: 'success' | 'error' = 'success') {
  snackbarText.value = text
  snackbarType.value = type
  snackbar.value = true
  setTimeout(() => {
    snackbar.value = false
  }, 3000)
}

const editedRoom = reactive({
  id: 0,
  name: '',
  minlevel: 0,
  minstats: 0,
  minconsistency: 0,
  description: '',
  date: ''
})

watch(() => props.room, (newRoom) => {
  if (newRoom) {
    editedRoom.id = newRoom.id
    editedRoom.name = newRoom.name
    editedRoom.minlevel = newRoom.minlevel
    editedRoom.minstats = newRoom.minstats
    editedRoom.minconsistency = newRoom.minconsistency
    editedRoom.description = newRoom.description ?? ''
    editedRoom.date = newRoom.date ?? ''
  }
}, { immediate: true })

const handleEdit = async () => {
  try {
    const updatedRoom = {
      id: editedRoom.id,
      name: editedRoom.name,
      minlevel: editedRoom.minlevel,
      minstats: editedRoom.minstats,
      minconsistency: editedRoom.minconsistency,
      description: editedRoom.description,
      date: editedRoom.date
    }

    const response = await store.editRoom(editedRoom.id, updatedRoom)

    if (response !== null) {
      showSnackbar("Sala editada correctamente", 'success')
      closePopup()
      emit('edit')
    } else {
      showSnackbar("Hubo un problema al editar la sala", 'error')
    }
  } catch (error) {
    logger.error('Error editing room:', error)
    showSnackbar("Hubo un problema al editar la sala", 'error')
  }
}
</script>

<template>
  <Transition name="popup">
    <div v-if="dialogVisible" class="popup-overlay" @click="closePopup">
      <div class="popup-content" @click.stop>
        <div class="popup-header">
          <div class="header-icon-wrapper">
            <div class="header-icon"><v-icon>mdi-pencil</v-icon></div>
            <div ></div>
          </div>
          <h2 class="popup-title">Editar Sala</h2>
          <button @click="closePopup" class="close-btn" type="button">
            <span><v-icon>mdi-close</v-icon></span>
          </button>
        </div>

        <div class="popup-body">
          <form @submit.prevent="handleEdit">
            <div class="form-group">
              <label class="form-label">
                <span class="label-icon"><v-icon>mdi-tag</v-icon></span>
                <span>Nombre de la sala</span>
              </label>
              <input
                v-model="editedRoom.name"
                type="text"
                class="form-input"
                placeholder="Ej: Gimnasio Elite"
                required
              />
            </div>

            <div class="form-group">
              <label class="form-label">
                <span class="label-icon"><v-icon>mdi-chart-bar</v-icon></span>
                <span>Nivel mínimo</span>
              </label>
              <input
                v-model.number="editedRoom.minlevel"
                type="number"
                min="1"
                class="form-input"
                placeholder="Ej: 10"
                required
              />
            </div>

            <div class="form-group">
              <label class="form-label">
                <span class="label-icon"><v-icon>mdi-arm-flex</v-icon></span>
                <span>Stats mínimas</span>
              </label>
              <input
                v-model.number="editedRoom.minstats"
                type="number"
                min="0"
                class="form-input"
                placeholder="Ej: 50"
                required
              />
            </div>

            <div class="form-group">
              <label class="form-label">
                <span class="label-icon"><v-icon>mdi-target</v-icon></span>
                <span>Consistencia mínima (%)</span>
              </label>
              <input
                v-model.number="editedRoom.minconsistency"
                type="number"
                min="0"
                max="100"
                class="form-input"
                placeholder="Ej: 80"
                required
              />
            </div>
          </form>
        </div>

        <div class="popup-footer">
          <button @click="closePopup" class="btn btn-cancel" type="button">
            <span class="btn-icon"><v-icon>mdi-close</v-icon></span>
            <span>Cancelar</span>
          </button>
          <button @click="handleEdit" class="btn btn-save" type="button">
            <span class="btn-icon"><v-icon>mdi-content-save</v-icon></span>
            <span>Guardar Cambios</span>
          </button>
        </div>
      </div>
    </div>
  </Transition>

  <Transition name="snackbar">
    <div v-if="snackbar" :class="['snackbar', `snackbar-${snackbarType}`]">
      <span class="snackbar-icon">
        <v-icon>{{ snackbarType === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}</v-icon>
      </span>
      <span class="snackbar-text">{{ snackbarText }}</span>
    </div>
  </Transition>
</template>

<style scoped>
/* Overlay */
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

/* Contenedor principal */
.popup-content {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 24px;
  width: 100%;
  max-width: 500px;
  border: 2px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.5);
  overflow: hidden;
}

/* Header */
.popup-header {
  position: relative;
  padding: 2rem;
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1) 0%, rgba(37, 99, 235, 0.05) 100%);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-icon-wrapper {
  position: relative;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.header-icon {
  font-size: 2rem;
  z-index: 2;
  position: relative;
}

.popup-title {
  flex: 1;
  font-size: 1.75rem;
  font-weight: 700;
  color: #f8fafc;
  margin: 0;
}

.close-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #e2e8f0;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  background: rgba(239, 68, 68, 0.2);
  border-color: rgba(239, 68, 68, 0.5);
  transform: rotate(90deg);
}

/* Body */
.popup-body {
  padding: 2rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.95rem;
  font-weight: 600;
  color: #cbd5e1;
  margin-bottom: 0.75rem;
}

.label-icon {
  font-size: 1.2rem;
}

.form-input {
  width: 100%;
  padding: 1rem 1.25rem;
  font-size: 1rem;
  color: #f8fafc;
  background: rgba(255, 255, 255, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  transition: all 0.3s ease;
  outline: none;
}

.form-input::placeholder {
  color: #64748b;
}

.form-input:focus {
  background: rgba(255, 255, 255, 0.08);
  border-color: #3b82f6;
  box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1);
}

/* Remove spinner for number inputs */
.form-input[type="number"]::-webkit-inner-spin-button,
.form-input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.form-input[type="number"] {
  -moz-appearance: textfield;
}

/* Footer */
.popup-footer {
  padding: 1.5rem 2rem;
  background: rgba(0, 0, 0, 0.2);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.btn {
  padding: 1rem 1.5rem;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn-icon {
  font-size: 1.1rem;
}

.btn-cancel {
  background: rgba(255, 255, 255, 0.05);
  color: #e2e8f0;
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.btn-cancel:hover {
  background: rgba(239, 68, 68, 0.1);
  border-color: rgba(239, 68, 68, 0.3);
  color: #fca5a5;
}

.btn-save {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
  border: 2px solid transparent;
}

.btn-save:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(59, 130, 246, 0.4);
}

.btn-save:active {
  transform: translateY(0);
}

/* Snackbar */
.snackbar {
  position: fixed;
  top: 2rem;
  left: 50%;
  transform: translateX(-50%);
  padding: 1rem 1.5rem;
  border-radius: 12px;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-weight: 600;
  font-size: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  z-index: 2000;
  min-width: 300px;
  backdrop-filter: blur(10px);
}

.snackbar-success {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.95) 0%, rgba(21, 128, 61, 0.95) 100%);
  border: 2px solid rgba(134, 239, 172, 0.3);
  color: white;
}

.snackbar-error {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.95) 0%, rgba(185, 28, 28, 0.95) 100%);
  border: 2px solid rgba(252, 165, 165, 0.3);
  color: white;
}

.snackbar-icon {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  flex-shrink: 0;
}

.snackbar-text {
  flex: 1;
}

/* Snackbar transitions */
.snackbar-enter-active,
.snackbar-leave-active {
  transition: all 0.3s ease;
}

.snackbar-enter-from {
  opacity: 0;
  transform: translateX(-50%) translateY(-20px);
}

.snackbar-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-20px);
}

/* Transitions */
.popup-enter-active,
.popup-leave-active {
  transition: all 0.3s ease;
}

.popup-enter-from,
.popup-leave-to {
  opacity: 0;
}

.popup-enter-from .popup-content,
.popup-leave-to .popup-content {
  transform: scale(0.9) translateY(20px);
}

/* Responsive */
@media (max-width: 640px) {
  .popup-content {
    border-radius: 20px;
  }

  .popup-header {
    padding: 1.5rem;
  }

  .popup-title {
    font-size: 1.5rem;
  }

  .popup-body {
    padding: 1.5rem;
  }

  .popup-footer {
    grid-template-columns: 1fr;
    padding: 1.25rem 1.5rem;
  }

  .btn-cancel {
    order: 2;
  }

  .btn-save {
    order: 1;
  }

  .snackbar {
    top: 1rem;
    left: 1rem;
    right: 1rem;
    transform: none;
    min-width: auto;
  }

  .snackbar-enter-from {
    transform: translateY(-20px);
  }

  .snackbar-leave-to {
    transform: translateY(-20px);
  }
}
</style>