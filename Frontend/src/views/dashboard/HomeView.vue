<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { RouterLink } from 'vue-router';
import { useMapStore } from '@/stores/mapStore';
import { useI18n } from 'vue-i18n';
import * as L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { logger } from '@/utils/logger';

const { t } = useI18n();

const mapStore = useMapStore();
const map = ref<L.Map | null>(null);
const markers = ref<L.Marker[]>([]);
const searchAddress = ref('');
const loading = ref(false);
const gymsFound = ref<any[]>([]);
const selectedGym = ref<any>(null);
const showMap = ref(false);

// Variables de paginación
const currentPage = ref(1);
const itemsPerPage = 8;

// Snackbar state
const snackbar = ref(false);
const snackbarMessage = ref('');
const snackbarColor = ref('success');

const showSnackbar = (message: string, color: string = 'success') => {
  snackbarMessage.value = message;
  snackbarColor.value = color;
  snackbar.value = true;
}

// Computed para paginación
const totalPages = computed(() => Math.ceil(gymsFound.value.length / itemsPerPage));

const paginatedGyms = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return gymsFound.value.slice(start, end);
});

// Iconos personalizados para el mapa
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

onMounted(() => {
  if (showMap.value) {
    initMap();
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

  // Inicializar el mapa centrado en Zaragoza
  map.value = L.map('map').setView([41.6488, -0.8891], 13);
  // Agregar capa de OpenStreetMap 
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
  }).addTo(map.value as L.Map);
  // Agregar evento de clic para buscar gimnasios cerca de la ubicación clicada
  map.value.on('click', async (e: L.LeafletMouseEvent) => {
    await searchGymsNearLocation(e.latlng.lat, e.latlng.lng);
  });
}

async function searchByAddress() {
  if (!searchAddress.value.trim()) return; // No hacer nada si el campo de búsqueda está vacío

  loading.value = true;
  currentPage.value = 1; // Reset a la primera página
  try {
    const coords = await mapStore.getCooredadas(searchAddress.value); // Obtener coordenadas de la dirección ingresada
    await searchGymsNearLocation(coords.lat, coords.lon); // Buscar gimnasios cerca de las coordenadas obtenidas
  } catch (error) {
    logger.error(t('address_search_error'), error);
    showSnackbar(t('home_could_not_find_address'), 'error');
  } finally {
    loading.value = false;
  }
}

async function searchGymsNearLocation(lat: number, lon: number) {
  loading.value = true;

  markers.value.forEach(marker => marker.remove()); // Eliminar marcadores anteriores del mapa
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
    currentPage.value = 1; // Reset a la primera página

    gyms.forEach((gym: any) => {
      if (gym.lat && gym.lon && map.value) { // Asegurarse de que el gimnasio tenga coordenadas válidas antes de agregar el marcador
        const marker = L.marker([parseFloat(gym.lat), parseFloat(gym.lon)], { icon: gymIcon }) // Usar icono personalizado para gimnasios
          .addTo(map.value as L.Map)
          .bindPopup(`<strong>${gym.nombre || 'Gimnasio'}</strong><br>${gym.direccion || ''}<br>${gym.telefono ? '<i class="mdi mdi-phone"></i> ' + gym.telefono : ''}<br>${gym.sitioWeb ? '<i class="mdi mdi-web"></i> <a href="' + gym.sitioWeb + '" target="_blank">Web</a>' : ''}`); // Datos de los gimnasios encontrados 

        marker.on('click', () => { // Al hacer clic en el marcador, mostrar detalles del gimnasio en la sección lateral
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
  // Scroll suave hacia arriba de los resultados
  const resultsElement = document.querySelector('.results-header-epic');
  if (resultsElement) {
    resultsElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
</script>

<template>
  <v-app>
    <v-main class="main-container">
      <v-container v-if="!showMap" fluid class="hero-section pa-0">
        <!-- Fondo animado con partículas -->
        <div class="hero-background">
          <div class="gradient-overlay"></div>
          <div class="particles">
            <div class="particle" v-for="n in 20" :key="n"></div>
          </div>
          <div class="grid-overlay"></div>
        </div>

        <v-container class="hero-content">
          <v-row align="center" justify="center">
            <!-- Contenido Principal -->
            <v-col cols="12" lg="6" class="hero-left">
              <!-- Super Badge -->
              <div class="super-badge mb-6">
                <div class="badge-glow"></div>
                <v-icon color="amber" size="24" class="mr-2">mdi-trophy-variant</v-icon>
                <span class="badge-text">Plataforma de Fitness Gamificada #1</span>
                <v-icon color="amber" size="20" class="ml-2">mdi-fire</v-icon>
              </div>

              <!-- Título épico -->
              <h1 class="epic-title mb-4">
                <span class="title-line-1">Transforma Tu</span>
                <span class="title-line-2">
                  <span class="gradient-text">Entrenamiento</span>
                </span>
                <span class="title-line-3">En Una Aventura</span>
              </h1>

              <!-- Subtítulo mejorado -->
              <p class="epic-subtitle mb-8">
                <span v-html="$t('home_join_traininghub')"></span>
                <span class="highlight">fitness social</span>,
                <span class="highlight">{{ $t('home_gamification') }}</span> y
                <span class="highlight">{{ $t('home_epic_rewards') }}</span>.
                Entrena con amigos, conquista retos y sube de nivel como nunca antes.
              </p>

              <!-- Features destacados -->
              <div class="features-grid mb-8">
                <div class="feature-item">
                  <div class="feature-icon">
                    <v-icon size="32" color="white">mdi-account-group</v-icon>
                  </div>
                  <div class="feature-content">
                    <div class="feature-title">{{ $t('training_rooms') }}</div>
                    <div class="feature-desc">{{ $t('train_group') }}</div>
                  </div>
                </div>

                <div class="feature-item">
                  <div class="feature-icon gradient-warning">
                    <v-icon size="32" color="white">mdi-target</v-icon>
                  </div>
                  <div class="feature-content">
                    <div class="feature-title">{{ $t('home_epic_challenges') }}</div>
                    <div class="feature-desc">{{ $t('challenges') }}</div>
                  </div>
                </div>

                <div class="feature-item">
                  <div class="feature-icon gradient-success">
                    <v-icon size="32" color="white">mdi-star-circle</v-icon>
                  </div>
                  <div class="feature-content">
                    <div class="feature-title">Sistema XP</div>
                    <div class="feature-desc">Sube de nivel</div>
                  </div>
                </div>

                <div class="feature-item">
                  <div class="feature-icon gradient-info">
                    <v-icon size="32" color="white">mdi-cash-multiple</v-icon>
                  </div>
                  <div class="feature-content">
                    <div class="feature-title">{{ $t('home_virtual_economy') }}</div>
                    <div class="feature-desc">Gana recompensas</div>
                  </div>
                </div>
              </div>

              <!-- Botones de acción épicos -->
              <div class="action-buttons mb-8">
                <RouterLink to="/register" class="no-decoration">
                  <v-btn class="epic-btn primary" size="x-large" elevation="0">
                    <div class="btn-content">
                      <v-icon size="28" class="mr-3">mdi-rocket-launch</v-icon>
                      <div>
                        <div class="btn-main-text">Comenzar Aventura</div>
                        <div class="btn-sub-text">Gratis • Sin tarjeta</div>
                      </div>
                    </div>
                  </v-btn>
                </RouterLink>

                <v-btn class="epic-btn secondary ml-4" size="x-large" elevation="0" @click="toggleMapView">
                  <div class="btn-content">
                    <v-icon size="28" class="mr-3">mdi-map-marker-radius</v-icon>
                    <div>
                      <div class="btn-main-text">{{ $t('buscar') }}</div>
                      <div class="btn-sub-text">Cerca de ti</div>
                    </div>
                  </div>
                </v-btn>
              </div>

              <!-- Stats en tiempo real -->
              <div class="live-stats">
                <div class="stat-card">
                  <div class="stat-value">1,247</div>
                  <div class="stat-label">
                    <v-icon size="16" color="success">mdi-circle</v-icon>
                    {{ $t('active_users') }}
                  </div>
                </div>
                <div class="stat-separator"></div>
                <div class="stat-card">
                  <div class="stat-value">89.4K</div>
                  <div class="stat-label">
                    <v-icon size="16" color="amber">mdi-fire</v-icon>
                    {{ $t('available_challenges') }}
                  </div>
                </div>
              </div>
            </v-col>

            <!-- Visual Section -->
            <v-col cols="12" lg="6" class="hero-right">
              <div class="visual-container-epic">
                <!-- Anillos orbitales de fondo -->
                <div class="orbital-rings">
                  <div class="ring ring-1"></div>
                  <div class="ring ring-2"></div>
                  <div class="ring ring-3"></div>
                </div>

                <!-- Centro: Mockup principal con efecto 3D -->
                <div class="main-visual-hub">
                  <div class="hub-glow-massive"></div>
                  <div class="phone-mockup-3d">
                    <div class="phone-frame">
                      <div class="phone-notch"></div>
                      <div class="phone-screen">
                        <v-img src="@/assets/imgs/People.png" alt="TheTrainingHub App" class="phone-content" cover />
                        <!-- Overlay con brillo dinámico -->
                        <div class="screen-shine"></div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Cards flotantes rediseñadas con nuevo estilo -->
                <div class="floating-metrics">
                  <!-- Métrica XP Superior Derecha -->
                  <div class="metric-card metric-xp">
                    <div class="metric-glow metric-glow-yellow"></div>
                    <div class="metric-icon-container">
                      <div class="icon-pulse"></div>
                      <v-icon size="32" color="white">mdi-lightning-bolt</v-icon>
                    </div>
                    <div class="metric-data">
                      <div class="metric-value">+150</div>
                      <div class="metric-label">XP Ganado</div>
                      <div class="metric-badge">{{ $t('home_epic_badge') }}</div>
                    </div>
                  </div>

                  <!-- Métrica de Nivel Izquierda Superior -->
                  <div class="metric-card metric-level">
                    <div class="metric-glow metric-glow-purple"></div>
                    <div class="level-display">
                      <div class="level-crown">
                        <v-icon size="40" color="white">mdi-crown</v-icon>
                      </div>
                      <div class="level-number">12</div>
                    </div>
                    <div class="level-progress-bar">
                      <div class="level-progress-fill" style="width: 73%">
                        <span class="progress-percentage">73%</span>
                      </div>
                    </div>
                    <div class="metric-label">Nivel Actual</div>
                  </div>

                  <!-- Sala Social Izquierda Inferior -->
                  <div class="metric-card metric-room">
                    <div class="metric-glow metric-glow-blue"></div>
                    <div class="room-status">
                      <div class="live-indicator"></div>
                      <span class="live-text">EN VIVO</span>
                    </div>
                    <div class="room-title">
                      <v-icon size="24" color="white">mdi-account-group</v-icon>
                      <span>Sala Entrenamiento</span>
                    </div>
                    <div class="room-members">
                      <div class="member-avatars">
                        <div class="member-avatar" v-for="n in 3" :key="n" :style="{ zIndex: 10 - n }">
                          <v-icon size="20" color="white">mdi-account</v-icon>
                        </div>
                      </div>
                      <div class="member-count">+12 entrenando</div>
                    </div>
                  </div>

                  <!-- Racha de Fuego Derecha Inferior -->
                  <div class="metric-card metric-streak">
                    <div class="metric-glow metric-glow-orange"></div>
                    <div class="streak-fire">
                      <v-icon size="56" class="fire-icon">mdi-fire</v-icon>
                      <div class="fire-particles">
                        <div class="particle" v-for="n in 8" :key="n"></div>
                      </div>
                    </div>
                    <div class="streak-count">7 {{ $t('home_days') }}</div>
                    <div class="metric-label">{{ $t('active_streak') }}</div>
                  </div>

                  <!-- Monedas Derecha Centro -->
                  <div class="metric-card metric-coins">
                    <div class="metric-glow metric-glow-gold"></div>
                    <div class="coins-animation">
                      <v-icon size="48" color="#FFD700">mdi-bitcoin</v-icon>
                      <div class="coin-sparkles">
                        <div class="sparkle" v-for="n in 6" :key="n"></div>
                      </div>
                    </div>
                    <div class="coins-earned">+50</div>
                    <div class="metric-label">{{ $t('coins') }}</div>
                  </div>

                  <!-- Estadística Circular Superior Izquierda -->
                  <div class="metric-card metric-circular">
                    <div class="metric-glow metric-glow-green"></div>
                    <svg class="progress-ring" viewBox="0 0 120 120">
                      <circle class="progress-ring-bg" cx="60" cy="60" r="52" />
                      <circle class="progress-ring-fill" cx="60" cy="60" r="52" stroke-dasharray="327"
                        stroke-dashoffset="82" />
                    </svg>
                    <div class="circular-content">
                      <v-icon size="32" color="white">mdi-target</v-icon>
                      <div class="circular-percentage">75%</div>
                    </div>
                    <div class="metric-label-small">{{ $t('objective') }}</div>
                  </div>
                </div>

                <!-- Partículas ambientales -->
                <div class="ambient-particles">
                  <div class="ambient-particle" v-for="n in 15" :key="n"></div>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Sección de confianza -->
          <v-row class="trust-section mt-16">
            <v-col cols="12" class="text-center">
              <p class="trust-text mb-6">{{ $t('footer_join_community') }}</p>
              <div class="stats-row">
                <div class="mega-stat">
                  <div class="mega-stat-icon">
                    <v-icon size="48" color="amber">mdi-account-multiple</v-icon>
                  </div>
                  <div class="mega-stat-value">10K+</div>
                  <div class="mega-stat-label">{{ $t('active_users') }}</div>
                </div>

                <div class="mega-stat">
                  <div class="mega-stat-icon gradient-success">
                    <v-icon size="48" color="white">mdi-dumbbell</v-icon>
                  </div>
                  <div class="mega-stat-value">250K+</div>
                  <div class="mega-stat-label">{{ $t('workouts') }}</div>
                </div>

                <div class="mega-stat">
                  <div class="mega-stat-icon gradient-warning">
                    <v-icon size="48" color="white">mdi-trophy</v-icon>
                  </div>
                  <div class="mega-stat-value">500+</div>
                  <div class="mega-stat-label">{{ $t('available_challenges') }}</div>
                </div>

                <div class="mega-stat">
                  <div class="mega-stat-icon gradient-info">
                    <v-icon size="48" color="white">mdi-star</v-icon>
                  </div>
                  <div class="mega-stat-value">4.9</div>
                  <div class="mega-stat-label">{{ $t('home_average_rating') }}</div>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Todas las características -->
          <v-row class="all-features-section">
            <v-col cols="12" class="text-center mb-8">
              <h2 class="features-section-title">{{ $t('home_all_features_title') }}</h2>
              <p class="features-section-subtitle">{{ $t('home_all_features_subtitle') }}</p>
            </v-col>
            <v-col cols="12">
              <div class="features-categories-grid">
                <!-- Entrenamiento -->
                <div class="category-card">
                  <div class="category-card-header">
                    <div class="category-card-icon gradient-purple">
                      <v-icon size="28" color="white">mdi-dumbbell</v-icon>
                    </div>
                    <span class="category-card-title">{{ $t('category_training') }}</span>
                  </div>
                  <div class="category-features-list">
                    <div class="category-feature-item">
                      <v-icon size="22" color="#a78bfa">mdi-calendar-check</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('routines') }}</div>
                        <div class="cf-desc">{{ $t('routines_desc') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#a78bfa">mdi-book-open-variant</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('exercises_title') }}</div>
                        <div class="cf-desc">{{ $t('exercises_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#a78bfa">mdi-account-group</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('training_rooms') }}</div>
                        <div class="cf-desc">{{ $t('rooms_desc_short') }}</div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Progreso -->
                <div class="category-card">
                  <div class="category-card-header">
                    <div class="category-card-icon gradient-blue">
                      <v-icon size="28" color="white">mdi-trending-up</v-icon>
                    </div>
                    <span class="category-card-title">{{ $t('category_progress') }}</span>
                  </div>
                  <div class="category-features-list">
                    <div class="category-feature-item">
                      <v-icon size="22" color="#22d3ee">mdi-lightning-bolt</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('levels') }} & XP</div>
                        <div class="cf-desc">{{ $t('levels_desc') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#22d3ee">mdi-scale-bathroom</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('body_metrics_title') }}</div>
                        <div class="cf-desc">{{ $t('metrics_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#22d3ee">mdi-medal</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('achievements') }}</div>
                        <div class="cf-desc">{{ $t('achievements_desc_short') }}</div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Social -->
                <div class="category-card">
                  <div class="category-card-header">
                    <div class="category-card-icon gradient-pink">
                      <v-icon size="28" color="white">mdi-earth</v-icon>
                    </div>
                    <span class="category-card-title">{{ $t('category_social') }}</span>
                  </div>
                  <div class="category-features-list">
                    <div class="category-feature-item">
                      <v-icon size="22" color="#f093fb">mdi-account-multiple</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('train_group') }}</div>
                        <div class="cf-desc">{{ $t('join_rooms') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#f093fb">mdi-chat</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('chat_rooms') }}</div>
                        <div class="cf-desc">{{ $t('chat_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#f093fb">mdi-trophy</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('Ranking') }}</div>
                        <div class="cf-desc">{{ $t('ranking_desc_short') }}</div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Premium -->
                <div class="category-card premium-card">
                  <div class="category-card-header">
                    <div class="category-card-icon gradient-gold">
                      <v-icon size="28" color="white">mdi-star</v-icon>
                    </div>
                    <span class="category-card-title">{{ $t('category_premium') }}</span>
                    <span class="premium-mini-badge">PREMIUM</span>
                  </div>
                  <div class="category-features-list">
                    <div class="category-feature-item">
                      <v-icon size="22" color="#ffd700">mdi-robot</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('coach_ai') }}</div>
                        <div class="cf-desc">{{ $t('coach_ai_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#ffd700">mdi-calculator</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('calculator') }}</div>
                        <div class="cf-desc">{{ $t('calculator_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#ffd700">mdi-clipboard-list</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('plans') }}</div>
                        <div class="cf-desc">{{ $t('plans_desc_short') }}</div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Economía -->
                <div class="category-card">
                  <div class="category-card-header">
                    <div class="category-card-icon gradient-green">
                      <v-icon size="28" color="white">mdi-cash-multiple</v-icon>
                    </div>
                    <span class="category-card-title">{{ $t('category_economy') }}</span>
                  </div>
                  <div class="category-features-list">
                    <div class="category-feature-item">
                      <v-icon size="22" color="#4ade80">mdi-store</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('shop_header') }}</div>
                        <div class="cf-desc">{{ $t('shop_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#4ade80">mdi-bag-personal</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('inventario') }}</div>
                        <div class="cf-desc">{{ $t('inventory_desc_short') }}</div>
                      </div>
                    </div>
                    <div class="category-feature-item">
                      <v-icon size="22" color="#4ade80">mdi-bitcoin</v-icon>
                      <div class="cf-content">
                        <div class="cf-title">{{ $t('coins') }}</div>
                        <div class="cf-desc">{{ $t('coins_desc_short') }}</div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </v-container>

      <!-- Map Section Mejorada -->
      <v-container v-else fluid class="map-section-epic pa-0">
        <!-- Header del mapa -->
        <div class="map-header-epic">
          <v-btn class="back-btn-epic" icon size="large" @click="toggleMapView">
            <v-icon>mdi-arrow-left</v-icon>
          </v-btn>

          <div class="map-title-container">
            <h2 class="map-main-title">
              <v-icon size="40" color="amber" class="mr-3">mdi-map-marker-star</v-icon>
              Encuentra Tu Gimnasio Perfecto
            </h2>
            <p class="map-main-subtitle">
              Descubre los mejores gimnasios cerca de ti y comienza tu aventura
            </p>
          </div>
        </div>

        <v-container class="map-content-container py-8">
          <!-- Búsqueda mejorada -->
          <v-row>
            <v-col cols="12">
              <div class="search-box-epic">
                <div class="search-box-header">
                  <v-icon color="purple" size="28" class="mr-3">mdi-magnify</v-icon>
                  <div>
                    <div class="search-box-title">{{ $t('home_search_location') }}</div>
                    <div class="search-box-subtitle">{{ $t('home_search_location') }}</div>
                  </div>
                </div>

                <v-row align="center" class="mt-4">
                  <v-col cols="12" md="8">
                    <v-text-field v-model="searchAddress" placeholder="Ej: Calle Mayor 10, Zaragoza" variant="solo"
                      density="comfortable" hide-details class="search-input-epic" @keyup.enter="searchByAddress">
                      <template v-slot:prepend-inner>
                        <v-icon color="grey">mdi-map-search</v-icon>
                      </template>
                    </v-text-field>
                  </v-col>
                  <v-col cols="12" md="4">
                    <v-btn block size="x-large" class="search-btn-epic" :loading="loading" @click="searchByAddress">
                      <v-icon left size="24">mdi-radar</v-icon>
                      {{ $t('buscar') }}
                    </v-btn>
                  </v-col>
                </v-row>
              </div>
            </v-col>
          </v-row>

          <!-- Mapa -->
          <v-row class="mt-6">
            <v-col cols="12">
              <div class="map-wrapper-epic">
                <div id="map" class="map-element-epic"></div>
                <div class="map-controls">
                  <v-btn icon size="small" class="map-control-btn" title="Centrar mapa">
                    <v-icon>mdi-crosshairs-gps</v-icon>
                  </v-btn>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Resultados mejorados -->
          <v-row v-if="gymsFound.length > 0" class="mt-6">
            <v-col cols="12">
              <div class="results-header-epic">
                <div class="results-title-content">
                  <v-icon color="success" size="32" class="mr-3">mdi-check-circle</v-icon>
                  <div>
                    <h3 class="results-main-title">{{ gymsFound.length }} Gimnasios Encontrados</h3>
                    <p class="results-main-subtitle">
                      Mostrando {{ (currentPage - 1) * itemsPerPage + 1 }} -
                      {{ Math.min(currentPage * itemsPerPage, gymsFound.length) }} de {{ gymsFound.length }}
                    </p>
                  </div>
                </div>
              </div>

              <div class="gyms-grid-epic">
                <div v-for="(gym, index) in paginatedGyms" :key="index" class="gym-card-epic"
                  :class="{ 'selected': selectedGym === gym }" @click="selectedGym = gym">
                  <div class="gym-card-header">
                    <div class="gym-icon-wrapper">
                      <v-icon color="white" size="32">mdi-dumbbell</v-icon>
                    </div>
                    <v-chip small color="success" class="gym-badge">
                      <v-icon x-small left>mdi-check-circle</v-icon>
                      Verificado
                    </v-chip>
                  </div>

                  <div class="gym-card-body">
                    <h4 class="gym-name-epic">
                      {{ gym.nombre || 'Centro Deportivo' }}
                    </h4>
                    <p class="gym-address-epic">
                      <v-icon small color="grey">mdi-map-marker</v-icon>
                      {{ gym.direccion || $t('home_address_not_available') }}
                    </p>
                    <div class="gym-extra-info">
                      <p v-if="gym.telefono" class="gym-extra-line">
                        <v-icon small color="primary">mdi-phone</v-icon>
                        <span>{{ gym.telefono }}</span>
                      </p>
                      <p v-if="gym.sitioWeb" class="gym-extra-line">
                        <v-icon small color="primary">mdi-web</v-icon>
                        <a :href="gym.sitioWeb" target="_blank" rel="noopener noreferrer" class="gym-link">{{
                          $t('home_visit_website') }}</a>
                      </p>
                      <p v-if="gym.horarioApertura" class="gym-extra-line">
                        <v-icon small color="primary">mdi-clock-outline</v-icon>
                        <span>{{ gym.horarioApertura }}</span>
                      </p>
                      <p v-if="gym.accesibilidad" class="gym-extra-line">
                        <v-icon small color="primary">mdi-wheelchair-accessibility</v-icon>
                        <span>{{ $t('home_accessibility') }}: {{ gym.accesibilidad }}</span>
                      </p>
                      <p v-if="gym.operador" class="gym-extra-line">
                        <v-icon small color="primary">mdi-domain</v-icon>
                        <span>{{ gym.operador }}</span>
                      </p>
                    </div>
                  </div>

                  <div class="gym-card-footer">
                    <div class="gym-distance">
                      <v-icon small color="blue">mdi-walk</v-icon>
                      <span>~ 1.2 km</span>
                    </div>
                    <v-btn size="small" color="primary" variant="text" class="gym-action-btn">
                      {{ $t('home_see_more') }}
                      <v-icon right small>mdi-arrow-right</v-icon>
                    </v-btn>
                  </div>

                  <div class="gym-card-glow"></div>
                </div>
              </div>

              <!-- Paginador Épico -->
              <div v-if="totalPages > 1" class="pagination-container-epic">
                <div class="pagination-info">
                  <v-icon color="primary" class="mr-2">mdi-information-outline</v-icon>
                  <span>{{ $t('home_page_of', [currentPage, totalPages]) }}</span>
                </div>

                <div class="pagination-controls">
                  <!-- Botón Primera Página -->
                  <v-btn icon variant="text" :disabled="currentPage === 1" @click="changePage(1)"
                    class="pagination-btn">
                    <v-icon>mdi-page-first</v-icon>
                  </v-btn>

                  <!-- Botón Anterior -->
                  <v-btn icon variant="text" :disabled="currentPage === 1" @click="changePage(currentPage - 1)"
                    class="pagination-btn">
                    <v-icon>mdi-chevron-left</v-icon>
                  </v-btn>

                  <!-- Números de Página -->
                  <div class="page-numbers">
                    <v-btn v-for="page in totalPages" :key="page"
                      :class="['page-number-btn', { 'active': page === currentPage }]"
                      :variant="page === currentPage ? 'flat' : 'text'"
                      :color="page === currentPage ? 'primary' : 'default'" @click="changePage(page)" size="small">
                      {{ page }}
                    </v-btn>
                  </div>

                  <!-- Botón Siguiente -->
                  <v-btn icon variant="text" :disabled="currentPage === totalPages" @click="changePage(currentPage + 1)"
                    class="pagination-btn">
                    <v-icon>mdi-chevron-right</v-icon>
                  </v-btn>

                  <!-- Botón Última Página -->
                  <v-btn icon variant="text" :disabled="currentPage === totalPages" @click="changePage(totalPages)"
                    class="pagination-btn">
                    <v-icon>mdi-page-last</v-icon>
                  </v-btn>
                </div>

                <div class="pagination-jump">
                  <span class="jump-label">{{ $t('home_go_to_page') }}</span>
                  <v-select v-model="currentPage" :items="Array.from({ length: totalPages }, (_, i) => i + 1)"
                    variant="outlined" density="compact" hide-details class="page-select"
                    @update:model-value="changePage"></v-select>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </v-container>
      <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4000" location="top" multi-line>
        <div class="d-flex align-center">
          <v-icon :icon="snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'" class="mr-3"></v-icon>
          <span>{{ snackbarMessage }}</span>
        </div>

        <template v-slot:actions>
          <v-btn variant="text" @click="snackbar = false" icon="mdi-close"></v-btn>
        </template>
      </v-snackbar>
    </v-main>
  </v-app>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@400;500;600;700;800;900&family=Inter:wght@400;500;600;700;800&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.main-container {
  min-height: 100vh;
  background: #0a0a0f;
  position: relative;
  overflow-x: hidden;
  font-family: 'Inter', sans-serif;
}

/* ===== HERO SECTION ===== */
.hero-section {
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

.hero-background {
  position: absolute;
  inset: 0;
  z-index: 0;
}

.gradient-overlay {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at 20% 30%, rgba(102, 126, 234, 0.15) 0%, transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(255, 87, 51, 0.15) 0%, transparent 50%),
    linear-gradient(135deg, #0a0a0f 0%, #1a1a2e 50%, #16213e 100%);
}

.particles {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(102, 126, 234, 0.5);
  border-radius: 50%;
  animation: float-particle 20s infinite ease-in-out;
}

.particle:nth-child(odd) {
  background: rgba(255, 193, 7, 0.4);
  animation-duration: 15s;
}

@keyframes float-particle {

  0%,
  100% {
    transform: translate(0, 0) scale(1);
    opacity: 0;
  }

  10% {
    opacity: 1;
  }

  90% {
    opacity: 1;
  }

  100% {
    transform: translate(100vw, -100vh) scale(0);
    opacity: 0;
  }
}

.particle:nth-child(1) {
  top: 20%;
  left: 10%;
  animation-delay: 0s;
}

.particle:nth-child(2) {
  top: 60%;
  left: 20%;
  animation-delay: 2s;
}

.particle:nth-child(3) {
  top: 40%;
  left: 70%;
  animation-delay: 4s;
}

.particle:nth-child(4) {
  top: 80%;
  left: 40%;
  animation-delay: 1s;
}

.particle:nth-child(5) {
  top: 10%;
  left: 80%;
  animation-delay: 3s;
}

.particle:nth-child(6) {
  top: 70%;
  left: 60%;
  animation-delay: 5s;
}

.particle:nth-child(7) {
  top: 30%;
  left: 30%;
  animation-delay: 6s;
}

.particle:nth-child(8) {
  top: 50%;
  left: 90%;
  animation-delay: 2.5s;
}

.particle:nth-child(9) {
  top: 90%;
  left: 15%;
  animation-delay: 4.5s;
}

.particle:nth-child(10) {
  top: 15%;
  left: 50%;
  animation-delay: 1.5s;
}

.grid-overlay {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(102, 126, 234, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(102, 126, 234, 0.05) 1px, transparent 1px);
  background-size: 50px 50px;
  animation: grid-move 20s linear infinite;
}

@keyframes grid-move {
  0% {
    transform: translate(0, 0);
  }

  100% {
    transform: translate(50px, 50px);
  }
}

.hero-content {
  position: relative;
  z-index: 1;
  padding: 100px 20px 80px;
}

/* ===== HERO LEFT ===== */
.hero-left {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.super-badge {
  display: inline-flex;
  align-items: center;
  padding: 12px 24px;
  background: rgba(255, 193, 7, 0.1);
  border: 2px solid rgba(255, 193, 7, 0.3);
  border-radius: 50px;
  backdrop-filter: blur(20px);
  position: relative;
  width: fit-content;
  animation: badge-pulse 3s ease-in-out infinite;
}

.badge-glow {
  position: absolute;
  inset: -4px;
  background: linear-gradient(90deg, #ffd700, #ff8c00, #ffd700);
  border-radius: 50px;
  filter: blur(8px);
  opacity: 0.5;
  z-index: -1;
  animation: badge-glow-rotate 3s linear infinite;
}

@keyframes badge-pulse {

  0%,
  100% {
    transform: scale(1);
  }

  50% {
    transform: scale(1.05);
  }
}

@keyframes badge-glow-rotate {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.badge-text {
  color: #ffd700;
  font-weight: 700;
  font-size: 0.95rem;
  letter-spacing: 0.5px;
}

.epic-title {
  font-family: 'Space Grotesk', sans-serif;
  font-weight: 900;
  line-height: 1.1;
  color: white;
  margin-bottom: 2rem;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.title-line-1 {
  font-size: 3.5rem;
  opacity: 0.9;
  animation: slide-in-left 0.8s ease-out;
}

.title-line-2 {
  font-size: 4.5rem;
  animation: slide-in-left 0.8s ease-out 0.2s backwards;
}

.title-line-3 {
  font-size: 3.5rem;
  opacity: 0.9;
  animation: slide-in-left 0.8s ease-out 0.4s backwards;
}

@keyframes slide-in-left {
  from {
    opacity: 0;
    transform: translateX(-50px);
  }

  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.gradient-text {
  background: linear-gradient(135deg, #667eea 0%, #ffd700 50%, #f093fb 100%);
  background-size: 200% 200%;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: gradient-shift 5s ease infinite;
}

@keyframes gradient-shift {

  0%,
  100% {
    background-position: 0% 50%;
  }

  50% {
    background-position: 100% 50%;
  }
}

.epic-subtitle {
  font-size: 1.2rem;
  line-height: 1.8;
  color: rgba(255, 255, 255, 0.8);
  max-width: 600px;
  animation: fade-in-up 0.8s ease-out 0.6s backwards;
}

@keyframes fade-in-up {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.highlight {
  color: #ffd700;
  font-weight: 700;
  position: relative;
  padding: 0 4px;
}

.highlight::after {
  content: '';
  position: absolute;
  bottom: 2px;
  left: 0;
  right: 0;
  height: 8px;
  background: rgba(255, 193, 7, 0.2);
  z-index: -1;
}

/* ===== FEATURES GRID ===== */
.features-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
  animation: fade-in-up 0.8s ease-out 0.8s backwards;
}

.feature-item {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  padding: 1.25rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  backdrop-filter: blur(20px);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
}

.feature-item:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: rgba(102, 126, 234, 0.5);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 12px 32px rgba(102, 126, 234, 0.3);
}

.feature-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.4);
}

.gradient-warning {
  background: linear-gradient(135deg, #f093fb, #f5576c);
  box-shadow: 0 8px 24px rgba(245, 87, 108, 0.4);
}

.gradient-success {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
  box-shadow: 0 8px 24px rgba(79, 172, 254, 0.4);
}

.gradient-info {
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  box-shadow: 0 8px 24px rgba(255, 140, 0, 0.4);
}

.feature-title {
  font-size: 1rem;
  font-weight: 700;
  color: white;
  margin-bottom: 2px;
}

.feature-desc {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
}

/* ===== ACTION BUTTONS ===== */
.action-buttons {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  animation: fade-in-up 0.8s ease-out 1s backwards;
}

.no-decoration {
  text-decoration: none;
}

.epic-btn {
  border-radius: 16px !important;
  padding: 0 !important;
  height: auto !important;
  text-transform: none;
  font-family: 'Space Grotesk', sans-serif;
  position: relative;
  overflow: hidden;
}

.epic-btn::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.epic-btn:hover::before {
  opacity: 1;
}

.epic-btn.primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.4);
}

.epic-btn.primary:hover {
  box-shadow: 0 12px 48px rgba(102, 126, 234, 0.6);
  transform: translateY(-4px);
}

.epic-btn.secondary {
  background: rgba(255, 255, 255, 0.05) !important;
  border: 2px solid rgba(102, 126, 234, 0.5);
  backdrop-filter: blur(20px);
}

.epic-btn.secondary:hover {
  background: rgba(102, 126, 234, 0.1) !important;
  border-color: #667eea;
  transform: translateY(-4px);
}

.btn-content {
  display: flex;
  align-items: center;
  padding: 1.25rem 2rem;
  color: white;
}

.btn-main-text {
  font-size: 1.1rem;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 4px;
}

.btn-sub-text {
  font-size: 0.75rem;
  opacity: 0.8;
  font-weight: 500;
}

/* ===== LIVE STATS ===== */
.live-stats {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 1.25rem 1.75rem;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  backdrop-filter: blur(20px);
  width: fit-content;
  animation: fade-in-up 0.8s ease-out 1.2s backwards;
}

.stat-card {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 800;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
}

.stat-label {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.6);
  display: flex;
  align-items: center;
  gap: 6px;
}

.stat-separator {
  width: 1px;
  height: 40px;
  background: rgba(255, 255, 255, 0.15);
}

/* ===== HERO RIGHT (VISUAL) ===== */
.hero-right {
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.visual-container {
  position: relative;
  width: 100%;
  max-width: 600px;
  height: 600px;
}

.main-mockup {
  position: relative;
  width: 100%;
  height: 100%;
  animation: fade-in-scale 1s ease-out 0.4s backwards;
}

@keyframes fade-in-scale {
  from {
    opacity: 0;
    transform: scale(0.9);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

.mockup-glow {
  position: absolute;
  inset: -40px;
  background: radial-gradient(circle, rgba(102, 126, 234, 0.3) 0%, transparent 70%);
  animation: glow-pulse 3s ease-in-out infinite;
  z-index: 0;
}

@keyframes glow-pulse {

  0%,
  100% {
    opacity: 0.5;
    transform: scale(1);
  }

  50% {
    opacity: 1;
    transform: scale(1.1);
  }
}

.mockup-image {
  position: relative;
  z-index: 1;
  border-radius: 32px;
  box-shadow: 0 24px 64px rgba(0, 0, 0, 0.6);
}

/* ===== FLOATING ELEMENTS ===== */
.floating-elements {
  position: absolute;
  inset: 0;
  z-index: 2;
}

.float-card {
  position: absolute;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 16px;
  padding: 1rem 1.25rem;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.2);
  display: flex;
  align-items: center;
  gap: 12px;
  animation: float-animation 3s ease-in-out infinite;
}

@keyframes float-animation {

  0%,
  100% {
    transform: translateY(0px);
  }

  50% {
    transform: translateY(-15px);
  }
}

.xp-card {
  top: 10%;
  right: -5%;
  animation-delay: 0s;
}

.card-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.card-pulse {
  position: absolute;
  inset: -4px;
  border-radius: 14px;
  border: 2px solid #ffd700;
  animation: pulse-ring 2s ease-out infinite;
}

@keyframes pulse-ring {
  0% {
    transform: scale(1);
    opacity: 1;
  }

  100% {
    transform: scale(1.5);
    opacity: 0;
  }
}

.card-value {
  font-size: 1.5rem;
  font-weight: 900;
  color: #1a1a2e;
  font-family: 'Space Grotesk', sans-serif;
}

.card-label {
  font-size: 0.85rem;
  color: #666;
  font-weight: 600;
}

.level-card {
  top: 35%;
  left: -8%;
  animation-delay: 0.5s;
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  padding: 1.25rem;
}

.level-icon {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
}

.level-badge {
  font-size: 1.25rem;
  font-weight: 900;
  margin-bottom: 8px;
  letter-spacing: 1px;
}

.level-progress {
  width: 120px;
  height: 8px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 10px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #ffd700, #fff);
  border-radius: 10px;
  animation: progress-fill 2s ease-out;
}

@keyframes progress-fill {
  from {
    width: 0%;
  }
}

.room-card {
  bottom: 15%;
  left: -5%;
  animation-delay: 1s;
  flex-direction: column;
  align-items: flex-start;
  gap: 8px;
}

.room-header {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 700;
  color: #1a1a2e;
}

.room-avatars {
  display: flex;
  gap: -8px;
}

.avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border: 3px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: -8px;
}

.avatar:first-child {
  margin-left: 0;
}

.avatar-more {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: #1a1a2e;
  border: 3px solid white;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 700;
  font-size: 0.75rem;
  margin-left: -8px;
}

.streak-card {
  top: 60%;
  right: -3%;
  animation-delay: 1.5s;
  flex-direction: column;
  text-align: center;
  gap: 4px;
}

.streak-number {
  font-size: 2rem;
  font-weight: 900;
  color: #1a1a2e;
  font-family: 'Space Grotesk', sans-serif;
}

.streak-label {
  font-size: 0.8rem;
  color: #666;
  font-weight: 600;
}

.coins-card {
  bottom: 8%;
  right: 10%;
  animation-delay: 2s;
  padding: 0.75rem 1rem;
}

.coins-value {
  font-size: 1.5rem;
  font-weight: 900;
  color: #1a1a2e;
  font-family: 'Space Grotesk', sans-serif;
}

/* ===== CIRCULAR STATS ===== */
.circular-stats {
  position: absolute;
  top: 5%;
  left: 5%;
  animation: fade-in-scale 1s ease-out 1s backwards;
}

.circular-stat {
  position: relative;
  width: 80px;
  height: 80px;
}

.circular-chart {
  display: block;
  max-width: 100%;
  max-height: 100%;
}

.circle-bg {
  fill: none;
  stroke: rgba(255, 255, 255, 0.1);
  stroke-width: 3;
}

.circle {
  fill: none;
  stroke: #ffd700;
  stroke-width: 3;
  stroke-linecap: round;
  animation: circle-progress 2s ease-out forwards;
}

@keyframes circle-progress {
  from {
    stroke-dasharray: 0, 100;
  }
}

.percentage {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 1.25rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
}

/* ===== TRUST SECTION ===== */
.trust-section {
  margin-top: 80px;
  padding-top: 60px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.trust-text {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

.stats-row {
  display: flex;
  justify-content: center;
  gap: 3rem;
  flex-wrap: wrap;
}

.mega-stat {
  text-align: center;
  animation: fade-in-up 0.8s ease-out backwards;
}

.mega-stat:nth-child(1) {
  animation-delay: 0.2s;
}

.mega-stat:nth-child(2) {
  animation-delay: 0.4s;
}

.mega-stat:nth-child(3) {
  animation-delay: 0.6s;
}

.mega-stat:nth-child(4) {
  animation-delay: 0.8s;
}

.mega-stat-icon {
  width: 80px;
  height: 80px;
  border-radius: 20px;
  background: rgba(255, 193, 7, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  box-shadow: 0 8px 24px rgba(255, 193, 7, 0.3);
  transition: all 0.3s ease;
}

.mega-stat:hover .mega-stat-icon {
  transform: translateY(-8px) scale(1.1);
}

.mega-stat-icon.gradient-success {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
  box-shadow: 0 8px 24px rgba(79, 172, 254, 0.4);
}

.mega-stat-icon.gradient-warning {
  background: linear-gradient(135deg, #f093fb, #f5576c);
  box-shadow: 0 8px 24px rgba(245, 87, 108, 0.4);
}

.mega-stat-icon.gradient-info {
  background: linear-gradient(135deg, #667eea, #764ba2);
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.4);
}

.mega-stat-value {
  font-size: 3rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  line-height: 1;
  margin-bottom: 8px;
}

.mega-stat-label {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

/* ===== MAP SECTION ===== */
.map-section-epic {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a0a0f 0%, #1a1a2e 100%);
  position: relative;
}

.map-header-epic {
  background: linear-gradient(135deg, #667eea, #764ba2);
  padding: 3rem 2rem;
  position: relative;
  overflow: hidden;
}

.map-header-epic::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1), transparent 70%);
  animation: header-float 6s ease-in-out infinite;
}

@keyframes header-float {

  0%,
  100% {
    transform: translate(0, 0);
  }

  50% {
    transform: translate(-30px, -30px);
  }
}

.back-btn-epic {
  position: absolute;
  top: 20px;
  left: 20px;
  background: rgba(255, 255, 255, 0.2) !important;
  backdrop-filter: blur(10px);
  z-index: 10;
}

.map-title-container {
  text-align: center;
  position: relative;
  z-index: 1;
}

.map-main-title {
  font-size: 3rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.map-main-subtitle {
  font-size: 1.2rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 500;
}

.map-content-container {
  max-width: 1400px;
}

/* ===== SEARCH BOX ===== */
.search-box-epic {
  background: rgba(255, 255, 255, 0.95);
  border-radius: 24px;
  padding: 2rem;
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.2);
}

.search-box-header {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
}

.search-box-title {
  font-size: 1.5rem;
  font-weight: 800;
  color: #1a1a2e;
  font-family: 'Space Grotesk', sans-serif;
}

.search-box-subtitle {
  font-size: 0.9rem;
  color: #666;
  font-weight: 500;
}

.search-input-epic>>>.v-input__control {
  border-radius: 16px !important;
}

.search-btn-epic {
  background: linear-gradient(135deg, #667eea, #764ba2) !important;
  color: white !important;
  font-weight: 700;
  border-radius: 14px !important;
  text-transform: none;
  letter-spacing: 0.5px;
}

.search-btn-epic:hover {
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.4) !important;
}

/* ===== MAP WRAPPER ===== */
.map-wrapper-epic {
  position: relative;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 24px 64px rgba(0, 0, 0, 0.3);
}

.map-element-epic {
  width: 100%;
  height: 600px;
}

.map-controls {
  position: absolute;
  top: 20px;
  right: 20px;
  z-index: 1000;
}

.map-control-btn {
  background: white !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* ===== RESULTS ===== */
.results-header-epic {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 1.5rem 2rem;
  backdrop-filter: blur(20px);
  margin-bottom: 2rem;
}

.results-title-content {
  display: flex;
  align-items: center;
}

.results-main-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  margin-bottom: 4px;
}

.results-main-subtitle {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.7);
  margin: 0;
}

.gyms-grid-epic {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.gym-card-epic {
  background: rgba(255, 255, 255, 0.95);
  border-radius: 20px;
  padding: 1.5rem;
  position: relative;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 2px solid transparent;
}

.gym-card-epic::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.gym-card-epic:hover::before,
.gym-card-epic.selected::before {
  opacity: 1;
}

.gym-card-epic:hover,
.gym-card-epic.selected {
  transform: translateY(-8px);
  border-color: #667eea;
  box-shadow: 0 16px 48px rgba(102, 126, 234, 0.3);
}

.gym-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.gym-icon-wrapper {
  width: 64px;
  height: 64px;
  border-radius: 16px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.4);
}

.gym-badge {
  font-weight: 700;
}

.gym-card-body {
  margin-bottom: 1rem;
}

.gym-name-epic {
  font-size: 1.25rem;
  font-weight: 800;
  color: #1a1a2e;
  margin-bottom: 8px;
  font-family: 'Space Grotesk', sans-serif;
}

.gym-address-epic {
  font-size: 0.95rem;
  color: #666;
  display: flex;
  align-items: center;
  gap: 6px;
  margin: 0;
}

.gym-extra-info {
  margin-top: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.gym-extra-line {
  font-size: 0.85rem;
  color: #555;
  display: flex;
  align-items: center;
  gap: 6px;
  margin: 0;
}

.gym-link {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.gym-link:hover {
  text-decoration: underline;
}

.gym-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 1rem;
  border-top: 1px solid rgba(0, 0, 0, 0.1);
}

.gym-distance {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 0.9rem;
  color: #666;
  font-weight: 600;
}

.gym-action-btn {
  font-weight: 700;
}

.gym-card-glow {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #667eea, #764ba2);
  transform: scaleX(0);
  transform-origin: left;
  transition: transform 0.3s ease;
}

.gym-card-epic:hover .gym-card-glow,
.gym-card-epic.selected .gym-card-glow {
  transform: scaleX(1);
}

/* ===== HERO RIGHT (VISUAL) REDISEÑADO ===== */
.hero-right {
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.visual-container-epic {
  position: relative;
  width: 100%;
  max-width: 650px;
  height: 650px;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Anillos orbitales */
.orbital-rings {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ring {
  position: absolute;
  border-radius: 50%;
  border: 2px solid rgba(102, 126, 234, 0.2);
  animation: rotate-ring 20s linear infinite;
}

.ring-1 {
  width: 400px;
  height: 400px;
  border-color: rgba(102, 126, 234, 0.3);
  animation-duration: 25s;
}

.ring-2 {
  width: 500px;
  height: 500px;
  border-color: rgba(255, 193, 7, 0.2);
  animation-duration: 30s;
  animation-direction: reverse;
}

.ring-3 {
  width: 600px;
  height: 600px;
  border-color: rgba(245, 87, 108, 0.2);
  animation-duration: 35s;
}

@keyframes rotate-ring {
  from {
    transform: rotate(0deg);
  }

  to {
    transform: rotate(360deg);
  }
}

/* Centro visual principal */
.main-visual-hub {
  position: relative;
  z-index: 5;
  animation: fade-in-scale 1s ease-out 0.4s backwards;
  margin-left: 80px;
  /* Desplazar el móvil hacia la derecha */
}

@keyframes fade-in-scale {
  from {
    opacity: 0;
    transform: scale(0.85) rotateY(-15deg);
  }

  to {
    opacity: 1;
    transform: scale(1) rotateY(0deg);
  }
}

.hub-glow-massive {
  position: absolute;
  inset: -60px;
  background: radial-gradient(circle,
      rgba(102, 126, 234, 0.4) 0%,
      rgba(255, 193, 7, 0.2) 40%,
      transparent 70%);
  animation: massive-pulse 4s ease-in-out infinite;
  filter: blur(40px);
}

@keyframes massive-pulse {

  0%,
  100% {
    opacity: 0.6;
    transform: scale(1);
  }

  50% {
    opacity: 1;
    transform: scale(1.15);
  }
}

/* Mockup de teléfono 3D */
.phone-mockup-3d {
  position: relative;
  width: 320px;
  height: 650px;
  perspective: 1000px;
  animation: phone-float 6s ease-in-out infinite;
}

@keyframes phone-float {

  0%,
  100% {
    transform: translateY(0px) rotateY(5deg);
  }

  50% {
    transform: translateY(-20px) rotateY(-5deg);
  }
}

.phone-frame {
  width: 100%;
  height: 100%;
  background: linear-gradient(145deg, #1a1a2e, #0f0f1e);
  border-radius: 45px;
  padding: 12px;
  box-shadow:
    0 30px 80px rgba(0, 0, 0, 0.6),
    inset 0 0 0 2px rgba(255, 255, 255, 0.1);
  position: relative;
  transform-style: preserve-3d;
}

.phone-notch {
  position: absolute;
  top: 12px;
  left: 50%;
  transform: translateX(-50%);
  width: 140px;
  height: 28px;
  background: #0a0a0f;
  border-radius: 0 0 20px 20px;
  z-index: 10;
}

.phone-screen {
  width: 100%;
  height: 100%;
  border-radius: 35px;
  overflow: hidden;
  position: relative;
  background: #000;
}

.phone-content {
  width: 100%;
  height: 100%;
}

.screen-shine {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg,
      transparent 0%,
      rgba(255, 255, 255, 0.1) 45%,
      rgba(255, 255, 255, 0.2) 50%,
      rgba(255, 255, 255, 0.1) 55%,
      transparent 100%);
  animation: screen-shine-move 3s ease-in-out infinite;
}

@keyframes screen-shine-move {
  0% {
    transform: translateX(-100%) translateY(-100%) rotate(30deg);
  }

  100% {
    transform: translateX(100%) translateY(100%) rotate(30deg);
  }
}

/* Métricas flotantes rediseñadas */
.floating-metrics {
  position: absolute;
  inset: 0;
  z-index: 10;
}

.metric-card {
  position: absolute;
  background: rgba(20, 20, 35, 0.95);
  backdrop-filter: blur(25px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 20px;
  padding: 1.25rem;
  box-shadow: 0 15px 45px rgba(0, 0, 0, 0.4);
  animation: metric-float 4s ease-in-out infinite;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
}

.metric-card:hover {
  transform: translateY(-8px) scale(1.05);
  border-color: rgba(102, 126, 234, 0.6);
  box-shadow: 0 20px 60px rgba(102, 126, 234, 0.4);
}

@keyframes metric-float {

  0%,
  100% {
    transform: translateY(0px);
  }

  50% {
    transform: translateY(-12px);
  }
}

.metric-glow {
  position: absolute;
  inset: -10px;
  border-radius: 22px;
  filter: blur(15px);
  opacity: 0.6;
  z-index: -1;
}

.metric-glow-yellow {
  background: radial-gradient(circle, rgba(255, 193, 7, 0.6), transparent 70%);
}

.metric-glow-purple {
  background: radial-gradient(circle, rgba(102, 126, 234, 0.6), transparent 70%);
}

.metric-glow-blue {
  background: radial-gradient(circle, rgba(79, 172, 254, 0.6), transparent 70%);
}

.metric-glow-orange {
  background: radial-gradient(circle, rgba(255, 87, 51, 0.6), transparent 70%);
}

.metric-glow-gold {
  background: radial-gradient(circle, rgba(255, 215, 0, 0.6), transparent 70%);
}

.metric-glow-green {
  background: radial-gradient(circle, rgba(0, 242, 254, 0.6), transparent 70%);
}

/* Métrica XP */
.metric-xp {
  top: 8%;
  right: -8%;
  /* Ajustado para seguir al móvil */
  animation-delay: 0s;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.metric-icon-container {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  box-shadow: 0 8px 24px rgba(255, 193, 7, 0.5);
}

.icon-pulse {
  position: absolute;
  inset: -5px;
  border-radius: 18px;
  border: 3px solid #ffd700;
  animation: pulse-ring-strong 2s ease-out infinite;
}

@keyframes pulse-ring-strong {
  0% {
    transform: scale(1);
    opacity: 1;
  }

  100% {
    transform: scale(1.6);
    opacity: 0;
  }
}

.metric-data {
  display: flex;
  flex-direction: column;
  gap: 4px;
  color: white;
}

.metric-value {
  font-size: 2rem;
  font-weight: 900;
  font-family: 'Space Grotesk', sans-serif;
  line-height: 1;
  background: linear-gradient(135deg, #ffd700, #fff);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.metric-label {
  font-size: 0.85rem;
  opacity: 0.8;
  font-weight: 600;
  color: white;
}

.metric-badge {
  display: inline-block;
  padding: 4px 8px;
  background: rgba(255, 193, 7, 0.2);
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 800;
  color: #ffd700;
  width: fit-content;
}

/* Métrica de Nivel */
.metric-level {
  top: 30%;
  left: -3%;
  /* Ajustado para estar más cerca del móvil */
  animation-delay: 0.5s;
  background: linear-gradient(135deg, #667eea, #764ba2);
  padding: 1.5rem;
  text-align: center;
}

.level-display {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.level-crown {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  animation: crown-bounce 2s ease-in-out infinite;
}

@keyframes crown-bounce {

  0%,
  100% {
    transform: translateY(0px) rotate(0deg);
  }

  50% {
    transform: translateY(-5px) rotate(10deg);
  }
}

.level-number {
  font-size: 3rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
}

.level-progress-bar {
  width: 100%;
  height: 12px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 8px;
}

.level-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #ffd700, #fff);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-right: 8px;
  animation: progress-fill-smooth 1.5s ease-out;
  position: relative;
  overflow: hidden;
}

@keyframes progress-fill-smooth {
  from {
    width: 0%;
  }
}

.level-progress-fill::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.5), transparent);
  animation: progress-shimmer 2s ease-in-out infinite;
}

@keyframes progress-shimmer {
  0% {
    transform: translateX(-100%);
  }

  100% {
    transform: translateX(200%);
  }
}

.progress-percentage {
  font-size: 0.7rem;
  font-weight: 800;
  color: white;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
}

/* Métrica de Sala */
.metric-room {
  bottom: 20%;
  left: -2%;
  /* Ajustado */
  animation-delay: 1s;
  background: linear-gradient(135deg, #4facfe, #00f2fe);
  min-width: 200px;
}

.room-status {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}

.live-indicator {
  width: 10px;
  height: 10px;
  background: #ff4444;
  border-radius: 50%;
  animation: live-pulse 1.5s ease-in-out infinite;
  box-shadow: 0 0 0 0 rgba(255, 68, 68, 0.7);
}

@keyframes live-pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(255, 68, 68, 0.7);
  }

  50% {
    box-shadow: 0 0 0 8px rgba(255, 68, 68, 0);
  }

  100% {
    box-shadow: 0 0 0 0 rgba(255, 68, 68, 0);
  }
}

.live-text {
  font-size: 0.75rem;
  font-weight: 800;
  color: white;
  letter-spacing: 1px;
}

.room-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 1.1rem;
  font-weight: 700;
  color: white;
  margin-bottom: 12px;
}

.room-members {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}

.member-avatars {
  display: flex;
  margin-left: 0;
}

.member-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border: 3px solid rgba(255, 255, 255, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: -12px;
  transition: all 0.3s ease;
}

.member-avatar:first-child {
  margin-left: 0;
}

.member-avatar:hover {
  transform: translateY(-4px) scale(1.1);
  border-color: white;
  z-index: 10;
}

.member-count {
  font-size: 0.85rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  background: rgba(255, 255, 255, 0.2);
  padding: 4px 12px;
  border-radius: 20px;
}

/* Métrica de Racha */
.metric-streak {
  top: 58%;
  right: -6%;
  /* Ajustado */
  animation-delay: 1.5s;
  background: linear-gradient(135deg, #ff5733, #ff8c00);
  text-align: center;
  min-width: 140px;
}

.streak-fire {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 12px;
}

.fire-icon {
  color: #ffd700;
  animation: fire-flicker 1.5s ease-in-out infinite;
  filter: drop-shadow(0 0 20px rgba(255, 193, 7, 0.8));
}

@keyframes fire-flicker {

  0%,
  100% {
    transform: scale(1) rotate(-5deg);
    filter: drop-shadow(0 0 20px rgba(255, 193, 7, 0.8));
  }

  25% {
    transform: scale(1.05) rotate(5deg);
    filter: drop-shadow(0 0 25px rgba(255, 140, 0, 1));
  }

  50% {
    transform: scale(0.98) rotate(-3deg);
    filter: drop-shadow(0 0 18px rgba(255, 193, 7, 0.7));
  }

  75% {
    transform: scale(1.03) rotate(3deg);
    filter: drop-shadow(0 0 22px rgba(255, 87, 51, 0.9));
  }
}

.fire-particles {
  position: absolute;
  inset: 0;
}

.fire-particles .particle {
  position: absolute;
  width: 6px;
  height: 6px;
  background: radial-gradient(circle, #ffd700, #ff8c00);
  border-radius: 50%;
  bottom: 0;
  left: 50%;
  animation: fire-particle-rise 2s ease-out infinite;
}

.fire-particles .particle:nth-child(1) {
  animation-delay: 0s;
  left: 45%;
}

.fire-particles .particle:nth-child(2) {
  animation-delay: 0.3s;
  left: 55%;
}

.fire-particles .particle:nth-child(3) {
  animation-delay: 0.6s;
  left: 40%;
}

.fire-particles .particle:nth-child(4) {
  animation-delay: 0.9s;
  left: 60%;
}

.fire-particles .particle:nth-child(5) {
  animation-delay: 1.2s;
  left: 50%;
}

.fire-particles .particle:nth-child(6) {
  animation-delay: 1.5s;
  left: 48%;
}

.fire-particles .particle:nth-child(7) {
  animation-delay: 1.8s;
  left: 52%;
}

.fire-particles .particle:nth-child(8) {
  animation-delay: 0.4s;
  left: 57%;
}

@keyframes fire-particle-rise {
  0% {
    transform: translateY(0) scale(1);
    opacity: 1;
  }

  100% {
    transform: translateY(-60px) scale(0);
    opacity: 0;
  }
}

.streak-count {
  font-size: 2.5rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
  margin-bottom: 4px;
  letter-spacing: 2px;
}

/* Métrica de Monedas */
.metric-coins {
  top: 42%;
  right: 0%;
  /* Ajustado para estar más cerca del móvil */
  animation-delay: 2s;
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  text-align: center;
  min-width: 130px;
}

.coins-animation {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
  animation: coin-spin 3s linear infinite;
}

@keyframes coin-spin {

  0%,
  100% {
    transform: rotateY(0deg);
  }

  50% {
    transform: rotateY(180deg);
  }
}

.coin-sparkles {
  position: absolute;
  inset: -10px;
}

.coin-sparkles .sparkle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: white;
  border-radius: 50%;
  animation: sparkle-twinkle 1.5s ease-in-out infinite;
}

.coin-sparkles .sparkle:nth-child(1) {
  top: 0;
  left: 50%;
  animation-delay: 0s;
}

.coin-sparkles .sparkle:nth-child(2) {
  top: 25%;
  right: 0;
  animation-delay: 0.2s;
}

.coin-sparkles .sparkle:nth-child(3) {
  bottom: 0;
  left: 50%;
  animation-delay: 0.4s;
}

.coin-sparkles .sparkle:nth-child(4) {
  top: 25%;
  left: 0;
  animation-delay: 0.6s;
}

.coin-sparkles .sparkle:nth-child(5) {
  top: 75%;
  right: 10%;
  animation-delay: 0.8s;
}

.coin-sparkles .sparkle:nth-child(6) {
  top: 75%;
  left: 10%;
  animation-delay: 1s;
}

@keyframes sparkle-twinkle {

  0%,
  100% {
    transform: scale(0);
    opacity: 0;
  }

  50% {
    transform: scale(1.5);
    opacity: 1;
  }
}

.coins-earned {
  font-size: 2rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
  margin-bottom: 4px;
}

/* Métrica Circular */
.metric-circular {
  top: 12%;
  left: 8%;
  /* Ajustado para estar más cerca */
  animation-delay: 0.8s;
  background: linear-gradient(135deg, #00f2fe, #4facfe);
  padding: 1rem;
  width: 150px;
  height: 150px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  position: relative;
}

.progress-ring {
  width: 120px;
  height: 120px;
  transform: rotate(-90deg);
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) rotate(-90deg);
}

.progress-ring-bg {
  fill: none;
  stroke: rgba(255, 255, 255, 0.2);
  stroke-width: 8;
}

.progress-ring-fill {
  fill: none;
  stroke: white;
  stroke-width: 8;
  stroke-linecap: round;
  animation: circle-progress-draw 2s ease-out forwards;
}

@keyframes circle-progress-draw {
  from {
    stroke-dashoffset: 327;
  }

  to {
    stroke-dashoffset: 82;
  }
}

.circular-content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.circular-percentage {
  font-size: 1.75rem;
  font-weight: 900;
  color: white;
  font-family: 'Space Grotesk', sans-serif;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.metric-label-small {
  position: absolute;
  bottom: 12px;
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1px;
}

/* Partículas ambientales */
.ambient-particles {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
}

.ambient-particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(102, 126, 234, 0.6);
  border-radius: 50%;
  animation: ambient-float 15s ease-in-out infinite;
}

.ambient-particle:nth-child(odd) {
  background: rgba(255, 193, 7, 0.5);
}

/* Estilos del Paginador Épico */
.pagination-container-epic {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 24px;
  margin-top: 48px;
  padding: 32px;
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.05) 0%, rgba(168, 85, 247, 0.05) 100%);
  border-radius: 24px;
  border: 1px solid rgba(99, 102, 241, 0.1);
  position: relative;
  overflow: hidden;
}

.pagination-container-epic::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 50% 50%, rgba(99, 102, 241, 0.1) 0%, transparent 70%);
  pointer-events: none;
}

.pagination-info {
  display: flex;
  align-items: center;
  font-size: 16px;
  font-weight: 600;
  color: #6366f1;
  background: rgba(255, 255, 255, 0.8);
  padding: 12px 24px;
  border-radius: 50px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(99, 102, 241, 0.2);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.1);
  z-index: 1;
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 8px;
  z-index: 1;
}

.pagination-btn {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(99, 102, 241, 0.1);
}

.pagination-btn:not(:disabled):hover {
  background: linear-gradient(135deg, #6366f1 0%, #a855f7 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(99, 102, 241, 0.3);
}

.pagination-btn:not(:disabled):hover :deep(.v-icon) {
  color: white !important;
}

.pagination-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.page-numbers {
  display: flex;
  gap: 6px;
  padding: 0 12px;
}

.page-number-btn {
  min-width: 48px;
  height: 48px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 16px;
  transition: all 0.3s ease;
  position: relative;
}

.page-number-btn:not(.active) {
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(99, 102, 241, 0.1);
  color: #64748b;
}

.page-number-btn:not(.active):hover {
  background: rgba(99, 102, 241, 0.1);
  color: #6366f1;
  transform: translateY(-2px);
  border-color: rgba(99, 102, 241, 0.3);
}

.page-number-btn.active {
  background: linear-gradient(135deg, #6366f1 0%, #a855f7 100%);
  color: white;
  box-shadow: 0 8px 24px rgba(99, 102, 241, 0.4);
  transform: scale(1.05);
  border: none;
}

.page-number-btn.active::before {
  content: '';
  position: absolute;
  inset: -3px;
  border-radius: 14px;
  background: linear-gradient(135deg, #6366f1, #a855f7);
  opacity: 0.3;
  filter: blur(8px);
  z-index: -1;
}

.pagination-jump {
  display: flex;
  align-items: center;
  gap: 12px;
  z-index: 1;
}

.jump-label {
  font-size: 14px;
  font-weight: 600;
  color: #64748b;
}

.page-select {
  width: 100px;
}

.page-select :deep(.v-field) {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  border: 1px solid rgba(99, 102, 241, 0.2);
  transition: all 0.3s ease;
}

.page-select :deep(.v-field:hover) {
  border-color: #6366f1;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.15);
}

.page-select :deep(.v-field--focused) {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1);
}

/* Responsive */
@media (max-width: 768px) {
  .pagination-container-epic {
    padding: 24px 16px;
    gap: 20px;
  }

  .pagination-controls {
    flex-wrap: wrap;
    justify-content: center;
  }

  .page-numbers {
    order: 3;
    width: 100%;
    justify-content: center;
    padding: 12px 0 0 0;
    border-top: 1px solid rgba(99, 102, 241, 0.1);
  }

  .pagination-btn,
  .page-number-btn {
    width: 40px;
    height: 40px;
    min-width: 40px;
  }

  .pagination-info {
    font-size: 14px;
    padding: 10px 20px;
  }

  .pagination-jump {
    flex-direction: column;
    gap: 8px;
  }
}

/* Animaciones */
@keyframes pageEnter {
  from {
    opacity: 0;
    transform: translateY(20px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.gyms-grid-epic {
  animation: pageEnter 0.5s ease-out;
}

.ambient-particle:nth-child(1) {
  top: 10%;
  left: 15%;
  animation-delay: 0s;
  animation-duration: 12s;
}

.ambient-particle:nth-child(2) {
  top: 25%;
  right: 20%;
  animation-delay: 1s;
  animation-duration: 14s;
}

.ambient-particle:nth-child(3) {
  bottom: 30%;
  left: 10%;
  animation-delay: 2s;
  animation-duration: 16s;
}

.ambient-particle:nth-child(4) {
  top: 60%;
  right: 15%;
  animation-delay: 3s;
  animation-duration: 13s;
}

.ambient-particle:nth-child(5) {
  bottom: 15%;
  right: 25%;
  animation-delay: 1.5s;
  animation-duration: 15s;
}

.ambient-particle:nth-child(6) {
  top: 40%;
  left: 20%;
  animation-delay: 2.5s;
  animation-duration: 17s;
}

.ambient-particle:nth-child(7) {
  bottom: 45%;
  right: 30%;
  animation-delay: 0.5s;
  animation-duration: 14s;
}

.ambient-particle:nth-child(8) {
  top: 75%;
  left: 25%;
  animation-delay: 3.5s;
  animation-duration: 12s;
}

.ambient-particle:nth-child(9) {
  top: 20%;
  right: 35%;
  animation-delay: 1.8s;
  animation-duration: 16s;
}

.ambient-particle:nth-child(10) {
  bottom: 60%;
  left: 30%;
  animation-delay: 2.8s;
  animation-duration: 13s;
}

.ambient-particle:nth-child(11) {
  top: 50%;
  right: 10%;
  animation-delay: 0.8s;
  animation-duration: 15s;
}

.ambient-particle:nth-child(12) {
  bottom: 20%;
  left: 35%;
  animation-delay: 3.2s;
  animation-duration: 14s;
}

.ambient-particle:nth-child(13) {
  top: 35%;
  right: 40%;
  animation-delay: 1.2s;
  animation-duration: 17s;
}

.ambient-particle:nth-child(14) {
  bottom: 50%;
  right: 5%;
  animation-delay: 2.2s;
  animation-duration: 12s;
}

.ambient-particle:nth-child(15) {
  top: 80%;
  left: 40%;
  animation-delay: 0.3s;
  animation-duration: 16s;
}

@keyframes ambient-float {

  0%,
  100% {
    transform: translate(0, 0) scale(1);
    opacity: 0.3;
  }

  25% {
    transform: translate(20px, -30px) scale(1.2);
    opacity: 0.6;
  }

  50% {
    transform: translate(-15px, -60px) scale(0.8);
    opacity: 0.4;
  }

  75% {
    transform: translate(25px, -40px) scale(1.1);
    opacity: 0.7;
  }
}

/* ===== RESPONSIVE ===== */
@media (max-width: 1264px) {
  .visual-container-epic {
    height: 550px;
  }

  .main-visual-hub {
    margin-left: 50px;
    /* Reducir desplazamiento en tablets */
  }

  .phone-mockup-3d {
    width: 280px;
    height: 570px;
  }

  .metric-card {
    padding: 1rem;
  }

  .metric-xp {
    right: -5%;
  }

  .metric-level {
    left: -1%;
  }

  .metric-room {
    left: 0%;
  }

  .metric-streak {
    right: -4%;
  }

  .metric-circular {
    width: 130px;
    height: 130px;
    left: 6%;
  }

  .progress-ring {
    width: 100px;
    height: 100px;
  }
}

@media (max-width: 960px) {
  .visual-container-epic {
    height: 450px;
    margin-top: 3rem;
  }

  .main-visual-hub {
    margin-left: 0;
    /* Centrar en móvil */
  }

  .phone-mockup-3d {
    width: 240px;
    height: 490px;
  }

  .orbital-rings .ring {
    display: none;
  }

  .metric-card {
    padding: 0.75rem 1rem;
    border-radius: 16px;
  }

  .metric-xp {
    top: 5%;
    right: 0;
  }

  .metric-level {
    top: 25%;
    left: -3%;
    padding: 1rem;
  }

  .level-display {
    gap: 0.75rem;
  }

  .level-crown {
    width: 48px;
    height: 48px;
  }

  .level-number {
    font-size: 2.5rem;
  }

  .metric-room {
    bottom: 18%;
    left: -2%;
    min-width: 180px;
  }

  .metric-streak {
    top: 60%;
    right: 0;
    min-width: 120px;
  }

  .streak-count {
    font-size: 2rem;
  }

  .metric-coins {
    top: 45%;
    right: 5%;
    min-width: 110px;
  }

  .metric-circular {
    top: 8%;
    left: 5%;
    width: 110px;
    height: 110px;
  }

  .progress-ring {
    width: 85px;
    height: 85px;
  }

  .circular-percentage {
    font-size: 1.4rem;
  }
}

@media (max-width: 600px) {
  .visual-container-epic {
    height: 400px;
    max-width: 100%;
  }

  .phone-mockup-3d {
    width: 200px;
    height: 410px;
  }

  .phone-frame {
    border-radius: 35px;
    padding: 10px;
  }

  .phone-screen {
    border-radius: 28px;
  }

  .phone-notch {
    width: 120px;
    height: 24px;
  }

  /* Ocultar algunas métricas en móvil para mejor legibilidad */
  .metric-circular {
    display: none;
  }

  .metric-coins {
    display: none;
  }

  .metric-xp {
    top: 3%;
    right: -2%;
    padding: 0.75rem;
    flex-direction: column;
    text-align: center;
  }

  .metric-icon-container {
    width: 48px;
    height: 48px;
  }

  .metric-value {
    font-size: 1.5rem;
  }

  .metric-badge {
    font-size: 0.65rem;
  }

  .metric-level {
    top: 22%;
    left: 0;
    padding: 0.75rem 1rem;
  }

  .level-display {
    flex-direction: column;
    gap: 0.5rem;
  }

  .level-crown {
    width: 40px;
    height: 40px;
  }

  .level-number {
    font-size: 2rem;
  }

  .level-progress-bar {
    height: 10px;
  }

  .metric-room {
    bottom: 15%;
    left: 0;
    min-width: 160px;
    padding: 0.75rem;
  }

  .room-title {
    font-size: 0.95rem;
  }

  .member-avatar {
    width: 32px;
    height: 32px;
  }

  .member-count {
    font-size: 0.75rem;
    padding: 3px 10px;
  }

  .metric-streak {
    top: 58%;
    right: -2%;
    min-width: 100px;
    padding: 0.75rem;
  }

  .fire-icon {
    font-size: 42px !important;
  }

  .streak-count {
    font-size: 1.75rem;
  }

  .ambient-particles {
    display: none;
  }
}

/* Mejoras adicionales de rendimiento */
@media (prefers-reduced-motion: reduce) {

  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}

/* Modo oscuro mejorado */
@media (prefers-color-scheme: dark) {
  .metric-card {
    background: rgba(10, 10, 20, 0.98);
    border-color: rgba(255, 255, 255, 0.2);
  }

  .phone-frame {
    background: linear-gradient(145deg, #0a0a14, #050508);
  }
}

/* ===== RESPONSIVE ===== */
@media (max-width: 1264px) {
  .features-grid {
    grid-template-columns: 1fr;
  }

  .visual-container {
    height: 500px;
  }
}

@media (max-width: 960px) {
  .hero-content {
    padding: 60px 20px;
  }

  .title-line-1,
  .title-line-3 {
    font-size: 2.5rem;
  }

  .title-line-2 {
    font-size: 3rem;
  }

  .action-buttons {
    flex-direction: column;
  }

  .epic-btn {
    width: 100%;
  }

  .epic-btn.secondary {
    margin-left: 0 !important;
  }

  .live-stats {
    width: 100%;
    justify-content: space-around;
  }

  .stats-row {
    gap: 1.5rem;
  }

  .mega-stat-value {
    font-size: 2.5rem;
  }

  .float-card {
    display: none;
  }

  .circular-stats {
    display: none;
  }

  .map-main-title {
    font-size: 2rem;
    flex-direction: column;
  }

  .gyms-grid-epic {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .super-badge {
    font-size: 0.8rem;
    padding: 10px 16px;
  }

  .title-line-1,
  .title-line-3 {
    font-size: 2rem;
  }

  .title-line-2 {
    font-size: 2.5rem;
  }

  .epic-subtitle {
    font-size: 1rem;
  }

  .btn-content {
    padding: 1rem 1.5rem;
  }

  .btn-main-text {
    font-size: 1rem;
  }

  .btn-sub-text {
    font-size: 0.7rem;
  }

  .visual-container {
    height: 400px;
  }

  .mega-stat-icon {
    width: 60px;
    height: 60px;
  }

  .mega-stat-value {
    font-size: 2rem;
  }

  .mega-stat-label {
    font-size: 0.9rem;
  }

  .search-box-epic {
    padding: 1.5rem;
  }

  .map-element-epic {
    height: 400px;
  }
}

/* ===== ALL FEATURES SECTION ===== */
.all-features-section {
  padding-top: 4rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.features-section-title {
  font-family: 'Space Grotesk', sans-serif;
  font-size: clamp(1.75rem, 3vw, 2.5rem);
  font-weight: 800;
  color: white;
  margin-bottom: 0.5rem;
}

.features-section-subtitle {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 500;
}

.features-categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 1.5rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  backdrop-filter: blur(10px);
}

.category-card:hover {
  transform: translateY(-6px);
  border-color: rgba(102, 126, 234, 0.5);
  box-shadow: 0 16px 48px rgba(102, 126, 234, 0.2);
  background: rgba(255, 255, 255, 0.05);
}

.premium-card {
  border-color: rgba(255, 193, 7, 0.2);
  background: linear-gradient(135deg, rgba(255, 193, 7, 0.05), rgba(139, 92, 246, 0.05));
}

.premium-card:hover {
  border-color: rgba(255, 193, 7, 0.5);
  box-shadow: 0 16px 48px rgba(255, 193, 7, 0.2);
}

.category-card-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.25rem;
}

.category-card-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.gradient-purple {
  background: linear-gradient(135deg, #667eea, #764ba2);
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.4);
}

.gradient-blue {
  background: linear-gradient(135deg, #4facfe, #00f2fe);
  box-shadow: 0 8px 24px rgba(79, 172, 254, 0.4);
}

.gradient-pink {
  background: linear-gradient(135deg, #f093fb, #f5576c);
  box-shadow: 0 8px 24px rgba(245, 87, 108, 0.4);
}

.gradient-gold {
  background: linear-gradient(135deg, #ffd700, #ff8c00);
  box-shadow: 0 8px 24px rgba(255, 140, 0, 0.4);
}

.gradient-green {
  background: linear-gradient(135deg, #4ade80, #22c55e);
  box-shadow: 0 8px 24px rgba(74, 222, 128, 0.4);
}

.category-card-title {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 1.1rem;
  font-weight: 700;
  color: white;
}

.premium-mini-badge {
  margin-left: auto;
  font-size: 0.6rem;
  font-weight: 800;
  color: #0a0a0f;
  background: linear-gradient(135deg, #ffd700, #ffb700);
  padding: 0.2rem 0.5rem;
  border-radius: 50px;
  letter-spacing: 1px;
}

.category-features-list {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.category-feature-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 12px;
  transition: all 0.2s ease;
}

.category-feature-item:hover {
  background: rgba(0, 0, 0, 0.35);
}

.cf-content {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.cf-title {
  color: #e2e8f0;
  font-size: 0.9rem;
  font-weight: 700;
  line-height: 1.2;
}

.cf-desc {
  color: rgba(255, 255, 255, 0.5);
  font-size: 0.8rem;
  line-height: 1.3;
}
</style>
