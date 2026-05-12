<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import CreditCardForm from '@/components/payment/CreditCardForm.vue'
import PaymentSummary from '@/components/payment/PaymentSummary.vue'
import PaymentSnackbar from '@/components/payment/PaymentSnackbar.vue'

const { t } = useI18n()

const snackbarRef = ref<InstanceType<typeof PaymentSnackbar>>()

const handlePaymentSuccess = () => {
  snackbarRef.value?.show()
}
</script>

<template>
  <div class="checkout">
    <v-container class="checkout-container">
      <!-- Fondo decorativo -->
      <div class="background-decoration">
        <div class="gradient-blob blob-1"></div>
        <div class="gradient-blob blob-2"></div>
      </div>

      <!-- Sección de encabezado -->
      <div class="header-section">
        <div class="header-content">
          <div class="header-badge">{{ $t('secure_payment_verified') }}</div>
          <h1 class="page-title">{{ $t('complete_purchase') }}</h1>
          <p class="page-subtitle">{{ $t('enter_payment_data') }}</p>
        </div>
      </div>

      <!-- Contenido principal -->
      <div class="content-section">
        <div class="payment-grid">
          <div class="payment-col form-col">
            <CreditCardForm @payment-success="handlePaymentSuccess" />
          </div>
          <div class="payment-col summary-col">
            <PaymentSummary />
          </div>
        </div>
      </div>

      <!-- Sección de confianza -->
      <div class="trust-section">
        <div class="trust-card">
          <div class="trust-icon shield">
            <v-icon size="28">mdi-shield-check</v-icon>
          </div>
          <div class="trust-text">
            <div class="trust-title">{{ $t('payment_100_secure') }}</div>
            <div class="trust-desc">{{ $t('ssl_encryption') }}</div>
          </div>
        </div>

        <div class="trust-divider"></div>

        <div class="trust-card">
          <div class="trust-icon refund">
            <v-icon size="28">mdi-undo</v-icon>
          </div>
          <div class="trust-text">
            <div class="trust-title">{{ $t('guarantee_30_days') }}</div>
            <div class="trust-desc">{{ $t('full_refund') }}</div>
          </div>
        </div>

        <div class="trust-divider"></div>

        <div class="trust-card">
          <div class="trust-icon instant">
            <v-icon size="28">mdi-lightning-bolt</v-icon>
          </div>
          <div class="trust-text">
            <div class="trust-title">{{ $t('immediate_access') }}</div>
            <div class="trust-desc">{{ $t('activate_subscription_now') }}</div>
          </div>
        </div>
      </div>
    </v-container>

    <!-- Snackbar de éxito -->
    <PaymentSnackbar ref="snackbarRef" />
  </div>
</template>

<style scoped>
.checkout {
  width: 100%;
  min-height: 100vh;
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fb 100%);
  color: #0a0a0a;
  position: relative;
  overflow: hidden;
}

/* Fondos decorativos */
.background-decoration {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
  z-index: 0;
}

.gradient-blob {
  position: absolute;
  filter: blur(80px);
  opacity: 0.1;
  border-radius: 50%;
  pointer-events: none;
}

.blob-1 {
  width: 600px;
  height: 600px;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  top: -200px;
  right: -300px;
  animation: float 8s ease-in-out infinite;
}

.blob-2 {
  width: 500px;
  height: 500px;
  background: linear-gradient(135deg, #3b82f6, #06b6d4);
  bottom: -200px;
  left: -250px;
  animation: float 10s ease-in-out infinite reverse;
}

@keyframes float {

  0%,
  100% {
    transform: translateY(0px);
  }

  50% {
    transform: translateY(30px);
  }
}

.checkout-container {
  max-width: 1400px;
  padding: 3rem 2rem;
  position: relative;
  z-index: 1;
}

/* Sección de encabezado */
.header-section {
  text-align: center;
  margin-bottom: 4rem;
  padding: 2rem 0;
  animation: slideDown 0.8s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.header-content {
  max-width: 700px;
  margin: 0 auto;
}

.header-badge {
  display: inline-block;
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.1), rgba(139, 92, 246, 0.1));
  border: 1px solid rgba(99, 102, 241, 0.3);
  color: #4f46e5;
  padding: 0.5rem 1.2rem;
  border-radius: 50px;
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin-bottom: 1.5rem;
  backdrop-filter: blur(10px);
}

.page-title {
  font-size: clamp(2.5rem, 5vw, 3.5rem);
  font-weight: 800;
  margin: 0 0 1rem;
  letter-spacing: -0.8px;
  color: #0a0a0a;
  background: linear-gradient(135deg, #0a0a0a, #4f46e5);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.page-subtitle {
  font-size: 1.1rem;
  color: #666666;
  margin: 0;
  font-weight: 400;
  line-height: 1.7;
  letter-spacing: 0.3px;
}

/* Sección de contenido */
.content-section {
  margin-bottom: 4rem;
  animation: fadeIn 0.8s ease-out 0.2s both;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.payment-grid {
  display: flex;
  flex-direction: row;
  align-items: stretch;
  gap: 2.5rem;
  width: 100%;
}

.payment-col {
  display: flex;
  flex-direction: column;
}

.payment-col > * {
  height: 100%;
}

.form-col {
  flex: 7;
  min-width: 0;
}

.summary-col {
  flex: 5;
  min-width: 0;
}

/* Sección de confianza */
.trust-section {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 2rem;
  padding: 3.5rem 2rem;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.7), rgba(248, 249, 251, 0.7));
  border: 1px solid rgba(99, 102, 241, 0.1);
  border-radius: 20px;
  backdrop-filter: blur(20px);
  flex-wrap: wrap;
  margin-top: 2rem;
  animation: fadeIn 0.8s ease-out 0.4s both;
}

.trust-card {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  flex: 0 1 auto;
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.trust-card:hover {
  transform: translateY(-2px);
}

.trust-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-weight: 600;
}

.trust-icon.shield {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15), rgba(34, 197, 94, 0.05));
  color: #22c55e;
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.trust-icon.refund {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.15), rgba(59, 130, 246, 0.05));
  color: #3b82f6;
  border: 1px solid rgba(59, 130, 246, 0.2);
}

.trust-icon.instant {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.15), rgba(168, 85, 247, 0.05));
  color: #a855f7;
  border: 1px solid rgba(168, 85, 247, 0.2);
}

.trust-text {
  text-align: left;
}

.trust-title {
  font-size: 0.95rem;
  font-weight: 700;
  color: #0a0a0a;
  margin-bottom: 0.3rem;
  letter-spacing: -0.3px;
}

.trust-desc {
  font-size: 0.8rem;
  color: #888888;
  line-height: 1.5;
}

.trust-divider {
  width: 1px;
  height: 50px;
  background: linear-gradient(180deg, transparent, rgba(0, 0, 0, 0.1), transparent);
}

/* Responsive */
@media (max-width: 1024px) {
  .checkout-container {
    padding: 2.5rem 1.5rem;
  }

  .header-section {
    margin-bottom: 3rem;
  }

  .content-section {
    margin-bottom: 3rem;
  }

  .payment-grid {
    gap: 2rem;
  }
}

@media (max-width: 960px) {
  .payment-grid {
    flex-direction: column;
    gap: 2rem;
  }

  .payment-col > * {
    height: auto;
  }

  .summary-col {
    order: -1;
  }

  .trust-section {
    flex-direction: column;
    gap: 1.5rem;
    padding: 2.5rem 1.5rem;
  }

  .trust-divider {
    display: none;
  }

  .trust-card {
    width: 100%;
    justify-content: flex-start;
  }

  .blob-1 {
    width: 300px;
    height: 300px;
    top: -100px;
    right: -100px;
  }

  .blob-2 {
    width: 250px;
    height: 250px;
    bottom: -100px;
    left: -100px;
  }
}

@media (max-width: 600px) {
  .checkout-container {
    padding: 1rem 0.75rem;
  }

  .header-section {
    margin-bottom: 2rem;
    padding: 0.5rem 0;
  }

  .header-badge {
    font-size: 0.7rem;
    padding: 0.35rem 0.8rem;
    margin-bottom: 1rem;
  }

  .page-title {
    font-size: 1.65rem;
    letter-spacing: -0.5px;
  }

  .page-subtitle {
    font-size: 0.9rem;
    line-height: 1.5;
  }

  .content-section {
    margin-bottom: 2rem;
  }

  .payment-grid {
    gap: 1.25rem;
  }

  .trust-section {
    padding: 1.5rem 1rem;
    border-radius: 14px;
    margin-top: 1rem;
    gap: 1.25rem;
  }

  .trust-card {
    gap: 0.8rem;
  }

  .trust-icon {
    width: 44px;
    height: 44px;
    border-radius: 12px;
  }

  .trust-title {
    font-size: 0.85rem;
  }

  .trust-desc {
    font-size: 0.75rem;
  }
}

@media (max-width: 480px) {
  .checkout-container {
    padding: 0.75rem 0.5rem;
  }

  .header-section {
    margin-bottom: 1.5rem;
  }

  .page-title {
    font-size: 1.45rem;
  }

  .page-subtitle {
    font-size: 0.85rem;
  }

  .trust-section {
    gap: 1rem;
    padding: 1.25rem 0.75rem;
    border-radius: 12px;
  }

  .trust-icon {
    width: 40px;
    height: 40px;
  }

  .trust-title {
    font-size: 0.8rem;
  }

  .trust-desc {
    font-size: 0.7rem;
  }
}
</style>
