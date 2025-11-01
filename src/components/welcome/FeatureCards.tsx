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
      color: 'from-blue-500 to-cyan-500',
    },
    {
      icon: '📚',
      titleKey: 'features.createCourses.title' as const,
      descriptionKey: 'features.createCourses.description' as const,
      color: 'from-purple-500 to-pink-500',
    },
    {
      icon: '🏆',
      titleKey: 'features.achievements.title' as const,
      descriptionKey: 'features.achievements.description' as const,
      color: 'from-orange-500 to-red-500',
    },
    {
      icon: '📊',
      titleKey: 'features.progress.title' as const,
      descriptionKey: 'features.progress.description' as const,
      color: 'from-green-500 to-emerald-500',
    },
  ]

  return (
    <section id="features" className="container mx-auto px-4 py-20">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true }}
        transition={{ duration: 0.6 }}
        className="text-center mb-12"
      >
        <h2 className="text-4xl font-bold mb-4 text-gray-900 dark:text-white">
          {t('features.title')}
        </h2>
        <p className="text-xl text-gray-600 dark:text-gray-400">
          {t('features.subtitle')}
        </p>
      </motion.div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {features.map((feature, index) => (
          <motion.div
            key={index}
            initial={{ opacity: 0, y: 30 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.1, duration: 0.5 }}
            whileHover={{ y: -10, transition: { duration: 0.2 } }}
            className="group"
          >
            <div className="h-full bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all border border-gray-200 dark:border-gray-700">
              <div className={`w-16 h-16 rounded-xl bg-gradient-to-br ${feature.color} flex items-center justify-center text-3xl mb-4 group-hover:scale-110 transition-transform`}>
                {feature.icon}
              </div>
              <h3 className="text-xl font-bold mb-2 text-gray-900 dark:text-white">
                {t(feature.titleKey)}
              </h3>
              <p className="text-gray-600 dark:text-gray-400">
                {t(feature.descriptionKey)}
              </p>
            </div>
          </motion.div>
        ))}
      </div>
    </section>
  )
}

