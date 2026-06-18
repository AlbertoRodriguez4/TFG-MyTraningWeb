<template>
  <div class="subscription-container">
    <div v-if="isLoading" class="loading-state">
      <v-progress-circular
        size="64"
        color="#FFD700"
        indeterminate
      />
      <p class="loading-text">{{ $t('loading_subscription') }}</p>
    </div>

    <div v-else-if="!hasActiveSubscription" class="no-subscription">
      <v-card class="settings-card plan-card" elevation="0" border>
        <v-card-title class="card-title">
          <v-icon>mdi-crown-outline</v-icon>
          {{ $t('your_subscription') }}
        </v-card-title>

        <v-divider class="card-divider" />

        <v-card-text>
          <div class="free-plan">
            <div class="plan-badge free">{{ $t('subscription.free') }}</div>

            <h3 class="plan-name">{{ $t('subscription.freePlan') }}</h3>
            <p class="plan-description">
              {{ $t('free_plan') }}
            </p>

            <div class="premium-features">
              <div class="feature">
                <v-icon size="small">mdi-check-circle</v-icon>
                <span>{{ $t('health_calculators') }}</span>
              </div>
              <div class="feature">
                <v-icon size="small">mdi-check-circle</v-icon>
                <span>{{ $t('chat_coach_ai') }}</span>
              </div>
              <div class="feature">
                <v-icon size="small">mdi-check-circle</v-icon>
                <span>{{ $t('premiumRoutines') }}</span>
              </div>
              <div class="feature">
                <v-icon size="small">mdi-check-circle</v-icon>
                <span>{{ $t('advanced_tracking') }}</span>
              </div>
              <div class="feature">
                <v-icon size="small">mdi-check-circle</v-icon>
                <span>{{ $t('soporte_247') }}</span>
              </div>
            </div>

            <div class="pricing-info">
              <span class="price-tag">10€<small>/mes</small></span>
              <p class="price-desc">{{ $t('cancel_anytime') }}</p>
            </div>

            <v-btn
              class="subscribe-btn"
              color="#FFD700"
              size="large"
              @click="goToPayment"
            >
              <v-icon start>mdi-lock-check</v-icon>
              {{ $t('subscribe_now') }}
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <div v-else>
      <v-card class="settings-card plan-card" elevation="0" border>
        <v-card-title class="card-title">
          <v-icon>mdi-crown</v-icon>
          {{ $t('your_subscription') }}
        </v-card-title>

        <v-divider class="card-divider" />

        <v-card-text>
          <div class="current-plan">
            <div class="plan-badge premium">{{ $t('premium') }}</div>

            <h3 class="plan-name">{{ subscription?.planType || $t('plan_premium') }}</h3>
            <p class="plan-price">{{ subscription?.monthlyPrice.toFixed(2) }}€ <span class="period">/mes</span></p>

            <div class="renewal-info">
              <v-icon size="small" color="white">mdi-calendar-check</v-icon>
              <span>{{ $t('renewal_on') }} <strong>{{ formatDate(subscription?.endDate) }}</strong></span>
            </div>

            <div class="plan-status">
              <v-chip color="#4ade80" text-color="#000" size="small">
                <v-icon start size="small">mdi-check-circle</v-icon>
                {{ $t('premium_active') }}
              </v-chip>
            </div>

            <div class="plan-actions">
              <v-btn
                variant="outlined"
                size="large"
                class="action-btn"
                @click="renewSubscription"
                :loading="isRenewing"
              >
                {{ $t('subscribe_now') }}
              </v-btn>
              <v-btn
                variant="outlined"
                size="large"
                color="error"
                class="action-btn"
                @click="confirmCancel"
                :loading="isCancelling"
              >
                {{ $t('cancel_subscription') }}
              </v-btn>
            </div>
          </div>
        </v-card-text>
      </v-card>

      <v-card class="settings-card" elevation="0" border>
        <v-card-title class="card-title">
          <v-icon>mdi-receipt</v-icon>
          {{ $t('subscription_history') }}
        </v-card-title>

        <v-divider class="card-divider" />

        <v-card-text>
          <div v-if="subscriptionHistory.length > 0" class="billing-table">
            <div class="table-header">
              <div class="table-col">{{ $t('subscription.startDate') }}</div>
              <div class="table-col">{{ $t('subscription.endDate') }}</div>
              <div class="table-col">{{ $t('price') }}</div>
              <div class="table-col">{{ $t('subscription.status') }}</div>
            </div>

            <div
              v-for="sub in subscriptionHistory"
              :key="sub.subscriptionId"
              class="table-row"
            >
              <div class="table-col">{{ formatDate(sub.startDate) }}</div>
              <div class="table-col">{{ formatDate(sub.endDate) }}</div>
              <div class="table-col">
                <strong>{{ sub.monthlyPrice.toFixed(2) }}€</strong>
              </div>
              <div class="table-col">
                <v-chip
                  :label="true"
                  size="small"
                  :color="sub.isActive ? '#4ade80' : '#ff9500'"
                  :text-color="sub.isActive ? '#000' : '#000'"
                >
                  {{ sub.isActive ? 'activa' : 'finalizada' }}
                </v-chip>
              </div>
            </div>
          </div>
          <div v-else class="no-history">
            <v-icon size="48" color="white">mdi-receipt</v-icon>
            <p>{{ $t('subscription.noHistory') }}</p>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <v-dialog v-model="showCancelDialog" max-width="400">
      <v-card class="dialog-card">
        <v-card-title class="dialog-title">
          <v-icon color="white">mdi-alert-circle</v-icon>
          {{ $t('cancel_subscription_confirm') }}
        </v-card-title>
        <v-card-text class="dialog-text">
          {{ $t('cancel_subscription_text') }}
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-btn variant="text" @click="showCancelDialog = false">{{ $t('subscription.back') }}</v-btn>
          <v-btn color="error" @click="cancelSubscription">{{ $t('yes_cancel') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="showSnackbar" :color="snackbarColor" timeout="3000" location="top">
      {{ snackbarMessage }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { useUserStore } from '@/stores/userStore'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const router = useRouter()
const route = useRoute()
const subscriptionStore = useSubscriptionStore()

const isLoading = ref(true)
const isRenewing = ref(false)
const isCancelling = ref(false)
const showCancelDialog = ref(false)
const showSnackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')

const hasActiveSubscription = computed(() => subscriptionStore.hasActiveSubscription)
const subscription = computed(() => subscriptionStore.activeSubscription)
const subscriptionHistory = computed(() => subscriptionStore.subscriptionHistory)

onMounted(async () => {
  await loadSubscriptionData()

  // Mostrar mensaje de activación si venimos de la página de pago con una suscripción activa
  if (route.query.subscribed === 'true') {
    showSnackbarMessage(t('subscription_activated'), 'success')
    router.replace({ query: {} })
  }
})
// Obtener datos de suscripción al montar el componente
async function loadSubscriptionData() {
  isLoading.value = true
  try {
    // Comprobar si el usuario tiene una suscripción activa
    await subscriptionStore.checkSubscription()
    if (subscriptionStore.hasActiveSubscription) { // si tiene una suscripción activa, cargar los detalles y el historial
      await subscriptionStore.getActiveSubscription()
      await subscriptionStore.getSubscriptionHistory()
    }
  } catch (error) {
    logger.error('Error loading subscription:', error)
    showSnackbarMessage(t('subscription_load_error'), 'error')
  } finally {
    isLoading.value = false
  }
}
// Función para renovar la suscripción
async function renewSubscription() {
  isRenewing.value = true
  try {
    // Se renueva la suscripción y se muestra un mensaje según el resultado
    const result = await subscriptionStore.renewSubscription()
    if (result.success) {
      showSnackbarMessage(result.message || t('subscription_renewed'), 'success')
    } else {
      showSnackbarMessage(result.error || t('subscription_renew_error'), 'error')
    }
  } catch (error) {
    logger.error('Error renewing subscription:', error)
    showSnackbarMessage(t('subscription_renew_error'), 'error')
  } finally {
    isRenewing.value = false
  }
}

function confirmCancel() {
  showCancelDialog.value = true
}
// Función para cancelar la suscripción
async function cancelSubscription() {
  if (!subscription.value) return

  isCancelling.value = true
  showCancelDialog.value = false

  try {
    // Se cancela la suscripción y se muestra un mensaje según el resultado
    const result = await subscriptionStore.cancelSubscription(subscription.value.subscriptionId)
    if (result.success) {
      showSnackbarMessage(result.message || t('subscription_cancelled'), 'success')
      await loadSubscriptionData()
    } else {
      showSnackbarMessage(result.error || t('subscription_cancel_error'), 'error')
    }
  } catch (error) {
    logger.error('Error cancelling subscription:', error)
    showSnackbarMessage(t('subscription_cancel_error'), 'error')
  } finally {
    isCancelling.value = false
  }
}

function goToPayment() {
  router.push('/payment')
}

function formatDate(dateString: string | undefined): string {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

function showSnackbarMessage(message: string, color: string) {
  snackbarMessage.value = message
  snackbarColor.value = color
  showSnackbar.value = true
}
</script>

<style scoped>
.subscription-container {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* Loading State */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  gap: 1.5rem;
}

.loading-text {
  color: rgba(255, 255, 255, 0.7);
  font-size: 1rem;
}

/* No Subscription */
.free-plan {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  text-align: center;
}

.plan-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-weight: 900;
  font-size: 0.8rem;
  letter-spacing: 1px;
  text-transform: uppercase;
  width: fit-content;
  margin: 0 auto;
}

.plan-badge.free {
  background: rgba(255, 255, 255, 0.2);
  color: rgba(255, 255, 255, 0.8);
}

.plan-badge.premium {
  background: linear-gradient(135deg, #ffcc00, #ff9900);
  color: #000;
}

.plan-name {
  margin: 0.5rem 0 0;
  font-size: 1.8rem;
  font-weight: 900;
  color: rgba(255, 255, 255, 0.9);
}

.plan-description {
  margin: 0;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.95rem;
  line-height: 1.6;
}

.premium-features {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
  text-align: left;
  background: rgba(0, 0, 0, 0.3);
  padding: 1.5rem;
  border-radius: 12px;
}

.feature {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.95rem;
}

.feature :deep(.v-icon) {
  color: #4ade80;
}

.pricing-info {
  margin-top: 1rem;
}

.price-tag {
  display: block;
  font-size: 3rem;
  font-weight: 900;
  color: #FFD700;
  letter-spacing: -2px;
}

.price-tag small {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: normal;
}

.price-desc {
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.9rem;
  margin: 0.5rem 0 0;
}

.subscribe-btn {
  width: 100%;
  font-weight: 700;
  font-size: 1.1rem;
  padding: 1.25rem;
  text-transform: none;
  margin-top: 1rem;
}

/* Current Plan */
.current-plan {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.plan-price {
  margin: 0;
  font-size: 2.5rem;
  font-weight: 900;
  color: #ffcc00;
}

.period {
  font-size: 1.2rem;
  color: rgba(255, 255, 255, 0.6);
}

.renewal-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  border-radius: 8px;
  background: rgba(74, 222, 128, 0.1);
  border: 1px solid rgba(74, 222, 128, 0.2);
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.95rem;
}

.plan-status {
  display: flex;
  justify-content: flex-start;
}

.plan-actions {
  display: flex;
  gap: 1rem;
}

.action-btn {
  flex: 1;
  color: rgba(255, 255, 255, 0.7) !important;
  border-color: rgba(255, 204, 0, 0.3) !important;
  font-weight: 600;
}

/* Billing Table */
.billing-table {
  display: flex;
  flex-direction: column;
  gap: 0;
}

.table-header {
  display: grid;
  grid-template-columns: 1fr 1fr 100px 100px;
  gap: 1rem;
  padding: 1rem;
  background: rgba(255, 204, 0, 0.05);
  border-radius: 8px 8px 0 0;
  font-weight: 700;
  color: #ffcc00;
  font-size: 0.9rem;
}

.table-row {
  display: grid;
  grid-template-columns: 1fr 1fr 100px 100px;
  gap: 1rem;
  padding: 1rem;
  border-bottom: 1px solid rgba(255, 204, 0, 0.1);
  align-items: center;
}

.table-row:last-child {
  border-bottom: none;
  border-radius: 0 0 8px 8px;
}

.table-col {
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.9rem;
  overflow: hidden;
  text-overflow: ellipsis;
}

.no-history {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  gap: 1rem;
  color: rgba(255, 255, 255, 0.5);
}

/* Dialog */
.dialog-card {
  background: rgba(20, 20, 20, 0.95);
  border: 1px solid rgba(255, 204, 0, 0.2);
}

.dialog-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 700;
}

.dialog-text {
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.6;
}

.dialog-actions {
  padding: 1rem;
  gap: 0.5rem;
}

/* Card Styles */
.settings-card {
  background: rgba(20, 20, 20, 0.5) !important;
  border: 1px solid rgba(255, 204, 0, 0.15) !important;
  border-radius: 12px;
}

.plan-card {
  border: 2px solid rgba(255, 204, 0, 0.3) !important;
  background: linear-gradient(135deg, rgba(255, 204, 0, 0.05) 0%, rgba(255, 204, 0, 0.02) 100%) !important;
}

.card-title {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  color: #ffcc00;
  font-weight: 700;
  font-size: 1.2rem;
  padding: 1.5rem;
  padding-bottom: 0;
}

.card-divider {
  margin: 1rem 0;
  border-color: rgba(255, 204, 0, 0.1) !important;
}

/* Responsive */
@media (max-width: 768px) {
  .plan-actions {
    flex-direction: column;
  }

  .table-header,
  .table-row {
    grid-template-columns: 1fr 100px;
    gap: 0.5rem;
  }

  .table-header > div:nth-child(2),
  .table-row > div:nth-child(2) {
    display: none;
  }
}

@media (max-width: 600px) {
  .plan-price {
    font-size: 2rem;
  }
}
</style>
