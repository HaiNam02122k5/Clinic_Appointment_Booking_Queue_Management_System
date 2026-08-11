<script setup lang="ts">
import { ref } from 'vue'

const currentWeek = ref('Tuần hiện tại')
const search = ref('')
const specialtyFilter = ref('all')
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         PAGE HEADER
    ========================== -->

    <div class="flex flex-wrap items-start justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">
          Quản lý lịch làm việc
        </h1>

        <p class="mt-1 text-sm text-slate-500">
          Theo dõi và quản lý lịch làm việc của bác sĩ
        </p>
      </div>

      <button
        class="rounded-lg bg-brand-600 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-brand-700"
      >
        + Thêm lịch làm việc
      </button>
    </div>


    <!-- =========================
         FILTER / TOOLBAR
    ========================== -->

    <div class="rounded-xl border border-slate-200 bg-white p-4">

      <div class="flex flex-wrap items-center gap-3">

        <!-- Search -->
        <div class="min-w-[240px] flex-1">
          <input
            v-model="search"
            type="text"
            placeholder="Tìm tên bác sĩ..."
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none transition placeholder:text-slate-400 focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>

        <!-- Specialty -->
        <select
          v-model="specialtyFilter"
          class="h-10 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-600 outline-none focus:border-brand-500"
        >
          <option value="all">
            Tất cả chuyên khoa
          </option>

          <option value="cardiology">
            Tim mạch
          </option>

          <option value="internal">
            Nội khoa
          </option>

          <option value="surgery">
            Ngoại khoa
          </option>

          <option value="dermatology">
            Da liễu
          </option>

          <option value="neurology">
            Thần kinh
          </option>
        </select>

        <!-- Week -->
        <select
          v-model="currentWeek"
          class="h-10 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-600 outline-none focus:border-brand-500"
        >
          <option value="Tuần hiện tại">
            Tuần hiện tại
          </option>

          <option value="Tuần trước">
            Tuần trước
          </option>

          <option value="Tuần sau">
            Tuần sau
          </option>
        </select>

      </div>

    </div>


    <!-- =========================
         SCHEDULE TABLE
    ========================== -->

    <div class="overflow-hidden rounded-xl border border-slate-200 bg-white">

      <div class="overflow-x-auto">

        <table class="w-full min-w-[1100px] border-collapse">

          <!-- Table header -->
          <thead>

            <tr class="border-b border-slate-200 bg-slate-50">

              <th
                class="sticky left-0 z-10 w-56 min-w-56 border-r border-slate-200 bg-slate-50 px-5 py-4 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Bác sĩ
              </th>

              <th
                v-for="day in [
                  'Thứ 2',
                  'Thứ 3',
                  'Thứ 4',
                  'Thứ 5',
                  'Thứ 6',
                  'Thứ 7',
                  'Chủ nhật',
                ]"
                :key="day"
                class="min-w-[130px] border-r border-slate-200 px-4 py-4 text-center text-xs font-semibold uppercase tracking-wide text-slate-500 last:border-r-0"
              >
                {{ day }}
              </th>

            </tr>

          </thead>


          <!-- Table body -->
          <tbody>

            <!-- Empty state -->
            <tr>

              <td
                colspan="8"
                class="px-5 py-20 text-center"
              >

                <div class="flex flex-col items-center">

                  <div
                    class="mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-slate-100"
                  >
                    <span class="text-2xl">
                      🗓️
                    </span>
                  </div>

                  <p class="text-sm font-medium text-slate-600">
                    Chưa có lịch làm việc
                  </p>

                  <p class="mt-1 max-w-md text-xs text-slate-400">
                    Lịch làm việc của bác sĩ sẽ được hiển thị tại đây
                    sau khi dữ liệu được tải từ API.
                  </p>

                </div>

              </td>

            </tr>

          </tbody>

        </table>

      </div>

    </div>


    <!-- =========================
         LEGEND
    ========================== -->

    <div class="rounded-xl border border-slate-200 bg-white p-5">

      <h2 class="mb-4 text-sm font-semibold text-slate-800">
        Chú thích
      </h2>

      <div class="flex flex-wrap gap-6">

        <!-- Morning -->
        <div class="flex items-center gap-2">
          <span
            class="h-3 w-3 rounded bg-blue-100 ring-1 ring-blue-200"
          />

          <span class="text-sm text-slate-600">
            Ca sáng
          </span>
        </div>

        <!-- Afternoon -->
        <div class="flex items-center gap-2">
          <span
            class="h-3 w-3 rounded bg-purple-100 ring-1 ring-purple-200"
          />

          <span class="text-sm text-slate-600">
            Ca chiều
          </span>
        </div>

        <!-- Full day -->
        <div class="flex items-center gap-2">
          <span
            class="h-3 w-3 rounded bg-green-100 ring-1 ring-green-200"
          />

          <span class="text-sm text-slate-600">
            Cả ngày
          </span>
        </div>

        <!-- Off -->
        <div class="flex items-center gap-2">
          <span
            class="h-3 w-3 rounded bg-slate-100 ring-1 ring-slate-200"
          />

          <span class="text-sm text-slate-600">
            Nghỉ
          </span>
        </div>

      </div>

    </div>


    <!-- =========================
         PAGINATION / SUMMARY
    ========================== -->

    <div class="flex items-center justify-between">

      <span class="text-xs text-slate-400">
        Chưa có dữ liệu lịch làm việc
      </span>

      <div class="flex items-center gap-1">

        <button
          disabled
          class="flex h-8 w-8 items-center justify-center rounded border border-slate-200 text-slate-300"
        >
          ‹
        </button>

        <button
          disabled
          class="flex h-8 w-8 items-center justify-center rounded border border-slate-200 text-slate-300"
        >
          ›
        </button>

      </div>

    </div>

  </div>
</template>
