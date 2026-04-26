<script setup lang="ts">
import PlanRender from '../../components/Renders/PlanRender.vue'
</script>

<template>
  <div class="plan-view">
    <section class="hero">
      <div class="hero-bg-glow"></div>
      <div class="hero-content">
        <v-icon class="hero-icon">mdi-trophy-variant-outline</v-icon>
        <h1 class="hero-title">Elige tu Plan</h1>
        <p class="hero-subtitle">Desbloquea tu máximo potencial con nuestros planes de entrenamiento</p>
      </div>
    </section>

    <section class="plans-section">
      <PlanRender />
    </section>

    <section class="features">
      <div class="features-grid">
        <div class="feature-card">
          <v-icon size="32" color="#00ff88">mdi-lightning-bolt</v-icon>
          <h3 class="feature-title">Activación Inmediata</h3>
          <p class="feature-text">Acceso instantáneo al contenido premium</p>
        </div>
        <div class="feature-card">
          <v-icon size="32" color="#00D2FF">mdi-account-group</v-icon>
          <h3 class="feature-title">Comunidad Élite</h3>
          <p class="feature-text">Entrena con los mejores atletas</p>
        </div>
        <div class="feature-card">
          <v-icon size="32" color="#A855F7">mdi-chart-timeline-variant</v-icon>
          <h3 class="feature-title">Progreso Detallado</h3>
          <p class="feature-text">Analytics y estadísticas avanzadas</p>
        </div>
        <div class="feature-card">
          <v-icon size="32" color="#FFD700">mdi-headset</v-icon>
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

.hero {
  position: relative;
  text-align: center;
  padding: 3rem 1.5rem 2rem;
}

.hero-bg-glow {
  position: absolute;
  top: 0; left: 50%;
  transform: translateX(-50%);
  width: 600px; height: 600px;
  background: radial-gradient(circle, rgba(255, 215, 0, 0.06) 0%, transparent 70%);
  pointer-events: none;
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-icon {
  font-size: 3rem !important;
  color: #FFD700;
  filter: drop-shadow(0 0 20px rgba(255, 215, 0, 0.5));
  margin-bottom: 1rem;
}

.hero-title {
  font-size: clamp(2rem, 5vw, 3rem);
  font-weight: 900;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  margin: 0 0 0.75rem;
  animation: gradient-shift 4s ease infinite;
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

.hero-subtitle {
  font-size: clamp(0.9rem, 1.8vw, 1.1rem);
  color: rgba(255, 255, 255, 0.6);
  font-weight: 400;
  max-width: 500px;
  margin: 0 auto;
  line-height: 1.5;
}

.plans-section {
  padding: 1rem 1rem 2.5rem;
}

.features {
  padding: 1rem 1rem 2.5rem;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  max-width: 900px;
  margin: 0 auto;
}

.feature-card {
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 14px;
  padding: 1.5rem 1.25rem;
  text-align: center;
  transition: all 0.3s ease;
}

.feature-card:hover {
  transform: translateY(-4px);
  border-color: rgba(255, 215, 0, 0.25);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
}

.feature-title {
  font-size: 0.95rem;
  font-weight: 700;
  color: #ffffff;
  margin: 0.75rem 0 0.4rem;
}

.feature-text {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.55);
  line-height: 1.4;
  margin: 0;
}

@media (max-width: 960px) {
  .hero { padding: 2.5rem 1.25rem 1.5rem; }
  .hero-icon { font-size: 2.5rem !important; }
  .features-grid { grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); }
}

@media (max-width: 600px) {
  .hero { padding: 2rem 1rem 1.25rem; }
  .hero-icon { font-size: 2.2rem !important; }
  .hero-title { font-size: 1.7rem; }
  .hero-subtitle { font-size: 0.85rem; }
  .features-grid { grid-template-columns: 1fr 1fr; gap: 0.75rem; }
  .feature-card { padding: 1.25rem 1rem; }
}

@media (max-width: 400px) {
  .features-grid { grid-template-columns: 1fr; }
}
</style>
