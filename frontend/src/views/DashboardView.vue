<template>
  <div>
    <!-- Page Header -->
    <div class="page-header">
      <div>
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">Tổng quan hệ thống QuizCore</p>
      </div>
      <div style="display:flex;gap:10px;align-items:center">
        <span class="badge badge-success">● Hệ thống hoạt động</span>
      </div>
    </div>

    <!-- Stats -->
    <div class="stats-grid" style="margin-bottom:28px">
      <div v-for="stat in stats" :key="stat.label" class="stat-card">
        <div class="stat-icon" :style="{ background: stat.bg }">{{ stat.icon }}</div>
        <div class="stat-body">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
          <div class="stat-change up">{{ stat.change }}</div>
        </div>
      </div>
    </div>

    <!-- Two column grid -->
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px">
      <!-- Quick Actions -->
      <div class="card">
        <div class="card-header">
          <h3 class="card-title">Thao tác nhanh</h3>
        </div>
        <div style="display:flex;flex-direction:column;gap:10px">
          <RouterLink
            v-for="action in quickActions"
            :key="action.label"
            :to="action.to"
            style="display:flex;align-items:center;gap:14px;padding:14px;border-radius:var(--radius);border:1px solid var(--border);text-decoration:none;transition:all var(--transition)"
            @mouseenter="e => e.currentTarget.style.borderColor = 'var(--border-strong)'"
            @mouseleave="e => e.currentTarget.style.borderColor = 'var(--border)'"
          >
            <div class="stat-icon" style="width:40px;height:40px;font-size:1.1rem" :style="{ background: action.bg }">
              {{ action.icon }}
            </div>
            <div>
              <div style="font-weight:600;font-size:0.9rem;color:var(--text-primary)">{{ action.label }}</div>
              <div style="font-size:0.8rem;color:var(--text-muted)">{{ action.desc }}</div>
            </div>
            <svg style="margin-left:auto;width:16px;height:16px;color:var(--text-muted)" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
            </svg>
          </RouterLink>
        </div>
      </div>

      <!-- System Info -->
      <div class="card">
        <div class="card-header">
          <h3 class="card-title">Thông tin hệ thống</h3>
        </div>
        <div style="display:flex;flex-direction:column;gap:14px">
          <div v-for="info in systemInfo" :key="info.label"
            style="display:flex;align-items:center;justify-content:space-between;padding-bottom:14px;border-bottom:1px solid var(--border)"
          >
            <span style="font-size:0.875rem;color:var(--text-secondary)">{{ info.label }}</span>
            <span :class="info.badgeClass" class="badge">{{ info.value }}</span>
          </div>
        </div>

        <div style="margin-top:20px;padding:16px;background:var(--bg-elevated);border-radius:var(--radius);border:1px solid var(--border)">
          <div style="font-size:0.78rem;color:var(--text-muted);margin-bottom:8px">Đăng nhập với tư cách</div>
          <div style="display:flex;align-items:center;gap:10px">
            <div class="sidebar-avatar" style="cursor:default">{{ userInitials }}</div>
            <div>
              <div style="font-weight:600;font-size:0.9rem">{{ authStore.user?.fullName }}</div>
              <div style="font-size:0.8rem;color:var(--text-muted)">{{ authStore.user?.email }}</div>
            </div>
            <span class="badge badge-primary" style="margin-left:auto">{{ authStore.user?.role }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const userInitials = computed(() => {
  const name = authStore.user?.fullName || '?'
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})

const stats = [
  { icon: '❓', label: 'Câu hỏi', value: '—', change: 'Ngân hàng câu hỏi', bg: 'var(--primary-light)' },
  { icon: '📝', label: 'Đề thi',   value: '—', change: 'Đề thi hiện có',    bg: 'rgba(34,197,94,0.12)' },
  { icon: '🏫', label: 'Lớp học',  value: '—', change: 'Lớp đang hoạt động', bg: 'rgba(245,158,11,0.12)' },
  { icon: '👥', label: 'Học sinh', value: '—', change: 'Người dùng hệ thống', bg: 'rgba(59,130,246,0.12)' },
]

const quickActions = [
  { icon: '❓', label: 'Ngân hàng câu hỏi', desc: 'Thêm và quản lý câu hỏi', to: '/questions', bg: 'var(--primary-light)' },
  { icon: '📝', label: 'Quản lý đề thi',    desc: 'Tạo và cấu hình đề thi',   to: '/exams',    bg: 'rgba(34,197,94,0.12)' },
]

const systemInfo = [
  { label: 'Backend API',      value: 'Online',     badgeClass: 'badge-success' },
  { label: 'Database',         value: 'PostgreSQL',  badgeClass: 'badge-info' },
  { label: 'Cache',            value: 'Redis',       badgeClass: 'badge-warning' },
  { label: 'Phiên bản',        value: 'Phase 4',     badgeClass: 'badge-primary' },
]
</script>
