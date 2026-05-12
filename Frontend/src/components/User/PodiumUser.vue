<script setup lang="ts">
import defaultAvatar from '@/assets/imgs/usuario.png'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface EquipItem {
  name: string
  bonus: number
}

interface User {
  id: number
  name: string
  level: number
  strength: number
  endurance: number
  avatarUrl?: string
  equippedStrengthItem?: EquipItem
  equippedEnduranceItem?: EquipItem
}

interface Props {
  user: User
  position: 1 | 2 | 3
}

const props = defineProps<Props>()

const getAvatar = (user: User) => user.avatarUrl || defaultAvatar

const positionConfig = {
  1: {
    medalColor: '#FFD700',
    borderColor: '#FFD700',
    ringClass: 'gold',
    cardClass: 'gold',
    pedestalClass: 'gold',
    badgeText: t('champion'),
    width: 280,
    padding: '2rem 1.5rem',
    glowColor: 'rgba(255, 215, 0, 0.4)'
  },
  2: {
    medalColor: '#C0C0C0',
    borderColor: '#C0C0C0',
    ringClass: 'silver',
    cardClass: 'silver',
    pedestalClass: 'silver',
    badgeText: '',
    width: 240,
    padding: '1.5rem',
    glowColor: 'rgba(192, 192, 192, 0.3)'
  },
  3: {
    medalColor: '#CD7F32',
    borderColor: '#CD7F32',
    ringClass: 'bronze',
    cardClass: 'bronze',
    pedestalClass: 'bronze',
    badgeText: '',
    width: 240,
    padding: '1.5rem',
    glowColor: 'rgba(205, 127, 50, 0.3)'
  }
}

const config = positionConfig[props.position]
const isGold = props.position === 1
const ringClass = config.ringClass
const cardClass = config.cardClass
const pedestalClass = config.pedestalClass
const badgeText = config.badgeText

const getEquipItem = () => {
  return props.user.equippedStrengthItem || props.user.equippedEnduranceItem
}
</script>

<template>
  <div class="podium-user" :class="`position-${position}`">
    <!-- Crown for first place -->
    <div v-if="isGold" class="crown-wrapper">
      <v-icon class="crown-icon">mdi-crown</v-icon>
      <div class="crown-rays">
        <div class="ray" v-for="i in 5" :key="i" :style="{
          transform: `rotate(${-40 + i * 20}deg)`,
          animationDelay: `${i * 0.15}s`
        }"></div>
      </div>
    </div>

    <!-- Position Ribbon -->
    <div class="position-ribbon" :class="ringClass">
      <span>{{ position }}º</span>
    </div>

    <div class="user-card" :class="cardClass" :style="{ width: `${config.width}px`, padding: config.padding }">
      <!-- Card Glow Effect -->
      <div class="card-glow" :style="{ background: `radial-gradient(ellipse at top, ${config.glowColor}, transparent 70%)` }"></div>
      <div class="card-shine"></div>

      <!-- Medal Badge -->
      <div
        class="medal-badge"
        :class="{ 'badge-gold': isGold }"
        :style="{ borderColor: config.medalColor }"
      >
        <span class="medal-number">{{ position }}</span>
        <v-icon class="medal-icon" :style="{ color: config.medalColor }">
          {{ position === 1 ? 'mdi-trophy' : position === 2 ? 'mdi-medal' : 'mdi-medal-outline' }}
        </v-icon>
        <div class="medal-glow" :style="{ background: `radial-gradient(circle, ${config.medalColor}44, transparent)` }"></div>
      </div>

      <!-- Avatar -->
      <div class="avatar-wrapper">
        <div class="avatar-ring-outer" :class="ringClass"></div>
        <div class="avatar-ring-inner" :class="ringClass"></div>
        <img
          :src="getAvatar(user)"
          :alt="user.name"
          class="avatar"
          :class="`avatar-${ringClass}`"
        />
        <!-- Level Badge -->
        <div class="level-badge" :class="ringClass">
          <v-icon size="14">mdi-shield-star</v-icon>
          <span>{{ user.level }}</span>
        </div>
      </div>

      <!-- User Info -->
      <h3 class="user-name" :class="{ 'name-gold': isGold }">
        <span class="name-text">{{ user.name }}</span>
        <div class="name-underline" v-if="isGold"></div>
      </h3>

      <!-- Champion Badge -->
      <div v-if="isGold" class="champion-badge">
        <v-icon size="14">mdi-star</v-icon>
        <span>{{ badgeText }}</span>
        <v-icon size="14">mdi-star</v-icon>
      </div>

      <!-- Stats Bar -->
      <div class="stats-bar">
        <div class="mini-stat strength">
          <div class="stat-icon-wrapper">
            <v-icon size="16">mdi-arm-flex</v-icon>
          </div>
          <span class="stat-value">{{ user.strength }}</span>
          <div class="stat-bar-fill strength-fill"></div>
        </div>
        <div class="mini-stat endurance">
          <div class="stat-icon-wrapper">
            <v-icon size="16">mdi-heart-pulse</v-icon>
          </div>
          <span class="stat-value">{{ user.endurance }}</span>
          <div class="stat-bar-fill endurance-fill"></div>
        </div>
      </div>

      <!-- Equipment -->
      <div
        v-if="getEquipItem()"
        class="equipment"
        :class="{ 'equipment-gold': isGold }"
      >
        <div class="equip-icon">
          <v-icon size="16" :color="isGold ? '#FFD700' : 'rgba(255,255,255,0.6)'">mdi-sword-cross</v-icon>
        </div>
        <span class="equip-name" :class="{ 'equip-gold': isGold }">
          {{ getEquipItem()?.name }}
        </span>
        <span class="equip-bonus" :class="{ 'bonus-gold': isGold }">
          +{{ getEquipItem()?.bonus }}
        </span>
      </div>
    </div>

    <!-- Pedestal -->
    <div class="pedestal" :class="pedestalClass">
      <div class="pedestal-front">
        <span class="pedestal-num">{{ position }}</span>
        <div class="pedestal-decoration" :style="{ background: config.medalColor }"></div>
      </div>
      <div class="pedestal-top" :style="{ background: config.medalColor }"></div>
    </div>

    <!-- Floor Shadow -->
    <div class="floor-shadow" :style="{ background: `radial-gradient(ellipse, ${config.glowColor} 0%, transparent 70%)` }"></div>
  </div>
</template>

<style scoped>
.podium-user {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
}

.podium-user.position-1 {
  order: 2;
  z-index: 30;
}

.podium-user.position-2 {
  order: 1;
  z-index: 20;
}

.podium-user.position-3 {
  order: 3;
  z-index: 10;
}

/* Crown */
.crown-wrapper {
  position: relative;
  margin-bottom: 0.5rem;
  animation: crown-float 2s ease-in-out infinite;
}

.crown-icon {
  font-size: 3rem !important;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 0 20px rgba(255, 215, 0, 0.8));
}

.crown-rays {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100px;
  height: 100px;
}

.ray {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 3px;
  height: 20px;
  background: linear-gradient(to top, rgba(255, 215, 0, 0.8), transparent);
  transform-origin: center bottom;
  border-radius: 2px;
  animation: ray-pulse 1.5s ease-in-out infinite;
}

@keyframes crown-float {
  0%, 100% { transform: translateY(0) rotate(-5deg); }
  50% { transform: translateY(-12px) rotate(5deg); }
}

@keyframes ray-pulse {
  0%, 100% { opacity: 0.3; height: 15px; }
  50% { opacity: 1; height: 25px; }
}

/* Position Ribbon */
.position-ribbon {
  position: absolute;
  top: -10px;
  right: -10px;
  width: 45px;
  height: 45px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
  font-size: 1.1rem;
  color: #000;
  z-index: 10;
  animation: ribbon-pulse 2s ease-in-out infinite;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.position-ribbon.gold {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  box-shadow: 0 0 30px rgba(255, 215, 0, 0.6);
}

.position-ribbon.silver {
  background: linear-gradient(135deg, #E8E8E8 0%, #C0C0C0 100%);
  box-shadow: 0 0 20px rgba(192, 192, 192, 0.4);
}

.position-ribbon.bronze {
  background: linear-gradient(135deg, #E4A059 0%, #CD7F32 100%);
  box-shadow: 0 0 20px rgba(205, 127, 50, 0.4);
}

@keyframes ribbon-pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

/* User Card */
.user-card {
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.95) 0%, rgba(15, 23, 42, 0.98) 100%);
  backdrop-filter: blur(20px);
  border-radius: 28px;
  text-align: center;
  position: relative;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  overflow: hidden;
}

.card-glow {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 50%;
  opacity: 0.5;
  pointer-events: none;
}

.card-shine {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent 30%, rgba(255, 255, 255, 0.05) 50%, transparent 70%);
  transform: rotate(45deg);
  transition: all 0.6s ease;
  opacity: 0;
}

.user-card:hover {
  transform: translateY(-12px) scale(1.03);
}

.user-card:hover .card-shine {
  opacity: 1;
}

.user-card.gold {
  border: 3px solid #FFD700;
  box-shadow:
    0 20px 60px rgba(255, 215, 0, 0.3),
    0 0 80px rgba(255, 215, 0, 0.2),
    inset 0 1px 0 rgba(255, 215, 0, 0.3);
}

.user-card.gold:hover {
  box-shadow:
    0 25px 70px rgba(255, 215, 0, 0.5),
    0 0 100px rgba(255, 215, 0, 0.3),
    inset 0 1px 0 rgba(255, 215, 0, 0.5);
}

.user-card.silver {
  border: 2px solid #C0C0C0;
  box-shadow:
    0 15px 50px rgba(192, 192, 192, 0.25),
    0 0 60px rgba(192, 192, 192, 0.15);
}

.user-card.bronze {
  border: 2px solid #CD7F32;
  box-shadow:
    0 15px 50px rgba(205, 127, 50, 0.25),
    0 0 60px rgba(205, 127, 50, 0.15);
}

/* Medal Badge */
.medal-badge {
  position: absolute;
  top: -20px;
  right: 25px;
  width: 55px;
  height: 55px;
  border-radius: 50%;
  background: linear-gradient(135deg, #1a1f2e 0%, #2a3548 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  border: 3px solid;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
  z-index: 5;
}

.badge-gold {
  width: 65px;
  height: 65px;
  top: -25px;
  right: 30px;
  animation: badge-pulse 2s ease-in-out infinite;
}

@keyframes badge-pulse {
  0%, 100% {
    box-shadow: 0 4px 25px rgba(255, 215, 0, 0.5);
  }
  50% {
    box-shadow: 0 4px 35px rgba(255, 215, 0, 0.8);
  }
}

.medal-number {
  position: absolute;
  font-size: 1.1rem;
  font-weight: 900;
  color: #fff;
  z-index: 2;
}

.medal-icon {
  position: absolute;
  opacity: 0.15;
  font-size: 2.5rem !important;
}

.medal-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
  border-radius: 50%;
  pointer-events: none;
}

/* Avatar Wrapper */
.avatar-wrapper {
  position: relative;
  display: inline-block;
  margin: 1.5rem 0 1rem;
}

.avatar-ring-outer,
.avatar-ring-inner {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 50%;
  border: 2px solid;
}

.avatar-ring-outer {
  animation: ring-rotate 10s linear infinite;
}

.avatar-ring-inner {
  animation: ring-rotate 8s linear infinite reverse;
}

.avatar-ring-outer.gold,
.avatar-ring-inner.gold {
  border-color: #FFD700;
}

.avatar-ring-outer.silver,
.avatar-ring-inner.silver {
  border-color: #C0C0C0;
}

.avatar-ring-outer.bronze,
.avatar-ring-inner.bronze {
  border-color: #CD7F32;
}

.avatar-ring-outer {
  width: 120px;
  height: 120px;
}

.avatar-ring-inner {
  width: 100px;
  height: 100px;
}

@keyframes ring-rotate {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

.avatar {
  position: relative;
  z-index: 2;
  border-radius: 50%;
  object-fit: cover;
  transition: all 0.3s ease;
}

.avatar-gold {
  width: 95px;
  height: 95px;
  border: 4px solid #FFD700;
  box-shadow:
    0 0 25px rgba(255, 215, 0, 0.6),
    0 0 50px rgba(255, 215, 0, 0.3);
}

.avatar-silver {
  width: 80px;
  height: 80px;
  border: 3px solid #C0C0C0;
  box-shadow: 0 0 20px rgba(192, 192, 192, 0.5);
}

.avatar-bronze {
  width: 70px;
  height: 70px;
  border: 3px solid #CD7F32;
  box-shadow: 0 0 20px rgba(205, 127, 50, 0.5);
}

.user-card:hover .avatar {
  transform: scale(1.05);
}

/* Level Badge */
.level-badge {
  position: absolute;
  bottom: 5px;
  right: 5px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  padding: 0.3rem 0.6rem;
  border-radius: 14px;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  font-weight: 700;
  color: #fff;
  border: 2px solid rgba(20, 30, 48, 0.9);
  z-index: 3;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
}

.level-badge.gold {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  color: #000;
}

/* User Name */
.user-name {
  position: relative;
  font-size: 1.25rem;
  font-weight: 800;
  color: #fff;
  margin: 0.75rem 0 0.5rem;
  text-shadow: 2px 2px 8px rgba(0, 0, 0, 0.8);
}

.name-gold {
  font-size: 1.5rem;
}

.name-text {
  position: relative;
  z-index: 2;
}

.name-gold .name-text {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: gradient-shift 4s ease infinite;
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

.name-underline {
  position: absolute;
  bottom: -8px;
  left: 50%;
  transform: translateX(-50%);
  width: 80%;
  height: 3px;
  background: linear-gradient(90deg, transparent, #FFD700, transparent);
  border-radius: 2px;
}

/* Champion Badge */
.champion-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  font-weight: 800;
  color: #FFD700;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.15), rgba(255, 215, 0, 0.05));
  padding: 0.4rem 1rem;
  border-radius: 20px;
  border: 1px solid rgba(255, 215, 0, 0.4);
  letter-spacing: 2px;
  margin: 0.5rem 0 1rem;
  box-shadow: 0 4px 15px rgba(255, 215, 0, 0.2);
}

/* Stats Bar */
.stats-bar {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin: 1rem 0;
}

.mini-stat {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.4rem;
  background: rgba(0, 0, 0, 0.3);
  padding: 0.6rem 1rem;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.stat-bar-fill {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  opacity: 0.5;
}

.strength-fill {
  background: linear-gradient(90deg, #FF4757, #FF6B81);
}

.endurance-fill {
  background: linear-gradient(90deg, #00D2FF, #00A8CC);
}

.mini-stat:hover {
  transform: translateY(-3px);
}

.stat-icon-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
}

.strength .stat-icon-wrapper .v-icon {
  color: #FF4757;
}

.endurance .stat-icon-wrapper .v-icon {
  color: #00D2FF;
}

.stat-value {
  font-size: 0.95rem;
  font-weight: 700;
  color: #fff;
}

/* Equipment */
.equipment {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.6rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.equipment-gold {
  border-top-color: rgba(255, 215, 0, 0.4);
}

.equip-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 50%;
}

.equip-name {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.7);
  max-width: 130px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.equip-gold {
  color: #FFD700;
  font-weight: 600;
}

.equip-bonus {
  font-size: 0.75rem;
  font-weight: 800;
  padding: 0.2rem 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  color: rgba(255, 255, 255, 0.8);
}

.bonus-gold {
  background: rgba(255, 215, 0, 0.2);
  color: #FFD700;
}

/* Pedestal */
.pedestal {
  position: relative;
  margin-top: 0.5rem;
  perspective: 500px;
}

.pedestal-front {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 8px 8px 0 0;
  overflow: hidden;
}

.pedestal-num {
  font-size: 2.5rem;
  font-weight: 900;
  color: rgba(0, 0, 0, 0.5);
  text-shadow: 0 1px 2px rgba(255, 255, 255, 0.2);
}

.pedestal-decoration {
  position: absolute;
  bottom: 8px;
  width: 60%;
  height: 4px;
  border-radius: 2px;
  opacity: 0.6;
}

.pedestal-top {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  height: 10px;
  border-radius: 4px 4px 0 0;
}

.pedestal.gold {
  width: 150px;
  height: 100px;
}

.pedestal.gold .pedestal-front {
  background: linear-gradient(180deg, #FFD700 0%, #FFA500 100%);
  box-shadow:
    0 0 40px rgba(255, 215, 0, 0.5),
    inset 0 1px 0 rgba(255, 255, 255, 0.3);
}

.pedestal.gold .pedestal-top {
  width: 140px;
  background: linear-gradient(180deg, #FFE55C 0%, #FFD700 100%);
}

.pedestal.silver {
  width: 120px;
  height: 70px;
}

.pedestal.silver .pedestal-front {
  background: linear-gradient(180deg, #E8E8E8 0%, #C0C0C0 100%);
  box-shadow:
    0 0 30px rgba(192, 192, 192, 0.4),
    inset 0 1px 0 rgba(255, 255, 255, 0.3);
}

.pedestal.silver .pedestal-top {
  width: 110px;
  background: linear-gradient(180deg, #F0F0F0 0%, #E8E8E8 100%);
}

.pedestal.bronze {
  width: 110px;
  height: 55px;
}

.pedestal.bronze .pedestal-front {
  background: linear-gradient(180deg, #E4A059 0%, #CD7F32 100%);
  box-shadow:
    0 0 30px rgba(205, 127, 50, 0.4),
    inset 0 1px 0 rgba(255, 255, 255, 0.2);
}

.pedestal.bronze .pedestal-top {
  width: 100px;
  background: linear-gradient(180deg, #E4A059 0%, #D49049 100%);
}

/* Floor Shadow */
.floor-shadow {
  position: absolute;
  bottom: -30px;
  width: 120px;
  height: 20px;
  filter: blur(10px);
  opacity: 0.5;
  pointer-events: none;
}

/* Responsive */
@media (max-width: 768px) {
  .podium-user.position-1,
  .podium-user.position-2,
  .podium-user.position-3 {
    order: unset;
  }

  .user-card,
  .user-card.gold {
    width: 280px !important;
  }

  .pedestal.gold,
  .pedestal.silver,
  .pedestal.bronze {
    height: 60px;
  }

  .avatar-ring-outer {
    width: 100px;
    height: 100px;
  }

  .avatar-ring-inner {
    width: 80px;
    height: 80px;
  }
}

@media (max-width: 480px) {
  .crown-icon {
    font-size: 2.2rem !important;
  }

  .user-name {
    font-size: 1.1rem;
  }

  .name-gold {
    font-size: 1.3rem;
  }

  .position-ribbon {
    width: 38px;
    height: 38px;
    font-size: 1rem;
  }
}
</style>
