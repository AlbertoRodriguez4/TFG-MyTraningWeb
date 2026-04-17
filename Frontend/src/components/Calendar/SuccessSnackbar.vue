<template>
  <v-snackbar
    :value="value"
    @input="$emit('input', $event)"
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

<script>
export default {
  name: 'SuccessSnackbar',
  props: {
    value: {
      type: Boolean,
      default: false
    }
  }
};
</script>

<style scoped>
</style>