import api from '@/api/axios'

export const classesApi = {
  getAll: () => api.get('/classes'),
  getById: (id) => api.get(`/classes/${id}`),
  create: (data) => api.post('/classes', data),
  update: (id, data) => api.put(`/classes/${id}`, data),
  delete: (id) => api.delete(`/classes/${id}`),
  getStudents: (id) => api.get(`/classes/${id}/students`),
  assignStudents: (id, userIds) => api.post(`/classes/${id}/students`, { userIds }),
  removeStudent: (classId, userId) => api.delete(`/classes/${classId}/students/${userId}`),
}
