import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export const useThemeStore = defineStore('theme', () => {
  const savedTheme = localStorage.getItem('app-theme') || 'dark'
  const currentTheme = ref(savedTheme)

  // Aplicar tema al elemento html cuando cambia
  const applyTheme = (theme: string) => {
    if (theme === 'dark') {
      document.documentElement.classList.add('dark')
      document.documentElement.classList.remove('light')
      document.documentElement.setAttribute('data-theme', 'dark')
    } else if (theme === 'light') {
      document.documentElement.classList.add('light')
      document.documentElement.classList.remove('dark')
      document.documentElement.setAttribute('data-theme', 'light')
    } else if (theme === 'auto') {
      // Auto sigue las preferencias del sistema
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
      const autoTheme = prefersDark ? 'dark' : 'light'
      if (autoTheme === 'dark') {
        document.documentElement.classList.add('dark')
        document.documentElement.classList.remove('light')
        document.documentElement.setAttribute('data-theme', 'dark')
      } else {
        document.documentElement.classList.add('light')
        document.documentElement.classList.remove('dark')
        document.documentElement.setAttribute('data-theme', 'light')
      }
    }
  }

  // Aplicar tema inicial al cargar el store
  applyTheme(currentTheme.value)

  // Watch para actualizar cuando cambia el tema
  watch(currentTheme, (newTheme) => {
    localStorage.setItem('app-theme', newTheme)
    applyTheme(newTheme)
  })

  // Escuchar cambios en preferencias del sistema cuando está en modo auto
  if (currentTheme.value === 'auto') {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
      const autoTheme = e.matches ? 'dark' : 'light'
      if (autoTheme === 'dark') {
        document.documentElement.classList.add('dark')
        document.documentElement.classList.remove('light')
        document.documentElement.setAttribute('data-theme', 'dark')
      } else {
        document.documentElement.classList.add('light')
        document.documentElement.classList.remove('dark')
        document.documentElement.setAttribute('data-theme', 'light')
      }
    })
  }

  const setTheme = (theme: string) => {
    currentTheme.value = theme
  }

  return {
    currentTheme,
    setTheme
  }
})
