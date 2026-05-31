export interface ChatMessage {
  id: number
  role: string
  text: string
  time: string
  isTyping?: boolean
}

export interface Stat {
  emoji: string
  label: string
  value: string
  pct: number
  color: string
}

export interface QuickPrompt {
  icon: string
  text: string
}
