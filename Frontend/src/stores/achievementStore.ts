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

  async function fetchAllAchievements() {
    loading.value = true;
    try {
      const res = await fetch(`${API_BASE_URL}/api/achievement`);
      if (!res.ok) throw new Error('Error al cargar logros');
      achievements.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchMyAchievements() {
    loading.value = true;
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${API_BASE_URL}/api/achievement/my-achievements`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) throw new Error('Error al cargar mis logros');
      userAchievements.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

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
