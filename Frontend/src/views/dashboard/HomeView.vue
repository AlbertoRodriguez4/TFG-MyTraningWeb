<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useMapStore } from '@/stores/mapStore';
import { useUserStore } from '@/stores/userStore';
import { useI18n } from 'vue-i18n';
import * as L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { logger } from '@/utils/logger';
import logoImg from '@/assets/imgs/Logo.png';

const { t } = useI18n();

const mapStore = useMapStore();
const userStore = useUserStore();
const map = ref<L.Map | null>(null);
const markers = ref<L.Marker[]>([]);
const searchAddress = ref('');
const loading = ref(false);
const gymsFound = ref<any[]>([]);
const selectedGym = ref<any>(null);
const showMap = ref(false);

const searchExamples = ['Gran Vía Madrid', 'Calle Alcalá', 'Paseo de Gracia Barcelona', 'Avenida de la Constitución Sevilla'];

const searchHistory = ref<string[]>([]);
const showSuggestions = ref(false);

const searchSuggestions = computed(() => {
  if (!searchAddress.value.trim()) return searchHistory.value.slice(0, 5);
  const query = searchAddress.value.toLowerCase();
  return searchHistory.value
    .filter(h => h.toLowerCase().includes(query))
    .slice(0, 5);
});

function addToHistory(address: string) {
  if (!address.trim()) return;
  searchHistory.value = [address, ...searchHistory.value.filter(h => h !== address)].slice(0, 10);
}

function selectSuggestion(suggestion: string) {
  searchAddress.value = suggestion;
  showSuggestions.value = false;
  searchByAddress();
}

function hideSuggestions() {
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
}

const communityStats = ref({ totalUsers: 0, activeToday: 0, totalCheckIns: 0 });

function formatStatNumber(num: number): string {
  if (num >= 1_000_000) return (num / 1_000_000).toFixed(1).replace(/\.0$/, '') + 'M';
  if (num >= 1_000) return (num / 1_000).toFixed(1).replace(/\.0$/, '') + 'K';
  return num.toLocaleString();
}

const currentPage = ref(1);
const itemsPerPage = 8;

const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('success');

const showSnackbar = (message: string, color: string = 'success') => {
  snackbarMessage.value = message;
  snackbarColor.value = color;
  snackbar.value = true;
}

const totalPages = computed(() => Math.ceil(gymsFound.value.length / itemsPerPage));

const paginatedGyms = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return gymsFound.value.slice(start, end);
});

const userIcon = L.icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});

const gymIcon = L.icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});

onMounted(async () => {
  if (showMap.value) {
    initMap();
  }
  try {
    communityStats.value = await userStore.getCommunityStats();
  } catch (e) {
    logger.error('Error cargando estadísticas de comunidad:', e);
  }
});

watch(showMap, (newVal) => {
  if (newVal) {
    setTimeout(() => initMap(), 100);
  }
});

function initMap() {
  if (map.value) return;

  const mapElement = document.getElementById('map');
  if (!mapElement) return;

  map.value = L.map('map').setView([41.6488, -0.8891], 13);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
  }).addTo(map.value as L.Map);
  map.value.on('click', async (e: L.LeafletMouseEvent) => {
    await searchGymsNearLocation(e.latlng.lat, e.latlng.lng);
  });
}

async function searchByAddress() {
  if (!searchAddress.value.trim()) {
    showSnackbar('Por favor, introduce una dirección para buscar', 'warning');
    return;
  }

  loading.value = true;
  currentPage.value = 1;
  showSuggestions.value = false;
  
  try {
    const coords = await mapStore.getCooredadas(searchAddress.value);
    addToHistory(searchAddress.value);
    await searchGymsNearLocation(coords.lat, coords.lon);
    
    if (gymsFound.value.length === 0) {
      showSnackbar('No se encontraron gimnasios cerca de esta dirección. Prueba con otra ubicación.', 'info');
    }
  } catch (error) {
    logger.error(t('address_search_error'), error);
    showSnackbar(t('home_could_not_find_address'), 'error');
  } finally {
    loading.value = false;
  }
}

async function searchGymsNearLocation(lat: number, lon: number) {
  loading.value = true;

  markers.value.forEach(marker => marker.remove());
  markers.value = [];

  if (map.value) {
    const userMarker = L.marker([lat, lon], { icon: userIcon })
      .addTo(map.value as L.Map)
      .bindPopup(t('home_your_location'));
    markers.value.push(userMarker);
    map.value.setView([lat, lon], 14);
  }

  try {
    const gyms = await mapStore.getEstablecimientos(lat, lon, 'sports_centre');
    gymsFound.value = gyms;
    currentPage.value = 1;

    gyms.forEach((gym: any) => {
      if (gym.lat && gym.lon && map.value) {
        const marker = L.marker([parseFloat(gym.lat), parseFloat(gym.lon)], { icon: gymIcon })
          .addTo(map.value as L.Map)
          .bindPopup(`<strong>${gym.nombre || 'Gimnasio'}</strong><br>${gym.direccion || ''}<br>${gym.telefono ? '<i class="mdi mdi-phone"></i> ' + gym.telefono : ''}<br>${gym.sitioWeb ? '<i class="mdi mdi-web"></i> <a href="' + gym.sitioWeb + '" target="_blank">Web</a>' : ''}`);

        marker.on('click', () => {
          selectedGym.value = gym;
        });

        markers.value.push(marker);
      }
    });
  } catch (error) {
    logger.error('Error buscando gimnasios:', error);
  } finally {
    loading.value = false;
  }
}

function toggleMapView() {
  showMap.value = !showMap.value;
}

function changePage(page: number) {
  currentPage.value = page;
  const resultsElement = document.querySelector('.results-header-epic');
  if (resultsElement) {
    resultsElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
</script>

<template>
  <div class="home-view-wrapper">
    <div class="main-container">
      <v-container fluid class="hero-section fill-height d-flex flex-column justify-center align-center text-center px-4 px-sm-6 px-md-10">

        <!-- Hero panel -->
        <div v-if="!showMap" class="glass-panel pa-4 pa-sm-6 pa-md-8 rounded-xl elevation-10 mt-n12">
          <img :src="logoImg" alt="TheTrainingHub Logo" class="mb-4 hero-logo" />
          <h1 class="hero-title font-weight-bold mb-4 text-white">TheTrainingHub</h1>
          <p class="hero-description text-white mb-6 mb-md-8 font-weight-regular mx-auto">
            Descubre la primera plataforma que combina <strong class="text-primary">fitness social</strong>, <strong class="text-primary">gamificación</strong> y <strong class="text-primary">recompensas</strong>.
            <br/><br/>
            Entrena con amigos, conquista retos y sube de nivel como nunca antes.
          </p>

          <div class="d-flex justify-center flex-wrap gap-3 gap-md-4">
            <v-btn size="large" color="primary" class="font-weight-bold px-6 px-md-8 hero-btn" to="/register" rounded="pill">
              Comenzar Aventura
            </v-btn>
            <v-btn size="large" variant="outlined" color="white" class="font-weight-bold px-6 px-md-8 hero-btn" rounded="pill" @click="toggleMapView">
              <v-icon start>mdi-map-marker</v-icon>
              Buscar Gimnasios
            </v-btn>
          </div>
        </div>

        <!-- Map section — full-bleed wide panel -->
        <div v-else class="map-section glass-panel rounded-xl elevation-10 pa-6 mt-4">

          <!-- Header -->
          <div class="d-flex justify-space-between align-center mb-5">
            <div class="d-flex align-center gap-3">
              <v-icon color="primary" size="28">mdi-map-search</v-icon>
              <h2 class="text-h5 font-weight-bold text-white">Buscar Gimnasios</h2>
            </div>
            <v-btn icon="mdi-close" variant="text" color="white" @click="toggleMapView" />
          </div>

          <!-- Search bar -->
          <div class="search-wrapper mb-4">
            <v-text-field
              v-model="searchAddress"
              placeholder="Ej: Cmo. de las Torrecillas, 14-18, 50014 Zaragoza"
              variant="solo"
              bg-color="rgba(255,255,255,0.08)"
              rounded="lg"
              append-inner-icon="mdi-magnify"
              @click:append-inner="searchByAddress"
              @keyup.enter="searchByAddress"
              @focus="showSuggestions = searchSuggestions.length > 0"
              @blur="hideSuggestions"
              class="search-field"
              hide-details
              :loading="loading"
            >
              <template #prepend-inner>
                <v-icon color="grey-lighten-1" class="mr-1">mdi-map-marker-outline</v-icon>
              </template>
            </v-text-field>

            <!-- Suggestions dropdown -->
            <div v-if="showSuggestions && searchSuggestions.length > 0" class="suggestions-dropdown">
              <div
                v-for="suggestion in searchSuggestions"
                :key="suggestion"
                class="suggestion-item"
                @mousedown.prevent="selectSuggestion(suggestion)"
              >
                <v-icon size="16" color="grey-lighten-1" class="mr-2">mdi-history</v-icon>
                <span>{{ suggestion }}</span>
              </div>
            </div>
          </div>

          <!-- Search examples -->
          <div class="d-flex flex-wrap align-center gap-2 mb-4">
            <span class="text-caption text-white" style="opacity: 0.5;">Prueba con:</span>
            <v-chip
              v-for="example in searchExamples"
              :key="example"
              size="x-small"
              variant="outlined"
              color="grey-lighten-2"
              class="search-example-chip"
              @click="searchAddress = example; searchByAddress()"
            >
              {{ example }}
            </v-chip>
          </div>

          <!-- Hint text -->
          <p class="text-caption text-white mb-4" style="opacity: 0.5;">
            <v-icon size="14" class="mr-1">mdi-information-outline</v-icon>
            Puedes buscar por calle completa con número y código postal, o haz clic en el mapa para buscar gimnasios cercanos.
          </p>

          <!-- Map -->
          <div class="map-wrapper mb-6">
            <div id="map" class="map-container rounded-xl"></div>
          </div>

          <!-- Results -->
          <div v-if="gymsFound.length > 0" class="results-section">
            <div class="results-header-epic d-flex align-center justify-space-between mb-5">
              <div class="d-flex align-center gap-3">
                <div class="results-badge">
                  <v-icon size="18" color="white">mdi-dumbbell</v-icon>
                  <span class="text-white font-weight-bold">{{ gymsFound.length }}</span>
                </div>
                <span class="text-subtitle-1 font-weight-medium text-white">
                  {{ gymsFound.length === 1 ? 'gimnasio encontrado' : 'gimnasios encontrados' }}
                </span>
              </div>
              <div class="page-indicator">
                <v-icon size="16" color="grey-lighten-1">mdi-book-open-page-variant</v-icon>
                <span>{{ currentPage }}</span>
                <span class="text-grey">/</span>
                <span>{{ totalPages }}</span>
              </div>
            </div>

            <v-row dense>
              <v-col
                v-for="gym in paginatedGyms"
                :key="gym.id"
                cols="12"
                sm="6"
                md="4"
                lg="3"
              >
                <v-card
                  class="gym-card"
                  :class="{ 'gym-card--selected': selectedGym?.id === gym.id }"
                  elevation="0"
                  rounded="lg"
                  @click="selectedGym = gym"
                >
                  <div class="gym-card__glow" />
                  
                  <v-card-text class="pa-4">
                    <div class="d-flex align-start justify-space-between mb-3">
                      <div class="gym-card__icon-wrapper">
                        <v-icon size="20" color="primary">mdi-dumbbell</v-icon>
                      </div>
                      <v-chip 
                        v-if="selectedGym?.id === gym.id" 
                        size="x-small" 
                        color="primary" 
                        variant="flat"
                        class="font-weight-bold"
                      >
                        Seleccionado
                      </v-chip>
                    </div>

                    <span class="gym-card__name text-subtitle-1 font-weight-bold d-block mb-2">
                      {{ gym.nombre || 'Gimnasio' }}
                    </span>

                    <div class="d-flex align-start gap-2 mb-3">
                      <v-icon size="14" color="grey-lighten-1" style="margin-top:3px; flex-shrink:0;">mdi-map-marker</v-icon>
                      <span class="text-caption gym-card__address">
                        {{ gym.direccion || 'Dirección no disponible' }}
                      </span>
                    </div>

                    <v-divider class="mb-3" style="border-color: rgba(255,255,255,0.08);" />

                    <div class="d-flex flex-column gap-2">
                      <div v-if="gym.telefono" class="d-flex align-center gap-2">
                        <div class="gym-card__contact-icon">
                          <v-icon size="12" color="primary">mdi-phone</v-icon>
                        </div>
                        <a :href="`tel:${gym.telefono}`" class="text-caption gym-card__link">
                          {{ gym.telefono }}
                        </a>
                      </div>

                      <div v-if="gym.sitioWeb" class="d-flex align-center gap-2">
                        <div class="gym-card__contact-icon">
                          <v-icon size="12" color="primary">mdi-web</v-icon>
                        </div>
                        <a :href="gym.sitioWeb" target="_blank" rel="noopener" class="text-caption gym-card__link gym-card__link--web">
                          Visitar web
                        </a>
                      </div>
                    </div>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>

            <div v-if="totalPages > 1" class="pagination-wrapper mt-6">
              <v-btn
                icon="mdi-chevron-left"
                variant="text"
                size="small"
                :disabled="currentPage === 1"
                @click="changePage(currentPage - 1)"
                class="pagination-arrow"
              />
              
              <div class="pagination-dots">
                <button
                  v-for="page in totalPages"
                  :key="page"
                  class="pagination-dot"
                  :class="{ 'pagination-dot--active': currentPage === page }"
                  @click="changePage(page)"
                >
                  <span class="pagination-dot__number">{{ page }}</span>
                </button>
              </div>

              <v-btn
                icon="mdi-chevron-right"
                variant="text"
                size="small"
                :disabled="currentPage === totalPages"
                @click="changePage(currentPage + 1)"
                class="pagination-arrow"
              />
            </div>
          </div>

          <!-- Empty state -->
          <div v-else-if="!loading" class="empty-state text-center py-6">
            <v-icon size="48" color="grey" class="mb-3">mdi-map-marker-off-outline</v-icon>
            <p class="text-body-2 text-white" style="opacity:0.5;">
              Busca una zona para ver los gimnasios disponibles.
            </p>
          </div>
        </div>
      </v-container>

      <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4000" location="top">
        {{ snackbarMessage }}
      </v-snackbar>
    </div>
  </div>
</template>

<style scoped>
.main-container {
  min-height: 100vh;
  background: transparent !important;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hero-section {
  min-height: calc(100vh - 64px);
}

/* ─── Glass panels ─── */
.glass-panel {
  background: rgba(20, 20, 20, 0.4) !important;
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  max-width: 900px;
  width: 100%;
}

/* ─── Hero responsive ─── */
.hero-logo {
  height: 80px;
  width: auto;
  filter: drop-shadow(0 4px 6px rgba(0,0,0,0.3));
}

.hero-title {
  font-size: clamp(1.75rem, 5vw, 3rem);
  line-height: 1.2;
  word-break: break-word;
}

.hero-description {
  font-size: clamp(0.95rem, 2.5vw, 1.25rem);
  max-width: 650px;
  opacity: 0.9;
  line-height: 1.6;
}

.hero-btn {
  min-width: 160px;
  font-size: 0.9rem;
}

@media (min-width: 600px) {
  .hero-logo {
    height: 100px;
  }
  
  .hero-btn {
    min-width: 180px;
    font-size: 1rem;
  }
}

@media (min-width: 960px) {
  .hero-logo {
    height: 120px;
  }
  
  .hero-btn {
    min-width: 200px;
  }
}

.map-section {
  max-width: 1400px !important;
  width: 100%;
}

@media (max-width: 600px) {
  .map-section {
    margin: 0 8px;
  }
  
  .map-wrapper {
    height: 350px;
  }
  
  .results-header-epic {
    flex-direction: column;
    gap: 12px;
    align-items: flex-start !important;
  }
  
  .pagination-wrapper {
    flex-wrap: wrap;
  }
  
  .pagination-dots {
    order: 2;
    width: 100%;
    justify-content: center;
  }
  
  .pagination-arrow:first-child {
    order: 1;
  }
  
  .pagination-arrow:last-child {
    order: 3;
  }
}

/* ─── Search field ─── */
.search-wrapper {
  position: relative;
}

.search-field :deep(.v-field) {
  color: white;
}
.search-field :deep(.v-field input::placeholder) {
  color: rgba(255, 255, 255, 0.4);
}

.suggestions-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: rgba(30, 30, 30, 0.98);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  margin-top: 4px;
  z-index: 1000;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
}

.suggestion-item {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  cursor: pointer;
  transition: background 0.15s ease;
  color: rgba(255, 255, 255, 0.85);
  font-size: 0.875rem;
}

.suggestion-item:hover {
  background: rgba(var(--v-theme-primary), 0.15);
}

.suggestion-item:not(:last-child) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.search-example-chip {
  cursor: pointer;
  transition: all 0.2s ease;
  border-color: rgba(255, 255, 255, 0.2) !important;
}

.search-example-chip:hover {
  background: rgba(var(--v-theme-primary), 0.15) !important;
  border-color: rgba(var(--v-theme-primary), 0.5) !important;
  transform: translateY(-1px);
}

/* ─── Map ─── */
.map-wrapper {
  position: relative;
  height: 480px;
  border-radius: 16px;
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.map-container {
  height: 100%;
  width: 100%;
  z-index: 1;
}

/* ─── Results header ─── */
.results-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  background: linear-gradient(135deg, rgb(var(--v-theme-primary)), rgba(var(--v-theme-primary), 0.7));
  padding: 8px 16px;
  border-radius: 24px;
  box-shadow: 0 4px 12px rgba(var(--v-theme-primary), 0.3);
}

.page-indicator {
  display: flex;
  align-items: center;
  gap: 6px;
  background: rgba(255, 255, 255, 0.06);
  padding: 8px 16px;
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.875rem;
  font-weight: 500;
}

/* ─── Gym cards ─── */
.gym-card {
  background: rgba(255, 255, 255, 0.04) !important;
  border: 1px solid rgba(255, 255, 255, 0.08);
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  height: 100%;
}

.gym-card:hover {
  transform: translateY(-4px);
  background: rgba(255, 255, 255, 0.08) !important;
  border-color: rgba(var(--v-theme-primary), 0.5);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.4), 0 0 20px rgba(var(--v-theme-primary), 0.1);
}

.gym-card--selected {
  border-color: rgb(var(--v-theme-primary)) !important;
  background: rgba(var(--v-theme-primary), 0.1) !important;
  box-shadow: 0 0 0 2px rgba(var(--v-theme-primary), 0.3), 0 8px 32px rgba(var(--v-theme-primary), 0.2);
}

.gym-card__glow {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle at center, rgba(var(--v-theme-primary), 0.08) 0%, transparent 60%);
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.gym-card:hover .gym-card__glow {
  opacity: 1;
}

.gym-card__icon-wrapper {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: rgba(var(--v-theme-primary), 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(var(--v-theme-primary), 0.2);
}

.gym-card__name {
  color: rgba(255, 255, 255, 0.95);
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.gym-card__address {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.55);
}

.gym-card__contact-icon {
  width: 24px;
  height: 24px;
  border-radius: 6px;
  background: rgba(255, 255, 255, 0.06);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.gym-card__link {
  color: rgba(255, 255, 255, 0.7);
  text-decoration: none;
  transition: color 0.2s ease;
}

.gym-card__link:hover {
  color: rgb(var(--v-theme-primary));
}

.gym-card__link--web {
  text-decoration: underline;
  text-underline-offset: 3px;
  text-decoration-color: rgba(var(--v-theme-primary), 0.4);
}

.gym-card__link--web:hover {
  text-decoration-color: rgb(var(--v-theme-primary));
}

/* ─── Pagination ─── */
.pagination-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  padding: 16px 0;
}

.pagination-arrow {
  color: rgba(255, 255, 255, 0.7) !important;
  transition: all 0.2s ease;
}

.pagination-arrow:hover:not(:disabled) {
  color: rgb(var(--v-theme-primary)) !important;
  background: rgba(var(--v-theme-primary), 0.1) !important;
}

.pagination-arrow:disabled {
  opacity: 0.3;
}

.pagination-dots {
  display: flex;
  align-items: center;
  gap: 8px;
}

.pagination-dot {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(255, 255, 255, 0.04);
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
}

.pagination-dot:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.pagination-dot--active {
  background: linear-gradient(135deg, rgb(var(--v-theme-primary)), rgba(var(--v-theme-primary), 0.8)) !important;
  border-color: rgb(var(--v-theme-primary)) !important;
  box-shadow: 0 4px 16px rgba(var(--v-theme-primary), 0.4);
  transform: scale(1.05);
}

.pagination-dot__number {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.8rem;
  font-weight: 600;
  transition: color 0.2s ease;
}

.pagination-dot:hover .pagination-dot__number {
  color: rgba(255, 255, 255, 0.9);
}

.pagination-dot--active .pagination-dot__number {
  color: white !important;
}

/* ─── Empty state ─── */
.empty-state {
  border: 1px dashed rgba(255, 255, 255, 0.15);
  border-radius: 12px;
}

/* ─── Utilities ─── */
.gap-2 { gap: 8px; }
.gap-3 { gap: 12px; }
.gap-4 { gap: 16px; }
</style>