import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

const BASE_URL = API_BASE_URL

export interface Subscription {
  subscriptionId: number
  userId: number
  userName: string
  userEmail: string
  startDate: string
  endDate: string
  isActive: boolean
  planType: string
  monthlyPrice: number
  nextRenewalDate?: string
}

export const useSubscriptionStore = defineStore('subscription', () => {
  const { t } = useI18n()
  const activeSubscription = ref<Subscription | null>(null)
  const subscriptionHistory = ref<Subscription[]>([])
  const hasActiveSubscription = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)


  // Función para verificar si el usuario tiene una suscripción activa
  async function checkSubscription() {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/check`, {
        method: 'GET',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      hasActiveSubscription.value = data.hasActiveSubscription
    } catch (err) {
      logger.error('Error checking subscription:', err)
      error.value = t('subscription_check_error')
      hasActiveSubscription.value = false
    } finally {
      isLoading.value = false
    }
  }

  // Función para obtener los detalles de la suscripción activa del usuario
  async function getActiveSubscription() {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/my-subscription`, {
        method: 'GET',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      if (!response.ok) {
        if (response.status === 404) {
          activeSubscription.value = null
          hasActiveSubscription.value = false
          return null
        }
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      // Settear los valores de la suscripción activa y el estado de si tiene una suscripción activa o no
      activeSubscription.value = data
      hasActiveSubscription.value = data.isActive

      return data
    } catch (err) {
      logger.error('Error getting active subscription:', err)
      error.value = t('subscription_active_error')
      activeSubscription.value = null
      hasActiveSubscription.value = false
      return null
    } finally {
      isLoading.value = false
    }
  }

  // Función para obtener el historial de suscripciones del usuario
  async function getSubscriptionHistory() {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/my-history`, {
        method: 'GET',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      subscriptionHistory.value = data

      return data
    } catch (err) {
      logger.error('Error getting subscription history:', err)
      error.value = t('subscription_history_error')
      return []
    } finally {
      isLoading.value = false
    }
  }

  // Función para realizar la compra de una suscripción (Es una simulación)
  async function purchaseSubscription(): Promise<{ success: boolean; message?: string; error?: string }> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/purchase`, {
        method: 'POST',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      const result = await response.json()

      if (!response.ok) {
        return {
          success: false,
          error: result.message || t('subscription_purchase_error')
        }
      }

      // Actualizar el estado después de la compra
      await getActiveSubscription()

      return {
        success: true,
        message: result.message || t('subscription_activated')
      }
    } catch (err) {
      logger.error('Error purchasing subscription:', err)
      error.value = t('subscription_payment_connection_error')
      return {
        success: false,
        error: t('subscription_payment_connection_error')
      }
    } finally {
      isLoading.value = false
    }
  }

  // Función para renovar la suscripción existente  
  async function renewSubscription(): Promise<{ success: boolean; message?: string; error?: string }> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/renew`, {
        method: 'POST',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      const result = await response.json()

      if (!response.ok) {
        return {
          success: false,
          error: result.message || t('subscription_renew_error')
        }
      }

      // Actualizar el estado después de la renovación
      await getActiveSubscription()

      return {
        success: true,
        message: result.message || t('subscription_renewed')
      }
    } catch (err) {
      logger.error('Error renewing subscription:', err)
      error.value = t('subscription_renew_connection_error')
      return {
        success: false,
        error: t('subscription_renew_connection_error')
      }
    } finally {
      isLoading.value = false
    }
  }

  // Función para cancelar la suscripción activa del usuario
  async function cancelSubscription(subscriptionId: number): Promise<{ success: boolean; message?: string; error?: string }> {
    isLoading.value = true
    error.value = null

    try {
      const response = await fetch(`${BASE_URL}/api/Subscription/${subscriptionId}`, {
        method: 'DELETE',
        mode: 'cors',
        headers: getAuthHeaders()
      })

      const result = await response.json()

      if (!response.ok) {
        return {
          success: false,
          error: result.message || t('subscription_cancel_error')
        }
      }

      // Actualizar el estado después de la cancelación
      hasActiveSubscription.value = false
      activeSubscription.value = null

      return {
        success: true,
        message: result.message || t('subscription_cancelled')
      }
    } catch (err) {
      logger.error('Error cancelling subscription:', err)
      error.value = t('subscription_cancel_connection_error')
      return {
        success: false,
        error: t('subscription_cancel_connection_error')
      }
    } finally {
      isLoading.value = false
    }
  }

  // Función para inicializar el estado de la suscripción al cargar la aplicación (Necesario para el header )
  async function initializeSubscription() {
    if (!localStorage.getItem('token')) {
      hasActiveSubscription.value = false
      activeSubscription.value = null
      return
    }
 // Comprobar si el usuario tiene una suscripción activa y obtener los detalles de la suscripción activa e historial de suscripciones al cargar la aplicación 
 // para tener el estado actualizado desde el inicio
    await checkSubscription()
    if (hasActiveSubscription.value) {
      await getActiveSubscription()
    }
  }

  return {
    // State
    activeSubscription,
    subscriptionHistory,
    hasActiveSubscription,
    isLoading,
    error,
    // Actions
    checkSubscription,
    getActiveSubscription,
    getSubscriptionHistory,
    purchaseSubscription,
    renewSubscription,
    cancelSubscription,
    initializeSubscription
  }
})
