<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { PlanItem } from '@/components/Models/Payment'

const { t } = useI18n()
// Mostrar el plan seleccionado por el usuario
const selectedPlan = computed<PlanItem>(() => ({
  name: t('plan_premium'),
  period: t('monthly'),
  price: 10.00,
  originalPrice: 10.00,
  features: [
    t('feature_health_calculators'),
    t('feature_coach_ai_chat'),
    t('feature_custom_routines'),
    t('feature_progress_tracking'),
    t('feature_priority_support'),
    t('feature_weekly_updates')
  ]
}))
// Calcular el descuento aplicado, si es que hay alguno, y el porcentaje de descuento para mostrar en la interfaz
const discount = computed(() => selectedPlan.value.originalPrice - selectedPlan.value.price)
const discountPercentage = computed(() =>
  Math.round((discount.value / selectedPlan.value.originalPrice) * 100)
)
</script>

<template>
  <div class="summary">
    <div class="summary-header">
      <h2 class="summary-title">{{ $t('your_purchase') }}</h2>
      <p class="summary-subtitle">{{ $t('review_what_you_get') }}</p>
    </div>

    <div class="plan-card">
      <div v-if="discount > 0" class="discount-badge">
        <v-icon class="discount-icon" size="18">mdi-lightning-bolt</v-icon>
        <span class="discount-text">{{ $t('save_label') }} {{ discountPercentage }}%</span>
      </div>

      <div class="plan-content">
        <h3 class="plan-name">{{ selectedPlan.name }}</h3>
        <p class="plan-period">{{ selectedPlan.period }}</p>

        <div class="pricing">
          <div v-if="discount > 0" class="original-price">€{{ selectedPlan.originalPrice.toFixed(2) }}</div>
          <div class="current-price">€{{ selectedPlan.price.toFixed(2) }}</div>
        </div>
      </div>

      <div class="features">
        <div class="feature" v-for="(feature, index) in selectedPlan.features" :key="index">
          <v-icon size="18" color="white">mdi-check-circle</v-icon>
          <span>{{ feature }}</span>
        </div>
      </div>
    </div>

      <div class="order-breakdown">
      <div class="breakdown-row">
        <span class="breakdown-label">{{ $t('subtotal') }}</span>
        <span class="breakdown-value">€{{ selectedPlan.originalPrice.toFixed(2) }}</span>
      </div>

      <div v-if="discount > 0" class="breakdown-row discount-row">
        <span class="breakdown-label">{{ $t('discount') }}</span>
        <span class="breakdown-value discount">-€{{ discount.toFixed(2) }}</span>
      </div>

      <div class="breakdown-divider"></div>

      <div class="breakdown-row total-row">
        <span class="breakdown-label">{{ $t('total_to_pay') }}</span>
        <span class="breakdown-value">€{{ selectedPlan.price.toFixed(2) }}</span>
      </div>

      <p class="payment-period">{{ $t('per_month_cancel_anytime') }}</p>
    </div>

  </div>
</template>

<style scoped>
.summary {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  padding: 2rem;
  height: 100%;
  min-height: auto;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

/* Encabezado */
.summary-header {
  margin-bottom: 1.5rem;
}

.summary-title {
  font-size: 1.4rem;
  font-weight: 800;
  margin: 0 0 0.5rem;
  color: #1a1a1a;
  letter-spacing: -0.5px;
}

.summary-subtitle {
  font-size: 0.9rem;
  color: rgba(0, 0, 0, 0.6);
  margin: 0;
  font-weight: 400;
  line-height: 1.5;
}

/* Tarjeta del plan */
.plan-card {
  position: relative;
  background: linear-gradient(135deg, #f8f9ff 0%, #f3e8ff 100%);
  border: 1px solid #e9d5ff;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

/* Badge de descuento */
.discount-badge {
  position: absolute;
  top: -12px;
  right: 20px;
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
  color: white;
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.3);
}

.discount-icon {
  font-size: 0.9rem;

}

/* Contenido del plan */
.plan-content {
  text-align: center;
  margin-bottom: 2rem;
}

.plan-name {
  font-size: 1.3rem;
  font-weight: 800;
  color: #1a1a1a;
  margin: 0 0 0.5rem;
  letter-spacing: -0.3px;
}

.plan-period {
  font-size: 0.85rem;
  color: rgba(0, 0, 0, 0.6);
  margin: 0 0 1.5rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}

/* Precios */
.pricing {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.original-price {
  font-size: 0.95rem;
  color: rgba(0, 0, 0, 0.6);
  text-decoration: line-through;
  font-weight: 500;
}

.current-price {
  font-size: 2.5rem;
  font-weight: 900;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Características */
.features {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e9d5ff;
}

.feature {
  display: flex;
  align-items: flex-start;
  gap: 0.85rem;
  font-size: 0.9rem;
  color: rgba(0, 0, 0, 0.8);
  line-height: 1.6;
  transition: all 0.2s ease;
}

.feature :deep(.v-icon) {
  margin-top: 1px;
  flex-shrink: 0;
}

/* Desglose de pedido */
.order-breakdown {
  margin-bottom: 2rem;
}

.breakdown-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
  font-size: 0.95rem;
}

.breakdown-label {
  color: rgba(0, 0, 0, 0.8);
  font-weight: 600;
}

.breakdown-value {
  color: #1a1a1a;
  font-weight: 600;
}

.breakdown-value.discount {
  color: #22c55e;
}

.breakdown-divider {
  height: 1px;
  background: linear-gradient(90deg, transparent, #e9d5ff, transparent);
  margin: 1rem 0;
}

.breakdown-row.total-row {
  padding: 1.25rem 0;
  border-top: 2px solid #6366f1;
  border-bottom: 2px solid #6366f1;
  font-size: 1.05rem;
  font-weight: 800;
}

.breakdown-row.total-row .breakdown-value {
  font-size: 1.4rem;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.payment-period {
  font-size: 0.75rem;
  color: rgba(0, 0, 0, 0.6);
  margin: 0.75rem 0 0;
  text-align: center;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 1200px) {
  .summary {
    position: sticky;
    top: 2rem;
  }
}

@media (max-width: 600px) {
  .summary {
    padding: 1.5rem;
    position: static;
    min-height: auto;
    border-radius: 16px;
  }

  .summary-header {
    margin-bottom: 1.5rem;
  }

  .summary-title {
    font-size: 1.2rem;
  }

  .plan-card {
    padding: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .discount-badge {
    top: -10px;
    right: 12px;
    padding: 0.5rem 1rem;
    font-size: 0.75rem;
  }

  .plan-name {
    font-size: 1.1rem;
  }

  .current-price {
    font-size: 2rem;
  }

  .features {
    gap: 0.7rem;
  }

  .feature {
    font-size: 0.85rem;
    gap: 0.65rem;
  }

  .breakdown-row.total-row .breakdown-value {
    font-size: 1.2rem;
  }
}

@media (max-width: 480px) {
  .summary {
    padding: 1.2rem;
    border-radius: 16px;
  }

  .plan-name {
    font-size: 1rem;
  }

  .current-price {
    font-size: 1.8rem;
  }

  .breakdown-row {
    padding: 0.8rem 0;
  }

  .discount-badge {
    padding: 0.4rem 0.8rem;
    font-size: 0.7rem;
  }
}
</style>