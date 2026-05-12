import { defineStore } from 'pinia';
import { ref } from 'vue';
import { API_BASE_URL } from '@/config/api';

export interface Exercise {
  id: number;
  name: string;
  description: string;
  muscleGroup: string;
  difficulty: string;
  imageUrl?: string;
  videoUrl?: string;
  gifUrl?: string;
  equipment: string;
  instructions?: string;
  commonMistakes?: string;
  bodyPart?: string;
  target?: string;
  category?: string;
  secondaryMuscles?: string[];
  instructionSteps?: string[];
}

export const useExerciseStore = defineStore('exercise', () => {
  const exercises = ref<Exercise[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const muscleGroups = [
    'chest', 'back', 'shoulders', 'arms', 'legs', 'core', 'cardio'
  ];

  const difficulties = ['beginner', 'intermediate', 'advanced'];

  async function fetchExercises() {
    loading.value = true;
    try {
      const res = await fetch(`${API_BASE_URL}/api/exercise`);
      if (!res.ok) throw new Error('Error al cargar ejercicios');
      exercises.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

  async function searchExercises(query: string) {
    loading.value = true;
    try {
      const res = await fetch(`${API_BASE_URL}/api/exercise/search/${encodeURIComponent(query)}`);
      if (!res.ok) throw new Error('Error al buscar ejercicios');
      exercises.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

  async function filterByMuscleGroup(muscleGroup: string) {
    loading.value = true;
    try {
      const res = await fetch(`${API_BASE_URL}/api/exercise/muscle-group/${encodeURIComponent(muscleGroup)}`);
      if (!res.ok) throw new Error('Error al filtrar ejercicios');
      exercises.value = await res.json();
    } catch (e: any) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }

  return {
    exercises,
    loading,
    error,
    muscleGroups,
    difficulties,
    fetchExercises,
    searchExercises,
    filterByMuscleGroup
  };
});
