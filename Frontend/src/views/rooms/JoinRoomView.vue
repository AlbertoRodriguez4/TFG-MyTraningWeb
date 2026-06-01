<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/userStore'
import { useRoomStore } from '@/stores/RoomStore'
import { useUserRoomStore } from '@/stores/UsersRoomStore'
import defaultAvatar from '@/assets/imgs/usuario.png'
import { logger } from '@/utils/logger'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const roomStore = useRoomStore()
const userRoomStore = useUserRoomStore()

const loggedUser = ref(userStore.loggedUser)
const showJoinPopup = ref(false)
const isLoading = ref(true)

// Snackbar state
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')

// Diálogo de confirmación para salir
const showLeaveConfirmDialog = ref(false)

const showSnackbar = (message: string, color: string = 'success') => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

// Datos de la sala
const roomData = ref({
  id: Number(route.query.id || route.params.id),
  name: route.query.name as string || '',
  minlevel: Number(route.query.minlevel) || 0,
  minstats: Number(route.query.minstats) || 0,
  minconsistency: Number(route.query.minconsistency) || 0,
  description: route.query.description as string || '',
  date: route.query.date as string || '',
  localization: route.query.localization as string || ''
})

// Computed para verificar si el usuario está en la sala
const isJoined = computed(() => {
  if (!loggedUser.value?.name) return false
  return userRoomStore.isMemberInRoom(loggedUser.value.name)
})

// Computed para obtener los usuarios de la sala formateados
const roomUsers = computed(() => {
  return userRoomStore.currentRoomMembers.map(member => {
    const user = member.user || {}

    return {
      id: user.name || member.userid,
      username: user.name || t('room.unknownUser'),
      level: user.level || 0,
      stats: (user.strength || 0) + (user.endurance || 0),
      strength: user.strength || 0,
      endurance: user.endurance || 0,
      experience: user.experience || 0,
      consistency: user.consistencyStreak || 0,
      avatarUrl: user.avatarUrl || null,  // ← campo real del backend
      status: 'online',
      equippedStrengthItem: user.equippedStrengthItem || null,
      equippedEnduranceItem: user.equippedEnduranceItem || null
    }
  })
})

onMounted(async () => {
  try {
    await userRoomStore.fetchMembersByRoomId(roomData.value.id)
    if (loggedUser.value?.id) {
      await userRoomStore.fetchRoomsByUserId(loggedUser.value.id)
    }
  } catch (error) {
    logger.error('Error al cargar la información de la sala:', error)
  } finally {
    isLoading.value = false
  }

})

const getRoomDifficulty = (level: number) => {
  if (level >= 50) return 'legendary'
  if (level >= 30) return 'epic'
  if (level >= 15) return 'rare'
  return 'common'
}

const getRoomIcon = (level: number) => {
  if (level >= 50) return 'mdi-dragon'
  if (level >= 30) return 'mdi-star'
  if (level >= 15) return 'mdi-diamond-stone'
  return 'mdi-target'
}

const getStatusColor = (status: string) => {
  switch (status) {
    case 'online': return '#22c55e'
    case 'training': return '#f59e0b'
    case 'offline': return '#6b7280'
    default: return '#6b7280'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'online': return t('room_online')
    case 'training': return t('room.training')
    case 'offline': return t('room.offline')
    default: return t('room.unknown')
  }
}
// Verificar que el usuario cumple con los requisitos para unirse a la sala 
const canJoinRoom = computed(() => {
  if (!loggedUser.value) return false

  const userLevel = loggedUser.value.level || 0
  const userStats = (loggedUser.value.strength || 0) + (loggedUser.value.endurance || 0)
  const userConsistency = loggedUser.value.consistencyStreak || 0

  return userLevel >= roomData.value.minlevel &&
    userStats >= roomData.value.minstats &&
    userConsistency >= roomData.value.minconsistency
})

const openJoinPopup = () => {
  if (canJoinRoom.value && !isJoined.value) {
    showJoinPopup.value = true
  }
}

const closeJoinPopup = () => {
  showJoinPopup.value = false
}

const confirmJoinRoom = async () => {
  if (!loggedUser.value?.id) {
    logger.error('No hay usuario logueado')
    return
  }

  try {
    await userRoomStore.joinRoom(loggedUser.value.id, roomData.value.id)
    await userRoomStore.fetchMembersByRoomId(roomData.value.id)
    // Disparar evento para actualizar la lista de salas
    window.dispatchEvent(new CustomEvent('room-membership-changed')) // Evento personalizado para actualizar la lista de salas en RoomListView
    showJoinPopup.value = false
    showSnackbar(t('room.joinedSuccess'), 'success')
  } catch (error: any) {
    logger.error('Error al unirse a la sala:', error)
    showSnackbar(error.message || t('room.joinError'), 'error')
  }
}

const openLeaveConfirmDialog = () => {
  showLeaveConfirmDialog.value = true
}

const closeLeaveConfirmDialog = () => {
  showLeaveConfirmDialog.value = false
}

const leaveRoom = async () => {
  if (!loggedUser.value?.id) {
    showSnackbar(t('room.mustBeLogged'), 'error')
    return
  }

  try {
    // Misma logica que al unirse pero para salir de la sala, se hace la petición a la API para salir de la sala, se actualiza la lista de miembros y salas, 
    // se dispara un evento personalizado para actualizar la lista de salas en RoomListView y se muestra un mensaje de éxito o error según corresponda.
    await userRoomStore.leaveRoom(loggedUser.value.id, roomData.value.id)
    await userRoomStore.fetchMembersByRoomId(roomData.value.id)
    await userRoomStore.fetchRoomsByUserId(loggedUser.value.id)
    window.dispatchEvent(new CustomEvent('room-membership-changed')) // Evento personalizado para actualizar la lista de salas en RoomListView
    showLeaveConfirmDialog.value = false
    showSnackbar(t('room.leftSuccess'), 'success')
  } catch (error: any) {
    showLeaveConfirmDialog.value = false
    showSnackbar(error.message || t('room.leaveError'), 'error')
  }
}

const goBack = () => {
  router.push('/room')
}
</script>

<template>
  <div class="room-view-wrapper">
    <!-- Header con botón volver -->
    <div class="view-header">
      <button @click="goBack" class="back-btn">
        <span class="back-icon"><v-icon>mdi-arrow-left</v-icon></span>
        <span>{{ $t('volver_salas') }}</span>
      </button>
    </div>

    <div v-if="isLoading || userRoomStore.loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>{{ $t('cargando_sala') }}</p>
    </div>

    <div v-else class="room-view-container">
      <!-- Información de la Sala -->
      <div class="room-info-card" :class="`difficulty-${getRoomDifficulty(roomData.minlevel)}`">
        <div class="info-card-glow"></div>

        <div class="room-info-header">
          <div class="room-main-avatar">
            <div class="main-avatar-icon"><v-icon>{{ getRoomIcon(roomData.minlevel) }}</v-icon></div>
            <div class="main-avatar-ring"></div>
          </div>

          <div class="room-title-section">
            <h1 class="room-title">{{ roomData.name }}</h1>
            <div class="difficulty-badge-large" :class="`badge-${getRoomDifficulty(roomData.minlevel)}`">
              {{ getRoomDifficulty(roomData.minlevel).toUpperCase() }}
            </div>
          </div>
        </div>

        <!-- Descripción de la Sala -->
        <div class="room-description-box">
          <div class="description-box-header">
            <span class="description-box-icon"><v-icon>mdi-pencil</v-icon></span>
            <span class="description-box-title">{{ $t('description') }}</span>
          </div>
          <p class="description-box-text">{{ roomData.description || $t('room_no_description') }}</p>
        </div>

        <!-- Fecha -->
        <div class="room-date-box">
          <span class="date-box-icon"><v-icon>mdi-calendar</v-icon></span>
          <div class="date-box-content">
            <span class="date-box-label">{{ $t('room_event_day') }}</span>
            <span class="date-box-value">{{ roomData.date }}</span>
          </div>
        </div>

        <!-- Localización -->
        <div class="room-localization-box">
          <span class="localization-box-icon"><v-icon>mdi-map-marker</v-icon></span>
          <div class="localization-box-content">
            <span class="localization-box-label">{{ $t('room_localization') }}</span>
            <span class="localization-box-value">{{ roomData.localization || $t('not_specified') }}</span>
          </div>
        </div>

        <div class="room-requirements-grid">
          <div class="requirement-card">
            <div class="req-card-icon"><v-icon>mdi-chart-bar</v-icon></div>
            <div class="req-card-content">
              <span class="req-card-label">{{ $t('room_min_level') }}</span>
              <span class="req-card-value">{{ roomData.minlevel }}</span>
            </div>
          </div>
          <div class="requirement-card">
            <div class="req-card-icon"><v-icon>mdi-arm-flex</v-icon></div>
            <div class="req-card-content">
              <span class="req-card-label">{{ $t('min_stats_req') }}</span>
              <span class="req-card-value">{{ roomData.minstats }}</span>
            </div>
          </div>
          <div class="requirement-card">
            <div class="req-card-icon"><v-icon>mdi-target</v-icon></div>
            <div class="req-card-content">
              <span class="req-card-label">{{ $t('min_consistency_req') }}</span>
              <span class="req-card-value">{{ roomData.minconsistency }}%</span>
            </div>
          </div>
        </div>

        <!-- Botón de unirse/salir -->
        <div class="join-section">
          <div v-if="!canJoinRoom && !isJoined" class="requirements-warning">
            <span class="warning-icon"><v-icon>mdi-alert</v-icon></span>
            <span>{{ $t('no_cumples_requisitos') }}</span>
          </div>

          <button v-if="!isJoined" @click="openJoinPopup" class="main-join-btn" :disabled="!canJoinRoom"
            :class="{ disabled: !canJoinRoom }">
            <span class="btn-icon"><v-icon>mdi-rocket</v-icon></span>
            <span>{{ $t('unirse_sala') }}</span>
          </button>

          <button v-else @click.prevent="openLeaveConfirmDialog" class="leave-btn" type="button">
            <span class="btn-icon"><v-icon>mdi-door</v-icon></span>
            <span>{{ $t('salir_sala_btn') }}</span>
          </button>
        </div>
      </div>

      <!-- Usuarios en la Sala -->
      <div class="users-section">
        <div class="users-header">
          <h2 class="users-title">
            <span class="users-icon"><v-icon>mdi-account-group</v-icon></span>
            <span>{{ $t('usuarios_sala') }}</span>
          </h2>
          <div class="users-count-badge">{{ userRoomStore.memberCount }}</div>
        </div>

        <div v-if="roomUsers.length === 0" class="empty-users">
          <v-icon class="empty-icon" color="grey">mdi-account-outline</v-icon>
          <p>{{ $t('no_usuarios') }}</p>
        </div>

        <div v-else class="users-grid">
          <div v-for="user in roomUsers" :key="user.id" class="user-card">
            <div class="user-card-header">

              <!-- Avatar con imagen real -->
              <div class="user-avatar">
                <img :src="user.avatarUrl || defaultAvatar" :alt="user.username" class="user-avatar-img"
                  @error="(e) => (e.target as HTMLImageElement).src = defaultAvatar" />
                <div class="user-status-dot" :style="{ backgroundColor: getStatusColor(user.status) }"></div>
              </div>

              <div class="user-info">
                <h3 class="user-name">{{ user.username }}</h3>
                <span class="user-status-text" :style="{ color: getStatusColor(user.status) }">
                  {{ getStatusText(user.status) }}
                </span>
              </div>
            </div>

            <div class="user-stats-row">
              <div class="user-stat">
                <span class="stat-label">{{ $t('level_label') }}</span>
                <span class="stat-value">{{ user.level }}</span>
              </div>
              <div class="user-stat">
                <span class="stat-label">{{ $t('experiencia') }}</span>
                <span class="stat-value">{{ user.experience }}</span>
              </div>
            </div>

            <div class="user-stats-row">
              <div class="user-stat">
                <span class="stat-label"><v-icon>mdi-arm-flex</v-icon> {{ $t('strength_label') }}</span>
                <span class="stat-value">{{ user.strength }}</span>
              </div>
              <div class="user-stat">
                <span class="stat-label"><v-icon>mdi-run</v-icon> {{ $t('endurance_label') }}</span>
                <span class="stat-value">{{ user.endurance }}</span>
              </div>
            </div>

            <div class="user-consistency">
              <span class="consistency-label"><v-icon>mdi-target</v-icon> {{ $t('consistency_streak_label') }}</span>
              <span class="consistency-value">{{ user.consistency }} {{ $t('days_count') }}</span>
            </div>

            <!-- Equipped Items Section -->
            <div v-if="user.equippedStrengthItem || user.equippedEnduranceItem" class="user-equipment">
              <div class="equipment-header">
                <span class="equipment-icon"><v-icon>mdi-sword-cross</v-icon></span>
                <span class="equipment-title">{{ $t('equipamiento') }}</span>
              </div>
              <div class="equipment-items">
                <div v-if="user.equippedStrengthItem" class="equipment-item item-strength">
                  <div class="item-icon"><v-icon>mdi-arm-flex</v-icon></div>
                  <div class="item-info">
                    <span class="item-name">{{ user.equippedStrengthItem.name }}</span>
                    <span class="item-bonus">+{{ user.equippedStrengthItem.bonus }} {{ $t('strength_label') }}</span>
                  </div>
                </div>
                <div v-if="user.equippedEnduranceItem" class="equipment-item item-endurance">
                  <div class="item-icon"><v-icon>mdi-run</v-icon></div>
                  <div class="item-info">
                    <span class="item-name">{{ user.equippedEnduranceItem.name }}</span>
                    <span class="item-bonus">+{{ user.equippedEnduranceItem.bonus }} {{ $t('endurance_label') }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Popup de Confirmación para Unirse -->
    <Transition name="popup">
      <div v-if="showJoinPopup" class="popup-overlay" @click="closeJoinPopup">
        <div class="popup-content" @click.stop>
          <div class="popup-icon-container">
            <div class="popup-icon"><v-icon>mdi-alert</v-icon></div>
          </div>
          <h2 class="popup-title">{{ $t('codigo_conducta') }}</h2>
          <div class="popup-body">
            <p class="popup-text">
              {{ $t('codigo_conducta_text') }}
            </p>
            <ul class="rules-list">
              <li><v-icon>mdi-handshake</v-icon> {{ $t('respetar') }}</li>
              <li><v-icon>mdi-message</v-icon> {{ $t('lenguaje') }}</li>
              <li><v-icon>mdi-target</v-icon> {{ $t('enfocarte') }}</li>
              <li><v-icon>mdi-cancel</v-icon> {{ $t('no_spam') }}</li>
              <li><v-icon>mdi-trending-neutral</v-icon> {{ $t('aceptar_consecuencias') }}</li>
            </ul>
            <p class="popup-warning">
              {{ $t('advertencia_conducta') }}
            </p>
          </div>
          <div class="popup-actions">
            <button @click="closeJoinPopup" class="popup-btn cancel-btn">
              <span>{{ $t('cancelar') }}</span>
            </button>
            <button @click="confirmJoinRoom" class="popup-btn confirm-btn" :disabled="userRoomStore.loading">
              <span v-if="!userRoomStore.loading">{{ $t('acepto_unirme') }}</span>
              <span v-else>{{ $t('joining') }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Diálogo de Confirmación para Salir -->
    <Transition name="popup">
      <div v-if="showLeaveConfirmDialog" class="popup-overlay" @click="closeLeaveConfirmDialog">
        <div class="popup-content confirm-leave-dialog" @click.stop>
          <div class="popup-icon-container">
            <div class="popup-icon"><v-icon>mdi-help-circle</v-icon></div>
          </div>
          <h2 class="popup-title">{{ $t('salir_sala') }}</h2>
          <div class="popup-body">
            <p class="popup-text">
              {{ $t('salir_sala_text') }} <strong>{{ roomData.name }}</strong>?
            </p>
            <p class="popup-text-secondary">
              {{ $t('volver_unir') }}
            </p>
          </div>
          <div class="popup-actions">
            <button @click="closeLeaveConfirmDialog" class="popup-btn cancel-btn">
              <span>{{ $t('cancelar') }}</span>
            </button>
            <button @click="leaveRoom" class="popup-btn danger-btn" :disabled="userRoomStore.loading">
              <span v-if="!userRoomStore.loading">{{ $t('yes_leave') }}</span>
              <span v-else>{{ $t('room.leaving') }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Snackbar -->
    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4000" location="top" multi-line>
      <div class="d-flex align-center">
        <v-icon :icon="snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'" class="mr-3"></v-icon>
        <span>{{ snackbarMessage }}</span>
      </div>
      <template v-slot:actions>
        <v-btn variant="text" @click="snackbar = false" icon="mdi-close"></v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<style scoped>
.confirm-leave-dialog {
  max-width: 450px;
}

.popup-text-secondary {
  color: #94a3b8;
  font-size: 0.9rem;
  line-height: 1.5;
  margin-top: 0.5rem;
  text-align: center;
}

.danger-btn {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  color: white;
}

.danger-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(239, 68, 68, 0.4);
}

.danger-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.room-view-wrapper {
  min-height: 100vh;
  height: 100vh;
  overflow-y: auto;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  padding: 2rem;
}

.view-header {
  max-width: 1400px;
  margin: 0 auto 2rem;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  color: #e2e8f0;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.back-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateX(-4px);
}

.back-icon {
  font-size: 1.2rem;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  gap: 1rem;
  color: #e2e8f0;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.1);
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.room-view-container {
  max-width: 1400px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.room-info-card {
  position: relative;
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  border-radius: 24px;
  padding: 2.5rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
  height: fit-content;
}

.info-card-glow {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.1) 0%, transparent 70%);
  animation: glow-pulse 3s ease-in-out infinite;
}

@keyframes glow-pulse {

  0%,
  100% {
    opacity: 0.5;
  }

  50% {
    opacity: 1;
  }
}

.room-info-header {
  display: flex;
  align-items: center;
  gap: 2rem;
  margin-bottom: 2.5rem;
}

.room-main-avatar {
  position: relative;
  width: 100px;
  height: 100px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.main-avatar-icon {
  font-size: 3.5rem;
  z-index: 2;
  position: relative;
}

.main-avatar-ring {
  position: absolute;
  inset: -10px;
  border-radius: 50%;
  border: 3px solid rgba(59, 130, 246, 0.3);
  animation: ring-rotate 3s linear infinite;
}

@keyframes ring-rotate {
  to {
    transform: rotate(360deg);
  }
}

.room-title-section {
  flex: 1;
}

.room-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #f8fafc;
  margin-bottom: 0.75rem;
}

.difficulty-badge-large {
  display: inline-block;
  padding: 0.5rem 1.25rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 700;
  letter-spacing: 1px;
}

.badge-common {
  background: linear-gradient(135deg, #64748b 0%, #475569 100%);
  color: #f1f5f9;
}

.badge-rare {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: #f0f9ff;
}

.badge-epic {
  background: linear-gradient(135deg, #a855f7 0%, #9333ea 100%);
  color: #faf5ff;
}

.badge-legendary {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: #fffbeb;
}

.room-requirements-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.requirement-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  padding: 1.5rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  transition: all 0.3s ease;
}

.requirement-card:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.req-card-icon {
  font-size: 2rem;
}

.req-card-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.req-card-label {
  font-size: 0.85rem;
  color: #94a3b8;
  font-weight: 500;
}

.req-card-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #f8fafc;
}

.join-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.requirements-warning {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
  color: #fca5a5;
  font-size: 0.9rem;
}

.warning-icon {
  font-size: 1.5rem;
}

.main-join-btn,
.leave-btn {
  width: 100%;
  padding: 1.25rem;
  border-radius: 16px;
  font-size: 1.1rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  z-index: 50;
}

.main-join-btn {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
}

.main-join-btn:hover:not(.disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(59, 130, 246, 0.4);
}

.main-join-btn.disabled {
  background: rgba(100, 116, 139, 0.5);
  cursor: not-allowed;
  opacity: 0.6;
}

.leave-btn {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  color: white;
}

.leave-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(239, 68, 68, 0.4);
}

/* Users Section */
.users-section {
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(15, 23, 42, 0.9) 100%);
  border-radius: 24px;
  padding: 2.5rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.users-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.users-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.75rem;
  font-weight: 700;
  color: #f8fafc;
}

.users-icon {
  font-size: 2rem;
}

.users-count-badge {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1.1rem;
}

.empty-users {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  gap: 1rem;
  color: #94a3b8;
}

.empty-icon {
  font-size: 4rem;
  opacity: 0.5;
}

.users-grid {
  display: grid;
  gap: 1.25rem;
}

.user-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  padding: 1.5rem;
  transition: all 0.3s ease;
}

.user-card:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateX(4px);
}

.user-card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

/* Avatar con imagen real */
.user-avatar {
  position: relative;
  width: 60px;
  height: 60px;
  flex-shrink: 0;
}

.user-avatar-img {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid rgba(59, 130, 246, 0.4);
  background-color: #1e293b;
  display: block;
  transition: border-color 0.3s ease;
}

.user-card:hover .user-avatar-img {
  border-color: rgba(59, 130, 246, 0.7);
}

.user-status-dot {
  position: absolute;
  bottom: 2px;
  right: 2px;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 3px solid #1e293b;
}

.user-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.user-name {
  font-size: 1.1rem;
  font-weight: 600;
  color: #f8fafc;
}

.user-status-text {
  font-size: 0.85rem;
  font-weight: 500;
}

.user-stats-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1rem;
}

.user-stat {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
}

.stat-label {
  font-size: 0.8rem;
  color: #94a3b8;
  font-weight: 500;
}

.stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #f8fafc;
}

.user-consistency {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1) 0%, rgba(37, 99, 235, 0.1) 100%);
  border: 1px solid rgba(59, 130, 246, 0.2);
  border-radius: 10px;
  margin-bottom: 1rem;
}

.consistency-label {
  font-size: 0.85rem;
  color: #94a3b8;
  font-weight: 500;
}

.consistency-value {
  font-size: 1rem;
  font-weight: 700;
  color: #3b82f6;
}

.user-equipment {
  margin-top: 1rem;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.05);
}

.equipment-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.equipment-icon {
  font-size: 1.25rem;
}

.equipment-title {
  font-size: 0.85rem;
  font-weight: 700;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.equipment-items {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.equipment-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  border: 1px solid;
  transition: all 0.3s ease;
}

.equipment-item:hover {
  transform: translateX(4px);
  background: rgba(255, 255, 255, 0.08);
}

.item-strength {
  border-color: rgba(239, 68, 68, 0.3);
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.1) 0%, rgba(255, 255, 255, 0.05) 100%);
}

.item-strength:hover {
  border-color: rgba(239, 68, 68, 0.5);
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.2);
}

.item-endurance {
  border-color: rgba(59, 130, 246, 0.3);
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1) 0%, rgba(255, 255, 255, 0.05) 100%);
}

.item-endurance:hover {
  border-color: rgba(59, 130, 246, 0.5);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.2);
}

.item-icon {
  font-size: 1.5rem;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
}

.item-strength .item-icon {
  background: rgba(239, 68, 68, 0.2);
}

.item-endurance .item-icon {
  background: rgba(59, 130, 246, 0.2);
}

.item-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.item-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: #e2e8f0;
}

.item-bonus {
  font-size: 0.75rem;
  font-weight: 700;
  color: #fbbf24;
}

.item-strength .item-bonus {
  color: #ef4444;
}

.item-endurance .item-bonus {
  color: #3b82f6;
}

/* Popup */
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.popup-content {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 24px;
  padding: 2.5rem;
  max-width: 550px;
  width: 100%;
  border: 2px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.5);
}

.popup-icon-container {
  display: flex;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.popup-icon {
  font-size: 4rem;
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-10px);
  }
}

.popup-title {
  font-size: 2rem;
  font-weight: 700;
  color: #f8fafc;
  text-align: center;
  margin-bottom: 1.5rem;
}

.popup-body {
  margin-bottom: 2rem;
}

.popup-text {
  color: #cbd5e1;
  font-size: 1rem;
  line-height: 1.6;
  margin-bottom: 1.5rem;
  text-align: center;
}

.rules-list {
  list-style: none;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.rules-list li {
  color: #e2e8f0;
  font-size: 0.95rem;
  padding: 0.75rem 1rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  border-left: 4px solid #3b82f6;
}

.popup-warning {
  color: #fca5a5;
  font-size: 0.9rem;
  font-weight: 500;
  padding: 1rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  border-radius: 12px;
}

.popup-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.popup-btn {
  padding: 1rem;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: rgba(255, 255, 255, 0.1);
  color: #e2e8f0;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.cancel-btn:hover {
  background: rgba(255, 255, 255, 0.15);
}

.confirm-btn {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: white;
}

.confirm-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(59, 130, 246, 0.4);
}

.confirm-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.popup-enter-active,
.popup-leave-active {
  transition: all 0.3s ease;
}

.popup-enter-from,
.popup-leave-to {
  opacity: 0;
}

.popup-enter-from .popup-content,
.popup-leave-to .popup-content {
  transform: scale(0.9) translateY(20px);
}

/* Descripción */
.room-description-box {
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.08), rgba(139, 92, 246, 0.08));
  border: 2px solid rgba(99, 102, 241, 0.25);
  border-radius: 16px;
  padding: 1.25rem;
  margin-bottom: 1.5rem;
  transition: all 0.3s ease;
}

.room-description-box:hover {
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.12), rgba(139, 92, 246, 0.12));
  border-color: rgba(99, 102, 241, 0.4);
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(99, 102, 241, 0.15);
}

.description-box-header {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  margin-bottom: 0.75rem;
}

.description-box-icon {
  font-size: 1.5rem;
  filter: drop-shadow(0 2px 6px rgba(99, 102, 241, 0.3));
}

.description-box-title {
  font-size: 1rem;
  font-weight: 800;
  color: #6366f1;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.description-box-text {
  margin: 0;
  font-size: 1rem;
  line-height: 1.6;
  color: #cbd5e1;
  padding-left: 2.125rem;
}

/* Fecha */
.room-date-box {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.08), rgba(251, 146, 60, 0.08));
  border: 2px solid rgba(245, 158, 11, 0.25);
  border-radius: 16px;
  margin-bottom: 1.5rem;
  transition: all 0.3s ease;
}

.room-date-box:hover {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.12), rgba(251, 146, 60, 0.12));
  border-color: rgba(245, 158, 11, 0.4);
  transform: translateX(4px);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.15);
}

.date-box-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 6px rgba(245, 158, 11, 0.3));
}

.date-box-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.date-box-label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #92400e;
  text-transform: uppercase;
  letter-spacing: 0.8px;
}

.date-box-value {
  font-size: 1.125rem;
  font-weight: 800;
  color: #f59e0b;
  font-family: 'Courier New', monospace;
}

/* Localización */
.room-localization-box {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, rgba(16, 185, 129, 0.08), rgba(5, 150, 105, 0.08));
  border: 2px solid rgba(16, 185, 129, 0.25);
  border-radius: 16px;
  margin-bottom: 1.5rem;
  transition: all 0.3s ease;
}

.room-localization-box:hover {
  background: linear-gradient(135deg, rgba(16, 185, 129, 0.12), rgba(5, 150, 105, 0.12));
  border-color: rgba(16, 185, 129, 0.4);
  transform: translateX(4px);
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.15);
}

.localization-box-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 6px rgba(16, 185, 129, 0.3));
}

.localization-box-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.localization-box-label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #065f46;
  text-transform: uppercase;
  letter-spacing: 0.8px;
}

.localization-box-value {
  font-size: 1.125rem;
  font-weight: 800;
  color: #10b981;
  font-family: 'Courier New', monospace;
}

/* Responsive */
@media (max-width: 1024px) {
  .room-view-container {
    grid-template-columns: 1fr;
  }

  .room-requirements-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .room-view-wrapper {
    padding: 1rem;
    padding-bottom: 2rem;
  }

  .view-header {
    margin-bottom: 1rem;
  }

  .back-btn {
    padding: 0.6rem 1rem;
    font-size: 0.85rem;
  }

  .room-info-header {
    flex-direction: column;
    text-align: center;
  }

  .room-title {
    font-size: 1.75rem;
  }

  .room-info-card {
    padding: 1.5rem;
    border-radius: 18px;
  }

  .room-requirements-grid {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }

  .requirement-card {
    padding: 1rem;
  }

  .users-section {
    margin-top: 0;
  }

  .users-grid {
    gap: 0.75rem;
  }

  .user-card {
    padding: 1rem;
  }

  .user-avatar-img {
    width: 50px;
    height: 50px;
  }

  .user-avatar {
    width: 50px;
    height: 50px;
  }

  .popup-actions {
    grid-template-columns: 1fr;
  }

  .popup-content {
    max-height: 90vh;
    overflow-y: auto;
  }

  .join-section {
    margin-top: 1.5rem;
  }

  .main-join-btn,
  .leave-btn {
    padding: 1rem 1.5rem;
    font-size: 1rem;
  }
}
</style>