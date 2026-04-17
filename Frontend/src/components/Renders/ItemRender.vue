<script setup lang="ts">
import { computed, onMounted, ref, watchEffect } from 'vue'
import { useItemStore } from '@/stores/itemStore'
import { useUserStore } from '@/stores/userStore'
import { usePurchaseStore } from '@/stores/PurchaseStore'
import type { Item } from '@/components/Models/Item'

const itemStore = useItemStore()
const userStore = useUserStore()
const purchaseStore = usePurchaseStore()

const loggedUser = ref(userStore.loggedUser)
watchEffect(() => {
  loggedUser.value = userStore.loggedUser
})

onMounted(() => {
  itemStore.fetchDailyStrengthItems()
  itemStore.fetchDailyEnduranceItems()
  itemStore.fetchDailyGeneralItems()
})

const dailyRotation = computed(() => itemStore.generalItems)
const strengthItems = computed(() => itemStore.strengthItems)
const enduranceItems = computed(() => itemStore.enduranceItems)

const showDialog = ref(false)
const selectedItem = ref<Item | null>(null)

// Snackbar state
const snackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')

const showSnackbar = (message: string, color: string = 'success') => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbar.value = true
}

const openPopup = (item: Item) => {
  selectedItem.value = item
  showDialog.value = true
}

const closePopup = () => {
  showDialog.value = false
  selectedItem.value = null
}

const handleBuy = async () => {
  const userId = loggedUser.value?.id
  const item = selectedItem.value
  if (!userId || !item) return

  try {
    const purchase = await purchaseStore.addPurchase(userId, item.id, item.price)
    if (purchase && purchase.id) {
      showSnackbar("Compra realizada correctamente", "success")
      await userStore.refreshLoggedUser()
      closePopup()
    } else {
      throw new Error("Respuesta inválida del servidor")
    }
  } catch (error: any) {
    const message = error?.data?.message
    if (message?.includes("no tiene suficiente oro")) {
      showSnackbar("No tienes suficiente oro para realizar esta compra.", "error")
    } else {
      showSnackbar(message || "Hubo un problema al realizar la compra.", "error")
    }
    closePopup()
  }
}

const canAfford = (price: number) => {
  return (loggedUser.value?.gold || 0) >= price
}

const getItemIcon = (type: string) => {
  if (type === 'Strength') return 'mdi-dumbbell'
  if (type === 'Endurance') return 'mdi-run-fast'
  return 'mdi-star-circle'
}

const getItemColor = (type: string) => {
  if (type === 'Strength') return '#FF4757'
  if (type === 'Endurance') return '#00D2FF'
  return '#FFD700'
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.style.display = 'none'
  const fallbackIcon = img.nextElementSibling as HTMLElement
  if (fallbackIcon) {
    fallbackIcon.style.display = 'flex'
  }
}
</script>

<template>
  <v-container fluid class="items-container pa-6">
    <!-- Sección: Rotación diaria -->
    <section class="shop-section mb-10">
      <div class="section-header">
        <div class="header-icon-wrapper daily">
          <v-icon size="32" color="black">mdi-clock-fast</v-icon>
        </div>
        <div>
          <h2 class="section-title">{{ $t('Rotacion Diaria') }}</h2>
          <p class="section-subtitle">{{ $t('Ofertas limitadas que cambian cada día') }}</p>
        </div>
        <div class="daily-badge">
          <v-icon size="20" color="#FFD700">mdi-timer-sand</v-icon>
          <span>24h</span>
        </div>
      </div>

      <!-- justify="center" centra los items cuando hay pocos -->
      <v-row class="items-grid" justify="center">
        <v-col
          v-for="item in dailyRotation"
          :key="item.id"
          cols="12"
          sm="6"
          md="6"
          lg="4"
        >
          <div class="item-card daily-item" @click="openPopup(item)">
            <div class="item-glow" :style="{ background: `radial-gradient(circle, ${getItemColor(item.type)}40 0%, transparent 70%)` }"></div>
            <div class="daily-ribbon">
              <span>DIARIO</span>
            </div>

            <div class="item-image-wrapper" :style="{ borderColor: getItemColor(item.type), boxShadow: `0 0 20px ${getItemColor(item.type)}40` }">
              <div class="item-image-inner">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.name"
                  class="item-image"
                  @error="handleImageError"
                />
                <div class="item-icon-fallback">
                  <v-icon :color="getItemColor(item.type)" size="40">{{ getItemIcon(item.type) }}</v-icon>
                </div>
              </div>
            </div>

            <div class="item-content">
              <h3 class="item-name">{{ item.name }}</h3>

              <div class="item-stats">
                <div class="stat-badge" :style="{ background: `${getItemColor(item.type)}20`, borderColor: `${getItemColor(item.type)}60` }">
                  <v-icon :color="getItemColor(item.type)" size="18">{{ getItemIcon(item.type) }}</v-icon>
                  <span :style="{ color: getItemColor(item.type) }">+{{ item.bonus }}</span>
                </div>
              </div>

              <div class="item-price" :class="{ 'insufficient-gold': !canAfford(item.price) }">
                <v-icon size="20" color="#FFD700">mdi-currency-usd</v-icon>
                <span>{{ item.price }}</span>
              </div>
            </div>

            <v-btn
              v-if="loggedUser?.role !== 'userMaster'"
              :disabled="!canAfford(item.price)"
              class="buy-btn"
              :color="canAfford(item.price) ? 'primary' : 'grey'"
              variant="elevated"
              block
            >
              <v-icon left size="18">mdi-cart</v-icon>
              {{ canAfford(item.price) ? $t('comprar') : $t('oro insuficiente') }}
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </section>

    <!-- Sección: Objetos de Fuerza -->
    <section class="shop-section mb-10">
      <div class="section-header">
        <div class="header-icon-wrapper strength">
          <v-icon size="32" color="black">mdi-arm-flex</v-icon>
        </div>
        <div>
          <h2 class="section-title strength-title">{{ $t('Objetos de fuerza') }}</h2>
          <p class="section-subtitle">{{ $t('Aumenta tu poder físico') }}</p>
        </div>
      </div>

      <v-row class="items-grid" justify="center">
        <v-col
          v-for="item in strengthItems"
          :key="item.id"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <div class="item-card strength-item" @click="openPopup(item)">
            <div class="item-glow" style="background: radial-gradient(circle, #FF475740 0%, transparent 70%)"></div>

            <div class="item-image-wrapper" style="border-color: #FF4757; box-shadow: 0 0 20px #FF475740;">
              <div class="item-image-inner">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.name"
                  class="item-image"
                  @error="handleImageError"
                />
                <div class="item-icon-fallback">
                  <v-icon color="#FF4757" size="40">mdi-dumbbell</v-icon>
                </div>
              </div>
            </div>

            <div class="item-content">
              <h3 class="item-name">{{ item.name }}</h3>

              <div class="item-stats">
                <div class="stat-badge" style="background: #FF475720; border-color: #FF475760;">
                  <v-icon color="#FF4757" size="18">mdi-dumbbell</v-icon>
                  <span style="color: #FF4757;">+{{ item.bonus }}</span>
                </div>
              </div>

              <div class="item-price" :class="{ 'insufficient-gold': !canAfford(item.price) }">
                <v-icon size="20" color="#FFD700">mdi-currency-usd</v-icon>
                <span>{{ item.price }}</span>
              </div>
            </div>

            <v-btn
              v-if="loggedUser?.role !== 'userMaster'"
              :disabled="!canAfford(item.price)"
              class="buy-btn"
              :color="canAfford(item.price) ? 'primary' : 'grey'"
              variant="elevated"
              block
            >
              <v-icon left size="18">mdi-cart</v-icon>
              {{ canAfford(item.price) ? $t('comprar') : $t('oro insuficiente') }}
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </section>

    <!-- Sección: Objetos de Resistencia -->
    <section class="shop-section">
      <div class="section-header">
        <div class="header-icon-wrapper endurance">
          <v-icon size="32" color="black">mdi-lightning-bolt</v-icon>
        </div>
        <div>
          <h2 class="section-title endurance-title">{{ $t('Objetos de resistencia') }}</h2>
          <p class="section-subtitle">{{ $t('Mejora tu aguante') }}</p>
        </div>
      </div>

      <v-row class="items-grid" justify="center">
        <v-col
          v-for="item in enduranceItems"
          :key="item.id"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <div class="item-card endurance-item" @click="openPopup(item)">
            <div class="item-glow" style="background: radial-gradient(circle, #00D2FF40 0%, transparent 70%)"></div>

            <div class="item-image-wrapper" style="border-color: #00D2FF; box-shadow: 0 0 20px #00D2FF40;">
              <div class="item-image-inner">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.name"
                  class="item-image"
                  @error="handleImageError"
                />
                <div class="item-icon-fallback">
                  <v-icon color="#00D2FF" size="40">mdi-run-fast</v-icon>
                </div>
              </div>
            </div>

            <div class="item-content">
              <h3 class="item-name">{{ item.name }}</h3>

              <div class="item-stats">
                <div class="stat-badge" style="background: #00D2FF20; border-color: #00D2FF60;">
                  <v-icon color="#00D2FF" size="18">mdi-run-fast</v-icon>
                  <span style="color: #00D2FF;">+{{ item.bonus }}</span>
                </div>
              </div>

              <div class="item-price" :class="{ 'insufficient-gold': !canAfford(item.price) }">
                <v-icon size="20" color="#FFD700">mdi-currency-usd</v-icon>
                <span>{{ item.price }}</span>
              </div>
            </div>

            <v-btn
              v-if="loggedUser?.role !== 'userMaster'"
              :disabled="!canAfford(item.price)"
              class="buy-btn"
              :color="canAfford(item.price) ? 'primary' : 'grey'"
              variant="elevated"
              block
            >
              <v-icon left size="18">mdi-cart</v-icon>
              {{ canAfford(item.price) ? $t('comprar') : $t('oro insuficiente') }}
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </section>

    <!-- Diálogo de Compra -->
    <v-dialog v-model="showDialog" max-width="450px">
      <v-card class="purchase-dialog">
        <div class="dialog-header" :style="{ background: `linear-gradient(135deg, ${getItemColor(selectedItem?.type || '')}30 0%, transparent 100%)` }">
          <!-- Imagen en el diálogo también con nuevo estilo -->
          <div class="dialog-image-wrapper" :style="{ borderColor: getItemColor(selectedItem?.type || ''), boxShadow: `0 0 30px ${getItemColor(selectedItem?.type || '')}50` }">
            <div class="dialog-image-inner">
              <img
                v-if="selectedItem?.imageUrl"
                :src="selectedItem.imageUrl"
                :alt="selectedItem.name"
                class="dialog-item-image"
                @error="handleImageError"
              />
              <div class="dialog-icon-fallback">
                <v-icon color="white" size="32">{{ getItemIcon(selectedItem?.type || '') }}</v-icon>
              </div>
            </div>
          </div>
          <h3 class="dialog-title">{{ selectedItem?.name }}</h3>
        </div>

        <v-card-text class="dialog-content">
          <div class="purchase-info">
            <div class="info-row">
              <span class="info-label">{{ $t('Precio') }}:</span>
              <div class="info-value gold-value">
                <v-icon size="20" color="#FFD700">mdi-currency-usd</v-icon>
                <strong>{{ selectedItem?.price }}</strong>
              </div>
            </div>

            <div class="info-row">
              <span class="info-label">{{ $t('Bonificación') }}:</span>
              <div class="info-value">
                <v-icon :color="getItemColor(selectedItem?.type || '')" size="20">{{ getItemIcon(selectedItem?.type || '') }}</v-icon>
                <strong :style="{ color: getItemColor(selectedItem?.type || '') }">+{{ selectedItem?.bonus }}</strong>
                <span v-if="selectedItem?.type === 'Strength'">{{ $t('fuerza') }}</span>
                <span v-else-if="selectedItem?.type === 'Endurance'">{{ $t('resistencia') }}</span>
              </div>
            </div>

            <div class="info-row balance-row" :class="{ 'insufficient': !canAfford(selectedItem?.price || 0) }">
              <span class="info-label">{{ $t('Tu oro') }}:</span>
              <div class="info-value">
                <strong>{{ loggedUser?.gold || 0 }}</strong>
              </div>
            </div>
          </div>

          <v-alert
            v-if="!canAfford(selectedItem?.price || 0)"
            type="warning"
            variant="tonal"
            class="mt-4"
          >
            {{ $t('No tienes suficiente oro para esta compra') }}
          </v-alert>
        </v-card-text>

        <v-card-actions class="dialog-actions">
          <v-btn color="grey-darken-1" variant="text" @click="closePopup">
            {{ $t('cancelar') }}
          </v-btn>
          <v-spacer />
          <v-btn
            :disabled="!canAfford(selectedItem?.price || 0)"
            color="primary"
            variant="elevated"
            @click="handleBuy"
          >
            <v-icon left size="18">mdi-cart-check</v-icon>
            {{ $t('confirmar compra') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Snackbar -->
    <v-snackbar
      v-model="snackbar"
      :color="snackbarColor"
      :timeout="4000"
      location="top"
      multi-line
    >
      <div class="d-flex align-center">
        <v-icon
          :icon="snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'"
          class="mr-3"
        ></v-icon>
        <span>{{ snackbarMessage }}</span>
      </div>
      <template v-slot:actions>
        <v-btn variant="text" @click="snackbar = false" icon="mdi-close"></v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<style scoped>
.items-container {
  max-width: 100%;
}

.shop-section {
  background: rgba(15, 23, 42, 0.4);
  backdrop-filter: blur(8px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.section-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid rgba(255, 255, 255, 0.1);
  flex-wrap: wrap;
}

.header-icon-wrapper {
  width: 60px;
  height: 60px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  flex-shrink: 0;
}

.header-icon-wrapper.daily   { background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); }
.header-icon-wrapper.strength { background: linear-gradient(135deg, #FF4757 0%, #FF6348 100%); }
.header-icon-wrapper.endurance{ background: linear-gradient(135deg, #00D2FF 0%, #3A7BD5 100%); }

.section-title {
  font-size: 1.8rem;
  font-weight: 800;
  color: #fff;
  margin: 0;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.section-subtitle {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.7);
  margin: 0.25rem 0 0 0;
}

.daily-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 215, 0, 0.2);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  border: 1px solid rgba(255, 215, 0, 0.4);
  color: #FFD700;
  font-weight: 700;
  margin-left: auto;
}

/* ─── CARD ─────────────────────────────────────────────── */
.item-card {
  position: relative;
  background: linear-gradient(145deg, rgba(30, 41, 59, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
  border-radius: 16px;
  padding: 1.5rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.item-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(145deg, rgba(255, 255, 255, 0.05) 0%, transparent 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.item-card:hover {
  transform: translateY(-8px);
  border-color: rgba(255, 255, 255, 0.3);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.4), 0 0 40px rgba(255, 255, 255, 0.1);
}

.item-card:hover::before { opacity: 1; }

.item-glow {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 200px; height: 200px;
  border-radius: 50%;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.4s ease;
}

.item-card:hover .item-glow { opacity: 1; }

.daily-ribbon {
  position: absolute;
  top: 12px;
  right: -30px;
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  color: #000;
  padding: 0.25rem 2.5rem;
  font-size: 0.7rem;
  font-weight: 800;
  transform: rotate(45deg);
  box-shadow: 0 2px 10px rgba(255, 215, 0, 0.5);
  letter-spacing: 1px;
}

/* ─── IMAGEN (nuevo — rectángulo redondeado) ────────────── */
/*
  Contenedor con aspect-ratio fijo (16:9 aprox.) para imágenes
  horizontales. Usa object-fit: contain para que nunca se recorte.
*/
.item-image-wrapper {
  width: 100%;
  border: 2px solid;
  border-radius: 14px;
  margin-bottom: 1.25rem;
  overflow: hidden;
  transition: transform 0.35s ease, box-shadow 0.35s ease;
  background: rgba(0, 0, 0, 0.35);
}

.item-card:hover .item-image-wrapper {
  transform: scale(1.04);
}

/* Ratio 16:10 — ajusta a tu gusto */
.item-image-inner {
  position: relative;
  width: 100%;
  padding-top: 62.5%;   /* 10/16 × 100 */
}

.item-image {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: contain;  /* nunca recorta imágenes horizontales */
  padding: 10px;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.4));
  transition: transform 0.35s ease;
}

.item-card:hover .item-image {
  transform: scale(1.06);
}

.item-icon-fallback {
  position: absolute;
  inset: 0;
  display: none;
  align-items: center;
  justify-content: center;
}

/* ─── CONTENIDO ────────────────────────────────────────── */
.item-content {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 0.9rem;
}

.item-name {
  font-size: 1.1rem;
  font-weight: 700;
  color: #fff;
  text-align: center;
  margin: 0;
  min-height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.item-stats {
  display: flex;
  justify-content: center;
}

.stat-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  border: 1px solid;
  font-weight: 700;
  font-size: 0.95rem;
}

.item-price {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 1.3rem;
  font-weight: 800;
  color: #FFD700;
  padding: 0.75rem;
  background: rgba(255, 215, 0, 0.1);
  border-radius: 12px;
  border: 1px solid rgba(255, 215, 0, 0.3);
}

.item-price.insufficient-gold {
  color: #FF4757;
  border-color: rgba(255, 71, 87, 0.3);
  background: rgba(255, 71, 87, 0.1);
}

.item-price.insufficient-gold .v-icon {
  color: #FF4757 !important;
}

/* ─── BOTÓN ────────────────────────────────────────────── */
.buy-btn {
  margin-top: auto;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  border-radius: 12px !important;
  min-height: 52px !important;
  animation: fadeInUp 0.4s ease-out;
}

.buy-btn:focus-visible {
  outline: 3px solid rgba(255, 215, 0, 0.5);
  outline-offset: 2px;
}

@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(10px); }
  to   { opacity: 1; transform: translateY(0); }
}

/* ─── HOVER POR TIPO ───────────────────────────────────── */
.strength-item:hover {
  border-color: rgba(255, 71, 87, 0.6);
  box-shadow: 0 12px 40px rgba(255, 71, 87, 0.3), 0 0 40px rgba(255, 71, 87, 0.2);
}

.endurance-item:hover {
  border-color: rgba(0, 210, 255, 0.6);
  box-shadow: 0 12px 40px rgba(0, 210, 255, 0.3), 0 0 40px rgba(0, 210, 255, 0.2);
}

.daily-item:hover {
  border-color: rgba(255, 215, 0, 0.6);
  box-shadow: 0 12px 40px rgba(255, 215, 0, 0.3), 0 0 40px rgba(255, 215, 0, 0.2);
}

/* Shimmer en items diarios */
.daily-item::after {
  content: '';
  position: absolute;
  top: 0; left: -100%;
  width: 100%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 215, 0, 0.15), transparent);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0%   { left: -100%; }
  100% { left: 100%; }
}

/* ─── DIÁLOGO ──────────────────────────────────────────── */
.purchase-dialog {
  background: linear-gradient(145deg, rgba(30, 41, 59, 1) 0%, rgba(15, 23, 42, 1) 100%);
  border: 2px solid rgba(255, 255, 255, 0.2);
  overflow: hidden;
}

.dialog-header {
  padding: 2rem;
  text-align: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

/* Imagen del diálogo con el mismo sistema */
.dialog-image-wrapper {
  width: 70%;
  max-width: 240px;
  margin: 0 auto 1rem;
  border: 2px solid;
  border-radius: 14px;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.3);
}

.dialog-image-inner {
  position: relative;
  width: 100%;
  padding-top: 62.5%;
}

.dialog-item-image {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: contain;
  padding: 12px;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.4));
}

.dialog-icon-fallback {
  position: absolute;
  inset: 0;
  display: none;
  align-items: center;
  justify-content: center;
}

.dialog-title {
  font-size: 1.5rem;
  font-weight: 800;
  color: #fff;
  margin: 0;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.dialog-content { padding: 2rem !important; }

.purchase-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.info-row.balance-row {
  border: 2px solid rgba(76, 175, 80, 0.5);
  background: rgba(76, 175, 80, 0.1);
}

.info-row.balance-row.insufficient {
  border-color: rgba(255, 71, 87, 0.5);
  background: rgba(255, 71, 87, 0.1);
}

.info-label {
  font-size: 0.95rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
}

.info-value {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.1rem;
  color: #fff;
  font-weight: 700;
}

.gold-value { color: #FFD700; }

.dialog-actions {
  padding: 1.5rem 2rem !important;
  background: rgba(0, 0, 0, 0.2);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

/* ─── RESPONSIVE ───────────────────────────────────────── */
@media (max-width: 960px) {
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  .daily-badge { margin-left: 0; }
  .section-title { font-size: 1.5rem; }
}

@media (max-width: 600px) {
  .shop-section { padding: 1.5rem; }
  .section-title { font-size: 1.3rem; }
  .header-icon-wrapper { width: 50px; height: 50px; }
  .item-card { padding: 1.25rem; }
  .item-name { font-size: 1rem; }
  .buy-btn { min-height: 48px !important; }
  .dialog-header { padding: 1.5rem; }
  .dialog-content { padding: 1.5rem !important; }
  .dialog-image-wrapper { width: 80%; }
  .dialog-title { font-size: 1.25rem; }
  .info-row { flex-direction: column; align-items: flex-start; gap: 0.5rem; }
  .info-value { align-self: flex-end; }
}

@media (hover: none) and (pointer: coarse) {
  .buy-btn { min-height: 52px !important; }
}

/* Scrollbar */
.items-container::-webkit-scrollbar { width: 8px; }
.items-container::-webkit-scrollbar-track { background: rgba(0,0,0,.2); border-radius: 10px; }
.items-container::-webkit-scrollbar-thumb { background: linear-gradient(180deg,#0D6EFD,#0a58ca); border-radius: 10px; }
.items-container::-webkit-scrollbar-thumb:hover { background: linear-gradient(180deg,#0a58ca,#084298); }
</style>