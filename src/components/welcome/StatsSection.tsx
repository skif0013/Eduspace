'use client'

import { motion } from 'framer-motion'
import { useLanguage } from '@/contexts/LanguageContext'

export function StatsSection() {
  const { t } = useLanguage()

  const stats = [
    {
      icon: '⚡',
      value: '98%',
      label: t('stats.completion'),
      color: 'from-yellow-500 to-orange-500',
    },
    {
      icon: '🎓',
      value: '50+',
      label: t('stats.instructors'),
      color: 'from-blue-500 to-purple-500',
    },
    {
      icon: '🌍',
      value: '30+',
      label: t('stats.countries'),
      color: 'from-green-500 to-teal-500',
    },
    {
      icon: '⭐',
      value: '4.9',
      label: t('stats.rating'),
      color: 'from-pink-500 to-rose-500',
    },
  ]

  return (
    <section className="container mx-auto px-4 py-20">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true }}
        transition={{ duration: 0.6 }}
        className="text-center mb-12"
      >
        <h2 className="text-4xl font-bold mb-4 text-gray-900 dark:text-white">
          {t('stats.title')}
        </h2>
        <p className="text-xl text-gray-600 dark:text-gray-400">
          {t('stats.subtitle')}
        </p>
      </motion.div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => (
          <motion.div
            key={index}
            initial={{ opacity: 0, scale: 0.8 }}
            whileInView={{ opacity: 1, scale: 1 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.1, duration: 0.5 }}
            className="relative"
          >
            <div className="bg-white dark:bg-gray-800 rounded-2xl p-8 shadow-lg border border-gray-200 dark:border-gray-700 text-center">
              <div className={`w-16 h-16 mx-auto rounded-xl bg-gradient-to-br ${stat.color} flex items-center justify-center text-3xl mb-4`}>
                {stat.icon}
              </div>
              <div className="text-4xl font-bold mb-2 text-gray-900 dark:text-white">
                {stat.value}
              </div>
              <div className="text-gray-600 dark:text-gray-400">
                {stat.label}
              </div>
            </div>
          </motion.div>
        ))}
      </div>
    </section>
  )
}

