<script setup lang="ts">
import { reactive, watch, onMounted } from 'vue'

import BaseButton from '@/components/ui/BaseButton.vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseCard from '@/components/ui/BaseCard.vue'

import type { Gender } from './users.types'
import { specialtiesApi } from '@/features/specialties/specialties.api'

type AccountType = 'Doctor' | 'Receptionist'

interface Specialty {
  id: string
  name: string
}

export interface CreateAccountForm {
  role: AccountType

  username: string
  password: string

  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: Gender
  address: string

  hireDate: string

  specialtyId: string
  licenseNumber: string
  qualification: string
  experienceYears: string
  biography: string
}

const props = defineProps<{
  open: boolean
  submitting?: boolean
}>()

const emit = defineEmits<{
  close: []
  submit: [value: CreateAccountForm]
}>()

const specialties = reactive<Specialty[]>([])

const getInitialForm = (): CreateAccountForm => ({
  role: 'Receptionist',

  username: '',
  password: '',

  fullName: '',
  phoneNumber: '',
  email: '',
  dateOfBirth: '',
  gender: 'Male',
  address: '',

  hireDate: '',

  specialtyId: '',
  licenseNumber: '',
  qualification: '',
  experienceYears: '',
  biography: '',
})

const form = reactive<CreateAccountForm>(
  getInitialForm(),
)

const resetForm = () => {
  Object.assign(
    form,
    getInitialForm(),
  )
}

const loadSpecialties = async () => {
  try {
    const response = await specialtiesApi.list({
      Page: 1,
      PageSize: 100,
    })

    specialties.splice(
      0,
      specialties.length,
      ...response.items,
    )
  } catch (error) {
    console.error(
      'Không thể tải danh sách chuyên khoa:',
      error,
    )
  }
}

onMounted(() => {
  loadSpecialties()
})

watch(
  () => props.open,
  (isOpen) => {
    if (isOpen) {
      resetForm()
    }
  },
)

const handleClose = () => {
  if (props.submitting) {
    return
  }

  resetForm()

  emit('close')
}

const handleSubmit = () => {
  emit(
    'submit',
    { ...form },
  )
}
</script>

<template>
  <div
    v-if="open"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
    @click.self="handleClose"
  >
    <BaseCard
      class="max-h-[90vh] w-full max-w-3xl overflow-y-auto bg-white text-slate-800 shadow-2xl"
    >
      <form
        class="flex flex-col gap-6 bg-white p-6"
        @submit.prevent="handleSubmit"
      >
        <!-- HEADER -->

        <div class="flex items-start justify-between gap-4">
          <div>
            <h2 class="text-xl font-semibold text-slate-900">
              Thêm tài khoản
            </h2>

            <p class="mt-1 text-sm text-slate-500">
              Tạo tài khoản mới cho bác sĩ hoặc lễ tân
            </p>
          </div>

          <button
            type="button"
            class="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg text-xl text-slate-400 hover:bg-slate-100 hover:text-slate-700"
            :disabled="submitting"
            @click="handleClose"
          >
            ×
          </button>
        </div>

        <!-- ACCOUNT TYPE -->

        <div class="space-y-2">
          <label class="block text-sm font-medium text-slate-700">
            Loại tài khoản
          </label>

          <select
            v-model="form.role"
            class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 outline-none transition focus:border-violet-500 focus:ring-2 focus:ring-violet-100"
          >
            <option value="Receptionist">
              Lễ tân
            </option>

            <option value="Doctor">
              Bác sĩ
            </option>
          </select>
        </div>

        <!-- ACCOUNT INFORMATION -->

        <div>
          <h3 class="mb-3 text-base font-semibold text-slate-800">
            Thông tin tài khoản
          </h3>

          <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
            <BaseInput
              v-model="form.username"
              label="Tên đăng nhập"
              required
            />

            <BaseInput
              v-model="form.password"
              label="Mật khẩu"
              type="password"
              required
            />
          </div>
        </div>

        <!-- PERSONAL INFORMATION -->

        <div>
          <h3 class="mb-3 text-base font-semibold text-slate-800">
            Thông tin cá nhân
          </h3>

          <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
            <BaseInput
              v-model="form.fullName"
              label="Họ và tên"
              required
            />

            <BaseInput
              v-model="form.email"
              label="Email"
              type="email"
              required
            />

            <BaseInput
              v-model="form.phoneNumber"
              label="Số điện thoại"
              required
            />

            <BaseInput
              v-model="form.dateOfBirth"
              label="Ngày sinh"
              type="date"
              required
            />

            <!-- GENDER -->

            <div class="flex flex-col gap-1">
              <label class="text-sm font-medium text-slate-700">
                Giới tính
              </label>

              <select
                v-model="form.gender"
                class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 outline-none transition focus:border-violet-500 focus:ring-2 focus:ring-violet-100"
              >
                <option value="Male">
                  Nam
                </option>

                <option value="Female">
                  Nữ
                </option>

                <option value="Other">
                  Khác
                </option>
              </select>
            </div>

            <BaseInput
              v-model="form.address"
              label="Địa chỉ"
              required
            />

            <BaseInput
              v-model="form.hireDate"
              label="Ngày vào làm"
              type="date"
              required
            />
          </div>
        </div>

        <!-- DOCTOR INFORMATION -->

        <div
          v-if="form.role === 'Doctor'"
          class="rounded-xl border border-violet-200 bg-violet-50 p-5"
        >
          <h3 class="mb-4 text-base font-semibold text-violet-800">
            Thông tin bác sĩ
          </h3>

          <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
            <!-- SPECIALTY -->

            <div class="flex flex-col gap-1">
              <label class="text-sm font-medium text-slate-700">
                Chuyên khoa
              </label>

              <select
                v-model="form.specialtyId"
                required
                class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 outline-none transition focus:border-violet-500 focus:ring-2 focus:ring-violet-100"
              >
                <option value="" disabled>
                  -- Chọn chuyên khoa --
                </option>

                <option
                  v-for="specialty in specialties"
                  :key="specialty.id"
                  :value="specialty.id"
                >
                  {{ specialty.name }}
                </option>
              </select>
            </div>

            <BaseInput
              v-model="form.licenseNumber"
              label="Số giấy phép hành nghề"
              required
            />

            <BaseInput
              v-model="form.qualification"
              label="Trình độ chuyên môn"
              required
            />

            <BaseInput
              v-model="form.experienceYears"
              label="Số năm kinh nghiệm"
              type="number"
              required
            />

            <div class="md:col-span-2">
              <label class="mb-1 block text-sm font-medium text-slate-700">
                Tiểu sử
              </label>

              <textarea
                v-model="form.biography"
                rows="4"
                class="w-full resize-none rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 outline-none placeholder:text-slate-400 transition focus:border-violet-500 focus:ring-2 focus:ring-violet-100"
                placeholder="Nhập thông tin giới thiệu về bác sĩ..."
              ></textarea>
            </div>
          </div>
        </div>

        <!-- ACTIONS -->

        <div
          class="flex justify-end gap-3 border-t border-slate-200 pt-5"
        >
          <BaseButton
            type="button"
            variant="secondary"
            :disabled="submitting"
            @click="handleClose"
          >
            Hủy
          </BaseButton>

          <BaseButton
            type="submit"
            :loading="submitting"
          >
            Tạo tài khoản
          </BaseButton>
        </div>
      </form>
    </BaseCard>
  </div>
</template>
