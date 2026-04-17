import type { Plan } from "../components/Models/Plan"
import { defineStore } from "pinia"
import { ref } from "vue"
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

export const usePlanStore = defineStore('plan', () => {
    const plan = ref<Plan[]>([])

    async function fetchPlan() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Plan`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            })

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`)
            const data = await response.json()
            plan.value = data.map((d: any) => ({
                id: d.id,
                userId: d.userId,
                description: d.description
            }))
        } catch (error) {
            logger.error("Error fetching plans:", error)
        }
    }

    return { plan, fetchPlan }
})
