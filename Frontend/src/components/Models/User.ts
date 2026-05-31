export interface EquipItem {
  name: string
  bonus: number
}

export interface User {
  id: number
  name: string
  email?: string
  passwordhash?: string
  level: number
  strength: number
  endurance: number
  consistencyStreak?: number
  gold?: number
  role?: string
  experience?: number
  xpRequired?: number
  xpRemaining?: number
  avatarUrl?: string
  equippedStrengthItem?: EquipItem
  equippedEnduranceItem?: EquipItem
  equippedStrengthItemId?: number | null
  equippedEnduranceItemId?: number | null
}

export interface Stats {
  totalUsers: number
  activeToday: number
  totalCheckIns: number
}
