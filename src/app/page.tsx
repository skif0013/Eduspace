'use client'

import { motion } from 'framer-motion'
import { Header } from '@/components/layout/Header'
import { Footer } from '@/components/layout/Footer'
import { WelcomeHero } from '@/components/welcome/WelcomeHero'
import { FeatureCards } from '@/components/welcome/FeatureCards'
import { StatsSection } from '@/components/welcome/StatsSection'

export default function HomePage() {
  return (
    <div className="min-h-screen">
      <Header />

      <main className="pt-20">
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.5 }}
        >
          <WelcomeHero />
          <FeatureCards />
          <StatsSection />
        </motion.div>
      </main>

      <Footer />
    </div>
  )
}

