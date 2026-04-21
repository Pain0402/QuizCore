import api from '@/api/axios'

export const examsApi = {
  getAll: (params) => api.get('/exams', { params }),
  getById: (id) => api.get(`/exams/${id}`),
  create: (data) => api.post('/exams', data),
  update: (id, data) => api.put(`/exams/${id}`, data),
  delete: (id) => api.delete(`/exams/${id}`),
  forceSubmit: (id, message) => api.post(`/exams/${id}/force-submit`, null, { params: { message } })
}
