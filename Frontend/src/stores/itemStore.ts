import type { Item } from "../components/Models/Item";
import { defineStore } from "pinia";
import { ref } from "vue";
import { API_BASE_URL, getAuthHeaders } from '@/config/api'
import { logger } from '@/utils/logger'

export const useItemStore = defineStore('item', () => {
    const items = ref<Item[]>([]);
    const strengthItems = ref<Item[]>([]);
    const enduranceItems = ref<Item[]>([]);
    const generalItems = ref<Item[]>([]);

    // Función auxiliar para mapear los datos y evitar repetir código
    const mapItem = (d: any): Item => ({
        id: d.id,
        name: d.name,
        type: d.type,
        bonus: d.bonus,
        price: d.price,
        imageUrl: d.imageUrl // Se mapea la imagen 
    });

    async function fetchItems() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const data = await response.json();
            
            // Usamos la función auxiliar mapItem
            items.value = data.map(mapItem);
        } catch (error) {
            logger.error("Error fetching items:", error);
        }
    }

    async function fetchDailyStrengthItems() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item/random-strength`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const data = await response.json();
            strengthItems.value = data.map(mapItem);
        } catch (error) {
            logger.error("Error fetching daily strength items:", error);
        }
    }

    async function fetchDailyEnduranceItems() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item/random-endurance`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const data = await response.json();
            enduranceItems.value = data.map(mapItem);
        } catch (error) {
            logger.error("Error fetching daily endurance items:", error);
        }
    }

    async function fetchDailyGeneralItems() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item/random-items`, {
                method: "GET",
                mode: "cors",
                headers: getAuthHeaders()
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const data = await response.json();
            generalItems.value = data.map(mapItem);
        } catch (error) {
            logger.error("Error fetching daily general items:", error);
        }
    }

    async function editItem(itemid: number, item: Item): Promise<void> {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item/${itemid}`, {
                method: 'PUT',
                mode: 'cors',
                headers: getAuthHeaders(),
                body: JSON.stringify(item)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            await fetchItems();
        } catch (error) {
            logger.error('Error editing item:', error);
        }
    }

    async function createItem(item: Omit<Item, 'id'>): Promise<Item | null> {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item`, {
                method: 'POST',
                mode: 'cors',
                headers: getAuthHeaders(),
                body: JSON.stringify(item)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const created = await response.json();
            await fetchItems();
            return created as Item;
        } catch (error) {
            logger.error('Error creating item:', error);
            return null;
        }
    }

    async function deleteItem(itemid: number): Promise<void> {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Item/${itemid}`, {
                method: 'DELETE',
                mode: 'cors',
                headers: getAuthHeaders(),
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            await fetchItems();
            const text = await response.text();
            if (text) return JSON.parse(text);
        } catch (error) {
            logger.error('Error deleting item:', error);
        }
    }

    return {
        items,
        strengthItems,
        enduranceItems,
        generalItems,
        fetchItems,
        fetchDailyStrengthItems,
        fetchDailyEnduranceItems,
        fetchDailyGeneralItems,
        createItem,
        editItem,
        deleteItem
    };
});
