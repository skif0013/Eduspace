'use client'

import { useLanguage } from '@/contexts/LanguageContext'

export function Footer() {
  const { t } = useLanguage()

  return (
    <footer className="mt-10 py-6 border-t border-gray-200 dark:border-gray-800">
      <div className="container mx-auto px-4 text-center text-gray-600 dark:text-gray-400">
        <p className="text-sm">{t('footer.rights')}</p>
      </div>
    </footer>
  )
}
