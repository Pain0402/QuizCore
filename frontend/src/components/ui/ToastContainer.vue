<template>
  <teleport to="body">
    <div class="toast-container">
      <div
        v-for="toast in notify.toasts"
        :key="toast.id"
        class="toast"
        :class="toast.exiting ? 'exiting' : ''"
      >
        <span class="toast-icon">{{ icons[toast.type] }}</span>
        <div class="toast-body">
          <div v-if="toast.title" class="toast-title">{{ toast.title }}</div>
          <div v-if="toast.message" class="toast-message">{{ toast.message }}</div>
        </div>
        <button class="toast-close" @click="notify.remove(toast.id)">×</button>
      </div>
    </div>
  </teleport>
</template>

<script setup>
import { useNotificationStore } from '@/stores/notification'

const notify = useNotificationStore()
const icons = {
  success: '✅',
  danger: '❌',
  warning: '⚠️',
  info: 'ℹ️',
}
</script>
