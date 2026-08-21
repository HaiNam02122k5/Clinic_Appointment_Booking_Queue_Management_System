<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { validators } from '@/utils/validators'

const router = useRouter()
const email = ref('')
const submitted = ref(false)
const error = ref('')

function validateEmail() {
  const res = validators.required(email.value.trim(), 'Email')
  if (!res.isValid) {
    error.value = res.message
    return false
  }

  const emailRule = validators.email(email.value.trim())
  if (!emailRule.isValid) {
    error.value = emailRule.message
    return false
  }

  error.value = ''
  return true
}

function handleSubmit() {
  if (!validateEmail()) return

  submitted.value = true
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center px-4 py-10">
    <div class="w-full max-w-md rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <div class="mb-6">
        <button
          type="button"
          class="mb-4 inline-flex items-center gap-2 text-sm font-medium text-[#0E4D92] hover:underline"
          @click="router.push('/login')"
        >
          ← Quay lại đăng nhập
        </button>
        <h1 class="text-2xl font-bold text-slate-800">Quên mật khẩu</h1>
        <p class="mt-2 text-sm text-slate-500">
          Nhập email đã đăng ký để nhận hướng dẫn đặt lại mật khẩu.
        </p>
      </div>

      <form v-if="!submitted" @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="mb-1.5 block text-sm font-medium text-slate-700">Email</label>
          <input
            v-model="email"
            type="email"
            placeholder="name@clinic.com"
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
          />
          <p v-if="error" class="mt-1 text-xs text-red-500">⚠️ {{ error }}</p>
        </div>

        <button
          type="submit"
          class="w-full rounded-xl bg-[#0E4D92] px-6 py-3 text-base font-semibold text-white transition hover:bg-[#0b3d75]"
        >
          Gửi yêu cầu
        </button>
      </form>

      <div v-else class="rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
        <p class="font-semibold">Yêu cầu đã được ghi nhận.</p>
        <p class="mt-2 leading-6">
          Nếu email này có tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi tới email của bạn.
          Nếu chưa nhận được, vui lòng kiểm tra hộp thư spam hoặc liên hệ bộ phận hỗ trợ.
        </p>
        <button
          type="button"
          class="mt-4 w-full rounded-xl border border-emerald-300 bg-white px-4 py-2.5 font-medium text-emerald-700 hover:bg-emerald-100"
          @click="router.push('/login')"
        >
          Về trang đăng nhập
        </button>
      </div>
    </div>
  </div>
</template>
