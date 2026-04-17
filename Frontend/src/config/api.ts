/**
 * Configuración centralizada de la API
 *
 * Usa variables de entorno si están disponibles, sino usa valores por defecto
 */

// URL base para todas las peticiones API
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:6873'

// Timeout para las peticiones (en milisegundos)
export const API_TIMEOUT = 30000

// Headers comunes
export const COMMON_HEADERS = {
  'Content-Type': 'application/json'
}

/**
 * Obtiene los headers de autenticación incluyendo el token JWT
 * @param token - Token JWT opcional (si no se proporciona, se obtiene del localStorage)
 */
export const getAuthHeaders = (token?: string) => {
  const authToken = token || localStorage.getItem('token')
  return {
    ...COMMON_HEADERS,
    'Authorization': authToken ? `Bearer ${authToken}` : ''
  }
}

/**
 * Verifica si un token JWT ha expirado
 * @param token - Token JWT a verificar
 * @returns true si el token ha expirado o es inválido, false si es válido
 */
export const isTokenExpired = (token: string): boolean => {
  try {
    const tokenParts = token.split('.')
    if (tokenParts.length !== 3) {
      return true
    }

    const payload = JSON.parse(atob(tokenParts[1]))
    const expTimestamp = payload.exp

    // Si no hay expiración, el token es válido indefinidamente
    if (!expTimestamp) {
      return false
    }

    // Comprobar si ha expirado (añadimos 30 segundos de margen)
    return expTimestamp * 1000 < Date.now() - 30000
  } catch {
    return true
  }
}

/**
 * Verifica si el token actual en localStorage es válido
 * @returns true si hay un token válido, false en caso contrario
 */
export const hasValidToken = (): boolean => {
  const token = localStorage.getItem('token')
  if (!token) {
    return false
  }
  return !isTokenExpired(token)
}
