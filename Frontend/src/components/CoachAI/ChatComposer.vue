<template>
  <div class="composer">

    <!-- Quick prompts (only shown before first message) -->
    <div class="quick-row" v-if="showQuickPrompts">
      <button
        class="quick"
        v-for="q in quickPrompts"
        :key="q.text"
        @click="$emit('send', q.text)"
      >
        <span class="quick-icon">{{ q.icon }}</span>
        {{ q.text }}
      </button>
    </div>

    <!-- Input row -->
    <div class="input-row">
      <textarea
        ref="textareaRef"
        v-model="draft"
        class="input-box"
        :placeholder="disabled ? t('chat.placeholderAnswering') : t('chat.placeholderAsk')"
        :disabled="disabled"
        rows="1"
        @keydown.enter.exact.prevent="submit"
        @input="autoResize"
      />
      <button
        class="send-btn"
        :disabled="!draft.trim() || disabled"
        @click="submit"
        :aria-label="t('chat.sendAria')"
      >
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
          <line x1="22" y1="2" x2="11" y2="13"/>
          <polygon points="22 2 15 22 11 13 2 9 22 2"/>
        </svg>
      </button>
    </div>

    <p class="hint">{{ t('chat.hint') }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { QuickPrompt } from '@/components/Models/Chat'

const { t } = useI18n()

/* ── Props ── */
interface Props {
  disabled?: boolean
  showQuickPrompts?: boolean
}

// Usamos withDefaults para mantener los valores por defecto
withDefaults(defineProps<Props>(), {
  disabled: false,
  showQuickPrompts: false,
})


// Tipamos exactamente qué eventos se emiten y qué datos envían
const emit = defineEmits<{
  (e: 'send', text: string): void
}>()


const draft = ref<string>('')
// Tipamos la referencia del DOM específicamente como un HTMLTextAreaElement
const textareaRef = ref<HTMLTextAreaElement | null>(null)

// Mensakes rápidos predefinidos para que el usuario pueda hacer clic y enviar sin escribir (solo se muestran antes del primer mensaje)
const quickPrompts = computed<QuickPrompt[]>(() => [
  { icon: '💪', text: t('chat.quickRoutine3Days') },
  { icon: '🥗', text: t('chat.quickDietMuscle')  },
  { icon: '🏃', text: t('chat.quickCardioPlan')     },
  { icon: '😴', text: t('chat.quickRecoveryTips')   },
])

// Función para autoajustar la altura del textarea según el contenido, con un máximo de 140px para evitar que crezca demasiado
function autoResize(): void {
  const el = textareaRef.value
  if (!el) return
  
  el.style.height = 'auto'
  el.style.height = `${Math.min(el.scrollHeight, 140)}px`
}
// Función para enviar el mensaje. Emite el evento 'send' con el texto, limpia el draft y resetea la altura del textarea
function submit(): void {
  const text = draft.value.trim()
  if (!text) return
  
  emit('send', text)
  draft.value = ''
  
  if (textareaRef.value) {
    textareaRef.value.style.height = 'auto'
  }
}
</script>
<style scoped>
.composer {
  padding: 14px 18px 16px;
  border-top: 1px solid var(--c-border);
  background: var(--c-surface);
  flex-shrink: 0;
}

/* Quick prompts */
.quick-row {
  display: flex;
  gap: 7px;
  margin-bottom: 10px;
  overflow-x: auto;
  padding-bottom: 2px;
}

.quick-row::-webkit-scrollbar { height: 0; }

.quick {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 7px 13px;
  border-radius: 99px;
  border: 1.5px solid var(--c-border);
  background: var(--c-bg);
  color: var(--c-muted);
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  white-space: nowrap;
  transition: all .15s;
  font-family: inherit;
  flex-shrink: 0;
}

.quick:hover {
  border-color: var(--c-accent);
  color: var(--c-accent);
  background: #fff1eb;
}

.quick-icon { font-size: 13px; }

/* Input row */
.input-row {
  display: flex;
  align-items: flex-end; /* Lo mantenemos así para que el botón se quede abajo si el texto crece */
  gap: 10px;
  background: var(--c-bg);
  border: 1.5px solid var(--c-border);
  border-radius: var(--r-lg);

  padding: 10px 10px 10px 16px; 
  transition: border-color .2s;
  min-height: 58px; /* Opcional: Establece una altura mínima para la caja para mayor consistencia */
}

.input-row:focus-within {
  border-color: var(--c-accent);
  box-shadow: 0 0 0 3px rgba(232, 93, 38, .1);
}

.input-box {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  font-family: inherit;
  font-size: 14px;
  color: var(--c-text);
  resize: none;
  max-height: 140px;
  line-height: 1.5;
  /* LA CLAVE ESTÁ AQUÍ: 
     Añadimos un pequeño padding superior e inferior. 
     Esto "empuja" el texto de una sola línea hacia el centro visual, 
     alineándolo perfectamente con el botón de 36px de altura. */
  padding: 8px 0; 
  margin: 0;
}

.input-box::placeholder { color: var(--c-faint); }
.input-box:disabled     { opacity: .6; cursor: not-allowed; }

/* Send button */
.send-btn {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: var(--c-accent);
  border: none;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  flex-shrink: 0;
  transition: all .15s;
  box-shadow: 0 3px 10px rgba(232, 93, 38, .35);
}

.send-btn:hover:not(:disabled) {
  background: #c9481a;
  transform: scale(1.05);
}

.send-btn:disabled {
  background: var(--c-border);
  color: var(--c-faint);
  box-shadow: none;
  cursor: not-allowed;
  transform: none;
}

.hint {
  text-align: center;
  font-size: 11px;
  color: var(--c-faint);
  margin-top: 8px;
}
</style>