<template>
  <div>
    <!-- Header -->
    <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:24px">
      <div>
        <h1 style="font-size:1.4rem;margin-bottom:4px">👥 Quản lý người dùng</h1>
        <p>Tạo, chỉnh sửa và quản lý tài khoản trong hệ thống</p>
      </div>
      <button class="btn btn-primary" @click="openCreate">+ Thêm người dùng</button>
    </div>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="search-box">
        <span class="search-icon">🔍</span>
        <input v-model="search" class="form-control" placeholder="Tìm theo tên, username, email..." @input="debouncedLoad" />
      </div>
      <select v-model="filterRole" class="form-control" style="width:160px" @change="loadUsers">
        <option value="">Tất cả vai trò</option>
        <option value="Admin">Admin</option>
        <option value="Teacher">Giáo viên</option>
        <option value="Student">Học sinh</option>
      </select>
    </div>

    <!-- Table -->
    <div class="card" style="padding:0;overflow:hidden">
      <div v-if="loading" style="padding:60px;text-align:center">
        <div class="loading-dots" style="justify-content:center"><span></span><span></span><span></span></div>
      </div>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Người dùng</th>
            <th>Email</th>
            <th>Vai trò</th>
            <th>Trạng thái</th>
            <th>Ngày tạo</th>
            <th style="text-align:right">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="!users.length">
            <td colspan="6" style="text-align:center;padding:40px;color:var(--text-muted)">Không tìm thấy người dùng nào</td>
          </tr>
          <tr v-for="user in users" :key="user.id" class="table-row">
            <td>
              <div style="display:flex;align-items:center;gap:10px">
                <div class="user-avatar">{{ user.fullName[0]?.toUpperCase() }}</div>
                <div>
                  <div style="font-weight:600;color:var(--text-primary)">{{ user.fullName }}</div>
                  <div style="font-size:0.78rem;color:var(--text-muted)">@{{ user.username }}</div>
                </div>
              </div>
            </td>
            <td>{{ user.email || '—' }}</td>
            <td>
              <span class="badge" :class="roleBadge(user.role)">{{ roleLabel(user.role) }}</span>
            </td>
            <td>
              <span class="badge" :class="user.status === 'Active' ? 'badge-success' : 'badge-danger'">
                {{ user.status === 'Active' ? '● Hoạt động' : '○ Vô hiệu' }}
              </span>
            </td>
            <td style="color:var(--text-muted);font-size:0.85rem">{{ fmtDate(user.createdAt) }}</td>
            <td>
              <div style="display:flex;gap:6px;justify-content:flex-end">
                <button class="btn btn-secondary btn-sm" @click="openEdit(user)" title="Sửa">✏️</button>
                <button class="btn btn-sm" :class="user.status === 'Active' ? 'btn-warning' : 'btn-success'"
                  @click="toggleStatus(user)" :title="user.status === 'Active' ? 'Vô hiệu hóa' : 'Kích hoạt'">
                  {{ user.status === 'Active' ? '🔒' : '🔓' }}
                </button>
                <button class="btn btn-danger btn-sm" @click="confirmDelete(user)" title="Xóa">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create / Edit Modal -->
    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay">
        <div class="modal">
          <div class="modal-header">
            <h3 class="modal-title">{{ editingUser ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới' }}</h3>
            <button class="modal-close" @click="closeModal">✕</button>
          </div>
          <div class="modal-body">
            <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px">
              <div class="form-group" style="grid-column:1/-1">
                <label class="form-label">Họ và tên *</label>
                <input v-model="form.fullName" class="form-control" placeholder="Nguyễn Văn A" />
              </div>
              <div class="form-group" v-if="!editingUser">
                <label class="form-label">Tên đăng nhập *</label>
                <input v-model="form.username" class="form-control" placeholder="nguyenvana" />
              </div>
              <div class="form-group" v-if="!editingUser">
                <label class="form-label">Mật khẩu *</label>
                <input v-model="form.password" class="form-control" type="password" placeholder="••••••••" />
              </div>
              <div v-if="editingUser" class="form-group" style="grid-column:1/-1">
                <label class="form-label">Mật khẩu mới <span style="color:var(--text-muted)">(để trống = giữ nguyên)</span></label>
                <input v-model="form.newPassword" class="form-control" type="password" placeholder="••••••••" />
              </div>
              <div class="form-group">
                <label class="form-label">Email</label>
                <input v-model="form.email" class="form-control" type="email" placeholder="email@example.com" />
              </div>
              <div class="form-group">
                <label class="form-label">Vai trò *</label>
                <select v-model="form.role" class="form-control">
                  <option value="Admin">Admin</option>
                  <option value="Teacher">Giáo viên</option>
                  <option value="Student">Học sinh</option>
                </select>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal">Hủy</button>
            <button class="btn btn-primary" :disabled="saving" @click="saveUser">
              <span v-if="saving" class="spinner"></span>
              {{ editingUser ? 'Cập nhật' : 'Tạo người dùng' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Delete Confirm -->
    <Transition name="fade">
      <div v-if="deletingUser" class="modal-overlay">
        <div class="modal modal-sm">
          <div class="modal-header"><h3 class="modal-title">Xác nhận xóa</h3></div>
          <div class="modal-body" style="text-align:center">
            <div style="font-size:2rem;margin-bottom:8px">⚠️</div>
            <p>Bạn có chắc muốn xóa tài khoản <strong>{{ deletingUser.fullName }}</strong>?</p>
            <p style="color:var(--danger);font-size:0.85rem;margin-top:6px">Hành động này không thể hoàn tác!</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="deletingUser = null">Hủy</button>
            <button class="btn btn-danger" @click="deleteUser">Xóa</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { usersApi } from '@/api/users'
import { useNotificationStore } from '@/stores/notification'

const notify = useNotificationStore()

const users      = ref([])
const loading    = ref(false)
const saving     = ref(false)
const showModal  = ref(false)
const editingUser = ref(null)
const deletingUser = ref(null)
const search     = ref('')
const filterRole = ref('')
let debounceTimer = null

const form = ref({ fullName: '', username: '', password: '', email: '', role: 'Student', newPassword: '' })

const roleBadge = (r) => ({ Admin:'badge-danger', Teacher:'badge-primary', Student:'badge-muted' }[r] || 'badge-muted')
const roleLabel = (r) => ({ Admin:'Quản trị', Teacher:'Giáo viên', Student:'Học sinh' }[r] || r)
const fmtDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN') : '—'

function debouncedLoad() {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(loadUsers, 400)
}

async function loadUsers() {
  loading.value = true
  try {
    const { data } = await usersApi.getAll({ role: filterRole.value || undefined, search: search.value || undefined })
    users.value = data.data
  } catch { notify.error('Lỗi', 'Không thể tải danh sách người dùng') }
  finally { loading.value = false }
}

function openCreate() {
  editingUser.value = null
  form.value = { fullName: '', username: '', password: '', email: '', role: 'Student', newPassword: '' }
  showModal.value = true
}

function openEdit(user) {
  editingUser.value = user
  form.value = { fullName: user.fullName, email: user.email || '', role: user.role, newPassword: '' }
  showModal.value = true
}

function closeModal() { showModal.value = false }

async function saveUser() {
  saving.value = true
  try {
    if (editingUser.value) {
      await usersApi.update(editingUser.value.id, {
        fullName: form.value.fullName,
        email: form.value.email,
        role: form.value.role,
        newPassword: form.value.newPassword || undefined
      })
      notify.success('Thành công', 'Đã cập nhật người dùng')
    } else {
      await usersApi.create(form.value)
      notify.success('Thành công', 'Đã tạo người dùng mới')
    }
    closeModal()
    loadUsers()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể lưu')
  } finally { saving.value = false }
}

async function toggleStatus(user) {
  try {
    await usersApi.toggleStatus(user.id)
    notify.success('Đã cập nhật', `Tài khoản ${user.fullName} đã ${user.status === 'Active' ? 'bị vô hiệu hóa' : 'được kích hoạt'}`)
    loadUsers()
  } catch { notify.error('Lỗi', 'Không thể cập nhật trạng thái') }
}

function confirmDelete(user) { deletingUser.value = user }

async function deleteUser() {
  try {
    await usersApi.delete(deletingUser.value.id)
    notify.success('Đã xóa', `Đã xóa tài khoản ${deletingUser.value.fullName}`)
    deletingUser.value = null
    loadUsers()
  } catch (err) {
    notify.error('Lỗi', err.response?.data?.message || 'Không thể xóa')
  }
}

onMounted(loadUsers)
</script>

<style scoped>
.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--primary-light);
  color: var(--primary);
  font-weight: 700;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  border: 1px solid var(--primary-border);
}
.btn-warning {
  background: var(--warning-light);
  color: var(--warning);
  border: 1px solid rgba(245,158,11,0.3);
}
.btn-warning:hover { background: rgba(245,158,11,0.2); }
</style>
