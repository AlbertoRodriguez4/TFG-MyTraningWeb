<script setup lang="ts">
</script>

<template>
  <div class="user-view-header">
    <div class="header-content">
      <div class="header-glow"></div>

      <div class="icon-wrapper">
        <div class="trophy-container">
          <v-icon class="trophy-icon">mdi-trophy-variant</v-icon>
        </div>
      </div>

      <h1 class="title">
        <span class="title-sub">HALL OF</span>
        <span class="title-main">CHAMPIONS</span>
      </h1>

      <p class="subtitle">
        Los atletas legendarios de TheTrainingHub
      </p>

      <div class="divider">
        <div class="divider-line"></div>
        <div class="divider-diamond">
          <v-icon size="16">mdi-diamond-stone</v-icon>
        </div>
        <div class="divider-line"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.user-view-header {
  text-align: center;
  margin-bottom: 2rem;
  position: relative;
}

.header-content {
  display: inline-block;
  position: relative;
}

.header-glow {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 200px; height: 200px;
  background: radial-gradient(ellipse, rgba(255, 215, 0, 0.12), transparent 70%);
  filter: blur(30px);
  pointer-events: none;
}

.icon-wrapper {
  position: relative;
  display: inline-block;
  margin-bottom: 0.75rem;
}

.trophy-icon {
  font-size: 3.5rem !important;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 0 20px rgba(255, 215, 0, 0.5));
}

.title {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  margin: 1rem 0 0.75rem;
  position: relative;
}

.title-sub {
  font-size: 1.1rem;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.45);
  letter-spacing: 5px;
  text-transform: uppercase;
}

.title-main {
  font-size: 2.5rem;
  font-weight: 900;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 3px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite;
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

.subtitle {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.55);
  font-weight: 500;
  letter-spacing: 1px;
  margin: 0.5rem 0 1rem;
}

.divider {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  max-width: 400px;
  margin: 0 auto;
}

.divider-line {
  flex: 1;
  height: 1.5px;
  background: linear-gradient(90deg, transparent, rgba(255, 215, 0, 0.2), rgba(255, 215, 0, 0.4), rgba(255, 215, 0, 0.2), transparent);
}

.divider-diamond {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #FFD700;
  filter: drop-shadow(0 0 6px rgba(255, 215, 0, 0.4));
}

@media (max-width: 768px) {
  .trophy-icon { font-size: 2.8rem !important; }
  .title-sub { font-size: 0.9rem; letter-spacing: 3px; }
  .title-main { font-size: 2rem; letter-spacing: 2px; }
  .subtitle { font-size: 0.85rem; }
}

@media (max-width: 480px) {
  .trophy-icon { font-size: 2.2rem !important; }
  .title-sub { font-size: 0.75rem; letter-spacing: 2px; }
  .title-main { font-size: 1.6rem; }
}
</style>
