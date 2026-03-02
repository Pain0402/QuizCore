import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useNotificationStore = defineStore('notification', () => {
  const toasts = ref([])
  let nextId = 0

  function add({ type = 'info', title, message, duration = 4000 }) {
    const id = ++nextId
    toasts.value.push({ id, type, title, message, exiting: false })

    if (duration > 0) {
      setTimeout(() => remove(id), duration)
    }
    return id
  }

  function remove(id) {
    const toast = toasts.value.find(t => t.id === id)
    if (toast) {
      toast.exiting = true
      setTimeout(() => {
        toasts.value = toasts.value.filter(t => t.id !== id)
      }, 250)
    }
  }

  const success = (title, message) => add({ type: 'success', title, message })
  const error   = (title, message) => add({ type: 'danger',  title, message })
  const warn    = (title, message) => add({ type: 'warning', title, message })
  const info    = (title, message) => add({ type: 'info',    title, message })

  return { toasts, add, remove, success, error, warn, info }
})
