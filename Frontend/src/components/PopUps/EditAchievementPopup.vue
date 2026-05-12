<script setup lang="ts">
import { watch, reactive, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Achievement } from '@/stores/achievementStore'
import { useAchievementStore } from '@/stores/achievementStore'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const store = useAchievementStore()

const props = defineProps<{
  achievement: Achievement | null
  visible: boolean
}>()

const emit = defineEmits(['close', 'save', 'delete'])

const dialogVisible = ref(props.visible)
watch(() => props.visible, val => dialogVisible.value = val)
watch(dialogVisible, val => { if (!val) emit('close') })

const errorMessage = ref('')
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')

const categories = ['general', 'strength', 'endurance', 'consistency', 'social', 'exploration']
const requirementTypes = [
  'tasks_completed',
  'streak_days',
  'gold_earned',
  'level_reached',
  'strength_reached',
  'endurance_reached',
  'gyms_searched',
  'rooms_joined'
]

const edited = reactive<Achievement>({
  id: 0,
  name: '',
  description: '',
  iconUrl: 'mdi-trophy',
  category: 'general',
  requirementType: 'tasks_completed',
  requirementValue: 1,
  rewardGold: 0,
  rewardXP: 0,
  isSecret: false
})

watch(() => props.achievement, (newVal) => {
  if (newVal) {
    Object.assign(edited, { ...newVal })
  } else {
    Object.assign(edited, {
      id: 0,
      name: '',
      description: '',
      iconUrl: 'mdi-trophy',
      category: 'general',
      requirementType: 'tasks_completed',
      requirementValue: 1,
      rewardGold: 0,
      rewardXP: 0,
      isSecret: false
    })
  }
}, { immediate: true })

function closePopup() {
  dialogVisible.value = false
}

function showSnackbar(text: string, color: string = 'success') {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}

const isEdit = computed(() => !!props.achievement)

// Cloudinary config
const CLOUDINARY_CLOUD_NAME = import.meta.env.VITE_CLOUDINARY_CLOUD_NAME
const CLOUDINARY_UPLOAD_PRESET = import.meta.env.VITE_CLOUDINARY_UPLOAD_PRESET
const fileInput = ref<HTMLInputElement>()
const previewImage = ref<string>('')
const selectedFile = ref<File | null>(null)
const uploadingImage = ref(false)
const uploadProgress = ref(0)

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const files = target.files
  if (!files || files.length === 0) return
  const file = files[0]

  if (!file.type.startsWith('image/')) {
    errorMessage.value = t('select_valid_image')
    return
  }
  if (file.size > 5 * 1024 * 1024) {
    errorMessage.value = t('image_max_5mb')
    return
  }

  selectedFile.value = file
  const reader = new FileReader()
  reader.onload = (e) => {
    previewImage.value = e.target?.result as string
  }
  reader.readAsDataURL(file)
}

const uploadImage = async (): Promise<string | null> => {
  if (!selectedFile.value) return null
  uploadingImage.value = true
  uploadProgress.value = 0

  try {
    const formData = new FormData()
    formData.append('file', selectedFile.value)
    formData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET)

    const xhr = new XMLHttpRequest()
    xhr.upload.addEventListener('progress', (event) => {
      if (event.lengthComputable) {
        uploadProgress.value = Math.round((event.loaded / event.total) * 100)
      }
    })

    const uploadPromise = new Promise<string>((resolve, reject) => {
      xhr.addEventListener('load', () => {
        if (xhr.status === 200) {
          const response = JSON.parse(xhr.responseText)
          resolve(response.secure_url)
        } else {
          reject(new Error('Error en la subida'))
        }
      })
      xhr.addEventListener('error', () => reject(new Error(t('connection_error'))))
      xhr.open('POST', `https://api.cloudinary.com/v1_1/${CLOUDINARY_CLOUD_NAME}/image/upload`)
      xhr.send(formData)
    })

    const url = await uploadPromise
    return url
  } catch (error) {
    logger.error('Error al subir imagen:', error)
    return null
  } finally {
    uploadingImage.value = false
    uploadProgress.value = 0
  }
}

async function handleSave() {
  errorMessage.value = ''

  if (!edited.name || edited.name.trim().length < 2) {
    errorMessage.value = t('validation_name_required')
    return
  }
  if (!edited.description || edited.description.trim().length < 5) {
    errorMessage.value = t('description')
    return
  }
  if (edited.requirementValue < 1) {
    errorMessage.value = t('achievement_requirement_value')
    return
  }

  try {
    let iconUrl = edited.iconUrl.trim()
    if (selectedFile.value) {
      const uploaded = await uploadImage()
      if (uploaded) iconUrl = uploaded
      else {
        errorMessage.value = t('image_upload_error')
        return
      }
    }

    const payload = {
      name: edited.name.trim(),
      description: edited.description.trim(),
      iconUrl: iconUrl,
      category: edited.category,
      requirementType: edited.requirementType,
      requirementValue: edited.requirementValue,
      rewardGold: edited.rewardGold,
      rewardXP: edited.rewardXP,
      isSecret: edited.isSecret
    }

    if (isEdit.value && props.achievement) {
      await store.updateAchievement(props.achievement.id, payload)
      showSnackbar(t('achievement_updated'), 'success')
      emit('save')
    } else {
      await store.createAchievement(payload)
      showSnackbar(t('achievement_created'), 'success')
      emit('save')
    }
    closePopup()
    selectedFile.value = null
    previewImage.value = ''
  } catch (e: any) {
    errorMessage.value = isEdit.value ? t('error_updating_achievement') : t('error_creating_achievement')
  }
}

async function handleDelete() {
  if (!props.achievement) return
  try {
    await store.deleteAchievement(props.achievement.id)
    showSnackbar(t('achievement_deleted'), 'success')
    emit('delete')
    closePopup()
  } catch {
    showSnackbar(t('error_deleting_achievement'), 'error')
  }
}
</script>

<template>
  <v-dialog v-model="dialogVisible" max-width="700" class="achievement-dialog">
    <v-card class="achievement-card">
      <div class="achievement-header">
        <div class="header-content">
          <v-icon class="header-icon">mdi-trophy</v-icon>
          <h2 class="header-title">
            {{ isEdit ? $t('edit_achievement') : $t('create_achievement') }}
          </h2>
        </div>
        <v-btn icon class="close-button" @click="closePopup" size="small">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <v-card-text class="achievement-content">
        <v-alert v-if="errorMessage" type="error" variant="tonal" class="error-alert">
          {{ errorMessage }}
        </v-alert>

        <div class="form-row">
          <div class="form-group flex-2">
            <label class="field-label">
              <v-icon size="small" class="label-icon">mdi-format-title</v-icon>
              {{ $t('achievement_name') }}
            </label>
            <v-text-field v-model="edited.name" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
          <div class="form-group flex-1">
            <label class="field-label">
              <v-icon size="small" class="label-icon">mdi-emoticon</v-icon>
              {{ $t('achievement_icon') }}
            </label>
            <v-text-field v-model="edited.iconUrl" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
        </div>

        <div class="form-group">
          <label class="field-label">
            <v-icon size="small" class="label-icon">mdi-image</v-icon>
            {{ $t('achievement_image') }}
          </label>
          <div class="image-upload-row">
            <input ref="fileInput" type="file" accept="image/*" style="display: none" @change="handleFileSelect" />
            <v-btn variant="outlined" color="amber" size="small" @click="triggerFileInput" :loading="uploadingImage">
              <v-icon left>mdi-camera</v-icon>
              {{ selectedFile ? $t('change_image') : $t('select_image') }}
            </v-btn>
            <span v-if="selectedFile" class="file-name">{{ selectedFile.name }}</span>
          </div>
          <div v-if="uploadProgress > 0 && uploadingImage" class="upload-progress-bar">
            <v-progress-linear :model-value="uploadProgress" color="amber" height="6" rounded />
          </div>
          <div class="image-preview-wrapper mt-2">
            <img v-if="previewImage" :src="previewImage" class="image-preview" alt="Preview" />
            <img v-else-if="edited.iconUrl && edited.iconUrl.startsWith('http')" :src="edited.iconUrl" class="image-preview" alt="Current" />
          </div>
        </div>

        <div class="form-group">
          <label class="field-label">
            <v-icon size="small" class="label-icon">mdi-text-box</v-icon>
            {{ $t('achievement_description') }}
          </label>
          <v-textarea v-model="edited.description" variant="outlined" density="comfortable" hide-details rows="2" class="custom-input" />
        </div>

        <div class="form-row">
          <div class="form-group flex-1">
            <label class="field-label">{{ $t('achievement_category') }}</label>
            <v-select v-model="edited.category" :items="categories" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
          <div class="form-group flex-1">
            <label class="field-label">{{ $t('achievement_requirement_type') }}</label>
            <v-select v-model="edited.requirementType" :items="requirementTypes" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group flex-1">
            <label class="field-label">{{ $t('achievement_requirement_value') }}</label>
            <v-text-field v-model.number="edited.requirementValue" type="number" min="1" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
          <div class="form-group flex-1">
            <label class="field-label">{{ $t('achievement_reward_gold') }}</label>
            <v-text-field v-model.number="edited.rewardGold" type="number" min="0" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
          <div class="form-group flex-1">
            <label class="field-label">{{ $t('achievement_reward_xp') }}</label>
            <v-text-field v-model.number="edited.rewardXP" type="number" min="0" variant="outlined" density="comfortable" hide-details class="custom-input" />
          </div>
        </div>

        <div class="form-group secret-toggle">
          <v-checkbox v-model="edited.isSecret" :label="$t('achievement_is_secret')" color="amber" hide-details />
        </div>

        <div class="preview-card">
          <div class="preview-label">{{ $t('preview') }}</div>
          <div class="preview-content">
            <div class="preview-icon-wrapper">
              <img v-if="previewImage || (edited.iconUrl && edited.iconUrl.startsWith('http'))" :src="previewImage || edited.iconUrl" class="preview-image" alt="Achievement" />
              <v-icon v-else size="large">{{ edited.iconUrl || 'mdi-trophy' }}</v-icon>
            </div>
            <div class="preview-info">
              <div class="preview-name">{{ edited.name || '???' }}</div>
              <div class="preview-desc">{{ edited.description || '...' }}</div>
            </div>
            <div class="preview-rewards">
              <v-chip v-if="edited.rewardGold > 0" x-small color="amber" class="mr-1">+{{ edited.rewardGold }} 🪙</v-chip>
              <v-chip v-if="edited.rewardXP > 0" x-small color="purple">+{{ edited.rewardXP }} XP</v-chip>
            </div>
          </div>
        </div>
      </v-card-text>

      <v-card-actions class="achievement-actions">
        <v-btn v-if="isEdit" color="error" variant="outlined" @click="handleDelete" class="action-btn delete-btn">
          <v-icon left>mdi-delete</v-icon>
          {{ $t('delete') }}
        </v-btn>
        <v-spacer />
        <v-btn color="grey" variant="outlined" @click="closePopup" class="action-btn cancel-btn">
          {{ $t('cancelar') }}
        </v-btn>
        <v-btn color="primary" variant="elevated" @click="handleSave" class="action-btn save-btn">
          <v-icon left>mdi-content-save</v-icon>
          {{ $t('save_achievement') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="3000" location="top" rounded="pill">
    <div class="d-flex align-center">
      <v-icon class="mr-2">{{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}</v-icon>
      {{ snackbarText }}
    </div>
  </v-snackbar>
</template>

<style scoped>
.achievement-card {
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  border-radius: 16px !important;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5) !important;
}

.achievement-header {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
}

.achievement-header::before {
  content: '';
  position: absolute;
  inset: 0;
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
  font-size: 22px;
  font-weight: 700;
  color: white;
}

.close-button {
  background: rgba(255, 255, 255, 0.2) !important;
  color: white !important;
  position: relative;
  z-index: 1;
}

.achievement-content {
  padding: 24px !important;
  max-height: 70vh;
  overflow-y: auto;
}

.error-alert {
  margin-bottom: 16px;
  border-radius: 12px !important;
}

.form-row {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
}

.form-group {
  flex: 1;
  margin-bottom: 16px;
}

.flex-2 {
  flex: 2;
}

.field-label {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #94a3b8;
  font-size: 13px;
  font-weight: 600;
  margin-bottom: 8px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.label-icon {
  color: #f59e0b !important;
}

.custom-input :deep(.v-field) {
  border-radius: 12px;
  background: rgba(30, 41, 59, 0.5);
}

.custom-input :deep(.v-field__outline) {
  color: rgba(148, 163, 184, 0.3);
}

.custom-input :deep(.v-field--focused .v-field__outline) {
  color: #f59e0b;
}

.custom-input :deep(input),
.custom-input :deep(textarea) {
  color: white;
}

.secret-toggle {
  margin-top: -8px;
}

.preview-card {
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(30, 41, 59, 0.4) 100%);
  border: 1px solid rgba(148, 163, 184, 0.2);
  border-radius: 12px;
  padding: 16px;
  margin-top: 8px;
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
  background: rgba(245, 158, 11, 0.2);
  color: #f59e0b;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.preview-info {
  flex: 1;
  min-width: 0;
}

.preview-name {
  color: white;
  font-weight: 600;
  font-size: 15px;
  margin-bottom: 4px;
}

.preview-desc {
  color: #94a3b8;
  font-size: 13px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.preview-rewards {
  display: flex;
  flex-direction: column;
  gap: 4px;
  align-items: flex-end;
}

.achievement-actions {
  background: rgba(15, 23, 42, 0.5);
  border-top: 1px solid rgba(148, 163, 184, 0.1);
  padding: 16px 24px !important;
}

.action-btn {
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  border-radius: 10px !important;
  padding: 0 20px !important;
  height: 40px !important;
}

.delete-btn {
  border-color: #dc2626 !important;
  color: #dc2626 !important;
}

.cancel-btn {
  border-color: #64748b !important;
  color: #94a3b8 !important;
}

.save-btn {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%) !important;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3) !important;
}

.save-btn:hover {
  box-shadow: 0 6px 20px rgba(245, 158, 11, 0.4) !important;
}

.achievement-content::-webkit-scrollbar {
  width: 8px;
}

.achievement-content::-webkit-scrollbar-track {
  background: rgba(30, 41, 59, 0.3);
  border-radius: 4px;
}

.achievement-content::-webkit-scrollbar-thumb {
  background: rgba(148, 163, 184, 0.3);
  border-radius: 4px;
}

.image-upload-row {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.file-name {
  color: #94a3b8;
  font-size: 0.85rem;
}

.upload-progress-bar {
  margin-top: 0.5rem;
}

.image-preview-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  border-radius: 12px;
  overflow: hidden;
  background: rgba(30, 41, 59, 0.5);
  border: 1px solid rgba(148, 163, 184, 0.2);
}

.image-preview {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.preview-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.preview-icon-wrapper {
  overflow: hidden;
}

@media (max-width: 600px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }
}
</style>
