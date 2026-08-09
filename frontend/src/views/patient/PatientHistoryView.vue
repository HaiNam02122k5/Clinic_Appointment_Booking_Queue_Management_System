<script setup lang="ts">
import { onMounted } from 'vue'
import { usePatientStore } from '@/stores/patient'

const patient = usePatientStore()

onMounted(() => {
  patient.loadHistory()
})
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-4">

    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Lịch sử khám bệnh
      </h1>

      <p class="mt-1 text-sm text-slate-400">
        {{ patient.history.length }}
        lần khám đã lưu
      </p>
    </div>

    <div
      v-if="patient.loading"
      class="py-10 text-center text-sm
             text-slate-400"
    >
      Đang tải lịch sử khám...
    </div>

    <div
      v-else-if="patient.history.length === 0"
      class="rounded-2xl border border-slate-200
             bg-white p-6 text-center text-sm
             text-slate-400"
    >
      Chưa có lịch sử khám.
    </div>

    <article
      v-for="record in patient.history"
      :key="record.id"
      class="rounded-2xl border border-slate-200
             bg-white p-5 shadow-sm"
    >

      <div
        class="mb-3 flex items-start
               justify-between gap-3"
      >

        <div>
          <p class="text-xs text-slate-400">
            {{ record.examinationDate }}
          </p>

          <h2 class="font-bold text-slate-800">
            {{ record.doctorName }}
          </h2>
        </div>

        <span
          class="rounded-full border border-blue-200
                 bg-blue-50 px-2.5 py-1 text-xs
                 text-blue-700"
        >
          {{ record.specialty }}
        </span>

      </div>

      <div
        class="space-y-3 rounded-xl bg-slate-50 p-4
               text-sm"
      >

        <div>
          <p
            class="text-xs font-medium uppercase
                   tracking-wide text-slate-400"
          >
            Chẩn đoán
          </p>

          <p class="mt-1 text-slate-800">
            {{ record.diagnosis }}
          </p>
        </div>

        <div
          class="border-t border-slate-200 pt-3"
        >
          <p
            class="text-xs font-medium uppercase
                   tracking-wide text-slate-400"
          >
            Đơn thuốc
          </p>

          <p class="mt-1 text-slate-800">
            {{ record.prescription }}
          </p>
        </div>

        <div
          v-if="record.note"
          class="border-t border-slate-200 pt-3"
        >
          <p
            class="text-xs font-medium uppercase
                   tracking-wide text-slate-400"
          >
            Ghi chú
          </p>

          <p class="mt-1 italic text-slate-600">
            {{ record.note }}
          </p>
        </div>

      </div>

    </article>

  </div>
</template>