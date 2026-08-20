<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'
import { http } from '@/lib/api/http'

const auth = useAuthStore()
const patient = usePatientStore()
const router = useRouter()

const saving = ref(false)
const message = ref<string | null>(null)

const form = ref({
  fullName: auth.user?.name ?? '',
  email: auth.user?.email ?? '',
  phoneNumber: '',
  address: '',
  dateOfBirth: '',
  gender: 0,
  insuranceNumber: '',
  emergencyContact: '',
})

function normalizeDateInput(value: unknown): string {
  if (!value) return ''

  if (typeof value === 'string') {
    const text = value.trim()
    if (!text) return ''
    const match = text.match(/^(\d{4}-\d{2}-\d{2})/)
    if (match && match[1]) return match[1]

    const parsed = new Date(text)
    if (!Number.isNaN(parsed.getTime())) {
      return parsed.toISOString().slice(0, 10)
    }

    return text.slice(0, 10)
  }

  if (value instanceof Date) {
    return value.toISOString().slice(0, 10)
  }

  return String(value).slice(0, 10)
}

function normalizeGenderValue(value: unknown): number {
  if (typeof value === 'number' && Number.isFinite(value)) return value

  if (typeof value === 'string') {
    const normalized = value.trim().toLowerCase()
    if (normalized === 'male') return 0
    if (normalized === 'female') return 1
    if (normalized === 'other') return 2

    const numeric = Number(value)
    if (Number.isFinite(numeric)) return numeric
  }

  return 0
}

onMounted(async () => {
  try {
    await patient.loadProfile()
    const profile = patient.profile
    if (!profile) return

    form.value = {
      fullName: profile.fullName ?? auth.user?.name ?? '',
      email: profile.email ?? auth.user?.email ?? '',
      phoneNumber: profile.phoneNumber ?? '',
      address: profile.address ?? '',
      dateOfBirth: normalizeDateInput(profile.dateOfBirth ?? ''),
      gender: normalizeGenderValue(profile.gender ?? 0),
      insuranceNumber: profile.insuranceNumber ?? '',
      emergencyContact: profile.emergencyContact ?? '',
    }
  } catch (error) {
    // ignore; form stays with auth data if profile endpoint is unavailable
  }
})

async function save() {
  saving.value = true
  message.value = null

  try {
    const payload = {
      email: form.value.email,
      gender: Number(form.value.gender ?? 0),
      address: form.value.address,
      insuranceNumber: form.value.insuranceNumber || null,
      emergencyContact: form.value.emergencyContact || null,
    }

    await http.put('/patients/me', payload)
    await patient.loadProfile()
    message.value = 'Đã lưu hồ sơ bệnh nhân.'

    try {
      await auth.ensurePatientProfile()
    } catch (e) {
      // ignore — ensurePatientProfile reports friendly messages
    }

    router.push({ name: 'patient-home' })
  } catch (err: any) {
    const status = err?.response?.status ?? 0
    const bodyMsg = err?.response?.data?.message ?? err?.response?.data?.title ?? err?.message ?? 'Lỗi khi lưu hồ sơ'
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
        <input v-model="form.fullName" readonly class="mt-1 w-full rounded border bg-slate-50 px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Email</label>
        <input v-model="form.email" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Số điện thoại</label>
        <input v-model="form.phoneNumber" readonly class="mt-1 w-full rounded border bg-slate-50 px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Địa chỉ</label>
        <input v-model="form.address" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700">Ngày sinh</label>
          <input type="date" v-model="form.dateOfBirth" readonly class="mt-1 w-full rounded border bg-slate-50 px-3 py-2" />
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

      <div>
        <label class="block text-sm font-medium text-slate-700">Số bảo hiểm</label>
        <input v-model="form.insuranceNumber" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div>
        <label class="block text-sm font-medium text-slate-700">Liên hệ khẩn cấp</label>
        <input v-model="form.emergencyContact" class="mt-1 w-full rounded border px-3 py-2" />
      </div>

      <div class="flex items-center gap-3">
        <button type="submit" :disabled="saving" class="rounded bg-[#0E4D92] px-4 py-2 text-white">Lưu</button>
        <RouterLink to="/patient" class="text-sm text-slate-600">Hủy</RouterLink>
      </div>
    </form>
  </div>
</template>
