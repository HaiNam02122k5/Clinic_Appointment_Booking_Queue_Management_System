import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'
import { useAuthStore } from '@/stores/auth'
import type { UserRole } from '@/features/auth/auth.types'

// Mở rộng kiểu dữ liệu RouteMeta cho TypeScript
declare module 'vue-router' {
  interface RouteMeta {
    public?: boolean
    roles?: UserRole[]
  }
}

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
    meta: { public: true },
  },
  {
    path: '/public/queue-display',
    name: 'public-queue-display',
    component: () => import('@/views/NotFoundView.vue'), // Sẽ thay bằng màn hình TV công cộng ở Sprint 2
    meta: { public: true },
  },
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/views/HomeView.vue'),
      },
      // --- Luồng Bệnh nhân ---
      {
        path: 'booking',
        name: 'booking',
        component: () => import('@/views/HomeView.vue'),
        meta: { roles: ['Patient'] },
      },
      {
        path: 'my-appointments',
        name: 'my-appointments',
        component: () => import('@/views/HomeView.vue'),
        meta: { roles: ['Patient'] },
      },
      // --- Luồng Lễ tân Điều phối ---
      {
        path: 'reception/queue',
        name: 'reception-queue',
        component: () => import('@/views/UsersView.vue'),
        meta: { roles: ['Receptionist', 'Admin'] },
      },
      // --- Luồng Bác sĩ Khám bệnh ---
      {
        path: 'doctor/examination',
        name: 'doctor-examination',
        component: () => import('@/views/UsersView.vue'),
        meta: { roles: ['Doctor'] },
      },
      // --- Luồng Quản trị Admin ---
      {
        path: 'admin/doctors',
        name: 'admin-doctors',
        component: () => import('@/views/UsersView.vue'),
        meta: { roles: ['Admin'] },
      },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
    meta: { public: true },
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

// Global Guard: Kiểm tra Xác thực & Phân quyền (RBAC)
router.beforeEach((to) => {
  const auth = useAuthStore()

  // 1. Nếu là trang Public
  if (to.meta.public) {
    if (to.name === 'login' && auth.isAuthenticated) {
      return { name: 'home' }
    }
    return true
  }

  // 2. Nếu chưa đăng nhập
  if (!auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  // 3. Kiểm tra Phân quyền theo Role
  const requiredRoles = to.meta.roles
  if (requiredRoles && requiredRoles.length > 0) {
    if (!auth.hasRole(requiredRoles)) {
      return { name: 'home' } // Không đủ quyền thì về trang chủ
    }
  }

  return true
})

export default router