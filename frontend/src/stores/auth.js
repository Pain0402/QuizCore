import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/auth'

const TOKEN_KEY = 'quizcore_token'
const USER_KEY  = 'quizcore_user'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem(TOKEN_KEY) || null)
  const user  = ref(JSON.parse(localStorage.getItem(USER_KEY) || 'null'))

  const isLoggedIn = computed(() => !!token.value)
  const isAdmin    = computed(() => user.value?.role === 'Admin')
  const isTeacher  = computed(() => user.value?.role === 'Teacher')
  const isStudent  = computed(() => user.value?.role === 'Student')
  const canManage  = computed(() => isAdmin.value || isTeacher.value)

  async function login(credentials) {
    const { data } = await authApi.login(credentials)
    if (data.data?.token) {
      setSession(data.data.token, data.data.user)
    }
    return data
  }

  function setSession(newToken, newUser) {
    token.value = newToken
    user.value  = newUser
    localStorage.setItem(TOKEN_KEY, newToken)
    localStorage.setItem(USER_KEY, JSON.stringify(newUser))
  }

  function logout() {
    token.value = null
    user.value  = null
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
  }

  return { token, user, isLoggedIn, isAdmin, isTeacher, isStudent, canManage, login, logout, setSession }
})
