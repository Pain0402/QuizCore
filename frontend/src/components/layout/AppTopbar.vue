<template>
  <header class="topbar">
    <div class="topbar-breadcrumb">
      <span>{{ title }}</span>
    </div>
    <div class="topbar-actions">
      <button class="btn btn-ghost btn-icon" title="Thông báo">
        <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
        </svg>
      </button>
      <div class="sidebar-avatar" style="cursor:default;width:32px;height:32px;font-size:0.78rem">
        {{ userInitials }}
      </div>
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

defineProps({ title: { type: String, default: '' } })

const authStore = useAuthStore()
const userInitials = computed(() => {
  const name = authStore.user?.fullName || authStore.user?.username || '?'
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})
</script>
