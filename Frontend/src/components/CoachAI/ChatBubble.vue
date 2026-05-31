<template>
    <div class="bubble-row" :class="message.role">

        <!-- Bot avatar -->
        <div v-if="message.role === 'assistant'" class="bot-ava">
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none">
                <path d="M12 2L14 9H21L15.5 13.5L17.5 21L12 16.5L6.5 21L8.5 13.5L3 9H10L12 2Z" fill="currentColor" />
            </svg>
        </div>

        <!-- Content -->
        <div class="bubble-wrap">
            <div class="bubble" :class="message.role">
                <!-- Typing indicator -->
                <div v-if="message.isTyping" class="dots">
                    <span /><span /><span />
                </div>
                <!-- Text -->
                <span v-else v-html="formatted" />
            </div>
            <time class="ts">{{ message.time }}</time>
        </div>

    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ChatMessage } from '@/components/Models/Chat'

const props = defineProps<{
    message: ChatMessage
}>()
// Formatea el texto del mensaje para soportar negrita, cursiva y saltos de línea (mensajes personalizados que la IA recibe de la API)
const formatted = computed(() => {
    if (!props.message.text) return ''
    return props.message.text
        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
        .replace(/\*(.*?)\*/g, '<em>$1</em>')
        .replace(/\n/g, '<br/>')
        .replace(/• /g, '<span class="bul">•</span> ')
})
</script>

<style scoped>
.bubble-row {
    display: flex;
    gap: 9px;
    align-items: flex-end;
    animation: fadeSlideUp .25s ease forwards;
}

.bubble-row.user {
    flex-direction: row-reverse;
}

.bot-ava {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: linear-gradient(135deg, #fff1eb, #ffe0cc);
    border: 1.5px solid #fbd5bb;
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--c-accent);
    flex-shrink: 0;
    margin-bottom: 18px;
}

.bubble-wrap {
    display: flex;
    flex-direction: column;
    gap: 3px;
    max-width: 74%;
}

.bubble-row.user .bubble-wrap {
    align-items: flex-end;
}

.bubble {
    padding: 12px 16px;
    border-radius: 18px;
    font-size: 14px;
    line-height: 1.65;
    word-break: break-word;
}

.bubble.assistant {
    background: var(--c-surface);
    border: 1px solid var(--c-border);
    border-bottom-left-radius: 4px;
    color: var(--c-text);
    box-shadow: 0 1px 6px rgba(0, 0, 0, .05);
}

.bubble.user {
    background: var(--c-accent);
    color: #fff;
    border-bottom-right-radius: 4px;
    font-weight: 500;
    box-shadow: 0 4px 18px rgba(232, 93, 38, .3);
}

.ts {
    font-size: 10.5px;
    color: var(--c-faint);
    padding: 0 3px;
}

/* Typing dots */
.dots {
    display: flex;
    gap: 5px;
    align-items: center;
    height: 18px;
    padding: 0 2px;
}

.dots span {
    width: 7px;
    height: 7px;
    border-radius: 50%;
    background: var(--c-accent);
    animation: blink 1.2s ease-in-out infinite;
}

.dots span:nth-child(2) {
    animation-delay: .2s;
}

.dots span:nth-child(3) {
    animation-delay: .4s;
}

/* Rich text (v-html) */
:deep(.bul) {
    color: var(--c-accent);
    font-weight: 700;
}

:deep(strong) {
    color: #b84319;
    font-weight: 600;
}

:deep(em) {
    color: var(--c-muted);
    font-style: italic;
}
</style>