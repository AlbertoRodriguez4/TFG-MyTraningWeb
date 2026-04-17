<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useUserStore } from '@/stores/userStore'
import { useItemStore } from '@/stores/itemStore'
import EditUserPopup from '../PopUps/EditUserPopup.vue'
import EditItemPopup from '../PopUps/EditItemPopup.vue'
import type { User } from '@/components/Models/User'
import type { Item } from '@/components/Models/Item'
import router from '@/router'
import { useI18n } from 'vue-i18n'

const store = useUserStore()
const loggedUser = computed(() => store.loggedUser)
const itemStore = useItemStore()
const selectedUser = defineModel<User | null>('selectedUser')
const selectedItem = defineModel<Item | null>('selectedItem')
const showPopup = ref(false)
const userSearch = ref('')
const itemSearch = ref('')
const { t, locale } = useI18n()
locale.value = 'es'

// Stats computadas para el dashboard
const totalUsers = computed(() => store.user.length)
const totalItems = computed(() => itemStore.items.length)
const activeUsers = computed(() => store.user.filter(u => u.role !== 'userMaster').length)

onMounted(async () => {
    if (loggedUser.value?.email && loggedUser.value.passwordhash) {
        await store.getItems()
    }
    if (loggedUser.value?.role === 'userMaster') {
        await store.fetchUser()
        await itemStore.fetchItems()
    }
})

watch(
    () => store.loggedUser,
    (newUser) => {
        if (!newUser || !newUser.email) {
            router.push({ name: 'home' })
        }
    },
    { immediate: true }
)

const openPopup = (item: Item) => {
    selectedItem.value = item
    showPopup.value = true
}

const closePopup = () => {
    showPopup.value = false
    selectedItem.value = null
}

const openUserPopup = (user: User) => {
    selectedUser.value = user
    showPopup.value = true
}

const closeUserPopup = () => {
    showPopup.value = false
    selectedUser.value = null
}

const filteredUsers = computed(() =>
    store.user.filter(user => user.name.toLowerCase().includes(userSearch.value.toLowerCase()))
)

const filteredItems = computed(() =>
    itemStore.items.filter(item => item.name.toLowerCase().includes(itemSearch.value.toLowerCase()))
)
</script>

<template>
  <v-container fluid class="admin-panel">
    <!-- Particles background -->
    <div class="particles-bg">
      <div class="particle" v-for="i in 30" :key="i" :style="{ 
        left: Math.random() * 100 + '%', 
        top: Math.random() * 100 + '%',
        animationDelay: Math.random() * 10 + 's',
        animationDuration: (8 + Math.random() * 12) + 's'
      }"></div>
    </div>

    <!-- Hero Header -->
    <div class="admin-hero">
      <div class="hero-glow"></div>
      
      <!-- Crown Icon -->
      <div class="crown-container">
        <div class="crown-rings">
          <div class="crown-ring ring-1"></div>
          <div class="crown-ring ring-2"></div>
          <div class="crown-ring ring-3"></div>
        </div>
        <v-icon class="crown-icon">mdi-crown</v-icon>
      </div>

      <!-- Title Section -->
      <div class="hero-title-section">
        <h1 class="hero-title">
          <span class="title-top">CONTROL CENTER</span>
          <span class="title-main">{{ loggedUser?.name }}</span>
        </h1>
        <div class="admin-badge">
          <v-icon size="20">mdi-shield-crown</v-icon>
          <span>MASTER ADMIN</span>
        </div>
      </div>

      <!-- Divider -->
      <div class="hero-divider">
        <div class="divider-line"></div>
        <v-icon class="divider-icon">mdi-lightning-bolt</v-icon>
        <div class="divider-line"></div>
      </div>

      <!-- Stats Dashboard -->
      <div class="stats-dashboard">
        <div class="stat-mega-card stat-users">
          <div class="stat-bg-pattern"></div>
          <div class="stat-icon-wrapper">
            <v-icon class="stat-mega-icon">mdi-account-multiple</v-icon>
          </div>
          <div class="stat-content">
            <div class="stat-mega-value">{{ totalUsers }}</div>
            <div class="stat-mega-label">{{ $t('USUARIOS TOTALES') }}</div>
          </div>
          <div class="stat-shine"></div>
        </div>

        <div class="stat-mega-card stat-items">
          <div class="stat-bg-pattern"></div>
          <div class="stat-icon-wrapper">
            <v-icon class="stat-mega-icon">mdi-gift</v-icon>
          </div>
          <div class="stat-content">
            <div class="stat-mega-value">{{ totalItems }}</div>
            <div class="stat-mega-label">{{ $t('ITEMS DISPONIBLES') }}</div>
          </div>
          <div class="stat-shine"></div>
        </div>

        <div class="stat-mega-card stat-active">
          <div class="stat-bg-pattern"></div>
          <div class="stat-icon-wrapper">
            <v-icon class="stat-mega-icon">mdi-account-check</v-icon>
          </div>
          <div class="stat-content">
            <div class="stat-mega-value">{{ activeUsers }}</div>
            <div class="stat-mega-label">{{ $t('USUARIOS ACTIVOS') }}</div>
          </div>
          <div class="stat-shine"></div>
        </div>
      </div>
    </div>

    <!-- Management Section -->
    <v-row class="management-section">
      <!-- USERS PANEL -->
      <v-col cols="12" lg="6">
        <div class="panel-card panel-users">
          <div class="panel-header">
            <div class="panel-header-content">
              <div class="panel-icon-badge">
                <v-icon size="28" color="white">mdi-account-group</v-icon>
              </div>
              <div class="panel-title-group">
                <h2 class="panel-title">{{ $t('GESTIÓN DE USUARIOS') }}</h2>
                <p class="panel-subtitle">Administra todos los atletas registrados</p>
              </div>
            </div>
            <div class="panel-counter">
              <span class="counter-number">{{ filteredUsers.length }}</span>
            </div>
          </div>

          <div class="panel-body">
            <!-- Search Bar -->
            <div class="search-container">
              <v-text-field
                v-model="userSearch"
                variant="solo"
                density="comfortable"
                :placeholder="$t('Buscar usuario...')"
                class="search-input"
                hide-details
                clearable
              >
                <template v-slot:prepend-inner>
                  <v-icon color="#00D2FF">mdi-magnify</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Users List -->
            <div class="entity-list">
              <div 
                v-for="(user, index) in filteredUsers" 
                :key="user.id"
                class="entity-item user-item"
                :style="{ animationDelay: (index * 0.05) + 's' }"
              >
                <div class="entity-avatar">
                  <div class="avatar-ring ring-cyan"></div>
                  <span class="avatar-letter">{{ user.name.charAt(0).toUpperCase() }}</span>
                </div>
                
                <div class="entity-info">
                  <div class="entity-name">{{ user.name }}</div>
                  <div class="entity-meta">
                    <v-icon size="14">mdi-email</v-icon>
                    <span>{{ user.email }}</span>
                  </div>
                </div>

                <v-btn
                  class="action-btn btn-edit"
                  size="small"
                  @click="openUserPopup(user)"
                >
                  <v-icon size="18">mdi-pencil</v-icon>
                  <span>EDITAR</span>
                </v-btn>
              </div>

              <!-- Empty State -->
              <div v-if="filteredUsers.length === 0" class="empty-state">
                <v-icon size="80" color="rgba(255,255,255,0.1)">mdi-account-off-outline</v-icon>
                <p>No se encontraron usuarios</p>
              </div>
            </div>
          </div>
        </div>
      </v-col>

      <!-- ITEMS PANEL -->
      <v-col cols="12" lg="6">
        <div class="panel-card panel-items">
          <div class="panel-header">
            <div class="panel-header-content">
              <div class="panel-icon-badge">
                <v-icon size="28" color="white">mdi-gift-open</v-icon>
              </div>
              <div class="panel-title-group">
                <h2 class="panel-title">{{ $t('GESTIÓN DE ITEMS') }}</h2>
                <p class="panel-subtitle">Controla recompensas y objetos</p>
              </div>
            </div>
            <div class="panel-counter">
              <span class="counter-number">{{ filteredItems.length }}</span>
            </div>
          </div>

          <div class="panel-body">
            <!-- Search Bar -->
            <div class="search-container">
              <v-text-field
                v-model="itemSearch"
                variant="solo"
                density="comfortable"
                :placeholder="$t('Buscar ítem...')"
                class="search-input"
                hide-details
                clearable
              >
                <template v-slot:prepend-inner>
                  <v-icon color="#A855F7">mdi-magnify</v-icon>
                </template>
              </v-text-field>
            </div>

            <!-- Items List -->
            <div class="entity-list">
              <div 
                v-for="(item, index) in filteredItems" 
                :key="item.id"
                class="entity-item item-item"
                :style="{ animationDelay: (index * 0.05) + 's' }"
              >
                <div class="entity-avatar">
                  <div class="avatar-ring ring-purple"></div>
                  <v-icon color="#A855F7" size="24">mdi-gift</v-icon>
                </div>
                
                <div class="entity-info">
                  <div class="entity-name">{{ item.name }}</div>
                  <div class="entity-meta">
                    <v-icon size="14">mdi-tag</v-icon>
                    <span>Item #{{ item.id }}</span>
                  </div>
                </div>

                <v-btn
                  class="action-btn btn-edit"
                  size="small"
                  @click="openPopup(item)"
                >
                  <v-icon size="18">mdi-pencil</v-icon>
                  <span>EDITAR</span>
                </v-btn>
              </div>

              <!-- Empty State -->
              <div v-if="filteredItems.length === 0" class="empty-state">
                <v-icon size="80" color="rgba(255,255,255,0.1)">mdi-package-variant-closed-remove</v-icon>
                <p>No se encontraron ítems</p>
              </div>
            </div>
          </div>
        </div>
      </v-col>
    </v-row>

    <!-- Popups -->
    <EditItemPopup
      v-if="selectedItem"
      :visible="showPopup"
      :item="selectedItem"
      @close="closePopup"
      @edit="(item) => console.log('Editando', item)"
    />
    <EditUserPopup
      v-if="selectedUser"
      :visible="showPopup"
      :user="selectedUser"
      @close="closeUserPopup"
      @edit="(user) => console.log('Editando', user)"
    />
  </v-container>
</template>

<style scoped>
/* Main Container */
.admin-panel {
  position: relative;
  min-height: 100vh;
  background: 
    radial-gradient(ellipse at top left, rgba(139, 92, 246, 0.15) 0%, transparent 50%),
    radial-gradient(ellipse at bottom right, rgba(59, 130, 246, 0.15) 0%, transparent 50%),
    linear-gradient(180deg, #0a0e1a 0%, #0f1419 50%, #0a0e1a 100%);
  padding: 3rem 2rem;
  overflow-x: hidden;
}

/* Particles */
.particles-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
  z-index: 0;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(139, 92, 246, 0.4);
  border-radius: 50%;
  animation: particle-float linear infinite;
}

@keyframes particle-float {
  0% {
    transform: translate(0, 0) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translate(100px, -800px) scale(1);
    opacity: 0;
  }
}

/* Hero Section */
.admin-hero {
  position: relative;
  text-align: center;
  margin-bottom: 4rem;
  z-index: 1;
}

.hero-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 1000px;
  height: 1000px;
  background: radial-gradient(circle, rgba(139, 92, 246, 0.15) 0%, transparent 70%);
  pointer-events: none;
}

/* Crown Container */
.crown-container {
  position: relative;
  display: inline-block;
  margin-bottom: 2rem;
}

.crown-rings {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
}

.crown-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border: 3px solid rgba(255, 215, 0, 0.3);
  border-radius: 50%;
  animation: crown-ring-expand 4s ease-out infinite;
}

.ring-1 {
  width: 100px;
  height: 100px;
  animation-delay: 0s;
}

.ring-2 {
  width: 100px;
  height: 100px;
  animation-delay: 1.3s;
}

.ring-3 {
  width: 100px;
  height: 100px;
  animation-delay: 2.6s;
}

@keyframes crown-ring-expand {
  0% {
    width: 100px;
    height: 100px;
    opacity: 1;
  }
  100% {
    width: 250px;
    height: 250px;
    opacity: 0;
  }
}

.crown-icon {
  font-size: 5rem !important;
  color: #FFD700;
  filter: drop-shadow(0 0 40px rgba(255, 215, 0, 0.8));
  animation: crown-pulse 3s ease-in-out infinite;
}

@keyframes crown-pulse {
  0%, 100% {
    transform: scale(1) rotate(-5deg);
    filter: drop-shadow(0 0 40px rgba(255, 215, 0, 0.8));
  }
  50% {
    transform: scale(1.1) rotate(5deg);
    filter: drop-shadow(0 0 60px rgba(255, 215, 0, 1));
  }
}

/* Hero Title */
.hero-title-section {
  margin-bottom: 2rem;
}

.hero-title {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin: 0 0 1rem;
}

.title-top {
  font-size: 1.2rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.5);
  letter-spacing: 8px;
  text-transform: uppercase;
}

.title-main {
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 900;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 50%, #FFD700 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 4px;
  text-transform: uppercase;
  animation: gradient-shift 4s ease infinite;
}

@keyframes gradient-shift {
  0%, 100% {
    background-position: 0% center;
  }
  50% {
    background-position: 100% center;
  }
}

.admin-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 165, 0, 0.2) 100%);
  border: 2px solid rgba(255, 215, 0, 0.4);
  border-radius: 30px;
  padding: 0.75rem 1.5rem;
  color: #FFD700;
  font-weight: 800;
  font-size: 0.9rem;
  letter-spacing: 2px;
  box-shadow: 0 4px 20px rgba(255, 215, 0, 0.3);
}

/* Hero Divider */
.hero-divider {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  max-width: 600px;
  margin: 0 auto 3rem;
}

.divider-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, transparent, rgba(139, 92, 246, 0.5), transparent);
}

.divider-icon {
  color: #8B5CF6;
  font-size: 2rem !important;
  animation: lightning-pulse 2s ease-in-out infinite;
}

@keyframes lightning-pulse {
  0%, 100% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.7;
    transform: scale(1.2);
  }
}

/* Stats Dashboard */
.stats-dashboard {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.stat-mega-card {
  position: relative;
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid;
  border-radius: 24px;
  padding: 2rem;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  cursor: pointer;
  animation: stat-entrance 0.8s ease-out backwards;
}

@keyframes stat-entrance {
  0% {
    opacity: 0;
    transform: translateY(50px) scale(0.9);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.stat-users {
  border-color: #00D2FF;
  box-shadow: 0 10px 40px rgba(0, 210, 255, 0.2);
  animation-delay: 0.1s;
}

.stat-items {
  border-color: #A855F7;
  box-shadow: 0 10px 40px rgba(168, 85, 247, 0.2);
  animation-delay: 0.2s;
}

.stat-active {
  border-color: #4ADE80;
  box-shadow: 0 10px 40px rgba(74, 222, 128, 0.2);
  animation-delay: 0.3s;
}

.stat-mega-card:hover {
  transform: translateY(-10px) scale(1.02);
}

.stat-users:hover {
  box-shadow: 0 20px 60px rgba(0, 210, 255, 0.4);
}

.stat-items:hover {
  box-shadow: 0 20px 60px rgba(168, 85, 247, 0.4);
}

.stat-active:hover {
  box-shadow: 0 20px 60px rgba(74, 222, 128, 0.4);
}

.stat-bg-pattern {
  position: absolute;
  top: -50%;
  right: -20%;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.05), transparent);
  border-radius: 50%;
}

.stat-icon-wrapper {
  margin-bottom: 1.5rem;
}

.stat-mega-icon {
  font-size: 3.5rem !important;
  color: rgba(255, 255, 255, 0.9);
  filter: drop-shadow(0 4px 20px rgba(0, 0, 0, 0.3));
}

.stat-users .stat-mega-icon {
  color: #00D2FF;
}

.stat-items .stat-mega-icon {
  color: #A855F7;
}

.stat-active .stat-mega-icon {
  color: #4ADE80;
}

.stat-content {
  position: relative;
  z-index: 2;
}

.stat-mega-value {
  font-size: 3.5rem;
  font-weight: 900;
  color: white;
  line-height: 1;
  margin-bottom: 0.5rem;
  text-shadow: 0 4px 10px rgba(0, 0, 0, 0.5);
}

.stat-mega-label {
  font-size: 0.85rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.6);
  letter-spacing: 2px;
  text-transform: uppercase;
}

.stat-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.1),
    transparent
  );
  transition: left 0.6s;
}

.stat-mega-card:hover .stat-shine {
  left: 100%;
}

/* Management Section */
.management-section {
  position: relative;
  z-index: 1;
}

/* Panel Card */
.panel-card {
  background: linear-gradient(135deg, rgba(20, 30, 48, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  backdrop-filter: blur(20px);
  border: 2px solid;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.4);
  transition: all 0.3s ease;
  height: 700px;
  display: flex;
  flex-direction: column;
}

.panel-users {
  border-color: rgba(0, 210, 255, 0.3);
}

.panel-items {
  border-color: rgba(168, 85, 247, 0.3);
}

.panel-card:hover {
  box-shadow: 0 25px 70px rgba(0, 0, 0, 0.5);
}

/* Panel Header */
.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 2rem;
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  border-bottom: 2px solid rgba(255, 255, 255, 0.1);
}

.panel-header-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.panel-icon-badge {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #00D2FF 0%, #0891B2 100%);
  box-shadow: 0 8px 25px rgba(0, 210, 255, 0.4);
}

.panel-items .panel-icon-badge {
  background: linear-gradient(135deg, #A855F7 0%, #7C3AED 100%);
  box-shadow: 0 8px 25px rgba(168, 85, 247, 0.4);
}

.panel-title-group {
  flex: 1;
}

.panel-title {
  font-size: 1.5rem;
  font-weight: 900;
  color: white;
  letter-spacing: 1px;
  margin: 0 0 0.25rem;
}

.panel-subtitle {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.5);
  margin: 0;
}

.panel-counter {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: rgba(0, 0, 0, 0.3);
  border: 3px solid rgba(0, 210, 255, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.panel-items .panel-counter {
  border-color: rgba(168, 85, 247, 0.5);
}

.counter-number {
  font-size: 1.5rem;
  font-weight: 900;
  color: white;
}

/* Panel Body */
.panel-body {
  flex: 1;
  padding: 2rem;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Search Container */
.search-container {
  margin-bottom: 1.5rem;
}

:deep(.search-input .v-field) {
  background: rgba(30, 30, 30, 0.6) !important;
  border: 2px solid rgba(255, 255, 255, 0.1) !important;
  border-radius: 16px !important;
  box-shadow: inset 0 2px 8px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
}

:deep(.search-input .v-field:hover) {
  border-color: rgba(0, 210, 255, 0.3) !important;
  background: rgba(35, 35, 35, 0.7) !important;
}

:deep(.search-input .v-field--focused) {
  border-color: rgba(0, 210, 255, 0.6) !important;
  box-shadow: 
    inset 0 2px 8px rgba(0, 0, 0, 0.3),
    0 0 20px rgba(0, 210, 255, 0.2) !important;
}

:deep(.panel-items .search-input .v-field:hover) {
  border-color: rgba(168, 85, 247, 0.3) !important;
}

:deep(.panel-items .search-input .v-field--focused) {
  border-color: rgba(168, 85, 247, 0.6) !important;
  box-shadow: 
    inset 0 2px 8px rgba(0, 0, 0, 0.3),
    0 0 20px rgba(168, 85, 247, 0.2) !important;
}

:deep(.search-input .v-field__input) {
  color: white !important;
  font-size: 1rem !important;
}

/* Entity List */
.entity-list::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 210, 255, 0.6);
}

.panel-items .entity-list::-webkit-scrollbar-thumb:hover {
  background: rgba(168, 85, 247, 0.6);
}

/* Entity Item */
.entity-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: rgba(30, 41, 59, 0.4);
  border: 2px solid rgba(255, 255, 255, 0.05);
  border-radius: 16px;
  padding: 1rem 1.25rem;
  margin-bottom: 1rem;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: item-fade-in 0.5s ease-out backwards;
}

@keyframes item-fade-in {
  0% {
    opacity: 0;
    transform: translateX(-20px);
  }
  100% {
    opacity: 1;
    transform: translateX(0);
  }
}

.entity-item:hover {
  background: rgba(30, 41, 59, 0.7);
  border-color: rgba(0, 210, 255, 0.4);
  transform: translateX(8px);
  box-shadow: 0 4px 20px rgba(0, 210, 255, 0.2);
}

.item-item:hover {
  border-color: rgba(168, 85, 247, 0.4);
  box-shadow: 0 4px 20px rgba(168, 85, 247, 0.2);
}

/* Entity Avatar */
.entity-avatar {
  position: relative;
  width: 50px;
  height: 50px;
  border-radius: 12px;
  background: linear-gradient(135deg, rgba(0, 210, 255, 0.2), rgba(0, 145, 178, 0.2));
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.item-item .entity-avatar {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.2), rgba(124, 58, 237, 0.2));
}

.avatar-ring {
  position: absolute;
  top: -4px;
  left: -4px;
  right: -4px;
  bottom: -4px;
  border: 2px solid;
  border-radius: 14px;
  animation: avatar-pulse 2s ease-in-out infinite;
}

@keyframes avatar-pulse {
  0%, 100% {
    opacity: 0.5;
    transform: scale(1);
  }
  50% {
    opacity: 1;
    transform: scale(1.05);
  }
}

.ring-cyan {
  border-color: #00D2FF;
}

.ring-purple {
  border-color: #A855F7;
}

.avatar-letter {
  font-size: 1.3rem;
  font-weight: 900;
  color: white;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.5);
}

/* Entity Info */
.entity-info {
  flex: 1;
  min-width: 0;
}

.entity-name {
  font-size: 1.1rem;
  font-weight: 800;
  color: white;
  margin-bottom: 0.25rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.entity-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.5);
}

.entity-meta span {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Action Button */
.action-btn {
  background: linear-gradient(135deg, #00D2FF 0%, #0891B2 100%) !important;
  color: white !important;
  text-transform: uppercase;
  font-weight: 800;
  letter-spacing: 1px;
  border-radius: 12px !important;
  padding: 0.5rem 1rem !important;
  box-shadow: 0 4px 15px rgba(0, 210, 255, 0.3);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.item-item .action-btn {
  background: linear-gradient(135deg, #A855F7 0%, #7C3AED 100%) !important;
  box-shadow: 0 4px 15px rgba(168, 85, 247, 0.3);
}

.action-btn:hover {
  transform: translateY(-2px) scale(1.05);
  box-shadow: 0 6px 25px rgba(0, 210, 255, 0.5);
}

.item-item .action-btn:hover {
  box-shadow: 0 6px 25px rgba(168, 85, 247, 0.5);
}

.action-btn span {
  display: none;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
}

.empty-state p {
  color: rgba(255, 255, 255, 0.3);
  font-size: 1.1rem;
  font-weight: 600;
  margin-top: 1rem;
}

/* Responsive Design */
@media (max-width: 1400px) {
  .stats-dashboard {
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
  }
}

@media (max-width: 1200px) {
  .admin-panel {
    padding: 2rem 1.5rem;
  }

  .crown-icon {
    font-size: 4rem !important;
  }

  .title-main {
    font-size: 2.5rem;
  }

  .stats-dashboard {
    gap: 1.25rem;
  }

  .stat-mega-card {
    padding: 1.5rem;
  }

  .stat-mega-value {
    font-size: 3rem;
  }

  .panel-card {
    height: auto;
    min-height: 600px;
  }
}

@media (max-width: 960px) {
  .admin-panel {
    padding: 2rem 1rem;
  }

  .admin-hero {
    margin-bottom: 3rem;
  }

  .crown-icon {
    font-size: 3.5rem !important;
  }

  .title-top {
    font-size: 1rem;
    letter-spacing: 6px;
  }

  .title-main {
    font-size: 2rem;
    letter-spacing: 2px;
  }

  .admin-badge {
    font-size: 0.8rem;
    padding: 0.6rem 1.25rem;
  }

  .stats-dashboard {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .stat-mega-card {
    padding: 1.5rem;
  }

  .stat-mega-icon {
    font-size: 3rem !important;
  }

  .stat-mega-value {
    font-size: 2.5rem;
  }

  .panel-header {
    padding: 1.5rem;
  }

  .panel-icon-badge {
    width: 50px;
    height: 50px;
  }

  .panel-title {
    font-size: 1.25rem;
  }

  .panel-subtitle {
    font-size: 0.8rem;
  }

  .panel-counter {
    width: 50px;
    height: 50px;
  }

  .counter-number {
    font-size: 1.25rem;
  }

  .panel-body {
    padding: 1.5rem;
  }

  .action-btn span {
    display: inline;
  }
}

@media (max-width: 600px) {
  .admin-panel {
    padding: 1.5rem 0.75rem;
  }

  .crown-icon {
    font-size: 3rem !important;
  }

  .title-top {
    font-size: 0.9rem;
    letter-spacing: 4px;
  }

  .title-main {
    font-size: 1.6rem;
  }

  .admin-badge {
    font-size: 0.75rem;
    padding: 0.5rem 1rem;
    gap: 0.4rem;
  }

  .admin-badge .v-icon {
    font-size: 16px !important;
  }

  .hero-divider {
    margin-bottom: 2rem;
  }

  .divider-icon {
    font-size: 1.5rem !important;
  }

  .stat-mega-card {
    padding: 1.25rem;
  }

  .stat-mega-icon {
    font-size: 2.5rem !important;
  }

  .stat-mega-value {
    font-size: 2rem;
  }

  .stat-mega-label {
    font-size: 0.75rem;
  }

  .panel-card {
    min-height: 500px;
  }

  .panel-header {
    flex-direction: column;
    gap: 1rem;
    padding: 1.25rem;
  }

  .panel-header-content {
    width: 100%;
    gap: 1rem;
  }

  .panel-icon-badge {
    width: 45px;
    height: 45px;
  }

  .panel-icon-badge .v-icon {
    font-size: 24px !important;
  }

  .panel-title {
    font-size: 1.1rem;
  }

  .panel-subtitle {
    font-size: 0.75rem;
  }

  .panel-counter {
    width: 45px;
    height: 45px;
    align-self: center;
  }

  .counter-number {
    font-size: 1.1rem;
  }

  .panel-body {
    padding: 1.25rem;
  }

  .entity-item {
    padding: 0.875rem 1rem;
    flex-wrap: wrap;
  }

  .entity-avatar {
    width: 45px;
    height: 45px;
  }

  .avatar-letter {
    font-size: 1.1rem;
  }

  .entity-name {
    font-size: 1rem;
  }

  .entity-meta {
    font-size: 0.8rem;
  }

  .action-btn {
    width: 100%;
    margin-top: 0.5rem;
    justify-content: center;
  }

  .action-btn span {
    display: inline;
  }
}

@media (max-width: 400px) {
  .title-main {
    font-size: 1.4rem;
  }

  .stat-mega-value {
    font-size: 1.75rem;
  }

  .panel-title {
    font-size: 1rem;
  }

  .entity-name {
    font-size: 0.95rem;
  }
}
</style> 