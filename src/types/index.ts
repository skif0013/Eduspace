// User types
export interface User {
  id: string
  email: string
  username: string
  avatar?: string
  level: number
  experience: number
  totalPoints: number
  createdAt: string
  updatedAt: string
}

// Course types
export interface Course {
  id: string
  title: string
  description: string
  thumbnail?: string
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  category: string
  authorId: string
  author?: User
  enrolledCount: number
  rating: number
  duration: number // in minutes
  lessons: Lesson[]
  createdAt: string
  updatedAt: string
}

export interface Lesson {
  id: string
  courseId: string
  title: string
  content: string
  order: number
  duration: number
  videoUrl?: string
  quiz?: Quiz
}

// Quiz types
export interface Quiz {
  id: string
  title: string
  description?: string
  lessonId?: string
  courseId?: string
  questions: Question[]
  timeLimit?: number // in seconds
  passingScore: number
  points: number
  createdAt: string
}

export interface Question {
  id: string
  quizId: string
  text: string
  type: 'multiple-choice' | 'true-false' | 'text'
  options?: string[]
  correctAnswer: string | string[]
  points: number
  explanation?: string
  order: number
}

export interface QuizAttempt {
  id: string
  userId: string
  quizId: string
  score: number
  answers: Answer[]
  completedAt: string
  timeSpent: number
}

export interface Answer {
  questionId: string
  userAnswer: string | string[]
  isCorrect: boolean
  pointsEarned: number
}

// Achievement types
export interface Achievement {
  id: string
  title: string
  description: string
  icon: string
  type: 'course' | 'quiz' | 'level' | 'streak' | 'special'
  requirement: number
  points: number
}

export interface UserAchievement {
  id: string
  userId: string
  achievementId: string
  achievement?: Achievement
  earnedAt: string
}

// Progress types
export interface CourseProgress {
  id: string
  userId: string
  courseId: string
  course?: Course
  completedLessons: string[]
  currentLessonId?: string
  progress: number // 0-100
  lastAccessedAt: string
  completedAt?: string
}

// Leaderboard types
export interface LeaderboardEntry {
  rank: number
  user: User
  points: number
  coursesCompleted: number
  quizzesCompleted: number
}

// API Response types
export interface ApiResponse<T> {
  data: T
  message?: string
  success: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

