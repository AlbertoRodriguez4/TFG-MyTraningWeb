import type { Item } from "../components/Models/Item"
import { defineStore } from "pinia"
import { ref } from "vue"
import { useUserStore } from "./userStore"
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

export const usePurchaseStore = defineStore('purchase', () => {
    const purchase = ref<Item[]>([])

    async function fetchPurchase() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            })

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`)
            const data = await response.json()
            purchase.value = data.map((d: any) => ({
                id: d.id,
                name: d.name,
                type: d.type,
                bonus: d.bonus,
                price: d.price
            }))
        } catch (error) {
            logger.error("Error fetching plans:", error)
        }
    }

    // Hacer compra de objeto y actualizar los datos del usuario
    async function addPurchase(userid: number, itemid: number, itemPrice: number) {
        // Cuerpo de la petición con los datos necesarios para realizar la compra
        const body = {
            userid,
            itemid,
            goldToSubtract: itemPrice
        }

        try {
            const response = await fetch(`${API_BASE_URL}/api/Purchase`, {
                method: "POST",
                mode: "cors",
                headers: getAuthHeaders(),
                body: JSON.stringify(body)
            })

            const result = await response.json()
            if (!response.ok) {
                throw { status: response.status, data: result }
            }

            
            // Se actualizan los datos del usuario para reflejar la compra (restar oro, añadir objeto al inventario, etc.)
            const userStore = useUserStore();
            await userStore.refreshLoggedUser();
            
            
            return result

        } catch (error) {
            logger.error("Error al agregar compra:", error)
            throw error
        }
    }

    return { purchase, fetchPurchase, addPurchase }
})