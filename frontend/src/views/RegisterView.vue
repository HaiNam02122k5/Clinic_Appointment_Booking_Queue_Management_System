<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { validators } from '@/utils/validators'

const router = useRouter()
const authStore = useAuthStore()

const fullName = ref('')
const phoneNumber = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const gender = ref<'Male' | 'Female' | 'Other'>('Male')
const dateOfBirth = ref('')

const errors = ref<Record<string, string>>({})
const touched = ref<Record<string, boolean>>({})

function handleBlur(field: string) {
  touched.value[field] = true
  validateField(field)
}

function validateField(field: string): boolean {
  let res = { isValid: true, message: '' }

  switch (field) {
    case 'fullName':
      res = validators.required(fullName.value, 'Họ và tên')
      break
    case 'phoneNumber':
      res = validators.required(phoneNumber.value, 'Số điện thoại')
      if (res.isValid) res = validators.phone(phoneNumber.value)
      break
    case 'email':
      res = validators.required(email.value, 'Email')
      if (res.isValid) res = validators.email(email.value)
      break
    case 'password':
      res = validators.required(password.value, 'Mật khẩu')
      if (res.isValid) res = validators.password(password.value)
      break
    case 'confirmPassword':
      res = validators.confirmPassword(password.value, confirmPassword.value)
      break
  }

  errors.value[field] = res.message
  return res.isValid
}

function validateAll(): boolean {
  const fields = ['fullName', 'phoneNumber', 'email', 'password', 'confirmPassword']
  let isValid = true
  fields.forEach((f) => {
    touched.value[f] = true
    if (!validateField(f)) isValid = false
  })
  return isValid
}

async function handleRegister() {
  if (!validateAll()) return

  try {
    await authStore.register({
      fullName: fullName.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
      password: password.value,
      gender: gender.value,
      dateOfBirth: dateOfBirth.value,
    })

    // Nếu Backend trả về token và tự động đăng nhập
    if (authStore.isAuthenticated) {
      router.push('/patient')
    } else {
      // Nếu Backend yêu cầu đăng nhập lại sau khi đăng ký
      router.push('/login?role=Patient')
    }
  } catch {
    /* authStore.error đã lưu thông báo lỗi từ Backend */
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex font-sans">
    <!-- Banner Trái -->
    <div class="hidden lg:flex lg:w-2/5 bg-gradient-to-br from-[#0E4D92] to-[#1565c0] flex-col justify-between p-10">
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 bg-white/20 rounded-xl flex items-center justify-center text-white font-bold text-lg">+</div>
        <span class="font-bold text-white text-base">ClinicQueue</span>
      </div>
      <div>
        <span class="inline-block bg-blue-400/20 text-blue-100 border border-blue-300/30 text-xs px-3 py-1 rounded-full font-medium mb-4">
          Dành cho Bệnh nhân
        </span>
        <h2 class="text-3xl font-bold text-white leading-tight mb-4">Tạo tài khoản khám bệnh trực tuyến</h2>
        <p class="text-white/70 text-sm leading-relaxed mb-6">
          Đăng ký tài khoản giúp bạn chủ động đặt lịch khám, lấy số thứ tự từ xa và nhận thông báo nhắc lịch tự động.
        </p>
      </div>
      <p class="text-white/40 text-xs">© 2026 ClinicQueue</p>
    </div>

    <!-- Form Phải -->
    <div class="flex-1 flex flex-col items-center justify-center px-6 py-10 overflow-y-auto">
      <div class="w-full max-w-md">
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Đăng ký tài khoản</h1>
          <p class="text-sm text-slate-500 mt-1">Nhập thông tin cá nhân để tạo hồ sơ khám bệnh.</p>
        </div>
      
        <div v-if="authStore.error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-xs text-red-600">
          ⚠️ {{ authStore.error }}
        </div>       

        <form @submit.prevent="handleRegister" class="space-y-4" novalidate>
          <!-- Họ tên -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Họ và tên <span class="text-red-500">*</span></label>
            <input
              v-model="fullName"
              @blur="handleBlur('fullName')"
              type="text"
              placeholder="Nguyễn Văn A"
              :class="[
                'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                touched.fullName && errors.fullName ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
              ]"
            />
            <p v-if="touched.fullName && errors.fullName" class="text-xs text-red-500 mt-1">⚠️ {{ errors.fullName }}</p>
          </div>

          <!-- SĐT + Giới tính -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Số điện thoại <span class="text-red-500">*</span></label>
              <input
                v-model="phoneNumber"
                @blur="handleBlur('phoneNumber')"
                type="tel"
                placeholder="0912345678"
                :class="[
                  'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                  touched.phoneNumber && errors.phoneNumber ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
                ]"
              />
              <p v-if="touched.phoneNumber && errors.phoneNumber" class="text-xs text-red-500 mt-1">⚠️ {{ errors.phoneNumber }}</p>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Giới tính</label>
              <select
                v-model="gender"
                class="w-full border border-slate-200 rounded-xl px-3.5 py-2.5 text-sm focus:ring-2 focus:ring-[#0E4D92] focus:outline-none bg-white"
              >
                <option value="Male">Nam</option>
                <option value="Female">Nữ</option>
                <option value="Other">Khác</option>
              </select>
            </div>
          </div>

          <!-- Ngày sinh + Email -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Ngày sinh</label>
              <input
                v-model="dateOfBirth"
                type="date"
                class="w-full border border-slate-200 rounded-xl px-3.5 py-2.5 text-sm focus:ring-2 focus:ring-[#0E4D92] focus:outline-none bg-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Email <span class="text-red-500">*</span></label>
              <input
                v-model="email"
                @blur="handleBlur('email')"
                type="email"
                placeholder="email@example.com"
                :class="[
                  'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                  touched.email && errors.email ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
                ]"
              />
              <p v-if="touched.email && errors.email" class="text-xs text-red-500 mt-1">⚠️ {{ errors.email }}</p>
            </div>
          </div>

          <!-- Mật khẩu & Xác nhận -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Mật khẩu <span class="text-red-500">*</span></label>
              <input
                v-model="password"
                @blur="handleBlur('password')"
                type="password"
                placeholder="••••••••"
                :class="[
                  'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                  touched.password && errors.password ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
                ]"
              />
              <p v-if="touched.password && errors.password" class="text-xs text-red-500 mt-1">⚠️ {{ errors.password }}</p>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Xác nhận mật khẩu <span class="text-red-500">*</span></label>
              <input
                v-model="confirmPassword"
                @blur="handleBlur('confirmPassword')"
                type="password"
                placeholder="••••••••"
                :class="[
                  'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                  touched.confirmPassword && errors.confirmPassword ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
                ]"
              />
              <p v-if="touched.confirmPassword && errors.confirmPassword" class="text-xs text-red-500 mt-1">⚠️ {{ errors.confirmPassword }}</p>
            </div>
          </div>

          <button
            type="submit"
            :disabled="authStore.status === 'loading'"
            class="w-full bg-[#0E4D92] text-white font-semibold rounded-xl py-3 text-sm hover:bg-[#0b3d75] transition-all disabled:opacity-50 mt-2"
          >
            <span v-if="authStore.status === 'loading'">Đang tạo tài khoản...</span>
            <span v-else>Đăng ký ngay</span>
          </button>
        </form>

        <p class="text-center text-sm text-slate-500 mt-6">
          Đã có tài khoản?
          <router-link to="/login?role=Patient" class="text-[#0E4D92] font-semibold hover:underline">
            Đăng nhập
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>