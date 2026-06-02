<script setup lang="ts">
import { useRoomStore } from '@/stores/RoomStore'
import { useUserRoomStore } from '@/stores/UsersRoomStore'
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import CreateRoomPopup from '../PopUps/RoomPopup.vue'
import { useUserStore } from '@/stores/userStore'
import EditRoomPopup from '../PopUps/EditRoomPopup.vue'
import RoomCard from '../Cards/RoomCard.vue'

const { t } = useI18n()

const store = useRoomStore()
const userRoomStore = useUserRoomStore()
const userStore = useUserStore()
const router = useRouter()
const loggedUser = ref(userStore.loggedUser)

const sortField = ref<'level' | 'stats' | null>(null)
const sortDirection = ref<'asc' | 'desc'>('asc')
const searchTerm = ref('')
const activeFilter = ref<'joinable' | 'joined' | 'trainer' | null>(null)
// selectedRoom se usa para pasar la sala a editar al EditRoomPopup
const selectedRoom = defineModel<{
  id: number; name: string; minlevel: number; minstats: number
  minconsistency: number; description: string; date: string; localization: string
} | null>('selectedItem')
const showRoomPopup = ref(false)
const isPopupVisible = ref(false)

const currentPage = ref(1)
const itemsPerPage = 9

const roomMemberCounts = ref<Map<number, number>>(new Map())
const joinedRoomIds = ref<Set<number>>(new Set())
// Escucha cambios en la membresía de las salas para actualizar la lista en tiempo real
const handleMembershipChange = async () => {
  await loadJoinedRooms()
  await loadRoomMemberCounts()
}
// Recarga datos al volver a enfocar la ventana, útil para reflejar cambios hechos en otras pestañas o ventanas
const handleFocus = async () => {
  await loadJoinedRooms()
  await loadRoomMemberCounts()
}
// Obtener las salas junto con los miembros unidos
onMounted(async () => {
  await store.fetchRoom()
  await loadRoomMemberCounts()
  await loadJoinedRooms()

  window.addEventListener('room-membership-changed', handleMembershipChange)
  window.addEventListener('focus', handleFocus)
})

onUnmounted(() => {
  window.removeEventListener('room-membership-changed', handleMembershipChange)
  window.removeEventListener('focus', handleFocus)
})

watch(searchTerm, () => { currentPage.value = 1 })
watch(activeFilter, () => { currentPage.value = 1 })
// Cargar los usuarios que se encuentran dentro de cada sala en base al id de la propia sala
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
// Cargar las salas en las que el usuario se encuentra unido para poder mostrarlo en la interfaz y aplicar filtros correctamente
async function loadJoinedRooms() {
  if (!loggedUser.value?.id) return
  try {
    const ids = new Set<number>()
    for (const room of store.room) {
      await userRoomStore.fetchMembersByRoomId(room.id)
      const members = userRoomStore.currentRoomMembers
      // Si el usuario actual se encuentra dentro de los miembros de la sala, se agrega el id de la sala al conjunto de salas unidas
      if (members.some((m) => m.userid === loggedUser.value?.id)) {
        ids.add(room.id)
      }
    }
    joinedRoomIds.value = ids
  } catch (e) {
    console.error('Error cargando salas unidas', e)
  }
}
// Obtener el conteo de miembros para una sala específica, utilizando el mapa de conteos cargado previamente
function getMemberCount(roomId: number) {
  return roomMemberCounts.value.get(roomId) || 0
}

function isJoined(roomId: number) {
  return joinedRoomIds.value.has(roomId)
}
// Filtro para determinar si el usuario cumple con los requisitos para unirse a una sala específica, basado en su nivel, estadísticas y racha de consistencia
function canJoin(room: { minlevel: number; minstats: number; minconsistency: number }) {
  const u = loggedUser.value
  if (!u) return false
  const level = u.level ?? 0
  const stats = (u.strength ?? 0) + (u.endurance ?? 0)
  const consistency = u.consistencyStreak ?? 0
  return level >= room.minlevel && stats >= room.minstats && consistency >= room.minconsistency
}
// Función para alternar el orden de clasificación por nivel o estadísticas, cambiando entre ascendente y descendente al hacer clic repetidamente en el mismo campo
function toggleSort(field: 'level' | 'stats') {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
  currentPage.value = 1
}

function toggleFilter(filter: 'joinable' | 'joined' | 'trainer') {
  activeFilter.value = activeFilter.value === filter ? null : filter
}
// Función para limpiar todos los filtros y volver a la vista predeterminada, restableciendo el campo de búsqueda, ordenación y filtros activos
function clearAllFilters() {
  activeFilter.value = null
  sortField.value = null
  sortDirection.value = 'asc'
  searchTerm.value = ''
  currentPage.value = 1
}
// Función computada que devuelve la lista de salas filtradas y ordenadas según el término de búsqueda, el campo de ordenación seleccionado y el filtro activo 
// (unido, disponible para unirse o creador entrenador)
const filteredRooms = computed(() => {
  const term = searchTerm.value.toLowerCase()
  let result = store.room.filter(r => r.name.toLowerCase().includes(term))
  // Si se ha seleccionado un filtro, se ordena la lista de salas según el campo seleccionado (nivel o estadísticas) y la dirección de ordenación (ascendente o descendente)
  if (sortField.value) {
    const f = sortField.value === 'level' ? 'minlevel' : 'minstats'
    result = [...result].sort((a, b) =>
      sortDirection.value === 'asc' ? a[f] - b[f] : b[f] - a[f]
    )
  }
// Si el filtro seleccionado es el de que el usuario esta unido a la sala, se filtran las salas para mostrar solo aquellas en las que el usuario esta unido. 
  if (activeFilter.value === 'joined') {
    result = result.filter(r => isJoined(r.id))
    // Si el filtro es el de salas disponibles para unirse, se muestran solo aquellas en las que el usuario no esta unido pero cumple con los requisitos para unirse. 
  } else if (activeFilter.value === 'joinable') {
    result = result.filter(r => !isJoined(r.id) && canJoin(r))
    // Si el filtro es el de creador entrenador, se muestran solo aquellas salas cuyo creador tiene el rol de 'userStaff'
  } else if (activeFilter.value === 'trainer') {
    result = result.filter(r => r.creatorRole === 'userStaff')
  }

  return result
})
// Paginador que calcula el número total de páginas en función de la cantidad de salas filtradas y el número de elementos por página, 
// y devuelve la lista de salas correspondientes a la página actual
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
  // Lógica para mostrar un máximo de 5 números de página en la paginación, con puntos suspensivos para indicar páginas adicionales cuando el total de páginas es 
  // mayor que el máximo permitido.
  if (totalPages.value <= max) {
    for (let i = 1; i <= totalPages.value; i++) pages.push(i)
  } else if (currentPage.value <= 3) { // Si la página actual es una de las primeras 3, se muestran las primeras 4 páginas y luego puntos suspensivos seguidos del número de la última página.
    for (let i = 1; i <= 4; i++) pages.push(i)
    pages.push('...'); pages.push(totalPages.value)
  } else if (currentPage.value >= totalPages.value - 2) {// Si la página actual es una de las últimas 3, se muestran el número de la primera página seguido de puntos suspensivos y luego las últimas 4 páginas.
    pages.push(1); pages.push('...')
    for (let i = totalPages.value - 3; i <= totalPages.value; i++) pages.push(i)
  } else { // Si la página actual está en el medio, se muestran el número de la primera página seguido de puntos suspensivos, luego la página anterior, la página actual y la página siguiente, seguidos de puntos suspensivos y el número de la última página.
    pages.push(1); pages.push('...')
    for (let i = currentPage.value - 1; i <= currentPage.value + 1; i++) pages.push(i)
    pages.push('...'); pages.push(totalPages.value)
  }
  return pages
})

function showPopup() { isPopupVisible.value = true }
function closePopup() { isPopupVisible.value = false }

async function handleCreateRoom(newRoom: any) {
  await store.createRoom(newRoom, loggedUser.value?.id ?? 0, loggedUser.value?.role)
  await loadRoomMemberCounts()
  closePopup()
}

function openEditPopup(room: any) {
  selectedRoom.value = { ...room, localization: room.localization ?? '' }
  showRoomPopup.value = true
}

function closeRoomPopup() { showRoomPopup.value = false }
// Función para navegar a la página de detalles de una sala específica, pasando el id de la sala como parámetro en la ruta y también como query para facilitar el 
// acceso a los datos de la sala en la página de destino sin necesidad de hacer una nueva consulta al backend.
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
    <!-- Header -->
    <div class="rooms-header">
      <div class="header-left">
        <div class="header-icon">
          <span><v-icon>mdi-bank</v-icon></span>
        </div>
        <div class="header-text">
          <h2 class="header-title">{{ $t('training_rooms') }}</h2>
          <p class="header-sub">
            <span class="sub-dot"></span>
            {{ $t('rooms_available', { count: filteredRooms.length }) }}
          </p>
        </div>
      </div>
      <button @click="showPopup" class="create-btn">
        <span class="create-plus">+</span>
        <span>{{ $t('create_room') }}</span>
      </button>
    </div>

    <!-- Divider -->
    <div class="header-rule">
      <div class="rule-line"></div>
      <div class="rule-glow"></div>
    </div>

    <!-- Controls -->
    <div class="controls-section">
      <!-- Search -->
      <div class="search-box">
        <span class="search-icon"><v-icon>mdi-magnify</v-icon></span>
        <input v-model="searchTerm" type="text" :placeholder="$t('search_room')" class="search-input" />
        <span v-if="searchTerm" class="search-clear" @click="searchTerm = ''"><v-icon>mdi-close</v-icon></span>
      </div>

      <!-- Filters Bar -->
      <div class="filters-bar">
        <div class="filter-group">
          <span class="filter-group-label">{{ $t('sort_by') }}</span>
          <button class="filter-chip sort-chip" :class="{ active: sortField === 'level' }" @click="toggleSort('level')">
            <span class="chip-icon"><v-icon>mdi-chart-bar</v-icon></span>
            <span class="chip-text">{{ $t('level_label') }}</span>
            <span v-if="sortField === 'level'" class="sort-arrow">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </span>
          </button>
          <button class="filter-chip sort-chip" :class="{ active: sortField === 'stats' }" @click="toggleSort('stats')">
            <span class="chip-icon"><v-icon>mdi-arm-flex</v-icon></span>
            <span class="chip-text">{{ $t('stats_label') }}</span>
            <span v-if="sortField === 'stats'" class="sort-arrow">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </span>
          </button>
        </div>

        <div class="filter-group">
          <span class="filter-group-label">{{ $t('filter_by') }}</span>
          <button class="filter-chip filter-joined" :class="{ active: activeFilter === 'joined' }"
            @click="toggleFilter('joined')">
            <span class="chip-icon"><v-icon>mdi-check-circle</v-icon></span>
            <span class="chip-text">{{ $t('joined_rooms') }}</span>
          </button>
          <button class="filter-chip filter-joinable" :class="{ active: activeFilter === 'joinable' }"
            @click="toggleFilter('joinable')">
            <span class="chip-icon"><v-icon>mdi-rocket</v-icon></span>
            <span class="chip-text">{{ $t('available_rooms') }}</span>
          </button>
          <button class="filter-chip filter-trainer" :class="{ active: activeFilter === 'trainer' }"
            @click="toggleFilter('trainer')">
            <span class="chip-icon"><v-icon>mdi-medal</v-icon></span>
            <span class="chip-text">{{ $t('trainer_rooms') }}</span>
          </button>
        </div>

        <button class="filter-chip filter-clear" @click="clearAllFilters">
          <span class="chip-icon"><v-icon>mdi-broom</v-icon></span>
          <span class="chip-text">{{ $t('clear_filters') }}</span>
        </button>
      </div>
    </div>

    <!-- Results Counter -->
    <div class="results-bar">
      <div class="results-line"></div>
      <span class="results-label">
        {{ paginatedRooms.length }} / {{ filteredRooms.length }} {{ $t('rooms') }}
      </span>
      <div class="results-line"></div>
    </div>

    <!-- Empty State -->
    <div v-if="filteredRooms.length === 0" class="empty-state">
      <div class="empty-emoji">
        <v-icon>{{ activeFilter === 'joined' ? 'mdi-lock' : activeFilter === 'joinable' ? 'mdi-boom-gate' : activeFilter === 'trainer' ? 'mdi-medal' : 'mdi-weight-lifter' }}</v-icon>
      </div>
      <p class="empty-title">
        {{
          activeFilter === 'joined' ? $t('not_in_any_room') :
            activeFilter === 'joinable' ? $t('no_requirements_met') :
              activeFilter === 'trainer' ? $t('no_trainer_rooms') :
                $t('no_results')
        }}
      </p>
      <p class="empty-sub">
        {{
          activeFilter === 'joined' ? $t('join_room_to_see') :
            activeFilter === 'joinable' ? $t('keep_training') :
              activeFilter === 'trainer' ? $t('no_trainer_rooms_desc') :
                $t('adjust_search')
        }}
      </p>
    </div>

    <!-- Grid de Salas -->
    <TransitionGroup v-else name="room-grid" tag="div" class="rooms-grid" appear>
      <RoomCard v-for="room in paginatedRooms" :key="room.id" :room="{ ...room, localization: room.localization ?? '' }"
        :member-count="getMemberCount(room.id)" :is-staff="loggedUser?.role === 'userStaff'"
        :is-admin="loggedUser?.role === 'userMaster'" :is-joined="isJoined(room.id)"
        :creator-role="room.creatorRole" @view="goToRoom" @edit="openEditPopup" />
    </TransitionGroup>

    <!-- Paginación -->
    <div v-if="filteredRooms.length > 0 && totalPages > 1" class="pagination">
      <button class="page-btn page-nav" :disabled="currentPage === 1" @click="previousPage">
        <span class="nav-arrow">←</span>
        <span class="nav-label">{{ $t('previous') }}</span>
      </button>
      <div class="page-numbers">
        <button v-for="(page, i) in pageNumbers" :key="i" class="page-btn"
          :class="{ active: page === currentPage, ellipsis: page === '...' }" :disabled="page === '...'"
          @click="typeof page === 'number' ? goToPage(page) : null">{{ page }}</button>
      </div>
      <button class="page-btn page-nav" :disabled="currentPage === totalPages" @click="nextPage">
        <span class="nav-label">{{ $t('next') }}</span>
        <span class="nav-arrow">→</span>
      </button>
    </div>

    <p v-if="filteredRooms.length > 0" class="pagination-info">
      {{ $t('rooms_pagination', {
        start: (currentPage - 1) * itemsPerPage + 1,
        end: Math.min(currentPage * itemsPerPage, filteredRooms.length),
        total: filteredRooms.length
      }) }}
    </p>

    <!-- Popups -->
    <CreateRoomPopup :isVisible="isPopupVisible" @close="closePopup" @create="handleCreateRoom" />
    <EditRoomPopup v-if="selectedRoom" :isVisible="showRoomPopup" :room="selectedRoom" @close="closeRoomPopup"
      @edit="(room) => console.log('Editando', room)" />
  </div>
</template>

<style scoped>
.rooms-wrapper {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1.5rem 3rem;
  font-family: 'Inter', system-ui, sans-serif;
  color: var(--text-primary);
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
  font-size: 1.75rem;
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.15), rgba(245, 158, 11, 0.1));
  border: 1px solid rgba(251, 191, 36, 0.35);
  border-radius: 16px;
  box-shadow: 0 0 24px rgba(251, 191, 36, 0.15), inset 0 0 16px rgba(251, 191, 36, 0.08);
  flex-shrink: 0;
  animation: icon-float 3s ease-in-out infinite;
}

@keyframes icon-float {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-4px);
  }
}

.header-title {
  font-size: 1.5rem;
  font-weight: 900;
  color: var(--text-primary);
  margin: 0 0 0.2rem;
  letter-spacing: -0.5px;
}

.header-sub {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.75rem;
  color: var(--text-muted);
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  font-weight: 700;
}

.sub-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #fbbf24;
  box-shadow: 0 0 8px #fbbf24;
  animation: sdot 2s ease-in-out infinite;
}

@keyframes sdot {

  0%,
  100% {
    opacity: 1;
    transform: scale(1);
  }

  50% {
    opacity: 0.3;
    transform: scale(0.6);
  }
}

.create-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border: 1px solid rgba(251, 191, 36, 0.5);
  border-radius: 12px;
  color: #0f172a;
  font-size: 0.875rem;
  font-weight: 800;
  cursor: pointer;
  transition: all 0.25s ease;
  box-shadow: 0 4px 20px rgba(251, 191, 36, 0.4);
  white-space: nowrap;
}

.create-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 28px rgba(251, 191, 36, 0.55);
  filter: brightness(1.15);
}

.create-plus {
  font-size: 1.25rem;
  line-height: 1;
  font-weight: 300;
}

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
  background: linear-gradient(90deg, transparent, rgba(251, 191, 36, 0.35) 30%, rgba(251, 191, 36, 0.35) 70%, transparent);
}

.rule-glow {
  position: absolute;
  top: -2px;
  left: 50%;
  transform: translateX(-50%);
  width: 140px;
  height: 5px;
  background: radial-gradient(ellipse, rgba(251, 191, 36, 0.6), transparent 70%);
  filter: blur(3px);
}

/* ── Controls ── */
.controls-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 14px;
  padding: 0.75rem 1rem;
  transition: all 0.3s ease;
  max-width: 480px;
  width: 100%;
}

.search-box:focus-within {
  border-color: rgba(251, 191, 36, 0.5);
  background: rgba(251, 191, 36, 0.04);
  box-shadow: 0 0 0 3px rgba(251, 191, 36, 0.1);
}

.search-icon {
  font-size: 0.875rem;
  opacity: 0.5;
  flex-shrink: 0;
}

.search-input {
  flex: 1;
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.9375rem;
  color: var(--text-primary);
  min-width: 0;
  font-family: inherit;
}

.search-input::placeholder {
  color: var(--text-muted);
}

.search-clear {
  font-size: 0.75rem;
  color: #64748b;
  cursor: pointer;
  padding: 0.2rem 0.35rem;
  border-radius: 6px;
  transition: all 0.2s;
  flex-shrink: 0;
}

.search-clear:hover {
  color: #94a3b8;
  background: rgba(255, 255, 255, 0.06);
}

/* Filters Bar */
.filters-bar {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.75rem;
  padding: 0.875rem 1.125rem;
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 14px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.filter-group-label {
  font-size: 0.6875rem;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: var(--text-muted);
  font-weight: 700;
  margin-right: 0.25rem;
}

.filter-chip {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  padding: 0.5rem 0.875rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  font-size: 0.8125rem;
  font-weight: 700;
  color: #94a3b8;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
  font-family: inherit;
}

.filter-chip:hover {
  color: #e2e8f0;
  border-color: rgba(255, 255, 255, 0.2);
  background: rgba(255, 255, 255, 0.07);
  transform: translateY(-1px);
}

/* Sort chips */
.sort-chip {
  background: rgba(139, 92, 246, 0.08);
  border-color: rgba(139, 92, 246, 0.25);
  color: #c4b5fd;
}

.sort-chip:hover {
  background: rgba(139, 92, 246, 0.14);
  border-color: rgba(139, 92, 246, 0.4);
}

.sort-chip.active {
  background: rgba(139, 92, 246, 0.2);
  border-color: rgba(139, 92, 246, 0.5);
  color: #a78bfa;
  box-shadow: 0 0 0 1px rgba(139, 92, 246, 0.2), 0 4px 16px rgba(139, 92, 246, 0.15);
}

.sort-arrow {
  font-weight: 900;
  font-size: 0.875rem;
  margin-left: 0.1rem;
}

/* Filter Joined */
.filter-joined {
  background: rgba(56, 189, 248, 0.08);
  border-color: rgba(56, 189, 248, 0.25);
  color: #7dd3fc;
}

.filter-joined:hover {
  background: rgba(56, 189, 248, 0.14);
  border-color: rgba(56, 189, 248, 0.4);
}

.filter-joined.active {
  background: rgba(56, 189, 248, 0.2);
  border-color: rgba(56, 189, 248, 0.5);
  color: #38bdf8;
  box-shadow: 0 0 0 1px rgba(56, 189, 248, 0.2), 0 4px 16px rgba(56, 189, 248, 0.15);
}

/* Filter Joinable */
.filter-joinable {
  background: rgba(52, 211, 153, 0.08);
  border-color: rgba(52, 211, 153, 0.25);
  color: #6ee7b7;
}

.filter-joinable:hover {
  background: rgba(52, 211, 153, 0.14);
  border-color: rgba(52, 211, 153, 0.4);
}

.filter-joinable.active {
  background: rgba(52, 211, 153, 0.2);
  border-color: rgba(52, 211, 153, 0.5);
  color: #34d399;
  box-shadow: 0 0 0 1px rgba(52, 211, 153, 0.2), 0 4px 16px rgba(52, 211, 153, 0.15);
}

/* Filter Trainer */
.filter-trainer {
  background: rgba(251, 191, 36, 0.08);
  border-color: rgba(251, 191, 36, 0.25);
  color: #fbbf24;
}

.filter-trainer:hover {
  background: rgba(251, 191, 36, 0.14);
  border-color: rgba(251, 191, 36, 0.4);
}

.filter-trainer.active {
  background: rgba(251, 191, 36, 0.2);
  border-color: rgba(251, 191, 36, 0.5);
  color: #fbbf24;
  box-shadow: 0 0 0 1px rgba(251, 191, 36, 0.2), 0 4px 16px rgba(251, 191, 36, 0.15);
}

/* Clear */
.filter-clear {
  margin-left: auto;
  background: rgba(239, 68, 68, 0.08);
  border-color: rgba(239, 68, 68, 0.25);
  color: #f87171;
}

.filter-clear:hover {
  background: rgba(239, 68, 68, 0.15);
  border-color: rgba(239, 68, 68, 0.45);
  color: #ef4444;
}

.chip-icon {
  font-size: 0.875rem;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
}

.chip-text {
  letter-spacing: 0.3px;
  text-transform: uppercase;
  font-size: 0.7rem;
}

/* ── Results Bar ── */
.results-bar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.results-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.08), transparent);
}

.results-label {
  font-size: 0.625rem;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  color: var(--text-muted);
  font-weight: 700;
  white-space: nowrap;
}

/* ── Empty State ── */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 5rem 2rem;
  text-align: center;
}

.empty-emoji {
  font-size: 4rem;
  opacity: 0.4;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.3));
  animation: empty-bounce 3s ease-in-out infinite;
}

@keyframes empty-bounce {

  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-8px);
  }
}

.empty-title {
  font-size: 1.125rem;
  font-weight: 800;
  color: var(--text-secondary);
  margin: 0;
}

.empty-sub {
  font-size: 0.875rem;
  color: var(--text-muted);
  margin: 0;
  max-width: 320px;
  line-height: 1.6;
}

/* ── Grid ── */
.rooms-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 1.5rem;
}

/* Transiciones del grid */
.room-grid-move,
.room-grid-enter-active,
.room-grid-leave-active {
  transition: all 0.45s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.room-grid-enter-from,
.room-grid-leave-to {
  opacity: 0;
  transform: translateY(30px) scale(0.95);
}

.room-grid-leave-active {
  position: absolute;
}

/* ── Paginación ── */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 2.5rem;
}

.page-numbers {
  display: flex;
  gap: 0.35rem;
}

.page-btn {
  min-width: 38px;
  height: 38px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.3rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  font-size: 0.8125rem;
  font-weight: 700;
  color: var(--text-secondary);
  cursor: pointer;
  transition: all 0.25s ease;
  font-family: 'Inter', system-ui, sans-serif;
}

.page-btn:hover:not(:disabled):not(.ellipsis) {
  border-color: rgba(251, 191, 36, 0.5);
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.08);
  transform: translateY(-1px);
}

.page-btn.active {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border-color: transparent;
  color: #0f172a;
  font-weight: 900;
  box-shadow: 0 2px 16px rgba(251, 191, 36, 0.5);
  transform: scale(1.08);
}

.page-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.page-btn.ellipsis {
  border: none;
  background: transparent;
  color: #475569;
  cursor: default;
}

.page-nav {
  padding: 0 1rem;
  min-width: auto;
  gap: 0.4rem;
}

.nav-arrow {
  font-size: 0.875rem;
}

.nav-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.pagination-info {
  text-align: center;
  margin-top: 1rem;
  font-size: 0.6875rem;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 1px;
  font-weight: 700;
}

/* ── Responsive ── */
@media (max-width: 768px) {
  .rooms-wrapper {
    padding: 1.25rem 1rem 2rem;
  }

  .rooms-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .create-btn {
    width: 100%;
    justify-content: center;
  }

  .filters-bar {
    gap: 0.5rem;
    padding: 0.625rem;
  }

  .filter-group {
    width: 100%;
  }

  .filter-group-label {
    display: none;
  }

  .filter-chip {
    flex: 1;
    justify-content: center;
    font-size: 0.75rem;
  }

  .filter-clear {
    width: 100%;
    margin-left: 0;
  }

  .rooms-grid {
    grid-template-columns: 1fr;
  }

  .page-nav .nav-label {
    display: none;
  }

  .header-title {
    font-size: 1.25rem;
  }
}

@media (max-width: 480px) {
  .rooms-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .search-box {
    max-width: 100%;
  }
}
</style>
