import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import type { PasswordStrength, ValidationErrors } from '@/components/Models/Auth'

export function useAuthValidation() {
  const { t } = useI18n()
  const errors = reactive<ValidationErrors>({})
  const passwordStrength = ref<PasswordStrength>({ strength: 0, feedback: [] })
  const isPasswordValid = ref(false)
  const isEmailValid = ref(false)

  // Validar el correo electrónico usando regex y proporcionar mensajes de error específicos para campos vacíos o formato no válido
  const validateEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    const isValid = emailRegex.test(email)

    if (!email) {
      errors.email = t('validation_email_required')
      isEmailValid.value = false
      return false
    }

    if (!isValid) {
      errors.email = t('validation_email_not_valid')
      isEmailValid.value = false
      return false
    }

    errors.email = undefined
    isEmailValid.value = true
    return true
  }

  // Comprobar la fortaleza de la contrseña y proporcionar feedback al usuario sobre qué criterios se han cumplido o no, además de marcar la contraseña como válida si cumple 
  // al menos 4 de los 5 criterios establecidos
  const updatePasswordStrength = (password: string): void => {
    const strengthCriteria = [
      { test: (p: string) => p.length >= 8, message: t('validation_min_8_chars') },
      { test: (p: string) => /[A-Z]/.test(p), message: t('validation_one_uppercase') },
      { test: (p: string) => /[a-z]/.test(p), message: t('validation_one_lowercase') },
      { test: (p: string) => /[0-9]/.test(p), message: t('validation_one_number') },
      { test: (p: string) => /[!@#$%^&*(),.?":{}|<>]/.test(p), message: t('validation_one_special') }
    ]

    const passedCriteria = strengthCriteria.filter(criteria => criteria.test(password)) // Filtrar los criterios que se han cumplido
    passwordStrength.value = {
      strength: passedCriteria.length,
      feedback: passedCriteria.map(c => c.message) // Proporcionar feedback específico sobre qué criterios se han cumplido
    }
    isPasswordValid.value = passedCriteria.length >= 4
  }

  // Validar la contraseña asegurándose de que no esté vacía, cumpla con los requisitos mínimos de longitud y contenga mayúsculas, minúsculas y números, 
  // proporcionando mensajes de error específicos para cada caso
  const validatePassword = (password: string): boolean => {
    if (!password) {
      errors.password = t('validation_password_required')
      isPasswordValid.value = false
      return false
    }

    if (password.length < 8) {
      errors.password = t('validation_password_min_8')
      isPasswordValid.value = false
      return false
    }

    if (!/[A-Z]/.test(password)) {
      errors.password = t('validation_password_need_uppercase')
      isPasswordValid.value = false
      return false
    }

    if (!/[a-z]/.test(password)) {
      errors.password = t('validation_password_need_lowercase')
      isPasswordValid.value = false
      return false
    }

    if (!/[0-9]/.test(password)) {
      errors.password = t('validation_password_need_number')
      isPasswordValid.value = false
      return false
    }

    errors.password = undefined
    isPasswordValid.value = true
    return true
  }

  // Se comprueba que las contraseñas coincidan 
  const validateConfirmPassword = (password: string, confirmPassword: string): boolean => {
    if (!confirmPassword) {
      errors.confirmPassword = t('validation_confirm_required')
      return false
    }

    if (password !== confirmPassword) {
      errors.confirmPassword = t('validation_passwords_match')
      return false
    }

    errors.confirmPassword = undefined
    return true
  }

  // Validar que el nombnre del usuario sea correcto 
  const validateName = (name: string): boolean => {
    if (!name || name.trim().length === 0) {
      errors.name = t('validation_name_required')
      return false
    }

    if (name.trim().length < 2) {
      errors.name = t('validation_name_min_2')
      return false
    }

    errors.name = undefined
    return true
  }

  const clearErrors = () => {
    errors.email = undefined
    errors.password = undefined
    errors.confirmPassword = undefined
    errors.name = undefined
  }

  return {
    errors,
    passwordStrength,
    isPasswordValid,
    isEmailValid,
    validateEmail,
    validatePassword,
    validateConfirmPassword,
    validateName,
    updatePasswordStrength,
    clearErrors
  }
}
