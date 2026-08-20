<script setup lang="ts">
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { validators } from '@/utils/validators'
import { useFormValidation } from '@/composables/useFormValidation'

// Khởi tạo router và authStore để thao tác với luồng người dùng
const router = useRouter()
const authStore = useAuthStore()

onMounted(() => {
  authStore.clearBrowserSessionState()
})

// Trích xuất thông báo lỗi từ authStore để hiển thị lỗi backend trong form validation
const { error: backendError } = storeToRefs(authStore)

// Khởi tạo các biến và hàm xử lý form từ composable
const {
  formData,
  errors,
  touched,
  isSubmitting,
  handleBlur,
  validateAll
} = useFormValidation(
  {
    username: '',
    fullName: '',
    phoneNumber: '',
    email: '',
    password: '',
    confirmPassword: '',
    gender: 'Male' as 'Male' | 'Female' | 'Other',
    dateOfBirth: '',
    address: '',
    rememberMe: false,
  },

  // Các quy tắc kiểm tra (validation rules) cho từng trường dữ liệu
  {
    username: [
      (val) => validators.required(val, 'Tên đăng nhập'),
      (val) => (val && val.trim().length < 8 ? { isValid: false, message: 'Tên đăng nhập phải có ít nhất 8 ký tự' } : { isValid: true, message: '' }),
    ],
    fullName: [(val) => validators.required(val, 'Họ và tên')],
    phoneNumber: [
      (val) => validators.required(val, 'Số điện thoại'),
      (val) => validators.phone(val),
    ],
    email: [
      (val) => validators.required(val, 'Email'),
      (val) => validators.email(val),
    ],
    address: [(val) => validators.required(val, 'Địa chỉ')],
    dateOfBirth: [(val) => (val ? { isValid: true, message: '' } : { isValid: false, message: 'Ngày sinh là bắt buộc' })],
    password: [
      (val) => validators.required(val, 'Mật khẩu'),
      (val) => (val && val.trim().length < 8 ? { isValid: false, message: 'Mật khẩu phải có ít nhất 8 ký tự' } : { isValid: true, message: '' }),
      (val) => validators.password(val, 8),
    ],
    confirmPassword: [(val, formData) => validators.confirmPassword(formData.password, val)],
  },
  // truyền lỗi từ be vào composable để hiển thị giao diện
  { externalError: backendError }
)

// Hàm xử lý khi người dùng nhấn nút đăng ký
async function handleRegister() {
  if (!validateAll()) return

  isSubmitting.value = true
  try {
    await authStore.register({
      ...formData,
      username: (formData.username || formData.email).trim(),
      email: formData.email.trim(),
      fullName: formData.fullName.trim(),
      phoneNumber: formData.phoneNumber.trim(),
      address: formData.address?.trim() || 'Chưa cập nhật',
      dateOfBirth: formData.dateOfBirth,
      gender: formData.gender,
    })

    if (authStore.isAuthenticated) {
      router.push('/select-role')
    } else {
      router.push('/login')
    }
  } catch {
    /* Lỗi đã được lưu trong authStore */
  } finally {
    isSubmitting.value = false
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
          Hệ thống phòng khám
        </span>
        <h2 class="text-3xl font-bold text-white leading-tight mb-4">Tạo tài khoản để truy cập hệ thống</h2>
        <p class="text-white/70 text-sm leading-relaxed mb-6">
          Đăng ký tài khoản để tiếp tục sử dụng hệ thống quản lý khám bệnh, lịch hẹn và quy trình nội bộ theo vai trò được phân quyền.
        </p>
      </div>
      <p class="text-white/40 text-xs">© 2026 ClinicQueue</p>
    </div>

    <!-- Banner Phải -->
    <div class="flex-1 flex flex-col items-center justify-center px-6 py-10 overflow-y-auto">
      <div class="w-full max-w-md">
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-slate-800">Đăng ký tài khoản</h1>
          <p class="text-sm text-slate-500 mt-1">Nhập thông tin cá nhân để tạo hồ sơ khám bệnh.</p>
        </div>
      
        <div v-if="authStore.error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-xs text-red-600">
          ⚠️ {{ authStore.error }}
        </div>       

        <form @submit.prevent="handleRegister" autocomplete="off" autocorrect="off" autocapitalize="none" spellcheck="false" class="space-y-4" novalidate>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Tên đăng nhập <span class="text-red-500">*</span></label>
            <input
              v-model="formData.username"
              @blur="handleBlur('username')"
              type="text"
              name="username"
              autocomplete="off"
              autocapitalize="none"
              autocorrect="off"
              spellcheck="false"
              placeholder="patient01"
              :class="[
                'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                touched.username && errors.username ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
              ]"
            />
            <p v-if="touched.username && errors.username" class="text-xs text-red-500 mt-1">⚠️ {{ errors.username }}</p>
          </div>

          <!-- Họ tên -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Họ và tên <span class="text-red-500">*</span></label>
            <input
              v-model="formData.fullName"
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
                v-model="formData.phoneNumber"
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
                v-model="formData.gender"
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
                v-model="formData.dateOfBirth"
                type="date"
                class="w-full border border-slate-200 rounded-xl px-3.5 py-2.5 text-sm focus:ring-2 focus:ring-[#0E4D92] focus:outline-none bg-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Email <span class="text-red-500">*</span></label>
              <input
                v-model="formData.email"
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

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">Địa chỉ <span class="text-red-500">*</span></label>
            <textarea
              v-model="formData.address"
              @blur="handleBlur('address')"
              rows="2"
              placeholder="Số nhà, đường, phường, quận..."
              :class="[
                'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all resize-none',
                touched.address && errors.address ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
              ]"
            />
            <p v-if="touched.address && errors.address" class="text-xs text-red-500 mt-1">⚠️ {{ errors.address }}</p>
          </div>

          <!-- Mật khẩu & Xác nhận -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Mật khẩu <span class="text-red-500">*</span></label>
              <input
                v-model="formData.password"
                @blur="handleBlur('password')"
                type="password"
                name="new-password"
                autocomplete="new-password"
                autocapitalize="none"
                autocorrect="off"
                spellcheck="false"
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
                v-model="formData.confirmPassword"
                @blur="handleBlur('confirmPassword')"
                type="password"
                name="new-password"
                autocomplete="new-password"
                autocapitalize="none"
                autocorrect="off"
                spellcheck="false"
                placeholder="••••••••"
                :class="[
                  'w-full border rounded-xl px-3.5 py-2.5 text-sm focus:outline-none transition-all',
                  touched.confirmPassword && errors.confirmPassword ? 'border-red-500 bg-red-50/30' : 'border-slate-200 focus:ring-2 focus:ring-[#0E4D92]'
                ]"
              />
              <p v-if="touched.confirmPassword && errors.confirmPassword" class="text-xs text-red-500 mt-1">⚠️ {{ errors.confirmPassword }}</p>
            </div>
          </div>

          <div class="flex items-center justify-between mt-2">
            <label class="flex items-start gap-2 text-sm text-slate-600 cursor-pointer select-none">
              <input v-model="formData.rememberMe" type="checkbox" class="mt-1 rounded text-[#0E4D92]" />
              <span>
                <span class="block font-medium text-slate-700">Ghi nhớ đăng nhập sau khi đăng ký</span>
                <span class="block text-xs text-slate-500">Nếu bỏ tích, hệ thống chỉ giữ phiên đăng nhập trong trình duyệt hiện tại.</span>
              </span>
            </label>
          </div>

          <button
            type="submit"
            :disabled="isSubmitting || authStore.status === 'loading'"
            class="w-full bg-[#0E4D92] text-white font-semibold rounded-xl py-3 text-sm hover:bg-[#0b3d75] transition-all disabled:opacity-50 mt-2"
          >
            <span v-if="isSubmitting || authStore.status === 'loading'">Đang tạo tài khoản...</span>
            <span v-else>Đăng ký ngay</span>
          </button>
        </form>

        <p class="text-center text-sm text-slate-500 mt-6">
          Đã có tài khoản?
          <router-link to="/login" class="text-[#0E4D92] font-semibold hover:underline">
            Đăng nhập
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>