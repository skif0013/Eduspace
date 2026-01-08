import HomeView from '../views/HomeView.vue'
import Login from "../Auth/Login.vue"; // <-- Важно!

const router = createRouter({
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView
        },
        {
            path: '/login',
            name: 'login',
            component: Login
        }
    ]
})