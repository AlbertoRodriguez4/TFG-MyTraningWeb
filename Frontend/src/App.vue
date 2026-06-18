<script setup lang="ts">
import { RouterView } from 'vue-router';
import { onMounted } from 'vue';
import HeaderView from './views/layout/HeaderView.vue';
import FooterView from './views/layout/FooterView.vue';
import { useThemeStore } from '@/stores/useTheme';
import { useSubscriptionStore } from '@/stores/SubscriptionStore';

// Inicializar el store para aplicar el tema guardado
useThemeStore();

// Inicializar el store de suscripciones al cargar la app, necesario para manejar las notificaciones push y el header entre otras cosas
const subscriptionStore = useSubscriptionStore();
onMounted(async () => {
  await subscriptionStore.initializeSubscription();
});
</script>

<template>
  <v-app class="app-root">
    <div class="global-background-overlay"></div>

    <HeaderView />

    <v-main class="main-wrapper">
      <RouterView />
    </v-main>

    <FooterView />
  </v-app>
</template>

<style>
*,
*::before,
*::after {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

#app {
  width: 100%;
  height: 100%;
}

html,
body {
  width: 100%;
  height: 100%;
  overflow-x: hidden;
  overflow-y: scroll;
  scrollbar-width: none;
}

body::-webkit-scrollbar {
  display: none;
}

.v-application,
.v-application__wrap {
  background: transparent !important;
  min-height: 100vh !important;
}

.v-main {
  padding: 0 !important;
}

.v-main__wrap {
  padding: 0 !important;
  display: flex;
  flex-direction: column;
}
</style>

<style scoped>
.app-root {
  width: 100%;
  min-height: 100vh;
  overflow-x: hidden;
}

.main-wrapper {
  width: 100%;
  min-height: 100vh;
  padding: 0 !important;
  background: transparent !important;
  position: relative;
  z-index: 10;
}

/* Background Effects */
.global-background-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url('@/assets/imgs/gimansio-fondo.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  opacity: 0.15;
  z-index: 0;
  pointer-events: none;
}

.global-grid-pattern {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image:
    linear-gradient(rgba(13, 110, 253, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(13, 110, 253, 0.03) 1px, transparent 1px);
  background-size: 50px 50px;
  z-index: 1;
  pointer-events: none;
}

/* Gradient Orbs */
.global-gradient-orb {
  position: fixed;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.15;
  z-index: 1;
  pointer-events: none;

}

.orb-1 {
  width: 500px;
  height: 500px;
  background: radial-gradient(circle, #0D6EFD, transparent);
  top: -250px;
  right: -250px;
  animation-delay: 0s;
}

.orb-2 {
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, #6610f2, transparent);
  bottom: -200px;
  left: -200px;
  animation-delay: 5s;
}

.orb-3 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, #0dcaf0, transparent);
  top: 50%;
  left: 50%;
  animation-delay: 10s;
}
</style>