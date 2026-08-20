<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { validators } from '@/utils/validators'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseButton from '@/components/ui/BaseButton.vue'

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
  <div class="min-h-screen bg-slate-50 flex items-center justify-center px-4 py-10 font-sans">
    <div class="w-full max-w-md rounded-3xl border border-slate-200 bg-white p-6 sm:p-8 shadow-sm">
      <div class="mb-6">
        <button
          type="button"
          class="mb-4 inline-flex items-center gap-1.5 text-sm font-medium text-[#0E4D92] hover:underline cursor-pointer focus:outline-none"
          @click="router.push('/login')"
        >
          ← Quay lại đăng nhập
        </button>
        <h1 class="text-2xl font-bold text-slate-800">Quên mật khẩu</h1>
        <p class="mt-2 text-sm text-slate-500">
          Nhập email đã đăng ký để nhận hướng dẫn đặt lại mật khẩu.
        </p>
      </div>

      <form v-if="!submitted" @submit.prevent="handleSubmit" class="space-y-4" novalidate>
        <BaseInput
          v-model="email"
          label="Email"
          required
          type="email"
          placeholder="name@clinic.com"
          :error="error"
          autocomplete="email"
        />

        <div class="pt-2">
          <BaseButton
            type="submit"
            variant="primary"
            size="lg"
            block
          >
            Gửi yêu cầu
          </BaseButton>
        </div>
      </form>

      <div v-else class="space-y-4">
        <BaseAlert
          type="success"
          title="Yêu cầu đã được ghi nhận"
        >
          Nếu email này có tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi tới email của bạn. Nếu chưa nhận được, vui lòng kiểm tra hộp thư spam hoặc liên hệ bộ phận hỗ trợ.
        </BaseAlert>

        <BaseButton
          type="button"
          variant="outline"
          size="md"
          block
          @click="router.push('/login')"
        >
          Về trang đăng nhập
        </BaseButton>
      </div>
    </div>
  </div>
</template>
