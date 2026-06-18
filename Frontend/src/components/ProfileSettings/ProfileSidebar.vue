<template>
  <v-card class="sidebar-card" elevation="0" border>
    <v-list class="settings-list">
      <v-list-item
        v-for="item in menuItems"
        :key="item.id"
        :title="item.label"
        :prepend-icon="item.icon"
        @click="$emit('update:modelValue', item.id)"
        :active="modelValue === item.id"
        class="menu-item"
      />
    </v-list>
  </v-card>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

defineProps<{
  modelValue: string
}>()

defineEmits<{
  'update:modelValue': [value: string]
}>()
// Elementos que componen el menú lateral de configuración del perfil
const menuItems = [
  { id: 'personal', label: t('personal_info'), icon: 'mdi-account-circle' },
  { id: 'security', label: t('security_password'), icon: 'mdi-shield-lock' },
  { id: 'notifications', label: 'Notificaciones', icon: 'mdi-bell' },
]
</script>

<style scoped>
.sidebar-card {
  background: rgba(20, 20, 20, 0.5) !important;
  border: 1px solid rgba(255, 204, 0, 0.15) !important;
  border-radius: 12px;
  position: sticky;
  top: 80px;
}

.settings-list {
  padding: 0.5rem 0;
  background: transparent !important;
}

.menu-item {
  color: rgba(255, 255, 255, 0.7) !important;
  border-left: 3px solid transparent;
  transition: all 0.25s ease;
  margin: 0.4rem 0;
}

.menu-item.v-list-item--active {
  background: rgba(255, 204, 0, 0.12) !important;
  color: #ffcc00 !important;
  border-left-color: #ffcc00;
  font-weight: 600;
}

/* Responsive */
@media (max-width: 960px) {
  .sidebar-card {
    position: static;
    margin-bottom: 2rem;
  }
}

@media (max-width: 600px) {
  .sidebar-card {
    margin-bottom: 1.5rem;
  }

  .menu-item {
    padding: 0.6rem 0.8rem !important;
  }
}
</style>