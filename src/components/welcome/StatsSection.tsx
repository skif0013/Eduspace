"use client";

import { motion } from "framer-motion";
import { useLanguage } from "@/contexts/LanguageContext";

export function StatsSection() {
  const { t } = useLanguage();

  const stats = [
    {
      icon: "⚡",
      value: "98%",
      label: t("stats.completion"),
      color: "from-yellow-500 to-orange-500",
    },
    {
      icon: "🎓",
      value: "50+",
      label: t("stats.instructors"),
      color: "from-blue-500 to-purple-500",
    },
    {
      icon: "🌍",
      value: "30+",
      label: t("stats.countries"),
      color: "from-green-500 to-teal-500",
    },
    {
      icon: "⭐",
      value: "4.9",
      label: t("stats.rating"),
      color: "from-pink-500 to-rose-500",
    },
  ];

  const subtitle = t("stats.subtitle");

  return (
    <section className="container mx-auto px-4 py-12">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true }}
        transition={{ duration: 0.6 }}
        className="text-center mb-8"
      >
        {t("stats.title") ? (
          <h2 className="text-2xl sm:text-3xl font-semibold mb-2 text-gray-900 dark:text-white">
            {t("stats.title")}
          </h2>
        ) : null}
        {subtitle ? (
          <p className="text-base sm:text-lg text-gray-600 dark:text-gray-400 max-w-2xl mx-auto">
            {subtitle}
          </p>
        ) : null}
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
        {stats.map((stat, index) => (
          <motion.div
            key={index}
            initial={{ opacity: 0, scale: 0.96 }}
            whileInView={{ opacity: 1, scale: 1 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.08, duration: 0.4 }}
            className="relative"
          >
            <div className="bg-white dark:bg-gray-800 rounded-2xl p-6 sm:p-8 shadow-sm border border-gray-200 dark:border-gray-700 text-center h-full flex flex-col items-center justify-center">
              <div
                className={`w-14 h-14 sm:w-16 sm:h-16 mx-auto rounded-xl bg-gradient-to-br ${stat.color} flex items-center justify-center text-2xl sm:text-3xl mb-3`}
              >
                {stat.icon}
              </div>
              <div className="text-2xl sm:text-3xl font-semibold mb-1 text-gray-900 dark:text-white">
                {stat.value}
              </div>
              <div className="text-sm sm:text-base text-gray-600 dark:text-gray-400">
                {stat.label}
              </div>
            </div>
          </motion.div>
        ))}
      </div>
    </section>
  );
}
