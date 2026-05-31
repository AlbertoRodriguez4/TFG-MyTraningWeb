<script setup lang="ts">
</script>

<template>
  <div class="user-view-header">
    <div class="header-content">
      <!-- Animated Background Glow -->
      <div class="header-glow"></div>

      <!-- Icon Wrapper -->
      <div class="icon-wrapper">
        <!-- Rotating Rings -->
        <div class="icon-rings">
          <div class="ring ring-outer"></div>
          <div class="ring ring-middle"></div>
          <div class="ring ring-inner"></div>
        </div>

        <!-- Sparkles -->
        <div class="sparkles">
          <div class="sparkle" v-for="i in 5" :key="i" :style="{
            left: `${20 + i * 15}%`,
            top: `${-10 + (i % 3) * 20}%`,
            animationDelay: `${i * 0.2}s`
          }"></div>
        </div>

        <!-- Main Trophy Icon -->
        <div class="trophy-container">
          <v-icon class="trophy-icon">mdi-trophy-variant</v-icon>
          <div class="trophy-shine"></div>
        </div>
      </div>

      <!-- Title -->
      <h1 class="title">
        <span class="title-sub">{{ $t('HALL OF') }}</span>
        <span class="title-main">
          <span class="title-text">{{ $t('CHAMPIONS') }}</span>
          <div class="title-glow"></div>
        </span>
      </h1>

      <!-- Subtitle -->
      <p class="subtitle">
        <span class="subtitle-icon"><v-icon>mdi-star-four-points</v-icon></span>
        {{ $t('Los atletas legendarios de TheTrainingHub') }}
        <span class="subtitle-icon"><v-icon>mdi-star-four-points</v-icon></span>
      </p>

      <!-- Decorative Divider -->
      <div class="divider">
        <div class="divider-line"></div>
        <div class="divider-diamond">
          <v-icon size="20">mdi-diamond-stone</v-icon>
        </div>
        <div class="divider-line"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.user-view-header {
  text-align: center;
  margin-bottom: 2.5rem;
  position: relative;
}

.header-content {
  display: inline-block;
  position: relative;
}

/* Background Glow */
.header-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 300px;
  height: 300px;
  background: radial-gradient(ellipse, rgba(255, 215, 0, 0.15), transparent 70%);
  filter: blur(40px);
  pointer-events: none;
  animation: glow-pulse 3s ease-in-out infinite;
}

@keyframes glow-pulse {
  0%, 100% { opacity: 0.5; transform: translate(-50%, -50%) scale(1); }
  50% { opacity: 1; transform: translate(-50%, -50%) scale(1.2); }
}

/* Icon Wrapper */
.icon-wrapper {
  position: relative;
  display: inline-block;
  margin-bottom: 1rem;
}

/* Rotating Rings */
.icon-rings {
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
  border-radius: 50%;
  border: 2px solid transparent;
  animation: ring-rotate linear infinite;
}

.ring-outer {
  width: 140px;
  height: 140px;
  border-top-color: rgba(255, 215, 0, 0.4);
  border-right-color: rgba(255, 215, 0, 0.2);
  animation-duration: 8s;
}

.ring-middle {
  width: 110px;
  height: 110px;
  border-bottom-color: rgba(255, 215, 0, 0.5);
  border-left-color: rgba(255, 215, 0, 0.3);
  animation-duration: 6s;
  animation-direction: reverse;
}

.ring-inner {
  width: 80px;
  height: 80px;
  border-top-color: rgba(255, 215, 0, 0.6);
  border-bottom-color: rgba(255, 215, 0, 0.4);
  animation-duration: 4s;
}

@keyframes ring-rotate {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

/* Sparkles */
.sparkles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.sparkle {
  position: absolute;
  width: 6px;
  height: 6px;
  background: #FFD700;
  border-radius: 50%;
  box-shadow: 0 0 10px #FFD700, 0 0 20px rgba(255, 215, 0, 0.5);
  animation: sparkle-twinkle 1.5s ease-in-out infinite;
}

@keyframes sparkle-twinkle {
  0%, 100% {
    opacity: 0;
    transform: scale(0) rotate(0deg);
  }
  50% {
    opacity: 1;
    transform: scale(1) rotate(180deg);
  }
}

/* Trophy Container */
.trophy-container {
  position: relative;
  display: inline-block;
}

.trophy-icon {
  font-size: 5rem !important;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 0 30px rgba(255, 215, 0, 0.6));
  animation: trophy-float 3s ease-in-out infinite;
}

@keyframes trophy-float {
  0%, 100% {
    transform: translateY(0) scale(1);
  }
  50% {
    transform: translateY(-10px) scale(1.05);
  }
}

.trophy-shine {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80px;
  height: 80px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.4), transparent 70%);
  border-radius: 50%;
  filter: blur(10px);
  animation: shine-pulse 2s ease-in-out infinite;
}

@keyframes shine-pulse {
  0%, 100% { opacity: 0.3; transform: translate(-50%, -50%) scale(1); }
  50% { opacity: 0.6; transform: translate(-50%, -50%) scale(1.2); }
}

/* Title */
.title {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  margin: 1.5rem 0 1rem;
  position: relative;
}

.title-sub {
  font-size: 1.4rem;
  font-weight: 800;
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: 6px;
  text-transform: uppercase;
  text-shadow: 0 0 20px rgba(255, 255, 255, 0.1);
}

.title-main {
  position: relative;
  display: inline-block;
}

.title-text {
  font-size: 3.5rem;
  font-weight: 900;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 4px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite;
  position: relative;
  z-index: 2;
}

@keyframes gradient-shift {
  0%, 100% { background-position: 0% center; }
  50% { background-position: 100% center; }
}

.title-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 50px;
  background: radial-gradient(ellipse, rgba(255, 215, 0, 0.3), transparent 70%);
  filter: blur(20px);
  pointer-events: none;
}

/* Subtitle */
.subtitle {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 500;
  letter-spacing: 1.5px;
  margin: 0.75rem 0 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.subtitle-icon {
  font-size: 1.2rem;
  animation: subtitle-sparkle 2s ease-in-out infinite;
}

@keyframes subtitle-sparkle {
  0%, 100% { opacity: 0.5; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.2); }
}

/* Decorative Divider */
.divider {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  max-width: 500px;
  margin: 0 auto;
}

.divider-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg,
    transparent,
    rgba(255, 215, 0, 0.2),
    rgba(255, 215, 0, 0.5),
    rgba(255, 215, 0, 0.2),
    transparent
  );
  position: relative;
  overflow: hidden;
}

.divider-line::after {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: line-shine 3s linear infinite;
}

@keyframes line-shine {
  to { left: 100%; }
}

.divider-diamond {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #FFD700;
  filter: drop-shadow(0 0 10px rgba(255, 215, 0, 0.6));
  animation: diamond-pulse 2s ease-in-out infinite;
}

@keyframes diamond-pulse {
  0%, 100% {
    opacity: 1;
    transform: scale(1) rotate(0deg);
  }
  50% {
    opacity: 0.8;
    transform: scale(1.1) rotate(45deg);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .trophy-icon {
    font-size: 3.5rem !important;
  }

  .title-sub {
    font-size: 1.1rem;
    letter-spacing: 4px;
  }

  .title-text {
    font-size: 2.5rem;
    letter-spacing: 2px;
  }

  .subtitle {
    font-size: 1rem;
  }

  .ring-outer {
    width: 110px;
    height: 110px;
  }

  .ring-middle {
    width: 85px;
    height: 85px;
  }

  .ring-inner {
    width: 60px;
    height: 60px;
  }
}

@media (max-width: 480px) {
  .trophy-icon {
    font-size: 2.8rem !important;
  }

  .title-sub {
    font-size: 0.9rem;
    letter-spacing: 3px;
  }

  .title-text {
    font-size: 2rem;
  }

  .subtitle {
    font-size: 0.9rem;
    flex-wrap: wrap;
  }

  .subtitle-icon {
    display: none;
  }
}
</style>
