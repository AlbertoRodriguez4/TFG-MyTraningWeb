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


      <div class="header-section">
        <div class="header-content">
          <div class="header-badge">{{ $t('secure_payment_verified') }}</div>
          <h1 class="page-title">{{ $t('complete_purchase') }}</h1>
          <p class="page-subtitle">{{ $t('enter_payment_data') }}</p>
        </div>
      </div>

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

    </v-container>

    <PaymentSnackbar ref="snackbarRef" />
  </div>
</template>

<style scoped>
.checkout {
  width: 100%;
  min-height: 100vh;
  background: transparent;
  color: var(--text-primary);
  position: relative;
  overflow: hidden;
}



.checkout-container {
  max-width: 1300px;
  padding: 2rem 1.5rem;
  position: relative;
  z-index: 1;
}

/* Sección de encabezado */
.header-section {
  text-align: center;
  margin-bottom: 4rem;
  padding: 2rem 0;

}

.header-content {
  max-width: 700px;
  margin: 0 auto;
}

.header-badge {
  display: inline-block;
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.1), rgba(139, 92, 246, 0.1));
  border: 1px solid rgba(99, 102, 241, 0.3);
  color: var(--accent-color);
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
  color: var(--text-primary);
  background: linear-gradient(135deg, var(--text-primary), #4f46e5);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.page-subtitle {
  font-size: 1.1rem;
  color: var(--text-secondary);
  margin: 0;
  font-weight: 400;
  line-height: 1.7;
  letter-spacing: 0.3px;
}

/* Sección de contenido */
.content-section {
  margin-bottom: 4rem;

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
  flex: 1.1;
  min-width: 0;
}

.summary-col {
  flex: 1;
  min-width: 0;
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
}
</style>
