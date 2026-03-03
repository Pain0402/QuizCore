import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  // Auth
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: { guest: true },
  },

  // Main App (requires auth)
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/dashboard',
      },
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/DashboardView.vue'),
        meta: { title: 'Dashboard' },
      },
      {
        path: 'questions',
        name: 'Questions',
        component: () => import('@/views/questions/QuestionsView.vue'),
        meta: { title: 'Ngân hàng câu hỏi', requiresManage: true },
      },
      {
        path: 'exams',
        name: 'Exams',
        component: () => import('@/views/exams/ExamsView.vue'),
        meta: { title: 'Quản lý đề thi', requiresManage: true },
      },
    ],
  },

  // 404
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation Guards
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    return next({ name: 'Login', query: { redirect: to.fullPath } })
  }

  if (to.meta.guest && authStore.isLoggedIn) {
    return next({ name: 'Dashboard' })
  }

  if (to.meta.requiresManage && !authStore.canManage) {
    return next({ name: 'Dashboard' })
  }

  next()
})

export default router
