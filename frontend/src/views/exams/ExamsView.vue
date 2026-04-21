<template>
  <div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Quản lý đề thi</h1>
        <p class="page-subtitle">Tạo và cấu hình các đề thi trong hệ thống</p>
      </div>
      <button class="btn btn-primary" @click="openCreate">
        <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
        </svg>
        Tạo đề thi
      </button>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="search-box">
        <span class="search-icon"><Search :size="16" /></span>
        <input v-model="search" class="form-control" placeholder="Tìm kiếm đề thi..." />
      </div>
      <button class="btn btn-secondary" @click="fetchExams">
        <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
        </svg>
        Làm mới
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="card" style="padding:48px;text-align:center">
      <div class="loading-dots" style="justify-content:center"><span></span><span></span><span></span></div>
      <p style="margin-top:12px;font-size:0.875rem">Đang tải...</p>
    </div>

    <!-- Grid of exam cards -->
    <div v-else-if="filteredExams.length" style="display:grid;grid-template-columns:repeat(auto-fill,minmax(300px,1fr));gap:16px">
      <div v-for="exam in filteredExams" :key="exam.id" class="card card-hover">
        <div style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:14px">
          <span class="badge badge-primary">📝 Đề thi</span>
          <div style="display:flex;gap:6px">
            <button class="btn btn-icon" style="background:#fee2e2; color:#dc2626; border-color:#fca5a5" title="Dừng thi / Thu bài khẩn cấp" @click="confirmForceSubmit(exam)"><ShieldAlert :size="16" stroke-width="2.5" /></button>
            <button class="btn btn-ghost btn-icon btn-icon" title="Chỉnh sửa" @click="openEdit(exam)"><Pencil :size="16" stroke-width="2.5" /></button>
            <button class="btn btn-danger btn-icon btn-icon" title="Xóa" @click="confirmDelete(exam)"><Trash :size="16" stroke-width="2.5" /></button>
          </div>
        </div>
        <h3 style="font-size:1rem;margin-bottom:8px;color:var(--text-primary)">{{ exam.title }}</h3>
        <p style="font-size:0.85rem;margin-bottom:16px;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden">
          {{ exam.description || 'Không có mô tả' }}
        </p>
        <div style="display:flex;gap:8px;flex-wrap:wrap;margin-top:16px;padding-top:14px;border-top:1px solid var(--border);align-items:center">
          <span class="badge badge-muted">⏱ {{ exam.duration }} phút</span>
          <span class="badge badge-muted">🔁 {{ exam.maxAttempts }} lượt</span>
          <span class="badge badge-info">📊 {{ exam.totalMark || 100 }} điểm</span>
          <RouterLink :to="`/exams/${exam.id}`" class="btn btn-primary btn-sm" style="margin-left:auto">Vào thi →</RouterLink>
        </div>
      </div>
    </div>

    <!-- Empty -->
    <div v-else class="card">
      <div class="empty-state">
        <div class="empty-state-icon">📝</div>
        <div class="empty-state-title">Chưa có đề thi nào</div>
        <div class="empty-state-desc">Tạo đề thi đầu tiên để bắt đầu tổ chức thi</div>
        <button class="btn btn-primary" style="margin-top:16px" @click="openCreate">Tạo đề thi</button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
        <div class="modal">
          <div class="modal-header">
            <h3 class="modal-title">{{ editingId ? 'Chỉnh sửa đề thi' : 'Tạo đề thi mới' }}</h3>
            <button class="modal-close" @click="closeModal">×</button>
          </div>
          <div class="modal-body">
            <div style="display:flex;flex-direction:column;gap:18px">
              <div class="form-group">
                <label class="form-label">Tiêu đề đề thi <span class="form-required">*</span></label>
                <input v-model="form.title" class="form-control" placeholder="Nhập tiêu đề đề thi..." />
              </div>
              <div class="form-group">
                <label class="form-label">Mô tả</label>
                <textarea v-model="form.description" class="form-control" placeholder="Mô tả ngắn về đề thi..." rows="3" />
              </div>
              <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
                <div class="form-group">
                  <label class="form-label">Thời gian thi (phút)</label>
                  <input v-model.number="form.duration" type="number" class="form-control" min="1" placeholder="60" />
                </div>
                <div class="form-group">
                  <label class="form-label">Số lượt thi tối đa</label>
                  <input v-model.number="form.maxAttempts" type="number" class="form-control" min="1" placeholder="1" />
                </div>
              </div>
              <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
                <div class="form-group">
                  <label class="form-label">Tổng điểm</label>
                  <input v-model.number="form.totalMark" type="number" class="form-control" min="1" placeholder="100" />
                </div>
                <div class="form-group">
                  <label class="form-label">Điểm đạt</label>
                  <input v-model.number="form.passMark" type="number" class="form-control" min="0" placeholder="50" />
                </div>
              </div>
            </div>

            
            <div class="form-group" style="margin-top: 16px; border-top: 1px solid var(--border-strong); padding-top: 16px;">
              <label class="form-label" style="display:flex; justify-content:space-between">
                <span>Chọn câu hỏi cho đề ({{ form.questionIds.length }} câu đã chọn)</span>
              </label>
              <div style="max-height: 200px; overflow-y: auto; border: 1px solid var(--border); border-radius: var(--radius-sm); padding: 12px; background: var(--bg-elevated)">
                <div v-if="allQuestions.length === 0" style="color:var(--text-muted); font-size:0.85rem">Chưa có câu hỏi nào trong ngân hàng.</div>
                <label v-for="q in allQuestions" :key="q.id" style="display:flex; align-items:flex-start; gap:10px; margin-bottom:12px; cursor:pointer">
                  <input type="checkbox" :value="q.id" v-model="form.questionIds" style="margin-top: 3px; width:16px; height:16px;" />
                  <div style="flex:1">
                    <div style="font-size: 0.88rem; font-weight:500; color:var(--text-primary); line-height:1.4">{{ q.content }}</div>
                    <div style="font-size: 0.75rem; color:var(--text-muted); margin-top:2px">
                      Loại: {{ {0:'Một đáp án',1:'Nhiều đáp án',2:'Đúng/Sai',3:'Tự luận',SingleChoice:'Một đáp án',MultipleChoice:'Nhiều đáp án',TrueFalse:'Đúng/Sai'}[q.questionType] || q.questionType }} | 
                      Độ khó: {{ {0:'Dễ',1:'Trung bình',2:'Khó',Easy:'Dễ',Medium:'Trung bình',Hard:'Khó'}[q.difficulty] || q.difficulty }}
                    </div>
                  </div>
                </label>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal">Hủy</button>
            <button class="btn btn-primary" :disabled="saving" @click="saveExam">
              <span v-if="saving" class="spinner"></span>
              {{ saving ? 'Đang lưu...' : (editingId ? 'Cập nhật' : 'Tạo đề thi') }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Delete Modal -->
    <Transition name="fade">
      <div v-if="deleteTarget" class="modal-overlay" @click.self="deleteTarget = null">
        <div class="modal modal-sm">
          <div class="modal-header">
            <h3 class="modal-title">Xác nhận xóa</h3>
            <button class="modal-close" @click="deleteTarget = null">×</button>
          </div>
          <div class="modal-body">
            <p>Bạn có chắc muốn xóa đề thi "<strong>{{ deleteTarget?.title }}</strong>" không?</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="deleteTarget = null">Hủy</button>
            <button class="btn btn-danger" :disabled="saving" @click="doDelete">
              <span v-if="saving" class="spinner"></span>
              Xóa đề thi
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Force Submit Modal -->
    <Transition name="fade">
      <div v-if="forceSubmitTarget" class="modal-overlay" @click.self="forceSubmitTarget = null">
        <div class="modal modal-sm" style="border: 3px solid var(--danger); box-shadow: 6px 6px 0px rgba(220, 38, 38, 0.2);">
          <div class="modal-header" style="border-bottom-color: var(--danger)">
            <h3 class="modal-title" style="color:var(--danger); display:flex; align-items:center; gap:8px">
              <ShieldAlert :size="20"/> Cảnh báo Khẩn
            </h3>
            <button class="modal-close" @click="forceSubmitTarget = null">×</button>
          </div>
          <div class="modal-body">
            <p style="text-align:center">Bạn sắp phát lệnh <strong>Cưỡng chế nộp bài</strong> đối với đề thi "<strong>{{ forceSubmitTarget?.title }}</strong>".</p>
            <div style="margin-top:12px; font-size:0.85rem; color: #dc2626; background: #fee2e2; padding: 12px; border-radius: 8px;">
              Tất cả bài làm đang dang dở trên trình duyệt của thí sinh sẽ lập tức bị chốt lại và tống khỏi phòng thi.
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="forceSubmitTarget = null">Hủy</button>
            <button class="btn btn-danger" :disabled="saving" @click="doForceSubmit">
              <span v-if="saving" class="spinner"></span>
              Phát Lệnh Ngay
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { Pencil, Trash, Search, ShieldAlert, FileQuestion, BookOpen, Clock } from 'lucide-vue-next'
import { ref, computed, onMounted, reactive } from 'vue'
import { examsApi } from '@/api/exams'
import { questionsApi } from '@/api/questions'
import { useNotificationStore } from '@/stores/notification'

const notify = useNotificationStore()

const allQuestions = ref([])

const exams       = ref([])
const loading     = ref(false)
const saving      = ref(false)
const showModal   = ref(false)
const editingId   = ref(null)
const deleteTarget = ref(null)
const forceSubmitTarget = ref(null)
const search      = ref('')

const form = reactive({
  title: '',
  description: '',
  duration: 60,
  maxAttempts: 1,
  totalMark: 100,
  passMark: 50,
  subjectId: 1,
  questionIds: [],
})

const filteredExams = computed(() =>
  !search.value
    ? exams.value
    : exams.value.filter(e => e.title.toLowerCase().includes(search.value.toLowerCase()))
)

async function fetchExams() {
  loading.value = true
  try {
    const { data } = await examsApi.getAll()
    exams.value = data.data || data || []
  } catch {
    notify.error('Lỗi', 'Không thể tải danh sách đề thi')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editingId.value = null
  Object.assign(form, { title: '', description: '', duration: 60, maxAttempts: 1, totalMark: 100, passMark: 50, subjectId: 1, questionIds: [] })
  showModal.value = true
}

async function openEdit(exam) {
  loading.value = true
  try {
    const { data } = await examsApi.getById(exam.id)
    const detailedExam = data.data || data
    editingId.value = detailedExam.id
    Object.assign(form, {
      title: detailedExam.title,
      description: detailedExam.description,
      duration: detailedExam.duration,
      maxAttempts: detailedExam.maxAttempts,
      totalMark: detailedExam.totalMark,
      passMark: detailedExam.passMark,
      subjectId: detailedExam.subjectId || 1,
      questionIds: detailedExam.examQuestions?.map(eq => eq.questionId) || []
    })
    showModal.value = true
  } catch (e) {
    notify.error('Lỗi', 'Không thể tải thông tin chi tiết đề thi')
  } finally {
    loading.value = false
  }
}

function closeModal() { showModal.value = false }
function confirmDelete(exam) { deleteTarget.value = exam }
function confirmForceSubmit(exam) { forceSubmitTarget.value = exam }

async function doForceSubmit() {
  saving.value = true
  try {
    const res = await examsApi.forceSubmit(forceSubmitTarget.value.id, "Hết giờ thi. Hệ thống đang tiến hành nộp bài tự động.")
    if (res.data?.success) {
      notify.success('Đã gửi hiệu lệnh', 'Toàn bộ bài làm dang dở đang được thu.')
    }
  } catch (err) {
    notify.error('Lỗi truyền sóng', err.response?.data?.message || 'Không thể kích hoạt lệnh thu bài.')
  } finally {
    saving.value = false
    forceSubmitTarget.value = null
  }
}

async function saveExam() {
  if (!form.title.trim()) return notify.warn('Thiếu thông tin', 'Vui lòng nhập tiêu đề đề thi')
  saving.value = true
  try {
    if (editingId.value) {
      await examsApi.update(editingId.value, form)
      notify.success('Cập nhật thành công', 'Đề thi đã được cập nhật')
    } else {
      await examsApi.create(form)
      notify.success('Tạo thành công', 'Đề thi mới đã được tạo')
    }
    closeModal()
    await fetchExams()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể lưu đề thi')
  } finally {
    saving.value = false
  }
}

async function doDelete() {
  saving.value = true
  try {
    await examsApi.delete(deleteTarget.value.id)
    notify.success('Đã xóa', 'Đề thi đã được xóa thành công')
    deleteTarget.value = null
    await fetchExams()
  } catch {
    notify.error('Lỗi', 'Không thể xóa đề thi này')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  await fetchExams()
  try {
    const { data } = await questionsApi.getAll()
    allQuestions.value = data.data || data || []
  } catch {
    console.warn('Failed to load questions')
  }
})
</script>
