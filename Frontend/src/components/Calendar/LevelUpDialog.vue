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
          <v-icon size="140" color="white">mdi-trophy-variant</v-icon>
        </div>
        <h2 class="text-h2 font-weight-black mb-2 level-title">
          {{ $t('levelUp.newLevel', { level: userLevel }) }}
        </h2>
          <p class="text-h5 mb-2 level-congrats">{{ $t('levelUp.congrats') }}</p>
          <p class="text-body-1 mb-6 level-description">
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

.level-title {
  color: #c4b5fd;
  text-shadow: 0 0 20px rgba(167, 139, 250, 0.5);
}

.level-congrats {
  color: #ffffff;
}

.level-description {
  color: rgba(255, 255, 255, 0.7);
}

.level-up-animation {

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