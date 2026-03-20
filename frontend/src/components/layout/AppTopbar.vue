<template>
  <header class="topbar">
    <div class="topbar-breadcrumb">
      <span>{{ title }}</span>
    </div>
    <div class="topbar-actions">
      <button class="btn btn-ghost btn-icon" title="Thông báo" style="border:1px solid var(--border);border-radius:var(--radius-sm)">
        <Bell :size="18" stroke-width="2.5" />
      </button>
      <div class="sidebar-avatar" style="cursor:default;width:34px;height:34px;font-size:0.8rem">
        {{ userInitials }}
      </div>
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { Bell } from 'lucide-vue-next'

defineProps({ title: { type: String, default: '' } })

const authStore = useAuthStore()
const userInitials = computed(() => {
  const name = authStore.user?.fullName || authStore.user?.username || '?'
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})
</script>
