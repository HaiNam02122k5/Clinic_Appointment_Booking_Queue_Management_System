<script setup lang="ts">
import { reactive } from 'vue'

import BaseButton from '@/components/ui/BaseButton.vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseCard from '@/components/ui/BaseCard.vue'

import type { Gender } from './users.types'

type AccountType = 'Doctor' | 'Receptionist'

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
  >
    <BaseCard
      class="max-h-[90vh] w-full max-w-3xl overflow-y-auto"
    >
      <form
        class="flex flex-col gap-6 p-6"
        @submit.prevent="handleSubmit"
      >
        <!-- HEADER -->

        <div>
          <h2
            class="text-xl font-semibold text-slate-800"
          >
            Thêm tài khoản
          </h2>

          <p
            class="mt-1 text-sm text-slate-500"
          >
            Tạo tài khoản mới cho bác sĩ hoặc lễ tân
          </p>
        </div>

        <!-- ACCOUNT TYPE -->

        <div class="space-y-2">
          <label
            class="text-sm font-medium text-slate-700"
          >
            Loại tài khoản
          </label>

          <select
            v-model="form.role"
            class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 outline-none focus:border-violet-500"
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
          <h3
            class="mb-3 text-base font-semibold text-slate-800"
          >
            Thông tin tài khoản
          </h3>

          <div
            class="grid grid-cols-1 gap-4 md:grid-cols-2"
          >
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
          <h3
            class="mb-3 text-base font-semibold text-slate-800"
          >
            Thông tin cá nhân
          </h3>

          <div
            class="grid grid-cols-1 gap-4 md:grid-cols-2"
          >
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

            <div
              class="flex flex-col gap-1"
            >
              <label
                class="text-sm font-medium text-slate-700"
              >
                Giới tính
              </label>

              <select
                v-model="form.gender"
                class="rounded-lg border border-slate-300 px-3 py-2.5 outline-none focus:border-violet-500"
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
          class="rounded-xl border border-violet-100 bg-violet-50/40 p-4"
        >
          <h3
            class="mb-4 text-base font-semibold text-violet-800"
          >
            Thông tin bác sĩ
          </h3>

          <div
            class="grid grid-cols-1 gap-4 md:grid-cols-2"
          >
            <BaseInput
              v-model="form.specialtyId"
              label="ID chuyên khoa"
              required
            />

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

            <div
              class="md:col-span-2"
            >
              <label
                class="mb-1 block text-sm font-medium text-slate-700"
              >
                Tiểu sử
              </label>

              <textarea
                v-model="form.biography"
                rows="4"
                class="w-full resize-none rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-violet-500"
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
