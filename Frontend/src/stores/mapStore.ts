import { defineStore } from "pinia";
import { API_BASE_URL } from '@/config/api';

export const useMapStore = defineStore('map', () => {
    async function getCooredadas(address: string) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/${encodeURIComponent(address)}`);
        if (!response.ok) {
            throw new Error('No se pudieron obtener coordenadas para la dirección');
        }

        const data = await response.json();
        if (data == null || typeof data.lat !== 'number' || typeof data.lon !== 'number') {
            throw new Error('Dirección no encontrada');
        }

        return { lat: data.lat, lon: data.lon };
    }

    async function getEstablecimientos(lat: number, lon: number, tipo: string) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/establecimientos?lat=${lat}&lon=${lon}&tipo=${encodeURIComponent(tipo)}&radio=1000`);
        if (!response.ok) {
            throw new Error('No se pudieron obtener establecimientos cercanos');
        }

        const data = await response.json();
        return data;
    }

    return { getCooredadas, getEstablecimientos };
})
