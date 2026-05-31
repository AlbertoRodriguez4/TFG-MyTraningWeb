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
        :routines="routineStore.routines"
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
        :selected-date="selectedDate"
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
            {{ $t('close') }}
          </v-btn>
        </template>
      </v-snackbar>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
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
const { t } = useI18n();

// Estado del calendario
const currentDate = ref(new Date());
const selectedDay = ref<number | null>(null);

// Fecha completa del día seleccionado para la creación de rutinas
const selectedDate = computed(() => {
  if (!selectedDay.value) return null;
  return new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    selectedDay.value
  );
});

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

// Configuración estática, nunca cambian 
const daysOfWeek: string[] = [t('sunday'), t('monday'), t('tuesday'), t('wednesday'), t('thursday'), t('friday'), t('saturday')];
const monthNames: string[] = [
  t('january'), t('february'), t('march'), t('april'), t('may'), t('june'),
  t('july'), t('august'), t('september'), t('october'), t('november'), t('december')
];

// Obtener nivel del usuario 
const userLevel = computed(() => {
  return userStore.loggedUser?.level || 1;
});

// Obtener el xp del usuario
const userXP = computed(() => {
  return userStore.loggedUser?.strength || 0;
});

// Dinero del usuario 
const coins = computed(() => {
  return userStore.loggedUser?.gold || 0;
});

// Consistencia del usuario 
const streak = computed(() => {
  return userStore.loggedUser?.consistencyStreak || 0;
});

//Calculadora de nivel siguiente
// Se calcula el XP necesario para alcanzar el siguiente nivel usando la fórmula: XP necesario = nivel actual * 300 + 1000. 
// Esto crea una progresión de XP que aumenta con cada nivel, haciendo que subir de nivel sea cada vez más desafiante a medida que el usuario avanza. 
// El resultado se muestra en la interfaz para que el usuario sepa cuánto XP necesita para alcanzar el próximo nivel.
const xpToNextLevel = computed(() => {
  return calculateNextLevelXP(userLevel.value);
});

// Porcentaje de progrso hacia el siguiente nivel 
const xpProgress = computed(() => {
  return (userXP.value / xpToNextLevel.value) * 100;
});



const completedRoutines = computed(() => {
  return routineStore.routines.filter(r => r.iscompleted).length;
});

// Obtener el nombre del mes actual para mostrarlo en el calendario
const monthName = computed(() => {
  return monthNames[currentDate.value.getMonth()];
});

 // Numero de dias del mes actual, se calcula creando una fecha con el día 0 del siguiente mes, lo que devuelve el último día del mes actual.
const daysInMonth = computed(() => {
  return new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 1,
    0
  ).getDate();
});

// Obtener el día de la semana del primer día del mes para alinear correctamente el calendario, se calcula creando una fecha con el día 1 del mes actual y 
// obteniendo su día de la semana.
const startingDayOfWeek = computed(() => {
  return new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    1
  ).getDay();
});

// Methods

// Mes anterior 
const previousMonth = (): void => {
  currentDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() - 1,
    1
  );
};

// Mes siguiente 
const nextMonth = (): void => {
  currentDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 1,
    1
  );
};

// Función para obtener la rutina de un día específico, se compara la fecha del día clicado con las fechas de creación de las rutinas del usuario para encontrar 
// si hay una rutina asignada a ese día.
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

// Verificar que la fecja clicada no sea una fecha anterior a hoy
const isPastDate = (day: number): boolean => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const clickedDate = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    day
  );
  clickedDate.setHours(0, 0, 0, 0);

  return clickedDate.getTime() < today.getTime();
};

// Manejo de clicar en el calendario
const handleDayClick = (day: number): void => {
  const routine = getRoutineForDay(day);

  if (routine) {
    // Si hay rutina, mostrar el modal de detalle
    selectedRoutine.value = routine;
    showDetailModal.value = true;
  } else if (isPastDate(day)) {
    // No permitir crear rutinas en fechas pasadas
    snackbar.message = t('calendar.cannotCreatePast');
    snackbar.color = 'warning';
    snackbar.show = true;
  } else {
    // Si no hay rutina y no es fecha pasada, abrir modal de crear
    selectedDay.value = day;
    showCreateModal.value = true;
  }
};

//Abrir el modal de crear rutina, se asigna la fecha seleccionada al estado para que el modal tenga la información necesaria para crear la rutina con la fecha correcta.
const openCreateModal = (): void => {
  currentDate.value = new Date();
  selectedDay.value = new Date().getDate();
  showCreateModal.value = true;
};

//Crear nueva rutina 
const handleCreateRoutine = async (routine: Routines): Promise<void> => {
  try {

    await routineStore.createRoutine(routine);


    closeModal();

    snackbar.message = t('routine_created_success');
    snackbar.color = 'success';
    snackbar.show = true;

    if (userStore.loggedUser?.id) {
      await routineStore.getRoutineByUserId(userStore.loggedUser.id);
    }

  } catch (error) {
    logger.error('❌ Error al crear la rutina:', error);
    snackbar.message = t('error_creating_routine');
    snackbar.color = 'error';
    snackbar.show = true;
  }
};

const handleCompleteFromDetail = async (routineId: number): Promise<void> => {
  try {
    const routine = routineStore.routines.find(r => r.id === routineId);

    if (!routine) {
      logger.error('❌ Rutina no encontrada');
      return;
    }

    if (routine.iscompleted) {
      logger.warn('⚠️ La rutina ya está completada');
      snackbar.message = t('routine_already_completed');
      snackbar.color = 'warning';
      snackbar.show = true;
      return;
    }

    if (!userStore.loggedUser) {
      logger.error('❌ No hay usuario logueado');
      return;
    }

    const previousLevel = userStore.loggedUser.level || 1;

    await routineStore.completeTask(routineId);

    await userStore.refreshLoggedUser();

    if ((userStore.loggedUser?.level || 1) > previousLevel) {
      showLevelUp.value = true;
    }

    closeDetailModal();

    showCompleted.value = true;

    snackbar.message = t('routine_completed_reward', { reward: routine.reward });
    snackbar.color = 'success';
    snackbar.show = true;

    if (userStore.loggedUser?.id) {
      await routineStore.getRoutineByUserId(userStore.loggedUser.id);
    }

  } catch (error) {
    logger.error('❌ Error al completar rutina:', error);
    snackbar.message = t('error_completing_routine');
    snackbar.color = 'error';
    snackbar.show = true;
  }
};


// Completar rutina desde el modal 
const completeRoutine = async (day: number, routineId: number): Promise<void> => {
  await handleCompleteFromDetail(routineId);
};

// Función para calcular el XP necesario para alcanzar el siguiente nivel usando la fórmula: XP necesario = nivel actual * 300 + 1000.
const calculateNextLevelXP = (level: number): number => {
  return level * 300 + 1000;
};

// Cerrar el modal 
const closeModal = (): void => {
  showCreateModal.value = false;
  selectedDay.value = null;
};

// Cerrar el modal de detalle de rutina
const closeDetailModal = (): void => {
  showDetailModal.value = false;
  selectedRoutine.value = null;
};

// Cargar los datos iniciales del usuario al montar el componente, se cargan las rutinas del usuario para mostrarlas en el calendario y tener la 
// información necesaria para manejar las acciones de completar rutinas y mostrar los detalles.
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
    snackbar.message = t('error_loading_data');
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
  background: var(--bg-primary);
  min-height: 100vh;
  width: 100%;
}

.v-application {
  min-height: 100vh;
}
</style>