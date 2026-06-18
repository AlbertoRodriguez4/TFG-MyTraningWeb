<template>
  <div class="profile-hero">
    <v-container>
      <div class="hero-content">
        <div class="avatar-section">
          <div class="avatar-wrapper">
            <v-avatar size="120" class="avatar-main" v-if="!store.loggedUser?.avatarUrl">
              {{ userInitials }}
            </v-avatar>
            <v-img v-else :src="store.loggedUser.avatarUrl" class="avatar-image" alt="User Avatar" />
            <v-btn icon="mdi-camera" size="small" class="avatar-edit-btn" variant="flat" :loading="uploading"
              @click="triggerFileInput" />
            <input ref="fileInput" type="file" accept="image/*" style="display: none" @change="handleFileSelect" />
          </div>

          <div class="user-info">
            <h1 class="user-name">{{ userName }}</h1>
            <p class="user-email">{{ userEmail }}</p>
            <div class="user-badge">
              <v-icon size="small">mdi-star</v-icon>
              <span>{{ $t('header.level') }} {{ userLevel }} • {{ $t('racha_consistencia') }}: {{ consistencyStreak }} {{ $t('days_count') }}</span>
            </div>
          </div>
        </div>

        <div class="stats-section">
          <div class="stat-item">
            <span class="stat-icon"><v-icon>mdi-lightning-bolt</v-icon></span>
            <div class="stat-content">
              <p class="stat-label">{{ $t('strength_label') }}</p>
              <p class="stat-value">{{ userStrength }}</p>
            </div>
          </div>
          <div class="stat-item">
            <span class="stat-icon"><v-icon>mdi-weather-windy</v-icon></span>
            <div class="stat-content">
              <p class="stat-label">{{ $t('endurance_label') }}</p>
              <p class="stat-value">{{ userEndurance }}</p>
            </div>
          </div>
          <div class="stat-item">
            <span class="stat-icon"><v-icon>mdi-gold</v-icon></span>
            <div class="stat-content">
              <p class="stat-label">{{ $t('gold_label') }}</p>
              <p class="stat-value">{{ userGold }}</p>
            </div>
          </div>
        </div>

        <div class="xp-progress">
          <div class="xp-header">
            <span class="xp-label">{{ $t('experiencia') }}</span>
            <span class="xp-text">{{ userExperience }} / {{ xpRequired }}</span>
          </div>
          <v-progress-linear :model-value="xpPercentage" color="#ffcc00" height="6" rounded />
        </div>
      </div>
    </v-container>

    <v-snackbar v-model="showNotification" :color="notificationType" :timeout="3000" location="top">
      {{ notificationMessage }}
    </v-snackbar>

    <v-dialog v-model="showPreviewDialog" max-width="400">
      <v-card>
        <v-card-title class="dialog-title">
          Cambiar Avatar
        </v-card-title>

        <v-card-text class="preview-content">
          <div v-if="previewImage" class="preview-wrapper">
            <img :src="previewImage" alt="Preview" class="preview-image" />
          </div>
          <div v-else class="preview-empty">
            <v-icon size="64" color="white">
              mdi-image
            </v-icon>
            <p>{{ $t('profile.selectImage') }}</p>
          </div>

          <div class="upload-progress" v-if="uploading">
            <v-progress-linear :model-value="uploadProgress" color="#ffcc00" height="4" />
            <p class="progress-text">{{ $t('profile.uploading') }} {{ uploadProgress }}%</p>
          </div>
        </v-card-text>

        <v-card-actions class="dialog-actions">
          <v-spacer />
          <v-btn variant="text" @click="closePreviewDialog" :disabled="uploading" class="cancel-btn">
            {{ $t('common.cancel') }}
          </v-btn>
          <v-btn color="#ffcc00" text-color="#000" variant="flat" @click="uploadToCloudinary" :loading="uploading">
            <v-icon start>mdi-upload</v-icon>
            {{ $t('save') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/stores/userStore'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const store = useUserStore()

// Constantes de Cloudinary
const CLOUDINARY_CLOUD_NAME = import.meta.env.VITE_CLOUDINARY_CLOUD_NAME
const CLOUDINARY_UPLOAD_PRESET = import.meta.env.VITE_CLOUDINARY_UPLOAD_PRESET

// Refs
const fileInput = ref<HTMLInputElement>()
const previewImage = ref<string>('')
const selectedFile = ref<File | null>(null)
const uploading = ref(false)
const uploadProgress = ref(0)
const showPreviewDialog = ref(false)

// Notificaciones
const showNotification = ref(false)
const notificationMessage = ref('')
const notificationType = ref<'success' | 'error'>('success')

// Computed
const userName = computed(() => store.loggedUser?.name || 'Usuario')
const userEmail = computed(() => store.loggedUser?.email || 'email@example.com')
const userLevel = computed(() => store.loggedUser?.level || 1)
const userStrength = computed(() => store.loggedUser?.strength || 0)
const userEndurance = computed(() => store.loggedUser?.endurance || 0)
const userGold = computed(() => store.loggedUser?.gold || 0)
const userExperience = computed(() => store.loggedUser?.experience || 0)
const xpRequired = computed(() => store.loggedUser?.xpRequired || 100)
const consistencyStreak = computed(() => store.loggedUser?.consistencyStreak || 0)

const xpPercentage = computed(() => {
  const current = userExperience.value
  const required = xpRequired.value
  return (current / required) * 100
})
// Generar iniciales del usuario para el avatar
const userInitials = computed(() => {
  return (store.loggedUser?.name || 'U')
    .split(' ')
    .map(n => n.charAt(0))
    .join('')
    .toUpperCase()
})

// Método para activar el input de archivo
const triggerFileInput = () => {
  fileInput.value?.click()
}
// Manejar selección de archivo
const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const files = target.files

  if (!files || files.length === 0) return

  const file = files[0]

  // Validar tipo de archivo
  if (!file.type.startsWith('image/')) {
    showErrorNotification(t('select_valid_image'))
    return
  }

  // Validar tamaño del archivo (máximo 15MB)
  if (file.size > 15 * 1024 * 1024) { // La formula es 15MB en bytes, es decir, 15 * 1024KB * 1024B
    showErrorNotification(t('image_max_15mb'))
    return
  }

  selectedFile.value = file

  // Create preview
  const reader = new FileReader()
  reader.onload = (e) => {
    previewImage.value = e.target?.result as string
    showPreviewDialog.value = true
  }
  reader.readAsDataURL(file)
}

const uploadToCloudinary = async () => {
  if (!selectedFile.value) return

  uploading.value = true
  uploadProgress.value = 0

  try {
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET)

    // Create XMLHttpRequest para rastrear el progreso
    const xhr = new XMLHttpRequest()

    // Track upload progress
    xhr.upload.addEventListener('progress', (event) => {
      if (event.lengthComputable) {
        uploadProgress.value = Math.round((event.loaded / event.total) * 100)
      }
    })

    // Usar Promises con XMLHttpRequest
    const uploadPromise = new Promise<string>((resolve, reject) => {
      xhr.addEventListener('load', () => {
        if (xhr.status === 200) {
          const response = JSON.parse(xhr.responseText)
          resolve(response.secure_url)
        } else {
          reject(new Error('Error en la subida'))
        }
      })

      xhr.addEventListener('error', () => {
        reject(new Error(t('connection_error')))
      })

      xhr.open(
        'POST',
        `https://api.cloudinary.com/v1_1/${CLOUDINARY_CLOUD_NAME}/image/upload`
      )
      xhr.send(formData)
    })

    const cloudinaryUrl = await uploadPromise

    // Actualizar avatar en el usuario
    if (store.loggedUser) {
      await store.editUser(store.loggedUser.id, {
        ...store.loggedUser,
        avatarUrl: cloudinaryUrl,
      })

      showSuccessNotification('¡Avatar actualizado exitosamente!')
      closePreviewDialog()
    }
  } catch (error) {
    logger.error('Error al subir imagen:', error)
    showErrorNotification('Error al subir la imagen. Intenta de nuevo.')
  } finally {
    uploading.value = false
    uploadProgress.value = 0
  }
}

const closePreviewDialog = () => {
  showPreviewDialog.value = false
  previewImage.value = ''
  selectedFile.value = null
  uploadProgress.value = 0

  // Reset file input
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const showSuccessNotification = (message: string) => {
  notificationMessage.value = message
  notificationType.value = 'success'
  showNotification.value = true
}

const showErrorNotification = (message: string) => {
  notificationMessage.value = message
  notificationType.value = 'error'
  showNotification.value = true
}
</script>

<style scoped>
.profile-hero {
  background: linear-gradient(180deg, rgba(255, 204, 0, 0.08) 0%, rgba(255, 204, 0, 0.03) 100%);
  border-bottom: 2px solid rgba(255, 204, 0, 0.2);
  padding: 3rem 0;
  margin-bottom: 2rem;
}

.hero-content {
  display: flex;
  align-items: center;
  gap: 3rem;
  flex-wrap: wrap;
}

.avatar-section {
  display: flex;
  align-items: center;
  gap: 2rem;
  flex: 1;
  min-width: 300px;
}

.avatar-wrapper {
  position: relative;
  flex-shrink: 0;
}

.avatar-main {
  background: linear-gradient(135deg, #ffcc00 0%, #ff9900 100%) !important;
  font-weight: 900;
  font-size: 2.5rem;
  color: #000;
  box-shadow: 0 8px 24px rgba(255, 204, 0, 0.3);
}

.avatar-image {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  box-shadow: 0 8px 24px rgba(255, 204, 0, 0.3);
  border: 3px solid rgba(255, 204, 0, 0.3);
  object-fit: cover;
}

.avatar-edit-btn {
  position: absolute;
  bottom: 0;
  right: 0;
  background: linear-gradient(135deg, #ffcc00, #ff9900) !important;
  color: #000 !important;
  box-shadow: 0 4px 12px rgba(255, 204, 0, 0.3);
  cursor: pointer;
  transition: all 0.2s ease;
}

.avatar-edit-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(255, 204, 0, 0.5);
}

.user-info {
  flex: 1;
  min-width: 200px;
}

.user-name {
  font-size: 2rem;
  font-weight: 900;
  color: #ffcc00;
  margin: 0 0 0.5rem 0;
  text-shadow: 0 2px 10px rgba(255, 204, 0, 0.3);
}

.user-email {
  color: rgba(255, 255, 255, 0.7);
  margin: 0 0 1rem 0;
  font-size: 1rem;
}

.user-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 204, 0, 0.1);
  border: 1px solid rgba(255, 204, 0, 0.3);
  border-radius: 8px;
  padding: 0.5rem 1rem;
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.9rem;
  width: fit-content;
}

.stats-section {
  display: flex;
  gap: 1.5rem;
  flex: 0 1 auto;
  flex-wrap: wrap;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  background: rgba(255, 204, 0, 0.05);
  border: 1px solid rgba(255, 204, 0, 0.15);
  border-radius: 8px;
  padding: 0.8rem 1.2rem;
  min-width: 140px;
}

.stat-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.stat-content {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
}

.stat-label {
  margin: 0;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.6);
  text-transform: uppercase;
  letter-spacing: 1px;
  font-weight: 600;
}

.stat-value {
  margin: 0;
  font-size: 1.3rem;
  font-weight: 900;
  color: #ffcc00;
}

/* XP Progress */
.xp-progress {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
  flex: 0 1 auto;
  min-width: 200px;
}

.xp-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.xp-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
  font-weight: 600;
}

.xp-text {
  color: #ffcc00;
  font-size: 0.85rem;
  font-weight: 700;
}

/* Dialog Styles */
.dialog-title {
  color: #ffcc00 !important;
  font-weight: 700;
}

.preview-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  min-height: 300px;
}

.preview-wrapper {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  overflow: hidden;
  background: rgba(255, 204, 0, 0.05);
}

.preview-image {
  max-width: 100%;
  max-height: 250px;
  object-fit: contain;
  border-radius: 8px;
}

.preview-empty {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  border: 2px dashed rgba(255, 204, 0, 0.3);
  background: rgba(255, 204, 0, 0.02);
  color: rgba(255, 255, 255, 0.5);
}

.preview-empty p {
  margin-top: 1rem;
  font-weight: 600;
}

.upload-progress {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.progress-text {
  margin: 0;
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.7);
  text-align: center;
  font-weight: 600;
}

.dialog-actions {
  padding: 1rem;
  gap: 0.5rem;
}
.cancel-btn {
    color:white  !important;   
}
/* Responsive */
@media (max-width: 1024px) {
  .profile-hero {
    padding: 2rem 0;
    margin-bottom: 1.5rem;
  }

  .hero-content {
    gap: 2rem;
  }

  .avatar-section {
    gap: 1.5rem;
    min-width: 250px;
  }

  .avatar-main {
    width: 100px !important;
    height: 100px !important;
    font-size: 2rem;
  }

  .avatar-image {
    width: 100px;
    height: 100px;
  }

  .user-name {
    font-size: 1.5rem;
  }

  .stats-section {
    gap: 1rem;
  }

  .stat-item {
    padding: 0.6rem 1rem;
    min-width: 130px;
  }

  .xp-progress {
    min-width: 180px;
  }
}

@media (max-width: 600px) {
  .profile-hero {
    padding: 1.5rem 0;
    margin-bottom: 1rem;
  }

  .hero-content {
    flex-direction: column;
    gap: 1.5rem;
  }

  .avatar-section {
    width: 100%;
    min-width: auto;
  }

  .avatar-main {
    width: 80px !important;
    height: 80px !important;
    font-size: 1.8rem;
  }

  .avatar-image {
    width: 80px;
    height: 80px;
  }

  .user-name {
    font-size: 1.25rem;
  }

  .stats-section {
    width: 100%;
    flex-direction: column;
    gap: 0.8rem;
  }

  .stat-item {
    min-width: auto;
  }

  .xp-progress {
    width: 100%;
    min-width: auto;
  }
}
</style>