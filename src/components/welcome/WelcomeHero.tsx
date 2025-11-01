'use client'

import { motion } from 'framer-motion'
import { useLanguage } from '@/contexts/LanguageContext'

export function WelcomeHero() {
  const { t } = useLanguage()

  return (
    <section className="flex items-center justify-center min-h-screen px-4">
      <div className="max-w-3xl w-full text-center">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6 }}
        >
          <motion.div
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            transition={{ type: 'spring', stiffness: 200, delay: 0.2 }}
            className="inline-block mb-4 px-3 py-1 bg-primary-100 dark:bg-primary-900/20 rounded-full"
          >
            <span className="text-primary-600 dark:text-primary-400 font-medium text-sm">
              {t('hero.badge')}
            </span>
          </motion.div>

          <h1 className="text-3xl sm:text-5xl md:text-6xl font-bold mb-4 text-gray-900 dark:text-white leading-tight">
            {t('hero.title')}
          </h1>

          <p className="text-lg sm:text-xl text-gray-600 dark:text-gray-400 mb-6">
            {t('hero.subtitle')}
          </p>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4 }}
            className="flex flex-col sm:flex-row gap-3 justify-center items-center"
          >
            <motion.button
              whileHover={{ scale: 1.03 }}
              whileTap={{ scale: 0.98 }}
              className="px-6 py-3 bg-primary-600 text-white font-semibold rounded-lg shadow-sm hover:shadow transition-all"
            >
              {t('hero.startLearning')}
            </motion.button>
            <motion.button
              whileHover={{ scale: 1.03 }}
              whileTap={{ scale: 0.98 }}
              className="px-6 py-3 bg-white dark:bg-gray-800 text-gray-900 dark:text-white font-semibold rounded-lg border border-gray-200 dark:border-gray-700 hover:border-primary-600 dark:hover:border-primary-400 transition-all"
            >
              {t('hero.learnMore')}
            </motion.button>
          </motion.div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, scale: 0.98 }}
          whileInView={{ opacity: 1, scale: 1 }}
          transition={{ delay: 0.6, duration: 0.6 }}
          className="mt-12 w-full"
        >
          <div className="relative">
            <div className="relative bg-white dark:bg-gray-800 rounded-2xl shadow-sm p-6 sm:p-8 border border-gray-200 dark:border-gray-700">
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div className="text-center">
                  <div className="text-2xl sm:text-3xl font-bold text-primary-600 dark:text-primary-400 mb-1">
                    10
                  </div>
                  <div className="text-sm sm:text-base text-gray-600 dark:text-gray-400">
                    {t('hero.courses')}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-2xl sm:text-3xl font-bold text-secondary-600 dark:text-secondary-400 mb-1">
                    10
                  </div>
                  <div className="text-sm sm:text-base text-gray-600 dark:text-gray-400">
                    {t('hero.quizzes')}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-2xl sm:text-3xl font-bold text-primary-600 dark:text-primary-400 mb-1">
                    10
                  </div>
                  <div className="text-sm sm:text-base text-gray-600 dark:text-gray-400">
                    {t('hero.students')}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </motion.div>
      </div>
    </section>
  )
}
