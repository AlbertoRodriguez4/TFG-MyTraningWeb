<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { logger } from '@/utils/logger'
import type { ValidationState } from '@/components/Models/Payment'

const { t } = useI18n()

const router = useRouter()
const subscriptionStore = useSubscriptionStore()

const emit = defineEmits<{
  'payment-success': []
}>()

const cardNumber = ref('')
const cardHolder = ref('')
const expiryDate = ref('')
const cvv = ref('')
const isProcessing = ref(false)
const showSuccess = ref(false)
const errorMessage = ref('')
// Estado de validación para cada campo del formulario
const validationErrors = ref<ValidationState>({
  cardNumber: false,
  cardHolder: false,
  expiryDate: false,
  cvv: false
})
// Función para formatear el número de tarjeta en grupos de 4 dígitos
const formatCardNumber = (value: string) => {
  const cleaned = value.replace(/\D/g, '')
  const groups = cleaned.match(/.{1,4}/g)
  return groups ? groups.join(' ') : cleaned
}
// Función para formatear la fecha de caducidad en formato MM/YY
const formatExpiryDate = (value: string) => {
  const cleaned = value.replace(/\D/g, '')
  if (cleaned.length >= 2) {
    return `${cleaned.substring(0, 2)}/${cleaned.substring(2, 4)}`
  }
  return cleaned
}
// Función para validar el fornulario antes de enviar el pago
const validateForm = (): boolean => {
  errorMessage.value = ''
  validationErrors.value = {
    cardNumber: false,
    cardHolder: false,
    expiryDate: false,
    cvv: false
  }

  let isValid = true
  // Validar número de tarjeta (mínimo 16 dígitos, ignorando espacios)
  if (!cardNumber.value || cardNumber.value.replace(/\s/g, '').length < 16) {
    validationErrors.value.cardNumber = true
    isValid = false
  }
  // Validar nombre del titular (mínimo 3 caracteres)
  if (!cardHolder.value || cardHolder.value.trim().length < 3) {
    validationErrors.value.cardHolder = true
    isValid = false
  }
  // Validar fecha de vencimiento (mínimo 5 caracteres para formato MM/YY)
  if (!expiryDate.value || expiryDate.value.length < 5) {
    validationErrors.value.expiryDate = true
    isValid = false
  }
  // Validar CVV (mínimo 3 dígitos)
  if (!cvv.value || cvv.value.length < 3) {
    validationErrors.value.cvv = true
    isValid = false
  }

  // Validar formato de fecha MM/YY
  const [month, year] = expiryDate.value.split('/')
  if (month && year) {
    const monthNum = parseInt(month)
    if (monthNum < 1 || monthNum > 12) {
      validationErrors.value.expiryDate = true
      isValid = false
    }
  }
  // Si el formulario no es válido, mostrar mensaje de error genérico
  if (!isValid) {
    errorMessage.value = t('fill_all_fields_correctly')
  }

  return isValid
}
// Procesar pago al enviar el formulario, simulando una llamada al backend y manejando el resultado
const handleSubmit = async () => {
  if (!validateForm()) {
    return
  }

  isProcessing.value = true
  errorMessage.value = ''

  try {
    // Simular pequeño delay de procesamiento
    await new Promise(resolve => setTimeout(resolve, 1500))

    // Llamar al store para procesar la suscripción
    const result = await subscriptionStore.purchaseSubscription()
    // En caso de que sea exitoso, mostrar mensaje de éxito y redirigir a perfil después de un delay para que el usuario vea la confirmación
    if (result.success) {
      showSuccess.value = true
      emit('payment-success')

      setTimeout(() => {
        // Redirigir a la vista de perfil con parámetro de éxito
        router.push('/profile?tab=subscription&subscribed=true')
      }, 2500)
    } else {
      errorMessage.value = result.error || t('subscription_purchase_error')
      showSuccess.value = false
    }
  } catch (err) {
    logger.error('Payment error:', err)
    errorMessage.value = t('connection_error_retry')
    showSuccess.value = false
  } finally {
    isProcessing.value = false
  }
}
</script>

<template>
  <div class="card-form" :class="{ 'form-success': showSuccess }">
    <!-- Encabezado del formulario -->
    <div class="form-header">
      <h2 class="form-title">{{ $t('payment_info') }}</h2>
      <p class="form-subtitle">{{ $t('data_protected_ssl') }}</p>
    </div>

    <!-- Vista previa de tarjeta -->
    <div class="card-preview">
      <div class="card-top">
        <div class="card-chip"></div>
        <div class="card-logo"><v-icon>mdi-credit-card</v-icon></div>
      </div>
      <div class="card-number-display">{{ cardNumber || '•••• •••• •••• ••••' }}</div>
      <div class="card-footer">
        <div class="card-holder">{{ (cardHolder || $t('card_holder').toUpperCase()).toUpperCase() }}</div>
        <div class="card-expiry">{{ expiryDate || 'MM/YY' }}</div>
      </div>
    </div>

    <!-- Formulario -->
    <v-form @submit.prevent="handleSubmit" class="payment-form">
      <!-- Número de tarjeta -->
      <div class="form-group">
        <label class="form-label">{{ $t('card_number') }}</label>
        <div class="input-wrapper">
          <v-text-field v-model="cardNumber" :placeholder="$t('payment.cardNumberPlaceholder')" maxlength="19"
            variant="outlined" density="comfortable" class="form-input" :error="validationErrors.cardNumber"
            @update:model-value="(val) => cardNumber = formatCardNumber(val)" />
          <div class="card-icons">
            <v-icon v-if="cardNumber" size="20" color="#4f46e5">mdi-credit-card</v-icon>
          </div>
        </div>
        <div v-if="validationErrors.cardNumber" class="error-text">
          {{ $t('card_number_invalid') }}
        </div>
      </div>

      <!-- Nombre del titular -->
      <div class="form-group">
        <label class="form-label">{{ $t('card_holder') }}</label>
        <div class="input-wrapper">
          <v-text-field v-model="cardHolder" :placeholder="$t('card_holder_placeholder')" maxlength="50"
            variant="outlined" density="comfortable" class="form-input" :error="validationErrors.cardHolder" />
          <div class="card-icons">
            <v-icon v-if="cardHolder" size="20" color="#4f46e5">mdi-account</v-icon>
          </div>
        </div>
        <div v-if="validationErrors.cardHolder" class="error-text">
          {{ $t('card_holder_min_chars') }}
        </div>
      </div>

      <!-- Vencimiento y CVV -->
      <div class="form-row">
        <div class="form-group">
          <label class="form-label">{{ $t('expiry_date') }}</label>
          <div class="input-wrapper">
            <v-text-field v-model="expiryDate" :placeholder="$t('payment.expiryPlaceholder')" maxlength="5"
              variant="outlined" density="comfortable" class="form-input" :error="validationErrors.expiryDate"
              @update:model-value="(val) => expiryDate = formatExpiryDate(val)" />
            <div class="card-icons">
              <v-icon v-if="expiryDate" size="20" color="#4f46e5">mdi-calendar</v-icon>
            </div>
          </div>
          <div v-if="validationErrors.expiryDate" class="error-text">
            {{ $t('expiry_date_invalid') }}
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">{{ $t('cvv') }}</label>
          <div class="input-wrapper">
            <v-text-field v-model="cvv" :placeholder="$t('payment.cvvPlaceholder')" maxlength="4" type="password"
              variant="outlined" density="comfortable" class="form-input" :error="validationErrors.cvv"
              @update:model-value="(val) => cvv = val.replace(/\D/g, '')" />
            <div class="card-icons">
              <v-tooltip :text="$t('cvv_tooltip')">
                <template #activator="{ props }">
                  <v-icon v-bind="props" size="20" color="#4f46e5">mdi-lock</v-icon>
                </template>
              </v-tooltip>
            </div>
          </div>
          <div v-if="validationErrors.cvv" class="error-text">
            {{ $t('cvv_invalid') }}
          </div>
        </div>
      </div>

      <!-- Métodos de pago -->
      <div class="payment-methods">
        <div class="method visa" title="Visa">
          <v-icon size="28">mdi-visa</v-icon>
        </div>
        <div class="method mastercard" title="Mastercard">
          <v-icon size="28">mdi-mastercard</v-icon>
        </div>
        <div class="method amex" title="American Express">
          <v-icon size="28">mdi-american-credit-card</v-icon>
        </div>
      </div>

      <!-- Mensaje de error -->
      <v-alert v-if="errorMessage" type="error" variant="tonal" closable class="error-alert">
        {{ errorMessage }}
      </v-alert>

      <!-- Botón de envío -->
      <v-btn type="submit" :loading="isProcessing" :disabled="isProcessing" block size="large" class="submit-btn">
        <span v-if="!isProcessing">{{ $t('confirm_payment') }} €10,00</span>
        <span v-else>{{ $t('processing_payment') }}</span>
      </v-btn>

      <!-- Pie de página -->
      <p class="form-footer">
        {{ $t('terms_acceptance') }}
      </p>
    </v-form>
  </div>
</template>

<style scoped>
.card-form {
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

.card-form:hover {
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.08);
}

/* Encabezado */
.form-header {
  margin-bottom: 2rem;
}

.form-title {
  font-size: 1.4rem;
  font-weight: 800;
  margin: 0 0 0.5rem;
  color: #1a1a1a;
  letter-spacing: -0.5px;
}

.form-subtitle {
  font-size: 0.9rem;
  color: rgba(0, 0, 0, 0.6);
  margin: 0;
  font-weight: 400;
  line-height: 1.5;
}

/* Vista previa de tarjeta */
.card-preview {
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
  border-radius: 16px;
  padding: 2.5rem;
  margin-bottom: 2.5rem;
  min-height: 180px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 12px 32px rgba(99, 102, 241, 0.3);
  position: relative;
  overflow: hidden;
}

.card-preview::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.2), transparent);
  border-radius: 50%;
}

.card-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.card-chip {
  width: 50px;
  height: 38px;
  background: linear-gradient(135deg, #fbbf24, #d97706);
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.card-logo {
  font-size: 1.8rem;
  opacity: 0.9;
}

.card-number-display {
  font-size: 1.5rem;
  font-weight: 600;
  color: white;
  letter-spacing: 2px;
  font-family: 'Courier New', monospace;
  margin: 1rem 0;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
}

.card-holder {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.9);
  text-transform: uppercase;
  letter-spacing: 0.8px;
  font-weight: 600;
}

.card-expiry {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.9);
  letter-spacing: 0.8px;
  font-weight: 600;
}

/* Formulario */
.payment-form {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 1.5rem;
  position: relative;
}

.form-label {
  display: block;
  font-size: 0.8rem;
  font-weight: 700;
  color: #1a1a1a;
  margin-bottom: 0.6rem;
  text-transform: uppercase;
  letter-spacing: 0.6px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.form-input {
  width: 100%;
}

.form-input :deep(.v-field) {
  border: 1px solid #e5e7eb;
  background: white;
  border-radius: 10px;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.form-input :deep(.v-field:hover) {
  border-color: #d1d5db;
}

.form-input :deep(.v-field--focused) {
  border-color: #6366f1 !important;
  box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1) !important;
}

.form-input :deep(input) {
  color: #1a1a1a;
  font-size: 0.95rem;
  font-weight: 500;
}

.form-input :deep(input::placeholder) {
  color: rgba(0, 0, 0, 0.6);
}

.form-input :deep(.v-field--error) {
  border-color: #ef4444 !important;
}

.form-input :deep(.v-field--error:focus) {
  box-shadow: 0 0 0 4px rgba(239, 68, 68, 0.1) !important;
}

.card-icons {
  position: absolute;
  right: 12px;
  display: flex;
  align-items: center;
  pointer-events: none;
}

.error-text {
  font-size: 0.75rem;
  color: #ef4444;
  margin-top: 0.4rem;
  font-weight: 500;
}

/* Form row */
.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-row .form-group {
  margin-bottom: 1.5rem;
}

/* Métodos de pago */
.payment-methods {
  display: flex;
  gap: 1rem;
  margin: 2rem 0;
  padding: 1.5rem 0;
  border-top: 1px solid #e5e7eb;
  border-bottom: 1px solid #e5e7eb;
}

.method {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 50px;
  border-radius: 10px;
  background: #f3f4f6;
  color: #64748b;
  border: 1px solid #e5e7eb;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  cursor: pointer;
}

.method:hover {
  background: #f0f1f3;
  color: #475569;
}

.method.visa:hover {
  border-color: #1a1f71;
  color: #1a1f71;
}

.method.mastercard:hover {
  border-color: #eb001b;
  color: #eb001b;
}

.method.amex:hover {
  border-color: #006fcf;
  color: #006fcf;
}

/* Alert de error */
.error-alert {
  margin-bottom: 1.5rem;
  border-radius: 10px;
}

/* Botón de envío */
.submit-btn {
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%) !important;
  color: white !important;
  font-weight: 700;
  font-size: 1rem;
  text-transform: none;
  letter-spacing: 0.3px;
  border-radius: 10px !important;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  margin-top: auto;
  margin-bottom: 1rem;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.3);
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(99, 102, 241, 0.4);
}

.submit-btn:disabled {
  opacity: 0.7;
}

/* Pie de página */
.form-footer {
  font-size: 0.75rem;
  color: rgba(0, 0, 0, 0.6);
  text-align: center;
  margin: 0;
  line-height: 1.5;
}

/* Responsive */
@media (max-width: 600px) {
  .card-form {
    padding: 1.5rem;
    min-height: auto;
    border-radius: 16px;
  }

  .form-header {
    margin-bottom: 1.5rem;
  }

  .form-title {
    font-size: 1.2rem;
  }

  .card-preview {
    padding: 1.5rem;
    min-height: 160px;
    margin-bottom: 1.5rem;
  }

  .card-number-display {
    font-size: 1.2rem;
    letter-spacing: 1px;
  }

  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }

  .form-row .form-group:first-child {
    margin-bottom: 1.5rem;
  }

  .form-row .form-group:last-child {
    margin-bottom: 1.5rem;
  }

  .payment-methods {
    gap: 0.75rem;
  }

  .method {
    height: 44px;
  }

  .submit-btn {
    font-size: 0.95rem;
  }
}

@media (max-width: 480px) {
  .card-form {
    padding: 1.2rem;
    min-height: auto;
  }

  .form-label {
    font-size: 0.75rem;
  }

  .card-chip {
    width: 40px;
    height: 30px;
  }

  .card-number-display {
    font-size: 1rem;
    letter-spacing: 1px;
  }

  .card-holder,
  .card-expiry {
    font-size: 0.75rem;
  }
}
</style>