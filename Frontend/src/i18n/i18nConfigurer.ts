import { createI18n } from 'vue-i18n'
import en from '../i18n/en.json'
import fr from '../i18n/fr.json'
import es from '../i18n/es.json'

const i18n = createI18n({ //obtenemos la configuracion de i18n, que viene de los 3 jsons creados en la carpeta, y ponemos cono lenguaje por defecto el español
  legacy: false, 
  globalInjection: true,
  locale: 'es', 
  fallbackLocale: 'en',  // Si el idioma seleccionado no tiene una traducción para una clave específica, se usará la traducción en inglés como respaldo
  messages: {
    en,
    es,
    fr
  }
})

export default i18n
