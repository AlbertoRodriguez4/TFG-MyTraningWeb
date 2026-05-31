
const isDevelopment = import.meta.env.DEV
const isLogsEnabled = import.meta.env.VITE_ENABLE_LOGS === 'true'

const shouldLog = isDevelopment || isLogsEnabled

const LOG_PREFIX = '[TFG]'

export const logger = {
    // 
  log: (...args: any[]) => {
    if (shouldLog) {
      console.log(LOG_PREFIX, ...args)
    }
  },

  
  error: (...args: any[]) => {
    console.error(LOG_PREFIX, ...args)
  },

  
  warn: (...args: any[]) => {
    if (shouldLog) {
      console.warn(LOG_PREFIX, ...args)
    }
  },

  
  debug: (...args: any[]) => {
    if (shouldLog) {
      console.debug(LOG_PREFIX, ...args)
    }
  },

  
  info: (...args: any[]) => {
    if (shouldLog) {
      console.info(LOG_PREFIX, ...args)
    }
  }
}
