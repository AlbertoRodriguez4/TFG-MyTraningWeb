<template>
  <div ref="feedEl" class="feed">

    <!-- Welcome screen (shown when no messages) -->
    <div v-if="messages.length === 0" class="welcome">
      <div class="welcome-orb">
        <svg width="36" height="36" viewBox="0 0 24 24" fill="none">
          <path d="M12 2L14 9H21L15.5 13.5L17.5 21L12 16.5L6.5 21L8.5 13.5L3 9H10L12 2Z" fill="currentColor"/>
        </svg>
      </div>
      <h2 class="welcome-h">{{ t('chat.welcomeTitle') }}</h2>
      <p class="welcome-p">
        {{ t('chat.welcomeDesc') }}
      </p>
      <div class="chip-row">
        <button
          class="chip"
          v-for="c in chips"
          :key="c"
          @click="$emit('send', c)"
        >
          {{ c }}
        </button>
      </div>
    </div>

    <!-- Message list -->
    <ChatBubble
      v-for="msg in messages"
      :key="msg.id"
      :message="msg"
    />

  </div>
</template>

<script setup>
import { ref, watch, nextTick, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import ChatBubble from './ChatBubble.vue'

const { t } = useI18n()

const props = defineProps({
  messages: { type: Array, required: true }
})

defineEmits(['send'])

const feedEl = ref(null)

const chips = computed(() => [
  t('chat.chipStrengthRoutine'),
  t('chat.chipNutritionPlan'),
  t('chat.chipRecovery'),
  t('chat.chipLoseWeight'),
])

// Auto-scroll to bottom whenever messages change
watch(
  () => props.messages.length,
  () => nextTick(() => {
    if (feedEl.value) feedEl.value.scrollTop = feedEl.value.scrollHeight
  })
)
</script>

<style scoped>
.feed {
  flex: 1;
  overflow-y: auto;
  padding: 22px 22px 12px;
  display: flex;
  flex-direction: column;
  gap: 14px;
  background: #faf8f5;
}

.feed::-webkit-scrollbar       { width: 4px; }
.feed::-webkit-scrollbar-track { background: transparent; }
.feed::-webkit-scrollbar-thumb { background: #e0d9d0; border-radius: 99px; }

/* Welcome */
.welcome {
  margin: auto;
  text-align: center;
  max-width: 400px;
  animation: fadeSlideUp .5s ease forwards;
  padding: 20px 0;
}

.welcome-orb {
  width: 80px;
  height: 80px;
  border-radius: 24px;
  background: linear-gradient(135deg, #fff1eb, #ffe0cc);
  border: 2px solid #fbd5bb;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--c-accent);
  margin: 0 auto 20px;
  box-shadow: 0 8px 28px rgba(232, 93, 38, .18);
}

.welcome-h {
  font-family: 'Syne', sans-serif;
  font-size: 30px;
  font-weight: 800;
  color: var(--c-text);
  letter-spacing: .3px;
  margin-bottom: 10px;
}

.welcome-p {
  font-size: 14px;
  color: var(--c-muted);
  line-height: 1.7;
  margin-bottom: 22px;
}

.chip-row {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: center;
}

.chip {
  padding: 7px 14px;
  border-radius: 99px;
  border: 1.5px solid var(--c-border);
  background: var(--c-surface);
  color: var(--c-text);
  font-size: 12.5px;
  font-weight: 500;
  cursor: pointer;
  transition: all .15s;
  font-family: inherit;
}

.chip:hover {
  border-color: var(--c-accent);
  color: var(--c-accent);
  background: #fff1eb;
}
</style>