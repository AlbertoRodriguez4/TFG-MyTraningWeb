<script setup lang="ts">
import PlanRender from '../../components/Renders/PlanRender.vue'
</script>

<template>
  <div class="plan-view">
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-bg-glow"></div>

      <div class="hero-content">
        <v-icon class="hero-icon">mdi-trophy-variant-outline</v-icon>
        <h1 class="hero-title">Elige tu Plan</h1>
        <p class="hero-subtitle">Desbloquea tu máximo potencial con nuestros planes de entrenamiento premium</p>
      </div>
    </section>

    <!-- Plans Section -->
    <section class="plans-section">
      <PlanRender />
    </section>

    <!-- Features Grid -->
    <section class="features">
      <div class="features-grid">
        <div class="feature-card">
          <v-icon size="40" color="#00ff88">mdi-lightning-bolt</v-icon>
          <h3 class="feature-title">Activación Inmediata</h3>
          <p class="feature-text">Acceso instantáneo al contenido premium</p>
        </div>

        <div class="feature-card">
          <v-icon size="40" color="#00D2FF">mdi-account-group</v-icon>
          <h3 class="feature-title">Comunidad Élite</h3>
          <p class="feature-text">Entrena con los mejores atletas</p>
        </div>

        <div class="feature-card">
          <v-icon size="40" color="#A855F7">mdi-chart-timeline-variant</v-icon>
          <h3 class="feature-title">Progreso Detallado</h3>
          <p class="feature-text">Analytics y estadísticas avanzadas</p>
        </div>

        <div class="feature-card">
          <v-icon size="40" color="#FFD700">mdi-headset</v-icon>
          <h3 class="feature-title">Soporte 24/7</h3>
          <p class="feature-text">Asistencia premium cuando la necesites</p>
        </div>
      </div>
    </section>

  </div>
</template>

<style scoped>
.plan-view {
  position: relative;
  min-height: 100vh;
  background: linear-gradient(180deg, #0a0e1a 0%, #0f1419 50%, #0a0e1a 100%);
  overflow-x: hidden;
}

/* Hero Section */
.hero {
  position: relative;
  text-align: center;
  padding: 5rem 2rem 3rem;
}

.hero-bg-glow {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 800px;
  height: 800px;
  background: radial-gradient(circle, rgba(255, 215, 0, 0.08) 0%, transparent 70%);
  pointer-events: none;
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
  animation: icon-glow 3s ease-in-out infinite;
}

@keyframes icon-glow {

  0%,
  100% {
    filter: drop-shadow(0 0 30px rgba(255, 215, 0, 0.6));
  }

  50% {
    filter: drop-shadow(0 0 50px rgba(255, 215, 0, 0.9));
  }
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
  animation: gradient-shift 4s ease infinite;
}

@keyframes gradient-shift {

  0%,
  100% {
    background-position: 0% center;
  }

  50% {
    background-position: 100% center;
  }
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
}

/* Features Grid */
.features {
  padding: 2rem 1rem 3rem;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  max-width: 1100px;
  margin: 0 auto;
}

.feature-card {
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 2rem 1.5rem;
  text-align: center;
  transition: all 0.3s ease;
}

.feature-card:hover {
  transform: translateY(-5px);
  border-color: rgba(255, 215, 0, 0.3);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
}

.feature-title {
  font-size: 1.1rem;
  font-weight: 700;
  color: #ffffff;
  margin: 1rem 0 0.5rem;
}

.feature-text {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.5;
}

/* CTA Section */
.cta-section {
  padding: 2rem 1rem 3rem;
}

.cta-card {
  max-width: 650px;
  margin: 0 auto;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.1) 0%, rgba(255, 165, 0, 0.08) 100%);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 215, 0, 0.2);
  border-radius: 20px;
  padding: 3rem 2rem;
  text-align: center;
  transition: all 0.3s ease;
}

.cta-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 20px 50px rgba(255, 215, 0, 0.15);
}

.cta-icon {
  margin-bottom: 1rem;
}

.cta-title {
  font-size: 1.6rem;
  font-weight: 800;
  color: #ffffff;
  margin: 0 0 1rem;
}

.cta-text {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.6;
  margin: 0 0 1.5rem;
}

.cta-btn {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%) !important;
  color: #000 !important;
  font-weight: 800;
  letter-spacing: 1px;
  padding: 1.25rem 2rem !important;
  border-radius: 50px !important;
  box-shadow: 0 8px 25px rgba(255, 215, 0, 0.3);
  transition: all 0.3s ease;
}

.cta-btn:hover {
  transform: translateY(-2px) scale(1.03);
  box-shadow: 0 12px 35px rgba(255, 215, 0, 0.5);
}

/* Trust Section */
.trust-section {
  padding: 2rem 1rem 4rem;
}

.trust-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  max-width: 1000px;
  margin: 0 auto;
}

.trust-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 1.25rem;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 12px;
  transition: all 0.3s ease;
}

.trust-item:hover {
  background: rgba(255, 255, 255, 0.06);
  transform: translateY(-2px);
}

.trust-text {
  font-size: 0.9rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.8);
}

/* Responsive */
@media (max-width: 960px) {
  .hero {
    padding: 4rem 1.5rem 2.5rem;
  }

  .hero-icon {
    font-size: 3.5rem !important;
  }

  .features-grid {
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  }
}

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

  .features-grid {
    grid-template-columns: 1fr;
  }

  .feature-card {
    padding: 1.5rem;
  }

  .cta-card {
    padding: 2rem 1.5rem;
  }

  .cta-title {
    font-size: 1.3rem;
  }

  .cta-text {
    font-size: 0.9rem;
  }

  .trust-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 400px) {
  .trust-grid {
    grid-template-columns: 1fr;
  }
}
</style>
