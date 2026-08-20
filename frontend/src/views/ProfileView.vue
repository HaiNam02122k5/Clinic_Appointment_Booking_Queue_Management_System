<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { http } from '@/lib/api/http'

const auth = useAuthStore()
const router = useRouter()

const saving = ref(false)
const message = ref<string | null>(null)

const form = ref({
  fullName: auth.user?.name ?? '',
  email: auth.user?.email ?? '',
  phoneNumber: (auth.user as any)?.phoneNumber ?? '',
  address: (auth.user as any)?.address ?? '',
  dateOfBirth: (auth.user as any)?.dateOfBirth ?? '',
  gender: (auth.user as any)?.gender ?? 0,
})

async function save() {
  saving.value = true
  message.value = null
  try {
    // Try to create patient profile via POST /patients
    await http.post('/patients', form.value)
    message.value = 'Đã lưu hồ sơ bệnh nhân.'

    // Re-run ensurePatientProfile to let app refresh and load patient data
    try {
      await auth.ensurePatientProfile()
    } catch (e) {
      // ignore — ensurePatientProfile reports friendly messages
    }

    // Redirect to patient home
    router.push({ name: 'patient-home' })
  } catch (err: any) {
    const status = err?.response?.status ?? 0
    const bodyMsg = err?.response?.data?.message ?? err?.message ?? 'Lỗi khi lưu hồ sơ'
    message.value = `Lỗi (${status}): ${bodyMsg}`
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-6">
    <h1 class="text-2xl font-bold">Hoàn thiện hồ sơ</h1>

    <div v-if="message" class="rounded p-3 bg-slate-50 text-sm">{{ message }}</div>

    <form @submit.prevent="save" class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-slate-700">Họ và tên</label>
        <input v-model="form.fullName" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Email</label>
        <input v-model="form.email" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Số điện thoại</label>
        <input v-model="form.phoneNumber" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Địa chỉ</label>
        <input v-model="form.address" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700">Ngày sinh</label>
          <input type="date" v-model="form.dateOfBirth" class="mt-1 w-full rounded border px-3 py-2" />
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-700">Giới tính</label>
          <select v-model.number="form.gender" class="mt-1 w-full rounded border px-3 py-2">
            <option :value="0">Nam</option>
            <option :value="1">Nữ</option>
            <option :value="2">Khác</option>
          </select>
        </div>
      </div>

      <div class="flex items-center gap-3">
        <button type="submit" :disabled="saving" class="rounded bg-[#0E4D92] px-4 py-2 text-white">Lưu</button>
        <RouterLink to="/patient" class="text-sm text-slate-600">Hủy</RouterLink>
      </div>
    </form>
  </div>
</template>
