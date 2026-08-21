<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'
import { http } from '@/lib/api/http'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'

const auth = useAuthStore()
const patient = usePatientStore()
const router = useRouter()

const saving = ref(false)
const errorMessage = ref<string | null>(null)
const successMessage = ref<string | null>(null)

const form = ref({
  fullName: auth.user?.name ?? '',
  email: auth.user?.email ?? '',
  phoneNumber: (auth.user as any)?.phoneNumber ?? '',
  address: (auth.user as any)?.address ?? '',
  dateOfBirth: (auth.user as any)?.dateOfBirth ?? '',
  gender: (auth.user as any)?.gender ?? 0,
})

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
      dateOfBirth: profile.dateOfBirth ?? '',
      gender: typeof profile.gender === 'number' ? profile.gender : 0,
    }
  } catch (error) {
    // ignore; form stays with auth data if profile endpoint is unavailable
  }
})

async function save() {
  saving.value = true
  errorMessage.value = null
  successMessage.value = null
  try {
    await http.post('/patients', form.value)
    successMessage.value = 'Đã lưu hồ sơ bệnh nhân thành công!'

    try {
      await auth.ensurePatientProfile()
    } catch (e) {
      // ignore
    }

    setTimeout(() => {
      router.push({ name: 'patient-home' })
    }, 600)
  } catch (err: any) {
    const status = err?.response?.status ?? 0
    const bodyMsg = err?.response?.data?.message ?? err?.message ?? 'Lỗi khi lưu hồ sơ'
    errorMessage.value = `Lỗi (${status}): ${bodyMsg}`
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Hoàn thiện hồ sơ bệnh nhân
      </h1>
      <p class="mt-1 text-xs text-slate-500">
        Thông tin này được sử dụng cho việc đặt lịch và hồ sơ bệnh án điện tử tại phòng khám.
      </p>
    </div>

    <BaseAlert
      v-if="successMessage"
      type="success"
      :message="successMessage"
    />

    <BaseAlert
      v-if="errorMessage"
      type="error"
      :message="errorMessage"
    />

    <form @submit.prevent="save" class="rounded-2xl border border-slate-200 bg-white p-6 space-y-4 shadow-xs">
      <div>
        <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
          Họ và tên <span class="text-red-500">*</span>
        </label>
        <input
          v-model="form.fullName"
          required
          class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
          placeholder="Nhập họ và tên đầy đủ"
        />
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
            Email
          </label>
          <input
            v-model="form.email"
            type="email"
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
            placeholder="example@gmail.com"
          />
        </div>

        <div>
          <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
            Số điện thoại <span class="text-red-500">*</span>
          </label>
          <input
            v-model="form.phoneNumber"
            required
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
            placeholder="0912345678"
          />
        </div>
      </div>

      <div>
        <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
          Địa chỉ
        </label>
        <input
          v-model="form.address"
          class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
          placeholder="Số nhà, đường, phường/xã, quận/huyện..."
        />
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
            Ngày sinh
          </label>
          <input
            type="date"
            v-model="form.dateOfBirth"
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
          />
        </div>

        <div>
          <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5">
            Giới tính
          </label>
          <select
            v-model.number="form.gender"
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-800 focus:border-[#0E4D92] focus:outline-none focus:ring-1 focus:ring-[#0E4D92]"
          >
            <option :value="0">Nam</option>
            <option :value="1">Nữ</option>
            <option :value="2">Khác</option>
          </select>
        </div>
      </div>

      <div class="flex items-center gap-3 pt-3 border-t border-slate-100">
        <BaseButton
          type="submit"
          variant="primary"
          :loading="saving"
          loading-text="Đang lưu hồ sơ..."
        >
          Lưu hồ sơ
        </BaseButton>

        <RouterLink
          to="/patient"
          class="px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-600 hover:bg-slate-50 transition-colors"
        >
          Hủy bỏ
        </RouterLink>
      </div>
    </form>
  </div>
</template>
