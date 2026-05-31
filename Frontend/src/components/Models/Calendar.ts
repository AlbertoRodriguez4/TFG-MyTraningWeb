import type { Routines } from './Routines'

export interface MonthlyStats {
  key: string
  name: string
  year: number
  month: number
  total: number
  completed: number
  pending: number
  completionRate: number
  totalXP: number
  routines: Routines[]
}
