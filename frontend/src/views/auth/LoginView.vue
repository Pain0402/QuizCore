<template>
  <div class="auth-page">
    <div class="auth-card">
      <!-- Logo -->
      <div class="auth-logo">
        <div class="auth-logo-icon">🎯</div>
        <h1>QuizCore</h1>
        <p>Hệ thống thi trắc nghiệm trực tuyến</p>
      </div>

      <!-- Error alert -->
      <div v-if="errorMsg" class="alert alert-danger" style="margin-bottom:20px">
        <span>⚠️</span>
        <span>{{ errorMsg }}</span>
      </div>

      <!-- Form -->
      <form class="auth-form" @submit.prevent="handleLogin">
        <div class="form-group">
          <label class="form-label">Tên đăng nhập <span class="form-required">*</span></label>
          <input
            v-model="form.username"
            id="username"
            type="text"
            class="form-control"
            :class="{ 'is-error': errors.username }"
            placeholder="Nhập tên đăng nhập..."
            autocomplete="username"
          />
          <span v-if="errors.username" class="form-error">{{ errors.username }}</span>
        </div>

        <div class="form-group">
          <label class="form-label">Mật khẩu <span class="form-required">*</span></label>
          <input
            v-model="form.password"
            id="password"
            :type="showPassword ? 'text' : 'password'"
            class="form-control"
            :class="{ 'is-error': errors.password }"
            placeholder="Nhập mật khẩu..."
            autocomplete="current-password"
          />
          <span v-if="errors.password" class="form-error">{{ errors.password }}</span>
        </div>

        <div style="display:flex;align-items:center;gap:8px">
          <input type="checkbox" id="showPwd" v-model="showPassword" style="cursor:pointer;accent-color:var(--primary)" />
          <label for="showPwd" style="font-size:0.85rem;color:var(--text-secondary);cursor:pointer">Hiển thị mật khẩu</label>
        </div>

        <button type="submit" class="btn btn-primary auth-btn" :disabled="loading">
          <span v-if="loading" class="spinner"></span>
          {{ loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
        </button>
      </form>

      <p style="margin-top:24px;text-align:center;font-size:0.8rem">
        Tài khoản thử nghiệm: <code style="font-size:0.8rem">admin / Admin@123</code>
      </p>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'

const router = useRouter()
const route  = useRoute()
const authStore = useAuthStore()
const notify = useNotificationStore()

const form = reactive({ username: '', password: '' })
const errors = reactive({ username: '', password: '' })
const loading = ref(false)
const errorMsg = ref('')
const showPassword = ref(false)

function validate() {
  errors.username = form.username.trim() ? '' : 'Vui lòng nhập tên đăng nhập'
  errors.password = form.password       ? '' : 'Vui lòng nhập mật khẩu'
  return !errors.username && !errors.password
}

async function handleLogin() {
  errorMsg.value = ''
  if (!validate()) return

  loading.value = true
  try {
    await authStore.login({ username: form.username, password: form.password })
    notify.success('Đăng nhập thành công', `Chào mừng trở lại!`)
    const redirect = route.query.redirect || '/dashboard'
    router.push(redirect)
  } catch (err) {
    const msg = err.response?.data?.message || 'Tên đăng nhập hoặc mật khẩu không đúng'
    errorMsg.value = msg
  } finally {
    loading.value = false
  }
}
</script>
