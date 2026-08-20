<script setup lang="ts">
import { computed, ref } from 'vue'

import {
  DAYS,
  SCHEDULE_ROWS,
} from '@/features/admin/admin.mock'

const selectedWeek = ref('11/08/2026 - 17/08/2026')
const search = ref('')
const selectedSpecialty = ref('all')

const specialties = computed(() => {
  return [...new Set(SCHEDULE_ROWS.map((doctor) => doctor.spec))]
})

const filteredRows = computed(() => {
  const keyword = search.value.trim().toLowerCase()

  return SCHEDULE_ROWS.filter((doctor) => {
    const matchesSearch =
      !keyword ||
      doctor.name.toLowerCase().includes(keyword) ||
      doctor.spec.toLowerCase().includes(keyword)

    const matchesSpecialty =
      selectedSpecialty.value === 'all' ||
      doctor.spec === selectedSpecialty.value

    return matchesSearch && matchesSpecialty
  })
})

function slotClass(slot: string | null) {
  if (!slot) {
    return ''
  }

  if (slot === 'Cả ngày') {
    return 'bg-violet-50 text-violet-700 border-violet-100'
  }

  if (slot === 'Sáng') {
    return 'bg-blue-50 text-blue-700 border-blue-100'
  }

  return 'bg-amber-50 text-amber-700 border-amber-100'
}

function resetFilters() {
  search.value = ''
  selectedSpecialty.value = 'all'
}

function previousWeek() {
  // Chưa nối API nên hiện chỉ là UI.
}

function nextWeek() {
  // Chưa nối API nên hiện chỉ là UI.
}
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         TITLE
    ========================== -->

    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Lịch làm việc
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Quản lý lịch làm việc của bác sĩ
      </p>
    </div>

    <!-- =========================
         TOOLBAR
    ========================== -->

    <div
      class="rounded-xl border border-slate-200
             bg-white p-5"
    >
      <div
        class="flex flex-col gap-3
               lg:flex-row lg:items-center"
      >

        <!-- SEARCH -->

        <div class="relative flex-1">
          <input
            v-model="search"
            type="text"
            placeholder="Tìm kiếm bác sĩ..."
            class="w-full rounded-lg
                   border border-slate-200
                   bg-white px-3 py-2.5 pl-10
                   text-sm text-slate-800
                   placeholder:text-slate-400
                   focus:border-violet-500
                   focus:outline-none"
          />

          <svg
            class="absolute left-3 top-1/2
                   h-4 w-4 -translate-y-1/2
                   text-slate-400"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <circle cx="11" cy="11" r="7" />
            <path d="m20 20-3.5-3.5" />
          </svg>
        </div>

        <!-- SPECIALTY -->

        <select
          v-model="selectedSpecialty"
          class="rounded-lg border border-slate-200
                 bg-white px-3 py-2.5
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
        >
          <option value="all">
            Tất cả chuyên khoa
          </option>

          <option
            v-for="specialty in specialties"
            :key="specialty"
            :value="specialty"
          >
            {{ specialty }}
          </option>
        </select>

        <!-- WEEK -->

        <select
          v-model="selectedWeek"
          class="rounded-lg border border-slate-200
                 bg-white px-3 py-2.5
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
        >
          <option value="11/08/2026 - 17/08/2026">
            11/08/2026 - 17/08/2026
          </option>

          <option value="18/08/2026 - 24/08/2026">
            18/08/2026 - 24/08/2026
          </option>
        </select>

        <!-- RESET -->

        <button
          class="rounded-lg border border-slate-200
                 px-4 py-2.5 text-sm font-medium
                 text-slate-600 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <!-- ADD -->

        <button
          class="rounded-lg bg-violet-600
                 px-4 py-2.5 text-sm font-semibold
                 text-white hover:bg-violet-700"
        >
          + Thêm lịch
        </button>

      </div>
    </div>

    <!-- =========================
         WEEK NAVIGATION
    ========================== -->

    <div
      class="flex items-center justify-between
             rounded-xl border border-slate-200
             bg-white px-5 py-4"
    >
      <button
        class="rounded-lg border border-slate-200
               px-3 py-2 text-sm text-slate-600
               hover:bg-slate-50"
        @click="previousWeek"
      >
        ← Tuần trước
      </button>

      <div class="text-center">
        <div class="text-sm font-semibold text-slate-800">
          Tuần làm việc
        </div>

        <div class="mt-1 text-xs text-slate-400">
          {{ selectedWeek }}
        </div>
      </div>

      <button
        class="rounded-lg border border-slate-200
               px-3 py-2 text-sm text-slate-600
               hover:bg-slate-50"
        @click="nextWeek"
      >
        Tuần sau →
      </button>
    </div>

    <!-- =========================
         SCHEDULE TABLE
    ========================== -->

    <div
      class="overflow-hidden rounded-xl
             border border-slate-200 bg-white"
    >

      <div class="border-b border-slate-200 p-5">
        <h2 class="text-sm font-semibold text-slate-800">
          Lịch làm việc bác sĩ
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          {{ filteredRows.length }} bác sĩ
        </p>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full min-w-[1000px] text-sm">

          <!-- HEADER -->

          <thead>
            <tr
              class="border-b border-slate-200
                     bg-slate-50"
            >
              <th
                class="w-64 px-5 py-4 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Bác sĩ
              </th>

              <th
                class="w-32 px-4 py-4 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Chuyên khoa
              </th>

              <th
                v-for="day in DAYS"
                :key="day"
                class="px-3 py-4 text-center
                       text-xs font-semibold
                       text-slate-500"
              >
                {{ day }}
              </th>

              <th
                class="px-4 py-4 text-right
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Thao tác
              </th>
            </tr>
          </thead>

          <!-- BODY -->

          <tbody class="divide-y divide-slate-100">

            <tr
              v-for="doctor in filteredRows"
              :key="doctor.name"
              class="hover:bg-slate-50"
            >

              <!-- DOCTOR -->

              <td class="px-5 py-4">
                <div class="font-medium text-slate-800">
                  {{ doctor.name }}
                </div>
              </td>

              <!-- SPECIALTY -->

              <td class="px-4 py-4">
                <span class="text-xs text-slate-500">
                  {{ doctor.spec }}
                </span>
              </td>

              <!-- SLOTS -->

              <td
                v-for="(slot, index) in doctor.slots"
                :key="`${doctor.name}-${index}`"
                class="px-2 py-3 text-center"
              >
                <div
                  v-if="slot"
                  class="mx-auto rounded-md border
                         px-2 py-2 text-xs font-medium"
                  :class="slotClass(slot)"
                >
                  {{ slot }}
                </div>

                <span
                  v-else
                  class="text-slate-300"
                >
                  —
                </span>
              </td>

              <!-- ACTIONS -->

              <td class="px-4 py-4">
                <div
                  class="flex justify-end gap-2"
                >
                  <button
                    class="rounded-md px-2.5 py-1.5
                           text-xs font-medium
                           text-slate-600
                           hover:bg-slate-100"
                  >
                    Xem
                  </button>

                  <button
                    class="rounded-md px-2.5 py-1.5
                           text-xs font-medium
                           text-violet-600
                           hover:bg-violet-50"
                  >
                    Sửa
                  </button>
                </div>
              </td>

            </tr>

            <!-- EMPTY -->

            <tr v-if="filteredRows.length === 0">
              <td
                :colspan="DAYS.length + 3"
                class="px-5 py-12 text-center
                       text-sm text-slate-400"
              >
                Không tìm thấy lịch làm việc phù hợp.
              </td>
            </tr>

          </tbody>

        </table>
      </div>

    </div>

    <!-- =========================
         LEGEND
    ========================== -->

    <div
      class="flex flex-wrap items-center gap-5
             rounded-xl border border-slate-200
             bg-white px-5 py-4"
    >
      <span class="text-xs font-semibold text-slate-600">
        Chú thích:
      </span>

      <div class="flex items-center gap-2">
        <span class="h-3 w-3 rounded bg-blue-100" />
        <span class="text-xs text-slate-500">
          Sáng
        </span>
      </div>

      <div class="flex items-center gap-2">
        <span class="h-3 w-3 rounded bg-amber-100" />
        <span class="text-xs text-slate-500">
          Chiều
        </span>
      </div>

      <div class="flex items-center gap-2">
        <span class="h-3 w-3 rounded bg-violet-100" />
        <span class="text-xs text-slate-500">
          Cả ngày
        </span>
      </div>
    </div>

  </div>
</template>
