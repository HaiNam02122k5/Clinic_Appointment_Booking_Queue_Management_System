import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'
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
        path: 'patient',
        name: 'patient-home',
        component: () => import('@/views/patient/PatientHomeView.vue'),
        meta: { roles: ['Patient'] },
      },
      {
        path: 'patient/booking',
        name: 'patient-booking',
        component: () => import('@/views/patient/PatientBookingView.vue'),
        meta: { roles: ['Patient'] },
      },
      {
        path: 'patient/queue',
        name: 'patient-queue',
        component: () => import('@/views/patient/PatientQueueView.vue'),
        meta: { roles: ['Patient'] },
      },
      {
        path: 'patient/history',
        name: 'patient-history',
        component: () => import('@/views/patient/PatientHistoryView.vue'),
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
router.beforeEach(async (to) => {
  // Lazily import the auth store to avoid static circular imports at module load time.
  const mod = await import('@/stores/auth')
  const auth = mod.useAuthStore()

  // If the store is hydrating (fetching profile), wait a short time for it to finish so we don't redirect prematurely.
  if ((auth as any).hydrating) {
    await new Promise((resolve) => {
      const start = Date.now()
      const iv = setInterval(() => {
        if (!(auth as any).hydrating || Date.now() - start > 3000) {
          clearInterval(iv)
          resolve(true)
        }
      }, 50)
    })
  }

  if (to.path === '/' && !auth.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.public) {
    if (['login', 'select-role', 'register'].includes(to.name as string) && auth.isAuthenticated) {
      const userRoles = auth.user?.roles ?? []
      if (userRoles.length > 1 && !auth.currentUserRole) {
        return { name: 'select-role' }
      }
      return { name: 'home' }
    }
    return true
  }

  if (!auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (!auth.currentUserRole) {
    return { name: 'select-role', query: { redirect: to.fullPath } }
  }

  const requiredRoles = to.meta.roles
  if (requiredRoles && requiredRoles.length > 0) {
    if (!auth.hasRole(requiredRoles)) {
      return { name: 'home' }
    }
  }

  return true
})
export default router