<script setup lang="ts">
import { useRoomStore } from '@/stores/RoomStore'
import { useUserRoomStore } from '@/stores/UsersRoomStore'
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import CreateRoomPopup from '../PopUps/RoomPopup.vue'
import { useUserStore } from '@/stores/userStore'
import EditRoomPopup from '../PopUps/EditRoomPopup.vue'
import RoomCard from '../Cards/RoomCard.vue'

const store = useRoomStore()
const userRoomStore = useUserRoomStore()
const userStore = useUserStore()
const router = useRouter()
const loggedUser = ref(userStore.loggedUser)

const sortField      = ref<'level' | 'stats' | null>(null)
const sortDirection  = ref<'asc' | 'desc'>('asc')
const searchTerm     = ref('')
const activeFilter   = ref<'joinable' | 'joined' | null>(null)

const selectedRoom = defineModel<{
  id: number; name: string; minlevel: number; minstats: number
  minconsistency: number; description: string; date: string; localization: string
} | null>('selectedItem')
const showRoomPopup  = ref(false)
const isPopupVisible = ref(false)

const currentPage  = ref(1)
const itemsPerPage = 3

const roomMemberCounts = ref<Map<number, number>>(new Map())
// Set con los IDs de salas en las que el usuario ya está
const joinedRoomIds    = ref<Set<number>>(new Set())

// Event listeners para limpiar
const handleMembershipChange = async () => {
  await loadJoinedRooms()
  await loadRoomMemberCounts()
}

const handleFocus = async () => {
  await loadJoinedRooms()
  await loadRoomMemberCounts()
}

onMounted(async () => {
  await store.fetchRoom()
  await loadRoomMemberCounts()
  await loadJoinedRooms()

  // Escuchar evento de cambio de membresía
  window.addEventListener('room-membership-changed', handleMembershipChange)
  window.addEventListener('focus', handleFocus)
})

onUnmounted(() => {
  window.removeEventListener('room-membership-changed', handleMembershipChange)
  window.removeEventListener('focus', handleFocus)
})

watch(searchTerm,    () => { currentPage.value = 1 })
watch(activeFilter,  () => { currentPage.value = 1 })

async function loadRoomMemberCounts() {
  for (const room of store.room) {
    try {
      await userRoomStore.fetchMembersByRoomId(room.id)
      roomMemberCounts.value.set(room.id, userRoomStore.memberCount)
    } catch {
      roomMemberCounts.value.set(room.id, 0)
    }
  }
}

async function loadJoinedRooms() {
  if (!loggedUser.value?.id) return
  try {
    const ids = new Set<number>()
    for (const room of store.room) {
      await userRoomStore.fetchMembersByRoomId(room.id)
      // currentRoomMembers contiene objetos { userid, roomid, user: {...} }
      const members = userRoomStore.currentRoomMembers
      if (members.some((m) => m.userid === loggedUser.value?.id)) {
        ids.add(room.id)
      }
    }
    joinedRoomIds.value = ids
  } catch (e) {
    console.error('Error cargando salas unidas', e)
  }
}

function getMemberCount(roomId: number) {
  return roomMemberCounts.value.get(roomId) || 0
}

function isJoined(roomId: number) {
  return joinedRoomIds.value.has(roomId)
}

// Comprueba si el usuario cumple los requisitos mínimos de la sala
function canJoin(room: { minlevel: number; minstats: number; minconsistency: number }) {
  const u = loggedUser.value as any
  if (!u) return false
  const level       = u.level       ?? u.nivelUsuario  ?? 0
  const stats       = u.stats       ?? u.statsUsuario  ?? 0
  const consistency = u.consistency ?? u.consistencia  ?? 0
  return level >= room.minlevel && stats >= room.minstats && consistency >= room.minconsistency
}

function toggleSort(field: 'level' | 'stats') {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
  currentPage.value = 1
}

function toggleFilter(filter: 'joinable' | 'joined') {
  activeFilter.value = activeFilter.value === filter ? null : filter
}

function clearAllFilters() {
  activeFilter.value = null
  sortField.value = null
  sortDirection.value = 'asc'
  searchTerm.value = ''
  currentPage.value = 1
}

const filteredRooms = computed(() => {
  const term = searchTerm.value.toLowerCase()
  let result = store.room.filter(r => r.name.toLowerCase().includes(term))

  // Filtro de dificultad/sort
  if (sortField.value) {
    const f = sortField.value === 'level' ? 'minlevel' : 'minstats'
    result = [...result].sort((a, b) =>
      sortDirection.value === 'asc' ? a[f] - b[f] : b[f] - a[f]
    )
  }

  // Filtros de membresía
  if (activeFilter.value === 'joined') {
    result = result.filter(r => isJoined(r.id))
  } else if (activeFilter.value === 'joinable') {
    result = result.filter(r => !isJoined(r.id) && canJoin(r))
  }

  return result
})

const totalPages = computed(() => Math.ceil(filteredRooms.value.length / itemsPerPage))

const paginatedRooms = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return filteredRooms.value.slice(start, start + itemsPerPage)
})

function goToPage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function previousPage() {
  if (currentPage.value > 1) {
    currentPage.value--
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const pageNumbers = computed(() => {
  const pages: (number | string)[] = []
  const max = 5
  if (totalPages.value <= max) {
    for (let i = 1; i <= totalPages.value; i++) pages.push(i)
  } else if (currentPage.value <= 3) {
    for (let i = 1; i <= 4; i++) pages.push(i)
    pages.push('...'); pages.push(totalPages.value)
  } else if (currentPage.value >= totalPages.value - 2) {
    pages.push(1); pages.push('...')
    for (let i = totalPages.value - 3; i <= totalPages.value; i++) pages.push(i)
  } else {
    pages.push(1); pages.push('...')
    for (let i = currentPage.value - 1; i <= currentPage.value + 1; i++) pages.push(i)
    pages.push('...'); pages.push(totalPages.value)
  }
  return pages
})

function showPopup()  { isPopupVisible.value = true  }
function closePopup() { isPopupVisible.value = false }

async function handleCreateRoom(newRoom: any) {
  await store.createRoom(newRoom, loggedUser.value?.id ?? 0)
  await loadRoomMemberCounts()
  closePopup()
}

function openEditPopup(room: any) {
  selectedRoom.value = { ...room, localization: room.localization ?? '' }
  showRoomPopup.value = true
}

function closeRoomPopup() { showRoomPopup.value = false }

function goToRoom(roomId: number) {
  const room = store.room.find(r => r.id === roomId)
  if (!room) return
  router.push({
    name: 'sala',
    params: { id: room.id },
    query: {
      id: room.id, name: room.name, minlevel: room.minlevel,
      minstats: room.minstats, minconsistency: room.minconsistency,
      description: room.description, date: room.date, localization: room.localization
    }
  })
}
</script>

<template>
  <div class="rooms-wrapper">

    <!-- ── Header ── -->
    <div class="rooms-header">
      <div class="header-left">
        <div class="header-icon">🏛️</div>
        <div class="header-text">
          <h2 class="header-title">Salas de Entrenamiento</h2>
          <p class="header-sub">
            <span class="sub-dot"></span>
            {{ filteredRooms.length }} salas disponibles
          </p>
        </div>
      </div>
      <button @click="showPopup" class="create-btn">
        <span class="create-plus">＋</span>
        <span>Crear sala</span>
      </button>
    </div>

    <!-- ── Divider ── -->
    <div class="header-rule">
      <div class="rule-line"></div>
      <div class="rule-glow"></div>
    </div>

    <!-- ── Controls ── -->
    <div class="controls-bar">
      <!-- Búsqueda -->
      <div class="search-box">
        <span class="search-icon">🔍</span>
        <input
          v-model="searchTerm"
          type="text"
          placeholder="Buscar sala..."
          class="search-input"
        />
        <span v-if="searchTerm" class="search-clear" @click="searchTerm = ''">✕</span>
      </div>
    </div>

    <!-- ── Filtros combinados ── -->
    <div class="all-filters">
      <!-- Filtros de ordenación -->
      <div class="filter-section">
        <span class="filter-section-title">Ordenar por:</span>
        <button
          class="mfilter-btn mfilter-level"
          :class="{ active: sortField === 'level' }"
          @click="toggleSort('level')"
        >
          <span class="mfilter-icon">📊</span>
          <span class="mfilter-label">Nivel</span>
          <span v-if="sortField === 'level'" class="sort-indicator">
            {{ sortDirection === 'asc' ? '↑' : '↓' }}
          </span>
        </button>

        <button
          class="mfilter-btn mfilter-stats"
          :class="{ active: sortField === 'stats' }"
          @click="toggleSort('stats')"
        >
          <span class="mfilter-icon">💪</span>
          <span class="mfilter-label">Stats</span>
          <span v-if="sortField === 'stats'" class="sort-indicator">
            {{ sortDirection === 'asc' ? '↑' : '↓' }}
          </span>
        </button>
      </div>

      <!-- Filtros de membresía -->
      <div class="filter-section">
        <span class="filter-section-title">Filtrar por:</span>
        <button
          class="mfilter-btn mfilter-joined"
          :class="{ active: activeFilter === 'joined' }"
          @click="toggleFilter('joined')"
        >
          <span class="mfilter-icon">✅</span>
          <span class="mfilter-label">Unidas</span>
        </button>

        <button
          class="mfilter-btn mfilter-joinable"
          :class="{ active: activeFilter === 'joinable' }"
          @click="toggleFilter('joinable')"
        >
          <span class="mfilter-icon">🚀</span>
          <span class="mfilter-label">Disponibles</span>
        </button>
      </div>

      <!-- Botón limpiar filtros -->
      <button
        class="mfilter-btn mfilter-clear"
        @click="clearAllFilters"
      >
        <span class="mfilter-icon">🧹</span>
        <span class="mfilter-label">Limpiar filtros</span>
      </button>
    </div>

    <!-- ── Contador ── -->
    <div class="results-bar">
      <div class="results-line"></div>
      <span class="results-label">{{ paginatedRooms.length }} / {{ filteredRooms.length }} salas</span>
      <div class="results-line"></div>
    </div>

    <!-- ── Empty state ── -->
    <div v-if="filteredRooms.length === 0" class="empty-state">
      <div class="empty-emoji">
        {{ activeFilter === 'joined' ? '🔒' : activeFilter === 'joinable' ? '🚧' : '🏋️‍♂️' }}
      </div>
      <p class="empty-title">
        {{
          activeFilter === 'joined'   ? 'No estás en ninguna sala' :
          activeFilter === 'joinable' ? 'No cumples requisitos para ninguna sala disponible' :
          'Sin resultados'
        }}
      </p>
      <p class="empty-sub">
        {{
          activeFilter === 'joined'   ? 'Únete a una sala para verla aquí' :
          activeFilter === 'joinable' ? 'Sigue entrenando para subir tus stats' :
          'Ajusta la búsqueda o crea una nueva sala'
        }}
      </p>
    </div>

    <!-- ── Lista ── -->
    <div v-else class="rooms-list">
      <RoomCard
        v-for="room in paginatedRooms"
        :key="room.id"
        :room="{ ...room, localization: room.localization ?? '' }"
        :member-count="getMemberCount(room.id)"
        :is-staff="loggedUser?.role === 'userStaff'"
        @view="goToRoom"
        @edit="openEditPopup"
      />
    </div>

    <!-- ── Paginación ── -->
    <div v-if="filteredRooms.length > 0 && totalPages > 1" class="pagination">
      <button class="page-btn page-nav" :disabled="currentPage === 1" @click="previousPage">
        ← <span class="nav-label">Anterior</span>
      </button>
      <div class="page-numbers">
        <button
          v-for="(page, i) in pageNumbers"
          :key="i"
          class="page-btn"
          :class="{ active: page === currentPage, ellipsis: page === '...' }"
          :disabled="page === '...'"
          @click="typeof page === 'number' ? goToPage(page) : null"
        >{{ page }}</button>
      </div>
      <button class="page-btn page-nav" :disabled="currentPage === totalPages" @click="nextPage">
        <span class="nav-label">Siguiente</span> →
      </button>
    </div>

    <p v-if="filteredRooms.length > 0" class="pagination-info">
      {{ (currentPage - 1) * itemsPerPage + 1 }}–{{ Math.min(currentPage * itemsPerPage, filteredRooms.length) }}
      de {{ filteredRooms.length }} salas
    </p>

    <!-- Popups -->
    <CreateRoomPopup :isVisible="isPopupVisible" @close="closePopup" @create="handleCreateRoom" />
    <EditRoomPopup
      v-if="selectedRoom"
      :isVisible="showRoomPopup"
      :room="selectedRoom"
      @close="closeRoomPopup"
      @edit="(room) => console.log('Editando', room)"
    />
  </div>
</template>

<style scoped>
.rooms-wrapper {
  width: 100%;
  max-width: 960px;
  margin: 0 auto;
  padding: 2rem 1.5rem 3rem;
  font-family: 'Inter', system-ui, sans-serif;
  color: #e2e8f0;
}

/* ── Header ── */
.rooms-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-icon {
  font-size: 1.5rem;
  width: 48px; height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(251, 191, 36, 0.1);
  border: 1px solid rgba(251, 191, 36, 0.3);
  border-radius: 14px;
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.15), inset 0 0 12px rgba(251, 191, 36, 0.08);
  flex-shrink: 0;
}

.header-title {
  font-size: 1.25rem;
  font-weight: 900;
  color: #f8fafc;
  margin: 0 0 0.2rem;
  letter-spacing: -0.4px;
}

.header-sub {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.6875rem;
  color: #64748b;
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  font-weight: 700;
}

.sub-dot {
  width: 5px; height: 5px;
  border-radius: 50%;
  background: #fbbf24;
  box-shadow: 0 0 6px #fbbf24;
  animation: sdot 2s ease-in-out infinite;
}
@keyframes sdot {
  0%, 100% { opacity: 1; }
  50%       { opacity: 0.2; }
}

.create-btn {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5625rem 1.125rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border: 1px solid rgba(251, 191, 36, 0.5);
  border-radius: 10px;
  color: #0f172a;
  font-size: 0.8125rem;
  font-weight: 800;
  cursor: pointer;
  transition: all 0.22s ease;
  box-shadow: 0 3px 16px rgba(251, 191, 36, 0.4);
  white-space: nowrap;
}

.create-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 24px rgba(251, 191, 36, 0.55);
  filter: brightness(1.1);
}

.create-plus { font-size: 1rem; line-height: 1; font-weight: 300; }

/* ── Divider ── */
.header-rule {
  position: relative;
  height: 1px;
  margin-bottom: 1.5rem;
  overflow: visible;
}

.rule-line {
  position: absolute;
  inset: 0;
  background: linear-gradient(90deg, transparent, rgba(251, 191, 36, 0.3) 30%, rgba(251, 191, 36, 0.3) 70%, transparent);
}

.rule-glow {
  position: absolute;
  top: -2px;
  left: 50%;
  transform: translateX(-50%);
  width: 120px; height: 5px;
  background: radial-gradient(ellipse, rgba(251,191,36,0.55), transparent 70%);
  filter: blur(3px);
}

/* ── Controls ── */
.controls-bar {
  display: flex;
  margin-bottom: 0.5rem;
}

.search-box {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  padding: 0.5625rem 0.875rem;
  transition: all 0.2s;
}

.search-box:focus-within {
  border-color: rgba(251, 191, 36, 0.5);
  background: rgba(251, 191, 36, 0.05);
  box-shadow: 0 0 0 3px rgba(251, 191, 36, 0.1);
}

.search-icon { font-size: 0.75rem; opacity: 0.45; flex-shrink: 0; }

.search-input {
  flex: 1;
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.8125rem;
  color: #e2e8f0;
  min-width: 0;
}

.search-input::placeholder { color: #475569; }

.search-clear {
  font-size: 0.625rem;
  color: #64748b;
  cursor: pointer;
  padding: 0.1rem 0.25rem;
  border-radius: 4px;
  transition: color 0.2s;
  flex-shrink: 0;
}
.search-clear:hover { color: #94a3b8; }

/* ════════════════════════════════════════
   FILTROS COMBINADOS
   ════════════════════════════════════════ */
.all-filters {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.25rem;
  flex-wrap: wrap;
  padding: 0.75rem 1rem;
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
}

.filter-section {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-section-title {
  font-size: 0.6875rem;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #475569;
  font-weight: 700;
  margin-right: 0.25rem;
}

.mfilter-btn {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5rem 0.9rem;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 10px;
  font-size: 0.8125rem;
  font-weight: 700;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
  position: relative;
  overflow: hidden;
}

.mfilter-btn:hover {
  color: #94a3b8;
  border-color: rgba(255, 255, 255, 0.2);
  background: rgba(255, 255, 255, 0.05);
  transform: translateY(-1px);
}

/* Ordenación - Nivel */
.mfilter-level {
  background: rgba(139, 92, 246, 0.08);
  border-color: rgba(139, 92, 246, 0.3);
  color: #c4b5fd;
}

.mfilter-level:hover {
  background: rgba(139, 92, 246, 0.12);
  border-color: rgba(139, 92, 246, 0.4);
}

.mfilter-level.active {
  background: rgba(139, 92, 246, 0.15);
  border-color: rgba(139, 92, 246, 0.5);
  color: #a78bfa;
  box-shadow:
    0 0 0 1px rgba(139, 92, 246, 0.2),
    0 4px 16px rgba(139, 92, 246, 0.15);
}

/* Ordenación - Stats */
.mfilter-stats {
  background: rgba(244, 114, 182, 0.08);
  border-color: rgba(244, 114, 182, 0.3);
  color: #f472b6;
}

.mfilter-stats:hover {
  background: rgba(244, 114, 182, 0.12);
  border-color: rgba(244, 114, 182, 0.4);
}

.mfilter-stats.active {
  background: rgba(244, 114, 182, 0.15);
  border-color: rgba(244, 114, 182, 0.5);
  color: #f472b6;
  box-shadow:
    0 0 0 1px rgba(244, 114, 182, 0.2),
    0 4px 16px rgba(244, 114, 182, 0.15);
}

/* Filtro Unidas */
.mfilter-joined {
  background: rgba(56, 189, 248, 0.08);
  border-color: rgba(56, 189, 248, 0.3);
  color: #7dd3fc;
}

.mfilter-joined:hover {
  background: rgba(56, 189, 248, 0.12);
  border-color: rgba(56, 189, 248, 0.4);
}

.mfilter-joined.active {
  background: rgba(56, 189, 248, 0.15);
  border-color: rgba(56, 189, 248, 0.5);
  color: #38bdf8;
  box-shadow:
    0 0 0 1px rgba(56, 189, 248, 0.2),
    0 4px 16px rgba(56, 189, 248, 0.15);
}

/* Filtro Disponibles */
.mfilter-joinable {
  background: rgba(52, 211, 153, 0.08);
  border-color: rgba(52, 211, 153, 0.3);
  color: #6ee7b7;
}

.mfilter-joinable:hover {
  background: rgba(52, 211, 153, 0.12);
  border-color: rgba(52, 211, 153, 0.4);
}

.mfilter-joinable.active {
  background: rgba(52, 211, 153, 0.15);
  border-color: rgba(52, 211, 153, 0.5);
  color: #34d399;
  box-shadow:
    0 0 0 1px rgba(52, 211, 153, 0.2),
    0 4px 16px rgba(52, 211, 153, 0.15);
}

/* Botón Limpiar filtros */
.mfilter-clear {
  margin-left: auto;
  background: rgba(239, 68, 68, 0.08);
  border-color: rgba(239, 68, 68, 0.3);
  color: #f87171;
}

.mfilter-clear:hover {
  background: rgba(239, 68, 68, 0.15);
  border-color: rgba(239, 68, 68, 0.5);
  color: #ef4444;
  transform: translateY(-1px);
}

.sort-indicator {
  font-weight: 900;
  font-size: 0.875rem;
  margin-left: 0.2rem;
}

.mfilter-icon {
  font-size: 0.875rem;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
}
.mfilter-label {
  letter-spacing: 0.3px;
  text-transform: uppercase;
  font-size: 0.7rem;
}

/* ── Barra de resultados ── */
.results-bar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.75rem;
}

.results-line {
  flex: 1;
  height: 1px;
  background: rgba(255, 255, 255, 0.06);
}

.results-label {
  font-size: 0.5625rem;
  text-transform: uppercase;
  letter-spacing: 1.3px;
  color: #475569;
  font-weight: 700;
  white-space: nowrap;
}

/* ── Empty ── */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 5rem 2rem;
}

.empty-emoji { font-size: 3rem; opacity: 0.3; }
.empty-title { font-size: 0.9375rem; font-weight: 700; color: #94a3b8; margin: 0; text-align: center; }
.empty-sub   { font-size: 0.75rem; color: #64748b; margin: 0; text-align: center; }

/* ── Lista ── */
.rooms-list {
  display: flex;
  flex-direction: column;
  gap: 1.75rem;
}

/* ── Paginación ── */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.375rem;
  margin-top: 2.5rem;
}

.page-numbers { display: flex; gap: 0.3rem; }

.page-btn {
  min-width: 34px;
  height: 34px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.3rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 9px;
  font-size: 0.75rem;
  font-weight: 700;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s;
  font-family: 'Courier New', monospace;
}

.page-btn:hover:not(:disabled):not(.ellipsis) {
  border-color: rgba(251, 191, 36, 0.5);
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.1);
}

.page-btn.active {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border-color: transparent;
  color: #0f172a;
  font-weight: 900;
  box-shadow: 0 2px 12px rgba(251, 191, 36, 0.5);
  transform: scale(1.08);
}

.page-btn:disabled { opacity: 0.25; cursor: not-allowed; }

.page-btn.ellipsis {
  border: none;
  background: transparent;
  color: #475569;
  cursor: default;
  font-family: inherit;
}

.page-nav {
  padding: 0 0.875rem;
  min-width: auto;
  font-family: inherit;
  letter-spacing: 0.2px;
}

.nav-label { font-size: 0.6875rem; text-transform: uppercase; letter-spacing: 0.5px; }

.pagination-info {
  text-align: center;
  margin-top: 0.75rem;
  font-size: 0.625rem;
  color: #475569;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-weight: 700;
}

/* ── Responsive ── */
@media (max-width: 640px) {
  .rooms-wrapper       { padding: 1.25rem 0.875rem 2rem; }
  .controls-bar        { margin-bottom: 0.5rem; }
  .search-box          { width: 100%; }
  .all-filters         { gap: 0.5rem; padding: 0.5rem; }
  .filter-section      { width: 100%; }
  .filter-section-title { display: none; }
  .mfilter-btn         { flex: 1; justify-content: center; font-size: 0.75rem; }
  .mfilter-clear       { width: 100%; margin-left: 0; }
  .rooms-list          { gap: 1.5rem; }
  .nav-label           { display: none; }
  .header-title        { font-size: 1.0625rem; }
}
</style>