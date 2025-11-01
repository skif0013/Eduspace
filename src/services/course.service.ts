import { apiClient } from '@/lib/api-client'
import type { Course, PaginatedResponse, ApiResponse } from '@/types'

export const courseService = {
  /**
   * Get all courses with pagination
   */
  async getCourses(page = 1, pageSize = 10, category?: string) {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      ...(category && { category }),
    })

    return apiClient.get<PaginatedResponse<Course>>(`/courses?${params}`)
  },

  /**
   * Get a single course by ID
   */
  async getCourse(id: string) {
    return apiClient.get<ApiResponse<Course>>(`/courses/${id}`)
  },

  /**
   * Create a new course
   */
  async createCourse(data: Partial<Course>) {
    return apiClient.post<ApiResponse<Course>>('/courses', data)
  },

  /**
   * Update a course
   */
  async updateCourse(id: string, data: Partial<Course>) {
    return apiClient.put<ApiResponse<Course>>(`/courses/${id}`, data)
  },

  /**
   * Delete a course
   */
  async deleteCourse(id: string) {
    return apiClient.delete<ApiResponse<void>>(`/courses/${id}`)
  },

  /**
   * Enroll in a course
   */
  async enrollCourse(courseId: string) {
    return apiClient.post<ApiResponse<void>>(`/courses/${courseId}/enroll`)
  },
}

