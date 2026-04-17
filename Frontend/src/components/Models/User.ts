export interface User {
  id: number;
  name: string;
  email: string;
  passwordhash: string;
  level: number;
  strength: number;
  endurance: number;
  gold: number;
  role: string;
  consistencyStreak: number;
  experience: number;
  xpRequired: number;
  xpRemaining: number;
  equippedStrengthItemId: number | null;
  equippedEnduranceItemId: number | null;
  avatarUrl: string;
  isEmailVerified?: boolean;
}
