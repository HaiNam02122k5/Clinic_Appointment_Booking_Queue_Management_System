<script setup lang="ts">
import { ref } from 'vue'

import CheckInResult from '@/features/receptionist/components/CheckInResult.vue'
import { useReceptionistStore } from '@/stores/receptionist'

import type { AppointmentPatient } from '@/features/receptionist/receptionist.mock'

const receptionistStore = useReceptionistStore()

const searchValue = ref('')

const patient = ref<AppointmentPatient | null>(null)

const errorMessage = ref('')

const queueNumber = ref<string | null>(null)

function searchPatient() {
  errorMessage.value = ''
  patient.value = null
  queueNumber.value = null

  const keyword = searchValue.value.trim()

  if (!keyword) {
    errorMessage.value =
      'Vui lòng nhập mã lịch hẹn hoặc số điện thoại.'

    return
  }

  const result =
    receptionistStore.findAppointment(keyword)

  if (!result) {
    errorMessage.value =
      'Không tìm thấy lịch hẹn phù hợp.'

    return
  }

  if (result.checkedIn) {
    errorMessage.value =
      'Bệnh nhân này đã check-in.'

    return
  }

  patient.value = result
}

function checkIn() {
  if (!patient.value) {
    return
  }

  const result = receptionistStore.checkIn(
    patient.value.appointmentId
  )

  if (!result) {
    errorMessage.value =
      'Không thể check-in bệnh nhân này.'

    return
  }

  queueNumber.value = result.no

  patient.value = {
    ...patient.value,
    checkedIn: true,
  }
}
</script>

<template>
  <div class="space-y-5 max-w-2xl">

    <!-- TITLE -->
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Check-in bệnh nhân
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Tra cứu lịch hẹn và xác nhận bệnh nhân đến khám
      </p>
    </div>

    <!-- SEARCH CARD -->
    <div
      class="bg-white border border-slate-200
             rounded-xl p-6"
    >

      <h2
        class="text-sm font-semibold
               text-slate-800 mb-4"
      >
        Tra cứu lịch hẹn
      </h2>

      <div>

        <label
          class="block text-xs font-medium
                 text-slate-600 mb-1.5"
        >
          Mã lịch hẹn hoặc Số điện thoại
        </label>

          <input
            v-model="searchValue"
            type="text"
            placeholder="Ví dụ: APT-001 hoặc 0901234567"
            class="w-full px-3 py-2.5
                  text-sm text-slate-900
                  border border-slate-200
                  rounded-lg
                  focus:outline-none
                  focus:border-violet-600
                  focus:ring-1
                  focus:ring-violet-600"
            @keyup.enter="searchPatient"
          />

      </div>

      <!-- ERROR -->
      <div
        v-if="errorMessage"
        class="mt-3 px-3 py-2
               bg-red-50 border border-red-200
               rounded-lg text-sm text-red-600"
      >
        {{ errorMessage }}
      </div>

      <button
        type="button"
        class="mt-4 w-full py-2.5
               bg-violet-600 text-white
               text-sm font-semibold
               rounded-lg
               hover:bg-violet-700
               transition-colors"
        @click="searchPatient"
      >
        Tra cứu
      </button>

    </div>

    <!-- PATIENT INFORMATION -->
    <div
      v-if="patient && !queueNumber"
      class="bg-white border border-slate-200
             rounded-xl p-6"
    >

      <h2
        class="text-sm font-semibold
               text-slate-800 mb-4"
      >
        Thông tin bệnh nhân
      </h2>

      <div class="space-y-3">

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Họ tên
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.name }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Mã lịch hẹn
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.appointmentId }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Số điện thoại
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.phone }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Bác sĩ
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.doctor }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Chuyên khoa
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.specialty }}
          </span>
        </div>

        <div class="flex justify-between py-2">

          <span class="text-sm text-slate-500">
            Giờ hẹn
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.appointmentTime }}
          </span>

        </div>

      </div>

      <!-- CHECK IN -->
      <button
        type="button"
        class="mt-5 w-full py-2.5
               bg-violet-600 text-white
               text-sm font-semibold
               rounded-lg
               hover:bg-violet-700
               transition-colors"
        @click="checkIn"
      >
        Xác nhận Check-in
      </button>

    </div>

    <!-- RESULT -->
    <CheckInResult
      v-if="queueNumber && patient"
      :queue-number="queueNumber"
      :patient-name="patient.name"
    />

  </div>
</template>
