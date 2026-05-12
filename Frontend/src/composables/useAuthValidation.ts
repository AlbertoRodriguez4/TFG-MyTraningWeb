import { ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'

export interface PasswordStrength {
  strength: number
  feedback: string[]
}

export interface ValidationErrors {
  email?: string
  password?: string
  confirmPassword?: string
  name?: string
}

/**
 * Composable para validación de formularios de autenticación
 * Centraliza la lógica de validación de email y contraseña
 */
export function useAuthValidation() {
  const { t } = useI18n()
  const errors = reactive<ValidationErrors>({})
  const passwordStrength = ref<PasswordStrength>({ strength: 0, feedback: [] })
  const isPasswordValid = ref(false)
  const isEmailValid = ref(false)

  /**
   * Valida un email usando regex estándar
   */
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

  /**
   * Evalúa la fortaleza de la contraseña
   * Criterios: longitud, mayúsculas, minúsculas, números, caracteres especiales
   */
  const updatePasswordStrength = (password: string): void => {
    const strengthCriteria = [
      { test: (p: string) => p.length >= 8, message: t('validation_min_8_chars') },
      { test: (p: string) => /[A-Z]/.test(p), message: t('validation_one_uppercase') },
      { test: (p: string) => /[a-z]/.test(p), message: t('validation_one_lowercase') },
      { test: (p: string) => /[0-9]/.test(p), message: t('validation_one_number') },
      { test: (p: string) => /[!@#$%^&*(),.?":{}|<>]/.test(p), message: t('validation_one_special') }
    ]

    const passedCriteria = strengthCriteria.filter(criteria => criteria.test(password))
    passwordStrength.value = {
      strength: passedCriteria.length,
      feedback: passedCriteria.map(c => c.message)
    }
    isPasswordValid.value = passedCriteria.length >= 4
  }

  /**
   * Valida una contraseña según criterios de fortaleza
   */
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

  /**
   * Valida que las contraseñas coincidan
   */
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

  /**
   * Valida un nombre de usuario
   */
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

  /**
   * Limpia todos los errores de validación
   */
  const clearErrors = () => {
    errors.email = undefined
    errors.password = undefined
    errors.confirmPassword = undefined
    errors.name = undefined
  }

  /**
   * Valida todos los campos del formulario de registro
   */
  const validateRegisterForm = (
    email: string,
    password: string,
    confirmPassword: string,
    name: string
  ): boolean => {
    const isEmailValid = validateEmail(email)
    const isPasswordValid = validatePassword(password)
    const isConfirmPasswordValid = validateConfirmPassword(password, confirmPassword)
    const isNameValid = validateName(name)

    return isEmailValid && isPasswordValid && isConfirmPasswordValid && isNameValid
  }

  /**
   * Valida todos los campos del formulario de login
   */
  const validateLoginForm = (email: string, password: string): boolean => {
    const isEmailValid = validateEmail(email)
    const isPasswordValid = validatePassword(password)

    return isEmailValid && isPasswordValid
  }

  return {
    // State
    errors,
    passwordStrength,
    isPasswordValid,
    isEmailValid,
    // Methods
    validateEmail,
    validatePassword,
    validateConfirmPassword,
    validateName,
    updatePasswordStrength,
    clearErrors,
    validateRegisterForm,
    validateLoginForm
  }
}
