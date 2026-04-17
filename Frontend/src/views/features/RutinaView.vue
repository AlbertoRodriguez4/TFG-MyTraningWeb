<template>
  <v-app>
    <v-main class="workout-hub">
      <!-- Hero Header Section -->
      <HeroSection 
        :user-level="userLevel" 
        :user-x-p="userXP" 
        :xp-to-next-level="xpToNextLevel" 
        :coins="coins"
        :completed-routines="completedRoutines" 
        :streak="streak" 
        :xp-progress="xpProgress"
        @create-routine="openCreateModal" 
      />

      <!-- Calendar Section -->
      <CalendarSection 
        :current-date="currentDate" 
        :month-name="monthName" 
        :days-of-week="daysOfWeek"
        :days-in-month="daysInMonth" 
        :starting-day-of-week="startingDayOfWeek"
        :routines="routineStore.routines.map(r => ({ ...r, createdat: r.createdat ?? r.createdat }))"
        :completed-routines="completedRoutines" 
        :user-x-p="userXP" 
        :xp-progress="xpProgress" 
        :streak="streak"
        @previous-month="previousMonth" 
        @next-month="nextMonth" 
        @day-click="handleDayClick"
        @complete-routine="completeRoutine" 
      />

      <!-- Create Routine Dialog -->
      <CreateRoutineDialog 
        v-model="showCreateModal" 
        :selected-day="selectedDay" 
        :month-name="monthName"
        :user-id="userStore.loggedUser?.id || 0" 
        @create="handleCreateRoutine" 
        @close="closeModal" 
      />

      <!-- Routine Detail Dialog -->
      <RoutineDetailDialog 
        v-model="showDetailModal" 
        :routine="selectedRoutine" 
        @close="closeDetailModal"
        @complete="handleCompleteFromDetail" 
      />

      <!-- Level Up Dialog -->
      <LevelUpDialog v-model="showLevelUp" :user-level="userLevel" />

      <!-- Success Snackbar -->
      <SuccessSnackbar v-model="showCompleted" />

      <!-- Snackbar para notificaciones -->
      <v-snackbar v-model="snackbar.show" :color="snackbar.color" :timeout="3000" top>
        {{ snackbar.message }}
        <template v-slot:actions>
          <v-btn text @click="snackbar.show = false">
            Cerrar
          </v-btn>
        </template>
      </v-snackbar>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, reactive } from 'vue';
import { useRoutineStore } from '@/stores/RoutineStore';
import { useUserStore } from '@/stores/userStore';
import type { Routines } from '@/components/Models/Routines';
import { logger } from '@/utils/logger';

import HeroSection from '../../components/Calendar/HeroSection.vue';
import CalendarSection from '../../components/Calendar/CalendarSection.vue';
import CreateRoutineDialog from '../../components/Calendar/CreateRoutineDialog.vue';
import RoutineDetailDialog from '../../components/Calendar/RoutineDetailDialog.vue';
import LevelUpDialog from '../../components/Calendar/LevelUpDialog.vue';
import SuccessSnackbar from '../../components/Calendar/SuccessSnackbar.vue';

// Stores
const routineStore = useRoutineStore();
const userStore = useUserStore();

// Estado del calendario
const currentDate = ref(new Date());
const selectedDay = ref<number | null>(null);

// Estado de modales y notificaciones
const showCreateModal = ref(false);
const showDetailModal = ref(false);
const showLevelUp = ref(false);
const showCompleted = ref(false);

// Estado de la rutina seleccionada
const selectedRoutine = ref<Routines | null>(null);

// Snackbar
const snackbar = reactive({
  show: false,
  message: '',
  color: 'success'
});

// Contador de reenvío de código (para limpiar en onBeforeUnmount)
let countdownInterval: number | null = null;

// Configuración estática
const daysOfWeek: string[] = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
const monthNames: string[] = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
];

// Computed Properties - Obteniendo datos del loggedUser

/**
 * Nivel del usuario desde el store
 */
const userLevel = computed(() => {
  return userStore.loggedUser?.level || 1;
});

/**
 * XP del usuario - calculado desde strength
 * Puedes ajustar esta lógica según tu modelo de datos
 */
const userXP = computed(() => {
  return userStore.loggedUser?.strength || 0;
});

/**
 * Monedas del usuario desde el store
 */
const coins = computed(() => {
  return userStore.loggedUser?.gold || 0;
});

/**
 * Racha de consistencia del usuario
 */
const streak = computed(() => {
  return userStore.loggedUser?.consistencyStreak || 0;
});

/**
 * XP necesario para el siguiente nivel
 * Fórmula: nivel * 300 + 1000
 */
const xpToNextLevel = computed(() => {
  return calculateNextLevelXP(userLevel.value);
});

/**
 * Porcentaje de progreso hacia el siguiente nivel
 */
const xpProgress = computed(() => {
  return (userXP.value / xpToNextLevel.value) * 100;
});

/**
 * Número total de rutinas completadas
 */
const completedRoutines = computed(() => {
  return routineStore.routines.filter(r => r.iscompleted).length;
});

/**
 * Nombre del mes actual
 */
const monthName = computed(() => {
  return monthNames[currentDate.value.getMonth()];
});

/**
 * Número de días en el mes actual
 */
const daysInMonth = computed(() => {
  return new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 1,
    0
  ).getDate();
});

/**
 * Día de la semana del primer día del mes (0 = Domingo, 6 = Sábado)
 */
const startingDayOfWeek = computed(() => {
  return new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    1
  ).getDay();
});

// Methods

/**
 * Navega al mes anterior
 */
const previousMonth = (): void => {
  currentDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() - 1,
    1
  );
};

/**
 * Navega al mes siguiente
 */
const nextMonth = (): void => {
  currentDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 1,
    1
  );
};

/**
 * Obtiene la rutina para un día específico
 */
const getRoutineForDay = (day: number): Routines | null => {
  const targetDate = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    day
  );
  targetDate.setHours(0, 0, 0, 0);

  return routineStore.routines.find(routine => {
    const routineDate = new Date(routine.createdat);
    routineDate.setHours(0, 0, 0, 0);
    return routineDate.getTime() === targetDate.getTime();
  }) || null;
};

/**
 * Maneja el click en un día del calendario
 */
const handleDayClick = (day: number): void => {
  const routine = getRoutineForDay(day);

  if (routine) {
    // Si hay rutina, mostrar el modal de detalle
    selectedRoutine.value = routine;
    showDetailModal.value = true;
  } else {
    // Si no hay rutina, abrir modal de crear
    selectedDay.value = day;
    showCreateModal.value = true;
  }
};

/**
 * Abre el modal de crear rutina con el día actual
 */
const openCreateModal = (): void => {
  selectedDay.value = new Date().getDate();
  showCreateModal.value = true;
};

/**
 * Crea una nueva rutina
 */
const handleCreateRoutine = async (routine: Routines): Promise<void> => {
  try {

    await routineStore.createRoutine(routine);


    closeModal();

    snackbar.message = '🎉 ¡Rutina creada exitosamente!';
    snackbar.color = 'success';
    snackbar.show = true;

    if (userStore.loggedUser?.id) {
      await routineStore.getRoutineByUserId(userStore.loggedUser.id);
    }

  } catch (error) {
    logger.error('❌ Error al crear la rutina:', error);
    snackbar.message = '❌ Error al crear la rutina. Intenta de nuevo.';
    snackbar.color = 'error';
    snackbar.show = true;
  }
};

/**
 * FUNCIÓN PRINCIPAL: Completa una rutina y actualiza las stats del usuario
 */
const handleCompleteFromDetail = async (routineId: number): Promise<void> => {
  try {
    const routine = routineStore.routines.find(r => r.id === routineId);

    if (!routine) {
      logger.error('❌ Rutina no encontrada');
      return;
    }

    if (routine.iscompleted) {
      logger.warn('⚠️ La rutina ya está completada');
      snackbar.message = '⚠️ Esta rutina ya está completada';
      snackbar.color = 'warning';
      snackbar.show = true;
      return;
    }

    if (!userStore.loggedUser) {
      logger.error('❌ No hay usuario logueado');
      return;
    }


    // 🎯 Llamar a la API para marcar como completada
    await routineStore.completeTask(routineId);


    // 🎯 ACTUALIZAR STATS DEL USUARIO EN EL BACKEND
    const updatedUser = {
      ...userStore.loggedUser,
      strength: (userStore.loggedUser.strength || 0) + routine.reward, // +XP
      gold: (userStore.loggedUser.gold || 0) + 50, // +50 monedas
      consistencyStreak: (userStore.loggedUser.consistencyStreak || 0) + 1, // +1 racha
    };

    // Verificar si sube de nivel
    const newXP = updatedUser.strength;
    const currentXpToNextLevel = calculateNextLevelXP(userStore.loggedUser.level || 1);

    if (newXP >= currentXpToNextLevel) {
      updatedUser.level = (userStore.loggedUser.level || 1) + 1;
      updatedUser.strength = newXP - currentXpToNextLevel; // XP sobrante
      showLevelUp.value = true;
    }

    // 🎯 Actualizar usuario en la API
    await userStore.editUser(userStore.loggedUser.id, updatedUser);


    // Refrescar el usuario logueado para obtener los datos actualizados
    await userStore.refreshLoggedUser();

    // Cerrar modal
    closeDetailModal();

    // Mostrar notificaciones
    showCompleted.value = true;

    snackbar.message = `✅ ¡Rutina completada! +${routine.reward} XP, +50 monedas`;
    snackbar.color = 'success';
    snackbar.show = true;


    // Recargar rutinas
    if (userStore.loggedUser?.id) {
      await routineStore.getRoutineByUserId(userStore.loggedUser.id);
    }

  } catch (error) {
    logger.error('❌ Error al completar rutina:', error);
    snackbar.message = '❌ Error al completar la rutina. Intenta de nuevo.';
    snackbar.color = 'error';
    snackbar.show = true;
  }
};

/**
 * Completa una rutina (wrapper)
 */
const completeRoutine = async (day: number, routineId: number): Promise<void> => {
  await handleCompleteFromDetail(routineId);
};

/**
 * Calcula el XP necesario para alcanzar el siguiente nivel
 */
const calculateNextLevelXP = (level: number): number => {
  return level * 300 + 1000;
};

/**
 * Cierra el modal de crear
 */
const closeModal = (): void => {
  showCreateModal.value = false;
  selectedDay.value = null;
};

/**
 * Cierra el modal de detalle
 */
const closeDetailModal = (): void => {
  showDetailModal.value = false;
  selectedRoutine.value = null;
};

/**
 * Carga los datos iniciales
 */
const loadUserData = async (): Promise<void> => {
  try {
    if (userStore.loggedUser?.id) {


      // Cargar rutinas del usuario
      await routineStore.getRoutineByUserId(userStore.loggedUser.id);


    } else {
      logger.warn('⚠️ No hay usuario logueado');
    }
  } catch (error) {
    logger.error('❌ Error cargando datos:', error);
    snackbar.message = 'Error al cargar los datos';
    snackbar.color = 'error';
    snackbar.show = true;
  }
};

onMounted(() => {
  loadUserData();
});

onBeforeUnmount(() => {
});
</script>

<style scoped>
.workout-hub {
  background: #f8f9fa;
  min-height: 100vh;
  width: 100%;
}

.v-application {
  min-height: 100vh;
}
</style>