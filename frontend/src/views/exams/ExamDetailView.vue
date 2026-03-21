<template>
  <div>
    <div v-if="loading" class="card" style="padding:60px;text-align:center">
      <div class="loading-dots" style="justify-content:center"><span></span><span></span><span></span></div>
    </div>

    <div v-else-if="exam" class="fade-in">
      <!-- Header -->
      <div style="margin-bottom:24px">
        <RouterLink to="/exams" style="font-size:0.85rem;color:var(--text-muted);display:inline-flex;align-items:center;gap:4px;text-decoration:none">
          ← Quay lại danh sách
        </RouterLink>
      </div>

      <div style="display:grid;grid-template-columns:1fr 340px;gap:24px;align-items:start">
        <!-- Main info -->
        <div style="display:flex;flex-direction:column;gap:20px">
          <div class="card">
            <div style="display:flex;align-items:flex-start;gap:16px;margin-bottom:20px">
              <div class="stat-icon" style="width:52px;height:52px;font-size:1.6rem;background:var(--primary-light);flex-shrink:0">
                <FileQuestion :size="32" stroke-width="1.5" style="color:var(--primary)" />
              </div>
              <div>
                <h1 style="font-size:1.5rem;margin-bottom:6px">{{ exam.title }}</h1>
                <p>{{ exam.description || 'Không có mô tả' }}</p>
              </div>
            </div>

            <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(150px,1fr));gap:16px">
              <div v-for="info in examInfo" :key="info.label"
                style="padding:14px;background:var(--bg-elevated);border-radius:var(--radius);text-align:center;border:1px solid var(--border)">
                <div style="margin-bottom:8px;color:var(--primary);display:flex;justify-content:center"><component :is="info.icon" :size="24" stroke-width="2" /></div>
                <div style="font-weight:700;font-size:1.1rem;color:var(--text-primary)">{{ info.value }}</div>
                <div style="font-size:0.8rem;color:var(--text-muted)">{{ info.label }}</div>
              </div>
            </div>
          </div>

          <!-- Rules -->
          <div class="card">
            <h3 class="card-title" style="margin-bottom:16px"><ClipboardList :size="18" style="margin-right:6px;display:inline-block;vertical-align:middle" /> Quy định khi thi</h3>
            <ul style="display:flex;flex-direction:column;gap:10px;padding-left:4px;list-style:none">
              <li v-for="rule in rules" :key="rule" style="display:flex;align-items:flex-start;gap:10px;font-size:0.875rem;color:var(--text-secondary)">
                <AlertTriangle :size="16" style="color:var(--warning);margin-top:1px;flex-shrink:0" />
                {{ rule }}
              </li>
            </ul>
          </div>
        </div>

        <!-- Action card -->
        <div style="position:sticky;top:calc(var(--topbar-height) + 20px)">
          <div class="card" style="text-align:center">
            <div style="margin-bottom:12px;color:var(--primary);display:flex;justify-content:center"><Target :size="48" stroke-width="1.5" /></div>
            <h3 style="margin-bottom:4px">Sẵn sàng?</h3>
            <p style="font-size:0.875rem;margin-bottom:20px">Sau khi bắt đầu, đồng hồ sẽ chạy ngay lập tức</p>

            <div style="display:flex;flex-direction:column;gap:10px;margin-bottom:20px;padding:16px;background:var(--bg-elevated);border-radius:var(--radius);border:1px solid var(--border)">
              <div style="display:flex;justify-content:space-between;font-size:0.875rem">
                <span style="color:var(--text-muted)">Thời gian</span>
                <span style="font-weight:600">{{ exam.duration }} phút</span>
              </div>
              <div style="display:flex;justify-content:space-between;font-size:0.875rem">
                <span style="color:var(--text-muted)">Số lần thi còn lại</span>
                <span style="font-weight:600" :style="remainingAttempts > 0 ? 'color:var(--success)' : 'color:var(--danger)'">{{ remainingAttempts }} / {{ exam.maxAttempts }}</span>
              </div>
              <div style="display:flex;justify-content:space-between;font-size:0.875rem">
                <span style="color:var(--text-muted)">Điểm đạt</span>
                <span style="font-weight:600">{{ exam.passMark }}/{{ exam.totalMark }}</span>
              </div>
            </div>

            <button class="btn btn-primary" style="width:100%;padding:12px 24px;font-size:1rem"
              :disabled="starting" @click="handleStart">
              <span v-if="starting" class="spinner"></span>
              <span v-if="starting">Đang khởi tạo...</span>
              <template v-else>
                <PlayCircle :size="18" style="margin-right:6px;vertical-align:middle" /> 
                <span style="vertical-align:middle">Bắt đầu thi</span>
              </template>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ClipboardList, AlertTriangle, Target, PlayCircle, FileQuestion, Clock, RotateCcw, Trophy, CheckCircle2 } from 'lucide-vue-next'
import { ref, computed, onMounted, markRaw } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { examsApi } from '@/api/exams'
import { attemptsApi } from '@/api/attempts'
import { useNotificationStore } from '@/stores/notification'

const route  = useRoute()
const router = useRouter()
const notify = useNotificationStore()

const exam    = ref(null)
const loading = ref(false)
const starting = ref(false)
const remainingAttempts = ref(0)

const examInfo = computed(() => exam.value ? [
  { icon: markRaw(Clock), value: `${exam.value.duration} phút`, label: 'Thời gian thi' },
  { icon: markRaw(RotateCcw), value: exam.value.maxAttempts, label: 'Số lần thi' },
  { icon: markRaw(Trophy), value: `${exam.value.totalMark} điểm`, label: 'Tổng điểm' },
  { icon: markRaw(CheckCircle2), value: `${exam.value.passMark} điểm`, label: 'Điểm đạt' },
] : [])

const rules = [
  'Không đóng tab trình duyệt trong khi thi',
  'Bài làm sẽ tự động lưu mỗi 15 giây',
  'Sau khi hết giờ, hệ thống sẽ tự động nộp bài',
  'Mỗi câu hỏi chỉ có thể trả lời một lần',
]

async function loadExam() {
  loading.value = true
  try {
    const { data } = await examsApi.getById(route.params.id)
    exam.value = data.data || data
    remainingAttempts.value = data.remainingAttempts !== undefined ? data.remainingAttempts : (exam.value?.maxAttempts || 0)
  } catch {
    notify.error('Lỗi', 'Không thể tải thông tin đề thi')
  } finally {
    loading.value = false
  }
}

async function handleStart() {
  starting.value = true
  try {
    const { data } = await attemptsApi.startExam(route.params.id)
    const attempt = data.data
    router.push({
      name: 'ExamTaking',
      params: { attemptId: attempt.attemptId },
      state: { attempt }
    })
  } catch (err) {
    notify.error('Không thể bắt đầu', err.response?.data?.message || 'Đã xảy ra lỗi')
  } finally {
    starting.value = false
  }
}

onMounted(loadExam)
</script>
