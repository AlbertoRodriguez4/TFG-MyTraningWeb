import { defineStore } from "pinia";
import { useI18n } from 'vue-i18n'
import { API_BASE_URL } from '@/config/api';

export const useMapStore = defineStore('map', () => {
    const { t } = useI18n()
    async function getCooredadas(address: string) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/${encodeURIComponent(address)}`);
        if (!response.ok) {
            throw new Error(t('map_coords_error'));
        }

        const data = await response.json();
        if (data == null || typeof data.lat !== 'number' || typeof data.lon !== 'number') {
            throw new Error(t('map_address_not_found'));
        }

        return { lat: data.lat, lon: data.lon };
    }

    async function getEstablecimientos(lat: number, lon: number, tipo: string, radio: number = 2000) {
        const response = await fetch(`${API_BASE_URL}/api/geocodificacion/establecimientos?lat=${lat}&lon=${lon}&tipo=${encodeURIComponent(tipo)}&radio=${radio}`);
        if (!response.ok) {
            throw new Error(t('map_establishments_error'));
        }

        const data = await response.json();
        return data;
    }

    return { getCooredadas, getEstablecimientos };
})
