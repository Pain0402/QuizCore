<template>
  <div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Ngân hàng câu hỏi</h1>
        <p class="page-subtitle">Quản lý toàn bộ câu hỏi trong hệ thống</p>
      </div>
      <div style="display:flex;gap:10px">
        <input type="file" id="excel-import" accept=".xlsx,.xls" style="display:none" @change="handleImport" />
        <button class="btn btn-secondary" style="border-color:var(--success);color:var(--success)" @click="triggerImport">
          📥 Nạp Excel
        </button>
        <button class="btn btn-secondary" style="border-color:var(--primary);color:var(--primary)" @click="exportToExcel">
          📤 Xuất Excel
        </button>
        <button class="btn btn-primary" @click="openCreate">
          <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
          </svg>
          Thêm câu hỏi
        </button>
      </div>
    </div>

    <div class="card unified-workspace" style="display:flex; align-items:stretch; margin-top: 20px; padding: 0; overflow: hidden; height: calc(100vh - 160px); min-height: 500px;">
      <!-- Left Sidebar (Tree) -->
      <div class="tree-sidebar" style="width: 260px; flex-shrink: 0; padding: 20px 16px; border-right: 1px solid var(--border); background: var(--bg-surface-alt); overflow-y: auto;">
        <h3 style="font-size: 1rem; margin-bottom: 16px; font-weight: 600; display:flex; align-items:center; gap:8px;">
          <FolderTree :size="18" style="color:var(--primary)"/> Cấu trúc thư mục
        </h3>
        <div style="display:flex; flex-direction:column; gap:4px;">
          <div class="tree-node" v-for="subj in subjectsTree" :key="subj.id" style="margin-bottom: 8px;">
            <div class="tree-node-title" @click="selectSubject(subj.id)" 
                 :style="{ fontWeight: 500, padding:'8px 12px', borderRadius:'6px', cursor:'pointer', display:'flex', alignItems:'center', gap:'8px', background: selectedSubject === subj.id && !selectedTopic ? 'var(--primary-light)' : 'transparent', color: selectedSubject === subj.id && !selectedTopic ? 'var(--primary)' : 'var(--text-primary)' }">
              <Folder :size="16" /> {{ subj.name }}
            </div>
            <div class="tree-children" v-if="subj.topics.length" style="padding-left: 10px; margin-top: 4px; display:flex; flex-direction:column; gap:2px; border-left: 1px dashed var(--border); margin-left: 16px;">
              <div class="tree-leaf" v-for="topic in subj.topics" :key="topic" @click="selectTopic(subj.id, topic)"
                   :style="{ padding:'6px 12px', fontSize:'0.85rem', cursor:'pointer', borderRadius:'6px', display:'flex', justifyContent:'space-between', background: selectedSubject === subj.id && selectedTopic === topic ? 'var(--primary-light)' : 'transparent', color: selectedSubject === subj.id && selectedTopic === topic ? 'var(--primary-hover)' : 'var(--text-secondary)' }">
                <span>🏷 {{ topic }}</span>
                <span style="font-size:0.75rem; color:var(--text-muted)">{{ filterCount(subj.id, topic) }}</span>
              </div>
            </div>
          </div>
          
          <div class="tree-node-title" @click="selectSubject(null)" 
               :style="{ fontWeight: 500, padding:'8px 12px', borderRadius:'6px', cursor:'pointer', display:'flex', alignItems:'center', gap:'8px', marginTop:'8px', borderTop:'1px solid var(--border)', paddingTop:'16px', background: selectedSubject === null ? 'var(--primary-light)' : 'transparent', color: selectedSubject === null ? 'var(--primary)' : 'var(--text-primary)' }">
            <Database :size="16" /> Tất cả câu hỏi
            <span class="badge badge-muted" style="margin-left:auto">{{ questions.length }}</span>
          </div>
        </div>
      </div>

      <!-- Right Content -->
      <div class="main-content" style="flex: 1; min-width: 0; padding: 24px; display:flex; flex-direction:column; background: var(--bg-surface); margin-left:0">
        <!-- Toolbar -->
        <div class="toolbar" style="margin-bottom: 20px;">
          <div class="search-box" style="flex:1;">
            <span class="search-icon"><Search :size="16" /></span>
            <input v-model="search" class="form-control" placeholder="Tìm kiếm tên câu hỏi..." />
          </div>
          <select v-model="filterDifficulty" class="form-control" style="width:140px">
            <option value="">Lọc độ khó</option>
            <option value="Easy">Dễ</option>
            <option value="Medium">Trung bình</option>
            <option value="Hard">Khó</option>
          </select>
          <select v-model="filterType" class="form-control" style="width:140px">
            <option value="">Lọc thể loại</option>
            <option value="Single">Một đáp án</option>
            <option value="Multiple">Nhiều đáp án</option>
            <option value="Short">Đúng/Sai</option>
          </select>
          <button class="btn btn-secondary btn-icon" title="Làm mới" @click="fetchQuestions">
            <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
          </button>
        </div>

        <!-- Loading -->
        <div v-if="loading" style="padding:48px;text-align:center; flex:1">
          <div class="loading-dots" style="justify-content:center">
            <span></span><span></span><span></span>
          </div>
          <p style="margin-top:12px;font-size:0.875rem; color:var(--text-muted)">Đang lấy dữ liệu...</p>
        </div>

        <!-- Table -->
        <div v-else-if="filteredQuestions.length" class="table-wrapper" style="border: 1px solid var(--border); border-radius: 8px; overflow-y: auto; box-shadow: none; flex: 1; min-height: 0;">
          <table class="data-table" style="width: 100%; border-collapse: collapse;">
            <thead style="position: sticky; top: 0; z-index: 10;">
              <tr style="background: var(--bg-elevated)">
                <th style="width: 48px; text-align: center">#</th>
                <th style="width: 30%">Nội dung câu hỏi</th>
                <th style="width: 18%">Môn / Chủ đề</th>
                <th style="width: 15%">Loại</th>
                <th style="width: 12%">Độ khó</th>
                <th style="width: 10%">Đáp án</th>
                <th style="width: 80px; text-align: center">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(q, i) in filteredQuestions" :key="q.id">
                <td style="color:var(--text-muted);text-align:center; font-weight: 500;">{{ i + 1 }}</td>
                <td style="max-width: 200px">
                  <div class="truncate" style="color:var(--text-primary);font-weight:500" :title="q.content">{{ q.content }}</div>
                </td>
                <td>
                  <span v-if="q.topic" class="badge badge-secondary" style="margin-right: 4px; background: var(--bg-hover); color: var(--text-primary); border: 1px solid var(--border)">🏷 {{ q.topic }}</span>
                  <span v-else class="badge badge-muted" style="margin-right: 4px;">{{ subjectsMap[q.subjectId] || 'Chung' }}</span>
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
                  <div style="display:flex;gap:6px; justify-content: center;">
                    <button class="btn btn-ghost btn-icon btn-sm" title="Chỉnh sửa" @click="openEdit(q)"><Pencil :size="14" stroke-width="2.5" /></button>
                    <button class="btn btn-danger btn-icon btn-sm" title="Xóa" @click="confirmDelete(q)"><Trash :size="14" stroke-width="2.5" /></button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty -->
        <div v-else style="padding: 64px 20px; flex:1; display:flex; align-items:center; justify-content:center;">
          <div class="empty-state" style="border: none; background: transparent;">
            <div class="empty-state-icon" style="background: var(--primary-light); color: var(--primary); width: 64px; height: 64px; line-height: 64px; border-radius: 50%; font-size: 24px; margin: 0 auto 16px;">❓</div>
            <div class="empty-state-title" style="font-size: 1.1rem; color: var(--text-primary)">Chưa có câu hỏi nào</div>
            <div class="empty-state-desc" style="color: var(--text-muted)">Thư mục này hiện tại chưa có câu hỏi nào.</div>
            <button class="btn btn-primary" style="margin-top:20px" @click="openCreate">Thêm câu hỏi mới</button>
          </div>
        </div>
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
                <textarea v-model="form.content" class="form-control" placeholder="Nhập nội dung câu hỏi... (Hỗ trợ Markdown và HTML)" rows="4" />
                <div style="font-size: 0.75rem; color: var(--text-muted); margin-top: 6px; display:flex; align-items:center; gap: 4px;">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>
                  Có thể sử dụng Markdown (**đậm**, *nghiêng*, `code`) hoặc thẻ HTML.
                </div>
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

              <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
                <div class="form-group">
                  <label class="form-label">Môn học</label>
                  <select v-model="form.subjectId" class="form-control">
                    <option :value="1">Toán học</option>
                    <option :value="2">Vật lý</option>
                    <option :value="3">Hóa học</option>
                    <option :value="4">Tiếng Anh</option>
                  </select>
                </div>
                <div class="form-group">
                  <label class="form-label">Chủ đề / Thẻ (Tag)</label>
                  <input v-model="form.topic" type="text" class="form-control" placeholder="VD: Lượng giác, Động lực học..." />
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
import * as XLSX from 'xlsx'
import { Pencil, Trash, Search, ShieldAlert, FileQuestion, BookOpen, Clock, FolderTree, Folder, Database } from 'lucide-vue-next'
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
  subjectId: 1,
  topic: '',
  answers: [],
})

const typeLabels       = { Single: 'Một đáp án', Multiple: 'Nhiều đáp án', Short: 'Đúng/Sai', SingleChoice: 'Một đáp án', MultipleChoice: 'Nhiều đáp án', TrueFalse: 'Đúng/Sai', 0: 'Một đáp án', 1: 'Nhiều đáp án', 2: 'Đúng/Sai', 3: 'Tự luận' }
const difficultyLabels = { Easy: 'Dễ', Medium: 'Trung bình', Hard: 'Khó', 0: 'Dễ', 1: 'Trung bình', 2: 'Khó' }
const difficultyBadge  = { Easy: 'badge-success', Medium: 'badge-warning', Hard: 'badge-danger', 0: 'badge-success', 1: 'badge-warning', 2: 'badge-danger' }

const selectedSubject = ref(null)
const selectedTopic = ref(null)

const subjectsMap = {
  1: 'Toán học',
  2: 'Vật lý',
  3: 'Hóa học',
  4: 'Tiếng Anh'
}

const subjectsTree = computed(() => {
  const tree = []
  for (const [id, name] of Object.entries(subjectsMap)) {
    const sId = parseInt(id)
    const questionsInSubj = questions.value.filter(q => q.subjectId === sId)
    if (questionsInSubj.length === 0) continue
    const uniqueTopics = [...new Set(questionsInSubj.map(q => q.topic).filter(t => t))]
    tree.push({ id: sId, name, topics: uniqueTopics.sort() })
  }
  return tree
})

function filterCount(sId, topic) {
  return questions.value.filter(q => q.subjectId === sId && q.topic === topic).length
}

function selectSubject(sId) {
  selectedSubject.value = sId
  selectedTopic.value = null
}

function selectTopic(sId, topic) {
  selectedSubject.value = sId
  selectedTopic.value = topic
}

const filteredQuestions = computed(() => {
  return questions.value.filter(q => {
    const matchSearch     = !search.value || q.content.toLowerCase().includes(search.value.toLowerCase())
    const matchDifficulty = !filterDifficulty.value || q.difficulty === filterDifficulty.value
    const matchType       = !filterType.value || q.questionType === filterType.value
    const matchSubject    = selectedSubject.value === null || q.subjectId === selectedSubject.value
    const matchTopic      = selectedTopic.value === null || q.topic === selectedTopic.value
    return matchSearch && matchDifficulty && matchType && matchSubject && matchTopic
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
  Object.assign(form, { content: '', questionType: 'SingleChoice', difficulty: 'Medium', subjectId: 1, topic: '', answers: [{ content: '', isCorrect: true }, { content: '', isCorrect: false }] })
  showModal.value = true
}

function openEdit(q) {
  editingId.value = q.id
  Object.assign(form, {
    content: q.content,
    questionType: q.questionType,
    difficulty: q.difficulty,
    subjectId: q.subjectId || 1,
    topic: q.topic || '',
    answers: q.answers?.map(a => ({ ...a })) || []
  })
  showModal.value = true
}

function closeModal() { showModal.value = false }

function addAnswer() { form.answers.push({ content: '', isCorrect: false }) }
function removeAnswer(i) { form.answers.splice(i, 1) }

function confirmDelete(q) { deleteTarget.value = q }

function exportToExcel() {
  const data = questions.value.map(q => ({
    'Nội dung': q.content,
    'Môn học': 'Toán học',
    'Chủ đề': q.topic || '',
    'Loại': typeLabels[q.questionType] || q.questionType,
    'Độ khó': difficultyLabels[q.difficulty] || q.difficulty,
    'Đáp án đúng': q.answers?.filter(a => a.isCorrect).map(a => a.content).join(' | '),
    'Các đáp án': q.answers?.map(a => a.content).join(' | ')
  }))
  const ws = XLSX.utils.json_to_sheet(data)
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, "CauHoi")
  XLSX.writeFile(wb, "NganHangCauHoi.xlsx")
}

function triggerImport() { document.getElementById('excel-import').click() }

async function handleImport(event) {
  const file = event.target.files[0]
  if (!file) return
  
  loading.value = true
  const reader = new FileReader()
  reader.onload = async (e) => {
    try {
      const data = new Uint8Array(e.target.result)
      const workbook = XLSX.read(data, { type: 'array' })
      const firstSheet = workbook.SheetNames[0]
      const jsonData = XLSX.utils.sheet_to_json(workbook.Sheets[firstSheet])
      
      let successCount = 0
      for (const row of jsonData) {
        const content = row['Nội dung'] || row['Content']
        if (!content) continue
        
        let diff = row['Độ khó'] || row['Difficulty'] || 'Easy'
        if (diff === 'Dễ') diff = 'Easy'
        if (diff === 'Trung bình') diff = 'Medium'
        if (diff === 'Khó') diff = 'Hard'

        let qt = row['Loại'] || row['Type'] || 'SingleChoice'
        if (qt === 'Một đáp án' || qt == 'Single') qt = 'SingleChoice'
        else if (qt === 'Nhiều đáp án') qt = 'MultipleChoice'
        else if (qt === 'Đúng/Sai') qt = 'TrueFalse'

        const subjStr = row['Môn học'] || row['Subject'] || ''
        let subjId = 1 // Toán học
        if (subjStr === 'Vật lý') subjId = 2
        else if (subjStr === 'Hóa học') subjId = 3
        else if (subjStr === 'Tiếng Anh') subjId = 4

        const topicStr = row['Chủ đề'] || row['Topic'] || ''

        const correctAnsStr = String(row['Đáp án đúng'] || row['Correct'] || '')
        const allAnsStr = String(row['Các đáp án'] || row['Answers'] || '')
        
        const allAnsPaths = allAnsStr.split('|').map(x => x.trim()).filter(x => x)
        const correctAnsPaths = correctAnsStr.split('|').map(x => x.trim()).filter(x => x)
        
        const answers = allAnsPaths.map(ans => ({
          content: ans,
          isCorrect: correctAnsPaths.includes(ans)
        }))

        try {
          await questionsApi.create({
            content,
            difficulty: diff,
            questionType: qt,
            subjectId: subjId,
            topic: topicStr,
            answers: answers.length > 0 ? answers : [{ content: 'Đúng', isCorrect: true }, { content: 'Sai', isCorrect: false }]
          })
          successCount++
        } catch (err) { console.error('Failed to row:', err) }
      }
      notify.success('Nhập file thành công', `Đã thêm ${successCount} câu hỏi từ Excel`)
      await fetchQuestions()
    } catch (e) {
      notify.error('Lỗi file Excel', 'File bị hỏng hoặc định dạng sai')
    } finally {
      loading.value = false
      event.target.value = ''
    }
  }
  reader.readAsArrayBuffer(file)
}

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
