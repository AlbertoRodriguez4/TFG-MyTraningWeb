import { defineStore } from "pinia";
import { API_BASE_URL } from '@/config/api';

export const useMapStore = defineStore('map', () => {
    async function getCooredadas(address: string) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/${encodeURIComponent(address)}`);
        const data = await response.json();
        if (data.length === 0) {
            throw new Error('Endereço não encontrado');
        }
        return { lat: parseFloat(data[0].lat), lon: parseFloat(data[0].lon) };
    }
    async function getEstablecimientos(lat: number, lon: number, tipo: string) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/establecimientos?lat=${lat}&lon=${lon}&tipo=${encodeURIComponent(tipo)}&radio=1000`);
        const data = await response.json();
        return data;
    }

    return { getCooredadas, getEstablecimientos };
})