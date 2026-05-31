<script setup lang="ts">
import { useUserStore } from '@/stores/userStore'
import { computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const store = useUserStore()
const loggedUser = computed(() => store.loggedUser)
// Cargar items al montar el componente
onMounted(() => {
  if (loggedUser.value?.email && loggedUser.value?.passwordhash) {
    store.getItems()
  }
})

watch(loggedUser, (newUser) => {
  if (newUser?.email && newUser?.passwordhash) {
    store.getItems()
  }
})

// Computed: Filtrar items por tipo
const strengthItems = computed(() => {
  return store.purchasedItems?.filter(item => 
    item.itemType.toLowerCase() === 'strength' || 
    item.itemType.toLowerCase() === 'fuerza'
  ) || []
})

const enduranceItems = computed(() => {
  return store.purchasedItems?.filter(item => 
    item.itemType.toLowerCase() === 'endurance' || 
    item.itemType.toLowerCase() === 'resistencia'
  ) || []
})

const totalItems = computed(() => store.purchasedItems?.length || 0)

// Verificar si un item está equipado
const isItemEquipped = (itemId: number, itemType: string) => {
  if (!loggedUser.value) return false
  
  const typeLower = itemType.toLowerCase()
  if (typeLower === 'strength' || typeLower === 'fuerza') {
    return loggedUser.value.equippedStrengthItemId === itemId
  }
  if (typeLower === 'endurance' || typeLower === 'resistencia') {
    return loggedUser.value.equippedEnduranceItemId === itemId
  }
  return false
}

// Manejar equipar/desequipar
const handleItemClick = async (item: any) => {
  const typeLower = item.itemType.toLowerCase()
  const isEquipped = isItemEquipped(item.itemId, item.itemType)
  
  if (isEquipped) {
    // Desequipar
    await store.unequipItem(typeLower === 'strength' || typeLower === 'fuerza' ? 'strength' : 'endurance')
  } else {
    // Equipar
    await store.equipItem(item.itemId)
  }
}
// Función para determinar la rareza del item basada en su bonus (las stats que suma el item al usuario)
const getItemRarity = (bonus: number) => {
  if (bonus >= 50) return 'legendary'
  if (bonus >= 30) return 'epic'
  if (bonus >= 15) return 'rare'
  return 'common'
}
// Función para obtener un icono representativo del tipo de item (fuerza o resistencia), se puede expandir para más tipos si es necesario (para futuras actualizaciones en tal caso)
const getItemIcon = (type: string) => {
  const icons: Record<string, string> = {
    'fuerza': 'mdi-arm-flex',
    'strength': 'mdi-arm-flex',
    'resistencia': 'mdi-run',
    'endurance': 'mdi-run',
    'velocidad': 'mdi-lightning-bolt',
    'defensa': 'mdi-shield',
    'agilidad': 'mdi-bird',
    'poder': 'mdi-star',
    'default': 'mdi-gift'
  }
  return icons[type.toLowerCase()] || icons.default
}
// Obtener tipo de item dependiendo de su tipo
const getItemTypeClass = (type: string) => {
  const typeLower = type.toLowerCase()
  if (typeLower === 'strength' || typeLower === 'fuerza') return 'type-strength'
  if (typeLower === 'endurance' || typeLower === 'resistencia') return 'type-endurance'
  return ''
}

// Función para manejar errores de carga de imagen
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
  <v-container
    v-if="loggedUser?.role !== 'userMaster'"
    class="inventory-wrapper"
    fluid
  >
    <div class="inventory-header">
      <div class="header-decoration"></div>
      <h2 class="inventory-title">
        <span class="title-icon"><v-icon>mdi-bag-personal</v-icon></span>
        {{ $t('inventario') }}
        <span class="item-count">{{ totalItems }}</span>
      </h2>
      <div class="header-decoration"></div>
    </div>

    <div v-if="totalItems === 0" class="empty-inventory">
      <v-icon class="empty-icon" color="grey">mdi-package-variant</v-icon>
      <p class="empty-text">{{ $t('inventario_vacio') }}</p>
      <p class="empty-subtext">{{ $t('compra_items') }}</p>
    </div>

    <div v-else class="inventory-sections">
      <!-- Sección de Fuerza -->
      <div class="section-container">
        <div class="section-header strength-header">
          <v-icon class="section-icon" color="amber">mdi-arm-flex</v-icon>
          <h3 class="section-title">{{ $t('items_fuerza') }}</h3>
          <span class="section-count">{{ strengthItems.length }}</span>
        </div>

        <div v-if="strengthItems.length === 0" class="empty-section">
          <p>{{ $t('no_items_fuerza') }}</p>
        </div>

        <v-row v-else justify="start" class="items-grid">
          <v-col
            v-for="item in strengthItems"
            :key="item.purchaseId"
            cols="12"
            sm="6"
            md="4"
            lg="3"
            xl="2"
          >
            <div
              class="item-card"
              :class="[
                `rarity-${getItemRarity(item.itemBonus)}`,
                getItemTypeClass(item.itemType),
                { 'equipped': isItemEquipped(item.itemId, item.itemType) }
              ]"
              @click="handleItemClick(item)"
            >
              <!-- Badge de equipado -->
              <div v-if="isItemEquipped(item.itemId, item.itemType)" class="equipped-badge">
                <span><v-icon>mdi-sword-cross</v-icon> {{ $t('equipped_label') }}</span>
              </div>

              <!-- Bonus -->
              <div class="bonus-badge">
                +{{ item.itemBonus }}
              </div>

              <!-- Imagen/Icono -->
              <div class="item-icon-wrapper">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.itemName"
                  class="item-image"
                  @error="handleImageError"
                />
                <div class="item-icon item-icon-fallback" style="display: none;">
                  <v-icon>{{ getItemIcon(item.itemType) }}</v-icon>
                </div>
              </div>

              <!-- Nombre y tipo -->
              <div class="item-content">
                <div class="item-name">{{ item.itemName }}</div>
                <div class="item-type">{{ item.itemType }}</div>
              </div>

              <!-- Botón de acción -->
              <div class="equip-action">
                <span v-if="isItemEquipped(item.itemId, item.itemType)" class="action-text unequip">
                  {{ $t('unequip_label') }}
                </span>
                <span v-else class="action-text equip">
                  {{ $t('equip_label') }}
                </span>
              </div>
            </div>
          </v-col>
        </v-row>
      </div>

      <!-- Sección de Resistencia -->
      <div class="section-container">
        <div class="section-header endurance-header">
          <v-icon class="section-icon" color="blue">mdi-run</v-icon>
          <h3 class="section-title">{{ $t('items_resistencia') }}</h3>
          <span class="section-count">{{ enduranceItems.length }}</span>
        </div>

        <div v-if="enduranceItems.length === 0" class="empty-section">
          <p>{{ $t('no_items_resistencia') }}</p>
        </div>

        <v-row v-else justify="start" class="items-grid">
          <v-col
            v-for="item in enduranceItems"
            :key="item.purchaseId"
            cols="12"
            sm="6"
            md="4"
            lg="3"
            xl="2"
          >
            <div
              class="item-card"
              :class="[
                `rarity-${getItemRarity(item.itemBonus)}`,
                getItemTypeClass(item.itemType),
                { 'equipped': isItemEquipped(item.itemId, item.itemType) }
              ]"
              @click="handleItemClick(item)"
            >
              <!-- Badge de equipado -->
              <div v-if="isItemEquipped(item.itemId, item.itemType)" class="equipped-badge">
                <span><v-icon>mdi-sword-cross</v-icon> {{ $t('equipped_label') }}</span>
              </div>

              <!-- Bonus -->
              <div class="bonus-badge">
                +{{ item.itemBonus }}
              </div>

              <!-- Imagen/Icono -->
              <div class="item-icon-wrapper">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.itemName"
                  class="item-image"
                  @error="handleImageError"
                />
                <div class="item-icon item-icon-fallback" style="display: none;">
                  <v-icon>{{ getItemIcon(item.itemType) }}</v-icon>
                </div>
              </div>

              <!-- Nombre y tipo -->
              <div class="item-content">
                <div class="item-name">{{ item.itemName }}</div>
                <div class="item-type">{{ item.itemType }}</div>
              </div>

              <!-- Botón de acción -->
              <div class="equip-action">
                <span v-if="isItemEquipped(item.itemId, item.itemType)" class="action-text unequip">
                  {{ $t('unequip_label') }}
                </span>
                <span v-else class="action-text equip">
                  {{ $t('equip_label') }}
                </span>
              </div>
            </div>
          </v-col>
        </v-row>
      </div>
    </div>
  </v-container>
</template>

<style scoped>
.inventory-wrapper {
  margin: 2rem auto;
  padding: 2rem;
  max-width: 1400px;
  position: relative;
}

/* Header */
.inventory-header {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  margin-bottom: 3rem;
  position: relative;
}

.header-decoration {
  height: 3px;
  width: 80px;
  background: linear-gradient(90deg, transparent, #fbbf24, transparent);
  border-radius: 2px;
}

.inventory-title {
  font-size: clamp(1.75rem, 4vw, 2.5rem);
  font-weight: 800;
  color: #ffffff;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 2px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
}

.title-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.3));
}

.item-count {
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  color: #0f172a;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 1rem;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
}

/* Empty State */
.empty-inventory {
  text-align: center;
  padding: 4rem 2rem;
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 24px;
  border: 2px dashed rgba(255, 255, 255, 0.2);
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  opacity: 0.7;
  animation: float 3s ease-in-out infinite;
}

.empty-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: #ffffff;
  margin-bottom: 0.5rem;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.empty-subtext {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.7);
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
}

/* Inventory Sections */
.inventory-sections {
  display: flex;
  flex-direction: column;
  gap: 3rem;
}

.section-container {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 24px;
  padding: 2rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

/* Section Headers */
.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 3px solid;
}

.strength-header {
  border-bottom-color: #f87171;
}

.endurance-header {
  border-bottom-color: #22d3ee;
}

.section-icon {
  font-size: 2.5rem;
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.3));
}

.section-title {
  font-size: 1.75rem;
  font-weight: 800;
  margin: 0;
  flex: 1;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #ffffff;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.section-count {
  background: linear-gradient(135deg, #475569, #334155);
  color: #ffffff;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 1.25rem;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  min-width: 50px;
  text-align: center;
}

.strength-header .section-count {
  background: linear-gradient(135deg, #f87171, #dc2626);
  box-shadow: 0 4px 12px rgba(248, 113, 113, 0.4);
}

.endurance-header .section-count {
  background: linear-gradient(135deg, #22d3ee, #0891b2);
  box-shadow: 0 4px 12px rgba(34, 211, 238, 0.4);
}

/* Empty Section */
.empty-section {
  text-align: center;
  padding: 3rem 2rem;
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 16px;
  border: 2px dashed rgba(255, 255, 255, 0.2);
}

.empty-section p {
  font-size: 1.125rem;
  color: rgba(255, 255, 255, 0.7);
  margin: 0;
  font-weight: 600;
}

/* Items Grid */
.items-grid {
  gap: 2rem;
}

/* Item Card - Minimalista con contraste */
.item-card {
  position: relative;
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 20px;
  padding: 1.5rem;
  padding-bottom: 4rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  border: 2px solid rgba(255, 255, 255, 0.15);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.3);
  min-height: 380px;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
}

.item-card:hover {
  transform: translateY(-10px) scale(1.03);
  box-shadow: 0 16px 40px rgba(0, 0, 0, 0.5);
}

/* Estado equipado */
.item-card.equipped {
  border: 2px solid #fbbf24;
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.4);
}

.item-card.equipped:hover {
  box-shadow: 0 0 30px rgba(251, 191, 36, 0.6);
  transform: translateY(-12px) scale(1.05);
}

/* Badge de equipado */
.equipped-badge {
  position: absolute;
  top: 8px;
  left: 8px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a;
  padding: 0.5rem 0.875rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
  z-index: 15;
}

/* Bonus Badge */
.bonus-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  display: flex;
  align-items: baseline;
  gap: 0.15rem;
  background: linear-gradient(135deg, #10b981, #059669);
  color: #ffffff;
  padding: 0.5rem 0.9rem;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.4);
  font-weight: 900;
  z-index: 10;
  transition: all 0.3s ease;
}

.item-card:hover .bonus-badge {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.6);
}

.bonus-number {
  font-size: 1.4rem;
  line-height: 1;
  font-family: 'Courier New', monospace;
}

/* Acción de equipar/desequipar */
.equip-action {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 0.75rem;
  display: flex;
  justify-content: center;
  align-items: center;
}

.action-text {
  width: 100%;
  display: block;
  text-align: center;
  padding: 0.65rem 1rem;
  border-radius: 10px;
  font-weight: 800;
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
}

.action-text.equip {
  background: linear-gradient(135deg, #10b981, #059669);
  color: #ffffff;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
}

.action-text.equip:hover {
  background: linear-gradient(135deg, #059669, #047857);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.5);
  transform: translateY(-2px);
}

.action-text.unequip {
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: #ffffff;
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}

.action-text.unequip:hover {
  background: linear-gradient(135deg, #dc2626, #b91c1c);
  box-shadow: 0 6px 20px rgba(239, 68, 68, 0.5);
  transform: translateY(-2px);
}

/* Type-specific Styles */
.type-strength {
  border-color: #f87171 !important;
}

.type-strength:hover {
  border-color: #fca5a5 !important;
  box-shadow: 0 16px 40px rgba(248, 113, 113, 0.3) !important;
}

.type-strength.equipped {
  border-color: #fbbf24 !important;
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.4) !important;
}

.type-endurance {
  border-color: #22d3ee !important;
}

.type-endurance:hover {
  border-color: #67e8f9 !important;
  box-shadow: 0 16px 40px rgba(34, 211, 238, 0.3) !important;
}

.type-endurance.equipped {
  border-color: #fbbf24 !important;
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.4) !important;
}

/* Rarity Styles */
.rarity-common {
  border-color: #94a3b8;
}

.rarity-common:hover {
  border-color: #cbd5e1;
  box-shadow: 0 16px 40px rgba(148, 163, 184, 0.3);
}

.rarity-rare {
  border-color: #38bdf8;
}

.rarity-rare:hover {
  border-color: #7dd3fc;
  box-shadow: 0 16px 40px rgba(56, 189, 248, 0.3);
}

.rarity-epic {
  border-color: #f472b6;
}

.rarity-epic:hover {
  border-color: #fb7185;
  box-shadow: 0 16px 40px rgba(244, 114, 182, 0.3);
}

.rarity-legendary {
  border-color: #fbbf24;
  box-shadow: 0 0 15px rgba(251, 191, 36, 0.3);
}

.rarity-legendary:hover {
  border-color: #fcd34d;
  box-shadow: 0 0 25px rgba(251, 191, 36, 0.5);
}

/* Contenedor de imagen */
.item-icon-wrapper {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  max-width: 180px;
  aspect-ratio: 1;
  margin: 1rem auto;
  padding: 8px;
  background: linear-gradient(135deg, #1e293b, #0f172a);
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.item-card:hover .item-icon-wrapper {
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.4);
  transform: scale(1.05);
  transition: all 0.3s ease;
}

.item-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3));
  transition: all 0.3s ease;
  border-radius: 12px;
  image-rendering: crisp-edges;
  image-rendering: -webkit-optimize-contrast;
}

.item-card:hover .item-image {
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.4));
  transform: scale(1.08) rotate(2deg);
}

/* Items equipados */
.item-card.equipped .item-icon-wrapper {
  background: linear-gradient(135deg, #2d2a1f, #1f1b14);
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.3);
  border-color: rgba(251, 191, 36, 0.5);
}

.item-card.equipped:hover .item-icon-wrapper {
  box-shadow: 0 0 30px rgba(251, 191, 36, 0.5);
}

.item-card.equipped .item-image {
  filter: drop-shadow(0 6px 12px rgba(251, 191, 36, 0.3));
}

.item-card.equipped:hover .item-image {
  filter: drop-shadow(0 10px 20px rgba(251, 191, 36, 0.5));
}

/* Fallback icon emoji */
.item-icon {
  font-size: 4rem;
  filter: drop-shadow(0 4px 10px rgba(0, 0, 0, 0.3));
  transition: transform 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.item-icon-fallback {
  display: none;
}

.item-card:hover .item-icon {
  transform: scale(1.15) rotate(8deg);
}

/* Item Content */
.item-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  align-items: center;
  width: 100%;
  margin-top: 0.5rem;
}

.item-name {
  font-weight: 800;
  font-size: 1.1rem;
  color: #ffffff;
  text-align: center;
  line-height: 1.3;
  word-break: break-word;
  min-height: 2.4rem;
  display: flex;
  align-items: center;
  justify-content: center;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.item-type {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.7);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}

/* Responsive */
@media (max-width: 1200px) {
  .item-icon-wrapper {
    max-width: 160px;
  }

  .item-card {
    min-height: 360px;
  }
}

@media (max-width: 960px) {
  .item-icon-wrapper {
    max-width: 140px;
  }

  .item-card {
    min-height: 350px;
  }
}

@media (max-width: 600px) {
  .inventory-wrapper {
    padding: 1rem;
  }

  .inventory-header {
    margin-bottom: 2rem;
  }

  .header-decoration {
    width: 40px;
  }

  .inventory-sections {
    gap: 2rem;
  }

  .section-container {
    padding: 1.5rem;
  }

  .section-header {
    flex-wrap: wrap;
    gap: 0.75rem;
  }

  .section-icon {
    font-size: 2rem;
  }

  .section-title {
    font-size: 1.25rem;
    flex-basis: 100%;
  }

  .section-count {
    font-size: 1rem;
    padding: 0.35rem 0.75rem;
  }

  .items-grid {
    gap: 1.5rem;
  }

  .item-card {
    padding: 1.5rem;
    padding-bottom: 4rem;
    min-height: 380px;
  }

  .equipped-badge {
    top: 8px;
    left: 8px;
    padding: 0.5rem 0.8rem;
    font-size: 0.75rem;
  }

  .bonus-badge {
    top: 10px;
    right: 10px;
    padding: 0.5rem 0.85rem;
  }

  .bonus-number {
    font-size: 1.5rem;
  }

  .item-icon-wrapper {
    max-width: 140px;
    margin: 1rem auto;
    padding: 8px;
  }

  .item-icon {
    font-size: 3.5rem;
  }

  .item-name {
    font-size: 1.1rem;
    min-height: 2.4rem;
  }

  .item-type {
    font-size: 0.75rem;
  }

  .action-text {
    padding: 0.7rem 1rem;
    font-size: 0.8rem;
  }

  .empty-section {
    padding: 2.5rem 1.5rem;
  }

  .empty-section p {
    font-size: 1.1rem;
  }
}

@media (max-width: 360px) {
  .item-card {
    min-height: 360px;
    padding: 1.25rem;
    padding-bottom: 3.5rem;
  }

  .item-icon-wrapper {
    max-width: 120px;
  }

  .item-icon {
    font-size: 3rem;
  }

  .item-name {
    font-size: 1rem;
  }
}
</style>