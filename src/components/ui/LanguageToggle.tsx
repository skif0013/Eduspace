'use client'

import { motion } from 'framer-motion'
import { useLanguage } from '@/contexts/LanguageContext'
import { Language } from '@/locales/translations'

export function LanguageToggle() {
  const { language, setLanguage } = useLanguage()

  const toggleLanguage = () => {
    const newLanguage: Language = language === 'en' ? 'pl' : 'en'
    setLanguage(newLanguage)
  }

  return (
    <motion.button
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
      onClick={toggleLanguage}
      className="px-3 py-2 rounded-lg bg-gray-200 dark:bg-gray-800 hover:bg-gray-300 dark:hover:bg-gray-700 transition-colors font-semibold text-sm"
      aria-label="Toggle language"
    >
      {language === 'en' ? 'PL' : ' EN'}
    </motion.button>
  )
}

