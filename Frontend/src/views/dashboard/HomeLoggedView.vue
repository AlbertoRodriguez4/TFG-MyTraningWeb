<script setup lang="ts">
import UserStatsPanel from '../../components/Panels/UserStatsPanel.vue'
import InventoryPanel from '../../components/Panels/InventoryPanel.vue'
import AdminPanel from '../../components/Panels/AdminPanel.vue'
import { useUserStore } from '@/stores/userStore'
import type { User } from '@/components/Models/User'
import { computed, watch } from 'vue'
import type { Item } from '@/components/Models/Item'
import router from '@/router'
import { useI18n } from 'vue-i18n'
import { Icon } from '@iconify/vue'

const { t } = useI18n()
const store = useUserStore()
const loggedUser = computed(() => store.loggedUser)

const welcomeTitle = computed(() => {
  if (!loggedUser.value) return ''
  const role = loggedUser.value.role?.trim()
  if (role === 'userMaster') return t('bienvenido_master')
  if (role === 'userStaff') return t('bienvenido_staff')
  return t('bienvenido')
})

const welcomeSubtitle = computed(() => {
  if (!loggedUser.value) return ''
  const role = loggedUser.value.role?.trim()
  if (role === 'userMaster') return t('listo_master')
  if (role === 'userStaff') return t('listo_staff')
  return t('listo_entrenar')
})

const selectedItem = defineModel<Item | null>('selectedItem')
const selectedUser = defineModel<User | null>('selectedUser')

watch(
  () => store.loggedUser,
  (newUser) => {
    if (!newUser || !newUser.email) {
      router.push({ name: 'home' })
    }
  },
  { immediate: true }
)

function scrollToTop() {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
</script>

<template>
  <div class="main-wrapper">
    <v-container fluid class="content-container">
        <div class="welcome-banner" v-if="loggedUser">
          <div ></div>
          <div class="banner-content">
            <div class="welcome-icon"><v-icon>mdi-arm-flex</v-icon></div>
            <div class="welcome-text">
              <h1 class="welcome-title">{{ welcomeTitle }}, <span class="username">{{
                loggedUser.name }}</span></h1>
              <p class="welcome-subtitle">{{ welcomeSubtitle }}</p>
            </div>
          </div>
          <div class="banner-decoration">
            <div class="decoration-line"></div>
            <div class="decoration-dot"></div>
            <div class="decoration-line"></div>
          </div>
        </div>

        <section class="panel-section" v-if="loggedUser?.role !== 'userMaster'">
          <div class="section-header">
            <Icon icon="mdi:chart-bar" class="section-icon text-primary" width="32" height="32" />
            <h2 class="section-title">{{ $t('estadisticas') }}</h2>
            <div class="section-line"></div>
          </div>
          <div class="panel-wrapper">
            <UserStatsPanel />
          </div>
        </section>

        <section class="panel-section" v-if="loggedUser?.role !== 'userMaster'">
          <div class="section-header">
            <v-icon class="section-icon" color="white">mdi-bag-personal</v-icon>
            <h2 class="section-title">{{ $t('inventario') }}</h2>
            <div class="section-line"></div>
          </div>
          <div class="panel-wrapper">
            <InventoryPanel />
          </div>
        </section>

        <section class="panel-section admin-section" v-if="loggedUser?.role === 'userMaster'">
          <div class="section-header">
            <v-icon class="section-icon admin-icon" color="white">mdi-cog</v-icon>
            <h2 class="section-title admin-title">{{ $t('panel_admin') }}</h2>
            <div class="section-line admin-line"></div>
          </div>
          <div class="panel-wrapper admin-wrapper">
            <AdminPanel v-model:selectedItem="selectedItem" v-model:selectedUser="selectedUser" />
          </div>
          
        </section>
    </v-container>
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Main Wrapper */
.main-wrapper {
  position: relative;
  width: 100%;
  background: transparent;
  overflow-x: hidden;
  overflow-y: auto;
}


/* Particles */
.particles {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 1;
  pointer-events: none;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(13, 110, 253, 0.6);
  border-radius: 50%;

  box-shadow: 0 0 10px rgba(13, 110, 253, 0.8);
}

/* Content Container */
.content-container {
  width: 100% !important;
  max-width: 100% !important;
  padding: 2rem 1rem;
}

/* Welcome Banner */
.welcome-banner {
  position: relative;
  background: linear-gradient(135deg, rgba(13, 110, 253, 0.1) 0%, rgba(13, 110, 253, 0.05) 100%);
  border: 2px solid rgba(13, 110, 253, 0.3);
  border-radius: 24px;
  padding: 2.5rem;
  margin-bottom: 3rem;
  overflow: hidden;
  backdrop-filter: blur(10px);
  box-shadow:
    0 8px 32px rgba(13, 110, 253, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.banner-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.welcome-icon {
  font-size: 4rem;
  filter: drop-shadow(0 4px 12px rgba(13, 110, 253, 0.5));

}

.welcome-text {
  flex: 1;
}

.welcome-title {
  font-size: clamp(1.75rem, 4vw, 2.5rem);
  font-weight: 800;
  color: var(--text-primary);
  margin: 0;
  line-height: 1.2;
  text-shadow: 0 2px 20px rgba(13, 110, 253, 0.5);
}

.username {
  background: linear-gradient(135deg, #0D6EFD, #0dcaf0);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 900;
}

.welcome-subtitle {
  font-size: 1.125rem;
  color: var(--text-secondary);
  margin: 0.5rem 0 0 0;
  font-weight: 500;
}

.banner-decoration {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  opacity: 0.5;
}

.decoration-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, transparent, #0D6EFD, transparent);
}

.decoration-dot {
  width: 8px;
  height: 8px;
  background: #0D6EFD;
  border-radius: 50%;
  box-shadow: 0 0 10px #0D6EFD;
}

/* Panel Sections */
.panel-section {
  margin-bottom: 3rem;

}

.panel-section:nth-child(2) {
  animation-delay: 0.1s;
}

.panel-section:nth-child(3) {
  animation-delay: 0.2s;
}

.panel-section:nth-child(4) {
  animation-delay: 0.3s;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.section-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 8px rgba(13, 110, 253, 0.5));
}

.section-title {
  font-size: clamp(1.5rem, 3vw, 2rem);
  font-weight: 800;
  color: var(--text-primary);
  margin: 0;
  text-shadow: 0 2px 10px rgba(13, 110, 253, 0.3);
}

.section-line {
  flex: 1;
  height: 3px;
  background: linear-gradient(90deg, #0D6EFD, transparent);
  border-radius: 2px;
  margin-left: 1rem;
}

/* Admin Section Specific */
.admin-section .section-icon {
  filter: drop-shadow(0 2px 8px rgba(220, 53, 69, 0.5));
}

.admin-title {
  background: linear-gradient(135deg, #dc3545, #c82333);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.admin-line {
  background: linear-gradient(90deg, #dc3545, transparent);
}

.admin-wrapper {
  border-color: rgba(220, 53, 69, 0.3);
}

/* Panel Wrapper */
.panel-wrapper {
  position: relative;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 0.5rem;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.2);
}

/* Floating Action Button */
.fab-container {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  z-index: 1000;
}

.fab {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: linear-gradient(135deg, #0D6EFD, #0a58ca);
  border: none;
  box-shadow:
    0 4px 12px rgba(13, 110, 253, 0.4),
    0 0 0 0 rgba(13, 110, 253, 0.5);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;

}

.fab:active {
  transform: translateY(-2px) scale(1.05);
}

.fab-icon {
  font-size: 1.5rem;
  color: white;
  font-weight: bold;
}



/* Responsive */
@media (max-width: 768px) {
  .content-container {
    padding: 1rem 0.5rem;
  }

  .welcome-banner {
    padding: 1.5rem;
    margin-bottom: 2rem;
  }

  .banner-content {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .welcome-icon {
    font-size: 3rem;
  }

  .section-header {
    flex-wrap: wrap;
  }

  .section-line {
    width: 100%;
    margin-left: 0;
    margin-top: 0.5rem;
  }

  .fab-container {
    bottom: 1rem;
    right: 1rem;
  }

  .fab {
    width: 48px;
    height: 48px;
  }

  .gradient-orb {
    filter: blur(80px);
  }
}

@media (max-width: 480px) {
  .welcome-title {
    font-size: 1.5rem;
  }

  .welcome-subtitle {
    font-size: 0.875rem;
  }

  .section-title {
    font-size: 1.25rem;
  }

  .section-icon {
    font-size: 1.5rem;
  }
}

/* Accessibility */
@media (prefers-reduced-motion: reduce) {

  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
</style>
