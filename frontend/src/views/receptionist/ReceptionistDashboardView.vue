<script setup lang="ts">
import {
  computed,
  onMounted,
  ref,
} from 'vue'

import BaseCard from '@/components/ui/BaseCard.vue'
import BaseButton from '@/components/ui/BaseButton.vue'

import { useReceptionistStore } from '@/stores/receptionist'

import { doctorApi } from '@/features/doctor/doctor.api'

import type { Doctor } from '@/features/doctor/doctor.types'


const receptionistStore =
  useReceptionistStore()


// =========================
// Doctors
// =========================

const doctors = ref<Doctor[]>([])
const loadingDoctors = ref(false)
const doctorError = ref<string | null>(null)


// =========================
// Search / Filter
// =========================

const search = ref('')
const filterDoc = ref<number | null>(null)


// =========================
// Statistics
// =========================

const total = computed(() =>
  receptionistStore.queue.length,
)

const done = computed(() =>
  receptionistStore.queue.filter(
    (q) => q.status === 'ACTIVE',
  ).length,
)

const waiting = computed(() =>
  receptionistStore.queue.filter(
    (q) =>
      q.status === 'WAITING' ||
      q.status === 'EMERGENCY',
  ).length,
)

const urgent = computed(() =>
  receptionistStore.queue.filter(
    (q) => q.status === 'EMERGENCY',
  ).length,
)


// =========================
// Search
// =========================

const filtered = computed(() => {
  const keyword = search.value
    .trim()
    .toLowerCase()

  return receptionistStore.queue.filter(
    (q) => {
      if (!keyword) return true

      return (
        q.patient.name
          .toLowerCase()
          .includes(keyword) ||
        String(q.queueNumber).includes(
          search.value.trim(),
        )
      )
    },
  )
})


// =========================
// Load doctors
// =========================

async function loadDoctors() {
  loadingDoctors.value = true
  doctorError.value = null

  try {
    const response =
      await doctorApi.getDoctors({
        status: 'ACTIVE',
      })

    doctors.value = response.items
  } catch (error) {
    doctorError.value =
      error instanceof Error
        ? error.message
        : 'Không thể tải danh sách bác sĩ'
  } finally {
    loadingDoctors.value = false
  }
}


// =========================
// Load queue
// =========================

async function loadQueue() {
  const firstDoctor = doctors.value[0]

  if (!firstDoctor) return

  try {
    await receptionistStore.loadQueue(
      firstDoctor.id,
    )
  } catch (error) {
    console.error(
      'Không thể tải hàng đợi:',
      error,
    )
  }
}


// =========================
// Initial load
// =========================

async function initialize() {
  await loadDoctors()
  await loadQueue()
}

onMounted(() => {
  initialize()
})


// =========================
// Queue helpers
// =========================
//
// API:
// GET /doctors/{doctorId}/queue
//
// QueueItem BE trả về:
//
// {
//   queueNumber,
//   patient,
//   reason,
//   status
// }
//
// Không có doctorId trong QueueItem.
// Vì vậy không dùng q.doctorId.


function getDoctorQueue() {
  return filtered.value
}


// =========================
// Current patient
// =========================

function getCurrent() {
  return getDoctorQueue().find(
    (q) => q.status === 'ACTIVE',
  )
}


// =========================
// Waiting patients
// =========================

function getWaitingList() {
  return getDoctorQueue().filter(
    (q) =>
      q.status === 'WAITING' ||
      q.status === 'EMERGENCY',
  )
}


// =========================
// Emergency patients
// =========================

function getUrgentList() {
  return getDoctorQueue().filter(
    (q) => q.status === 'EMERGENCY',
  )
}
</script>


<template>
  <div
    class="max-w-5xl mx-auto space-y-5"
  >

    <!-- =========================
         HEADER
         ========================= -->

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
        ></span>

        <span
          class="text-xs text-[#00A878] font-medium"
        >
          Thời gian thực
        </span>
      </div>
    </div>


    <!-- =========================
         STATS
         ========================= -->

    <div
      class="grid grid-cols-2 lg:grid-cols-4 gap-3"
    >

      <!-- Total -->

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
          lượt khám
        </p>
      </BaseCard>


      <!-- Active -->

      <BaseCard class="p-4">
        <p
          class="text-xs text-slate-400"
        >
          Đang khám
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


      <!-- Waiting -->

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


      <!-- Emergency -->

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


    <!-- =========================
         SEARCH / FILTER
         ========================= -->

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

        <!-- All -->

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


        <!-- Doctors -->

        <button
          v-for="doctor in doctors"
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
          {{ doctor.fullName }}
        </button>

      </div>
    </div>


    <!-- =========================
         DOCTORS
         ========================= -->

    <div
      class="grid grid-cols-1 md:grid-cols-3 gap-4"
    >

      <BaseCard
        v-for="doctor in doctors"
        :key="doctor.id"
        class="flex flex-col overflow-hidden"
      >

        <!-- =========================
             DOCTOR HEADER
             ========================= -->

        <div
          class="p-4 flex items-center justify-between border-b bg-slate-50"
        >

          <div>
            <p
              class="font-bold text-sm"
            >
              {{ doctor.fullName }}
            </p>

            <p
              class="text-xs text-slate-500"
            >
              {{ doctor.specialty.name }}
            </p>
          </div>

          <div
            class="text-right"
          >
            <p
              class="text-lg font-bold"
            >
              {{ getWaitingList().length }}
            </p>

            <p
              class="text-[10px] text-slate-400"
            >
              đang chờ
            </p>
          </div>

        </div>


        <!-- =========================
             DOCTOR ERROR
             ========================= -->

        <p
          v-if="doctorError"
          class="px-4 pt-3 text-xs text-red-500"
        >
          {{ doctorError }}
        </p>


        <!-- =========================
             URGENT
             ========================= -->

        <div
          v-if="getUrgentList().length"
          class="mx-4 mt-3 bg-red-50 border border-red-200 rounded-xl p-2.5 text-xs text-red-700"
        >
          <span
            class="font-semibold"
          >
            ⚠️
            {{ getUrgentList().length }}
            trường hợp ưu tiên
          </span>
        </div>


        <!-- =========================
             CURRENT
             ========================= -->

        <div
          v-if="getCurrent()"
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
              {{ getCurrent()?.queueNumber }}
            </span>

            <div>
              <p
                class="text-sm font-semibold text-slate-800"
              >
                {{ getCurrent()?.patient.name }}
              </p>

              <p
                class="text-xs text-slate-400"
              >
                {{ getCurrent()?.reason }}
              </p>
            </div>

          </div>

        </div>


        <!-- =========================
             WAITING LIST
             ========================= -->

        <div
          class="flex-1 overflow-y-auto max-h-48 px-4 py-3 space-y-1.5"
        >

          <p
            v-if="!getWaitingList().length"
            class="text-xs text-slate-400 text-center py-4"
          >
            Không còn bệnh nhân chờ
          </p>


          <div
            v-for="patient in getWaitingList()"
            :key="patient.queueNumber"
            class="group flex items-center gap-2 px-2.5 py-2 rounded-xl hover:bg-slate-50"
          >

            <!-- Emergency -->

            <span
              v-if="
                patient.status ===
                'EMERGENCY'
              "
              class="text-red-500"
            >
              ⚠️
            </span>


            <!-- Queue number -->

            <span
              class="text-xs font-bold text-slate-400 w-9"
            >
              {{ patient.queueNumber }}
            </span>


            <!-- Patient -->

            <span
              class="flex-1 text-sm text-slate-700 truncate"
            >
              {{ patient.patient.name }}
            </span>


            <!-- Reason -->

            <span
              class="text-xs text-slate-400 truncate max-w-24"
            >
              {{ patient.reason }}
            </span>


            <!-- Skip -->

            <button
              type="button"
              disabled
              class="text-[10px] text-slate-300 cursor-not-allowed"
            >
              Bỏ
            </button>

          </div>

        </div>


        <!-- =========================
             CALL NEXT
             ========================= -->

        <div
          class="p-4 border-t border-slate-100"
        >

          <BaseButton
            class="w-full"
            disabled
          >
            📢 Gọi tiếp

            <span
              v-if="getWaitingList().length"
              class="ml-2 bg-white/20 px-2 py-0.5 rounded-lg"
            >
              {{
                getWaitingList()[0]
                  ?.queueNumber
              }}
            </span>
          </BaseButton>

        </div>

      </BaseCard>

    </div>

  </div>
</template>
