<template>
  <div>
    <div v-if="loading" class="card" style="padding:60px;text-align:center">
      <div class="loading-dots" style="justify-content:center"><span></span><span></span><span></span></div>
    </div>

    <div v-else-if="result" class="fade-in">
      <!-- Result Hero -->
      <div class="result-hero">
        <div class="result-icon">{{ result.isPassed ? '🏆' : '😔' }}</div>
        <h1 class="result-title">{{ result.isPassed ? 'Chúc mừng! Bạn đã đạt!' : 'Chưa đạt, hãy cố gắng hơn!' }}</h1>
        <p class="result-subtitle">{{ result.examTitle }}</p>

        <div class="result-score-circle" :class="result.isPassed ? 'score-pass' : 'score-fail'">
          <div class="score-value">{{ result.score }}</div>
          <div class="score-total">/ {{ result.totalMark }}</div>
        </div>

        <div class="result-stats">
          <div class="result-stat">
            <div class="result-stat-value">{{ result.correctAnswers }}</div>
            <div class="result-stat-label">Câu đúng</div>
          </div>
          <div class="result-stat-divider"></div>
          <div class="result-stat">
            <div class="result-stat-value">{{ result.totalQuestions - result.correctAnswers }}</div>
            <div class="result-stat-label">Câu sai</div>
          </div>
          <div class="result-stat-divider"></div>
          <div class="result-stat">
            <div class="result-stat-value">{{ percentage }}%</div>
            <div class="result-stat-label">Tỷ lệ đúng</div>
          </div>
          <div class="result-stat-divider"></div>
          <div class="result-stat">
            <div class="result-stat-value">{{ durationText }}</div>
            <div class="result-stat-label">Thời gian làm</div>
          </div>
        </div>

        <div style="display:flex;gap:12px;margin-top:8px">
          <RouterLink to="/exams" class="btn btn-secondary">← Danh sách đề thi</RouterLink>
          <button class="btn btn-primary" @click="showReview = !showReview">
            <ClipboardList :size="18" style="margin-right:6px" /> {{ showReview ? 'Ẩn' : 'Xem' }} chi tiết đáp án
          </button>
        </div>
      </div>

      <!-- Review section -->
      <Transition name="slide-up">
        <div v-if="showReview" style="margin-top:24px;display:flex;flex-direction:column;gap:16px">
          <h2 style="font-size:1.1rem;font-weight:700;margin-bottom:4px">Chi tiết từng câu hỏi</h2>

          <div v-for="(q, i) in result.review" :key="q.questionId"
            class="card"
            :class="q.isCorrect ? 'review-correct' : 'review-wrong'"
          >
            <div style="display:flex;align-items:flex-start;gap:12px;margin-bottom:14px">
              <span class="badge" :class="q.isCorrect ? 'badge-success' : 'badge-danger'" style="padding-left:8px">
                <Check v-if="q.isCorrect" :size="14" stroke-width="3" />
                <X v-else :size="14" stroke-width="3" />
                Câu {{ i + 1 }}
              </span>
              <span style="font-weight:500;color:var(--text-primary);flex:1">{{ q.content }}</span>
            </div>

            <div style="display:flex;flex-direction:column;gap:8px">
              <div
                v-for="ans in q.answers"
                :key="ans.answerId"
                class="review-answer"
                :class="{
                  'review-ans-correct':  ans.isCorrect,
                  'review-ans-selected': ans.wasSelected && !ans.isCorrect,
                }"
              >
                <span class="review-ans-icon" style="display:flex;align-items:center;justify-content:center">
                  <CheckCircle2 v-if="ans.isCorrect" :size="18" color="var(--success)" />
                  <XCircle v-else-if="ans.wasSelected" :size="18" color="var(--danger)" />
                </span>
                <span>{{ ans.content }}</span>
                <span v-if="ans.wasSelected && !ans.isCorrect"
                  style="margin-left:auto;font-size:0.75rem;color:var(--danger)">Bạn chọn</span>
                <span v-if="ans.isCorrect && !ans.wasSelected"
                  style="margin-left:auto;font-size:0.75rem;color:var(--success)">Đáp án đúng</span>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { attemptsApi } from '@/api/attempts'
import { useNotificationStore } from '@/stores/notification'
import { Check, X, CheckCircle2, XCircle, ClipboardList } from 'lucide-vue-next'

const route  = useRoute()
const notify = useNotificationStore()

const result     = ref(null)
const loading    = ref(true)
const showReview = ref(false)

const percentage = computed(() => {
  if (!result.value) return 0
  return Math.round(result.value.correctAnswers / result.value.totalQuestions * 100)
})

const durationText = computed(() => {
  if (!result.value?.startedAt || !result.value?.submittedAt) return '—'
  const secs = Math.floor(
    (new Date(result.value.submittedAt) - new Date(result.value.startedAt)) / 1000
  )
  const m = Math.floor(secs / 60)
  const s = secs % 60
  return `${m}:${s.toString().padStart(2,'0')}`
})

async function loadResult() {
  loading.value = true
  try {
    const { data } = await attemptsApi.getResult(route.params.attemptId)
    result.value = data.data
  } catch (err) {
    notify.error('Lỗi', 'Không thể tải kết quả thi')
  } finally {
    loading.value = false
  }
}

onMounted(loadResult)
</script>

<style scoped>
.result-hero {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: var(--radius-xl);
  padding: 40px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.result-icon { font-size: 3rem; }
.result-title { font-size: 1.5rem; font-weight: 700; }
.result-subtitle { color: var(--text-secondary); margin: 0; }

.result-score-circle {
  width: 140px;
  height: 140px;
  border-radius: 50%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border: 6px solid;
  margin: 8px 0;
}

.score-pass { border-color: var(--success); background: var(--success-light); }
.score-fail { border-color: var(--danger);  background: var(--danger-light);  }

.score-value { font-size: 2rem; font-weight: 800; }
.score-total { font-size: 0.9rem; color: var(--text-muted); }

.result-stats {
  display: flex;
  align-items: center;
  gap: 0;
  background: var(--bg-elevated);
  border-radius: var(--radius-lg);
  border: 1px solid var(--border);
  overflow: hidden;
}
.result-stat {
  padding: 14px 24px;
  text-align: center;
}
.result-stat-value { font-size: 1.25rem; font-weight: 700; color: var(--text-primary); }
.result-stat-label { font-size: 0.75rem; color: var(--text-muted); margin-top: 2px; }
.result-stat-divider { width: 1px; align-self: stretch; background: var(--border); }

.review-correct { border-left: 3px solid var(--success); }
.review-wrong   { border-left: 3px solid var(--danger); }

.review-answer {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: var(--radius-sm);
  border: 1px solid transparent;
  font-size: 0.875rem;
  color: var(--text-secondary);
}

.review-ans-correct  { background: var(--success-light); border-color: rgba(34,197,94,0.3); color: var(--text-primary); }
.review-ans-selected { background: var(--danger-light);  border-color: rgba(239,68,68,0.3);  color: var(--text-primary); }
.review-ans-icon { width: 20px; flex-shrink: 0; }
</style>
