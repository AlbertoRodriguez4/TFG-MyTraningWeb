<template>
  <v-snackbar
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    color="green"
    timeout="3000"
    top
    right
    rounded="pill"
  >
    <div class="d-flex align-center">
      <v-avatar size="40" color="white" class="mr-3">
        <v-icon color="green">mdi-check-bold</v-icon>
      </v-avatar>
      <div>
        <div class="font-weight-bold">¡Excelente Trabajo!</div>
        <div class="text-caption">Rutina completada exitosamente</div>
      </div>
    </div>
  </v-snackbar>
</template>

<script setup lang="ts">
defineProps<{
  modelValue: boolean
}>()

defineEmits<{
  'update:modelValue': [value: boolean]
}>()
</script>

<style scoped>
@media (max-width: 600px) {
  .d-flex.align-center .v-avatar {
    width: 32px !important;
    height: 32px !important;
    min-width: 32px !important;
  }

  .d-flex.align-center .font-weight-bold {
    font-size: 0.85rem;
  }

  .d-flex.align-center .text-caption {
    font-size: 0.7rem;
  }
}

@media (max-width: 360px) {
  .d-flex.align-center .v-avatar {
    width: 28px !important;
    height: 28px !important;
    min-width: 28px !important;
  }

  .d-flex.align-center .font-weight-bold {
    font-size: 0.75rem;
  }

  .d-flex.align-center .text-caption {
    display: none;
  }
}
</style>