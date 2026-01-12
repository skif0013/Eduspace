import { createRouter, createWebHistory } from 'vue-router'
import ProfileView from '../views/ProfileView.vue'
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
        },
        {
            path:'/profile',
            name: 'profile',
            component: ProfileView
        }
        ]
});

export default router