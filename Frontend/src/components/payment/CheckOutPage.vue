<script setup lang="ts">
import CreditCardForm from '@/components/payment/CreditCardForm.vue'
import PaymentSummary from '@/components/payment/PaymentSummary.vue'
import PaymentSnackbar from '@/components/payment/PaymentSnackbar.vue'
import { ref } from 'vue'

const snackbarRef = ref<InstanceType<typeof PaymentSnackbar>>()

const handlePaymentSuccess = () => {
  snackbarRef.value?.show()
}
</script>

<template>
  <div class="checkout-premium">
    <!-- Fondo decorativo con partículas -->
    <div class="particles-bg">
      <div class="particle" v-for="i in 35" :key="i" :style="{
        left: Math.random() * 100 + '%',
        animationDelay: Math.random() * 10 + 's',
        animationDuration: (5 + Math.random() * 8) + 's'
      }"></div>
    </div>

    <v-container fluid class="checkout-container">
      <!-- Sección Hero -->
      <div class="hero-section">
        <div class="hero-glow"></div>

        <!-- Contenedor del escudo -->
        <div class="shield-container">
          <div class="shield-rings">
            <div class="ring ring-1"></div>
            <div class="ring ring-2"></div>
            <div class="ring ring-3"></div>
          </div>
          <div class="shield-icon">🛡️</div>
        </div>

        <!-- Sección de título -->
        <div class="hero-title-section">
          <h1 class="hero-title">
            <span class="title-top">ASEGURA</span>
            <span class="title-main">TU ACCESO</span>
          </h1>
          <p class="hero-subtitle">Completa tu compra de forma segura y comienza tu transformación</p>
        </div>

        <!-- Divisor -->
        <div class="hero-divider">
          <div class="divider-line"></div>
          <v-icon class="divider-icon">mdi-lock-check</v-icon>
          <div class="divider-line"></div>
        </div>

        <!-- Tarjetas de seguridad -->
        <div class="security-grid">
          <div class="security-card security-1">
            <div class="security-icon-wrapper">
              <v-icon size="32" color="#22c55e">mdi-ssl</v-icon>
            </div>
            <div class="security-content">
              <div class="security-title">SSL Encriptado</div>
              <div class="security-text">256-bit protegido</div>
            </div>
          </div>

          <div class="security-card security-2">
            <div class="security-icon-wrapper">
              <v-icon size="32" color="#3b82f6">mdi-credit-card-check</v-icon>
            </div>
            <div class="security-content">
              <div class="security-title">Pago Verificado</div>
              <div class="security-text">Procesador confiable</div>
            </div>
          </div>

          <div class="security-card security-3">
            <div class="security-icon-wrapper">
              <v-icon size="32" color="#a855f7">mdi-lightning-bolt</v-icon>
            </div>
            <div class="security-content">
              <div class="security-title">Acceso al Instante</div>
              <div class="security-text">Actívate inmediatamente</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sección de formulario de pago -->
      <div class="payment-section">
        <v-row class="payment-grid" align="stretch">
          <v-col cols="12" lg="7">
            <CreditCardForm @payment-success="handlePaymentSuccess" />
          </v-col>

          <v-col cols="12" lg="5">
            <PaymentSummary />
          </v-col>
        </v-row>
      </div>

      <!-- Sección de confianza -->
      <div class="trust-section">
        <div class="trust-badge badge-1">
          <div class="badge-icon">
            <v-icon size="36" color="#22c55e">mdi-shield-check</v-icon>
          </div>
          <div class="badge-text">
            <div class="badge-title">Garantía de Reembolso</div>
            <div class="badge-subtitle">30 días sin preguntas</div>
          </div>
        </div>

        <div class="trust-badge badge-2">
          <div class="badge-icon">
            <v-icon size="36" color="#3b82f6">mdi-account-check</v-icon>
          </div>
          <div class="badge-text">
            <div class="badge-title">Soporte 24/7</div>
            <div class="badge-subtitle">Asistencia premium</div>
          </div>
        </div>

        <div class="trust-badge badge-3">
          <div class="badge-icon">
            <v-icon size="36" color="#a855f7">mdi-cog-outline</v-icon>
          </div>
          <div class="badge-text">
            <div class="badge-title">Cancela Cuando Quieras</div>
            <div class="badge-subtitle">Sin permanencia mínima</div>
          </div>
        </div>
      </div>
    </v-container>

    <!-- Snackbar de éxito -->
    <PaymentSnackbar ref="snackbarRef" />
  </div>
</template>

<style scoped>
.checkout-premium {
  position: relative;
  width: 100%;
  min-height: 100vh;
  background:
    radial-gradient(ellipse at top, rgba(99, 102, 241, 0.15) 0%, transparent 50%),
    radial-gradient(ellipse at bottom, rgba(59, 130, 246, 0.15) 0%, transparent 50%),
    linear-gradient(180deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
  overflow-x: hidden;
}

/* Partículas animadas */
.particles-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
  z-index: 0;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(148, 163, 184, 0.5);
  border-radius: 50%;
  animation: particle-rise linear infinite;
}

@keyframes particle-rise {
  0% {
    transform: translateY(100vh) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) scale(1);
    opacity: 0;
  }
}

.checkout-container {
  position: relative;
  z-index: 1;
  padding: 3rem 2rem;
}

/* Sección Hero */
.hero-section {
  position: relative;
  text-align: center;
  padding: 3rem 0 4rem;
  margin-bottom: 4rem;
}

.hero-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 1000px;
  height: 1000px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.1) 0%, transparent 70%);
  pointer-events: none;
  border-radius: 50%;
}

/* Contenedor del escudo */
.shield-container {
  position: relative;
  display: inline-block;
  margin-bottom: 2rem;
  animation: fadeInScale 0.8s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes fadeInScale {
  from {
    opacity: 0;
    transform: scale(0.8);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.shield-rings {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
}

.ring {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border: 2px solid rgba(99, 102, 241, 0.3);
  border-radius: 50%;
  animation: ring-expand 4s ease-out infinite;
}

.ring-1 {
  width: 100px;
  height: 100px;
  animation-delay: 0s;
}

.ring-2 {
  width: 100px;
  height: 100px;
  animation-delay: 1.3s;
}

.ring-3 {
  width: 100px;
  height: 100px;
  animation-delay: 2.6s;
}

@keyframes ring-expand {
  0% {
    width: 100px;
    height: 100px;
    opacity: 1;
  }
  100% {
    width: 280px;
    height: 280px;
    opacity: 0;
  }
}

.shield-icon {
  font-size: 5rem;
  animation: shield-float 3s ease-in-out infinite;
  filter: drop-shadow(0 0 40px rgba(99, 102, 241, 0.6));
}

@keyframes shield-float {
  0%, 100% {
    transform: translateY(0) rotate(-5deg);
    filter: drop-shadow(0 0 40px rgba(99, 102, 241, 0.6));
  }
  50% {
    transform: translateY(-15px) rotate(5deg);
    filter: drop-shadow(0 0 60px rgba(99, 102, 241, 0.8));
  }
}

/* Título Hero */
.hero-title-section {
  margin-bottom: 2.5rem;
}

.hero-title {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin: 0 0 1rem;
}

.title-top {
  font-size: clamp(1.2rem, 3vw, 1.8rem);
  font-weight: 700;
  color: rgba(148, 163, 184, 0.7);
  letter-spacing: 8px;
  text-transform: uppercase;
  animation: slideDown 0.8s cubic-bezier(0.34, 1.56, 0.64, 1) 0.1s both;
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

.title-main {
  font-size: clamp(3rem, 8vw, 5.5rem);
  font-weight: 900;
  background: linear-gradient(135deg, #6366f1 0%, #a855f7 50%, #6366f1 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 4px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite, slideDown 0.8s cubic-bezier(0.34, 1.56, 0.64, 1) 0.2s both;
}

@keyframes gradient-shift {
  0%, 100% {
    background-position: 0% center;
  }
  50% {
    background-position: 100% center;
  }
}

.hero-subtitle {
  font-size: clamp(1rem, 2vw, 1.3rem);
  color: rgba(148, 163, 184, 0.7);
  font-weight: 400;
  letter-spacing: 1px;
  max-width: 700px;
  margin: 0 auto;
  animation: slideDown 0.8s cubic-bezier(0.34, 1.56, 0.64, 1) 0.3s both;
}

/* Divisor Hero */
.hero-divider {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  max-width: 600px;
  margin: 0 auto 3rem;
}

.divider-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(99, 102, 241, 0.5), transparent);
}

.divider-icon {
  color: #22c55e;
  font-size: 2rem !important;
  animation: lock-pulse 1.5s ease-in-out infinite;
}

@keyframes lock-pulse {
  0%, 100% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.8;
    transform: scale(1.2);
  }
}

/* Grid de seguridad */
.security-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  max-width: 1000px;
  margin: 0 auto 3rem;
}

.security-card {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid;
  border-radius: 20px;
  padding: 1.75rem;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: card-entrance 0.6s ease-out backwards;
}

.security-1 {
  border-color: rgba(34, 197, 94, 0.3);
  animation-delay: 0.1s;
}

.security-2 {
  border-color: rgba(59, 130, 246, 0.3);
  animation-delay: 0.2s;
}

.security-3 {
  border-color: rgba(168, 85, 247, 0.3);
  animation-delay: 0.3s;
}

@keyframes card-entrance {
  0% {
    opacity: 0;
    transform: translateY(30px) scale(0.9);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.security-card:hover {
  transform: translateY(-8px) scale(1.02);
}

.security-1:hover {
  border-color: rgba(34, 197, 94, 0.6);
  box-shadow: 0 15px 40px rgba(34, 197, 94, 0.3);
}

.security-2:hover {
  border-color: rgba(59, 130, 246, 0.6);
  box-shadow: 0 15px 40px rgba(59, 130, 246, 0.3);
}

.security-3:hover {
  border-color: rgba(168, 85, 247, 0.6);
  box-shadow: 0 15px 40px rgba(168, 85, 247, 0.3);
}

.security-icon-wrapper {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  background: rgba(0, 0, 0, 0.3);
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.security-content {
  flex: 1;
  text-align: left;
}

.security-title {
  font-size: 1.1rem;
  font-weight: 800;
  color: white;
  margin-bottom: 0.25rem;
}

.security-text {
  font-size: 0.9rem;
  color: rgba(148, 163, 184, 0.7);
}

/* Sección de formulario de pago */
.payment-section {
  margin-bottom: 4rem;
  animation: fadeIn 0.8s ease-out 0.4s both;
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
  gap: 2rem;
  display: flex;
  align-items: stretch;
}

.payment-grid .v-col {
  display: flex;
  flex-direction: column;
}

.payment-grid .v-col > * {
  height: 100%;
}

/* Sección de confianza */
.trust-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 1.5rem;
  max-width: 1200px;
  margin: 0 auto 4rem;
  padding: 0 1rem;
}

.trust-badge {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid rgba(99, 102, 241, 0.3);
  border-radius: 20px;
  padding: 1.5rem;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: badge-entrance 0.6s ease-out backwards;
}

.badge-1 {
  animation-delay: 0.1s;
}

.badge-2 {
  animation-delay: 0.2s;
}

.badge-3 {
  animation-delay: 0.3s;
}

@keyframes badge-entrance {
  0% {
    opacity: 0;
    transform: translateY(30px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

.trust-badge:hover {
  transform: translateY(-8px);
  border-color: rgba(99, 102, 241, 0.6);
  box-shadow: 0 15px 40px rgba(99, 102, 241, 0.2);
}

.badge-icon {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.3);
  flex-shrink: 0;
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.badge-title {
  font-size: 1rem;
  font-weight: 800;
  color: white;
  margin-bottom: 0.25rem;
}

.badge-subtitle {
  font-size: 0.85rem;
  color: rgba(148, 163, 184, 0.6);
}

/* Responsive */
@media (max-width: 960px) {
  .checkout-container {
    padding: 2rem 1.5rem;
  }

  .hero-section {
    padding: 2rem 0 3rem;
    margin-bottom: 3rem;
  }

  .shield-icon {
    font-size: 4rem;
  }

  .security-grid {
    grid-template-columns: 1fr;
  }

  .payment-grid {
    gap: 1.5rem;
  }
}

@media (max-width: 600px) {
  .checkout-container {
    padding: 1.5rem 1rem;
  }

  .shield-icon {
    font-size: 3.5rem;
  }

  .hero-subtitle {
    font-size: 0.95rem;
  }

  .trust-section {
    grid-template-columns: 1fr;
    padding: 0;
  }

  .trust-badge {
    flex-direction: column;
    text-align: center;
  }

  .badge-text {
    text-align: center;
  }

  .title-main {
    font-size: 2.5rem;
  }

  .title-top {
    letter-spacing: 4px;
    font-size: 1.1rem;
  }
}

@media (max-width: 480px) {
  .checkout-container {
    padding: 1rem 0.8rem;
  }

  .hero-section {
    padding: 1.5rem 0 2.5rem;
    margin-bottom: 2.5rem;
  }

  .shield-icon {
    font-size: 3rem;
  }

  .title-main {
    font-size: 2rem;
  }

  .hero-subtitle {
    font-size: 0.9rem;
  }

  .security-card {
    padding: 1.25rem;
    flex-direction: column;
    text-align: center;
  }

  .security-content {
    text-align: center;
  }

  .trust-section {
    gap: 1rem;
    margin: 0 auto 2.5rem;
  }

  .trust-badge {
    padding: 1.2rem;
  }

  .badge-icon {
    width: 48px;
    height: 48px;
  }
}
</style>