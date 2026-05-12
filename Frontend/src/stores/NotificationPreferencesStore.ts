import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

const BASE_URL = API_BASE_URL

export interface NotificationPreferences {
  userId: number
  inactivityEnabled: boolean
  inactivityDays: number
  roomsEnabled: boolean
  purchasesEnabled: boolean
  subscriptionExpiryEnabled: boolean
}

export const useNotificationPreferencesStore = defineStore('notificationPreferences', () => {
  const { t } = useI18n()
  const preferences = ref<NotificationPreferences>({
    userId: 0,
    inactivityEnabled: true,
    inactivityDays: 3,
    roomsEnabled: true,
    purchasesEnabled: true,
    subscriptionExpiryEnabled: true
  })
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  /**
   * Obtiene las preferencias de notificación del usuario actual
   */
  async function fetchPreferences() {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/NotificationPreference/my-preferences`, {
        method: 'GET',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      preferences.value = data
    } catch (err) {
      logger.error('Error fetching notification preferences:', err)
      error.value = t('notification_load_error')
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Guarda las preferencias de notificación del usuario actual
   */
  async function savePreferences(): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/NotificationPreference/my-preferences`, {
        method: 'PUT',
        mode: 'cors',
        headers: getAuthHeaders(),
        body: JSON.stringify(preferences.value)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      preferences.value = data
      return true
    } catch (err) {
      logger.error('Error saving notification preferences:', err)
      error.value = t('notification_save_error')
      return false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Resetea las preferencias a los valores por defecto
   */
  async function resetDefaults(): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/NotificationPreference/reset`, {
        method: 'POST',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      preferences.value = data
      return true
    } catch (err) {
      logger.error('Error resetting notification preferences:', err)
      error.value = t('notification_reset_error')
      return false
    } finally {
      isLoading.value = false
    }
  }

  return {
    preferences,
    isLoading,
    error,
    fetchPreferences,
    savePreferences,
    resetDefaults
  }
})