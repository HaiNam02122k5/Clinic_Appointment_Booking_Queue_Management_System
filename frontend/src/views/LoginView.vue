<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { UserRole } from '@/features/auth/auth.types'
import { validators } from '@/utils/validators'
import { ApiError } from '@/lib/api/http'
import { getFieldErrors } from '@/lib/api/error-utils'
import { logger } from '@/lib/logger'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const rememberMe = ref(false)

const errors = ref({
  email: '',
  password: '',
})

const LOGIN_THEME = {
  label: 'Clinic Queue',
  colorClass: 'from-[#0E4D92] to-[#1565c0]',
  badgeClass: 'bg-blue-50 text-blue-700 border-blue-200',
  hintEmail: 'name@clinic.com',
} as const

function getRoleRoute(role: UserRole) {
  if (role === 'Admin') return '/admin/doctors'
  if (role === 'Receptionist') return '/reception/queue'
  if (role === 'Doctor') return '/doctor/examination'
  return '/patient'
}

function validateLogin(): boolean {
  errors.value.email = ''
  errors.value.password = ''

  const usernameRequired = validators.required(email.value, 'Tên đăng nhập')
  if (!usernameRequired.isValid) {
    errors.value.email = usernameRequired.message
  }

  const passwordRequired = validators.required(password.value, 'Mật khẩu')
  if (!passwordRequired.isValid) {
    errors.value.password = passwordRequired.message
  } else {
    const passwordValidation = validators.password(password.value)
    if (!passwordValidation.isValid) {
      errors.value.password = passwordValidation.message
    }
  }

  return !errors.value.email && !errors.value.password
}

async function handleLogin() {
  if (!validateLogin()) return

  try {
    const loggedInUser = await authStore.login({
      username: email.value.trim(),
      email: email.value.trim(),
      password: password.value,
      rememberMe: rememberMe.value,
    })

    const redirectQuery = route.query.redirect as string | undefined
    const availableRoles = loggedInUser?.roles?.length ? loggedInUser.roles : [loggedInUser?.activeRole ?? loggedInUser?.role ?? 'Patient']

    if (availableRoles.length > 1) {
      router.replace({
        path: '/select-role',
        query: redirectQuery ? { redirect: redirectQuery } : undefined,
      })
      return
    }

    const activeRole =
      (loggedInUser?.activeRole ?? loggedInUser?.role ?? availableRoles[0] ?? 'Patient') as UserRole
    authStore.setActiveRole(activeRole)

    router.replace(redirectQuery || getRoleRoute(activeRole))
  } catch (e: any) {
    // Map API field errors into form fields when available
    let mapped = false
    try {
      const fe = getFieldErrors(e)
      if (fe) {
        errors.value.email = fe.email ? fe.email.join(' ') : errors.value.email
        errors.value.password = fe.password ? fe.password.join(' ') : errors.value.password
        mapped = true
      }
    } catch (mapErr) {
      // ignore mapping errors
    }

    // If backend only returned a global error, show it in the top banner and near the username field
    if (!mapped) {
      const global = authStore.error || (e?.response?.data?.errorMessages && e.response.data.errorMessages.join(' ')) || (e?.message)
      if (global) {
        errors.value.email = errors.value.email || global
      }
    }

    logger.error('Login failed', e)
  }
}

function goBackToRoleSelect() {
  router.push('/select-role')
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex font-sans">
    <!-- Left Panel (Hiển thị trên màn hình Laptop/Desktop) -->
    <div
      :class="[
        'hidden lg:flex lg:w-2/5 bg-gradient-to-br flex-col justify-between p-10 transition-colors duration-300',
        LOGIN_THEME.colorClass
      ]"
    >
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 bg-white/20 rounded-xl flex items-center justify-center text-white font-bold text-lg">
          +
        </div>
        <span class="font-bold text-white text-base">{{ LOGIN_THEME.label }}</span>
      </div>

      <div>
        <p class="text-white/70 text-sm font-medium uppercase tracking-widest mb-3">
          Chào mừng
        </p>
        <h2 class="text-4xl font-bold text-white leading-tight mb-4">
          Đăng nhập<br />để tiếp tục 👋
        </h2>
        <p class="text-white/70 text-base leading-relaxed">
          Sau khi xác thực thành công, hệ thống sẽ giúp bạn chọn quyền truy cập phù hợp với công việc bạn đang thực hiện.
        </p>

        <div class="mt-8 space-y-3">
          <div v-for="feature in ['Bảo mật dữ liệu', 'Phân quyền theo chức năng', 'Hỗ trợ nhiều quy trình']" :key="feature" class="flex items-center gap-3 text-sm text-white/80">
            <span class="text-white">✓</span>
            {{ feature }}
          </div>
        </div>
      </div>

      <p class="text-white/40 text-xs">© 2026 ClinicQueue</p>
    </div>

    <!-- Right Panel (Form Đăng nhập) -->
    <div class="flex-1 flex flex-col items-center justify-center px-5 py-10">
      <div class="w-full max-w-md">
        <!-- Logo Mobile -->
        <div class="flex items-center gap-2.5 mb-8 lg:hidden">
          <div class="w-8 h-8 bg-[#0E4D92] rounded-xl flex items-center justify-center text-white font-bold text-lg">
            +
          </div>
          <span class="font-bold text-slate-800 text-base">{{ LOGIN_THEME.label }}</span>
        </div>

        <div class="mb-8">
          <h1 class="text-2xl font-bold text-slate-800 mb-1">Đăng nhập</h1>
          <p class="text-sm text-slate-500">Đăng nhập để sử dụng các chức năng của hệ thống.</p>
        </div>

        <!-- Thông báo Lỗi -->
        <div v-if="authStore.error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-xs text-red-600">
          ⚠️ {{ authStore.error }}
        </div>

        <form @submit.prevent="handleLogin" class="space-y-4 mb-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1.5">
              Tên đăng nhập / Email <span class="text-red-500">*</span>
            </label>
            <input
              v-model="email"
              type="text"
              required
              :placeholder="LOGIN_THEME.hintEmail"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92] bg-white transition-all"
            />
            <p
              v-if="errors.email"
              class="mt-1 text-xs text-red-500"
            >
              ⚠️ {{ errors.email }}
            </p>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1.5">
              Mật khẩu <span class="text-red-500">*</span>
            </label>
            <input
              v-model="password"
              type="password"
              required
              placeholder="••••••••"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92] bg-white transition-all"
            />
            <p
              v-if="errors.password"
              class="mt-1 text-xs text-red-500"
            >
              ⚠️ {{ errors.password }}
            </p>
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-start gap-2 text-sm text-slate-600 cursor-pointer select-none">
              <input v-model="rememberMe" type="checkbox" class="mt-1 rounded text-[#0E4D92]" />
              <span>
                <span class="block font-medium text-slate-700">Ghi nhớ đăng nhập</span>
                <span class="block text-xs text-slate-500">Lưu phiên đăng nhập trên thiết bị này nếu bạn muốn truy cập lại nhanh.</span>
              </span>
            </label>
            <button type="button" class="text-sm text-[#0E4D92] font-medium hover:underline">
              Quên mật khẩu?
            </button>
          </div>

          <button
            type="submit"
            :disabled="authStore.status === 'loading'"
            class="w-full bg-[#0E4D92] text-white font-semibold rounded-xl px-6 py-3.5 text-base hover:bg-[#0b3d75] focus:outline-none focus:ring-2 focus:ring-[#0E4D92] shadow-sm transition-all active:scale-[0.98] disabled:opacity-50"
          >
            <span v-if="authStore.status === 'loading'" class="flex items-center justify-center gap-2">
              <span class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              Đang xử lý…
            </span>
            <span v-else>Đăng nhập</span>
          </button>

          <p class="text-center text-sm text-slate-500 mt-4">
            Chưa có tài khoản?
            <router-link to="/register" class="text-[#0E4D92] font-semibold hover:underline">
              Đăng ký ngay
            </router-link>
          </p>
        </form>

        <p class="text-xs text-slate-400 text-center mt-5">
          Gặp vấn đề? Liên hệ <span class="text-[#0E4D92]">hotro@phongkham.vn</span>
        </p>
      </div>
    </div>
  </div>
</template>
