<template>
  <div>
    <!-- Page Header -->
    <div class="page-header">
      <div>
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">
          <span v-if="authStore.isStudent">Xin chào, {{ authStore.user?.fullName }}! Đây là các kỳ thi của bạn.</span>
          <span v-else>Tổng quan hệ thống QuizCore</span>
        </p>
      </div>
      <div style="display:flex;gap:10px;align-items:center">
        <span class="badge badge-success">● Hệ thống hoạt động</span>
      </div>
    </div>

    <!-- ===== STUDENT VIEW ===== -->
    <template v-if="authStore.isStudent">
      <!-- Stats -->
      <div class="stats-grid" style="margin-bottom:28px">
        <div class="stat-card">
          <div class="stat-icon" style="background:var(--primary-light);color:var(--primary)"><BookOpen :size="24"/></div>
          <div class="stat-body">
            <div class="stat-value">{{ studentStats.availableExams }}</div>
            <div class="stat-label">Kỳ thi có thể vào</div>
            <div class="stat-change up">Đang mở</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:rgba(34,197,94,0.12);color:#22c55e"><CheckCircle :size="24"/></div>
          <div class="stat-body">
            <div class="stat-value">{{ studentStats.completedExams }}</div>
            <div class="stat-label">Kỳ thi đã hoàn thành</div>
            <div class="stat-change up">Lịch sử thi</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:rgba(245,158,11,0.12);color:#f59e0b"><Trophy :size="24"/></div>
          <div class="stat-body">
            <div class="stat-value">{{ studentStats.avgScore || '—' }}</div>
            <div class="stat-label">Điểm trung bình</div>
            <div class="stat-change up">Tất cả kỳ thi</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:rgba(59,130,246,0.12);color:#3b82f6"><BarChart :size="24"/></div>
          <div class="stat-body">
            <div class="stat-value">{{ studentStats.passRate || '—' }}</div>
            <div class="stat-label">Tỷ lệ đạt</div>
            <div class="stat-change up">Kỳ thi đã nộp</div>
          </div>
        </div>
      </div>

      <!-- Available Exams -->
      <div class="card">
        <div class="card-header" style="display:flex;align-items:center;justify-content:space-between">
          <h3 class="card-title"><div style="display:flex;align-items:center;gap:8px"><BookOpen :size="20"/> Kỳ thi có thể tham gia</div></h3>
          <RouterLink to="/exams" class="btn btn-secondary btn-sm">Xem tất cả</RouterLink>
        </div>

        <div v-if="loadingExams" style="padding:40px;text-align:center">
          <div class="loading-dots" style="justify-content:center"><span></span><span></span><span></span></div>
        </div>

        <div v-else-if="!availableExams.length" style="padding:32px;text-align:center;color:var(--text-muted)">
          <div style="color:var(--text-muted);display:flex;justify-content:center;margin-bottom:12px"><ClipboardList :size="48" :stroke-width="1.5"/></div>
          <p>Chưa có kỳ thi nào được giao cho bạn</p>
        </div>

        <div v-else style="display:grid;grid-template-columns:repeat(auto-fill,minmax(280px,1fr));gap:14px">
          <div v-for="exam in availableExams" :key="exam.id"
            class="card" style="margin:0;border:1px solid var(--border);padding:16px">
            <div style="display:flex;align-items:flex-start;justify-content:space-between;margin-bottom:10px">
              <span class="badge badge-primary"><div style="display:flex;align-items:center;gap:4px"><BookOpen :size="14"/> Đề thi</div></span>
              <span class="badge badge-muted"><div style="display:flex;align-items:center;gap:4px"><Clock :size="14"/> {{ exam.duration }} phút</div></span>
            </div>
            <h4 style="font-size:0.95rem;font-weight:600;color:var(--text-primary);margin-bottom:6px">{{ exam.title }}</h4>
            <p style="font-size:0.8rem;color:var(--text-muted);margin-bottom:14px">
              {{ exam.description || 'Không có mô tả' }}
            </p>
            <div style="display:flex;gap:8px;align-items:center;padding-top:12px;border-top:1px solid var(--border)">
              <span class="badge badge-info"><div style="display:flex;align-items:center;gap:4px"><RotateCcw :size="14"/> {{ exam.maxAttempts }} lượt</div></span>
              <RouterLink :to="`/exams/${exam.id}`" class="btn btn-primary btn-sm" style="margin-left:auto">
                Vào thi →
              </RouterLink>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- ===== TEACHER / ADMIN VIEW ===== -->
    <template v-else>
      <!-- Stats -->
      <div class="stats-grid" style="margin-bottom:28px">
        <div v-for="stat in adminStats" :key="stat.label" class="stat-card">
          <div class="stat-icon" :style="{ background: stat.bg, color: stat.color }"><component :is="stat.icon" :size="24" :stroke-width="2.5"/></div>
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
              <div class="stat-icon" style="width:40px;height:40px;font-size:1.1rem" :style="{ background: action.bg, color: action.color }">
                <component :is="action.icon" :size="20" :stroke-width="2.5"/>
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
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, markRaw } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { examsApi } from '@/api/exams'
import { BookOpen, ClipboardList, Clock, RotateCcw, CheckCircle, Trophy, BarChart, FileQuestion, Users, GraduationCap } from 'lucide-vue-next'

const authStore = useAuthStore()

const userInitials = computed(() => {
  const name = authStore.user?.fullName || '?'
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2)
})

// ---- Student data ----
const availableExams = ref([])
const loadingExams   = ref(false)
const studentStats   = ref({ availableExams: 0, completedExams: 0, avgScore: '—', passRate: '—' })

async function loadStudentData() {
  loadingExams.value = true
  try {
    const { data } = await examsApi.getAll()
    availableExams.value = (data.data || data || [])
    studentStats.value.availableExams = availableExams.value.length
  } catch { /* silent */ }
  finally { loadingExams.value = false }
}

onMounted(() => {
  if (authStore.isStudent) loadStudentData()
})

// ---- Admin/Teacher data ----
const adminStats = [
  { icon: markRaw(FileQuestion), label: 'Câu hỏi',  value: '—', change: 'Ngân hàng câu hỏi',    bg: 'var(--primary-light)', color: 'var(--primary)' },
  { icon: markRaw(BookOpen), label: 'Đề thi',   value: '—', change: 'Đề thi hiện có',        bg: 'rgba(34,197,94,0.12)', color: '#22c55e' },
  { icon: markRaw(GraduationCap), label: 'Lớp học',  value: '—', change: 'Lớp đang hoạt động',    bg: 'rgba(245,158,11,0.12)', color: '#f59e0b' },
  { icon: markRaw(Users), label: 'Học sinh', value: '—', change: 'Người dùng hệ thống',   bg: 'rgba(59,130,246,0.12)', color: '#3b82f6' },
]

const quickActions = [
  { icon: markRaw(FileQuestion), label: 'Ngân hàng câu hỏi', desc: 'Thêm và quản lý câu hỏi', to: '/questions', bg: 'var(--primary-light)', color: 'var(--primary)' },
  { icon: markRaw(BookOpen), label: 'Quản lý đề thi',    desc: 'Tạo và cấu hình đề thi',   to: '/exams',    bg: 'rgba(34,197,94,0.12)', color: '#22c55e' },
]

const systemInfo = [
  { label: 'Backend API', value: 'Online',     badgeClass: 'badge-success' },
  { label: 'Database',   value: 'PostgreSQL',  badgeClass: 'badge-info' },
  { label: 'Cache',      value: 'Redis',       badgeClass: 'badge-warning' },
  { label: 'Phiên bản',  value: 'Phase 6',     badgeClass: 'badge-primary' },
]
</script>
