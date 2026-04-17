<script setup lang="ts">
import { usePlanStore } from '@/stores/PlanStore'
import { useRouter } from 'vue-router'

const router = useRouter()
const store = usePlanStore()
store.fetchPlan()

const navigateToPayment = () => {
  router.push('/payment')
}



const advantages = ['planCustomization', 'coachTracking', 'premiumAccess']

// Colores y características por tier
const tierColors = [
  { 
    primary: '#00ff88', 
    secondary: '#00d9ff', 
    glow: 'rgba(0, 255, 136, 0.4)', 
    badge: 'STARTER',
    icon: 'mdi-rocket-launch'
  },
  { 
    primary: '#00D2FF', 
    secondary: '#3A7BD5', 
    glow: 'rgba(0, 210, 255, 0.4)', 
    badge: 'PRO',
    icon: 'mdi-star'
  },
  { 
    primary: '#FFD700', 
    secondary: '#FFA500', 
    glow: 'rgba(255, 215, 0, 0.4)', 
    badge: 'PREMIUM',
    icon: 'mdi-crown'
  },
  { 
    primary: '#A855F7', 
    secondary: '#7C3AED', 
    glow: 'rgba(168, 85, 247, 0.5)', 
    badge: 'ELITE',
    icon: 'mdi-diamond'
  }
]

function getTierStyle(index: number) {
  return tierColors[index % tierColors.length]
}
</script>

<template>
  <div class="plans-wrapper">
    <v-container class="planes-container" fluid>
      <!-- Plans Grid -->
      <div class="plans-grid">
        <div
          v-for="(plan, index) in store.plan"
          :key="plan.id"
          class="plan-card"
          :class="{ 
            'featured': index === 2,
            'plan-starter': index === 0,
            'plan-pro': index === 1,
            'plan-premium': index === 2,
            'plan-elite': index === 3
          }"
          :style="{
            '--tier-primary': getTierStyle(index).primary,
            '--tier-secondary': getTierStyle(index).secondary,
            '--tier-glow': getTierStyle(index).glow,
            animationDelay: (index * 0.1) + 's'
          }"
        >
          <!-- Background Effects -->
          <div class="card-bg-pattern"></div>
          <div class="card-glow"></div>
          
          <!-- Featured Badge -->
          <div v-if="index === 2" class="featured-badge">
            <v-icon size="16">mdi-star</v-icon>
            <span>MÁS POPULAR</span>
          </div>

          <!-- Tier Badge -->
          <div class="tier-header">
            <div class="tier-icon-wrapper">
              <v-icon :size="index === 2 ? 40 : 32" color="white">
                {{ getTierStyle(index).icon }}
              </v-icon>
            </div>
            <div class="tier-badge">
              {{ getTierStyle(index).badge }}
            </div>
          </div>

          <!-- Card Content -->
          <div class="card-content">
            <!-- Title -->
            <div class="plan-header">
              <h2 class="plan-name">
                {{ $t('Tier') }} {{ plan.id }}
              </h2>
              <p class="plan-description">{{ plan.description }}</p>
            </div>

            <!-- Price Section -->
            <div class="price-section">
              <div class="price-wrapper">
                <div class="price-main">
                  <span class="currency">€</span>
                  <span class="price-amount">{{ 20 + (index * 10) }}</span>
                </div>
                <div class="price-meta">
                  <span class="price-period">/mes</span>
                  <span class="price-note">facturación mensual</span>
                </div>
              </div>
            </div>

            <!-- Advantages List -->
            <div class="advantages-section">
              <div class="advantages-header">
                <v-icon size="20" :color="getTierStyle(index).primary">mdi-check-decagram</v-icon>
                <span>{{ $t('INCLUYE') }}</span>
              </div>

              <div class="advantages-list">
                <div 
                  v-for="(item, idx) in advantages" 
                  :key="idx"
                  class="advantage-item"
                  :style="{ animationDelay: ((index * 0.1) + (idx * 0.05)) + 's' }"
                >
                  <div class="advantage-icon-wrapper">
                    <v-icon size="16" :color="getTierStyle(index).primary">mdi-check-circle</v-icon>
                  </div>
                  <span class="advantage-text">{{ $t(item) }}</span>
                </div>
              </div>
            </div>

            <!-- CTA Button -->
            <v-btn
              block
              size="x-large"
              class="cta-button"
              elevation="0"
              @click="navigateToPayment"
            >
              <v-icon class="mr-2" size="20">mdi-rocket-launch</v-icon>
              <span>EMPEZAR AHORA</span>
            </v-btn>

            <!-- Additional Info -->
            <div class="card-footer">
              <v-icon size="16" color="rgba(255,255,255,0.4)">mdi-information</v-icon>
              <span>Sin compromiso • Cancela cuando quieras</span>
            </div>
          </div>

          <!-- Shine Effect -->
          <div class="card-shine"></div>
        </div>
      </div>

      <!-- Bottom Guarantee -->
      <div class="guarantee-banner">
        <div class="guarantee-icon">
          <v-icon size="32" color="#FFD700">mdi-shield-check</v-icon>
        </div>
        <div class="guarantee-content">
          <div class="guarantee-title">Garantía de 30 Días</div>
          <div class="guarantee-text">Todos los planes incluyen satisfacción garantizada o te devolvemos tu dinero</div>
        </div>
      </div>
    </v-container>
  </div>
</template>

<style scoped>
.plans-wrapper {
  position: relative;
  padding: 2rem 0;
}

.planes-container {
  max-width: 1600px;
  margin: 0 auto;
  padding: 0 2rem;
}

/* Plans Grid */
.plans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

/* Plan Card */
.plan-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid;
  border-radius: 24px;
  padding: 2rem;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: card-entrance 0.8s ease-out backwards;
  cursor: pointer;
}

@keyframes card-entrance {
  0% {
    opacity: 0;
    transform: translateY(50px) scale(0.9);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.plan-starter {
  border-color: rgba(0, 255, 136, 0.3);
}

.plan-pro {
  border-color: rgba(0, 210, 255, 0.3);
}

.plan-premium {
  border-color: rgba(255, 215, 0, 0.4);
}

.plan-elite {
  border-color: rgba(168, 85, 247, 0.3);
}

.plan-card:hover {
  transform: translateY(-15px) scale(1.03);
  border-color: var(--tier-primary);
  box-shadow: 
    0 25px 70px var(--tier-glow),
    0 0 80px var(--tier-glow);
}

.plan-card.featured {
  transform: scale(1.05);
  box-shadow: 0 20px 60px var(--tier-glow);
}

.plan-card.featured:hover {
  transform: translateY(-15px) scale(1.08);
}

/* Background Pattern */
.card-bg-pattern {
  position: absolute;
  top: -50%;
  right: -30%;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, var(--tier-glow), transparent);
  opacity: 0.2;
  pointer-events: none;
}

/* Card Glow */
.card-glow {
  position: absolute;
  inset: -2px;
  background: linear-gradient(135deg, var(--tier-primary), var(--tier-secondary));
  border-radius: 24px;
  opacity: 0;
  transition: opacity 0.4s ease;
  z-index: -1;
  filter: blur(20px);
}

.plan-card:hover .card-glow {
  opacity: 0.5;
}

/* Featured Badge */
.featured-badge {
  position: absolute;
  top: -12px;
  right: 2rem;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  color: #000;
  padding: 0.6rem 1.25rem;
  border-radius: 30px;
  font-size: 0.75rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 1px;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  z-index: 10;
  box-shadow: 0 8px 25px rgba(255, 215, 0, 0.5);
  animation: badge-float 3s ease-in-out infinite;
}

@keyframes badge-float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-5px);
  }
}

/* Tier Header */
.tier-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
  position: relative;
}

.tier-icon-wrapper {
  width: 80px;
  height: 80px;
  border-radius: 20px;
  background: linear-gradient(135deg, var(--tier-primary), var(--tier-secondary));
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 30px var(--tier-glow);
  transition: all 0.3s ease;
}

.plan-card.featured .tier-icon-wrapper {
  width: 90px;
  height: 90px;
  animation: icon-pulse 2s ease-in-out infinite;
}

@keyframes icon-pulse {
  0%, 100% {
    transform: scale(1);
    box-shadow: 0 10px 30px var(--tier-glow);
  }
  50% {
    transform: scale(1.1);
    box-shadow: 0 15px 40px var(--tier-glow);
  }
}

.tier-badge {
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid var(--tier-primary);
  padding: 0.6rem 1.5rem;
  border-radius: 25px;
  font-size: 0.9rem;
  font-weight: 900;
  color: var(--tier-primary);
  text-transform: uppercase;
  letter-spacing: 2px;
  box-shadow: 0 4px 15px var(--tier-glow);
}

/* Card Content */
.card-content {
  position: relative;
  z-index: 2;
}

/* Plan Header */
.plan-header {
  text-align: center;
  margin-bottom: 2rem;
}

.plan-name {
  font-size: 1.8rem;
  font-weight: 900;
  color: white;
  margin: 0 0 0.75rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.plan-description {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.95rem;
  margin: 0;
  line-height: 1.5;
}

/* Price Section */
.price-section {
  margin-bottom: 2rem;
  padding: 2rem 1.5rem;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 20px;
  border: 2px solid rgba(255, 255, 255, 0.05);
  position: relative;
  overflow: hidden;
}

.price-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--tier-primary), var(--tier-secondary));
  opacity: 0.5;
}

.price-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.price-main {
  display: flex;
  align-items: flex-start;
  gap: 0.25rem;
}

.currency {
  font-size: 1.75rem;
  font-weight: 800;
  color: var(--tier-primary);
  margin-top: 0.5rem;
}

.price-amount {
  font-size: 4rem;
  font-weight: 900;
  color: white;
  line-height: 1;
  text-shadow: 0 4px 15px var(--tier-glow);
}

.price-meta {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
}

.price-period {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 600;
}

.price-note {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.4);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Advantages Section */
.advantages-section {
  margin-bottom: 2rem;
}

.advantages-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.9rem;
  font-weight: 800;
  color: white;
  text-transform: uppercase;
  letter-spacing: 1px;
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid rgba(255, 255, 255, 0.1);
}

.advantages-list {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.advantage-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 14px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  transition: all 0.3s ease;
  animation: advantage-fade-in 0.5s ease-out backwards;
}

@keyframes advantage-fade-in {
  0% {
    opacity: 0;
    transform: translateX(-20px);
  }
  100% {
    opacity: 1;
    transform: translateX(0);
  }
}

.advantage-item:hover {
  background: rgba(255, 255, 255, 0.06);
  border-color: var(--tier-primary);
  transform: translateX(5px);
}

.advantage-icon-wrapper {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.advantage-text {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.95rem;
  font-weight: 500;
  flex: 1;
}

/* CTA Button */
.cta-button {
  background: linear-gradient(135deg, var(--tier-primary), var(--tier-secondary)) !important;
  color: #000 !important;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 2px;
  border-radius: 16px !important;
  padding: 1.75rem 2rem !important;
  margin-bottom: 1.25rem;
  box-shadow: 0 10px 30px var(--tier-glow);
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.cta-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  transition: left 0.5s;
}

.cta-button:hover::before {
  left: 100%;
}

.cta-button:hover {
  transform: translateY(-3px) scale(1.02);
  box-shadow: 0 15px 40px var(--tier-glow);
}

.cta-button span {
  position: relative;
  z-index: 2;
}

/* Card Footer */
.card-footer {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.card-footer span {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.4);
  text-align: center;
}

/* Shine Effect */
.card-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.08),
    transparent
  );
  transition: left 0.7s ease;
  pointer-events: none;
}

.plan-card:hover .card-shine {
  left: 100%;
}

/* Guarantee Banner */
.guarantee-banner {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.15) 0%, rgba(255, 165, 0, 0.1) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(255, 215, 0, 0.3);
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(255, 215, 0, 0.2);
  animation: fadeIn 0.8s ease-out 0.5s backwards;
}

@keyframes fadeIn {
  0% {
    opacity: 0;
    transform: translateY(20px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

.guarantee-icon {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.guarantee-content {
  flex: 1;
}

.guarantee-title {
  font-size: 1.2rem;
  font-weight: 900;
  color: #FFD700;
  margin-bottom: 0.4rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.guarantee-text {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.5;
}

/* Responsive Design */
@media (max-width: 1400px) {
  .plans-grid {
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  }
}

@media (max-width: 1200px) {
  .plans-grid {
    gap: 1.5rem;
  }

  .plan-card.featured {
    transform: scale(1);
  }

  .plan-card.featured:hover {
    transform: translateY(-15px) scale(1.03);
  }
}

@media (max-width: 960px) {
  .planes-container {
    padding: 0 1.5rem;
  }

  .plans-grid {
    grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  }

  .price-amount {
    font-size: 3.5rem;
  }
}

@media (max-width: 600px) {
  .planes-container {
    padding: 0 1rem;
  }

  .plans-grid {
    grid-template-columns: 1fr;
    gap: 1.25rem;
  }

  .plan-card {
    padding: 1.5rem;
  }

  .tier-icon-wrapper {
    width: 70px;
    height: 70px;
  }

  .plan-card.featured .tier-icon-wrapper {
    width: 75px;
    height: 75px;
  }

  .plan-name {
    font-size: 1.5rem;
  }

  .price-amount {
    font-size: 3rem;
  }

  .cta-button {
    padding: 1.5rem 1.5rem !important;
  }

  .featured-badge {
    font-size: 0.7rem;
    padding: 0.5rem 1rem;
    top: -10px;
    right: 1rem;
  }

  .guarantee-banner {
    flex-direction: column;
    text-align: center;
    padding: 1.5rem;
  }

  .guarantee-icon {
    width: 50px;
    height: 50px;
  }
}

@media (max-width: 400px) {
  .plan-card {
    padding: 1.25rem;
  }

  .price-amount {
    font-size: 2.5rem;
  }

  .tier-badge {
    font-size: 0.8rem;
    padding: 0.5rem 1.25rem;
  }
}
</style>