'use client'

import { motion } from 'framer-motion'
import { useLanguage } from '@/contexts/LanguageContext'

export function FeatureCards() {
  const { t } = useLanguage()

  const features = [
    {
      icon: '🎯',
      titleKey: 'features.quizzes.title' as const,
      descriptionKey: 'features.quizzes.description' as const,
      color: 'text-blue-600',
    },
    {
      icon: '📚',
      titleKey: 'features.createCourses.title' as const,
      descriptionKey: 'features.createCourses.description' as const,
      color: 'text-purple-600',
    },
    {
      icon: '🏆',
      titleKey: 'features.achievements.title' as const,
      descriptionKey: 'features.achievements.description' as const,
      color: 'text-orange-500',
    },
    {
      icon: '📊',
      titleKey: 'features.progress.title' as const,
      descriptionKey: 'features.progress.description' as const,
      color: 'text-green-600',
    },
  ]

  return (
    <section
      id="features"
      className="flex flex-col items-center justify-center min-h-screen px-4 py-20 text-center"
    >
      <motion.div
        initial={{ opacity: 0, y: 10 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true }}
        transition={{ duration: 0.5 }}
        className="mb-12 max-w-3xl"
      >
        <h2 className="text-3xl font-semibold mb-3 text-gray-900 dark:text-white">
          {t('features.title')}
        </h2>
        <p className="text-base text-gray-600 dark:text-gray-400">
          {t('features.subtitle')}
        </p>
      </motion.div>

      {/* Центрируем карточки */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 justify-center justify-items-center">
        {features.map((feature, index) => (
          <motion.div
            key={index}
            initial={{ opacity: 0, y: 12 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.06, duration: 0.45 }}
            whileHover={{ y: -4, transition: { duration: 0.12 } }}
            className="group"
          >
            <div className="h-full bg-white dark:bg-gray-900 rounded-lg p-6 shadow-sm transition-border border border-gray-200 dark:border-gray-800">
              <div
                className={`w-20 h-13 rounded-md bg-gray-100 dark:bg-gray-800 flex items-center justify-center text-xl mb-4 ${feature.color}`}
                aria-hidden
              >
                {feature.icon}
              </div>
              <h3 className="text-lg font-medium mb-2 text-gray-900 dark:text-white">
                {t(feature.titleKey)}
              </h3>
              <p className="text-sm text-gray-600 dark:text-gray-400 leading-relaxed">
                {t(feature.descriptionKey)}
              </p>
            </div>
          </motion.div>
        ))}
      </div>
    </section>
  )
}
