import { ref, reactive } from 'vue'

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
      errors.email = 'El email es requerido'
      isEmailValid.value = false
      return false
    }

    if (!isValid) {
      errors.email = 'El email no es válido'
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
      { test: (p: string) => p.length >= 8, message: 'Al menos 8 caracteres' },
      { test: (p: string) => /[A-Z]/.test(p), message: 'Una mayúscula' },
      { test: (p: string) => /[a-z]/.test(p), message: 'Una minúscula' },
      { test: (p: string) => /[0-9]/.test(p), message: 'Un número' },
      { test: (p: string) => /[!@#$%^&*(),.?":{}|<>]/.test(p), message: 'Un carácter especial' }
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
      errors.password = 'La contraseña es requerida'
      isPasswordValid.value = false
      return false
    }

    if (password.length < 8) {
      errors.password = 'La contraseña debe tener al menos 8 caracteres'
      isPasswordValid.value = false
      return false
    }

    if (!/[A-Z]/.test(password)) {
      errors.password = 'La contraseña debe incluir una mayúscula'
      isPasswordValid.value = false
      return false
    }

    if (!/[a-z]/.test(password)) {
      errors.password = 'La contraseña debe incluir una minúscula'
      isPasswordValid.value = false
      return false
    }

    if (!/[0-9]/.test(password)) {
      errors.password = 'La contraseña debe incluir un número'
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
      errors.confirmPassword = 'Debes confirmar la contraseña'
      return false
    }

    if (password !== confirmPassword) {
      errors.confirmPassword = 'Las contraseñas no coinciden'
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
      errors.name = 'El nombre es requerido'
      return false
    }

    if (name.trim().length < 2) {
      errors.name = 'El nombre debe tener al menos 2 caracteres'
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
