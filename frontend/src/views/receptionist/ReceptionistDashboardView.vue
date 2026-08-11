<script setup lang="ts">
import { computed, ref } from 'vue'

import BaseCard from '@/components/ui/BaseCard.vue'
import BaseButton from '@/components/ui/BaseButton.vue'

import { DOCTORS, INIT_QUEUE } from '@/features/receptionist/receptionist.data'
import type { QueueItem } from '@/features/receptionist/receptionist.types'

const queue = ref<QueueItem[]>(
  INIT_QUEUE.map((item) => ({ ...item })),
)

const search = ref('')
const filterDoc = ref<number | null>(null)

const total = computed(() => queue.value.length)

const done = computed(() =>
  queue.value.filter(
    (q) => q.status === 'completed',
  ).length,
)

const waiting = computed(() =>
  queue.value.filter(
    (q) => q.status === 'waiting',
  ).length,
)

const urgent = computed(() =>
  queue.value.filter(
    (q) =>
      q.status === 'waiting' &&
      q.urgent,
  ).length,
)

const filtered = computed(() => {
  return queue.value.filter((q) => {
    const keyword = search.value
      .trim()
      .toLowerCase()

    const matchSearch =
      !keyword ||
      q.name
        .toLowerCase()
        .includes(keyword) ||
      q.ticket.includes(
        search.value
          .trim()
          .toUpperCase(),
      )

    const matchDoctor =
      filterDoc.value === null ||
      q.docId === filterDoc.value

    return matchSearch && matchDoctor
  })
})

function getDoctorQueue(
  docId: number,
) {
  return filtered.value.filter(
    (q) => q.docId === docId,
  )
}

function getCurrent(
  docId: number,
) {
  return getDoctorQueue(docId).find(
    (q) => q.status === 'in-progress',
  )
}

function getWaitingList(
  docId: number,
) {
  return getDoctorQueue(docId).filter(
    (q) => q.status === 'waiting',
  )
}

function getUrgentList(
  docId: number,
) {
  return getWaitingList(docId).filter(
    (q) => q.urgent,
  )
}

function callNext(docId: number) {
  const current = queue.value.find(
    (q) =>
      q.docId === docId &&
      q.status === 'in-progress',
  )

  const next =
    queue.value.find(
      (q) =>
        q.docId === docId &&
        q.status === 'waiting' &&
        q.urgent,
    ) ??
    queue.value.find(
      (q) =>
        q.docId === docId &&
        q.status === 'waiting',
    )

  if (!next) return

  queue.value = queue.value.map((q) => {
    if (q.ticket === current?.ticket) {
      return {
        ...q,
        status: 'completed',
      }
    }

    if (q.ticket === next.ticket) {
      return {
        ...q,
        status: 'in-progress',
      }
    }

    return q
  })
}

function skip(ticket: string) {
  queue.value = queue.value.map((q) =>
    q.ticket === ticket
      ? {
          ...q,
          status: 'skipped',
        }
      : q,
  )
}

function doctorHeaderClass(
  color: string,
) {
  const classes: Record<
    string,
    string
  > = {
    blue:
      'border-blue-200 bg-blue-50 text-[#0E4D92]',
    violet:
      'border-violet-200 bg-violet-50 text-violet-700',
    emerald:
      'border-emerald-200 bg-emerald-50 text-emerald-700',
  }

  return classes[color] ?? ''
}
</script>

<template>
  <div
    class="max-w-5xl mx-auto space-y-5"
  >

    <!-- Header -->
    <div
      class="flex items-center justify-between"
    >
      <div>
        <h1
          class="text-2xl font-bold text-slate-800"
        >
          Bảng điều khiển hàng đợi
        </h1>

        <p
          class="text-sm text-slate-400 mt-1"
        >
          Hôm nay
        </p>
      </div>

      <div
        class="flex items-center gap-2"
      >
        <span
          class="w-2 h-2 bg-[#00A878] rounded-full animate-pulse"
        />

        <span
          class="text-xs text-[#00A878] font-medium"
        >
          Thời gian thực
        </span>
      </div>
    </div>

    <!-- Stats -->
    <div
      class="grid grid-cols-2 lg:grid-cols-4 gap-3"
    >
      <BaseCard class="p-4">
        <p
          class="text-xs text-slate-400"
        >
          Tổng hôm nay
        </p>

        <p
          class="text-2xl font-bold text-slate-800 mt-1"
        >
          {{ total }}
        </p>

        <p
          class="text-xs text-slate-400"
        >
          lịch hẹn
        </p>
      </BaseCard>

      <BaseCard class="p-4">
        <p
          class="text-xs text-slate-400"
        >
          Đã hoàn thành
        </p>

        <p
          class="text-2xl font-bold text-[#00A878] mt-1"
        >
          {{ done }}
        </p>

        <p
          class="text-xs text-slate-400"
        >
          lượt khám
        </p>
      </BaseCard>

      <BaseCard class="p-4">
        <p
          class="text-xs text-slate-400"
        >
          Đang chờ
        </p>

        <p
          class="text-2xl font-bold text-amber-500 mt-1"
        >
          {{ waiting }}
        </p>

        <p
          class="text-xs text-slate-400"
        >
          bệnh nhân
        </p>
      </BaseCard>

      <BaseCard class="p-4">
        <p
          class="text-xs text-slate-400"
        >
          Ưu tiên khẩn
        </p>

        <p
          class="text-2xl font-bold text-red-500 mt-1"
        >
          {{ urgent }}
        </p>

        <p
          class="text-xs text-slate-400"
        >
          cần xử lý
        </p>
      </BaseCard>
    </div>

    <!-- Search -->
    <div
      class="flex flex-wrap items-center gap-3"
    >
      <input
        v-model="search"
        type="text"
        placeholder="Tìm tên hoặc số thứ tự…"
        class="border border-slate-200 rounded-xl px-4 py-2 text-sm w-full sm:w-64 focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
      />

      <div class="flex gap-2">

        <button
          type="button"
          class="px-3 py-2 rounded-xl text-xs font-medium border"
          :class="
            filterDoc === null
              ? 'bg-[#0E4D92] text-white border-[#0E4D92]'
              : 'border-slate-200 text-slate-600'
          "
          @click="filterDoc = null"
        >
          Tất cả
        </button>

        <button
          v-for="doctor in DOCTORS"
          :key="doctor.id"
          type="button"
          class="px-3 py-2 rounded-xl text-xs font-medium border"
          :class="
            filterDoc === doctor.id
              ? 'bg-[#0E4D92] text-white border-[#0E4D92]'
              : 'border-slate-200 text-slate-600'
          "
          @click="filterDoc = doctor.id"
        >
          {{ doctor.prefix }}
        </button>

      </div>
    </div>

    <!-- Doctors -->
    <div
      class="grid grid-cols-1 md:grid-cols-3 gap-4"
    >

      <BaseCard
        v-for="doctor in DOCTORS"
        :key="doctor.id"
        class="flex flex-col overflow-hidden"
      >

        <!-- Doctor -->
        <div
          class="p-4 flex items-center justify-between border-b"
          :class="
            doctorHeaderClass(
              doctor.color,
            )
          "
        >
          <div>
            <p
              class="font-bold text-sm"
            >
              {{ doctor.name }}
            </p>

            <p
              class="text-xs opacity-70"
            >
              {{ doctor.specialty }}
              ·
              {{ doctor.room }}
            </p>
          </div>

          <div
            class="text-right"
          >
            <p
              class="text-lg font-bold"
            >
              {{
                getWaitingList(
                  doctor.id,
                ).length
              }}
            </p>

            <p
              class="text-[10px] opacity-70"
            >
              đang chờ
            </p>
          </div>
        </div>

        <!-- Urgent -->
        <div
          v-if="
            getUrgentList(
              doctor.id,
            ).length
          "
          class="mx-4 mt-3 bg-red-50 border border-red-200 rounded-xl p-2.5 text-xs text-red-700"
        >
          ⚠️
          <span class="font-semibold">
            {{
              getUrgentList(
                doctor.id,
              ).length
            }}
            trường hợp ưu tiên
          </span>
        </div>

        <!-- Current -->
        <div
          v-if="
            getCurrent(
              doctor.id,
            )
          "
          class="mx-4 mt-3 bg-emerald-50 border border-emerald-200 rounded-xl p-3"
        >
          <p
            class="text-[10px] font-bold text-emerald-600 uppercase tracking-widest mb-1.5"
          >
            Đang khám
          </p>

          <div
            class="flex items-center gap-2"
          >
            <span
              class="text-2xl font-bold text-emerald-700"
            >
              {{
                getCurrent(
                  doctor.id,
                )?.ticket
              }}
            </span>

            <div>
              <p
                class="text-sm font-semibold text-slate-800"
              >
                {{
                  getCurrent(
                    doctor.id,
                  )?.name
                }}
              </p>

              <p
                class="text-xs text-slate-400"
              >
                {{
                  getCurrent(
                    doctor.id,
                  )?.time
                }}
              </p>
            </div>
          </div>
        </div>

        <!-- Waiting -->
        <div
          class="flex-1 overflow-y-auto max-h-48 px-4 py-3 space-y-1.5"
        >

          <p
            v-if="
              !getWaitingList(
                doctor.id,
              ).length
            "
            class="text-xs text-slate-400 text-center py-4"
          >
            Không còn bệnh nhân chờ
          </p>

          <div
            v-for="patient in getWaitingList(
              doctor.id,
            )"
            :key="patient.ticket"
            class="group flex items-center gap-2 px-2.5 py-2 rounded-xl hover:bg-slate-50"
          >
            <span
              v-if="patient.urgent"
              class="text-red-500"
            >
              ⚠️
            </span>

            <span
              class="text-xs font-bold text-slate-400 w-9"
            >
              {{ patient.ticket }}
            </span>

            <span
              class="flex-1 text-sm text-slate-700 truncate"
            >
              {{ patient.name }}
            </span>

            <span
              class="text-xs text-slate-400"
            >
              {{ patient.time }}
            </span>

            <button
              type="button"
              class="text-[10px] text-red-400 opacity-0 group-hover:opacity-100"
              @click="
                skip(patient.ticket)
              "
            >
              Bỏ
            </button>
          </div>

        </div>

        <!-- Call next -->
        <div
          class="p-4 border-t border-slate-100"
        >
          <BaseButton
            class="w-full"
            :disabled="
              !getWaitingList(
                doctor.id,
              ).length
            "
            @click="
              callNext(doctor.id)
            "
          >
            📢 Gọi tiếp

            <span
              v-if="
                getWaitingList(
                  doctor.id,
                ).length
              "
              class="ml-2 bg-white/20 px-2 py-0.5 rounded-lg"
            >
              {{
                getWaitingList(
                  doctor.id,
                )[0]?.ticket
              }}
            </span>
          </BaseButton>
        </div>

      </BaseCard>

    </div>
  </div>
</template>
