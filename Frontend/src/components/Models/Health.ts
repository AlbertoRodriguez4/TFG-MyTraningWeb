export interface HistoryItem {
  type: 'bmi' | 'calories'
  date: string
  bmi?: number
  tdee?: number
  weight?: number
  height?: number
}

export interface BMIData {
  weight: number | null
  height: number | null
  bmi?: number
  category?: string
}

export interface CaloriesData {
  bmr?: number
  tdee?: number
  macros?: {
    protein: number
    carbs: number
    fat: number
  }
}

export interface UserHealthProfile {
  weight?: number
  height?: number
  bmi?: number
  bmiCategory?: string
  age?: number
  gender?: 'male' | 'female'
  activity?: string
  goal?: string
  bmr?: number
  tdee?: number
  macros?: {
    protein: number
    carbs: number
    fat: number
  }
  lastUpdated?: string
}

export interface BMIResult {
  bmi: number
  category: string
}

export interface CaloriesResult {
  bmr: number
  tdee: number
  macros: {
    protein: number
    carbs: number
    fat: number
  }
}
