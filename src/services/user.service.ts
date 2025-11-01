import { apiClient } from '@/lib/api-client'
import type { User, ApiResponse } from '@/types'

export const userService = {
  /**
   * Get current user profile
   */
  async getCurrentUser() {
    return apiClient.get<ApiResponse<User>>('/users/me')
  },

  /**
   * Get user by ID
   */
  async getUser(id: string) {
    return apiClient.get<ApiResponse<User>>(`/users/${id}`)
  },

  /**
   * Update user profile
   */
  async updateUser(id: string, data: Partial<User>) {
    return apiClient.put<ApiResponse<User>>(`/users/${id}`, data)
  },

  /**
   * Login user
   */
  async login(email: string, password: string) {
    return apiClient.post<ApiResponse<{ user: User; token: string }>>(
      '/auth/login',
      { email, password }
    )
  },

  /**
   * Register new user
   */
  async register(data: { email: string; username: string; password: string }) {
    return apiClient.post<ApiResponse<{ user: User; token: string }>>(
      '/auth/register',
      data
    )
  },

  /**
   * Logout user
   */
  async logout() {
    return apiClient.post<ApiResponse<void>>('/auth/logout')
  },
}

