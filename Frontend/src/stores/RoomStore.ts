import type { Room } from "../components/Models/Room"
import { defineStore } from "pinia"
import { ref } from "vue"
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

export const useRoomStore = defineStore('room', () => {
    const room = ref<Room[]>([])
    const activeUsers = ref(0)
    const totalRooms = ref(0);
    const dailyChallenges = ref(0);

    async function fetchRoom() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Room`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const data = await response.json();
            room.value = data.map((d: any) => ({
                id: d.id,
                name: d.name,
                minlevel: d.minlevel,
                minstats: d.minstats,
                minconsistency: d.minconsistency,
                description: d.description,
                date: d.date,
                localization: d.localization
            }));
            totalRooms.value = room.value.length;
        } catch (error) {
            logger.error("Error fetching rooms:", error);
        }
    }

    async function fetchSortedRooms(field: 'level' | 'stats', direction: 'asc' | 'desc') {
        const endpoint = `${API_BASE_URL}/api/Room/sort-${field}-${direction}`;
        try {
            const response = await fetch(endpoint, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const data = await response.json();
            room.value = data.map((d: any) => ({
                id: d.room.id,
                name: d.room.name,
                minlevel: d.room.minlevel,
                minstats: d.room.minstats,
                minconsistency: d.room.minconsistency
            }));
        } catch (error) {
            logger.error("Error fetching sorted rooms:", error);
        }
    }

    async function createRoom(
        newRoom: { name: string; minlevel: number; minstats: number; minconsistency: number, description: string, date: string },
        userid: number
    ) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Room`, {
                method: "POST",
                mode: "cors",
                headers: getAuthHeaders(),
                body: JSON.stringify({
                    room: newRoom,
                    userid: userid
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();

            const createdRoom = {
                id: data,
                ...newRoom,
                users: []
            };

            room.value.push(createdRoom);
        } catch (error) {
            logger.error("Error creating room:", error);
        }
    }

    async function editRoom(roomid: number, newRoom: Room): Promise<void> {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Room/${roomid}`, {
                method: 'PUT',
                mode: 'cors',
                headers: getAuthHeaders(),
                body: JSON.stringify(newRoom)
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            await fetchRoom();
        } catch (error) {
            logger.error('Error editing room:', error);
        }
    }

    return { room, fetchRoom, fetchSortedRooms, createRoom, editRoom, activeUsers, totalRooms, dailyChallenges };
});
