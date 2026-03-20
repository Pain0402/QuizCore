<template>
  <aside class="sidebar">
    <!-- Logo -->
    <div class="sidebar-logo">
      <div class="sidebar-logo-icon" style="background:var(--primary);color:var(--bg-base)"><Target :size="20" stroke-width="2.5" /></div>
      <span class="sidebar-logo-text">QuizCore</span>
      <span class="sidebar-logo-badge">v1.0</span>
    </div>

    <!-- Navigation -->
    <nav class="sidebar-nav">
      <span class="sidebar-section">Tổng quan</span>

      <RouterLink to="/dashboard" class="sidebar-link" :class="{ active: isActive('/dashboard') }">
        <LayoutDashboard class="nav-icon" />
        Dashboard
      </RouterLink>

      <template v-if="authStore.canManage">
        <span class="sidebar-section" style="margin-top:20px">Quản lý nội dung</span>

        <RouterLink to="/questions" class="sidebar-link" :class="{ active: isActive('/questions') }">
          <FileQuestion class="nav-icon" />
          Câu hỏi
        </RouterLink>

        <RouterLink to="/exams" class="sidebar-link" :class="{ active: isActive('/exams') }">
          <BookOpen class="nav-icon" />
          Đề thi
        </RouterLink>
      </template>

      <!-- Admin section -->
      <template v-if="authStore.user?.role === 'Admin'">
        <span class="sidebar-section" style="margin-top:20px">Quản trị hệ thống</span>

        <RouterLink to="/admin/users" class="sidebar-link" :class="{ active: isActive('/admin/users') }">
          <Users class="nav-icon" />
          Người dùng
        </RouterLink>

        <RouterLink to="/admin/classes" class="sidebar-link" :class="{ active: isActive('/admin/classes') }">
          <GraduationCap class="nav-icon" />
          Lớp học
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
        <LogOut style="width:16px;height:16px;color:var(--text-muted);flex-shrink:0" />
      </div>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'
import { Target, LayoutDashboard, FileQuestion, BookOpen, Users, GraduationCap, LogOut } from 'lucide-vue-next'

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
