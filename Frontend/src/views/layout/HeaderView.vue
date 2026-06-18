<script setup lang="ts">
import { useUserStore } from '@/stores/userStore'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'
import { computed, ref, onMounted } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
import logoImg from '@/assets/imgs/Logo.png'

const store = useUserStore()
const subscriptionStore = useSubscriptionStore()
const router = useRouter()

const isLogged = computed(() => !!store.loggedUser?.email)
const hasActiveSubscription = computed(() => subscriptionStore.hasActiveSubscription)
const { locale, t } = useI18n()

onMounted(async () => {
  if (isLogged.value) {
    await subscriptionStore.checkSubscription()
  }
})

const mobileMenuOpen = ref(false)
const userDropdownOpen = ref(false)

// Toggle del menú móvil y dropdown de usuario, se asegura que solo uno esté abierto a la vez para evitar solapamientos y problemas de usabilidad.
const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
  userDropdownOpen.value = false
}
// Toggle del dropdown de usuario, se asegura que solo uno esté abierto a la vez para evitar solapamientos y problemas de usabilidad.
const toggleUserDropdown = () => {
  userDropdownOpen.value = !userDropdownOpen.value
  mobileMenuOpen.value = false
}

const handleLogout = () => {
  store.logoutUser()
  router.push({ name: 'home' })
  userDropdownOpen.value = false
  mobileMenuOpen.value = false
}

const handleNavClick = () => {
  mobileMenuOpen.value = false
}

const languages = computed(() => [
  { code: 'en', label: t('english') + ' 🇬🇧' },
  { code: 'es', label: t('spanish_language') + ' 🇪🇸' },
  { code: 'fr', label: t('français') + ' 🇫🇷' }
])

const currentLanguage = computed(() => {
  return languages.value.find(l => l.code === locale.value)?.label || t('language')
})
</script>

<template>
  <div class="header-wrapper">
    <header class="header-main">
      <div class="header-container">

        <RouterLink to="/" class="logo-section">
          <div class="logo-badge"><img :src="logoImg" alt="TheTrainingHub Logo" class="logo-image" /></div>
          <div class="brand">
            <span class="brand-name">TheTrainingHub</span>
            <span class="brand-tagline">Training Platform</span>
          </div>
        </RouterLink>

        <nav class="nav-desktop">
          <RouterLink to="/" class="nav-btn" :title="$t('home')"><v-icon>mdi-home</v-icon> {{ $t('home') }}</RouterLink>
          <RouterLink to="/room" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('rooms')"><v-icon>mdi-door</v-icon> {{ $t('rooms') }}</RouterLink>
          <RouterLink to="/plan" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('plans')"><v-icon>mdi-clipboard-text</v-icon> {{ $t('plans') }}</RouterLink>
          <RouterLink to="/rutina" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('routines')"><v-icon>mdi-calendar</v-icon> {{ $t('routines') }}</RouterLink>
          <RouterLink to="/purchase" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('shop_header')"><v-icon>mdi-cart</v-icon> {{ $t('shop_header') }}</RouterLink>
          <RouterLink to="/achievements" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('achievements_title')"><v-icon>mdi-trophy</v-icon> {{ $t('achievements_title') }}</RouterLink>
          <RouterLink to="/exercises" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('exercises_title')"><v-icon>mdi-arm-flex</v-icon> {{ $t('exercises_title') }}</RouterLink>
          <RouterLink to="/body-metrics" class="nav-btn" :class="{ disabled: !isLogged }" :title="$t('body_metrics_title')"><v-icon>mdi-chart-bar</v-icon> {{ $t('body_metrics_title') }}</RouterLink>

          <RouterLink v-if="isLogged && hasActiveSubscription" to="/CoachAi" class="nav-btn premium-btn" :title="$t('coach_ai')">
            <v-icon>mdi-robot-outline</v-icon> {{ $t('coach_ai') }}
          </RouterLink>
          <RouterLink v-if="isLogged && hasActiveSubscription" to="/calculator" class="nav-btn premium-btn" :title="$t('calculator')">
            <v-icon>mdi-calculator</v-icon> {{ $t('calculator') }}
          </RouterLink>
        </nav>

        <div class="header-actions">

          <div v-if="isLogged" class="user-dropdown-trigger" @click="toggleUserDropdown">
            <div class="user-avatar">
              {{ store.loggedUser?.email?.charAt(0).toUpperCase() }}
            </div>
            <span class="strength-badge"><v-icon>mdi-lightning-bolt</v-icon> {{ store.loggedUser?.strength || 0 }}</span>
            <span class="dropdown-arrow" :class="{ open: userDropdownOpen }">▼</span>
          </div>

          <button class="mobile-menu-btn" @click="toggleMobileMenu" aria-label="Menu">
            <span :class="{ active: mobileMenuOpen }"></span>
            <span :class="{ active: mobileMenuOpen }"></span>
            <span :class="{ active: mobileMenuOpen }"></span>
          </button>
        </div>
      </div>
    </header>

    <Transition name="fade">
      <div v-if="userDropdownOpen && isLogged" class="user-dropdown" @click.self="userDropdownOpen = false">
        <div class="dropdown-content">

          <div class="dropdown-header">
            <div class="user-avatar-large">
              {{ store.loggedUser?.email?.charAt(0).toUpperCase() }}
            </div>
            <div>
              <p class="user-level">{{ $t('header.level') }} {{ store.loggedUser?.level || 1 }}</p>
              <p class="user-email">{{ store.loggedUser?.email }}</p>
            </div>
          </div>


          <div v-if="hasActiveSubscription" class="premium-card premium-active">
            <span class="premium-icon"><v-icon>mdi-check-circle</v-icon></span>
            <div>
              <p class="premium-title">{{ $t('premium_active') }}</p>
              <p class="premium-desc">{{ $t('premium_unlocked_desc') }}</p>
            </div>
          </div>
          <div v-else class="premium-card">
            <span class="premium-icon"><v-icon>mdi-star</v-icon></span>
            <div>
              <p class="premium-title">{{ $t('premium_unlock') }}</p>
              <p class="premium-desc">{{ $t('premium_desc') }}</p>
            </div>
            <RouterLink to="/plan" class="premium-cta-btn" @click="userDropdownOpen = false">
              {{ $t('see_plans') }}
            </RouterLink>
          </div>

          <div class="dropdown-divider"></div>

          <RouterLink to="/profile" class="dropdown-item" @click="userDropdownOpen = false">
            <v-icon>mdi-cog</v-icon> {{ $t('profile_settings') }}
          </RouterLink>

          <div class="dropdown-item language-selector">
            <span><v-icon>mdi-earth</v-icon></span>
            <div class="lang-select-wrapper">
              <select v-model="locale" @change="handleNavClick">
                <option v-for="lang in languages" :key="lang.code" :value="lang.code">
                  {{ lang.label }}
                </option>
              </select>
            </div>
          </div>

          <RouterLink to="/user" class="dropdown-item" @click="userDropdownOpen = false">
            <v-icon>mdi-account-group</v-icon> {{ $t('community') }}
          </RouterLink>

          <button class="logout-btn" @click="handleLogout">
            <v-icon>mdi-lightning-bolt</v-icon> {{ $t('logout') }}
          </button>
        </div>
      </div>
    </Transition>

    <nav v-show="mobileMenuOpen" class="nav-mobile">

      <div v-if="isLogged && store.loggedUser" class="mobile-user-summary">
        <div>
          <p class="mobile-level">LV {{ store.loggedUser.level || 1 }}</p>
          <p class="mobile-stats"><v-icon>mdi-lightning-bolt</v-icon> {{ store.loggedUser.strength || 0 }} • <v-icon>mdi-gold</v-icon> {{ store.loggedUser.gold || 0 }}</p>
        </div>
      </div>

      <RouterLink to="/" class="mobile-nav-item" @click="handleNavClick"><v-icon>mdi-home</v-icon> {{ $t('home') }}</RouterLink>
      <RouterLink to="/room" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-door</v-icon> {{ $t('rooms') }}
      </RouterLink>
      <RouterLink to="/plan" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-clipboard-text</v-icon> {{ $t('plans') }}
      </RouterLink>
      <RouterLink to="/rutina" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-calendar</v-icon>
        {{ $t('routines') }}</RouterLink>
      <RouterLink to="/purchase" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-cart</v-icon>
        {{ $t('shop_header') }}</RouterLink>
      <RouterLink to="/achievements" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-trophy</v-icon>
        {{ $t('achievements_title') }}</RouterLink>
      <RouterLink to="/exercises" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-arm-flex</v-icon>
        {{ $t('exercises_title') }}</RouterLink>
      <RouterLink to="/body-metrics" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-chart-bar</v-icon>
        {{ $t('body_metrics_title') }}</RouterLink>

      <template v-if="isLogged && hasActiveSubscription">
        <RouterLink to="/CoachAi" class="mobile-nav-item premium-item" @click="handleNavClick"><v-icon>mdi-robot-outline</v-icon> {{ $t('coach_ai') }}</RouterLink>
        <RouterLink to="/calculator" class="mobile-nav-item premium-item" @click="handleNavClick"><v-icon>mdi-calculator</v-icon> {{ $t('calculator') }}
        </RouterLink>
      </template>

      <div class="mobile-divider"></div>

      <RouterLink to="/profile" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-cog</v-icon>
        {{ $t('profile_settings') }}</RouterLink>
      <RouterLink to="/user" class="mobile-nav-item" :class="{ disabled: !isLogged }" @click="handleNavClick"><v-icon>mdi-account-group</v-icon>
        {{ $t('community') }}</RouterLink>

      <div class="mobile-language">
        <span><v-icon>mdi-earth</v-icon> {{ $t('language') || 'Language' }}</span>
        <div class="lang-select-wrapper">
          <select v-model="locale" @change="handleNavClick">
            <option v-for="lang in languages" :key="lang.code" :value="lang.code">
              {{ lang.label }}
            </option>
          </select>
        </div>
      </div>

      <div v-if="hasActiveSubscription" class="mobile-premium-card premium-active">
        <span class="mpc-icon"><v-icon>mdi-check-circle</v-icon></span>
        <p>{{ $t('premium_active') }}</p>
        <p class="mpc-desc">{{ $t('premium_unlocked_desc') }}</p>
      </div>
      <div v-else class="mobile-premium-card">
        <span class="mpc-icon"><v-icon>mdi-star</v-icon></span>
        <p>{{ $t('premium_desc') }}</p>
        <RouterLink to="/plan" class="premium-cta-btn" @click="handleNavClick">
          {{ $t('see_plans') }}
        </RouterLink>
      </div>

      <button class="mobile-logout-btn" @click="handleLogout"><v-icon>mdi-lightning-bolt</v-icon> {{ $t('logout') }}</button>
         
    </nav>

            <div v-if="userDropdownOpen && !mobileMenuOpen" class="overlay" @click="userDropdownOpen = false"></div>
     
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.header-wrapper {
  position: relative;
}

/* ═══════════════════════════════════════
   MAIN HEADER
═══════════════════════════════════════ */
.header-main {
  position: sticky;
  top: 0;
  z-index: 1000;
  background: linear-gradient(180deg, rgba(0, 0, 0, 0.97) 0%, rgba(10, 10, 10, 0.93) 100%);
  backdrop-filter: blur(12px);
  border-bottom: 2px solid rgba(255, 204, 0, 0.3);
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.8);
}

.header-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.4rem 1.5rem;
  max-width: 1920px;
  margin: 0 auto;
  gap: 1rem;
  min-height: 48px;
}

/* ═══════════════════════════════════════
   LOGO
═══════════════════════════════════════ */
.logo-section {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  text-decoration: none;
  flex-shrink: 0;
  transition: transform 0.3s ease;
}

.logo-badge {
  width: 34px;
  height: 34px;
  border-radius: 9px;
  background: linear-gradient(135deg, #ffcc00 0%, #ff9900 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  box-shadow: 0 4px 18px rgba(255, 204, 0, 0.45);
  flex-shrink: 0;
  overflow: hidden;
}

.logo-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.brand {
  display: flex;
  flex-direction: column;
  gap: 0.05rem;
}

.brand-name {
  font-size: 1.1rem;
  font-weight: 900;
  color: #ffcc00;
  letter-spacing: -0.5px;
  text-shadow: 0 2px 10px rgba(255, 204, 0, 0.5);
  white-space: nowrap;
}

.brand-tagline {
  font-size: 0.6rem;
  color: rgba(255, 255, 255, 0.55);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  white-space: nowrap;
}

/* ═══════════════════════════════════════
   NAVIGATION (Desktop)
═══════════════════════════════════════ */
.nav-desktop {
  display: flex;
  align-items: center;
  gap: 0.2rem;
  flex: 1;
  justify-content: center;
  min-width: 0;
}

.nav-btn {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.35rem 0.5rem;
  color: rgba(255, 255, 255, 0.78);
  text-decoration: none;
  font-weight: 700;
  font-size: 0.75rem;
  border-radius: 7px;
  transition: all 0.25s;
  white-space: nowrap;
  border: 1px solid transparent;
  cursor: pointer;
  flex-shrink: 1;
  min-width: 0;
}

.nav-btn:hover {
  color: #ffcc00;
  background: rgba(255, 204, 0, 0.09);
  border-color: rgba(255, 204, 0, 0.2);
}

.nav-btn.router-link-exact-active,
.nav-btn.router-link-active:not([href="/"]) {
  color: #ffcc00;
  background: rgba(255, 204, 0, 0.18);
  border-color: rgba(255, 204, 0, 0.45);
  box-shadow: 0 0 20px rgba(255, 204, 0, 0.2), inset 0 0 12px rgba(255, 204, 0, 0.05);
  position: relative;
}

.nav-btn.router-link-exact-active::after,
.nav-btn.router-link-active:not([href="/"])::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 20%;
  right: 20%;
  height: 2px;
  background: linear-gradient(90deg, transparent, #ffcc00, transparent);
  border-radius: 2px;
}

.nav-btn.disabled {
  pointer-events: none;
  opacity: 0.28;
}

/* Premium Navigation Buttons */
.premium-btn {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.15), rgba(99, 102, 241, 0.1));
  border: 1px solid rgba(168, 85, 247, 0.3) !important;
  color: #e9d5ff !important;
  box-shadow: 0 0 10px rgba(168, 85, 247, 0.2);
}

.premium-btn:hover {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.25), rgba(99, 102, 241, 0.15));
  border-color: rgba(168, 85, 247, 0.5) !important;
  box-shadow: 0 0 20px rgba(168, 85, 247, 0.4);
}

.premium-btn.router-link-active {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.3), rgba(99, 102, 241, 0.2));
  border-color: rgba(168, 85, 247, 0.6) !important;
  box-shadow: 0 0 25px rgba(168, 85, 247, 0.5);
}

/* Premium Mobile Items */
.premium-item {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.1), rgba(99, 102, 241, 0.05));
  border-left: 3px solid rgba(168, 85, 247, 0.5);
  color: #e9d5ff !important;
}

.premium-item:hover,
.premium-item.router-link-active {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.2), rgba(99, 102, 241, 0.1));
  border-left-color: #a855f7;
  color: #f3e8ff !important;
}

/* ═══════════════════════════════════════
   HEADER ACTIONS
═══════════════════════════════════════ */
.header-actions {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  flex-shrink: 0;
}

/* User Dropdown Trigger */
.user-dropdown-trigger {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  background: rgba(255, 204, 0, 0.09);
  border: 1px solid rgba(255, 204, 0, 0.28);
  border-radius: 9px;
  padding: 0.3rem 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
}

.user-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ffcc00, #ff9900);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
  color: #000;
  font-size: 0.7rem;
  flex-shrink: 0;
}

.strength-badge {
  color: #ffcc00;
  font-weight: 700;
  font-size: 0.85rem;
}

.dropdown-arrow {
  font-size: 0.6rem;
  color: rgba(255, 255, 255, 0.6);
  transition: transform 0.2s;
}

.dropdown-arrow.open {
  transform: rotate(180deg);
}

/* Mobile Menu Button */
.mobile-menu-btn {
  display: none;
  flex-direction: column;
  gap: 0.3rem;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  flex-shrink: 0;
}

.mobile-menu-btn span {
  width: 22px;
  height: 2.5px;
  background: #ffcc00;
  border-radius: 2px;
  transition: all 0.3s ease;
  display: block;
}

.mobile-menu-btn span.active:nth-child(1) {
  transform: rotate(45deg) translateY(9px);
}

.mobile-menu-btn span.active:nth-child(2) {
  opacity: 0;
}

.mobile-menu-btn span.active:nth-child(3) {
  transform: rotate(-45deg) translateY(-9px);
}

/* ═══════════════════════════════════════
   USER DROPDOWN MENU
═══════════════════════════════════════ */
.user-dropdown {
  position: fixed;
  top: 48px;
  right: 20px;
  z-index: 2000;
  width: 300px;
  max-height: calc(100vh - 68px);
  overflow-y: auto;
}

.dropdown-content {
  background: rgba(5, 5, 5, 0.99);
  border: 1px solid rgba(255, 204, 0, 0.3);
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.8);
  padding: 1rem;
}

.dropdown-header {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  padding-bottom: 0.8rem;
  border-bottom: 1px solid rgba(255, 204, 0, 0.2);
  margin-bottom: 0.8rem;
}

.user-avatar-large {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ffcc00, #ff9900);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 900;
  color: #000;
  font-size: 0.95rem;
  flex-shrink: 0;
}

.user-level {
  margin: 0;
  font-weight: 700;
  color: #ffcc00;
  font-size: 0.9rem;
}

.user-email {
  margin: 0;
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.75rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.6rem;
  margin-bottom: 0.8rem;
  padding-bottom: 0.8rem;
  border-bottom: 1px solid rgba(255, 204, 0, 0.2);
}

.stat-card {
  background: rgba(255, 204, 0, 0.05);
  border: 1px solid rgba(255, 204, 0, 0.15);
  border-radius: 8px;
  padding: 0.6rem;
  text-align: center;
}

.stat-label {
  display: block;
  font-size: 0.65rem;
  color: rgba(255, 255, 255, 0.6);
  text-transform: uppercase;
  letter-spacing: 1px;
  margin-bottom: 0.3rem;
}

.stat-value {
  display: block;
  font-size: 1.1rem;
  font-weight: 900;
  color: #ffcc00;
}

/* Premium Card */
.premium-card {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.15) 0%, rgba(99, 102, 241, 0.1) 100%);
  border: 1px solid rgba(168, 85, 247, 0.2);
  border-radius: 8px;
  padding: 0.7rem;
  margin-bottom: 0.8rem;
  display: flex;
  gap: 0.7rem;
  align-items: flex-start;
  position: relative;
}

.premium-card.premium-active {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15) 0%, rgba(22, 163, 74, 0.1) 100%);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.premium-icon {
  font-size: 1.3rem;
  flex-shrink: 0;
}

.premium-title {
  margin: 0;
  font-weight: 700;
  color: #fff;
  font-size: 0.85rem;
}

.premium-desc {
  margin: 0;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1.4;
}

/* Premium CTA Button */
.premium-cta-btn {
  display: inline-block;
  margin-top: 0.5rem;
  padding: 0.4rem 0.8rem;
  background: linear-gradient(135deg, #a855f7, #6366f1);
  color: #fff !important;
  font-weight: 700;
  font-size: 0.75rem;
  border-radius: 6px;
  text-decoration: none;
  transition: all 0.2s;
}

.premium-cta-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(168, 85, 247, 0.4);
}

/* Dropdown Divider */
.dropdown-divider {
  height: 1px;
  background: rgba(255, 204, 0, 0.15);
  margin: 0.6rem 0;
}

/* Dropdown Items */
.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0.7rem 0.8rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  border-radius: 6px;
  transition: all 0.2s;
  cursor: pointer;
  font-size: 0.9rem;
  border: none;
  background: none;
  width: 100%;
  text-align: left;
  font-family: inherit;
}

.language-selector {
  justify-content: space-between;
}

.lang-select-wrapper {
  position: relative;
}

.lang-select-wrapper::after {
  content: '';
  position: absolute;
  top: 50%;
  right: 0.5rem;
  width: 0;
  height: 0;
  margin-top: -2px;
  border-left: 4px solid transparent;
  border-right: 4px solid transparent;
  border-top: 4px solid #ffcc00;
  pointer-events: none;
}

.language-selector select {
  background: rgba(30, 30, 30, 0.95);
  border: 1px solid rgba(255, 204, 0, 0.4);
  color: #ffffff;
  padding: 0.35rem 1.6rem 0.35rem 0.6rem;
  border-radius: 6px;
  font-size: 0.8rem;
  font-weight: 600;
  cursor: pointer;
  appearance: none;
  min-width: 120px;
}

.language-selector select option {
  background: #1a1a1a;
  color: #ffffff;
}

/* Logout Button */
.logout-btn {
  width: 100%;
  background: linear-gradient(135deg, #ff4444, #cc0000);
  border: none;
  color: #fff;
  font-weight: 700;
  font-size: 0.85rem;
  padding: 0.7rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  margin-top: 0.4rem;
  font-family: inherit;
}

.logout-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 18px rgba(255, 68, 68, 0.5);
}

/* ═══════════════════════════════════════
   MOBILE MENU
═══════════════════════════════════════ */
.nav-mobile {
  display: none;
  position: fixed;
  top: 48px;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(5, 5, 5, 0.99);
  border-top: 1px solid rgba(255, 204, 0, 0.25);
  padding: 0.5rem 0 1.5rem;
  overflow-y: auto;
  z-index: 1001;
}

/* Asegurar que los enlaces del menú móvil sean clicables */
.nav-mobile a {
  position: relative;
  z-index: 1002;
}

.mobile-user-summary {
  padding: 1rem 1.5rem;
  background: rgba(255, 204, 0, 0.08);
  border-bottom: 1px solid rgba(255, 204, 0, 0.18);
  margin-bottom: 0.5rem;
}

.mobile-level {
  margin: 0;
  font-weight: 900;
  color: #ffcc00;
  font-size: 1rem;
}

.mobile-stats {
  margin: 0;
  color: rgba(255, 255, 255, 0.85);
  font-weight: bold;
  font-size: 0.9rem;
}

.mobile-nav-item {
  display: block;
  padding: 0.85rem 1.5rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.2s;
  border-left: 3px solid transparent;
}

.mobile-nav-item:hover {
  color: #ffcc00;
  background: rgba(255, 204, 0, 0.06);
  border-left-color: rgba(255, 204, 0, 0.4);
}

.mobile-nav-item.router-link-exact-active,
.mobile-nav-item.router-link-active:not([href="/"]) {
  color: #ffcc00;
  background: rgba(255, 204, 0, 0.12);
  border-left: 3px solid #ffcc00;
  box-shadow: inset 4px 0 12px rgba(255, 204, 0, 0.08);
  padding-left: 1.75rem;
}

.mobile-nav-item.disabled {
  pointer-events: none;
  opacity: 0.25;
}

.mobile-divider {
  height: 1px;
  background: rgba(255, 204, 0, 0.15);
  margin: 0.5rem 0;
}

.mobile-language {
  padding: 0.85rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.9);
}

.mobile-language .lang-select-wrapper::after {
  right: 0.6rem;
  border-top: 5px solid #ffcc00;
}

.mobile-language select {
  background: rgba(30, 30, 30, 0.95);
  border: 1px solid rgba(255, 204, 0, 0.4);
  color: #ffffff;
  padding: 0.45rem 1.8rem 0.45rem 0.7rem;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  appearance: none;
  min-width: 130px;
}

.mobile-language select option {
  background: #1a1a1a;
  color: #ffffff;
}

.mobile-premium-card {
  margin: 1rem 1.5rem;
  padding: 0.8rem;
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.15) 0%, rgba(99, 102, 241, 0.1) 100%);
  border: 1px solid rgba(168, 85, 247, 0.2);
  border-radius: 8px;
  text-align: center;
}

.mobile-premium-card.premium-active {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15) 0%, rgba(22, 163, 74, 0.1) 100%);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.mobile-premium-card p {
  margin: 0;
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.9rem;
  font-weight: 600;
}

.mobile-premium-card .mpc-icon {
  font-size: 1.2rem;
  display: block;
  margin-bottom: 0.3rem;
}

.mobile-premium-card .mpc-desc {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.65);
  margin-top: 0.3rem;
}

.mobile-logout-btn {
  display: block;
  width: calc(100% - 3rem);
  margin: 0.5rem 1.5rem;
  background: linear-gradient(135deg, #ff4444, #cc0000);
  border: none;
  color: #fff;
  font-weight: 700;
  font-size: 0.85rem;
  padding: 0.65rem;
  border-radius: 8px;
  cursor: pointer;
  font-family: inherit;
}

/* ═══════════════════════════════════════
   OVERLAY
═══════════════════════════════════════ */
.overlay {
  position: fixed;
  top: 48px;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1000;
  background: rgba(0, 0, 0, 0.5);
}

/* ═══════════════════════════════════════
   TRANSITIONS
═══════════════════════════════════════ */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* ═══════════════════════════════════════
   RESPONSIVE
═══════════════════════════════════════ */

/* Desktop compact: 1500px and below */
@media (max-width: 1500px) {
  .brand-tagline {
    display: none;
  }

  .nav-btn {
    padding: 0.25rem 0.3rem;
    gap: 0.15rem;
    font-size: 0.65rem;
    max-width: none;
    overflow: visible;
  }

  .nav-btn :deep(.v-icon) {
    font-size: 1rem;
  }

  .strength-badge {
    display: none;
  }

  .user-dropdown-trigger {
    padding: 0.3rem 0.5rem;
  }
}

/* Tablet: 1200px and below */
@media (max-width: 1200px) {
  .nav-btn {
    padding: 0.2rem;
    font-size: 0.6rem;
    max-width: none;
  }

  .nav-btn :deep(.v-icon) {
    font-size: 0.9rem;
  }
}

/* Tablet: 1024px and below */
@media (max-width: 1024px) {
  .nav-desktop {
    gap: 0;
  }

  .nav-btn {
    padding: 0.2rem;
    font-size: 0.55rem;
    max-width: none;
  }

  .nav-btn :deep(.v-icon) {
    font-size: 0.8rem;
  }

  .brand-name {
    font-size: 0.95rem;
  }

  .logo-badge {
    width: 32px;
    height: 32px;
    font-size: 1rem;
  }

  .user-dropdown {
    width: 280px;
  }
}

/* Mobile: 900px and below */
@media (max-width: 900px) {
  .nav-desktop {
    display: none;
  }

  .mobile-menu-btn {
    display: flex;
  }

  .nav-mobile {
    display: flex;
    flex-direction: column;
  }

  .header-container {
    padding: 0.5rem 1rem;
    min-height: 52px;
  }

  .user-dropdown {
    right: 15px;
    width: 280px;
    top: 52px;
  }

  .nav-mobile {
    top: 52px;
  }

  .overlay {
    top: 52px;
  }

  .strength-badge {
    display: none;
  }

  .user-avatar {
    width: 26px;
    height: 26px;
    font-size: 0.7rem;
  }

  .user-dropdown-trigger {
    padding: 0.3rem 0.5rem;
  }
}

/* Small Mobile: 480px and below */
@media (max-width: 480px) {
  .brand-name {
    display: none;
  }

  .header-container {
    padding: 0.6rem 0.75rem;
  }

  .user-dropdown {
    width: calc(100vw - 30px);
    max-width: 300px;
  }
}

/* Extra Small: 360px and below */
@media (max-width: 360px) {
  .logo-badge {
    width: 32px;
    height: 32px;
    font-size: 0.95rem;
  }

  .header-container {
    padding: 0.5rem 0.5rem;
  }
}
</style>