
// URL base para todas las peticiones API (para no tener que repertirla en cada función de fectch)
// El objetivo de este archivo es centralizart las configuraciones de las apis y reducir la repetición  de código usando variables globales
export const API_BASE_URL = 'http://localhost:6873'

// Headers comunes
const COMMON_HEADERS = {
  'Content-Type': 'application/json'
}

// Función para obtener los headers de autenticación, incluyendo el token JWT si está presente en localStorage, se utiliza en las funciones de fetch para incluir ç
// el token en las peticiones al backend y así autenticar al usuario
export const getAuthHeaders = (token?: string) => {
  const authToken = token || localStorage.getItem('token')
  return {
    ...COMMON_HEADERS,
    'Authorization': authToken ? `Bearer ${authToken}` : ''
  }
}

//FUnción para verificar que el token no ha expirado
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

// Comprobar que el token almacenado en memora es valido/la expiración no ha ocurrido
export const hasValidToken = (): boolean => {
  const token = localStorage.getItem('token')
  if (!token) {
    return false
  }
  return !isTokenExpired(token)
}
