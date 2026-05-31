<template>
    <v-dialog 
        :model-value="modelValue" 
        @update:model-value="$emit('update:modelValue', $event)" 
        max-width="600" 
        persistent
    >
        <v-card v-if="routine" class="routine-detail-card">
            <div class="routine-header" :class="routine.iscompleted ? 'completed' : 'pending'">
                <div class="header-content">
                    <v-icon large class="header-icon">
                        {{ routine.iscompleted ? 'mdi-check-circle' : 'mdi-dumbbell' }}
                    </v-icon>
                    <div class="header-text">
                        <h2 class="routine-title">{{ routine.name }}</h2>
                        <v-chip small :color="routine.iscompleted ? 'success' : 'warning'" dark class="status-chip">
                            <v-icon small left>
                                {{ routine.iscompleted ? 'mdi-check' : 'mdi-clock-outline' }}
                            </v-icon>
                            {{ routine.iscompleted ? t('common.completed') : t('common.pending') }}
                        </v-chip>
                    </div>
                </div>
                <v-btn icon dark @click="$emit('update:modelValue', false)">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </div>

            <v-card-text class="routine-content">
                <div class="section">
                    <div class="section-label">
                        <v-icon color="purple" class="mr-2">mdi-text</v-icon>
                        {{ t('common.description') }}
                    </div>
                    <p class="section-text">{{ routine.description }}</p>
                </div>

                <v-row class="info-cards">
                    <v-col cols="12" sm="4">
                        <div class="info-card">
                            <v-icon :color="getDifficultyColor(routine.difficulty)" large>
                                mdi-gauge
                            </v-icon>
                            <div class="info-label">{{ t('common.difficulty') }}</div>
                            <div class="info-value" :style="{ color: getDifficultyColor(routine.difficulty) }">
                                {{ getDifficultyText(routine.difficulty) }}
                            </div>
                        </div>
                    </v-col>

                    <v-col cols="12" sm="4">
                        <div class="info-card">
                            <v-icon color="amber" large>mdi-star</v-icon>
                            <div class="info-label">{{ t('common.experience') }}</div>
                            <div class="info-value" style="color: #FFA726">+{{ routine.reward }} XP</div>
                        </div>
                    </v-col>

                    <v-col cols="12" sm="4">
                        <div class="info-card">
                            <v-icon color="yellow darken-2" large>mdi-coin</v-icon>
                            <div class="info-label">{{ t('common.coins') }}</div>
                            <div class="info-value" style="color: #F9A825">+50</div>
                        </div>
                    </v-col>
                </v-row>

                <div class="section">
                    <div class="section-label">
                        <v-icon color="purple" class="mr-2">mdi-calendar</v-icon>
                        {{ t('routineDetail.scheduledDate') }}
                    </div>
                    <p class="section-text">{{ routine.createdat ? formatDate(routine.createdat.toString()) : '-' }}</p>
                </div>

                <v-alert v-if="routine.iscompleted" type="success" colored-border elevation="2" class="mt-4">
                    <div class="completed-message">
                        <v-icon large color="success" class="mr-3">mdi-trophy</v-icon>
                        <div>
                            <div class="font-weight-bold">{{ t('routineDetail.greatJob') }}</div>
                            <div class="text-caption">{{ t('routineDetail.completedSuccess') }}</div>
                        </div>
                    </div>
                </v-alert>
            </v-card-text>

            <v-card-actions class="routine-actions">
                <v-spacer></v-spacer>
                <v-btn text large @click="$emit('update:modelValue', false)">
                    {{ t('common.close') }}
                </v-btn>
                <v-btn 
                    v-if="!routine.iscompleted" 
                    color="success" 
                    large 
                    elevation="2" 
                    class="complete-btn"
                    @click="showConfirmDialog = true"
                >
                    <v-icon left>mdi-check-circle</v-icon>
                    {{ t('routineDetail.markCompleted') }}
                </v-btn>
            </v-card-actions>
        </v-card>

        <!-- Diálogo de confirmación -->
        <v-dialog v-model="showConfirmDialog" max-width="400" persistent>
            <v-card class="confirm-dialog">
                <v-card-title class="text-h5 confirm-title">
                    <v-icon large color="amber" class="mr-2">mdi-alert-circle</v-icon>
                    {{ t('common.areYouSure') }}
                </v-card-title>
                <v-card-text class="confirm-text">
                    <p>{{ t('routineDetail.confirmCompleteText') }}</p>
                    <v-alert type="warning" dense outlined class="mt-3" color="amber">
                        <strong>{{ t('routineDetail.actionCannotBeUndone') }}</strong>
                    </v-alert>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn text @click="showConfirmDialog = false" class="cancel-btn">
                        {{ t('common.cancel') }}
                    </v-btn>
                    <v-btn color="success" @click="confirmComplete" class="confirm-btn">
                        <v-icon left>mdi-check</v-icon>
                        {{ t('routineDetail.yesComplete') }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Routines } from '@/components/Models/Routines'

const props = defineProps<{
    modelValue: boolean
    routine?: Routines | null
}>()

const emit = defineEmits<{
    'update:modelValue': [value: boolean]
    'complete': [id: number]
}>()

const { t, locale } = useI18n()
const showConfirmDialog = ref(false)

function confirmComplete() {
    if (props.routine) {
        emit('complete', props.routine.id)
        showConfirmDialog.value = false
    }
}

function getDifficultyText(difficulty: number): string {
    const map: Record<number, string> = {
        1: t('difficulty.easy'),
        2: t('difficulty.medium'),
        3: t('difficulty.hard'),
        4: t('difficulty.extreme')
    }
    return map[difficulty] || t('difficulty.medium')
}

function getDifficultyColor(difficulty: number): string {
    const colors: Record<number, string> = {
        1: '#4CAF50',
        2: '#FFA726',
        3: '#EF5350',
        4: '#7E57C2'
    }
    return colors[difficulty] || '#757575'
}

function formatDate(dateString: string): string {
    if (!dateString) return ''
    const date = new Date(dateString)
    const options: Intl.DateTimeFormatOptions = {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }
    return date.toLocaleDateString(locale.value, options)
}
</script>

<style scoped>
.routine-detail-card {
    overflow: hidden;
    border-radius: 16px;
    background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
    border: 2px solid rgba(139, 92, 246, 0.3);
}

.routine-header {
    padding: 24px;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    color: white;
    position: relative;
    overflow: hidden;
}

.routine-header::before {
    content: '';
    position: absolute;
    inset: 0;
    background: radial-gradient(circle at 30% 50%, rgba(167, 139, 250, 0.15) 0%, transparent 60%);
    opacity: 0.5;
}

.routine-header.completed {
    background: linear-gradient(135deg, rgba(52, 211, 153, 0.3) 0%, rgba(16, 185, 129, 0.2) 100%);
}

.routine-header.pending {
    background: linear-gradient(135deg, rgba(167, 139, 250, 0.3) 0%, rgba(34, 211, 238, 0.2) 100%);
}

.header-content {
    display: flex;
    gap: 16px;
    align-items: center;
    position: relative;
    z-index: 1;
}

.header-icon {
    background: rgba(255, 255, 255, 0.2);
    padding: 12px;
    border-radius: 12px;
    backdrop-filter: blur(10px);
}

.header-text {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.routine-title {
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0;
    text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.status-chip {
    width: fit-content;
    font-weight: 600;
}

.routine-content {
    padding: 24px !important;
}

.section {
    margin-bottom: 24px;
}

.section-label {
    display: flex;
    align-items: center;
    font-weight: 700;
    font-size: 0.95rem;
    color: rgba(255, 255, 255, 0.8);
    margin-bottom: 8px;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.section-text {
    font-size: 1.1rem;
    color: rgba(255, 255, 255, 0.7);
    line-height: 1.6;
    margin: 0;
}

.info-cards {
    margin: 24px 0;
}

.info-card {
    background: linear-gradient(135deg, rgba(26, 10, 46, 0.6) 0%, rgba(15, 10, 26, 0.8) 100%);
    border: 2px solid rgba(139, 92, 246, 0.3);
    padding: 20px;
    border-radius: 12px;
    text-align: center;
    transition: all 0.3s ease;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 8px;
}

.info-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(139, 92, 246, 0.3);
    border-color: rgba(139, 92, 246, 0.5);
}

.info-label {
    font-size: 0.85rem;
    color: rgba(255, 255, 255, 0.6);
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.info-value {
    font-size: 1.5rem;
    font-weight: 700;
    color: #ffffff;
}

.completed-message {
    display: flex;
    align-items: center;
}

.routine-actions {
    padding: 16px 24px;
    background: rgba(15, 10, 26, 0.8);
    border-top: 1px solid rgba(139, 92, 246, 0.3);
}

.complete-btn {
    font-weight: 700;
    text-transform: none;
    letter-spacing: 0.5px;
    padding: 0 32px !important;
}

.confirm-dialog {
    background: linear-gradient(135deg, rgba(26, 10, 46, 0.95) 0%, rgba(15, 10, 26, 0.98) 100%);
    border: 2px solid rgba(139, 92, 246, 0.3);
    border-radius: 16px;
}

.confirm-title {
    background: linear-gradient(135deg, rgba(251, 191, 36, 0.3) 0%, rgba(245, 158, 11, 0.2) 100%);
    color: white;
    padding: 20px 24px;
    border-bottom: 1px solid rgba(251, 191, 36, 0.3);
}

.confirm-text {
    padding: 24px;
    font-size: 1.1rem;
    color: rgba(255, 255, 255, 0.8);
}

.confirm-text p {
    color: rgba(255, 255, 255, 0.8);
}

.cancel-btn {
    color: rgba(255, 255, 255, 0.8);
}

.confirm-btn {
    font-weight: 700;
}

@media (max-width: 960px) {
    .routine-header {
        padding: 20px;
    }

    .routine-title {
        font-size: 1.5rem;
    }

    .header-icon {
        padding: 10px;
    }
}

@media (max-width: 600px) {
    .routine-header {
        padding: 16px;
    }

    .header-content {
        flex-direction: column;
        align-items: flex-start;
    }

    .routine-title {
        font-size: 1.25rem;
    }

    .info-value {
        font-size: 1.25rem;
    }

    .routine-content {
        padding: 16px !important;
    }

    .routine-actions {
        padding: 12px 16px;
        flex-direction: column;
        gap: 8px;
    }

    .complete-btn {
        width: 100%;
    }
}
</style>