<template>
  <div>
    <!-- Header -->
    <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:24px">
      <div>
        <h1 style="font-size:1.4rem;margin-bottom:4px;display:flex;align-items:center;gap:8px">
          <GraduationCap :size="24" :stroke-width="2.5" /> Quản lý lớp học
        </h1>
        <p>Tổ chức học sinh theo lớp và năm học</p>
      </div>
      <button class="btn btn-primary" @click="openCreate">+ Tạo lớp mới</button>
    </div>

    <!-- Class grid -->
    <div v-if="loading" style="display:flex;justify-content:center;padding:60px">
      <div class="loading-dots"><span></span><span></span><span></span></div>
    </div>

    <div v-else style="display:grid;grid-template-columns:repeat(auto-fill,minmax(300px,1fr));gap:16px">
      <div v-if="!classes.length" class="card" style="grid-column:1/-1;padding:40px;text-align:center;color:var(--text-muted)">
        Chưa có lớp học nào. Hãy tạo lớp đầu tiên!
      </div>
      <div v-for="cls in classes" :key="cls.id" class="card card-hover" style="cursor:default">
        <div style="display:flex;align-items:flex-start;justify-content:space-between;margin-bottom:12px">
          <div style="display:flex;align-items:center;gap:10px">
            <div class="stat-icon" style="width:40px;height:40px;background:var(--info-light);color:var(--info);display:flex;align-items:center;justify-content:center;border-radius:var(--radius-sm)"><UsersRound :size="20" stroke-width="2.5" /></div>
            <div>
              <div style="font-weight:700;font-size:1.05rem;color:var(--text-primary)">{{ cls.name }}</div>
              <div style="font-size:0.78rem;color:var(--text-muted)">Năm học: {{ cls.academicYear || '—' }}</div>
            </div>
          </div>
          <div style="display:flex;gap:6px">
            <button class="btn btn-secondary btn-icon" @click="openEdit(cls)" title="Sửa"><Pencil :size="16" stroke-width="2.5" /></button>
            <button class="btn btn-danger btn-icon" @click="confirmDelete(cls)" title="Xóa"><Trash :size="16" stroke-width="2.5" /></button>
          </div>
        </div>

        <div style="display:flex;align-items:center;justify-content:space-between;padding:10px 0;border-top:2px solid var(--border-strong)">
          <span style="color:var(--text-muted);font-size:0.85rem;display:flex;align-items:center;gap:6px"><Users :size="16" /> {{ cls.studentCount }} học sinh</span>
          <button class="btn btn-sm" style="background:var(--primary-light);color:var(--primary)"
            @click="openManageStudents(cls)">
            Quản lý học sinh →
          </button>
        </div>
      </div>
    </div>

    <!-- Create / Edit Modal -->
    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay">
        <div class="modal modal-sm">
          <div class="modal-header">
            <h3 class="modal-title">{{ editingClass ? 'Chỉnh sửa lớp' : 'Tạo lớp mới' }}</h3>
            <button class="modal-close" @click="showModal=false">✕</button>
          </div>
          <div class="modal-body">
            <div class="form-group">
              <label class="form-label">Tên lớp *</label>
              <input v-model="form.name" class="form-control" placeholder="VD: 12A1, CNTT-K22" />
            </div>
            <div class="form-group">
              <label class="form-label">Năm học</label>
              <input v-model="form.academicYear" class="form-control" placeholder="VD: 2024-2025" />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showModal=false">Hủy</button>
            <button class="btn btn-primary" :disabled="saving" @click="saveClass">
              <span v-if="saving" class="spinner"></span>
              {{ editingClass ? 'Cập nhật' : 'Tạo lớp' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Manage Students Modal -->
    <Transition name="fade">
      <div v-if="managingClass" class="modal-overlay">
        <div class="modal" style="max-width:700px">
          <div class="modal-header">
            <h3 class="modal-title" style="display:flex;align-items:center;gap:8px"><UsersRound :size="20"/> Danh sách học sinh – {{ managingClass.name }}</h3>
            <button class="modal-close" @click="managingClass=null"><X :size="20"/></button>
          </div>
          <div class="modal-body">
            <!-- Assign new students -->
            <div style="display:flex;gap:8px;margin-bottom:16px">
              <select v-model="selectedUserIds" class="form-control" multiple style="height:80px;flex:1">
                <option v-for="st in availableStudents" :key="st.id" :value="st.id">
                  {{ st.fullName }} (@{{ st.username }})
                </option>
              </select>
              <div style="display:flex;flex-direction:column;justify-content:center;gap:6px">
                <button class="btn btn-primary btn-sm" @click="assignStudents" :disabled="!selectedUserIds.length">
                  ← Thêm vào lớp
                </button>
              </div>
            </div>

            <!-- Current students list -->
            <div v-if="classStudents.length === 0" style="text-align:center;padding:20px;color:var(--text-muted)">
              Chưa có học sinh trong lớp
            </div>
            <table v-else class="table" style="margin-bottom:0">
              <thead><tr><th>Học sinh</th><th>Username</th><th>Email</th><th style="text-align:right"></th></tr></thead>
              <tbody>
                <tr v-for="st in classStudents" :key="st.userId" class="table-row">
                  <td>
                    <div style="display:flex;align-items:center;gap:8px">
                      <div class="user-avatar-sm">{{ st.fullName[0]?.toUpperCase() }}</div>
                      <span style="font-weight:600">{{ st.fullName }}</span>
                    </div>
                  </td>
                  <td>@{{ st.username }}</td>
                  <td style="color:var(--text-muted)">{{ st.email || '—' }}</td>
                  <td style="text-align:right">
                    <button class="btn btn-danger btn-icon" @click="removeStudent(st.userId)" title="Xóa khỏi lớp"><UserX :size="16" stroke-width="2.5"/></button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="managingClass=null">Đóng</button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Delete Confirm -->
    <Transition name="fade">
      <div v-if="deletingClass" class="modal-overlay">
        <div class="modal modal-sm">
          <div class="modal-header"><h3 class="modal-title">Xác nhận xóa</h3></div>
          <div class="modal-body" style="text-align:center">
            <div style="color:var(--warning);display:flex;justify-content:center;margin-bottom:12px"><ShieldAlert :size="48" :stroke-width="1.5" /></div>
            <p>Bạn có chắc muốn xóa lớp <strong>{{ deletingClass.name }}</strong>?</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="deletingClass=null">Hủy</button>
            <button class="btn btn-danger" @click="deleteClass">Xóa</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { classesApi } from '@/api/classes'
import { usersApi } from '@/api/users'
import { useNotificationStore } from '@/stores/notification'
import { Pencil, Trash, UserX, Users, GraduationCap, UsersRound, X, ShieldAlert } from 'lucide-vue-next'

const notify = useNotificationStore()

const classes       = ref([])
const allStudents   = ref([])
const classStudents = ref([])
const loading       = ref(false)
const saving        = ref(false)
const showModal     = ref(false)
const editingClass  = ref(null)
const deletingClass = ref(null)
const managingClass = ref(null)
const selectedUserIds = ref([])

const form = ref({ name: '', academicYear: '' })

const availableStudents = computed(() => {
  const inClass = new Set(classStudents.value.map(s => s.userId))
  return allStudents.value.filter(s => !inClass.has(s.id) && s.role === 'Student')
})

async function loadClasses() {
  loading.value = true
  try {
    const { data } = await classesApi.getAll()
    classes.value = data.data
  } catch { notify.error('Lỗi', 'Không thể tải lớp học') }
  finally { loading.value = false }
}

async function loadAllStudents() {
  try {
    const { data } = await usersApi.getAll({ role: 'Student' })
    allStudents.value = data.data
  } catch { /* silent */ }
}

function openCreate() {
  editingClass.value = null
  form.value = { name: '', academicYear: '' }
  showModal.value = true
}

function openEdit(cls) {
  editingClass.value = cls
  form.value = { name: cls.name, academicYear: cls.academicYear || '' }
  showModal.value = true
}

async function openManageStudents(cls) {
  managingClass.value = cls
  selectedUserIds.value = []
  const { data } = await classesApi.getStudents(cls.id)
  classStudents.value = data.data
  await loadAllStudents()
}

async function saveClass() {
  saving.value = true
  try {
    if (editingClass.value) {
      await classesApi.update(editingClass.value.id, form.value)
      notify.success('Thành công', 'Đã cập nhật lớp học')
    } else {
      await classesApi.create(form.value)
      notify.success('Thành công', 'Đã tạo lớp học mới')
    }
    showModal.value = false
    loadClasses()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể lưu')
  } finally { saving.value = false }
}

function confirmDelete(cls) { deletingClass.value = cls }

async function deleteClass() {
  try {
    await classesApi.delete(deletingClass.value.id)
    notify.success('Đã xóa', `Lớp ${deletingClass.value.name} đã được xóa`)
    deletingClass.value = null
    loadClasses()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể xóa lớp')
  }
}

async function assignStudents() {
  if (!selectedUserIds.value.length) return
  try {
    await classesApi.assignStudents(managingClass.value.id, selectedUserIds.value)
    notify.success('Thành công', `Đã thêm ${selectedUserIds.value.length} học sinh`)
    selectedUserIds.value = []
    const { data } = await classesApi.getStudents(managingClass.value.id)
    classStudents.value = data.data
    loadClasses()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể thêm học sinh')
  }
}

async function removeStudent(userId) {
  try {
    await classesApi.removeStudent(managingClass.value.id, userId)
    classStudents.value = classStudents.value.filter(s => s.userId !== userId)
    notify.success('Đã xóa', 'Học sinh đã được đưa ra khỏi lớp')
    loadClasses()
  } catch { notify.error('Lỗi', 'Không thể xóa học sinh') }
}

onMounted(loadClasses)
</script>

<style scoped>
.user-avatar-sm {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: var(--primary-light);
  color: var(--primary);
  font-weight: 700;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
</style>
