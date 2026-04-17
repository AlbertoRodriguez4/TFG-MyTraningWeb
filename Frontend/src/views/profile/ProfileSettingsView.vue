<template>
  <div class="profile-settings">
    <profile-header />

    <v-container class="settings-container">
      <v-row class="mt-4">
        <v-col cols="12" md="3">
          <profile-sidebar v-model="activeTab" />
        </v-col>

        <v-col cols="12" md="9">
          <profile-form v-if="activeTab === 'personal'" />

          <security-settings v-if="activeTab === 'security'" />

          <notification-settings v-if="activeTab === 'notifications'" />

          <subscription-card v-if="activeTab === 'subscription'" />

        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ProfileHeader from '../../components/ProfileSettings/ProfileHeader.vue'
import ProfileSidebar from '../../components/ProfileSettings/ProfileSidebar.vue'
import ProfileForm from '../../components/ProfileSettings/ProfileForm.vue'
import SecuritySettings from '../../components/ProfileSettings/SecuritySettings.vue'
import NotificationSettings from '../../components/ProfileSettings/NotificationSettings.vue'
import SubscriptionCard from '../../components/ProfileSettings/SubscriptionCard.vue'

const activeTab = ref('personal')
</script>

<style scoped>
.profile-settings {
  min-height: 100vh;
  background: linear-gradient(135deg, rgba(0, 0, 0, 0.98) 0%, rgba(15, 15, 15, 0.95) 100%);
}

.settings-container {
  max-width: 1400px;
}

/* Responsive adjustments */
@media (max-width: 960px) {
  .settings-container {
    padding: 0 16px;
  }
}

@media (max-width: 600px) {
  .settings-container {
    padding: 0 12px;
  }
}
</style>