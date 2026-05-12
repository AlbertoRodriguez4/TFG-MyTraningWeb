<template>
  <v-app>
    <v-main class="achievements-page">
      <v-container fluid class="pa-6">
        <!-- Header -->
        <div class="page-header mb-8">
          <div class="header-icon">
            <v-icon size="48" color="amber">mdi-trophy-variant</v-icon>
          </div>
          <div>
            <h1 class="text-h3 font-weight-bold text-white">{{ $t('achievements_title') }}</h1>
            <p class="text-subtitle-1 text-grey-light">{{ $t('achievements_subtitle') }}</p>
          </div>
          <v-spacer />
          <v-chip color="primary" size="large" class="stats-chip">
            <v-icon start>mdi-trophy</v-icon>
            {{ store.totalCount }} / {{ store.achievements.length }}
          </v-chip>
        </div>

        <!-- Grid de logros -->
        <v-row v-if="!store.loading">
          <v-col
            v-for="achievement in store.achievements"
            :key="achievement.id"
            cols="12" sm="6" md="4" lg="3"
          >
            <v-card
              class="achievement-card"
              :class="{ 'unlocked': isUnlocked(achievement.id), 'secret': achievement.isSecret && !isUnlocked(achievement.id) }"
              elevation="8"
            >
              <div class="card-glow" v-if="isUnlocked(achievement.id)"></div>
              <v-card-text class="pa-6 text-center">
                <div class="achievement-icon mb-4">
                  <v-icon
                    size="64"
                    :color="isUnlocked(achievement.id) ? getCategoryColor(achievement.category) : 'grey'"
                  >
                    {{ achievement.isSecret && !isUnlocked(achievement.id) ? 'mdi-lock' : achievement.iconUrl }}
                  </v-icon>
                </div>
                <h3 class="text-h6 font-weight-bold mb-2" :class="isUnlocked(achievement.id) ? 'text-white' : 'text-grey-medium'">
                  {{ achievement.isSecret && !isUnlocked(achievement.id) ? '???' : achievement.name }}
                </h3>
                <p class="text-body-2 mb-4 text-grey-light">
                  {{ achievement.isSecret && !isUnlocked(achievement.id) ? $t('achievement_secret_desc') : achievement.description }}
                </p>

                <div class="achievement-meta">
                  <v-chip x-small :color="getCategoryColor(achievement.category)" class="mr-2">
                    {{ $t(`category_${achievement.category}`) }}
                  </v-chip>
                  <v-chip x-small color="amber" v-if="achievement.rewardGold > 0">
                    +{{ achievement.rewardGold }} 🪙
                  </v-chip>
                  <v-chip x-small color="purple" v-if="achievement.rewardXP > 0">
                    +{{ achievement.rewardXP }} XP
                  </v-chip>
                </div>

                <div v-if="isUnlocked(achievement.id)" class="unlocked-date mt-3">
                  <v-icon x-small color="success">mdi-check-circle</v-icon>
                  <span class="text-success-light text-caption">
                    {{ $t('achievement_unlocked') }} {{ formatDate(getUnlockedDate(achievement.id)) }}
                  </span>
                </div>
                <div v-else class="progress-bar mt-3">
                  <v-progress-linear
                    :model-value="getProgress(achievement)"
                    :color="getCategoryColor(achievement.category)"
                    height="6"
                    rounded
                  />
                  <span class="text-caption text-grey-light">{{ getProgressText(achievement) }}</span>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <v-skeleton-loader v-else type="card" :loading="true" />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useAchievementStore } from '@/stores/achievementStore';
import { useUserStore } from '@/stores/userStore';

const store = useAchievementStore();
const userStore = useUserStore();

onMounted(async () => {
  await store.fetchAllAchievements();
  await store.fetchMyAchievements();
});

const userAchievementIds = computed(() =>
  store.userAchievements.map(ua => ua.achievementId)
);

function isUnlocked(achievementId: number) {
  return userAchievementIds.value.includes(achievementId);
}

function getUnlockedDate(achievementId: number) {
  const ua = store.userAchievements.find(x => x.achievementId === achievementId);
  return ua?.unlockedAt;
}

function getCategoryColor(category: string) {
  const colors: Record<string, string> = {
    strength: 'red',
    endurance: 'blue',
    consistency: 'orange',
    social: 'green',
    exploration: 'teal',
    general: 'purple'
  };
  return colors[category] || 'grey';
}

function getProgress(achievement: any) {
  const user = userStore.loggedUser;
  if (!user) return 0;
  let current = 0;
  switch (achievement.requirementType) {
    case 'tasks_completed': current = (user as any).tasksCompleted || 0; break;
    case 'streak_days': current = user.consistencyStreak || 0; break;
    case 'level_reached': current = user.level || 0; break;
    case 'gold_earned': current = user.gold || 0; break;
    case 'strength_reached': current = user.strength || 0; break;
    case 'endurance_reached': current = user.endurance || 0; break;
    default: current = 0;
  }
  return Math.min((current / achievement.requirementValue) * 100, 100);
}

function getProgressText(achievement: any) {
  const progress = getProgress(achievement);
  return `${Math.round(progress)}%`;
}

function formatDate(dateStr?: string) {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleDateString();
}
</script>

<style scoped>
.achievements-page {
  background: linear-gradient(135deg, #0a0a0f 0%, #1a1a2e 100%);
  min-height: 100vh;
}
.page-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}
.header-icon {
  width: 80px;
  height: 80px;
  border-radius: 20px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  display: flex;
  align-items: center;
  justify-content: center;
}
.achievement-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
}
.achievement-card:hover {
  transform: translateY(-4px);
}
.achievement-card.unlocked {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(102, 126, 234, 0.5);
}
.achievement-card.secret {
  opacity: 0.7;
}
.card-glow {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #667eea, #764ba2);
}
.achievement-icon {
  width: 100px;
  height: 100px;
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.05);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
}
.achievement-meta {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 4px;
}
.stats-chip {
  font-weight: 700;
  font-size: 1.1rem;
}

/* Colores para fondo oscuro */
.text-white {
  color: #ffffff !important;
}
.text-grey-light {
  color: rgba(255, 255, 255, 0.75) !important;
}
.text-grey-medium {
  color: rgba(255, 255, 255, 0.5) !important;
}
.text-success-light {
  color: #4ade80 !important;
}
</style>
