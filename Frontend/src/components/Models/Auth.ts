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
