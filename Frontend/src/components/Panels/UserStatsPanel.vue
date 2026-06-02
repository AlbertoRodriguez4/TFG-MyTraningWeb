<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/userStore'
import defaultAvatar from '@/assets/imgs/usuario.png'
import type { User } from '@/components/Models/User'

const { t } = useI18n()

const store = useUserStore()
const loggedUser = computed<User | null>(() => store.loggedUser)

const statList = ['strength', 'endurance', 'gold'] as const
type StatKey = typeof statList[number]

const colorMap: Record<StatKey, string> = {
  strength: '#ff4757',
  endurance: '#5352ed',
  gold: '#ffc107',
}

const iconMap: Record<StatKey, string> = {
  strength: 'mdi-arm-flex',
  endurance: 'mdi-run',
  gold: 'mdi-coin',
}

const labelMap = computed<Record<StatKey, string>>(() => ({
  strength: t('strength_label'),
  endurance: t('endurance_label'),
  gold: t('gold_label'),
}))
// Valores máximos alacanzables para cada estadística, usados para calcular el progreso
const maxValues: Record<StatKey, number> = {
  strength: 100000,
  endurance: 100000,
  gold: 1000000,
}

const calculateProgress = (stat: StatKey, value: number): number => {
  const maxValue = maxValues[stat]
  const percentage = (value / maxValue) * 100
  return Math.min(percentage, 100)
}
// Función para calcular el progreso de experiencia del usuario
const calculateXpProgress = (): number => {
  if (!loggedUser.value) return 0
  const currentXp = loggedUser.value.experience ?? 0
  const requiredXp = loggedUser.value.xpRequired ?? 1
  if (requiredXp === 0) return 100
  return Math.min((currentXp / requiredXp) * 100, 100)
}
// Función para formatear números grandes (e.g., 1500 -> 1.5K, 2000000 -> 2M) para evitar que se desborden en la interfaz
const formatNumber = (value: number): string => {
  if (value >= 1000000) {
    return `${(value / 1000000).toFixed(1)}M`
  } else if (value >= 1000) {
    return `${(value / 1000).toFixed(1)}K`
  }
  return value.toString()
}
</script>

<template>
  <div v-if="loggedUser && loggedUser.role !== 'userMaster'" class="user-stats-container">
    <div class="stats-wrapper">
      <!-- Card de Perfil -->
      <div class="profile-card">
        <div class="avatar-container">
          <div class="avatar-glow"></div>
          <div class="avatar-wrapper">
            <img
              :src="loggedUser.avatarUrl || defaultAvatar"
              alt="Avatar"
              class="avatar-image"
              @error="(e) => (e.target as HTMLImageElement).src = defaultAvatar"
            />
          </div>
          <div class="level-badge">
            <span class="level-icon"><v-icon>mdi-lightning-bolt</v-icon></span>
            <span class="level-text">LVL {{ loggedUser.level }}</span>
          </div>
        </div>
        <div class="user-info">
          <h3 class="user-name">{{ loggedUser.name }}</h3>
          <div class="streak-badge">
            <span class="streak-icon"><v-icon>mdi-fire</v-icon></span>
            <span class="streak-text">{{ loggedUser.consistencyStreak }} {{ $t('days_count') }}</span>
          </div>
        </div>

        <!-- Barra de Experiencia -->
        <div class="xp-container">
          <div class="xp-header">
            <span class="xp-icon"><v-icon>mdi-star-four-points</v-icon></span>
            <span class="xp-label">{{ $t('experiencia') }}</span>
          </div>
          <div class="xp-progress-wrapper">
            <div class="xp-progress-bar">
              <div class="xp-progress-fill" :style="{ width: `${calculateXpProgress()}%` }">
                <div class="xp-progress-shine"></div>
              </div>
            </div>
            <div class="xp-values">
              <span class="xp-current">{{ formatNumber(loggedUser.experience ?? 0) }}</span>
              <span class="xp-separator">/</span>
              <span class="xp-required">{{ formatNumber(loggedUser.xpRequired ?? 0) }}</span>
            </div>
          </div>
          <div class="xp-remaining">
            <span class="xp-remaining-icon"><v-icon>mdi-target</v-icon></span>
            <span>{{ formatNumber(loggedUser.xpRemaining ?? 0) }} XP para nivel {{ Number(loggedUser.level) + 1 }}</span>
          </div>
        </div>
      </div>

      <!-- Card de todas las estadísticas del usuario  -->
      <div class="stats-card">
        <h2 class="stats-title">
          <span class="title-icon"><v-icon>mdi-chart-bar</v-icon></span>
          {{ $t('statistics') }}
        </h2>
        <div class="stats-grid">
          <div v-for="stat in statList" :key="stat" class="stat-item">
            <div class="stat-header">
              <span class="stat-icon"><v-icon :class="['mdi', iconMap[stat]]"></v-icon></span>
              <span class="stat-label">{{ labelMap[stat] }}</span>
            </div>
            <div class="progress-container">
              <div class="progress-bar">
                <div class="progress-fill" :style="{
                  width: `${calculateProgress(stat, loggedUser[stat] ?? 0)}%`,
                  backgroundColor: colorMap[stat]
                }">
                  <div class="progress-shine"></div>
                </div>
              </div>
              <div class="stat-values">
                <span class="stat-value" :style="{ color: colorMap[stat] }">
                  {{ formatNumber(loggedUser[stat] ?? 0) }}
                </span>
                <span class="stat-max">/ {{ formatNumber(maxValues[stat]) }}</span>
              </div>
            </div>
            <div class="stat-percentage">
              {{ calculateProgress(stat, loggedUser[stat] ?? 0).toFixed(1) }}%
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.user-stats-container {
  width: 100%;
  padding: 1rem;
}

.stats-wrapper {
  display: flex;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  flex-wrap: wrap;
  justify-content: center;
}

/* Profile Card */
.profile-card {
  background: linear-gradient(135deg, rgba(26, 26, 46, 0.95), rgba(22, 33, 62, 0.95));
  border-radius: 24px;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  min-width: 280px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  border: 2px solid rgba(255, 193, 7, 0.3);
  position: relative;
  overflow: hidden;
}

.profile-card::before {
  content: "";
  position: absolute;
  top: -50%;
  right: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(255, 193, 7, 0.1) 0%, transparent 70%);
  animation: rotate 10s linear infinite;
}

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.avatar-container {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  z-index: 1;
}

.avatar-glow {
  position: absolute;
  width: 190px;
  height: 190px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(255, 193, 7, 0.4), transparent);
  animation: pulse-glow 2s ease-in-out infinite;
}

@keyframes pulse-glow {
  0%, 100% { transform: scale(1); opacity: 0.5; }
  50% { transform: scale(1.1); opacity: 0.8; }
}

/* Nuevo wrapper para el borde animado del avatar */
.avatar-wrapper {
  position: relative;
  width: 168px;
  height: 168px;
  border-radius: 50%;
  padding: 4px;
  background: linear-gradient(135deg, #ffc107, #ff9800, #ffc107);
  background-size: 200% 200%;
  animation: border-spin 3s linear infinite;
  box-shadow: 0 8px 32px rgba(255, 193, 7, 0.5);
  z-index: 1;
}

@keyframes border-spin {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

.avatar-image {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
  display: block;
  background-color: #1a1a2e;
}

.level-badge {
  background: linear-gradient(135deg, #ffc107, #ff9800);
  padding: 0.8rem 1.5rem;
  border-radius: 50px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 4px 15px rgba(255, 193, 7, 0.5);
  font-weight: bold;
  font-size: 1.2rem;
  color: #1a1a2e;
}

.level-icon {
  font-size: 1.5rem;
  animation: zap 1.5s ease-in-out infinite;
}

@keyframes zap {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.2); }
}

.user-info {
  text-align: center;
  z-index: 1;
}

.user-name {
  color: #ffc107;
  font-size: 1.8rem;
  margin-bottom: 0.5rem;
  font-weight: bold;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.streak-badge {
  background: rgba(255, 107, 107, 0.2);
  border: 2px solid #ff6b6b;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: #ff6b6b;
  font-weight: bold;
}

.streak-icon {
  font-size: 1.2rem;
  animation: flicker 2s ease-in-out infinite;
}

@keyframes flicker {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

/* XP Container */
.xp-container {
  width: 100%;
  background: rgba(139, 92, 246, 0.1);
  border: 2px solid rgba(139, 92, 246, 0.4);
  border-radius: 16px;
  padding: 1.2rem;
  z-index: 1;
}

.xp-header {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin-bottom: 0.8rem;
}

.xp-icon {
  font-size: 1.4rem;
  animation: sparkle 2s ease-in-out infinite;
}

@keyframes sparkle {
  0%, 100% { transform: scale(1) rotate(0deg); }
  50% { transform: scale(1.15) rotate(10deg); }
}

.xp-label {
  color: #a78bfa;
  font-weight: bold;
  font-size: 1.1rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.xp-progress-wrapper {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  margin-bottom: 0.6rem;
}

.xp-progress-bar {
  flex: 1;
  height: 20px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  overflow: hidden;
  position: relative;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.3);
}

.xp-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #8b5cf6, #a78bfa);
  border-radius: 10px;
  position: relative;
  transition: width 0.5s ease;
  overflow: hidden;
  box-shadow: 0 0 10px rgba(139, 92, 246, 0.6);
}

.xp-progress-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  animation: shine 2s infinite;
}

@keyframes shine {
  to { left: 100%; }
}

.xp-values {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  color: #a78bfa;
  font-weight: bold;
  font-size: 0.95rem;
  min-width: 100px;
}

.xp-current { color: #c4b5fd; }
.xp-separator { color: rgba(167, 139, 250, 0.5); }
.xp-required { color: rgba(167, 139, 250, 0.7); }

.xp-remaining {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  color: #e9d5ff;
  font-size: 0.9rem;
  font-weight: 600;
  justify-content: center;
}

.xp-remaining-icon { font-size: 1rem; }

/* Stats Card */
.stats-card {
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.95), rgba(240, 240, 240, 0.95));
  border-radius: 24px;
  padding: 2rem;
  flex: 1;
  min-width: 400px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  border: 2px solid rgba(255, 193, 7, 0.3);
}

.stats-title {
  color: #1a1a2e;
  font-size: 1.8rem;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: bold;
}

.title-icon { font-size: 2rem; }

.stats-grid {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.stat-item {
  background: white;
  padding: 1.5rem;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

.stat-header {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  margin-bottom: 1rem;
}

.stat-icon { font-size: 2rem; }

.stat-label {
  font-size: 1.3rem;
  font-weight: bold;
  color: #1a1a2e;
}

.progress-container {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.progress-bar {
  flex: 1;
  height: 24px;
  background: #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  position: relative;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.1);
}

.progress-fill {
  height: 100%;
  border-radius: 12px;
  position: relative;
  transition: width 0.5s ease;
  overflow: hidden;
}

.progress-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.4), transparent);
  animation: shine 2s infinite;
}

.stat-values {
  display: flex;
  align-items: baseline;
  gap: 0.3rem;
  min-width: 120px;
  justify-content: flex-end;
}

.stat-value {
  font-size: 1.4rem;
  font-weight: bold;
  text-align: right;
}

.stat-max {
  font-size: 1rem;
  color: #757575;
  font-weight: 600;
}

.stat-percentage {
  text-align: right;
  font-size: 0.9rem;
  color: #757575;
  font-weight: 600;
  margin-top: 0.3rem;
}

/* Responsive */
@media (max-width: 768px) {
  .stats-wrapper {
    flex-direction: column;
    gap: 1.5rem;
  }

  .profile-card,
  .stats-card {
    min-width: 100%;
  }

  .avatar-wrapper {
    width: 128px;
    height: 128px;
  }

  .avatar-glow {
    width: 148px;
    height: 148px;
  }

  .user-name { font-size: 1.5rem; }
  .stats-title { font-size: 1.5rem; }
  .stat-values { min-width: 100px; }
  .stat-value { font-size: 1.2rem; }
  .stat-max { font-size: 0.9rem; }
  .xp-values { font-size: 0.85rem; min-width: 80px; }
  .xp-remaining { font-size: 0.85rem; }
}

@media (max-width: 480px) {
  .user-stats-container { padding: 0.5rem; }

  .profile-card,
  .stats-card {
    padding: 1.5rem;
  }

  .avatar-wrapper {
    width: 108px;
    height: 108px;
  }

  .level-badge {
    padding: 0.6rem 1rem;
    font-size: 1rem;
  }

  .stat-item { padding: 1rem; }
  .stat-label { font-size: 1.1rem; }
  .stat-values { min-width: 90px; }
  .stat-value { font-size: 1.1rem; }
  .stat-max { font-size: 0.85rem; }
  .stat-percentage { font-size: 0.8rem; }
  .xp-container { padding: 1rem; }
  .xp-label { font-size: 1rem; }
  .xp-values { font-size: 0.8rem; }
  .xp-remaining { font-size: 0.8rem; }
}
</style>