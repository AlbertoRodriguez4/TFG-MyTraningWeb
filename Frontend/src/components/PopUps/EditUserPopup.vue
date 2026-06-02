<script setup lang="ts">
import { useUserStore } from '@/stores/userStore';
import { computed, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n'
import type { User } from '../Models/User';
import { logger } from '@/utils/logger';

const store = useUserStore();
const loggedUser = computed(() => store.loggedUser);
const { t } = useI18n()

const props = defineProps<{
  user: User;
  visible: boolean;
}>();

const emit = defineEmits(['close', 'edit', 'delete']);

const errorMessage = ref('');

// Estado del snackbar para mostrar mensajes de éxito o error
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')
// Editar el usuario, estado reactivo 
const editedUser = reactive<User>({
  id: 0,
  name: '',
  passwordhash: '',
  email: '',
  level: 0,
  strength: 0,
  endurance: 0,
  consistencyStreak: 0,
  gold: 0,
  role: '',
  experience: 0,
  xpRequired: 0,
  xpRemaining: 0,
  equippedStrengthItemId: 0,
  equippedEnduranceItemId: 0,
  avatarUrl: ''
});

const availableRoles = ['userNormal', 'userStaff', 'userMaster'];
// Sincronizar el estado del usuario editado con el usuario pasado por props
watch(() => props.user, (newUser) => {
  if (newUser) {
    editedUser.id = newUser.id;
    editedUser.name = newUser.name;
    editedUser.email = newUser.email;
    editedUser.passwordhash = newUser.passwordhash;
    editedUser.level = newUser.level;
    editedUser.strength = newUser.strength;
    editedUser.endurance = newUser.endurance;
    editedUser.consistencyStreak = newUser.consistencyStreak;
    editedUser.gold = newUser.gold;
    editedUser.role = newUser.role;
    editedUser.experience = newUser.experience;
    editedUser.xpRequired = newUser.xpRequired;
    editedUser.xpRemaining = newUser.xpRemaining;
    editedUser.equippedStrengthItemId = newUser.equippedStrengthItemId;
    editedUser.equippedEnduranceItemId = newUser.equippedEnduranceItemId;
    editedUser.avatarUrl = newUser.avatarUrl;
  }
}, { immediate: true });

function showSnackbar(text: string, color: string = 'success') {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}
// Validación de email simple
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const handleEdit = async () => {
  errorMessage.value = '';

  if (!editedUser.name || editedUser.name.trim().length < 3) {
    errorMessage.value = t('name_min_3_chars');
    return;
  }

  if (!editedUser.email || !emailRegex.test(editedUser.email.trim())) {
    errorMessage.value = t('enter_valid_email');
    return;
  }
// Validar que el rol seleccionado sea válido
  if (
    editedUser.level < 0 ||
    editedUser.strength < 0 ||
    editedUser.endurance < 0 ||
    (editedUser.gold ?? 0) < 0
  ) {
    errorMessage.value = t('no_negative_values');
    return;
  }

  try {
    const updatedUser = {
      id: props.user.id,
      name: editedUser.name.trim(),
      passwordhash: editedUser.passwordhash,
      email: editedUser.email.trim(),
      level: editedUser.level,
      strength: editedUser.strength,
      endurance: editedUser.endurance,
      gold: editedUser.gold,
      consistencyStreak: editedUser.consistencyStreak,
      role: editedUser.role,
      experience: editedUser.experience,
      xpRequired: editedUser.xpRequired,
      xpRemaining: editedUser.xpRemaining,
      equippedStrengthItemId: editedUser.equippedStrengthItemId,
      equippedEnduranceItemId: editedUser.equippedEnduranceItemId,
      avatarUrl: editedUser.avatarUrl
    };

    await store.editUser(updatedUser.id, updatedUser);
    showSnackbar(t('user_edited_success'), 'success')
    emit('close');
  } catch (error: any) {
    const message = error?.message || error?.data?.message;
    errorMessage.value = message || t('user_edit_error');
    logger.error("Error al editar el usuario:", error);
  }
};

const handleDelete = async () => {
  const userId = props.user.id;

  const result = await store.DeleteUser(userId);

  if (result != null) {
    showSnackbar(t('user_deleted_success'), 'success')
    emit('close');
  } else {
    showSnackbar(t('user_delete_error'), 'error')
  }
};

const internalVisible = ref(props.visible);

watch(() => props.visible, (val) => {
  internalVisible.value = val;
});
watch(internalVisible, (val) => {
  if (!val) emit('close');
});
</script>

<template>
  <v-dialog v-model="internalVisible" max-width="700" class="user-dialog">
    <v-card class="user-card">
      <!-- Header personalizado -->
      <div class="user-header">
        <div class="header-content">
          <v-icon class="header-icon">mdi-account-circle</v-icon>
          <h2 class="header-title">{{ $t('edit_user_profile') }}</h2>
        </div>
        <v-btn icon class="close-button" @click="emit('close')" size="small">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <v-card-text class="user-content">
        <!-- Error message -->
        <v-alert v-if="errorMessage" type="error" variant="tonal" class="error-alert">
          {{ errorMessage }}
        </v-alert>

        <!-- Información básica -->
        <div class="section">
          <div class="section-header">
            <v-icon class="section-icon">mdi-account</v-icon>
            <h3 class="section-title">{{ $t('basic_info') }}</h3>
          </div>

          <v-row>
            <v-col cols="12" md="6">
              <label class="field-label">
                <v-icon size="small" class="label-icon">mdi-account</v-icon>
                {{ $t('name_label') }}
              </label>
              <v-text-field v-model="editedUser.name" variant="outlined" density="comfortable" hide-details
                class="custom-input" />
            </v-col>

            <v-col cols="12" md="6">
              <label class="field-label">
                <v-icon size="small" class="label-icon">mdi-email</v-icon>
                {{ $t('email') }}
              </label>
              <v-text-field v-model="editedUser.email" type="email" variant="outlined" density="comfortable"
                hide-details class="custom-input" />
            </v-col>

            <v-col cols="12" md="6">
              <label class="field-label">
                <v-icon size="small" class="label-icon">mdi-shield-account</v-icon>
                {{ $t('role') }}
              </label>
              <v-btn-toggle
                v-model="editedUser.role"
                mandatory
                density="comfortable"
                class="role-toggle"
              >
                <v-btn value="userNormal" class="role-btn" :class="{ active: editedUser.role === 'userNormal' }">
                  <v-icon size="small" class="mr-1">mdi-account</v-icon>
                  Normal
                </v-btn>
                <v-btn value="userStaff" class="role-btn" :class="{ active: editedUser.role === 'userStaff' }">
                  <v-icon size="small" class="mr-1">mdi-account-tie</v-icon>
                  Staff
                </v-btn>
                <v-btn value="userMaster" class="role-btn" :class="{ active: editedUser.role === 'userMaster' }">
                  <v-icon size="small" class="mr-1">mdi-crown</v-icon>
                  Master
                </v-btn>
              </v-btn-toggle>
            </v-col>

            <v-col cols="12">
              <label class="field-label">
                <v-icon size="small" class="label-icon">mdi-lock</v-icon>
                {{ $t('password') }}
              </label>
              <v-text-field v-model="editedUser.passwordhash" type="password" variant="outlined" density="comfortable"
                hide-details placeholder="••••••••" class="custom-input" />
            </v-col>
          </v-row>
        </div>

        <!-- Estadísticas del jugador -->
        <div class="section">
          <div class="section-header">
            <v-icon class="section-icon">mdi-trophy</v-icon>
            <h3 class="section-title">{{ $t('player_stats') }}</h3>
          </div>

          <v-row>
            <v-col cols="6" sm="3">
              <div class="stat-card level-card">
                <div class="stat-header">
                  <v-icon class="stat-icon">mdi-trending-up</v-icon>
                  <span class="stat-label">{{ $t('level_label') }}</span>
                </div>
                <input v-model.number="editedUser.level" type="number" min="0" class="stat-input" />
              </div>
            </v-col>

            <v-col cols="6" sm="3">
              <div class="stat-card strength-card">
                <div class="stat-header">
                  <v-icon class="stat-icon">mdi-lightning-bolt</v-icon>
                  <span class="stat-label">{{ $t('fuerza') }}</span>
                </div>
                <input v-model.number="editedUser.strength" type="number" min="0" class="stat-input" />
              </div>
            </v-col>

            <v-col cols="6" sm="3">
              <div class="stat-card endurance-card">
                <div class="stat-header">
                  <v-icon class="stat-icon">mdi-heart-pulse</v-icon>
                  <span class="stat-label">{{ $t('resistencia') }}</span>
                </div>
                <input v-model.number="editedUser.endurance" type="number" min="0" class="stat-input" />
              </div>
            </v-col>

            <v-col cols="6" sm="3">
              <div class="stat-card gold-card">
                <div class="stat-header">
                  <v-icon class="stat-icon">mdi-gold</v-icon>
                  <span class="stat-label">{{ $t('oro') }}</span>
                </div>
                <input v-model.number="editedUser.gold" type="number" min="0" class="stat-input" />
              </div>
            </v-col>
          </v-row>
        </div>

        <!-- Racha de consistencia -->
        <div class="section">
          <div class="streak-card">
            <div class="streak-header">
              <v-icon class="streak-icon">mdi-fire</v-icon>
              <div class="streak-info">
                <span class="streak-label">{{ $t('consistency_streak_label') }}</span>
                <span class="streak-description">{{ $t('consecutive_training_days') }}</span>
              </div>
            </div>
            <input v-model.number="editedUser.consistencyStreak" type="number" min="0" class="streak-input" />
          </div>
        </div>

        <!-- Resumen del usuario -->
        <div class="summary-card">
          <div class="summary-header">
            <v-icon size="small">mdi-information</v-icon>
            <span>{{ $t('profile_summary') }}</span>
          </div>
          <div class="summary-content">
            <div class="summary-item">
              <v-icon size="small" class="summary-item-icon">mdi-account</v-icon>
              <span class="summary-item-label">{{ $t('user_label_summary') }}</span>
              <span class="summary-item-value">{{ editedUser.name || $t('no_name') }}</span>
            </div>
            <div class="summary-item">
              <v-icon size="small" class="summary-item-icon">mdi-shield-account</v-icon>
              <span class="summary-item-label">{{ $t('role') }}:</span>
              <span class="summary-item-value">{{ editedUser.role }}</span>
            </div>
            <div class="summary-item">
              <v-icon size="small" class="summary-item-icon">mdi-trending-up</v-icon>
              <span class="summary-item-label">{{ $t('level_label') }}:</span>
              <span class="summary-item-value">{{ editedUser.level }}</span>
            </div>
            <div class="summary-item">
              <v-icon size="small" class="summary-item-icon">mdi-fire</v-icon>
              <span class="summary-item-label">{{ $t('streak_label_summary') }}</span>
              <span class="summary-item-value">{{ editedUser.consistencyStreak }} {{ $t('days_count') }}</span>
            </div>
          </div>
        </div>
      </v-card-text>

      <!-- Footer con acciones -->
      <v-card-actions class="user-actions">
        <v-btn color="error" variant="outlined" @click="handleDelete" class="action-btn delete-btn">
          <v-icon left>mdi-delete</v-icon>
          {{ $t('delete_label') }}
        </v-btn>
        <v-spacer />
        <v-btn color="grey" variant="outlined" @click="emit('close')" class="action-btn cancel-btn">
          {{ $t('cancelar') }}
        </v-btn>
        <v-btn color="primary" variant="elevated" @click="handleEdit" class="action-btn save-btn">
          <v-icon left>mdi-content-save</v-icon>
          {{ $t('save_changes') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Snackbar -->
  <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="3000" location="top" rounded="pill">
    <div class="d-flex align-center">
      <v-icon class="mr-2">
        {{ snackbarColor === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle' }}
      </v-icon>
      {{ snackbarText }}
    </div>
  </v-snackbar>
</template>

<style scoped>
/* Dialog y Card base */
.user-card {
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  border-radius: 16px !important;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5) !important;
}

/* Header */
.user-header {
  background: linear-gradient(135deg, #a855f7 0%, #ec4899 100%);
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
}

.user-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.2);
  pointer-events: none;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
  z-index: 1;
}

.header-icon {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 8px;
  border-radius: 8px;
  color: white !important;
}

.header-title {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  color: white;
  letter-spacing: -0.5px;
}

.close-button {
  background: rgba(255, 255, 255, 0.2) !important;
  backdrop-filter: blur(10px);
  color: white !important;
  position: relative;
  z-index: 1;
}

.close-button:hover {
  background: rgba(255, 255, 255, 0.3) !important;
}

/* Content */
.user-content {
  padding: 24px !important;
  max-height: 70vh;
  overflow-y: auto;
}

.error-alert {
  margin-bottom: 20px;
  border-radius: 12px !important;
}

/* Sections */
.section {
  margin-bottom: 32px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 2px solid rgba(168, 85, 247, 0.3);
}

.section-icon {
  color: #a855f7 !important;
  font-size: 24px !important;
}

.section-title {
  margin: 0;
  color: #e9d5ff;
  font-size: 16px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Form fields */
.field-label {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #cbd5e1;
  font-size: 13px;
  font-weight: 600;
  margin-bottom: 8px;
}

.label-icon {
  color: #a855f7 !important;
}

.custom-input {
  margin-bottom: 0;
}

.custom-input :deep(.v-field) {
  border-radius: 10px;
  background: rgba(30, 41, 59, 0.5);
}

.custom-input :deep(.v-field__outline) {
  color: rgba(148, 163, 184, 0.3);
}

.custom-input :deep(.v-field--focused .v-field__outline) {
  color: #a855f7;
}

.custom-input :deep(input) {
  color: white;
}

/* Stat cards */
.stat-card {
  border-radius: 12px;
  padding: 16px;
  height: 100%;
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
}

.level-card {
  background: linear-gradient(135deg, rgba(251, 191, 36, 0.15) 0%, rgba(245, 158, 11, 0.05) 100%);
  border: 1px solid rgba(251, 191, 36, 0.3);
}

.strength-card {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.15) 0%, rgba(220, 38, 38, 0.05) 100%);
  border: 1px solid rgba(239, 68, 68, 0.3);
}

.endurance-card {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15) 0%, rgba(22, 163, 74, 0.05) 100%);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.gold-card {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.15) 0%, rgba(217, 119, 6, 0.05) 100%);
  border: 1px solid rgba(245, 158, 11, 0.3);
}

.stat-header {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 10px;
}

.stat-icon {
  font-size: 18px !important;
}

.level-card .stat-icon {
  color: #fbbf24 !important;
}

.strength-card .stat-icon {
  color: #ef4444 !important;
}

.endurance-card .stat-icon {
  color: #22c55e !important;
}

.gold-card .stat-icon {
  color: #f59e0b !important;
}

.stat-label {
  color: #cbd5e1;
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-input {
  width: 100%;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 10px;
  color: white;
  font-size: 28px;
  font-weight: 700;
  text-align: center;
  outline: none;
  transition: all 0.2s ease;
}

.stat-input:focus {
  border-color: rgba(255, 255, 255, 0.3);
  box-shadow: 0 0 0 3px rgba(168, 85, 247, 0.1);
}

/* Streak card */
.streak-card {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.15) 0%, rgba(236, 72, 153, 0.05) 100%);
  border: 1px solid rgba(168, 85, 247, 0.3);
  border-radius: 12px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 20px;
}

.streak-header {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.streak-icon {
  background: linear-gradient(135deg, #f59e0b 0%, #dc2626 100%);
  color: white !important;
  border-radius: 10px;
  padding: 12px;
  font-size: 32px !important;
}

.streak-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.streak-label {
  color: #e9d5ff;
  font-size: 15px;
  font-weight: 700;
}

.streak-description {
  color: #94a3b8;
  font-size: 12px;
}

.streak-input {
  width: 120px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(168, 85, 247, 0.3);
  border-radius: 10px;
  padding: 14px;
  color: white;
  font-size: 32px;
  font-weight: 700;
  text-align: center;
  outline: none;
  transition: all 0.2s ease;
}

.streak-input:focus {
  border-color: #a855f7;
  box-shadow: 0 0 0 3px rgba(168, 85, 247, 0.1);
}

/* Summary card */
.summary-card {
  background: linear-gradient(135deg, rgba(30, 41, 59, 0.8) 0%, rgba(30, 41, 59, 0.4) 100%);
  border: 1px solid rgba(148, 163, 184, 0.2);
  border-radius: 12px;
  padding: 16px;
  margin-top: 24px;
}

.summary-header {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #64748b;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  margin-bottom: 12px;
}

.summary-content {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.summary-item {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #cbd5e1;
  font-size: 14px;
}

.summary-item-icon {
  color: #a855f7 !important;
}

.summary-item-label {
  color: #94a3b8;
  min-width: 60px;
}

.summary-item-value {
  color: white;
  font-weight: 600;
}

.role-toggle {
  background: rgba(30, 41, 59, 0.5) !important;
  border: 1px solid rgba(148, 163, 184, 0.2) !important;
  border-radius: 10px !important;
  width: 100%;
}

.role-toggle :deep(.v-btn) {
  flex: 1;
  color: #94a3b8 !important;
  text-transform: none;
  font-weight: 600;
  font-size: 0.85rem;
  letter-spacing: 0.3px;
}

.role-toggle :deep(.v-btn--active) {
  background: linear-gradient(135deg, #a855f7 0%, #ec4899 100%) !important;
  color: white !important;
  box-shadow: 0 4px 12px rgba(168, 85, 247, 0.3) !important;
}

/* Actions */
.user-actions {
  background: rgba(15, 23, 42, 0.5);
  border-top: 1px solid rgba(148, 163, 184, 0.1);
  padding: 16px 24px !important;
}

.action-btn {
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.3px;
  border-radius: 10px !important;
  padding: 0 24px !important;
  height: 44px !important;
}

.delete-btn {
  border-color: #dc2626 !important;
  color: #dc2626 !important;
}

.delete-btn:hover {
  background: rgba(220, 38, 38, 0.1) !important;
}

.cancel-btn {
  border-color: #64748b !important;
  color: #94a3b8 !important;
}

.cancel-btn:hover {
  background: rgba(100, 116, 139, 0.1) !important;
}

.save-btn {
  background: linear-gradient(135deg, #a855f7 0%, #ec4899 100%) !important;
  box-shadow: 0 4px 12px rgba(168, 85, 247, 0.3) !important;
}

.save-btn:hover {
  box-shadow: 0 6px 20px rgba(168, 85, 247, 0.4) !important;
  transform: translateY(-1px);
}

/* Scrollbar */
.user-content::-webkit-scrollbar {
  width: 8px;
}

.user-content::-webkit-scrollbar-track {
  background: rgba(30, 41, 59, 0.3);
  border-radius: 4px;
}

.user-content::-webkit-scrollbar-thumb {
  background: rgba(148, 163, 184, 0.3);
  border-radius: 4px;
}

.user-content::-webkit-scrollbar-thumb:hover {
  background: rgba(148, 163, 184, 0.5);
}

/* Responsive adjustments */
@media (max-width: 600px) {
  .streak-card {
    flex-direction: column;
    align-items: stretch;
  }

  .streak-input {
    width: 100%;
  }

  .stat-input {
    font-size: 24px;
  }
}

/* Animaciones */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

.user-card {
  animation: fadeIn 0.3s ease-out;
}
</style>