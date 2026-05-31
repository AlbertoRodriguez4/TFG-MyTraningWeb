export interface ValidationState {
  cardNumber: boolean
  cardHolder: boolean
  expiryDate: boolean
  cvv: boolean
}

export interface PlanItem {
  name: string
  period: string
  price: number
  originalPrice: number
  features: string[]
}
