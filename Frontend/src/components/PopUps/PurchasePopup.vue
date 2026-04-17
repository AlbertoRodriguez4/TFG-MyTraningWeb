<script setup lang="ts">
import { useUserStore } from '@/stores/userStore'
import { usePurchaseStore } from '@/stores/PurchaseStore'
import { ref, watchEffect, watch } from 'vue'
import { logger } from '@/utils/logger'

const store = useUserStore()
const purchaseStore = usePurchaseStore()
const loggedUser = ref(store.loggedUser)

watchEffect(() => {
  loggedUser.value = store.loggedUser
})

const props = defineProps<{
  visible: boolean,
  item: {
    name: string,
    bonus: number,
    id: number,
    price: number,
    type: string
  }
}>()

const emit = defineEmits(['close', 'buy'])

// Snackbar state
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')

function showSnackbar(text: string, color: string = 'success') {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}

const dialogVisible = ref(props.visible)
watch(() => props.visible, val => dialogVisible.value = val)
watch(dialogVisible, val => { if (!val) emit('close') })

const handleBuy = async () => {
  const userId = loggedUser.value?.id
  if (!userId) {
    logger.error("Usuario no identificado")
    return
  }

  try {
    const purchase = await purchaseStore.addPurchase(userId, props.item.id, props.item.price)

    if (purchase && purchase.id) {
      showSnackbar("Compra realizada correctamente", 'success')
      await store.refreshLoggedUser()
      dialogVisible.value = false
    } else {
      throw new Error("Respuesta inválida del servidor")
    }
  } catch (error: any) {
    const message = error?.data?.message
    if (message?.includes("no tiene suficiente oro")) {
      showSnackbar("No tienes suficiente oro para realizar esta compra", 'error')
    } else if (message) {
      showSnackbar(message, 'error')
    } else {
      logger.error("Error al realizar la compra:", error)
      showSnackbar("Hubo un problema al realizar la compra", 'error')
    }
    dialogVisible.value = false
  }
}
</script>

<template>
  <v-dialog v-model="dialogVisible" max-width="400">
    <v-card>
      <v-card-title class="text-h6">{{ item.name }}</v-card-title>

      <v-card-subtitle>
        {{ $t('price') }}: {{ item.price }} {{ $t('oro') }}
      </v-card-subtitle>

      <v-card-text>
        <!-- Si tienes imagen, reemplaza src -->
        <v-img
          src="/path/to/item-image.jpg"
          alt="Imagen del ítem"
          height="200"
          contain
          class="mb-4"
        />

        <div v-if="item.type === 'Strength'" class="text-subtitle-2">
          Bonus: +{{ item.bonus }} {{ $t('fuerza') }}
        </div>
        <div v-else-if="item.type === 'Endurance'" class="text-subtitle-2">
          Bonus: +{{ item.bonus }} {{ $t('resistencia') }}
        </div>
      </v-card-text>

      <v-card-actions class="justify-end">
        <v-btn variant="outlined" color="secondary" @click="dialogVisible = false">
          {{ $t('cancelar') }}
        </v-btn>
        <v-btn color="primary" @click="handleBuy">
          {{ $t('comprar') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Snackbar -->
  <v-snackbar
    v-model="snackbar"
    :color="snackbarColor"
    :timeout="3000"
    location="top"
    rounded="pill"
  >
    <div class="d-flex align-center">
      <v-icon class="mr-2">
        {{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}
      </v-icon>
      {{ snackbarText }}
    </div>
  </v-snackbar>
</template>

<style scoped>
.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}
.popup-button.cancel {
  background-color: red;
  color: #000;
}

.popup-button.cancel:hover {
  background-color: #999;
}
.popup-container {
  background-color: white;
  border: 4px solid #007bff;
  border-radius: 12px;
  padding: 20px;
  width: 300px;
  text-align: center;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
}

.popup-title {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 15px;
  color: #007bff;
}

.popup-buy {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 15px;
  color: #ffc107;
}

.popup-image {
  width: 100%;
  height: 180px;
  background-color: #d3d3d3;
  margin-bottom: 20px;
}

.popup-bonus {
  font-size: 20px;
  font-weight: bold;
  margin-bottom: 20px;
  color: #007bff;
}

.popup-button {
  background-color: white;
  color: black;
  border: 2px solid #ffc107;
  border-radius: 20px;
  padding: 10px 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.popup-button:hover {
  background-color: #fff3cd;
}
</style>