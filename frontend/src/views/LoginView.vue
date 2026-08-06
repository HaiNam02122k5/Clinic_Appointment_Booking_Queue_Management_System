<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { UserRole } from '@/features/auth/auth.types'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const selectedRole = ref<UserRole>('Patient')
const email = ref('')
const password = ref('password')
const rememberMe = ref(false)

const ROLE_META = {
  Patient: {
    label: 'Bệnh nhân',
    colorClass: 'from-[#0E4D92] to-[#1565c0]',
    badgeClass: 'bg-blue-50 text-blue-700 border-blue-200',
    hintEmail: 'patient@clinic.com',
  },
  Receptionist: {
    label: 'Lễ tân',
    colorClass: 'from-violet-600 to-violet-800',
    badgeClass: 'bg-violet-50 text-violet-700 border-violet-200',
    hintEmail: 'receptionist@clinic.com',
  },
  Doctor: {
    label: 'Bác sĩ',
    colorClass: 'from-emerald-600 to-emerald-800',
    badgeClass: 'bg-emerald-50 text-emerald-700 border-emerald-200',
    hintEmail: 'doctor@clinic.com',
  },
  Admin: {
    label: 'Quản trị viên',
    colorClass: 'from-amber-500 to-amber-700',
    badgeClass: 'bg-amber-50 text-amber-700 border-amber-200',
    hintEmail: 'admin@clinic.com',
  },
}

const currentMeta = computed(() => ROLE_META[selectedRole.value])

// Đọc Role từ Query Parameter (nếu người dùng chuyển sang từ màn Chọn Vai Trò)
onMounted(() => {
  const queryRole = route.query.role as string
  if (queryRole && ['Patient', 'Receptionist', 'Doctor', 'Admin'].includes(queryRole)) {
    selectedRole.value = queryRole as UserRole
  }
  email.value = currentMeta.value.hintEmail
})

function switchRole(role: UserRole) {
  selectedRole.value = role
  email.value = ROLE_META[role].hintEmail
}

async function handleLogin() {
  try {
    await authStore.login({
      email: email.value,
      password: password.value,
    })
    const redirect = (route.query.redirect as string) || '/'
    router.replace(redirect)
  } catch {
    /* authStore đã lưu lỗi vào authStore.error */
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
        currentMeta.colorClass
      ]"
    >
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 bg-white/20 rounded-xl flex items-center justify-center text-white font-bold text-lg">
          +
        </div>
        <span class="font-bold text-white text-base">ClinicQueue</span>
      </div>

      <div>
        <p class="text-white/70 text-sm font-medium uppercase tracking-widest mb-3">
          {{ currentMeta.label }}
        </p>
        <h2 class="text-4xl font-bold text-white leading-tight mb-4">
          Chào mừng<br />trở lại 👋
        </h2>
        <p class="text-white/70 text-base leading-relaxed">
          Đăng nhập để truy cập các chức năng được phân quyền theo vai trò của bạn.
        </p>

        <div class="mt-8 space-y-3">
          <div v-for="feature in ['Dữ liệu mã hóa bảo mật', 'Phân quyền theo vai trò', 'Tự động đăng xuất sau 30 phút']" :key="feature" class="flex items-center gap-3 text-sm text-white/80">
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
          <span class="font-bold text-slate-800 text-base">ClinicQueue</span>
        </div>

        <div class="mb-8">
          <span :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border mb-2', currentMeta.badgeClass]">
            {{ currentMeta.label }}
          </span>
          <h1 class="text-2xl font-bold text-slate-800 mt-2 mb-1">Đăng nhập</h1>
          <p class="text-sm text-slate-500">Nhập thông tin tài khoản để tiếp tục.</p>
        </div>

        <!-- Thông báo Lỗi -->
        <div v-if="authStore.error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-xs text-red-600">
          ⚠️ Email hoặc mật khẩu không đúng
        </div>

        <form @submit.prevent="handleLogin" class="space-y-4 mb-6">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1.5">
              Email / Tên đăng nhập <span class="text-red-500">*</span>
            </label>
            <input
              v-model="email"
              type="email"
              required
              :placeholder="currentMeta.hintEmail"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92] bg-white transition-all"
            />
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
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-center gap-2 text-sm text-slate-600 cursor-pointer select-none">
              <input v-model="rememberMe" type="checkbox" class="rounded text-[#0E4D92]" />
              Ghi nhớ đăng nhập
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
        </form>

        <!-- Nút chọn Role nhanh cho Dev/Testing -->
        <div class="border-t border-slate-100 pt-4 mb-4">
          <p class="text-xs text-slate-400 mb-2 font-medium">⚡ Đổi nhanh vai trò thử nghiệm:</p>
          <div class="grid grid-cols-4 gap-1.5">
            <button
              v-for="(meta, r) in ROLE_META"
              :key="r"
              type="button"
              @click="switchRole(r as UserRole)"
              :class="[
                'py-1.5 px-2 rounded-lg text-xs font-semibold transition-all border text-center',
                selectedRole === r
                  ? 'bg-[#0E4D92] text-white border-[#0E4D92]'
                  : 'bg-slate-50 text-slate-600 border-slate-200 hover:bg-slate-100'
              ]"
            >
              {{ meta.label }}
            </button>
          </div>
        </div>

        <p class="text-xs text-slate-400 text-center mt-5">
          Gặp vấn đề? Liên hệ <span class="text-[#0E4D92]">hotro@phongkham.vn</span>
        </p>

        <button
          type="button"
          @click="goBackToRoleSelect"
          class="mt-6 w-full text-center text-sm text-slate-500 hover:text-[#0E4D92] transition-colors flex items-center justify-center gap-1 font-medium"
        >
          ← Quay lại chọn vai trò
        </button>
      </div>
    </div>
  </div>
</template>