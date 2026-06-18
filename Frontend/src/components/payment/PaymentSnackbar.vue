<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const isVisible = ref(false)
const isLoading = ref(true)

const show = () => {
  isVisible.value = true
  isLoading.value = true

  // Simular transición de carga cuando es satisfactorio
  setTimeout(() => {
    isLoading.value = false
  }, 1500)
}

const hide = () => {
  isVisible.value = false
}

defineExpose({ show, hide })
</script>

<template>
  <teleport to="body">
    <transition
      name="backdrop"
      @after-leave="isVisible = false"
    >
      <div v-if="isVisible" class="backdrop" @click="hide"></div>
    </transition>

    <transition name="snackbar" @after-leave="isVisible = false">
      <div v-if="isVisible" class="snackbar-wrapper">
        <div class="snackbar">
          <div v-if="isLoading" class="success-animation-wrapper">
            <div class="success-circle">
              <div class="spinner"></div>
            </div>
          </div>

          <div v-else class="success-content">
            <div class="checkmark-wrapper">
              <svg class="checkmark" viewBox="0 0 52 52">
                <circle class="checkmark-circle" cx="26" cy="26" r="25" fill="none" />
                <path class="checkmark-check" fill="none" d="M14.1 27.2l7.1 7.2 16.7-16.8" />
              </svg>
            </div>

            <div class="message-content">
              <h3 class="message-title">{{ $t('payment_completed') }}</h3>
              <p class="message-description">
                {{ $t('subscription_activated_redirect') }}
              </p>
            </div>
          </div>

          <button class="close-btn" @click="hide" :aria-label="$t('close')">
            <v-icon size="24">mdi-close</v-icon>
          </button>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<style scoped>
/* Fondo */
.backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.4);
  z-index: 999;
  backdrop-filter: blur(4px);
}

/* Transición del fondo */
.backdrop-enter-active,
.backdrop-leave-active {
  transition: opacity 0.3s ease;
}

.backdrop-enter-from,
.backdrop-leave-to {
  opacity: 0;
}

/* Contenedor del snackbar */
.snackbar-wrapper {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 1000;
  pointer-events: none;
}

/* Snackbar */
.snackbar {
  background: white;
  border-radius: 16px;
  padding: 2.5rem;
  min-width: 300px;
  max-width: 500px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
  display: flex;
  align-items: center;
  gap: 1.5rem;
  position: relative;
  pointer-events: auto;
  backdrop-filter: blur(20px);
  border: 1px solid rgba(99, 102, 241, 0.2);
}

/* Transición del snackbar */
.snackbar-enter-active,
.snackbar-leave-active {
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.snackbar-enter-from {
  opacity: 0;
  transform: translate(-50%, -50%) scale(0.8);
}

.snackbar-leave-to {
  opacity: 0;
  transform: translate(-50%, -50%) scale(0.8);
}

/* Animación de éxito (carga) */
.success-animation-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
}

.success-circle {
  position: relative;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;

}

/* Contenido de éxito */
.success-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

/* Checkmark animado */
.checkmark-wrapper {
  flex-shrink: 0;
}

.checkmark {
  width: 60px;
  height: 60px;
  filter: drop-shadow(0 2px 8px rgba(34, 197, 94, 0.2));
}

.checkmark-circle {
  stroke: #22c55e;
  stroke-width: 2;

}

.checkmark-check {
  stroke: #22c55e;
  stroke-width: 3;
  stroke-linecap: round;
  stroke-linejoin: round;

  stroke-dasharray: 48;
  stroke-dashoffset: 48;
}

/* Contenido del mensaje */
.message-content {
  flex: 1;
}

.message-title {
  font-size: 1.2rem;
  font-weight: 800;
  margin: 0 0 0.4rem;
  color: #1a1a1a;
  letter-spacing: -0.3px;
}

.message-description {
  font-size: 0.9rem;
  color: rgba(0, 0, 0, 0.8);
  margin: 0;
  line-height: 1.6;
}

/* Botón de cierre */
.close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  color: #64748b;
  transition: all 0.2s ease;
  z-index: 1;
}

.close-btn:hover {
  background: #f3f4f6;
  color: #1a1a1a;
}

/* Responsive */
@media (max-width: 600px) {
  .snackbar {
    min-width: auto;
    max-width: 90vw;
    padding: 2rem;
    flex-direction: column;
    text-align: center;
  }

  .success-content {
    flex-direction: column;
    gap: 1rem;
  }

  .message-title {
    font-size: 1.1rem;
  }

  .message-description {
    font-size: 0.85rem;
  }

  .checkmark {
    width: 50px;
    height: 50px;
  }

  .close-btn {
    top: 0.8rem;
    right: 0.8rem;
  }
}

@media (max-width: 480px) {
  .snackbar {
    padding: 1.5rem;
    min-width: auto;
    max-width: 85vw;
  }

  .checkmark {
    width: 44px;
    height: 44px;
  }

  .message-title {
    font-size: 1rem;
  }

  .message-description {
    font-size: 0.8rem;
  }
}
</style>