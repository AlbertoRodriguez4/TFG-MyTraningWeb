<script setup lang="ts">
import { usePlanStore } from '@/stores/PlanStore'
import { useRouter } from 'vue-router'

const router = useRouter()
const store = usePlanStore()
store.fetchPlan()

const navigateToPayment = () => {
  router.push('/payment')
}

const tierColors = [
  { primary: '#00ff88', secondary: '#00d9ff', glow: 'rgba(0, 255, 136, 0.3)', badge: 'STARTER', icon: 'mdi-rocket-launch' },
  { primary: '#00D2FF', secondary: '#3A7BD5', glow: 'rgba(0, 210, 255, 0.3)', badge: 'PRO', icon: 'mdi-star' },
  { primary: '#FFD700', secondary: '#FFA500', glow: 'rgba(255, 215, 0, 0.3)', badge: 'PREMIUM', icon: 'mdi-crown' },
  { primary: '#A855F7', secondary: '#7C3AED', glow: 'rgba(168, 85, 247, 0.3)', badge: 'ELITE', icon: 'mdi-diamond' }
]

function getTierStyle(index: number) {
  return tierColors[index % tierColors.length]
}
</script>

<template>
  <div class="plans-wrapper">
    <v-container class="planes-container" fluid>
      <div class="plans-grid">
        <div
          v-for="(plan, index) in store.plan"
          :key="plan.id"
          class="plan-card"
          :class="{ featured: index === 2 }"
          :style="{
            '--tier-primary': getTierStyle(index).primary,
            '--tier-secondary': getTierStyle(index).secondary,
            '--tier-glow': getTierStyle(index).glow,
            animationDelay: (index * 0.08) + 's'
          }"
        >
          <div v-if="index === 2" class="featured-badge">
            <v-icon size="12">mdi-star</v-icon>
            <span>MÁS POPULAR</span>
          </div>

          <div class="tier-header">
            <v-icon :size="index === 2 ? 28 : 22" color="white">
              {{ getTierStyle(index).icon }}
            </v-icon>
            <span class="tier-badge">{{ getTierStyle(index).badge }}</span>
          </div>

          <div class="card-content">
            <div class="plan-header">
              <h2 class="plan-name">Plan {{ plan.id }}</h2>
              <p class="plan-description">{{ plan.description }}</p>
            </div>

            <v-btn
              block
              size="small"
              class="cta-button"
              elevation="0"
              @click="navigateToPayment"
            >
              <v-icon class="mr-1" size="16">mdi-rocket-launch</v-icon>
              <span>EMPEZAR</span>
            </v-btn>

            <div class="card-footer">
              <v-icon size="12" color="rgba(255,255,255,0.4)">mdi-information</v-icon>
              <span>Sin compromiso</span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="store.plan.length === 0" class="empty-plans">
        <v-icon size="40" color="rgba(255,255,255,0.3)">mdi-package-variant</v-icon>
        <p>No hay planes disponibles en este momento</p>
      </div>
    </v-container>
  </div>
</template>

<style scoped>
.plans-wrapper {
  position: relative;
  padding: 1rem 0;
}

.planes-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.plans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1.25rem;
  margin-bottom: 2rem;
}

.plan-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 1.5px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 1.25rem;
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: card-entrance 0.6s ease-out backwards;
  cursor: pointer;
}

@keyframes card-entrance {
  0% { opacity: 0; transform: translateY(30px) scale(0.95); }
  100% { opacity: 1; transform: translateY(0) scale(1); }
}

.plan-card:hover {
  transform: translateY(-6px) scale(1.02);
  border-color: var(--tier-primary);
  box-shadow: 0 12px 40px var(--tier-glow);
}

.plan-card.featured {
  border-color: rgba(255, 215, 0, 0.35);
  box-shadow: 0 8px 30px rgba(255, 215, 0, 0.15);
}

.featured-badge {
  position: absolute;
  top: -8px;
  right: 1rem;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  color: #000;
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  z-index: 10;
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.4);
}

.tier-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.tier-badge {
  background: rgba(0, 0, 0, 0.4);
  border: 1.5px solid var(--tier-primary);
  padding: 0.35rem 0.9rem;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 900;
  color: var(--tier-primary);
  text-transform: uppercase;
  letter-spacing: 1.5px;
}

.card-content {
  position: relative;
  z-index: 2;
}

.plan-header {
  text-align: center;
  margin-bottom: 1rem;
}

.plan-name {
  font-size: 1.1rem;
  font-weight: 900;
  color: white;
  margin: 0 0 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.plan-description {
  color: rgba(255, 255, 255, 0.55);
  font-size: 0.8rem;
  margin: 0;
  line-height: 1.4;
  min-height: 2.2rem;
}

.cta-button {
  background: linear-gradient(135deg, var(--tier-primary), var(--tier-secondary)) !important;
  color: #000 !important;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  border-radius: 10px !important;
  padding: 0.6rem 1rem !important;
  margin-bottom: 0.75rem;
  box-shadow: 0 6px 20px var(--tier-glow);
  transition: all 0.25s ease;
  font-size: 0.75rem;
}

.cta-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 28px var(--tier-glow);
}

.card-footer {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.35rem;
  padding-top: 0.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
}

.card-footer span {
  font-size: 0.7rem;
  color: rgba(255, 255, 255, 0.35);
  text-align: center;
}

.empty-plans {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 3rem;
  color: rgba(255, 255, 255, 0.4);
  text-align: center;
}

.empty-plans p {
  font-size: 0.9rem;
  margin: 0;
}

@media (max-width: 960px) {
  .planes-container { padding: 0 1rem; }
  .plans-grid { grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; }
}

@media (max-width: 600px) {
  .plans-grid { grid-template-columns: 1fr; gap: 1rem; }
  .plan-card { padding: 1rem; }
  .plan-name { font-size: 1rem; }
}
</style>
