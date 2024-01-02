import { createRouter, createWebHashHistory } from 'vue-router'
import HomeView from '../HomePage.vue';
import LoginView from '../LoginUser.vue';

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/Login',
    name: 'login',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: LoginView
  },
  {
    path: '/Logout',
    name: 'logout',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: LoginView,
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

export default router
