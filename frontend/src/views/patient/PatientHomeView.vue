<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'

import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'

const auth = useAuthStore()
const patient = usePatientStore()

const today = new Date().toLocaleDateString('vi-VN', {
  weekday: 'long',
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

const isLoading = ref(true)

onMounted(async () => {
  try {
    await Promise.all([
      patient.loadAppointments(),
      patient.loadQueue(),
      patient.loadHistory()
    ])
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-5">

    <!-- Greeting -->
    <section
      class="flex items-center justify-between rounded-2xl
             bg-gradient-to-r from-[#0E4D92] to-[#1a6bbf]
             p-5 text-white"
    >
      <div>
        <p class="text-sm text-blue-200">Xin chào,</p>
        <h1 class="text-2xl font-bold">
          {{ auth.user?.name || 'Bệnh nhân' }} 👋
        </h1>
        <p class="mt-1 text-xs text-blue-200">{{ today }}</p>
      </div>

      <div
        class="flex h-16 w-16 items-center justify-center
               rounded-2xl bg-white/10 text-3xl"
      >
        🧑‍⚕️
      </div>
    </section>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 gap-3 sm:grid-cols-3">
      <RouterLink
        to="/patient/booking"
        class="rounded-2xl bg-[#0E4D92] p-4 text-white
               transition hover:-translate-y-0.5 hover:shadow-md"
      >
        <div class="mb-2 text-xl">📅</div>
        <p class="text-sm font-semibold">Đặt lịch khám</p>
      </RouterLink>

      <RouterLink
        to="/patient/queue"
        class="rounded-2xl bg-[#00A878] p-4 text-white
               transition hover:-translate-y-0.5 hover:shadow-md"
      >
        <div class="mb-2 text-xl">🔢</div>
        <p class="text-sm font-semibold">Theo dõi hàng đợi</p>
      </RouterLink>

      <RouterLink
        to="/patient/history"
        class="rounded-2xl border border-slate-200 bg-white
               p-4 text-slate-700 transition hover:bg-slate-50"
      >
        <div class="mb-2 text-xl">📋</div>
        <p class="text-sm font-semibold">Lịch sử khám</p>
      </RouterLink>
    </div>

    <!-- Upcoming Appointment -->
    <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div class="mb-4 flex items-center justify-between">
        <h2 class="font-semibold text-slate-700">Lịch hẹn sắp tới</h2>
        <RouterLink to="/patient/booking" class="text-xs font-medium text-[#0E4D92]">
          + Đặt thêm
        </RouterLink>
      </div>

      <div
        v-if="!patient.upcomingAppointments?.length"
        class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500"
      >
        {{ isLoading ? 'Đang tải lịch hẹn...' : 'Bạn chưa có lịch hẹn sắp tới.' }}
      </div>

      <div
        v-for="appointment in patient.upcomingAppointments"
        :key="appointment.id"
        class="rounded-xl border border-blue-200 bg-blue-50 p-4"
      >
        <div class="flex items-center gap-4">
          <div class="w-16 text-center">
            <p class="text-xs font-semibold text-blue-600">
              {{ appointment.appointmentDate }}
            </p>
            <p class="text-xl font-bold text-[#0E4D92]">
              {{ appointment.appointmentTime }}
            </p>
          </div>

          <div class="h-10 w-px bg-blue-200" />

          <div class="min-w-0 flex-1">
            <p class="font-semibold text-slate-800">
              {{ appointment.doctorName }}
            </p>
            <p class="text-xs text-slate-400">
              {{ appointment.specialty }}
            </p>
          </div>

          <div class="text-right">
            <span
              class="rounded-full bg-amber-50 px-2.5 py-1
                     text-xs font-medium text-amber-700"
            >
              {{ appointment.status }}
            </span>

            <p
              v-if="appointment.queueNumber"
              class="mt-1 text-xs font-bold text-[#0E4D92]"
            >
              {{ appointment.queueNumber }}
            </p>
          </div>
        </div>

        <button
          v-if="['Pending', 'Confirmed'].includes(appointment.status)"
          class="mt-3 text-xs font-medium text-red-500"
          @click="patient.cancelAppointment(appointment.id)"
        >
          Hủy lịch
        </button>
      </div>
    </section>

    <!-- Queue -->
    <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <h2 class="mb-4 font-semibold text-slate-700">Trạng thái hàng đợi</h2>

      <div v-if="patient.queue" class="flex items-center gap-5">
        <div class="text-center">
          <p class="text-xs text-slate-400">Số của bạn</p>
          <p class="text-4xl font-bold text-[#0E4D92]">
            {{ patient.queue.myTicket }}
          </p>
        </div>

        <div class="grid flex-1 grid-cols-2 gap-2">
          <div class="rounded-xl bg-slate-50 p-3">
            <p class="text-xs text-slate-400">Vị trí</p>
            <p class="font-bold">#{{ patient.queue.position }}</p>
          </div>

          <div class="rounded-xl bg-slate-50 p-3">
            <p class="text-xs text-slate-400">Chờ ~</p>
            <p class="font-bold text-amber-600">
              {{ patient.queue.estimatedWaitMinutes }} phút
            </p>
          </div>
        </div>
      </div>

      <div v-else class="text-sm text-slate-500">
        {{ isLoading ? 'Đang tải hàng đợi...' : 'Hiện tại bạn không có trong hàng đợi.' }}
      </div>

      <RouterLink
        to="/patient/queue"
        class="mt-4 block w-full rounded-xl border-2
               border-[#0E4D92] py-2 text-center text-sm
               font-semibold text-[#0E4D92]"
      >
        Xem chi tiết hàng đợi
      </RouterLink>
    </section>

    <!-- Recent History -->
    <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div class="mb-4 flex items-center justify-between">
        <h2 class="font-semibold text-slate-700">Lịch khám gần đây</h2>
        <RouterLink to="/patient/history" class="text-xs font-medium text-[#0E4D92]">
          Xem tất cả
        </RouterLink>
      </div>

      <div
        v-for="record in patient.history?.slice(0, 3)"
        :key="record.id"
        class="flex items-center gap-3 border-b border-slate-100 py-2.5 last:border-0"
      >
        <div class="flex h-9 w-9 items-center justify-center rounded-xl bg-blue-50">
          🏥
        </div>

        <div class="min-w-0 flex-1">
          <p class="truncate text-sm font-medium">
            {{ record.doctorName }}
          </p>
          <p class="truncate text-xs text-slate-400">
            {{ record.specialty }} · {{ record.diagnosis }}
          </p>
        </div>

        <p class="shrink-0 text-xs text-slate-400">
          {{ record.examinationDate }}
        </p>
      </div>
    </section>

  </div>
</template>