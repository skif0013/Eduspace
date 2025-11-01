'use client'

import { motion } from 'framer-motion'
import { ThemeToggle } from '@/components/ui/ThemeToggle'
import { LanguageToggle } from '@/components/ui/LanguageToggle'
import { useLanguage } from '@/contexts/LanguageContext'

export function Header() {
  const { t } = useLanguage()

  return (
    <motion.header
      initial={{ y: -100 }}
      animate={{ y: 0 }}
      transition={{ type: 'spring', stiffness: 100 }}
      className="fixed top-0 left-0 right-0 z-50 bg-white/80 dark:bg-gray-900/80 backdrop-blur-lg border-b border-gray-200 dark:border-gray-800"
    >
      <div className="container mx-auto px-4 py-2">
        <div className="flex items-center justify-between">
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.2 }}
            className="flex items-center space-x-2"
          >
            <div className="w-9 h-9 bg-gray-800 dark:bg-gray-200 rounded-lg flex items-center justify-center">
              <span className="text-white dark:text-gray-900 font-bold text-base">GC</span>
            </div>
            <h1 className="text-base sm:text-lg font-semibold text-gray-900 dark:text-white">
              {t('header.appName')}
            </h1>
          </motion.div>

          <motion.nav
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.2 }}
            className="flex items-center space-x-3"
          >
            <div className="hidden sm:flex items-center space-x-3">
              <a
                href="#features"
                className="text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400 transition-colors text-sm"
              >
                {t('header.features')}
              </a>
              <a
                href="#courses"
                className="text-gray-700 dark:text-gray-300 hover:text-primary-600 dark:hover:text-primary-400 transition-colors text-sm"
              >
                {t('header.courses')}
              </a>
            </div>

            <div className="flex items-center space-x-2 pr-2">
              <LanguageToggle />
              <ThemeToggle />
            </div>

            <a
              href="/signin"
              aria-label="Sign in"
              className="inline-flex items-center px-3 py-1.5 border border-transparent rounded-md text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 transition-colors"
            >
              Sign In
            </a>
          </motion.nav>
        </div>
      </div>
    </motion.header>
  )
}
