import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useUserStore } from '@/stores/userStore'
import { useSubscriptionStore } from '@/stores/SubscriptionStore'

import HomeView from '@/views/dashboard/HomeView.vue'
import HomeLoggedView from '@/views/dashboard/HomeLoggedView.vue'
import PlanView from '@/views/plans/PlanView.vue'
import PurchaseView from '@/views/features/PurchaseView.vue'
import UserView from '@/views/dashboard/UserView.vue'
import RoomView from '@/views/rooms/RoomView.vue'
import RegisterView from '@/views/auth/RegisterView.vue'
import LoginView from '@/views/auth/LoginView.vue'
import VerifyEmailView from '@/views/auth/VerifyEmailView.vue'
import RutinaView from '@/views/features/RutinaView.vue'
import JoinRoomView from '@/views/rooms/JoinRoomView.vue'
import ProfileSettingsView from '@/views/profile/ProfileSettingsView.vue'
import HealthCalculatorView from '@/views/features/HealthCalculatorView.vue'
import ChatView from '@/views/rooms/ChatView.vue'
import PaymentView from '@/views/plans/PaymentView.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
    meta: { requiresGuest: true },
  },
  {
    path: '/homeLogged',
    name: 'homeLogged',
    component: HomeLoggedView,
    meta: { requiresAuth: true },
  },
  {
    path: '/plan',
    name: 'plan',
    component: PlanView,
    meta: { requiresAuth: true },
  },
  {
    path: '/purchase',
    name: 'purchase',
    component: PurchaseView,
    meta: { requiresAuth: true },
  },
  {
    path: '/user',
    name: 'user',
    component: UserView,
    meta: { requiresAuth: true },
  },
  {
    path: '/room',
    name: 'room',
    component: RoomView,
    meta: { requiresAuth: true },
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterView,
    meta: { requiresGuest: true },
  },
  {
    path: '/verify-email',
    name: 'verifyEmail',
    component: VerifyEmailView,
    meta: { requiresAuth: true },
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { requiresGuest: true },
  },
  {
    path: '/rutina',
    name: 'rutina',
    component: RutinaView,
  },
  {
    path: '/sala',
    name: 'sala',
    component: JoinRoomView,
    meta: { requiresAuth: true },
  },
  {
    path: '/profile',
    name: 'profile',
    component: ProfileSettingsView,
    meta: { requiresAuth: true },
  },
  {
    path: '/calculator',
    name: 'calculator',
    component: HealthCalculatorView,
    meta: { requiresAuth: true, requiresPremium: true },
  },
  {
    path: '/CoachAi',
    name: 'coachAi',
    component: ChatView,
    meta: { requiresAuth: true, requiresPremium: true },
  },
  {
    path: '/payment',
    name: 'payment',
    component: PaymentView,
    meta: { requiresAuth: true },
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    return new Promise((resolve) => {
      setTimeout(() => {
        // Scroll en window
        window.scrollTo({ top: 0, left: 0, behavior: 'instant' })
        
        // Scroll en el body por si acaso
        document.documentElement.scrollTop = 0
        document.body.scrollTop = 0
        
        // También en el v-main de Vuetify
        const mainElement = document.querySelector('.v-main')
        if (mainElement) {
          mainElement.scrollTop = 0
        }
        
        resolve({ top: 0, left: 0 })
      }, 100) // Delay de 100ms para asegurar que el DOM esté renderizado
    })
  }
})

// **Global Guard**
router.beforeEach(async (to, from, next) => {
  const store = useUserStore()
  const subscriptionStore = useSubscriptionStore()

  // Verificar si hay token válido en localStorage (incluso si loggedUser no está poblado aún)
  const token = localStorage.getItem('token')
  const isLogged = !!store.loggedUser?.email || !!token

  if (to.meta.requiresGuest && isLogged) {
    // intenta ir a home/página pública estando logueado → homeLogged
    return next({ name: 'homeLogged' })
  }

  if (to.meta.requiresAuth && !isLogged) {
    // intenta ir a ruta protegida sin sesión → home público
    return next({ name: 'home' })
  }

  // Verificar suscripción premium para rutas protegidas
  if (to.meta.requiresPremium) {
    // Primero verificar que esté logueado
    if (!isLogged) {
      return next({ name: 'home' })
    }

    // Verificar si tiene suscripción activa
    await subscriptionStore.checkSubscription()

    if (!subscriptionStore.hasActiveSubscription) {
      // Sin suscripción → redirigir a página de planes
      return next({ name: 'plan', query: { premium: 'required' } })
    }
  }

  next()
})

export default router
