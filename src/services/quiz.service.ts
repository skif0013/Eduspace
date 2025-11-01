import { apiClient } from '@/lib/api-client'
import type { Quiz, QuizAttempt, ApiResponse, PaginatedResponse } from '@/types'

export const quizService = {
  /**
   * Get all quizzes with pagination
   */
  async getQuizzes(page = 1, pageSize = 10) {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    })

    return apiClient.get<PaginatedResponse<Quiz>>(`/quizzes?${params}`)
  },

  /**
   * Get a single quiz by ID
   */
  async getQuiz(id: string) {
    return apiClient.get<ApiResponse<Quiz>>(`/quizzes/${id}`)
  },

  /**
   * Create a new quiz
   */
  async createQuiz(data: Partial<Quiz>) {
    return apiClient.post<ApiResponse<Quiz>>('/quizzes', data)
  },

  /**
   * Submit quiz answers
   */
  async submitQuiz(quizId: string, answers: any[]) {
    return apiClient.post<ApiResponse<QuizAttempt>>(
      `/quizzes/${quizId}/submit`,
      { answers }
    )
  },

  /**
   * Get quiz attempts for a user
   */
  async getQuizAttempts(quizId: string) {
    return apiClient.get<ApiResponse<QuizAttempt[]>>(
      `/quizzes/${quizId}/attempts`
    )
  },
}

