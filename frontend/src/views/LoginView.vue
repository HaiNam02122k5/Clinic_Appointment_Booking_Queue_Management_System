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

  const emailRequired = validators.required(email.value, 'Email')
  if (!emailRequired.isValid) {
    errors.value.email = emailRequired.message
  } else {
    const emailValidation = validators.email(email.value)
    if (!emailValidation.isValid) {
      errors.value.email = emailValidation.message
    }
  }

  const passwordRequired = validators.required(password.value, 'Máº­t kháº©u')
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
      email: email.value,
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
    try {
      const fe = getFieldErrors(e)
      if (fe) {
        errors.value.email = fe.email ? fe.email.join(' ') : errors.value.email
        errors.value.password = fe.password ? fe.password.join(' ') : errors.value.password
      }
    } catch (mapErr) {
      // ignore mapping errors
    }
    // Log for debugging; authStore.error will contain user-facing message
    logger.error('Login failed', e)
  }
}

function goBackToRoleSelect() {
  router.push('/select-role')
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex font-sans">
    <!-- Left Panel (Hiá»ƒn thá»‹ trÃªn mÃ n hÃ¬nh Laptop/Desktop) -->
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
          ChÃ o má»«ng
        </p>
        <h2 class="text-4xl font-bold text-white leading-tight mb-4">
          ÄÄƒng nháº­p<br />Ä‘á»ƒ tiáº¿p tá»¥c ðŸ‘‹
        </h2>
        <p class="text-white/70 text-base leading-relaxed">
          Sau khi xÃ¡c thá»±c thÃ nh cÃ´ng, há»‡ thá»‘ng sáº½ giÃºp báº¡n chá»n quyá»n truy cáº­p phÃ¹ há»£p vá»›i cÃ´ng viá»‡c báº¡n Ä‘ang thá»±c hiá»‡n.
        </p>

        <div class="mt-8 space-y-3">
          <div v-for="feature in ['Báº£o máº­t dá»¯ liá»‡u', 'PhÃ¢n quyá»n theo chá»©c nÄƒng', 'Há»— trá»£ nhiá»u quy trÃ¬nh']" :key="feature" class="flex items-center gap-3 text-sm text-white/80">
            <span class="text-white">âœ“</span>
            {{ feature }}
          </div>
        </div>
      </div>

      <p class="text-white/40 text-xs">Â© 2026 ClinicQueue</p>
    </div>

    <!-- Right Panel (Form ÄÄƒng nháº­p) -->
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
          <h1 class="text-2xl font-bold text-slate-800 mb-1">ÄÄƒng nháº­p</h1>
          <p class="text-sm text-slate-500">ÄÄƒng nháº­p Ä‘á»ƒ sá»­ dá»¥ng cÃ¡c chá»©c nÄƒng cá»§a há»‡ thá»‘ng.</p>
        </div>

        <!-- ThÃ´ng bÃ¡o Lá»—i -->
        <div v-if="authStore.error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-xs text-red-600">
          âš ï¸ {{ authStore.error }}
        </div>

        <form @submit.prevent="handleLogin" class="space-y-4 mb-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1.5">
              Email / TÃªn Ä‘Äƒng nháº­p <span class="text-red-500">*</span>
            </label>
            <input
              v-model="email"
              type="email"
              required
              :placeholder="LOGIN_THEME.hintEmail"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92] bg-white transition-all"
            />
            <p
              v-if="errors.email"
              class="mt-1 text-xs text-red-500"
            >
              âš ï¸ {{ errors.email }}
            </p>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1.5">
              Máº­t kháº©u <span class="text-red-500">*</span>
            </label>
            <input
              v-model="password"
              type="password"
              required
              placeholder="â€¢â€¢â€¢â€¢â€¢â€¢â€¢â€¢"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92] bg-white transition-all"
            />
            <p
              v-if="errors.password"
              class="mt-1 text-xs text-red-500"
            >
              âš ï¸ {{ errors.password }}
            </p>
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-center gap-2 text-sm text-slate-600 cursor-pointer select-none">
              <input v-model="rememberMe" type="checkbox" class="rounded text-[#0E4D92]" />
              Ghi nhá»› Ä‘Äƒng nháº­p
            </label>
            <button type="button" class="text-sm text-[#0E4D92] font-medium hover:underline">
              QuÃªn máº­t kháº©u?
            </button>
          </div>

          <button
            type="submit"
            :disabled="authStore.status === 'loading'"
            class="w-full bg-[#0E4D92] text-white font-semibold rounded-xl px-6 py-3.5 text-base hover:bg-[#0b3d75] focus:outline-none focus:ring-2 focus:ring-[#0E4D92] shadow-sm transition-all active:scale-[0.98] disabled:opacity-50"
          >
            <span v-if="authStore.status === 'loading'" class="flex items-center justify-center gap-2">
              <span class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              Äang xá»­ lÃ½â€¦
            </span>
            <span v-else>ÄÄƒng nháº­p</span>
          </button>

          <p class="text-center text-sm text-slate-500 mt-4">
            ChÆ°a cÃ³ tÃ i khoáº£n?
            <router-link to="/register" class="text-[#0E4D92] font-semibold hover:underline">
              ÄÄƒng kÃ½ ngay
            </router-link>
          </p>
        </form>

        <p class="text-xs text-slate-400 text-center mt-5">
          Gáº·p váº¥n Ä‘á»? LiÃªn há»‡ <span class="text-[#0E4D92]">hotro@phongkham.vn</span>
        </p>
   
      </div>
    </div>
  </div>
</template>
