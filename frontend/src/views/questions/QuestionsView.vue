<template>
  <div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Ngân hàng câu hỏi</h1>
        <p class="page-subtitle">Quản lý toàn bộ câu hỏi trong hệ thống</p>
      </div>
      <button class="btn btn-primary" @click="openCreate">
        <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
        </svg>
        Thêm câu hỏi
      </button>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="search-box">
        <span class="search-icon"><Search :size="16" /></span>
        <input v-model="search" class="form-control" placeholder="Tìm kiếm câu hỏi..." />
      </div>
      <select v-model="filterDifficulty" class="form-control" style="width:auto">
        <option value="">Tất cả độ khó</option>
        <option value="Easy">Dễ</option>
        <option value="Medium">Trung bình</option>
        <option value="Hard">Khó</option>
      </select>
      <select v-model="filterType" class="form-control" style="width:auto">
        <option value="">Tất cả loại</option>
        <option value="Single">Một đáp án</option>
        <option value="Multiple">Nhiều đáp án</option>
        <option value="Short">Đúng/Sai</option>
      </select>
      <button class="btn btn-secondary" @click="fetchQuestions">
        <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
        </svg>
        Làm mới
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="card" style="padding:48px;text-align:center">
      <div class="loading-dots" style="justify-content:center">
        <span></span><span></span><span></span>
      </div>
      <p style="margin-top:12px;font-size:0.875rem">Đang tải...</p>
    </div>

    <!-- Table -->
    <div v-else-if="filteredQuestions.length" class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th>#</th>
            <th>Nội dung câu hỏi</th>
            <th>Loại</th>
            <th>Độ khó</th>
            <th>Đáp án</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(q, i) in filteredQuestions" :key="q.id">
            <td style="color:var(--text-muted);width:48px">{{ i + 1 }}</td>
            <td>
              <div class="truncate" style="max-width:380px;color:var(--text-primary);font-weight:500">{{ q.content }}</div>
            </td>
            <td><span class="badge badge-info">{{ typeLabels[q.questionType] || q.questionType }}</span></td>
            <td>
              <span class="badge" :class="difficultyBadge[q.difficulty]">
                {{ difficultyLabels[q.difficulty] || q.difficulty }}
              </span>
            </td>
            <td>
              <span class="badge badge-muted">{{ q.answers?.length || 0 }} đáp án</span>
            </td>
            <td>
              <div style="display:flex;gap:6px">
                <button class="btn btn-ghost btn-icon btn-icon" title="Chỉnh sửa" @click="openEdit(q)"><Pencil :size="16" stroke-width="2.5" /></button>
                <button class="btn btn-danger btn-icon btn-icon" title="Xóa" @click="confirmDelete(q)"><Trash :size="16" stroke-width="2.5" /></button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Empty -->
    <div v-else class="card">
      <div class="empty-state">
        <div class="empty-state-icon">❓</div>
        <div class="empty-state-title">Chưa có câu hỏi nào</div>
        <div class="empty-state-desc">Bắt đầu bằng cách thêm câu hỏi đầu tiên vào ngân hàng</div>
        <button class="btn btn-primary" style="margin-top:16px" @click="openCreate">Thêm câu hỏi</button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
        <div class="modal modal-lg">
          <div class="modal-header">
            <h3 class="modal-title">{{ editingId ? 'Chỉnh sửa câu hỏi' : 'Thêm câu hỏi mới' }}</h3>
            <button class="modal-close" @click="closeModal">×</button>
          </div>
          <div class="modal-body">
            <div style="display:flex;flex-direction:column;gap:18px">
              <div class="form-group">
                <label class="form-label">Nội dung câu hỏi <span class="form-required">*</span></label>
                <textarea v-model="form.content" class="form-control" placeholder="Nhập nội dung câu hỏi..." rows="3" />
              </div>
              <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
                <div class="form-group">
                  <label class="form-label">Loại câu hỏi</label>
                  <select v-model="form.questionType" class="form-control">
                    <option value="Single">Một đáp án</option>
                    <option value="Multiple">Nhiều đáp án</option>
                    <option value="Short">Đúng/Sai</option>
                  </select>
                </div>
                <div class="form-group">
                  <label class="form-label">Độ khó</label>
                  <select v-model="form.difficulty" class="form-control">
                    <option value="Easy">Dễ</option>
                    <option value="Medium">Trung bình</option>
                    <option value="Hard">Khó</option>
                  </select>
                </div>
              </div>

              <!-- Answers -->
              <div class="form-group">
                <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:10px">
                  <label class="form-label" style="margin:0">Đáp án</label>
                  <button type="button" class="btn btn-secondary btn-sm" @click="addAnswer">+ Thêm đáp án</button>
                </div>
                <div style="display:flex;flex-direction:column;gap:8px">
                  <div v-for="(ans, idx) in form.answers" :key="idx"
                    style="display:flex;align-items:center;gap:10px;padding:10px;background:var(--bg-elevated);border-radius:var(--radius-sm);border:1px solid var(--border)"
                  >
                    <input type="checkbox" v-model="ans.isCorrect" style="accent-color:var(--success);flex-shrink:0"
                      :title="ans.isCorrect ? 'Đáp án đúng' : 'Đáp án sai'" />
                    <input v-model="ans.content" class="form-control" style="flex:1"
                      :placeholder="`Đáp án ${idx + 1}...`" />
                    <span :class="ans.isCorrect ? 'badge-success' : 'badge-muted'" class="badge" style="flex-shrink:0">
                      {{ ans.isCorrect ? '✓ Đúng' : 'Sai' }}
                    </span>
                    <button type="button" class="btn btn-ghost btn-icon btn-icon" @click="removeAnswer(idx)">×</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal">Hủy</button>
            <button class="btn btn-primary" :disabled="saving" @click="saveQuestion">
              <span v-if="saving" class="spinner"></span>
              {{ saving ? 'Đang lưu...' : (editingId ? 'Cập nhật' : 'Tạo câu hỏi') }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Delete Confirm Modal -->
    <Transition name="fade">
      <div v-if="deleteTarget" class="modal-overlay" @click.self="deleteTarget = null">
        <div class="modal modal-sm">
          <div class="modal-header">
            <h3 class="modal-title">Xác nhận xóa</h3>
            <button class="modal-close" @click="deleteTarget = null">×</button>
          </div>
          <div class="modal-body">
            <p>Bạn có chắc muốn xóa câu hỏi này không? Hành động này không thể hoàn tác.</p>
            <div style="margin-top:12px;padding:12px;background:var(--bg-elevated);border-radius:var(--radius-sm);font-size:0.875rem;color:var(--text-muted)">
              {{ deleteTarget?.content }}
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="deleteTarget = null">Hủy</button>
            <button class="btn btn-danger" :disabled="saving" @click="doDelete">
              <span v-if="saving" class="spinner"></span>
              {{ saving ? 'Đang xóa...' : 'Xóa câu hỏi' }}
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
import { questionsApi } from '@/api/questions'
import { useNotificationStore } from '@/stores/notification'

const notify = useNotificationStore()

const questions       = ref([])
const loading         = ref(false)
const saving          = ref(false)
const showModal       = ref(false)
const editingId       = ref(null)
const deleteTarget    = ref(null)
const search          = ref('')
const filterDifficulty = ref('')
const filterType       = ref('')

const form = reactive({
  content: '',
  questionType: 'Single',
  difficulty: 'Medium',
  answers: [],
})

const typeLabels       = { Single: 'Một đáp án', Multiple: 'Nhiều đáp án', Short: 'Đúng/Sai', SingleChoice: 'Một đáp án', MultipleChoice: 'Nhiều đáp án', TrueFalse: 'Đúng/Sai' }
const difficultyLabels = { Easy: 'Dễ', Medium: 'Trung bình', Hard: 'Khó' }
const difficultyBadge  = { Easy: 'badge-success', Medium: 'badge-warning', Hard: 'badge-danger' }

const filteredQuestions = computed(() => {
  return questions.value.filter(q => {
    const matchSearch     = !search.value || q.content.toLowerCase().includes(search.value.toLowerCase())
    const matchDifficulty = !filterDifficulty.value || q.difficulty === filterDifficulty.value
    const matchType       = !filterType.value || q.questionType === filterType.value
    return matchSearch && matchDifficulty && matchType
  })
})

async function fetchQuestions() {
  loading.value = true
  try {
    const { data } = await questionsApi.getAll()
    questions.value = data.data || data || []
  } catch {
    notify.error('Lỗi', 'Không thể tải danh sách câu hỏi')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editingId.value = null
  Object.assign(form, { content: '', questionType: 'SingleChoice', difficulty: 'Medium', answers: [] })
  addAnswer(); addAnswer()
  showModal.value = true
}

function openEdit(q) {
  editingId.value = q.id
  Object.assign(form, {
    content: q.content,
    questionType: q.questionType,
    difficulty: q.difficulty,
    answers: (q.answers || []).map(a => ({ content: a.content, isCorrect: a.isCorrect }))
  })
  showModal.value = true
}

function closeModal() { showModal.value = false }

function addAnswer() { form.answers.push({ content: '', isCorrect: false }) }
function removeAnswer(i) { form.answers.splice(i, 1) }

function confirmDelete(q) { deleteTarget.value = q }

async function saveQuestion() {
  if (!form.content.trim()) return notify.warn('Thiếu thông tin', 'Vui lòng nhập nội dung câu hỏi')
  saving.value = true
  try {
    if (editingId.value) {
      await questionsApi.update(editingId.value, form)
      notify.success('Cập nhật thành công', 'Câu hỏi đã được cập nhật')
    } else {
      await questionsApi.create(form)
      notify.success('Tạo thành công', 'Câu hỏi mới đã được thêm vào ngân hàng')
    }
    closeModal()
    await fetchQuestions()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể lưu câu hỏi')
  } finally {
    saving.value = false
  }
}

async function doDelete() {
  if (!deleteTarget.value) return
  saving.value = true
  try {
    await questionsApi.delete(deleteTarget.value.id)
    notify.success('Đã xóa', 'Câu hỏi đã được xóa thành công')
    deleteTarget.value = null
    await fetchQuestions()
  } catch {
    notify.error('Lỗi', 'Không thể xóa câu hỏi này')
  } finally {
    saving.value = false
  }
}

onMounted(fetchQuestions)
</script>
