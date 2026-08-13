<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import AdminPagination from '@/features/admin/components/AdminPagination.vue'

import { DOCTORS } from '@/features/admin/admin.mock'

const search = ref('')
const specialtyFilter = ref('all')
const statusFilter = ref<'all' | 'active' | 'inactive'>('all')
const currentPage = ref(1)

const pageSize = 5

// ================================
// SPECIALTIES
// ================================

const specialties = computed(() => {
  return [...new Set(DOCTORS.map((doctor) => doctor.specialty))]
})

// ================================
// FILTER
// ================================

const filteredDoctors = computed(() => {
  const keyword = search.value.trim().toLowerCase()

  return DOCTORS.filter((doctor) => {
    const matchesSearch =
      !keyword ||
      doctor.name.toLowerCase().includes(keyword) ||
      doctor.specialty.toLowerCase().includes(keyword) ||
      doctor.room.toLowerCase().includes(keyword)

    const matchesSpecialty =
      specialtyFilter.value === 'all' ||
      doctor.specialty === specialtyFilter.value

    const matchesStatus =
      statusFilter.value === 'all' ||
      doctor.status === statusFilter.value

    return (
      matchesSearch &&
      matchesSpecialty &&
      matchesStatus
    )
  })
})

// ================================
// PAGINATION
// ================================

const totalPages = computed(() => {
  return Math.max(
    1,
    Math.ceil(filteredDoctors.value.length / pageSize),
  )
})

const pagedDoctors = computed(() => {
  const start =
    (currentPage.value - 1) * pageSize

  return filteredDoctors.value.slice(
    start,
    start + pageSize,
  )
})

watch(
  [search, specialtyFilter, statusFilter],
  () => {
    currentPage.value = 1
  },
)

// ================================
// HELPERS
// ================================

function statusText(
  status: 'active' | 'inactive',
) {
  return status === 'active'
    ? 'Đang hoạt động'
    : 'Ngừng hoạt động'
}

function statusClass(
  status: 'active' | 'inactive',
) {
  return status === 'active'
    ? 'bg-green-50 text-green-700'
    : 'bg-slate-100 text-slate-500'
}

function resetFilters() {
  search.value = ''
  specialtyFilter.value = 'all'
  statusFilter.value = 'all'
  currentPage.value = 1
}
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         TITLE
    ========================== -->

    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Quản lý bác sĩ
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Quản lý thông tin và lịch làm việc của bác sĩ
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
          v-model="specialtyFilter"
          class="rounded-lg
                 border border-slate-200
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

        <!-- STATUS -->

        <select
          v-model="statusFilter"
          class="rounded-lg
                 border border-slate-200
                 bg-white px-3 py-2.5
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
        >
          <option value="all">
            Tất cả trạng thái
          </option>

          <option value="active">
            Đang hoạt động
          </option>

          <option value="inactive">
            Ngừng hoạt động
          </option>
        </select>

        <!-- RESET -->

        <button
          class="rounded-lg
                 border border-slate-200
                 px-4 py-2.5
                 text-sm font-medium
                 text-slate-600
                 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <!-- ADD -->

        <button
          class="rounded-lg
                 bg-violet-600
                 px-4 py-2.5
                 text-sm font-semibold
                 text-white
                 hover:bg-violet-700"
        >
          + Thêm bác sĩ
        </button>

      </div>
    </div>

    <!-- =========================
         TABLE
    ========================== -->

    <div
      class="rounded-xl border border-slate-200
             bg-white p-5"
    >

      <div
        class="mb-4 flex items-center
               justify-between"
      >
        <div>
          <h2
            class="text-sm font-semibold
                   text-slate-800"
          >
            Danh sách bác sĩ
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            {{ filteredDoctors.length }} bác sĩ
          </p>
        </div>
      </div>

      <div
        class="overflow-hidden
               rounded-lg border
               border-slate-200"
      >
        <table class="w-full text-sm">

          <!-- HEADER -->

          <thead>
            <tr
              class="border-b
                     border-slate-200
                     bg-slate-50"
            >
              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                #
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Bác sĩ
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Chuyên khoa
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Phòng
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Lịch làm việc
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Số bệnh nhân
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Trạng thái
              </th>

              <th
                class="px-4 py-3 text-right
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
              v-for="(doctor, index) in pagedDoctors"
              :key="doctor.id"
              class="hover:bg-slate-50"
            >

              <!-- NUMBER -->

              <td
                class="px-4 py-3
                       text-xs text-slate-400"
              >
                {{
                  (currentPage - 1) * pageSize +
                  index +
                  1
                }}
              </td>

              <!-- DOCTOR -->

              <td class="px-4 py-3">
                <div
                  class="font-medium
                         text-slate-800"
                >
                  {{ doctor.name }}
                </div>
              </td>

              <!-- SPECIALTY -->

              <td
                class="px-4 py-3
                       text-slate-600"
              >
                {{ doctor.specialty }}
              </td>

              <!-- ROOM -->

              <td
                class="px-4 py-3
                       text-slate-600"
              >
                {{ doctor.room }}
              </td>

              <!-- SCHEDULE -->

              <td
                class="px-4 py-3
                       text-xs text-slate-500"
              >
                {{ doctor.schedule }}
              </td>

              <!-- PATIENTS -->

              <td
                class="px-4 py-3
                       font-medium
                       text-slate-700"
              >
                {{ doctor.patients }}
              </td>

              <!-- STATUS -->

              <td class="px-4 py-3">
                <span
                  class="rounded-md px-2.5 py-1
                         text-xs font-medium"
                  :class="
                    statusClass(doctor.status)
                  "
                >
                  {{ statusText(doctor.status) }}
                </span>
              </td>

              <!-- ACTIONS -->

              <td class="px-4 py-3">
                <div
                  class="flex justify-end
                         gap-2"
                >
                  <button
                    class="rounded-md
                           px-2.5 py-1.5
                           text-xs font-medium
                           text-slate-600
                           hover:bg-slate-100"
                  >
                    Xem
                  </button>

                  <button
                    class="rounded-md
                           px-2.5 py-1.5
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

            <tr
              v-if="pagedDoctors.length === 0"
            >
              <td
                colspan="8"
                class="px-4 py-12
                       text-center
                       text-sm text-slate-400"
              >
                Không tìm thấy bác sĩ phù hợp.
              </td>
            </tr>

          </tbody>

        </table>
      </div>

      <!-- PAGINATION -->

      <AdminPagination
        v-model:current-page="currentPage"
        :total-pages="totalPages"
      />

    </div>

  </div>
</template>
