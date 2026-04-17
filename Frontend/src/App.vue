<script setup lang="ts">
import { RouterView } from 'vue-router';
import { onMounted } from 'vue';
import HeaderView from './views/layout/HeaderView.vue';
import FooterView from './views/layout/FooterView.vue';
import { useThemeStore } from '@/stores/useTheme';
import { useSubscriptionStore } from '@/stores/SubscriptionStore';

// Inicializar el store para aplicar el tema guardado
useThemeStore();

// Inicializar el store de suscripciones al cargar la app
const subscriptionStore = useSubscriptionStore();
onMounted(async () => {
  await subscriptionStore.initializeSubscription();
});
</script>

<template>
  <v-app class="app-root">
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

/* Vuetify overrides */
.v-application,
.v-application__wrap {
  background: transparent !important;
  min-height: 100vh !important;
}

/* Eliminar el padding-top que Vuetify inyecta en v-main por el app bar */
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
  min-height: calc(100vh - 80px);
  padding: 0 !important;
  background: transparent !important;
}
</style>