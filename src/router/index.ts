import { createRouter, createWebHistory } from 'vue-router'

import HeroSection from '../components/HeroSection.vue'
import LoginView from '../Auth/LoginView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HeroSection
        },
        {
            path: '/login',
            name: 'login',
            component: LoginView
        }
    ]
})

export default router