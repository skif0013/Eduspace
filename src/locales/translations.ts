export const translations = {
  en: {
    // Header
    'header.features': 'Features',
    'header.courses': 'Courses',
    'header.appName': 'Gamified Courses',

    // Hero Section
    'hero.badge': '🎮 Learn Through Gaming',
    'hero.title': 'Welcome!',
    'hero.subtitle': 'A gamified learning platform where every course is an exciting adventure',
    'hero.startLearning': 'Start Learning',
    'hero.learnMore': 'Learn More',
    'hero.courses': 'Courses',
    'hero.quizzes': 'Quizzes',
    'hero.students': 'Students',

    // Features
    'features.title': 'Platform Features',
    'features.subtitle': 'Everything you need for effective learning',
    'features.quizzes.title': 'Quizzes & Tests',
    'features.quizzes.description': 'Test your knowledge with interactive quizzes and earn rewards',
    'features.createCourses.title': 'Create Courses',
    'features.createCourses.description': 'Create your own courses and share knowledge with the community',
    'features.achievements.title': 'Achievements',
    'features.achievements.description': 'Earn badges, level up and compete with friends',
    'features.progress.title': 'Progress Tracking',
    'features.progress.description': 'Track your progress and statistics in real-time',

    // Stats (made neutral / concise per request)
    'stats.title': 'Statistics',
    'stats.subtitle': '',
    'stats.completion': 'Completion Rate',
    'stats.instructors': 'Expert Instructors',
    'stats.countries': 'Countries',
    'stats.rating': 'Average Rating',

    // Footer
    'footer.rights': '© 2025 Gamified Courses. All rights reserved.',
  },
  pl: {
    // Header
    'header.features': 'Funkcje',
    'header.courses': 'Kursy',
    'header.appName': 'Gamified Courses',

    // Hero Section
    'hero.badge': '🎮 Nauka przez grę',
    'hero.title': 'Witamy!',
    'hero.subtitle': 'Platforma gamifikowanej nauki, gdzie każdy kurs to ekscytująca przygoda',
    'hero.startLearning': 'Rozpocznij naukę',
    'hero.learnMore': 'Dowiedz się więcej',
    'hero.courses': 'Kursy',
    'hero.quizzes': 'Quizy',
    'hero.students': 'Studenci',

    // Features
    'features.title': 'Funkcje platformy',
    'features.subtitle': 'Wszystko czego potrzebujesz do efektywnej nauki',
    'features.quizzes.title': 'Quizy i Testy',
    'features.quizzes.description': 'Sprawdź swoją wiedzę za pomocą interaktywnych quizów i zdobywaj nagrody',
    'features.createCourses.title': 'Tworzenie Kursów',
    'features.createCourses.description': 'Twórz własne kursy i dziel się wiedzą ze społecznością',
    'features.achievements.title': 'Osiągnięcia',
    'features.achievements.description': 'Zdobywaj odznaki, awansuj poziomy i rywalizuj z przyjaciółmi',
    'features.progress.title': 'Śledzenie Postępów',
    'features.progress.description': 'Monitoruj swoje postępy i statystyki w czasie rzeczywistym',

    // Stats
    'stats.title': 'Statystyki',
    'stats.subtitle': '',
    'stats.completion': 'Wskaźnik ukończenia',
    'stats.instructors': 'Eksperci instruktorzy',
    'stats.countries': 'Kraje',
    'stats.rating': 'Średnia ocena',

    // Footer
    'footer.rights': '© 2025 Gamified Courses. Wszelkie prawa zastrzeżone.',
  },
}

export type Language = keyof typeof translations
export type TranslationKey = keyof typeof translations.en
