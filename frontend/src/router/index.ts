import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'
import type { UserRole } from '@/features/auth/auth.types'

declare module 'vue-router' {
  interface RouteMeta {
    public?: boolean
    roles?: UserRole[]
  }
}

const routes: RouteRecordRaw[] = [
  // Root
  {
    path: '/',
    redirect: '/select-role',
  },

  // Route public: Chọn vai trò
  {
    path: '/select-role',
    name: 'select-role',
    component: () => import('@/views/SelectRoleView.vue'),
    meta: { public: true },
  },

  // Route public: Đăng nhập
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
    meta: { public: true },
  },

  // Route public: Đăng ký
  {
    path: '/register',
    name: 'register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { public: true },
  },

  // Route public: Màn hình hiển thị hàng đợi public (màn hình TV)
  {
    path: '/public/queue-display',
    name: 'public-queue-display',
    component: () => import('@/views/NotFoundView.vue'),
    meta: { public: true },
  },

  // Route chức năng (yêu cầu đăng nhập, dùng AppLayout)
  {
    path: '/',
    component: AppLayout,

    children: [
      // Luồng bệnh nhân
      {
        path: 'patient',
        name: 'patient-home',
        component: () => import('@/views/patient/PatientHomeView.vue'),
        meta: {
          roles: ['Patient'],
        },
      },
      {
        path: 'patient/booking',
        name: 'patient-booking',
        component: () => import('@/views/patient/PatientBookingView.vue'),
        meta: {
          roles: ['Patient'],
        },
      },
      {
        path: 'patient/queue',
        name: 'patient-queue',
        component: () => import('@/views/patient/PatientQueueView.vue'),
        meta: {
          roles: ['Patient'],
        },
      },
      {
        path: 'patient/history',
        name: 'patient-history',
        component: () => import('@/views/patient/PatientHistoryView.vue'),
        meta: {
          roles: ['Patient'],
        },
      },

      // Luồng lễ tân
      {
        path: 'reception',
        meta: {
          roles: ['Receptionist'],
        },
        children: [
          {
            path: '',
            name: 'reception-dashboard',
            component: () =>
              import('@/views/receptionist/ReceptionistDashboardView.vue'),
          },
          {
            path: 'queue',
            name: 'reception-queue',
            component: () =>
              import('@/views/receptionist/ReceptionistQueueView.vue'),
          },
          {
            path: 'checkin',
            name: 'reception-checkin',
            component: () =>
              import('@/views/receptionist/ReceptionistCheckinView.vue'),
          },
        ],
      },

      // Luồng bác sĩ
      {
        path: 'doctor/examination',
        name: 'doctor-examination',
        component: () => import('@/views/UsersView.vue'),
        meta: {
          roles: ['Doctor'],
        },
      },

      // Luồng Admin
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

  // Catch-all 404
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
    meta: {
      public: true,
    },
  },
]

// Tạo Router
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

// Global Navigation Guard
router.beforeEach(async (to) => {
  // Import auth store trong guard để tránh circular import
  const mod = await import('@/stores/auth')
  const auth = mod.useAuthStore()

  // Chờ auth store hydrate tối đa 3 giây
  if ((auth as any).hydrating) {
    await new Promise((resolve) => {
      const start = Date.now()

      const interval = setInterval(() => {
        if (
          !(auth as any).hydrating ||
          Date.now() - start > 3000
        ) {
          clearInterval(interval)
          resolve(true)
        }
      }, 50)
    })
  }

  // Chuyển hướng trang root "/" khi chưa đăng nhập
  if (to.path === '/' && !auth.isAuthenticated) {
    return {
      name: 'login',
    }
  }

  // Xử lý route public
  if (to.meta.public) {
    // Chuyển hướng người dùng đã đăng nhập khỏi các trang public không cần thiết
    if (
      ['login', 'select-role', 'register'].includes(
        to.name as string
      ) &&
      auth.isAuthenticated
    ) {
      const userRoles = auth.user?.roles ?? []

      // Trả về màn hình chọn role nếu user chưa chọn role hiện tại
      if (
        userRoles.length > 1 &&
        !auth.currentUserRole
      ) {
        return {
          name: 'select-role',
        }
      }

      // Điều hướng về trang theo role
      if (auth.hasRole(['Admin'])) {
        return {
          name: 'admin-dashboard',
        }
      }

      if (auth.hasRole(['Receptionist'])) {
        return {
          name: 'reception-dashboard',
        }
      }

      if (auth.hasRole(['Doctor'])) {
        return {
          name: 'doctor-examination',
        }
      }

      if (auth.hasRole(['Patient'])) {
        return {
          name: 'patient-home',
        }
      }
    }

    return true
  }

  // Yêu cầu đăng nhập nếu truy cập route protected
  if (!auth.isAuthenticated) {
    return {
      name: 'login',
      query: {
        redirect: to.fullPath,
      },
    }
  }

  // Bắt buộc chọn role nếu tài khoản chưa xác định currentUserRole
  if (!auth.currentUserRole) {
    return {
      name: 'select-role',
      query: {
        redirect: to.fullPath,
      },
    }
  }

  // Kiểm tra phân quyền RBAC
  const requiredRoles = to.meta.roles

  if (
    requiredRoles &&
    requiredRoles.length > 0
  ) {
    // Điều hướng về trang thuộc role hiện tại nếu không đủ quyền truy cập
    if (!auth.hasRole(requiredRoles)) {
      if (auth.hasRole(['Admin'])) {
        return {
          name: 'admin-dashboard',
        }
      }

      if (auth.hasRole(['Receptionist'])) {
        return {
          name: 'reception-dashboard',
        }
      }

      if (auth.hasRole(['Doctor'])) {
        return {
          name: 'doctor-examination',
        }
      }

      if (auth.hasRole(['Patient'])) {
        return {
          name: 'patient-home',
        }
      }

      return {
        name: 'select-role',
      }
    }
  }

  // Cho phép chuyển trang khi tất cả điều kiện đều hợp lệ
  return true
})

export default router