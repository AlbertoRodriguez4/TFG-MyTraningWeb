<template>
  <v-card class="month-nav-card" elevation="0" rounded="xl">
    <v-card-text class="pa-5">
      <div class="d-flex align-center justify-space-between">
        <!-- Botón Mes Anterior -->
        <v-btn
          fab
          color="purple"
          dark
          @click="handlePrevious"
          elevation="4"
          class="nav-fab"
          aria-label="Mes anterior"
        >
          <v-icon>mdi-chevron-left</v-icon>
        </v-btn>

        <!-- Información del Mes y Año -->
        <div class="text-center month-info">
          <h2 class="text-h3 font-weight-black mb-1 month-title">
            {{ monthName }}
          </h2>
          <p class="text-h6 grey--text year-subtitle mb-0">
            {{ year }}
          </p>
        </div>

        <!-- Botón Mes Siguiente -->
        <v-btn
          fab
          color="purple"
          dark
          @click="handleNext"
          elevation="4"
          class="nav-fab"
          aria-label="Mes siguiente"
        >
          <v-icon>mdi-chevron-right</v-icon>
        </v-btn>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
defineProps<{
  monthName: string
  year: number
}>()

const emit = defineEmits<{
  previous: []
  next: []
}>()

function handlePrevious() {
  emit('previous')
}

function handleNext() {
  emit('next')
}
</script>

<style scoped>
/* Tarjeta principal de navegación - Tema Oscuro */
.month-nav-card {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.9) 0%, rgba(15, 10, 26, 0.95) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

/* Información del mes central - Tema Oscuro */
.month-info {
  flex: 1;
  user-select: none; /* Evita selección de texto */
}

.month-title {
  color: #ffffff;
  letter-spacing: 0.5px;
  line-height: 1.2;
  transition: color 0.3s ease;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.3);
}

.year-subtitle {
  font-weight: 600;
  color: rgba(255, 255, 255, 0.7);
  transition: opacity 0.3s ease;
}

/* Botones de navegación FAB */
.nav-fab {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.nav-fab:hover {
  transform: scale(1.1);
  box-shadow: 0 8px 30px rgba(167, 139, 250, 0.5) !important;
}

.nav-fab:active {
  transform: scale(0.95);
}

/* Efecto de ripple mejorado */
.nav-fab::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.nav-fab:hover::before {
  opacity: 1;
}

/* Iconos dentro de los botones */
.nav-fab .v-icon {
  transition: transform 0.3s ease;
}

.nav-fab:hover .v-icon {
  transform: scale(1.2);
}

/* Animación sutil al cambiar de mes */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.month-title,
.year-subtitle {
  animation: fadeIn 0.4s ease-out;
}

/* Responsive Design */
@media (max-width: 960px) {
  .month-nav-card .v-card__text {
    padding: 1.25rem !important;
  }
  
  .month-title {
    font-size: 2rem !important;
  }
  
  .year-subtitle {
    font-size: 1.1rem !important;
  }
}

@media (max-width: 600px) {
  .month-nav-card .v-card__text {
    padding: 1rem !important;
  }
  
  .month-title {
    font-size: 1.75rem !important;
  }
  
  .year-subtitle {
    font-size: 1rem !important;
  }
  
  .nav-fab {
    width: 48px !important;
    height: 48px !important;
  }
  
  .nav-fab .v-icon {
    font-size: 24px !important;
  }
}

@media (max-width: 400px) {
  .month-title {
    font-size: 1.5rem !important;
  }
  
  .year-subtitle {
    font-size: 0.9rem !important;
  }
  
  .nav-fab {
    width: 44px !important;
    height: 44px !important;
  }
}

/* Modo oscuro (opcional, si tu app lo tiene) */
@media (prefers-color-scheme: dark) {
  .month-nav-card {
    background: #1e1e1e;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  }
  
  .month-title {
    color: #ffffff;
  }
  
  .year-subtitle {
    color: rgba(255, 255, 255, 0.7);
  }
}

/* Estados de accesibilidad */
.nav-fab:focus {
  outline: 3px solid rgba(102, 126, 234, 0.5);
  outline-offset: 2px;
}

.nav-fab:focus:not(:focus-visible) {
  outline: none;
}

/* Prevenir click doble rápido */
.nav-fab {
  pointer-events: all;
  -webkit-tap-highlight-color: transparent;
}
</style>