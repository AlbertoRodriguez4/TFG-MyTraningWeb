import { defineStore } from 'pinia'
import { ref } from 'vue'
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
  const activeSubscription = ref<Subscription | null>(null)
  const subscriptionHistory = ref<Subscription[]>([])
  const hasActiveSubscription = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)


  /**
   * Verifica si el usuario tiene una suscripción activa
   */
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
      error.value = 'No se pudo verificar el estado de la suscripción'
      hasActiveSubscription.value = false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Obtiene la suscripción activa del usuario
   */
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
      activeSubscription.value = data
      hasActiveSubscription.value = data.isActive

      return data
    } catch (err) {
      logger.error('Error getting active subscription:', err)
      error.value = 'No se pudo obtener la suscripción activa'
      activeSubscription.value = null
      hasActiveSubscription.value = false
      return null
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Obtiene el historial de suscripciones del usuario
   */
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
      error.value = 'No se pudo obtener el historial de suscripciones'
      return []
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Compra una nueva suscripción premium (simulación de 10€)
   */
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
          error: result.message || 'Error al procesar la compra'
        }
      }

      // Actualizar el estado después de la compra
      await getActiveSubscription()

      return {
        success: true,
        message: result.message || '¡Suscripción activada con éxito!'
      }
    } catch (err) {
      logger.error('Error purchasing subscription:', err)
      error.value = 'Error de conexión al procesar el pago'
      return {
        success: false,
        error: 'Error de conexión al procesar el pago'
      }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Renueva la suscripción existente
   */
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
          error: result.message || 'Error al renovar la suscripción'
        }
      }

      // Actualizar el estado después de la renovación
      await getActiveSubscription()

      return {
        success: true,
        message: result.message || '¡Suscripción renovada con éxito!'
      }
    } catch (err) {
      logger.error('Error renewing subscription:', err)
      error.value = 'Error de conexión al renovar'
      return {
        success: false,
        error: 'Error de conexión al renovar'
      }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Cancela una suscripción
   */
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
          error: result.message || 'Error al cancelar la suscripción'
        }
      }

      // Actualizar el estado después de la cancelación
      hasActiveSubscription.value = false
      activeSubscription.value = null

      return {
        success: true,
        message: result.message || 'Suscripción cancelada'
      }
    } catch (err) {
      logger.error('Error cancelling subscription:', err)
      error.value = 'Error de conexión al cancelar'
      return {
        success: false,
        error: 'Error de conexión al cancelar'
      }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Inicializa el estado de la suscripción (llamar al cargar la app)
   */
  async function initializeSubscription() {
    if (!localStorage.getItem('token')) {
      hasActiveSubscription.value = false
      activeSubscription.value = null
      return
    }

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
