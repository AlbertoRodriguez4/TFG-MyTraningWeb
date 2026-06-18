<script setup lang="ts">
import ItemRender from '../../components/Renders/ItemRender.vue';
import { useUserStore } from '@/stores/userStore';
import { ref, watchEffect } from 'vue';

const store = useUserStore()
const loggedUser = ref(store.loggedUser)

watchEffect(() => {
  loggedUser.value = store.loggedUser
})
</script>

  <template>
  <v-container fluid class="shop-container fill-height d-flex flex-column">
    <div class="shop-header-wrapper">
      <div class="shop-header">
        <div class="header-content">
          <div class="shop-icon">
            <v-icon size="48" color="white">mdi-store</v-icon>
          </div>
          <div class="shop-title-section">
            <h1 class="shop-title">
              {{ $t('tienda') }}
            </h1>
            <p class="shop-subtitle">{{ $t('shop_subtitle') }}</p>
          </div>
        </div>
        
        <div class="gold-badge">
          <v-icon class="gold-icon" size="28" color="white">mdi-currency-usd</v-icon>
          <div class="gold-info">
            <span class="gold-label">{{ $t('tu oro') }}</span>
            <span class="gold-amount">{{ loggedUser?.gold || 0 }}</span>
          </div>
        </div>
      </div>
    </div>

    <div class="items-section">
      <div class="items-wrapper">
        <ItemRender />
      </div>
    </div>
  </v-container>
</template>

<style scoped>


.shop-container {
  position: relative;
  z-index: 2;
  padding: clamp(1rem, 3vw, 2rem) clamp(1rem, 3vw, 3rem) !important;
  max-width: 100% !important;
  display: flex;
  flex-direction: column;
  gap: clamp(1.5rem, 3vw, 2rem);
}

/* Header Wrapper */
.shop-header-wrapper {
  width: 100%;
  display: flex;
  justify-content: center;
  margin-bottom: clamp(1rem, 2vw, 1.5rem);
}

.shop-header {
  width: 100%;
  max-width: 1400px;
  background: linear-gradient(135deg, rgba(13, 110, 253, 0.15) 0%, rgba(255, 215, 0, 0.1) 100%);
  backdrop-filter: blur(10px);
  border: 2px solid rgba(255, 215, 0, 0.3);
  border-radius: clamp(16px, 2vw, 20px);
  padding: clamp(1.25rem, 3vw, 2rem);
  box-shadow: 
    0 8px 32px rgba(0, 0, 0, 0.4),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: clamp(1rem, 2vw, 1.5rem);
}

.header-content {
  display: flex;
  align-items: center;
  gap: clamp(1rem, 2vw, 1.5rem);
  flex: 1;
  min-width: 250px;
}

.shop-icon {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
  border-radius: clamp(12px, 2vw, 16px);
  padding: clamp(0.75rem, 1.5vw, 1rem);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 20px rgba(255, 215, 0, 0.4);
  flex-shrink: 0;
}

.shop-icon .v-icon {
  font-size: clamp(32px, 5vw, 48px) !important;
}

.shop-title-section {
  display: flex;
  flex-direction: column;
  gap: clamp(0.15rem, 0.5vw, 0.25rem);
  flex: 1;
  min-width: 0;
}

.shop-title {
  font-size: clamp(1.5rem, 4vw, 2.5rem);
  font-weight: 800;
  color: #FFD700;
  margin: 0;
  text-shadow: 
    2px 2px 4px rgba(0, 0, 0, 0.5),
    0 0 20px rgba(255, 215, 0, 0.3);
  letter-spacing: clamp(0.5px, 0.1vw, 1px);
  line-height: 1.2;
}

.shop-subtitle {
  font-size: clamp(0.8rem, 1.5vw, 0.95rem);
  color: var(--text-secondary);
  margin: 0;
  font-weight: 400;
  line-height: 1.4;
}

.gold-badge {
  display: flex;
  align-items: center;
  gap: clamp(0.75rem, 1.5vw, 1rem);
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 165, 0, 0.15) 100%);
  padding: clamp(0.75rem, 1.5vw, 1rem) clamp(1rem, 2vw, 1.5rem);
  border-radius: 50px;
  border: 2px solid rgba(255, 215, 0, 0.4);
  box-shadow: 0 4px 15px rgba(255, 215, 0, 0.2);
  flex-shrink: 0;
}

.gold-icon {
  flex-shrink: 0;
}

.gold-info {
  display: flex;
  flex-direction: column;
  gap: 0.1rem;
  min-width: 0;
}

.gold-label {
  font-size: clamp(0.65rem, 1vw, 0.75rem);
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: clamp(0.5px, 0.1vw, 1px);
  font-weight: 600;
  line-height: 1.2;
}

.gold-amount {
  font-size: clamp(1.25rem, 2.5vw, 1.75rem);
  font-weight: 800;
  color: #FFD700;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  line-height: 1.2;
}

/* Items Section */
.items-section {
  flex: 1;
  width: 100%;
  display: flex;
  justify-content: center;
  overflow-y: auto;
}

.items-wrapper {
  width: 100%;
  max-width: 1400px;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(8px);
  border-radius: clamp(12px, 2vw, 16px);
  padding: clamp(1rem, 2vw, 1.5rem);
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  height: fit-content;
  min-height: 400px;
}

/* Responsive Design */

/* Tablets grandes y desktop pequeño */
@media (max-width: 1200px) {
  .shop-header,
  .items-wrapper {
    max-width: 100%;
  }
}

/* Tablets */
@media (max-width: 960px) {
  .shop-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-content {
    justify-content: center;
    min-width: auto;
  }

  .gold-badge {
    justify-content: center;
  }
}

/* Tablets pequeñas */
@media (max-width: 768px) {
  .shop-container {
    padding: 1rem !important;
  }

  .header-content {
    flex-direction: column;
    text-align: center;
  }

  .shop-title-section {
    align-items: center;
    text-align: center;
  }
}

/* Móviles */
@media (max-width: 600px) {
  .shop-container {
    padding: 0.75rem !important;
    gap: 1rem;
  }

  .shop-header {
    padding: 1rem;
    border-radius: 12px;
  }

  .shop-icon {
    padding: 0.65rem;
  }

  .gold-badge {
    width: 100%;
    padding: 0.85rem 1.25rem;
  }

  .items-wrapper {
    padding: 0.85rem;
    border-radius: 12px;
  }
}

/* Móviles muy pequeños */
@media (max-width: 400px) {
  .shop-container {
    padding: 0.5rem !important;
  }

  .shop-header {
    padding: 0.85rem;
    gap: 0.85rem;
  }

  .header-content {
    gap: 0.75rem;
  }

  .gold-badge {
    gap: 0.65rem;
    padding: 0.75rem 1rem;
  }
}

/* Pantallas ultra anchas */
@media (min-width: 1920px) {
  .shop-header,
  .items-wrapper {
    max-width: 1600px;
  }
}

/* Orientación landscape en móviles */
@media (max-height: 500px) and (orientation: landscape) {
  .shop-container {
    padding: 0.75rem 1.5rem !important;
  }

  .shop-header {
    padding: 0.85rem 1.25rem;
  }

  .shop-header-wrapper {
    margin-bottom: 0.75rem;
  }
}

/* Mejoras para dispositivos táctiles */
@media (hover: none) and (pointer: coarse) {
  .gold-badge {
    min-height: 60px;
  }

  .shop-icon {
    min-width: 56px;
    min-height: 56px;
  }
}

/* Scroll suave para la sección de items */
.items-section {
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 215, 0, 0.3) rgba(255, 255, 255, 0.1);
}

.items-section::-webkit-scrollbar {
  width: 8px;
}

.items-section::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
}

.items-section::-webkit-scrollbar-thumb {
  background: rgba(255, 215, 0, 0.3);
  border-radius: 4px;
  transition: background 0.3s ease;
}
</style>
