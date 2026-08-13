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
    path: '/',
    redirect: '/select-role',
  },

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
        path: 'reception',
        meta: {
          roles: ['Receptionist']
        },
        children: [
          {
            path: '',
            name: 'reception-dashboard',
            component: () =>
              import('@/views/receptionist/ReceptionistDashboardView.vue')
          },
          {
            path: 'queue',
            name: 'reception-queue',
            component: () =>
              import('@/views/receptionist/ReceptionistQueueView.vue')
          },
          {
            path: 'checkin',
            name: 'reception-checkin',
            component: () =>
              import('@/views/receptionist/ReceptionistCheckinView.vue')
          }
        ]
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
        path: 'admin',
        name: 'admin-dashboard',
        component: () =>
          import('@/views/admin/AdminDashboardView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/accounts',
        name: 'admin-accounts',
        component: () =>
          import('@/views/admin/AdminAccountsView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/doctors',
        name: 'admin-doctors',
        component: () =>
          import('@/views/admin/AdminDoctorsView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/specialties',
        name: 'admin-specialties',
        component: () =>
          import('@/views/admin/AdminSpecialtiesView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/schedule',
        name: 'admin-schedule',
        component: () =>
          import('@/views/admin/AdminScheduleView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/reports',
        name: 'admin-reports',
        component: () =>
          import('@/views/admin/AdminReportsView.vue'),
        meta: {
          roles: ['Admin'],
        },
      },

      {
        path: 'admin/settings',
        name: 'admin-settings',
        component: () =>
          import('@/views/admin/AdminSettingsView.vue'),
        meta: {
          roles: ['Admin'],
        },
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
    // Nếu đã đăng nhập mà cố vào login hoặc select-role
    // thì chuyển về trang tương ứng với role
    if (
      (to.name === 'login' || to.name === 'select-role') &&
      auth.isAuthenticated
    ) {
      if (auth.hasRole(['Admin'])) {
        return { name: 'admin-dashboard' }
      }

      if (auth.hasRole(['Receptionist'])) {
        return { name: 'reception-dashboard' }
      }

      if (auth.hasRole(['Doctor'])) {
        return { name: 'doctor-examination' }
      }

      if (auth.hasRole(['Patient'])) {
        return { name: 'booking' }
      }

      return { name: 'home' }
    }

    return true
  }

  // 2. Nếu chưa đăng nhập
  if (!auth.isAuthenticated) {
    return {
      name: 'select-role',
      query: {
        redirect: to.fullPath,
      },
    }
  }

  // 3. Kiểm tra phân quyền
  const requiredRoles = to.meta.roles

  if (requiredRoles && requiredRoles.length > 0) {
    if (!auth.hasRole(requiredRoles)) {
      // Không đủ quyền → chuyển về trang riêng của role
      if (auth.hasRole(['Admin'])) {
        return { name: 'admin-dashboard' }
      }

      if (auth.hasRole(['Receptionist'])) {
        return { name: 'reception-dashboard' }
      }

      if (auth.hasRole(['Doctor'])) {
        return { name: 'doctor-examination' }
      }

      if (auth.hasRole(['Patient'])) {
        return { name: 'booking' }
      }

      return { name: 'select-role' }
    }
  }

  return true
})

export default router
