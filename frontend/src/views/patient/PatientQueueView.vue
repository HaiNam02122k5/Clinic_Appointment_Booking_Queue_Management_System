<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { usePatientStore } from '@/stores/patient'

const patient = usePatientStore()

let timer: number | undefined

onMounted(async () => {
  await patient.loadQueue()

  timer = window.setInterval(() => {
    patient.loadQueue()
  }, 30000)
})

onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
  }
})
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-5">

    <div class="flex items-start justify-between">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">
          Theo dõi hàng đợi
        </h1>

        <p class="mt-1 text-sm text-slate-400">
          Cập nhật gần thời gian thực
        </p>
      </div>

      <div
        class="flex items-center gap-1.5 text-xs
               font-medium text-[#00A878]"
      >
        <span
          class="h-2 w-2 animate-pulse rounded-full
                 bg-[#00A878]"
        />

        Đang cập nhật
      </div>
    </div>

    <div
      v-if="patient.queue"
      class="rounded-2xl bg-gradient-to-br
             from-[#0E4D92] to-[#1a6bbf]
             p-6 text-white"
    >

      <div class="flex items-center gap-5">

        <div class="text-center">
          <p class="text-xs uppercase text-blue-200">
            Số của bạn
          </p>

          <p class="text-5xl font-bold">
            {{ patient.queue.myTicket }}
          </p>
        </div>

        <div class="h-16 w-px bg-white/20" />

        <div class="grid flex-1 grid-cols-2 gap-y-3">

          <div>
            <p class="text-xs text-blue-200">
              Vị trí hàng đợi
            </p>

            <p class="text-xl font-bold">
              #{{ patient.queue.position }}
            </p>
          </div>

          <div>
            <p class="text-xs text-blue-200">
              Chờ ước tính
            </p>

            <p class="text-xl font-bold text-amber-300">
              ~{{ patient.queue.estimatedWaitMinutes }}
              phút
            </p>
          </div>

          <div>
            <p class="text-xs text-blue-200">
              Bác sĩ
            </p>

            <p class="text-sm font-medium">
              {{ patient.queue.doctorName }}
            </p>
          </div>

          <div>
            <p class="text-xs text-blue-200">
              Giờ hẹn
            </p>

            <p class="font-bold">
              {{ patient.queue.appointmentTime }}
            </p>
          </div>

        </div>
      </div>

      <div
        class="mt-4 rounded-xl bg-white/10
               px-4 py-2.5 text-xs text-blue-100"
      >
        🔔 Bạn sẽ nhận thông báo khi sắp đến lượt.
      </div>

    </div>

    <!-- Queue List -->
    <section
      v-if="patient.queue"
      class="rounded-2xl border border-slate-200
             bg-white p-5 shadow-sm"
    >

      <div class="mb-4 flex items-center justify-between">
        <h2 class="font-semibold text-slate-700">
          Danh sách hàng đợi
        </h2>

        <span
          class="rounded-full bg-blue-50 px-2.5 py-1
                 text-xs font-medium text-blue-700"
        >
          {{
            patient.queue.entries.filter(
              (x) => x.status === 'Waiting',
            ).length
          }}
          đang chờ
        </span>
      </div>

      <div class="space-y-1.5">

        <div
          v-for="(entry, index) in patient.queue.entries"
          :key="entry.ticket"
          class="flex items-center gap-3
                 rounded-xl px-3 py-3"
          :class="
            entry.ticket === patient.queue?.myTicket
              ? 'border border-blue-200 bg-blue-50'
              : 'hover:bg-slate-50'
          "
        >

          <span class="w-4 text-xs text-slate-400">
            {{ index + 1 }}
          </span>

          <span
            class="w-12 text-sm font-bold"
            :class="
              entry.status === 'InProgress'
                ? 'text-[#00A878]'
                : entry.ticket === patient.queue?.myTicket
                  ? 'text-[#0E4D92]'
                  : 'text-slate-400'
            "
          >
            {{ entry.ticket }}
          </span>

          <div class="min-w-0 flex-1">

            <p class="truncate text-sm font-medium">
              {{ entry.patientName }}
            </p>

            <span
              v-if="entry.urgent"
              class="text-xs font-medium text-red-500"
            >
              ⚠ Ưu tiên
            </span>

          </div>

          <span class="text-xs text-slate-400">
            {{ entry.appointmentTime }}
          </span>

          <span
            v-if="entry.status === 'InProgress'"
            class="rounded-full bg-emerald-50
                   px-2.5 py-1 text-xs
                   text-emerald-700"
          >
            Đang khám
          </span>

          <span
            v-else-if="
              entry.ticket === patient.queue?.myTicket
            "
            class="rounded-full bg-blue-50
                   px-2.5 py-1 text-xs
                   text-blue-700"
          >
            Của bạn
          </span>

          <span
            v-else
            class="rounded-full bg-slate-100
                   px-2.5 py-1 text-xs
                   text-slate-500"
          >
            Đang chờ
          </span>

        </div>

      </div>
    </section>

    <p
      v-if="patient.error"
      class="rounded-xl bg-red-50 p-3 text-sm
             text-red-600"
    >
      {{ patient.error }}
    </p>

  </div>
</template>