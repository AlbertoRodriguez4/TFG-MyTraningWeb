import { defineStore } from "pinia"
import { ref, computed } from "vue"
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

// --- Interfaces ---
export interface UserRoom {
    userid: number;
    roomid: number;
    user?: any;
    room?: any;
}

export const useUserRoomStore = defineStore('userRoom', () => {
    // Estado del store
    const allUserRooms = ref<UserRoom[]>([]);
    const currentUserRooms = ref<UserRoom[]>([]);
    const currentRoomMembers = ref<UserRoom[]>([]);

    const loading = ref(false);
    const error = ref<string | null>(null);

  

    // Obtener todas las relaciones usuario-sala (para admin)
    async function fetchAllUserRooms() {
        loading.value = true;
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom`, {
                method: "GET",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const data = await response.json();
            allUserRooms.value = data;
        } catch (err: any) {
            logger.error("Error fetching all user rooms:", err);
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    }

    // Obtener salas a las que pertenece un usuario
    async function fetchRoomsByUserId(userId: number) {
        loading.value = true;
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom/user/${userId}`, {
                method: "GET",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const data = await response.json();
            currentUserRooms.value = data;
        } catch (err: any) {
            logger.error("Error fetching rooms by user id:", err);
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    }

    //Obtener miembros de una sala 
    async function fetchMembersByRoomId(roomId: number) {
        loading.value = true;
        currentRoomMembers.value = []; 
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom/room/${roomId}`, {
                method: "GET",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            // Se recupera data con formato de array con la estructura { userId, roomId, user: { id, name, ... }, room: { id, name, ... } }
            const data = await response.json();
            currentRoomMembers.value = data;
            
        } catch (err: any) {
            logger.error("Error fetching room members:", err);
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    }

    // Unirse a una sala (Add), al unirse a una sala se actualizan las salas a las que pertenece el usuario y los miembros de la sala para reflejar los cambios
    async function joinRoom(userId: number, roomId: number) {
        loading.value = true;
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom`, {
                method: "POST",
                headers: getAuthHeaders(),
                body: JSON.stringify({ userid: userId, roomid: roomId })
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || `HTTP error! Status: ${response.status}`);
            }

            await fetchRoomsByUserId(userId);
            return true;
        } catch (err: any) {
            logger.error("Error joining room:", err);
            error.value = err.message;
            throw err; 
        } finally {
            loading.value = false;
        }
    }

    // Salir de una sala (Delete), al salir de una sala se actualizan las salas a las que pertenece el usuario y los miembros de la sala para reflejar los cambios
    async function leaveRoom(userId: number, roomId: number) {
        loading.value = true;
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom/${userId}/${roomId}`, {
                method: "DELETE",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            currentUserRooms.value = currentUserRooms.value.filter(
                ur => !(ur.userid === userId && ur.roomid === roomId)
            );
            
            return true;
        } catch (err: any) {
            logger.error("Error leaving room:", err);
            error.value = err.message;
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // Actualizar relación (Update)
    async function updateUserRoom(userId: number, roomId: number, data: UserRoom) {
        loading.value = true;
        try {
            const response = await fetch(`${API_BASE_URL}/api/UserRoom/${userId}/${roomId}`, {
                method: "PUT",
                headers: getAuthHeaders(),
                body: JSON.stringify(data)
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            return true;
        } catch (err: any) {
            logger.error("Error updating user room:", err);
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    }

    // Funcion para obtener los miembros de una sala y el número de miembros de la sala para mostrarlo en la vista de la sala
    const memberCount = computed(() => currentRoomMembers.value.length);
    
    // Comprobación para mostrar en la vista de la sala si un usuario es miembro de la sala o no (para mostrar botón de unirse o salir de la sala)
    // Se comprueba si el nombre de usuario del usuario actual está en la lista de miembros de la sala
    const isMemberInRoom = computed(() => (userName: string) => {
        return currentRoomMembers.value.some(m => m.user?.name === userName);
    });

    return {
        allUserRooms,
        currentUserRooms,
        currentRoomMembers,
        loading,
        error,
        fetchAllUserRooms,
        fetchRoomsByUserId,
        fetchMembersByRoomId,
        joinRoom,
        leaveRoom,
        updateUserRoom,
        memberCount,
        isMemberInRoom
    }
});