import { defineStore } from 'pinia';
import { ref } from 'vue';
import { API_BASE_URL } from '@/config/api';

export interface BodyMetric {
  id?: number;
  userId?: number;
  date: string;
  weight?: number;
  height?: number;
  bodyFat?: number;
  chest?: number;
  waist?: number;
  arms?: number;
  thighs?: number;
  progressPhotoUrl?: string;
  notes?: string;
}

export const useBodyMetricStore = defineStore('bodyMetric', () => {
  const metrics = ref<BodyMetric[]>([]);
  const latestMetric = ref<BodyMetric | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchMyMetrics() {
    loading.value = true;
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API_BASE_URL}/api/bodymetric/my-metrics`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Error al cargar métricas');
      metrics.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchLatestMetric() {
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API_BASE_URL}/api/bodymetric/my-latest`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.ok) latestMetric.value = await res.json();
    } catch {
      // silenciar
    }
  }

  async function addMetric(metric: BodyMetric) {
    const token = localStorage.getItem('token');
    const res = await fetch(`${API_BASE_URL}/api/bodymetric`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(metric)
    });
    if (!res.ok) {
      const errorText = await res.text();
      console.error('Backend error saving metric:', res.status, errorText);
      throw new Error(`Error al guardar métrica (${res.status}): ${errorText || 'Error desconocido del servidor'}`);
    }
    const created = await res.json();
    metrics.value.unshift(created);
    latestMetric.value = created;
    return created;
  }

  async function deleteMetric(id: number) {
    const token = localStorage.getItem('token');
    const res = await fetch(`${API_BASE_URL}/api/bodymetric/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` }
    });
    if (!res.ok) throw new Error('Error al eliminar métrica');
    metrics.value = metrics.value.filter(m => m.id !== id);
  }

  function calculateBMI(weightKg: number, heightCm: number): number | null {
    if (heightCm <= 0) return null;
    const heightM = heightCm / 100;
    return parseFloat((weightKg / (heightM * heightM)).toFixed(1));
  }

  return {
    metrics,
    latestMetric,
    loading,
    error,
    fetchMyMetrics,
    fetchLatestMetric,
    addMetric,
    deleteMetric,
    calculateBMI
  };
});
