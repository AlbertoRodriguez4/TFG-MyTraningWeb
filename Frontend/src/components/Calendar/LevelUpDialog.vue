<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="500"
    persistent
  >
    <v-card rounded="xl" class="level-up-dialog">
      <v-card-text class="pa-10 text-center">
        <div class="level-up-animation mb-4">
          <v-icon size="140" color="amber">mdi-trophy-variant</v-icon>
        </div>
        <h2 class="text-h2 font-weight-black purple--text mb-2">
          {{ $t('levelUp.newLevel', { level: userLevel }) }}
        </h2>
          <p class="text-h5 mb-2">{{ $t('levelUp.congrats') }}</p>
          <p class="text-body-1 grey--text mb-6">
          {{ $t('levelUp.description') }}
        </p>
        <v-btn
          x-large
          color="purple"
          dark
          block
          rounded
          @click="$emit('update:modelValue', false)"
        >
          <v-icon left>mdi-rocket-launch</v-icon>
          {{ $t('levelUp.continue') }}
        </v-btn>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: boolean
  userLevel: number
}>()

defineEmits<{
  'update:modelValue': [value: boolean]
}>()
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

@media (max-width: 960px) {
  .level-up-animation .v-icon {
    font-size: 120px !important;
  }

  .level-up-dialog .text-h2 {
    font-size: 2.75rem !important;
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