<template>
  <div class="day-item">
    <v-card
      :class="getDayCardClass"
      elevation="4"
      @click="handleClick"
    >
      <div class="day-card-inner">
        <div class="day-top">
          <span class="day-num">{{ day }}</span>
          <v-chip
            v-if="isToday"
            x-small
            color="purple"
            dark
            class="today-chip"
          >
            {{ $t('common.today') }}
          </v-chip>
        </div>

        <div class="day-body">
          <div v-if="routine" class="has-workout">
            <div class="workout-icon-container">
              <v-icon 
                :color="white"
                size="28"
              >
                {{ routine.iscompleted ? 'mdi-check-circle' : 'mdi-dumbbell' }}
              </v-icon>
            </div>
            <p class="workout-name">{{ routine.name }}</p>
            
            <div v-if="!routine.iscompleted" class="pending-indicator">
                <v-chip small color="orange" dark>
                <v-icon small left>mdi-clock-outline</v-icon>
                {{ $t('common.pending') }}
              </v-chip>
              <p class="click-hint">{{ $t('calendar.clickToComplete') }}</p>
            </div>

            <div v-else class="completed-badge">
              <v-icon small color="white" class="mr-1">mdi-trophy</v-icon>
              <span>+{{ routine.reward }} XP</span>
            </div>
          </div>

          <div v-else class="no-workout">
            <div class="add-workout-icon">
              <v-icon size="40" color="white">mdi-plus</v-icon>
            </div>
            <p class="add-workout-text">{{ $t('calendar.addRoutine') }}</p>
          </div>
        </div>
      </div>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Routines } from '../Models/Routines';

interface Props {
  day: number;
  routine?: Routines | null;
  isToday?: boolean;
  currentDate: Date;
  isPastDate?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  routine: null,
  isToday: false,
  isPastDate: false
});

const emit = defineEmits<{
  click: [];
  complete: [];
}>();
// Computed para determinar las clases del card según el estado de la rutina
const getDayCardClass = computed(() => {
  return {
    'day-card': true,
    'day-card-today': props.isToday,
    'day-card-completed': props.routine?.iscompleted,
    'day-card-pending': props.routine && !props.routine.iscompleted,
    'day-card-empty': !props.routine,
    // Añadir clase para indicar que es clickeable solo si está pendiente o vacía
    'clickable': !props.routine?.iscompleted
  };
});
// Función para manejar el click en la tarjeta del día
const handleClick = () => {


  // Si no hay rutina, abrir modal de crear
  if (!props.routine) {
    emit('click');
    return;
  }

  // Si hay rutina PENDIENTE, abrir modal de detalle para completarla
  if (!props.routine.iscompleted) {
    emit('click');
    return;
  }

  // Si la rutina ya está COMPLETADA, no hacer nada
};
</script>

<style scoped>
.day-item {
  aspect-ratio: 1;

}

.day-card {
  height: 100%;
  border-radius: 16px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 3px solid rgba(139, 92, 246, 0.3);
  background: linear-gradient(135deg, rgba(26, 10, 46, 0.8) 0%, rgba(15, 10, 26, 0.9) 100%);
  backdrop-filter: blur(10px);
}

/* Solo aplicar cursor pointer y hover si es clickeable */
.day-card.clickable {
  cursor: pointer;
}

/* Las completadas tienen cursor normal y no tienen hover */
.day-card-completed {
  cursor: default;
  border-color: rgba(52, 211, 153, 0.6);
  background: linear-gradient(135deg, rgba(52, 211, 153, 0.15), rgba(16, 185, 129, 0.1));
  opacity: 0.9;
}

.day-card-empty {
  border-style: dashed;
  border-color: rgba(255, 255, 255, 0.2);
}

.day-card-today {
  border-color: rgba(167, 139, 250, 0.8);
  background: linear-gradient(135deg, rgba(167, 139, 250, 0.15), rgba(34, 211, 238, 0.1));
  box-shadow: 0 8px 25px rgba(167, 139, 250, 0.4) !important;
}

.day-card-pending {
  border-color: rgba(251, 191, 36, 0.6);
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.12), rgba(245, 158, 11, 0.1));

}

.day-card-inner {
  padding: 1rem;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.day-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.day-num {
  font-size: 1.8rem;
  font-weight: 900;
  color: #ffffff;
  text-shadow: 0 0 15px rgba(167, 139, 250, 0.4);
}

.today-chip {
  font-weight: 800;
  letter-spacing: 0.5px;
}

.day-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.has-workout {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.workout-icon-container {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: rgba(167, 139, 250, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.75rem;
  transition: all 0.3s ease;
  box-shadow: 0 2px 12px rgba(167, 139, 250, 0.2);
}

.workout-name {
  font-size: 0.9rem;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.85);
  margin-bottom: 0.75rem;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-height: 1.3;
}

.pending-indicator {
  margin-top: auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.click-hint {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.6);
  font-weight: 600;
  margin: 0;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.completed-badge {
  background: #4caf50;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: 800;
  font-size: 0.85rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: auto;
  box-shadow: 0 2px 8px rgba(76, 175, 80, 0.3);
}

.no-workout {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.add-workout-icon {
  margin-bottom: 0.5rem;
}

.add-workout-text {
  font-size: 0.85rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.5);
  margin: 0;
}

@media (max-width: 960px) {
  .day-num {
    font-size: 1.4rem;
  }

  .workout-name {
    font-size: 0.8rem;
  }

  .workout-icon-container {
    width: 45px;
    height: 45px;
  }
}

@media (max-width: 600px) {
  .day-item {
    aspect-ratio: auto;
    min-height: 100px;
  }

  .day-card-inner {
    padding: 0.6rem;
  }

  .day-top {
    margin-bottom: 0.5rem;
  }

  .day-num {
    font-size: 1rem;
    font-weight: 800;
  }

  .today-chip {
    font-size: 0.65rem;
    padding: 2px 6px;
    min-height: 20px;
  }

  .workout-name {
    font-size: 0.75rem;
    margin-bottom: 0.5rem;
    -webkit-line-clamp: 3;
  }

  .workout-icon-container {
    width: 32px;
    height: 32px;
    margin-bottom: 0.4rem;
  }

  .workout-icon-container .v-icon {
    font-size: 18px !important;
  }

  .completed-badge {
    padding: 0.35rem 0.65rem;
    font-size: 0.65rem;
    border-radius: 12px;
  }

  .add-workout-icon .v-icon {
    font-size: 24px !important;
  }

  .add-workout-text {
    font-size: 0.65rem;
    font-weight: 700;
  }

  .click-hint {
    font-size: 0.6rem;
  }

  .pending-indicator {
    gap: 0.25rem;
  }

  .pending-indicator .v-chip {
    min-height: 20px;
    font-size: 0.6rem;
  }
}

@media (max-width: 360px) {
  .day-card-inner {
    padding: 0.5rem;
  }

  .day-num {
    font-size: 0.9rem;
  }

  .workout-icon-container {
    width: 28px;
    height: 28px;
  }

  .workout-icon-container .v-icon {
    font-size: 16px !important;
  }

  .workout-name {
    font-size: 0.7rem;
  }

  .completed-badge {
    font-size: 0.6rem;
    padding: 0.25rem 0.5rem;
  }
}
</style>