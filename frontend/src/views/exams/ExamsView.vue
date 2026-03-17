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
        <span class="search-icon">🔍</span>
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
            <button class="btn btn-ghost btn-icon btn-sm" @click="openEdit(exam)">✏️</button>
            <button class="btn btn-danger btn-icon btn-sm" @click="confirmDelete(exam)">🗑️</button>
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
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue'
import { examsApi } from '@/api/exams'
import { useNotificationStore } from '@/stores/notification'

const notify = useNotificationStore()

const exams       = ref([])
const loading     = ref(false)
const saving      = ref(false)
const showModal   = ref(false)
const editingId   = ref(null)
const deleteTarget = ref(null)
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
  Object.assign(form, { title: '', description: '', duration: 60, maxAttempts: 1, totalMark: 100, passMark: 50 })
  showModal.value = true
}

function openEdit(exam) {
  editingId.value = exam.id
  Object.assign(form, {
    title: exam.title,
    description: exam.description,
    duration: exam.duration,
    maxAttempts: exam.maxAttempts,
    totalMark: exam.totalMark,
    passMark: exam.passMark,
  })
  showModal.value = true
}

function closeModal() { showModal.value = false }
function confirmDelete(exam) { deleteTarget.value = exam }

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

onMounted(fetchExams)
</script>
