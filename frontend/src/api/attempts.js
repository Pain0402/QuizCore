import api from '@/api/axios'

export const attemptsApi = {
  startExam: (examId) => api.post(`/attempts/exams/${examId}/start`),
  autoSave: (attemptId, data) => api.put(`/attempts/${attemptId}/autosave`, data),
  getProgress: (attemptId) => api.get(`/attempts/${attemptId}/progress`),
  submit: (attemptId) => api.post(`/attempts/${attemptId}/submit`),
  getResult: (attemptId) => api.get(`/attempts/${attemptId}/result`),
}
