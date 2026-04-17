import type { Routines } from "@/components/Models/Routines";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useUserStore } from "./userStore";
import { API_BASE_URL, getAuthHeaders, hasValidToken } from '@/config/api';
import { logger } from '@/utils/logger';

export const useRoutineStore = defineStore('routine', () => {
    // --- State ---
    const selectedRoutineId = ref<number | null>(null);
    const routines = ref<Routines[]>([]);

    async function getRoutines() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Task`, {
                method: 'GET',
                mode: 'cors',
                headers: getAuthHeaders()
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const data = await response.json();
            routines.value = data;
        } catch (error) {
            logger.error('Error fetching routines:', error);
        }
    }

    async function createRoutine(routineData: Routines) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Task`, {
                method: 'POST',
                mode: 'cors',
                headers: getAuthHeaders(),
                body: JSON.stringify(routineData)
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const newRoutine = await response.json();
            routines.value.push(newRoutine);
        } catch (error) {
            logger.error('Error creating routine:', error);
        }
    }

    async function getRoutineByUserId(userId: number) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Task/user/${userId}`, {
                method: 'GET',
                mode: 'cors',
                headers: getAuthHeaders()
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const data = await response.json();
            routines.value = data;
        }
        catch (error) {
            logger.error('Error fetching routines by user ID:', error);
        }
    }

    // FUNCIÓN CORREGIDA: Ahora actualiza los datos del usuario después de completar la tarea
    async function completeTask(taskId: number): Promise<void> {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Task/complete/${taskId}`, {
                method: 'POST',
                mode: 'cors',
                headers: getAuthHeaders()
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            // ACTUALIZAR LOS DATOS DEL USUARIO primero para obtener los nuevos valores
            const userStore = useUserStore();
            await userStore.refreshLoggedUser();

            // Solo actualizamos el estado local si todo salió bien
            const taskIndex = routines.value.findIndex(task => task.id === taskId);
            if (taskIndex !== -1) {
                routines.value[taskIndex].iscompleted = true;
            }
        } catch (error) {
            logger.error('Error completing task:', error);
            throw error; // Propagar el error para que el componente lo maneje
        }
    }

    // --- Actions ---
    function setSelectedRoutineId(routineId: number | null) {
        selectedRoutineId.value = routineId;
    }
    
    return {
        selectedRoutineId,
        routines,
        setSelectedRoutineId,
        getRoutines,
        createRoutine,
        getRoutineByUserId,
        completeTask
    };
});