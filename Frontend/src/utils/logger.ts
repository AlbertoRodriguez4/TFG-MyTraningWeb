/**
 * Utilidad de logging condicional
 *
 * Los logs solo se muestran en desarrollo (process.env.NODE_ENV === 'development')
 * o cuando VITE_ENABLE_LOGS está activado en .env
 */

const isDevelopment = import.meta.env.DEV
const isLogsEnabled = import.meta.env.VITE_ENABLE_LOGS === 'true'

const shouldLog = isDevelopment || isLogsEnabled

const LOG_PREFIX = '[TFG]'

export const logger = {
  /**
   * Log de información general
   */
  log: (...args: any[]) => {
    if (shouldLog) {
      console.log(LOG_PREFIX, ...args)
    }
  },

  /**
   * Log de errores - siempre se muestra para facilitar debugging en prod
   */
  error: (...args: any[]) => {
    console.error(LOG_PREFIX, ...args)
  },

  /**
   * Log de advertencias
   */
  warn: (...args: any[]) => {
    if (shouldLog) {
      console.warn(LOG_PREFIX, ...args)
    }
  },

  /**
   * Log de información de debug
   */
  debug: (...args: any[]) => {
    if (shouldLog) {
      console.debug(LOG_PREFIX, ...args)
    }
  },

  /**
   * Log de información general
   */
  info: (...args: any[]) => {
    if (shouldLog) {
      console.info(LOG_PREFIX, ...args)
    }
  }
}
