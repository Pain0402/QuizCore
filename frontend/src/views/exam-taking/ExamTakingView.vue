<template>
  <div class="exam-room">
    <!-- Loading -->
    <div v-if="loading" class="exam-loading">
      <div class="loading-dots"><span></span><span></span><span></span></div>
      <p>Đang tải đề thi...</p>
    </div>

    <template v-else-if="attempt">
      <!-- TOP BAR -->
      <div class="exam-topbar">
        <div class="exam-topbar-left">
          <div class="stat-icon" style="width:32px;height:32px;font-size:1rem;background:var(--primary-light)">📝</div>
          <span class="exam-topbar-title">{{ attempt.examTitle }}</span>
        </div>

        <!-- Timer -->
        <div class="exam-timer" :class="{ 'timer-warning': secondsLeft < 300, 'timer-danger': secondsLeft < 60 }">
          <span class="timer-icon">⏱️</span>
          <span class="timer-value">{{ formattedTime }}</span>
        </div>

        <div class="exam-topbar-right">
          <span style="font-size:0.85rem;color:var(--text-muted)">
            {{ answeredCount }}/{{ attempt.questions.length }} câu đã trả lời
          </span>
          <button class="btn btn-success" @click="confirmSubmit" :disabled="submitting">
            <span v-if="submitting" class="spinner"></span>
            {{ submitting ? 'Đang nộp...' : '✅ Nộp bài' }}
          </button>
        </div>
      </div>

      <!-- MAIN LAYOUT -->
      <div class="exam-body">
        <!-- Question panel -->
        <div class="exam-nav-panel">
          <div class="exam-nav-header">Câu hỏi</div>
          <div class="exam-nav-grid">
            <button
              v-for="(q, i) in attempt.questions" :key="q.questionId"
              class="exam-nav-btn"
              :class="{
                'nav-answered': answers[q.questionId]?.length > 0,
                'nav-current': currentIndex === i
              }"
              @click="currentIndex = i"
            >
              {{ i + 1 }}
            </button>
          </div>
          <div style="margin-top:20px;display:flex;flex-direction:column;gap:8px;font-size:0.78rem;color:var(--text-muted)">
            <div style="display:flex;align-items:center;gap:6px">
              <div style="width:12px;height:12px;border-radius:3px;background:var(--primary)"></div>
              <span>Câu hiện tại</span>
            </div>
            <div style="display:flex;align-items:center;gap:6px">
              <div style="width:12px;height:12px;border-radius:3px;background:var(--success)"></div>
              <span>Đã trả lời</span>
            </div>
            <div style="display:flex;align-items:center;gap:6px">
              <div style="width:12px;height:12px;border-radius:3px;background:var(--bg-elevated);border:1px solid var(--border-strong)"></div>
              <span>Chưa trả lời</span>
            </div>
          </div>
        </div>

        <!-- Question content -->
        <div class="exam-question-area">
          <div v-if="currentQuestion" class="exam-question-card">
            <div class="exam-q-header">
              <span class="badge badge-primary">Câu {{ currentIndex + 1 }}/{{ attempt.questions.length }}</span>
              <span class="badge badge-muted">{{ typeLabels[currentQuestion.questionType] }}</span>
              <span v-if="autosaveStatus" class="badge badge-success" style="margin-left:auto;font-size:0.72rem">
                ✓ Đã lưu
              </span>
            </div>

            <div class="exam-q-content">{{ currentQuestion.content }}</div>

            <div class="exam-answers">
              <label
                v-for="ans in currentQuestion.answers"
                :key="ans.answerId"
                class="exam-answer-item"
                :class="{ 'answer-selected': isSelected(currentQuestion.questionId, ans.answerId) }"
              >
                <input
                  :type="currentQuestion.questionType === 'MultipleChoice' ? 'checkbox' : 'radio'"
                  :name="`q${currentQuestion.questionId}`"
                  :value="ans.answerId"
                  :checked="isSelected(currentQuestion.questionId, ans.answerId)"
                  @change="handleAnswer(currentQuestion.questionId, ans.answerId, currentQuestion.questionType)"
                  style="display:none"
                />
                <div class="answer-indicator">
                  <div class="answer-check"></div>
                </div>
                <span class="answer-text">{{ ans.content }}</span>
              </label>
            </div>

            <!-- Navigation -->
            <div class="exam-q-nav">
              <button class="btn btn-secondary" :disabled="currentIndex === 0" @click="currentIndex--">
                ← Câu trước
              </button>
              <button v-if="currentIndex < attempt.questions.length - 1"
                class="btn btn-primary" @click="currentIndex++">
                Câu tiếp →
              </button>
              <button v-else class="btn btn-success" @click="confirmSubmit">
                Nộp bài ✅
              </button>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Submit confirm modal -->
    <Transition name="fade">
      <div v-if="showSubmitModal" class="modal-overlay">
        <div class="modal modal-sm">
          <div class="modal-header">
            <h3 class="modal-title">Xác nhận nộp bài</h3>
          </div>
          <div class="modal-body">
            <div style="text-align:center;margin-bottom:16px;font-size:2rem">📤</div>
            <p style="text-align:center;margin-bottom:12px">Bạn có chắc muốn nộp bài?</p>
            <div style="display:grid;grid-template-columns:1fr 1fr;gap:10px;padding:12px;background:var(--bg-elevated);border-radius:var(--radius);font-size:0.875rem">
              <div style="text-align:center">
                <div style="font-weight:700;font-size:1.25rem;color:var(--success)">{{ answeredCount }}</div>
                <div style="color:var(--text-muted)">Đã trả lời</div>
              </div>
              <div style="text-align:center">
                <div style="font-weight:700;font-size:1.25rem;color:var(--danger)">{{ attempt?.questions.length - answeredCount }}</div>
                <div style="color:var(--text-muted)">Chưa trả lời</div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showSubmitModal = false">Tiếp tục làm</button>
            <button class="btn btn-success" :disabled="submitting" @click="doSubmit">
              <span v-if="submitting" class="spinner"></span>
              Nộp bài ngay
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { attemptsApi } from '@/api/attempts'
import { useNotificationStore } from '@/stores/notification'

const route  = useRoute()
const router = useRouter()
const notify = useNotificationStore()

const attempt         = ref(null)
const loading         = ref(true)
const submitting      = ref(false)
const showSubmitModal = ref(false)
const currentIndex    = ref(0)
const answers         = ref({}) // { questionId: [answerId, ...] }
const secondsLeft     = ref(0)
const autosaveStatus  = ref(false)

let timerInterval  = null
let autosaveTimer  = null

const typeLabels = {
  SingleChoice: 'Một đáp án',
  MultipleChoice: 'Nhiều đáp án',
  TrueFalse: 'Đúng/Sai'
}

const currentQuestion = computed(() => attempt.value?.questions[currentIndex.value])

const answeredCount = computed(() =>
  Object.values(answers.value).filter(v => v?.length > 0).length
)

const formattedTime = computed(() => {
  const m = Math.floor(secondsLeft.value / 60).toString().padStart(2, '0')
  const s = (secondsLeft.value % 60).toString().padStart(2, '0')
  return `${m}:${s}`
})

function isSelected(questionId, answerId) {
  return (answers.value[questionId] || []).includes(answerId)
}

function handleAnswer(questionId, answerId, type) {
  if (type === 'MultipleChoice') {
    const curr = answers.value[questionId] || []
    if (curr.includes(answerId)) {
      answers.value[questionId] = curr.filter(id => id !== answerId)
    } else {
      answers.value[questionId] = [...curr, answerId]
    }
  } else {
    answers.value[questionId] = [answerId]
  }
  // Trigger autosave for current question immediately
  doAutoSave(questionId)
}

async function doAutoSave(questionId) {
  try {
    await attemptsApi.autoSave(route.params.attemptId, {
      questionId,
      selectedAnswerIds: answers.value[questionId] || []
    })
    autosaveStatus.value = true
    setTimeout(() => { autosaveStatus.value = false }, 2000)
  } catch { /* silent */ }
}

async function doAutoSaveAll() {
  // Save all answered questions every 15s
  const answerEntries = Object.entries(answers.value)
  for (const [qId, ids] of answerEntries) {
    if (ids?.length > 0) {
      try {
        await attemptsApi.autoSave(route.params.attemptId, {
          questionId: parseInt(qId),
          selectedAnswerIds: ids
        })
      } catch { /* silent */ }
    }
  }
}

function confirmSubmit() { showSubmitModal.value = true }

async function doSubmit() {
  submitting.value = true
  try {
    const { data } = await attemptsApi.submit(route.params.attemptId)
    clearTimers()
    notify.success('Nộp bài thành công!', 'Đang xem kết quả...')
    router.push({ name: 'ExamResult', params: { attemptId: route.params.attemptId } })
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể nộp bài')
  } finally {
    submitting.value = false
    showSubmitModal.value = false
  }
}

function clearTimers() {
  if (timerInterval) clearInterval(timerInterval)
  if (autosaveTimer) clearInterval(autosaveTimer)
}

async function loadAttempt() {
  loading.value = true
  try {
    const attemptId = route.params.attemptId
    // Try to restore progress first
    const { data: progressData } = await attemptsApi.getProgress(attemptId)
    const progress = progressData.data

    // Load exam data (re-use from start, stored in localStorage if needed)
    // For now we fetch attempt info via progress
    secondsLeft.value = progress.secondsRemaining

    // Restore saved answers
    if (progress.savedAnswers) {
      for (const [qId, ids] of Object.entries(progress.savedAnswers)) {
        answers.value[parseInt(qId)] = ids
      }
    }

    // We need the questions - start again to get them
    // Actually we store them from navigation state or re-fetch
    const stored = sessionStorage.getItem(`attempt_${attemptId}`)
    if (stored) {
      attempt.value = JSON.parse(stored)
      // Recalculate timer
      const elapsed = attempt.value.duration - progress.secondsRemaining
      secondsLeft.value = Math.max(0, progress.secondsRemaining)
    }
  } catch (err) {
    notify.error('Lỗi', 'Không thể khôi phục bài thi')
  } finally {
    loading.value = false
    startTimers()
  }
}

function startTimers() {
  // Main countdown timer
  timerInterval = setInterval(() => {
    if (secondsLeft.value > 0) {
      secondsLeft.value--
    } else {
      clearTimers()
      doSubmit()
    }
  }, 1000)

  // Autosave every 15s
  autosaveTimer = setInterval(doAutoSaveAll, 15000)
}

// Handle page unload
function handleBeforeUnload(e) {
  e.preventDefault()
  e.returnValue = ''
}

onMounted(async () => {
  // Get attempt data passed via router state
  const state = history.state
  if (state?.attempt) {
    attempt.value = state.attempt
    secondsLeft.value = Math.floor(
      (new Date(state.attempt.expiresAt) - Date.now()) / 1000
    )
    sessionStorage.setItem(`attempt_${route.params.attemptId}`, JSON.stringify(state.attempt))
    loading.value = false
    startTimers()
  } else {
    await loadAttempt()
  }
  window.addEventListener('beforeunload', handleBeforeUnload)
})

onUnmounted(() => {
  clearTimers()
  window.removeEventListener('beforeunload', handleBeforeUnload)
})
</script>

<style scoped>
.exam-room {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: var(--bg-base);
}

.exam-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  gap: 16px;
  color: var(--text-muted);
}

.exam-topbar {
  position: fixed;
  top: 0;
  left: var(--sidebar-width);
  right: 0;
  height: 60px;
  background: var(--bg-surface);
  border-bottom: 1px solid var(--border);
  display: flex;
  align-items: center;
  padding: 0 24px;
  gap: 16px;
  z-index: 40;
}

.exam-topbar-left {
  display: flex;
  align-items: center;
  gap: 10px;
  flex: 1;
  min-width: 0;
}

.exam-topbar-title {
  font-weight: 600;
  font-size: 0.95rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.exam-timer {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 16px;
  border-radius: var(--radius-sm);
  background: var(--bg-elevated);
  border: 1px solid var(--border-strong);
  font-weight: 700;
  font-family: var(--font-mono);
  font-size: 1.1rem;
  transition: all 0.3s;
}

.timer-warning {
  background: var(--warning-light);
  border-color: rgba(245,158,11,0.4);
  color: var(--warning);
}

.timer-danger {
  background: var(--danger-light);
  border-color: rgba(239,68,68,0.4);
  color: var(--danger);
  animation: pulse 1s infinite;
}

.exam-topbar-right {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.exam-body {
  display: flex;
  margin-top: 60px;
  min-height: calc(100vh - 60px);
}

.exam-nav-panel {
  width: 200px;
  flex-shrink: 0;
  background: var(--bg-surface);
  border-right: 1px solid var(--border);
  padding: 20px 14px;
  position: sticky;
  top: 60px;
  height: calc(100vh - 60px);
  overflow-y: auto;
}

.exam-nav-header {
  font-size: 0.78rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--text-muted);
  margin-bottom: 12px;
}

.exam-nav-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 6px;
}

.exam-nav-btn {
  aspect-ratio: 1;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-strong);
  background: var(--bg-elevated);
  color: var(--text-secondary);
  font-size: 0.8rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}

.exam-nav-btn:hover { background: var(--bg-hover); }
.exam-nav-btn.nav-answered { background: var(--success-light); border-color: rgba(34,197,94,0.4); color: var(--success); }
.exam-nav-btn.nav-current  { background: var(--primary) !important; border-color: var(--primary) !important; color: white !important; }

.exam-question-area {
  flex: 1;
  padding: 28px 32px;
  overflow-y: auto;
}

.exam-question-card {
  max-width: 800px;
  margin: 0 auto;
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: var(--radius-xl);
  padding: 32px;
}

.exam-q-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 20px;
}

.exam-q-content {
  font-size: 1.1rem;
  font-weight: 500;
  color: var(--text-primary);
  line-height: 1.7;
  margin-bottom: 24px;
  padding-bottom: 24px;
  border-bottom: 1px solid var(--border);
}

.exam-answers {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 28px;
}

.exam-answer-item {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 16px;
  border-radius: var(--radius);
  border: 1px solid var(--border-strong);
  background: var(--bg-elevated);
  cursor: pointer;
  transition: all 0.15s;
  user-select: none;
}

.exam-answer-item:hover {
  border-color: var(--primary-border);
  background: var(--primary-light);
}

.exam-answer-item.answer-selected {
  border-color: var(--primary);
  background: var(--primary-light);
}

.answer-indicator {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid var(--border-strong);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: all 0.15s;
}

.answer-selected .answer-indicator {
  background: var(--primary);
  border-color: var(--primary);
}

.answer-selected .answer-indicator::after {
  content: '';
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: white;
}

.answer-text {
  font-size: 0.95rem;
  color: var(--text-primary);
  line-height: 1.5;
}

.exam-q-nav {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 20px;
  border-top: 1px solid var(--border);
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}
</style>
