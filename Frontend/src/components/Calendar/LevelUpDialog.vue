<template>
  <v-dialog
    :value="value"
    @input="$emit('input', $event)"
    max-width="500"
    persistent
  >
    <v-card rounded="xl" class="level-up-dialog">
      <v-card-text class="pa-10 text-center">
        <div class="level-up-animation mb-4">
          <v-icon size="140" color="amber">mdi-trophy-variant</v-icon>
        </div>
        <h2 class="text-h2 font-weight-black purple--text mb-2">
          ¡NIVEL {{ userLevel }}!
        </h2>
        <p class="text-h5 mb-2">¡Felicitaciones!</p>
        <p class="text-body-1 grey--text mb-6">
          Has alcanzado un nuevo nivel. Sigue entrenando para desbloquear más recompensas.
        </p>
        <v-btn
          x-large
          color="purple"
          dark
          block
          rounded
          @click="$emit('input', false)"
        >
          <v-icon left>mdi-rocket-launch</v-icon>
          ¡Continuar!
        </v-btn>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  name: 'LevelUpDialog',
  props: {
    value: {
      type: Boolean,
      default: false
    },
    userLevel: {
      type: Number,
      required: true
    }
  }
};
</script>

<style scoped>
.level-up-dialog {
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
  border: 2px solid rgba(139, 92, 246, 0.3);
  backdrop-filter: blur(20px);
}

.level-up-animation {
  animation: trophy-rotate 1s ease-out;
}

@keyframes trophy-rotate {
  0% {
    transform: scale(0) rotate(-180deg);
    opacity: 0;
  }
  50% {
    transform: scale(1.2) rotate(180deg);
  }
  100% {
    transform: scale(1) rotate(360deg);
    opacity: 1;
  }
}

@media (max-width: 600px) {
  .level-up-dialog .v-card__text {
    padding: 2rem 1.5rem !important;
  }
  
  .level-up-animation .v-icon {
    font-size: 100px !important;
  }
  
  .level-up-dialog .text-h2 {
    font-size: 2.5rem !important;
  }
}
</style>