<template>
    <div class="msg-row" :class="message.role">
        <div v-if="message.role === 'assistant'" class="msg-avatar">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none">
                <defs>
                    <linearGradient id="mg" x1="0%" y1="0%" x2="100%" y2="100%">
                        <stop offset="0%" stop-color="#6366f1" />
                        <stop offset="100%" stop-color="#8b5cf6" />
                    </linearGradient>
                </defs>
                <path d="M12 2L13.5 8.5L20 7L15.5 12L20 17L13.5 15.5L12 22L10.5 15.5L4 17L8.5 12L4 7L10.5 8.5L12 2Z"
                    fill="url(#mg)" />
            </svg>
        </div>

        <div class="msg-body">
            <div class="msg-bubble" :class="message.role">
                <div v-if="message.isTyping" class="typing-indicator">
                    <span /><span /><span />
                </div>
                <span v-else class="msg-text" v-html="formattedText" />
            </div>
            <span class="msg-time">{{ message.time }}</span>
        </div>
    </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
    message: { type: Object, required: true }
})

const formattedText = computed(() => {
    if (!props.message.text) return ''
    return props.message.text
        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
        .replace(/\*(.*?)\*/g, '<em>$1</em>')
        .replace(/\n/g, '<br/>')
        .replace(/• /g, '<span class="bullet">•</span> ')
})
</script>

<style scoped>
.msg-row {
    display: flex;
    gap: 10px;
    animation: fadeUp 0.3s ease forwards;
    align-items: flex-end;
}

.msg-row.user {
    flex-direction: row-reverse;
}

.msg-avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: linear-gradient(135deg, #ede9fe, #ddd6fe);
    border: 1.5px solid #c4b5fd;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    margin-bottom: 18px;
    box-shadow: 0 2px 8px rgba(99, 102, 241, 0.15);
}

.msg-body {
    display: flex;
    flex-direction: column;
    gap: 4px;
    max-width: 75%;
}

.msg-row.user .msg-body {
    align-items: flex-end;
}

.msg-bubble {
    padding: 13px 17px;
    border-radius: 18px;
    line-height: 1.65;
    font-size: 14.5px;
    word-break: break-word;
}

/* ── Burbuja del asistente: blanco con borde suave ── */
.msg-bubble.assistant {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-bottom-left-radius: 4px;
    color: #1e293b;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06), 0 1px 3px rgba(0, 0, 0, 0.04);
}

/* ── Burbuja del usuario: degradado púrpura/índigo ── */
.msg-bubble.user {
    background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
    color: #ffffff;
    border-bottom-right-radius: 4px;
    font-weight: 500;
    box-shadow: 0 4px 20px rgba(99, 102, 241, 0.35), 0 1px 4px rgba(99, 102, 241, 0.2);
}

.msg-time {
    font-size: 11px;
    color: #94a3b8;
    padding: 0 4px;
}

/* ── Indicador de escritura ── */
.typing-indicator {
    display: flex;
    gap: 5px;
    align-items: center;
    padding: 2px 4px;
    height: 20px;
}

.typing-indicator span {
    width: 7px;
    height: 7px;
    border-radius: 50%;
    background: #6366f1;
    animation: blink 1.2s ease-in-out infinite;
    box-shadow: 0 0 6px rgba(99, 102, 241, 0.5);
}

.typing-indicator span:nth-child(2) { animation-delay: 0.2s; }
.typing-indicator span:nth-child(3) { animation-delay: 0.4s; }

@keyframes blink {
    0%, 80%, 100% { opacity: 0.2; transform: scale(0.85); }
    40%            { opacity: 1;   transform: scale(1);    }
}

@keyframes fadeUp {
    from { opacity: 0; transform: translateY(8px); }
    to   { opacity: 1; transform: translateY(0);   }
}

/* ── Formato de texto enriquecido ── */
:deep(.bullet) {
    color: #6366f1;
    font-weight: 700;
}

:deep(strong) {
    color: #4f46e5;
    font-weight: 600;
}

:deep(em) {
    color: #7c3aed;
    font-style: italic;
}
</style>