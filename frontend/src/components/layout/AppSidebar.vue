<template>
  <aside class="sidebar">
    <!-- Logo -->
    <div class="sidebar-logo">
      <div class="sidebar-logo-icon">🎯</div>
      <span class="sidebar-logo-text">QuizCore</span>
      <span class="sidebar-logo-badge">v1.0</span>
    </div>

    <!-- Navigation -->
    <nav class="sidebar-nav">
      <span class="sidebar-section">Tổng quan</span>

      <RouterLink to="/dashboard" class="sidebar-link" :class="{ active: isActive('/dashboard') }">
        <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
        </svg>
        Dashboard
      </RouterLink>

      <template v-if="authStore.canManage">
        <span class="sidebar-section" style="margin-top:20px">Quản lý nội dung</span>

        <RouterLink to="/questions" class="sidebar-link" :class="{ active: isActive('/questions') }">
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round"
              d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          Câu hỏi
        </RouterLink>

        <RouterLink to="/exams" class="sidebar-link" :class="{ active: isActive('/exams') }">
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round"
              d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          Đề thi
        </RouterLink>
      </template>
    </nav>

    <!-- User footer -->
    <div class="sidebar-footer">
      <div class="sidebar-user" @click="handleLogout">
        <div class="sidebar-avatar">{{ userInitials }}</div>
        <div class="sidebar-user-info">
          <div class="sidebar-user-name">{{ authStore.user?.fullName || authStore.user?.username }}</div>
          <div class="sidebar-user-role">{{ authStore.user?.role }}</div>
        </div>
        <svg style="width:16px;height:16px;color:var(--text-muted);flex-shrink:0"
          fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
        </svg>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const notify = useNotificationStore()

const isActive = (path) => route.path.startsWith(path)

const userInitials = computed(() => {
  const name = authStore.user?.fullName || authStore.user?.username || '?'
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})

function handleLogout() {
  authStore.logout()
  notify.info('Đã đăng xuất', 'Hẹn gặp lại!')
  router.push('/login')
}
</script>
