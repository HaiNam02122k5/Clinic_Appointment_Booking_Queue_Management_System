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
  // Route Public: Chọn Vai trò (Màn hình khởi đầu)
  {
    path: '/select-role',
    name: 'select-role',
    component: () => import('@/views/SelectRoleView.vue'),
    meta: { public: true },
  },
  // Route Public: Đăng nhập
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
    meta: { public: true },
  },

  // Route Public: Đăng ký
  {
    path: '/register',
    name: 'register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { public: true },
  },

  // --- Route Public: Màn hình TV Công cộng ---
  {
    path: '/public/queue-display',
    name: 'public-queue-display',
    component: () => import('@/views/NotFoundView.vue'), // Sẽ thay bằng màn hình TV công cộng ở Sprint 2
    meta: { public: true },
  },
  // --- Route Chức năng (Yêu cầu Đăng nhập & Layout chung) ---
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
      // {
      //   path: 'reception/queue',
      //   name: 'reception-queue',
      //   component: () => import('@/views/UsersView.vue'),
      //   meta: { roles: ['Receptionist', 'Admin'] },
      // },
      {
        path: 'reception/queue',
        name: 'reception-queue',
        component: () =>
          import('@/views/ReceptionistDashboardView.vue'),
        meta: {
          roles: ['Receptionist', 'Admin'],
        },
      },
      {
        path: 'reception/checkin',
        name: 'reception-checkin',
        component: () =>
          import('@/views/ReceptionistCheckinView.vue'),
        meta: {
          roles: ['Receptionist', 'Admin'],
        },
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
  // --- Catch-all 404 ---
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
    // Nếu đã đăng nhập mà cố vào login hoặc select-role -> chuyển về trang chủ
    if ((to.name === 'login' || to.name === 'select-role') && auth.isAuthenticated) {
      return { name: 'home' }
    }
    return true
  }

  // 2. Nếu chưa đăng nhập -> chuyển hướng về trang Chọn Vai Trò (select-role)
  if (!auth.isAuthenticated) {
    return { name: 'select-role', query: { redirect: to.fullPath } }
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
