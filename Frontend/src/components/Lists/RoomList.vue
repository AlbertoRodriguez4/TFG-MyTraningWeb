<script setup lang="ts">
import { useRoomStore } from '@/stores/RoomStore'
import { useUserRoomStore } from '@/stores/UsersRoomStore'
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import CreateRoomPopup from '../PopUps/RoomPopup.vue'
import { useUserStore } from '@/stores/userStore'
import EditRoomPopup from '../PopUps/EditRoomPopup.vue'
import RoomCard from './RoomCard.vue'

const store = useRoomStore()
const userRoomStore = useUserRoomStore()
const userStore = useUserStore()
const router = useRouter()
const loggedUser = ref(userStore.loggedUser)

const sortField = ref<'level' | 'stats' | null>(null)
const sortDirection = ref<'asc' | 'desc'>('asc')
const searchTerm = ref('')
const selectedRoom = defineModel<{
    id: number; name: string; minlevel: number; minstats: number
    minconsistency: number; description: string; date: string; localization: string
} | null>('selectedItem')
const showRoomPopup = ref(false)
const isPopupVisible = ref(false)

const currentPage = ref(1)
const itemsPerPage = 6
const roomMemberCounts = ref<Map<number, number>>(new Map())

onMounted(async () => {
    await store.fetchRoom()
    await loadRoomMemberCounts()
})

watch(searchTerm, () => { currentPage.value = 1 })

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

function getMemberCount(roomId: number) { return roomMemberCounts.value.get(roomId) || 0 }

function toggleSort(field: 'level' | 'stats') {
    if (sortField.value === field) {
        sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
        sortField.value = field
        sortDirection.value = 'asc'
    }
    currentPage.value = 1
}

const filteredRooms = computed(() => {
    const term = searchTerm.value.toLowerCase()
    let result = store.room.filter(r => r.name.toLowerCase().includes(term))
    if (sortField.value) {
        const f = sortField.value === 'level' ? 'minlevel' : 'minstats'
        result.sort((a, b) => sortDirection.value === 'asc' ? a[f] - b[f] : b[f] - a[f])
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
    if (totalPages.value <= 5) {
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

function showPopup() { isPopupVisible.value = true }
function closePopup() { isPopupVisible.value = false }

async function handleCreateRoom(newRoom: any) {
    await store.createRoom(newRoom, loggedUser.value?.id ?? 0)
    await loadRoomMemberCounts()
    closePopup()
}

const openEditPopup = (room: any) => {
    selectedRoom.value = { ...room, localization: room.localization ?? '' }
    showRoomPopup.value = true
}

const closeRoomPopup = () => { showRoomPopup.value = false }

const goToRoom = (roomId: number) => {
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
                <div class="header-icon-wrap">🏛️</div>
                <div>
                    <h2 class="header-title">Salas de Entrenamiento</h2>
                    <p class="header-sub">{{ filteredRooms.length }} salas disponibles</p>
                </div>
            </div>
            <button @click="showPopup" class="create-btn">
                <span class="create-plus">+</span> Crear sala
            </button>
        </div>

        <!-- Controls -->
        <div class="controls-bar">
            <div class="search-box">
                <span class="search-icon">🔍</span>
                <input v-model="searchTerm" type="text" placeholder="Buscar sala..." class="search-input" />
            </div>
            <div class="sort-group">
                <button @click="toggleSort('level')" class="sort-btn" :class="{ active: sortField === 'level' }">
                    <span class="sort-icon">📊</span>
                    Nivel
                    <span v-if="sortField === 'level'" class="sort-arrow">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                    </span>
                </button>
                <button @click="toggleSort('stats')" class="sort-btn" :class="{ active: sortField === 'stats' }">
                    <span class="sort-icon">💪</span>
                    Stats
                    <span v-if="sortField === 'stats'" class="sort-arrow">
                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                    </span>
                </button>
            </div>
        </div>

        <!-- Divisor -->
        <div class="section-divider">
            <div class="divider-line"></div>
            <span class="divider-label">{{ paginatedRooms.length }} de {{ filteredRooms.length }}</span>
            <div class="divider-line"></div>
        </div>

        <!-- Empty state -->
        <div v-if="filteredRooms.length === 0" class="empty-state">
            <div class="empty-icon">🏋️‍♂️</div>
            <p class="empty-title">No se encontraron salas</p>
            <p class="empty-sub">Ajusta la búsqueda o crea una nueva sala</p>
        </div>

        <!-- Lista de cards -->
        <!-- gap grande para dar espacio a las capas del stack -->
        <div v-else class="rooms-list">
            <RoomCard v-for="room in paginatedRooms" :key="room.id" :room="room" :member-count="getMemberCount(room.id)"
                :is-staff="loggedUser?.role === 'userStaff'" @view="goToRoom" @edit="openEditPopup" />
        </div>

        <!-- Paginación -->
        <div v-if="filteredRooms.length > 0 && totalPages > 1" class="pagination">
            <button class="page-btn" :disabled="currentPage === 1" @click="previousPage">←</button>
            <button v-for="(page, i) in pageNumbers" :key="i" class="page-btn"
                :class="{ active: page === currentPage, ellipsis: page === '...' }" :disabled="page === '...'"
                @click="typeof page === 'number' ? goToPage(page) : null">{{ page }}</button>
            <button class="page-btn" :disabled="currentPage === totalPages" @click="nextPage">→</button>
        </div>

        <p v-if="filteredRooms.length > 0" class="pagination-info">
            {{ (currentPage - 1) * itemsPerPage + 1 }}–{{ Math.min(currentPage * itemsPerPage, filteredRooms.length) }}
            de {{ filteredRooms.length }} salas
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
    max-width: 920px;
    margin: 0 auto;
    padding: 2rem 1.5rem;
    font-family: 'Inter', system-ui, sans-serif;
    color: #e2e8f0;
}

/* ── Header ── */
.rooms-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1.5rem;
    gap: 1rem;
}

.header-left {
    display: flex;
    align-items: center;
    gap: 0.875rem;
}

.header-icon-wrap {
    font-size: 1.25rem;
    width: 44px;
    height: 44px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: rgba(129, 140, 248, 0.1);
    border: 1px solid rgba(129, 140, 248, 0.22);
    border-radius: 12px;
    box-shadow: 0 0 16px rgba(129, 140, 248, 0.1);
}

.header-title {
    font-size: 1.125rem;
    font-weight: 800;
    color: #f1f5f9;
    margin: 0 0 0.1rem;
    letter-spacing: -0.3px;
}

.header-sub {
    font-size: 0.6875rem;
    color: #334155;
    margin: 0;
    text-transform: uppercase;
    letter-spacing: 0.6px;
    font-weight: 600;
}

/* Botón crear */
.create-btn {
    display: flex;
    align-items: center;
    gap: 0.4rem;
    padding: 0.5rem 1.125rem;
    background: linear-gradient(135deg, rgba(129, 140, 248, 0.9), rgba(99, 102, 241, 0.8));
    color: #fff;
    border: 1px solid rgba(129, 140, 248, 0.4);
    border-radius: 10px;
    font-size: 0.8125rem;
    font-weight: 700;
    cursor: pointer;
    transition: all 0.22s ease;
    box-shadow: 0 3px 14px rgba(99, 102, 241, 0.3);
    white-space: nowrap;
    letter-spacing: 0.2px;
}

.create-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 22px rgba(99, 102, 241, 0.45);
    filter: brightness(1.08);
}

.create-plus {
    font-size: 1.125rem;
    font-weight: 400;
    line-height: 1;
}

/* ── Controls ── */
.controls-bar {
    display: flex;
    gap: 0.625rem;
    margin-bottom: 1.25rem;
    flex-wrap: wrap;
}

.search-box {
    flex: 1;
    min-width: 180px;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(255, 255, 255, 0.03);
    border: 1px solid rgba(255, 255, 255, 0.08);
    border-radius: 10px;
    padding: 0.5625rem 0.875rem;
    transition: all 0.2s;
}

.search-box:focus-within {
    border-color: rgba(129, 140, 248, 0.45);
    box-shadow: 0 0 0 3px rgba(129, 140, 248, 0.1);
    background: rgba(129, 140, 248, 0.04);
}

.search-icon {
    font-size: 0.75rem;
    opacity: 0.35;
}

.search-input {
    border: none;
    outline: none;
    background: transparent;
    font-size: 0.8125rem;
    color: #e2e8f0;
    width: 100%;
}

.search-input::placeholder {
    color: #1e293b;
}

.sort-group {
    display: flex;
    gap: 0.4rem;
}

.sort-btn {
    display: flex;
    align-items: center;
    gap: 0.35rem;
    padding: 0.5rem 0.875rem;
    background: rgba(255, 255, 255, 0.03);
    border: 1px solid rgba(255, 255, 255, 0.08);
    border-radius: 10px;
    font-size: 0.75rem;
    font-weight: 600;
    color: #334155;
    cursor: pointer;
    transition: all 0.2s;
    white-space: nowrap;
}

.sort-btn:hover {
    border-color: rgba(255, 255, 255, 0.16);
    color: #64748b;
}

.sort-btn.active {
    background: rgba(129, 140, 248, 0.12);
    border-color: rgba(129, 140, 248, 0.35);
    color: #818cf8;
}

.sort-icon {
    font-size: 0.6875rem;
}

.sort-arrow {
    font-weight: 900;
}

/* ── Divisor ── */
.section-divider {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1.5rem;
}

.divider-line {
    flex: 1;
    height: 1px;
    background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.06), transparent);
}

.divider-label {
    font-size: 0.5625rem;
    text-transform: uppercase;
    letter-spacing: 1.2px;
    color: #1e293b;
    font-weight: 700;
    white-space: nowrap;
}

/* ── Empty ── */
.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    padding: 4rem 2rem;
    color: #334155;
}

.empty-icon {
    font-size: 2.5rem;
    opacity: 0.4;
}

.empty-title {
    font-size: 0.9375rem;
    font-weight: 700;
    color: #475569;
    margin: 0;
}

.empty-sub {
    font-size: 0.75rem;
    color: #1e293b;
    margin: 0;
}

/* ── Lista — gap amplio para las capas del stack ── */
.rooms-list {
    display: flex;
    flex-direction: column;
    /* 1.75rem entre stacks: suficiente para que las capas no se solapan */
    gap: 1.75rem;
}

/* ── Paginación ── */
.pagination {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.3rem;
    margin-top: 2.5rem;
}

.page-btn {
    min-width: 32px;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: rgba(255, 255, 255, 0.03);
    border: 1px solid rgba(255, 255, 255, 0.08);
    border-radius: 8px;
    font-size: 0.75rem;
    font-weight: 600;
    color: #334155;
    cursor: pointer;
    transition: all 0.2s;
}

.page-btn:hover:not(:disabled):not(.ellipsis) {
    border-color: rgba(129, 140, 248, 0.45);
    color: #818cf8;
    background: rgba(129, 140, 248, 0.08);
}

.page-btn.active {
    background: linear-gradient(135deg, rgba(129, 140, 248, 0.9), rgba(99, 102, 241, 0.8));
    color: #fff;
    border-color: transparent;
    box-shadow: 0 2px 10px rgba(99, 102, 241, 0.35);
}

.page-btn:disabled {
    opacity: 0.25;
    cursor: not-allowed;
}

.page-btn.ellipsis {
    border: none;
    cursor: default;
    background: transparent;
    color: #1e293b;
}

.pagination-info {
    text-align: center;
    margin-top: 0.625rem;
    font-size: 0.6875rem;
    color: #1e293b;
    text-transform: uppercase;
    letter-spacing: 0.6px;
    font-weight: 600;
}

/* ── Responsive ── */
@media (max-width: 600px) {
    .rooms-wrapper {
        padding: 1.25rem 0.875rem;
    }

    .controls-bar {
        flex-direction: column;
    }

    .search-box {
        min-width: 100%;
    }

    .sort-group {
        width: 100%;
    }

    .sort-btn {
        flex: 1;
        justify-content: center;
    }

    .rooms-list {
        gap: 1.5rem;
    }
}
</style>