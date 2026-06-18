<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'

const router = useRouter()
const { t } = useI18n()

const planPrice = 10

const features = [
  {
    icon: 'mdi-robot',
    title: 'coach_ai_personal',
    desc: 'coach_ai_desc'
  },
  {
    icon: 'mdi-calculator-variant',
    title: 'calorie_calculator',
    desc: 'calorie_calculator_desc'
  },
  {
    icon: 'mdi-clipboard-text',
    title: 'health_history',
    desc: 'health_history_desc'
  },
  {
    icon: 'mdi-lightning-bolt',
    title: 'immediate_activation',
    desc: 'immediate_activation_desc'
  },
  {
    icon: 'mdi-trending-up',
    title: 'advanced_tracking',
    desc: 'advanced_tracking_desc'
  },
  {
    icon: 'mdi-infinity',
    title: 'no_limits',
    desc: 'no_limits_desc'
  }
]

const navigateToPayment = () => {
  router.push('/payment')
}
</script>

<template>
  <div class="plan-view">
    <section class="hero">
      <div ></div>
      <div class="hero-content">
        <v-icon class="hero-icon">mdi-crown-outline</v-icon>
        <h1 class="hero-title">{{ $t('plan_premium') }}</h1>
        <p class="hero-subtitle">
          {{ $t('plan_hero_subtitle') }}
        </p>
      </div>
    </section>

    <section class="plans-section">
      <div class="plan-card">
        <div ></div>
        <div class="card-shine"></div>



        <div class="card-content layout-split">
          <div class="plan-left">
            <div class="tier-header">
              <div class="tier-icon-wrapper">
                <v-icon size="40" color="white">mdi-crown</v-icon>
              </div>
              <div class="tier-badge">{{ $t('premium') }}</div>
            </div>

            <div class="price-section">
              <div class="price-wrapper">
                <div class="price-main">
                  <span class="currency">€</span>
                  <span class="price-amount">{{ planPrice }}</span>
                </div>
                <div class="price-meta">
                  <span class="price-period">{{ $t('per_month') }}</span>
                  <span class="price-note">{{ $t('monthly_billing') }}</span>
                </div>
              </div>
            </div>

            <v-btn
              block
              size="x-large"
              class="cta-button"
              elevation="0"
              @click="navigateToPayment"
            >
              <v-icon class="mr-2" size="20">mdi-rocket-launch</v-icon>
              <span>{{ $t('subscribe_now') }}</span>
            </v-btn>

            <div class="card-footer">
              <v-icon size="16" color="white">mdi-information</v-icon>
              <span>{{ $t('no_commitment') }}</span>
            </div>
          </div>

          <div class="plan-right">
            <div class="features-section">
              <div class="features-header">
                <v-icon size="20" color="white">mdi-check-decagram</v-icon>
                <span>{{ $t('unlock_tools') }}</span>
              </div>

              <div class="features-list">
                <div
                  v-for="(item, idx) in features"
                  :key="idx"
                  class="feature-item"
                  :style="{ animationDelay: (idx * 0.07) + 's' }"
                >
                  <div class="feature-icon-wrapper">
                    <v-icon size="20" color="white">{{ item.icon }}</v-icon>
                  </div>
                  <div class="feature-body">
                    <span class="feature-title">{{ $t(item.title) }}</span>
                    <span class="feature-desc">{{ $t(item.desc) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="guarantee-banner">
        <div class="guarantee-icon">
          <v-icon size="32" color="white">mdi-shield-check</v-icon>
        </div>
        <div class="guarantee-content">
          <div class="guarantee-title">{{ $t('30_day_guarantee') }}</div>
          <div class="guarantee-text">
            {{ $t('guarantee_desc') }}
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
.plan-view {
  position: relative;
  min-height: 100vh;
  background: transparent;
  overflow-x: hidden;
}

/* Hero */
.hero {
  position: relative;
  text-align: center;
  padding: 5rem 2rem 3rem;
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-icon {
  font-size: 4rem !important;
  color: #FFD700;
  filter: drop-shadow(0 0 30px rgba(255, 215, 0, 0.6));
  margin-bottom: 1.5rem;

}

.hero-title {
  font-size: clamp(2.5rem, 6vw, 4rem);
  font-weight: 900;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 2px;
  text-transform: uppercase;
  margin: 0 0 1rem;

}

.hero-subtitle {
  font-size: clamp(1rem, 2vw, 1.25rem);
  color: rgba(255, 255, 255, 0.7);
  font-weight: 400;
  max-width: 600px;
  margin: 0 auto;
  line-height: 1.6;
}

/* Plans Section */
.plans-section {
  padding: 2rem 1rem 4rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 3rem;
}

/* Single Plan Card */
.plan-card {
  position: relative;
  width: 100%;
  max-width: 900px;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(255, 215, 0, 0.35);
  border-radius: 20px;
  padding: 2rem 2.5rem;
  overflow: hidden;
  box-shadow: 0 15px 45px rgba(255, 215, 0, 0.15);
}



.card-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.08), transparent);
  transition: left 0.7s ease;
  pointer-events: none;
}



/* Featured Badge */
.featured-badge {
  position: absolute;
  top: 12px;
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

}

/* Layout Split */
.layout-split {
  display: flex;
  flex-direction: row;
  gap: 3rem;
  align-items: center;
}

.plan-left {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.plan-right {
  flex: 1.2;
}

/* Tier Header */
.tier-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.tier-icon-wrapper {
  width: 70px;
  height: 70px;
  border-radius: 16px;
  background: linear-gradient(135deg, #FFD700, #FFA500);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 10px 30px rgba(255, 215, 0, 0.4);

}

.tier-badge {
  background: rgba(0, 0, 0, 0.4);
  border: 2px solid #FFD700;
  padding: 0.4rem 1.25rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 900;
  color: #FFD700;
  text-transform: uppercase;
  letter-spacing: 2px;
  box-shadow: 0 4px 15px rgba(255, 215, 0, 0.3);
}

/* Card Content */
.card-content {
  position: relative;
  z-index: 2;
}

.plan-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.plan-name {
  font-size: 2rem;
  font-weight: 900;
  color: white;
  margin: 0 0 0.5rem;
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
  margin-bottom: 1.5rem;
  padding: 1.25rem 1rem;
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
  background: linear-gradient(90deg, #FFD700, #FFA500);
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
  color: #FFD700;
  margin-top: 0.5rem;
}

.price-amount {
  font-size: 4rem;
  font-weight: 900;
  color: white;
  line-height: 1;
  text-shadow: 0 4px 15px rgba(255, 215, 0, 0.4);
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

/* Features Section */
.features-section {
  margin-bottom: 2rem;
}

.features-header {
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

.features-list {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.05);
  transition: all 0.3s ease;

}



.feature-icon-wrapper {
  width: 32px;
  height: 32px;
  border-radius: 10px;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-top: 0.1rem;
}

.feature-body {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  flex: 1;
}

.feature-title {
  color: #ffffff;
  font-size: 0.95rem;
  font-weight: 700;
}

.feature-desc {
  color: rgba(255, 255, 255, 0.55);
  font-size: 0.85rem;
  line-height: 1.4;
}

/* CTA Button */
.cta-button {
  background: linear-gradient(135deg, #FFD700, #FFA500) !important;
  color: #000 !important;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 2px;
  border-radius: 16px !important;
  padding: 1.75rem 2rem !important;
  margin-bottom: 1.25rem;
  box-shadow: 0 10px 30px rgba(255, 215, 0, 0.35);
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
  box-shadow: 0 15px 40px rgba(255, 215, 0, 0.5);
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

/* Guarantee Banner */
.guarantee-banner {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  max-width: 900px;
  width: 100%;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.15) 0%, rgba(255, 165, 0, 0.1) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(255, 215, 0, 0.3);
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(255, 215, 0, 0.2);

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

/* Responsive */
@media (max-width: 600px) {
  .hero {
    padding: 3rem 1rem 2rem;
  }

  .hero-icon {
    font-size: 3rem !important;
  }

  .hero-title {
    font-size: 2rem;
  }

  .hero-subtitle {
    font-size: 0.95rem;
  }

  .plans-section {
    padding: 1rem 1rem 3rem;
  }

  .plan-card {
    padding: 2rem 1.5rem;
  }

  .price-amount {
    font-size: 3.2rem;
  }

  .cta-button {
    padding: 1.5rem 1.5rem !important;
  }

  .featured-badge {
    font-size: 0.7rem;
    padding: 0.5rem 1rem;
    top: 10px;
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
  .price-amount {
    font-size: 2.8rem;
  }
}

@media (max-width: 850px) {
  .layout-split {
    flex-direction: column;
    gap: 2rem;
  }
}

@media (max-width: 768px) {
  .features-list {
    grid-template-columns: 1fr;
  }
}
</style>
