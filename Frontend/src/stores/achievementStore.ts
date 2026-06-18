import { defineStore } from 'pinia';
import { ref } from 'vue';
import { API_BASE_URL } from '@/config/api';
import { useI18n } from 'vue-i18n';

export interface Achievement {
  id: number;
  name: string;
  description: string;
  iconUrl: string;
  category: string;
  requirementType: string;
  requirementValue: number;
  rewardGold: number;
  rewardXP: number;
  isSecret: boolean;
}

export interface UserAchievement {
  id: number;
  userId: number;
  achievementId: number;
  achievement: Achievement;
  unlockedAt: string;
  isNew: boolean;
}

export const useAchievementStore = defineStore('achievement', () => {
  const achievements = ref<Achievement[]>([]);
  const userAchievements = ref<UserAchievement[]>([]);
  const totalCount = ref(0);
  const newCount = ref(0);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // obtener todos los logros
  async function fetchAllAchievements() {
    loading.value = true;
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API_BASE_URL}/api/achievement`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Error al cargar logros');
      achievements.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }
  // obtener logros del usuario
  async function fetchMyAchievements() {
    loading.value = true;
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        error.value = 'No hay token de autenticación';
        return;
      }
      const res = await fetch(`${API_BASE_URL}/api/achievement/my-achievements`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) throw new Error(`Error al cargar mis logros: ${res.status}`);
      const data = await res.json();
      userAchievements.value = Array.isArray(data) ? data.map((ua: any) => ({
        id: ua.id ?? ua.Id,
        userId: ua.userId ?? ua.UserId,
        achievementId: ua.achievementId ?? ua.AchievementId,
        achievement: ua.achievement ?? ua.Achievement,
        unlockedAt: ua.unlockedAt ?? ua.UnlockedAt,
        isNew: ua.isNew ?? ua.IsNew
      })) : [];
    } catch (e: any) {
      error.value = e.message;
      userAchievements.value = [];
    } finally {
      loading.value = false;
    }
  }
  // obtener conteo de logros nuevos y totales
  async function fetchAchievementCount() {
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API_BASE_URL}/api/achievement/my-count`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) return;
      const data = await res.json();
      totalCount.value = data.total;
      newCount.value = data.newAchievements;
    } catch {
      // silenciar
    }
  }
  // marcar logro como visto
  async function markAsSeen(userAchievementId: number) {
    try {
      const token = localStorage.getItem('token');
      await fetch(`${API_BASE_URL}/api/achievement/mark-seen/${userAchievementId}`, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` }
      });
      const ua = userAchievements.value.find(x => x.id === userAchievementId);
      if (ua) ua.isNew = false;
      newCount.value = Math.max(0, newCount.value - 1);
    } catch {
      // silenciar
    }
  }
  // crear nuevo logro
  async function createAchievement(achievement: Omit<Achievement, 'id'>) {
    const token = localStorage.getItem('token');
    const res = await fetch(`${API_BASE_URL}/api/achievement`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(achievement)
    });
    if (!res.ok) throw new Error('Error al crear logro');
    const created = await res.json();
    achievements.value.push(created);
    return created;
  }
// actualizar logro existente
  async function updateAchievement(id: number, achievement: Omit<Achievement, 'id'>) {
    const token = localStorage.getItem('token');
    const res = await fetch(`${API_BASE_URL}/api/achievement/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(achievement)
    });
    if (!res.ok) throw new Error('Error al actualizar logro');
    const updated = await res.json();
    const idx = achievements.value.findIndex(a => a.id === id);
    if (idx !== -1) achievements.value[idx] = updated;
    return updated;
  }

  async function deleteAchievement(id: number) {
    const token = localStorage.getItem('token');
    const res = await fetch(`${API_BASE_URL}/api/achievement/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` }
    });
    if (!res.ok) throw new Error('Error al eliminar logro');
    achievements.value = achievements.value.filter(a => a.id !== id);
  }

  return {
    achievements,
    userAchievements,
    totalCount,
    newCount,
    loading,
    error,
    fetchAllAchievements,
    fetchMyAchievements,
    fetchAchievementCount,
    markAsSeen,
    createAchievement,
    updateAchievement,
    deleteAchievement
  };
});
