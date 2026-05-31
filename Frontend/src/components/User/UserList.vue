<script setup lang="ts">
import defaultAvatar from '@/assets/imgs/usuario.png'
import { useI18n } from 'vue-i18n'
import type { User } from '@/components/Models/User'

const { t } = useI18n()

interface Props {
  users: User[]
  loading: boolean
}

defineProps<Props>()

const getAvatar = (user: User) => user.avatarUrl || defaultAvatar
// Mostrar item equipado
const getEquipItem = (user: User) => {
  return user.equippedStrengthItem || user.equippedEnduranceItem
}
</script>

<template>
  <div class="user-list-section">
    <div class="section-header">
      <div class="header-line"></div>
      <h3>
        <v-icon class="header-icon">mdi-account-star</v-icon>
        {{ $t('featured_athletes') }}
        <v-icon class="header-icon">mdi-account-star</v-icon>
      </h3>
      <div class="header-line"></div>
    </div>

    <div class="users-grid" v-if="!loading && users.length > 0">
      <div
        v-for="(user, index) in users"
        :key="user.id"
        class="user-card"
        :style="{ animationDelay: `${index * 0.1}s` }"
      >
        <div class="card-glow"></div>
        <div class="card-content">
          <!-- Rank Badge -->
          <div class="rank-badge">
            <span>{{ index + 4 }}</span>
          </div>

          <!-- Avatar -->
          <div class="avatar-container">
            <div class="avatar-ring"></div>
            <img
              :src="getAvatar(user)"
              :alt="user.name"
              class="avatar"
            />
            <div class="level-badge">
              <v-icon size="12">mdi-shield</v-icon>
              <span>{{ user.level }}</span>
            </div>
          </div>

          <!-- User Info -->
          <h4 class="user-name">{{ user.name }}</h4>

          <!-- Stats Row -->
          <div class="stats-row">
            <div class="stat-item strength">
              <v-icon size="16">mdi-arm-flex</v-icon>
              <span>{{ user.strength }}</span>
            </div>
            <div class="stat-item endurance">
              <v-icon size="16">mdi-heart-pulse</v-icon>
              <span>{{ user.endurance }}</span>
            </div>
          </div>

          <!-- Equipment -->
          <div v-if="getEquipItem(user)" class="equipment-bar">
            <v-icon size="14" color="rgba(255, 215, 0, 0.8)">mdi-sword-cross</v-icon>
            <span class="equip-name">{{ getEquipItem(user)?.name }}</span>
            <span class="equip-bonus">+{{ getEquipItem(user)?.bonus }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div class="loading-state" v-else-if="loading">
      <div class="skeleton-grid">
        <div
          v-for="i in 6"
          :key="i"
          class="skeleton-card"
          :style="{ animationDelay: `${i * 0.1}s` }"
        >
          <div class="skeleton-avatar"></div>
          <div class="skeleton-name"></div>
          <div class="skeleton-stats"></div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div class="empty-state" v-else>
      <v-icon size="60" color="rgba(255, 255, 255, 0.2)">mdi-account-group-outline</v-icon>
      <p>{{ $t('no_more_athletes') }}</p>
    </div>
  </div>
</template>

<style scoped>
.user-list-section {
  margin-top: 3rem;
  padding: 0 1rem;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.section-header h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #fff;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin: 0;
  white-space: nowrap;
}

.header-icon {
  color: #FFD700;
  filter: drop-shadow(0 0 8px rgba(255, 215, 0, 0.5));
}

.header-line {
  flex: 1;
  max-width: 150px;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(255, 215, 0, 0.4), transparent);
}

/* Users Grid */
.users-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1.25rem;
  padding: 1rem;
}

.user-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 1.25rem;
  text-align: center;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  overflow: hidden;
  animation: card-entrance 0.5s ease-out backwards;
}

@keyframes card-entrance {
  0% {
    opacity: 0;
    transform: translateY(20px) scale(0.95);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.card-glow {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(ellipse at center, rgba(255, 215, 0, 0.1), transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.user-card:hover {
  transform: translateY(-5px) scale(1.02);
  border-color: rgba(255, 215, 0, 0.4);
  box-shadow:
    0 10px 30px rgba(0, 0, 0, 0.4),
    0 0 40px rgba(255, 215, 0, 0.15);
}

.user-card:hover .card-glow {
  opacity: 1;
}

.card-content {
  position: relative;
  z-index: 2;
}

/* Rank Badge */
.rank-badge {
  position: absolute;
  top: 10px;
  left: 10px;
  width: 28px;
  height: 28px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.05));
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: rgba(255, 215, 0, 0.8);
  border: 1px solid rgba(255, 215, 0, 0.3);
}

/* Avatar Container */
.avatar-container {
  position: relative;
  display: inline-block;
  margin-bottom: 1rem;
}

.avatar-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 70px;
  height: 70px;
  border: 2px solid rgba(255, 215, 0, 0.3);
  border-radius: 50%;
  animation: ring-pulse 2s ease-in-out infinite;
}

@keyframes ring-pulse {
  0%, 100% {
    transform: translate(-50%, -50%) scale(1);
    opacity: 1;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.1);
    opacity: 0.5;
  }
}

.avatar {
  position: relative;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid rgba(255, 255, 255, 0.2);
  z-index: 2;
  transition: border-color 0.3s ease;
}

.user-card:hover .avatar {
  border-color: rgba(255, 215, 0, 0.6);
}

.level-badge {
  position: absolute;
  bottom: -5px;
  right: -5px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  padding: 0.2rem 0.5rem;
  border-radius: 12px;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.7rem;
  font-weight: 700;
  color: #fff;
  border: 2px solid rgba(20, 30, 48, 0.9);
  z-index: 3;
}

/* User Name */
.user-name {
  font-size: 1rem;
  font-weight: 700;
  color: #fff;
  margin: 0.5rem 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Stats Row */
.stats-row {
  display: flex;
  justify-content: center;
  gap: 0.75rem;
  margin: 0.75rem 0;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  background: rgba(0, 0, 0, 0.3);
  padding: 0.4rem 0.75rem;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 600;
  color: #fff;
  transition: transform 0.2s ease;
}

.stat-item:hover {
  transform: translateY(-2px);
}

.stat-item.strength {
  border: 1px solid rgba(255, 71, 87, 0.3);
}

.stat-item.strength .v-icon {
  color: #FF4757;
}

.stat-item.endurance {
  border: 1px solid rgba(0, 210, 255, 0.3);
}

.stat-item.endurance .v-icon {
  color: #00D2FF;
}

/* Equipment Bar */
.equipment-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding-top: 0.75rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  font-size: 0.75rem;
}

.equip-name {
  color: rgba(255, 255, 255, 0.6);
  max-width: 80px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.equip-bonus {
  font-weight: 700;
  color: #FFD700;
  background: rgba(255, 215, 0, 0.1);
  padding: 0.15rem 0.4rem;
  border-radius: 4px;
}

/* Loading State */
.loading-state {
  padding: 1rem;
}

.skeleton-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1.25rem;
}

.skeleton-card {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 20px;
  padding: 1.25rem;
  animation: skeleton-pulse 1.5s ease-in-out infinite;
}

@keyframes skeleton-pulse {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 0.6; }
}

.skeleton-avatar {
  width: 60px;
  height: 60px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 50%;
  margin: 0 auto 1rem;
}

.skeleton-name {
  height: 16px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  margin-bottom: 0.75rem;
}

.skeleton-stats {
  height: 28px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 0;
  gap: 1rem;
}

.empty-state p {
  color: rgba(255, 255, 255, 0.4);
  font-size: 1rem;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .users-grid {
    grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
    gap: 1rem;
  }

  .section-header h3 {
    font-size: 1.2rem;
  }

  .header-line {
    max-width: 100px;
  }
}

@media (max-width: 480px) {
  .users-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.75rem;
  }

  .user-card {
    padding: 1rem;
  }

  .section-header {
    gap: 1rem;
  }

  .section-header h3 {
    font-size: 1rem;
  }

  .header-line {
    max-width: 60px;
  }

  .header-icon {
    font-size: 1.2rem !important;
  }
}
</style>
