<script setup lang="ts">
import { computed } from 'vue'

interface PlanItem {
  name: string
  period: string
  price: number
  originalPrice: number
  features: string[]
}

const selectedPlan = computed<PlanItem>(() => ({
  name: 'Plan Premium',
  period: 'Mensual',
  price: 10.00,
  originalPrice: 10.00,
  features: [
    'Acceso a todas las calculadoras de salud',
    'Chat ilimitado con Coach AI personalizado',
    'Rutinas de entrenamiento personalizadas',
    'Seguimiento detallado de progreso',
    'Soporte prioritario 24/7',
    'Actualizaciones semanales de contenido'
  ]
}))

const discount = computed(() => selectedPlan.value.originalPrice - selectedPlan.value.price)
const discountPercentage = computed(() =>
  Math.round((discount.value / selectedPlan.value.originalPrice) * 100)
)
</script>

<template>
  <div class="summary">
    <!-- Encabezado del resumen -->
    <div class="summary-header">
      <h2 class="summary-title">Tu compra</h2>
      <p class="summary-subtitle">Revisa lo que vas a obtener</p>
    </div>

    <!-- Tarjeta del plan -->
    <div class="plan-card">
      <!-- Badge de descuento -->
      <div v-if="discount > 0" class="discount-badge">
        <span class="discount-icon">⚡</span>
        <span class="discount-text">Ahorra {{ discountPercentage }}%</span>
      </div>

      <!-- Contenido del plan -->
      <div class="plan-content">
        <h3 class="plan-name">{{ selectedPlan.name }}</h3>
        <p class="plan-period">{{ selectedPlan.period }}</p>

        <!-- Precios -->
        <div class="pricing">
          <div v-if="discount > 0" class="original-price">€{{ selectedPlan.originalPrice.toFixed(2) }}</div>
          <div class="current-price">€{{ selectedPlan.price.toFixed(2) }}</div>
        </div>
      </div>

      <!-- Características -->
      <div class="features">
        <div class="feature" v-for="(feature, index) in selectedPlan.features" :key="index">
          <v-icon size="18" color="#6366f1">mdi-check-circle</v-icon>
          <span>{{ feature }}</span>
        </div>
      </div>
    </div>

    <!-- Desglose de pedido -->
    <div class="order-breakdown">
      <div class="breakdown-row">
        <span class="breakdown-label">Subtotal</span>
        <span class="breakdown-value">€{{ selectedPlan.originalPrice.toFixed(2) }}</span>
      </div>

      <div v-if="discount > 0" class="breakdown-row discount-row">
        <span class="breakdown-label">Descuento</span>
        <span class="breakdown-value discount">-€{{ discount.toFixed(2) }}</span>
      </div>

      <div class="breakdown-divider"></div>

      <div class="breakdown-row total-row">
        <span class="breakdown-label">Total a pagar</span>
        <span class="breakdown-value">€{{ selectedPlan.price.toFixed(2) }}</span>
      </div>

      <p class="payment-period">/mes, cancela cuando quieras</p>
    </div>

    <!-- Garantía -->
    <div class="guarantee">
      <div class="guarantee-icon">
        <v-icon size="28">mdi-shield-check</v-icon>
      </div>
      <div class="guarantee-text">
        <div class="guarantee-title">Garantía de 30 días</div>
        <div class="guarantee-desc">Reembolso sin hacer preguntas</div>
      </div>
    </div>

    <!-- Beneficios adicionales -->
    <div class="additional-benefits">
      <div class="benefit-item">
        <v-icon size="20" color="#22c55e">mdi-lightning-bolt</v-icon>
        <span>Acceso instantáneo</span>
      </div>
      <div class="benefit-item">
        <v-icon size="20" color="#3b82f6">mdi-lock</v-icon>
        <span>Datos seguros</span>
      </div>
      <div class="benefit-item">
        <v-icon size="20" color="#a855f7">mdi-cancel</v-icon>
        <span>Cancela siempre</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.summary {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  padding: 2.5rem;
  height: 100%;
  min-height: 600px;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.summary:hover {
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.08);
}

/* Encabezado */
.summary-header {
  margin-bottom: 2rem;
}

.summary-title {
  font-size: 1.4rem;
  font-weight: 800;
  margin: 0 0 0.5rem;
  color: #0a0a0a;
  letter-spacing: -0.5px;
}

.summary-subtitle {
  font-size: 0.9rem;
  color: #888888;
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
  padding: 2rem;
  margin-bottom: 2rem;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.plan-card:hover {
  border-color: #d8b4fe;
  box-shadow: 0 4px 16px rgba(147, 51, 234, 0.1);
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
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}

/* Contenido del plan */
.plan-content {
  text-align: center;
  margin-bottom: 2rem;
}

.plan-name {
  font-size: 1.3rem;
  font-weight: 800;
  color: #0a0a0a;
  margin: 0 0 0.5rem;
  letter-spacing: -0.3px;
}

.plan-period {
  font-size: 0.85rem;
  color: #888888;
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
  color: #bbb;
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
  display: flex;
  flex-direction: column;
  gap: 0.9rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e9d5ff;
}

.feature {
  display: flex;
  align-items: flex-start;
  gap: 0.85rem;
  font-size: 0.9rem;
  color: #555555;
  line-height: 1.6;
  transition: all 0.2s ease;
}

.feature:hover {
  color: #0a0a0a;
  transform: translateX(4px);
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
  color: #666666;
  font-weight: 600;
}

.breakdown-value {
  color: #0a0a0a;
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
  color: #999999;
  margin: 0.75rem 0 0;
  text-align: center;
  font-weight: 500;
}

/* Garantía */
.guarantee {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: linear-gradient(135deg, #f0fdf4 0%, #f3faf8 100%);
  border: 1px solid #bbf7d0;
  border-radius: 12px;
  padding: 1.25rem;
  margin-bottom: 1.5rem;
}

.guarantee-icon {
  width: 52px;
  height: 52px;
  border-radius: 12px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #22c55e;
  flex-shrink: 0;
  border: 1px solid #bbf7d0;
}

.guarantee-text {
  flex: 1;
}

.guarantee-title {
  font-size: 0.95rem;
  font-weight: 700;
  color: #0a0a0a;
  margin-bottom: 0.25rem;
}

.guarantee-desc {
  font-size: 0.8rem;
  color: #666666;
  margin: 0;
}

/* Beneficios adicionales */
.additional-benefits {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
  margin-top: auto;
}

.benefit-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.85rem;
  color: #555555;
  font-weight: 500;
  transition: all 0.2s ease;
}

.benefit-item:hover {
  color: #0a0a0a;
  transform: translateX(4px);
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

  .guarantee {
    padding: 1rem;
    gap: 0.85rem;
  }

  .guarantee-icon {
    width: 44px;
    height: 44px;
  }

  .guarantee-title {
    font-size: 0.9rem;
  }

  .guarantee-desc {
    font-size: 0.75rem;
  }

  .benefit-item {
    font-size: 0.8rem;
    gap: 0.6rem;
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