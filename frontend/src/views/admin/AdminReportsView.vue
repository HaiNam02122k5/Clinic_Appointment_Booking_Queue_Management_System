<script setup lang="ts">
import { computed, ref } from 'vue'

import {
  REPORT_SUMMARY,
  SPEC_STATS,
} from '@/features/admin/admin.mock'

const period = ref('month')
const specialty = ref('all')

const filteredStats = computed(() => {
  if (specialty.value === 'all') {
    return SPEC_STATS
  }

  return SPEC_STATS.filter(
    (item) => item.name === specialty.value,
  )
})

const specialties = computed(() => {
  return SPEC_STATS.map((item) => item.name)
})

const maxVisits = computed(() => {
  return Math.max(
    ...filteredStats.value.map((item) => item.visits),
  )
})
</script>

<template>
  <div class="space-y-5">

    <!-- TITLE -->

    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Báo cáo
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Theo dõi và phân tích hoạt động khám bệnh
      </p>
    </div>

    <!-- FILTER -->

    <div
      class="flex flex-col gap-3
             rounded-xl border border-slate-200
             bg-white p-5
             lg:flex-row lg:items-center
             lg:justify-between"
    >

      <div>
        <h2 class="text-sm font-semibold text-slate-800">
          Báo cáo tổng hợp
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Thống kê theo khoảng thời gian
        </p>
      </div>

      <div class="flex gap-2">

        <select
          v-model="period"
          class="rounded-lg border border-slate-200
                 bg-white px-3 py-2
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
        >
          <option value="week">
            Tuần này
          </option>

          <option value="month">
            Tháng này
          </option>

          <option value="quarter">
            Quý này
          </option>
        </select>

        <button
          class="rounded-lg bg-violet-600
                 px-4 py-2 text-sm font-semibold
                 text-white hover:bg-violet-700"
        >
          Xuất báo cáo
        </button>

      </div>
    </div>

    <!-- SUMMARY -->

    <div
      class="grid grid-cols-1 gap-4
             sm:grid-cols-2
             xl:grid-cols-4"
    >
      <div
        v-for="item in REPORT_SUMMARY"
        :key="item.label"
        class="rounded-xl border
               border-slate-200
               bg-white p-5"
      >
        <div class="text-sm text-slate-500">
          {{ item.label }}
        </div>

        <div
          class="mt-2 text-2xl font-bold
                 text-slate-800"
        >
          {{ item.value }}
        </div>

        <div
          class="mt-1 text-xs"
          :class="
            item.change.startsWith('-')
              ? 'text-green-600'
              : 'text-green-600'
          "
        >
          {{ item.change }}
          so với kỳ trước
        </div>
      </div>
    </div>

    <!-- CHART -->

    <div
      class="rounded-xl border border-slate-200
             bg-white p-6"
    >

      <div
        class="mb-6 flex flex-col gap-3
               sm:flex-row sm:items-center
               sm:justify-between"
      >
        <div>
          <h2
            class="text-sm font-semibold
                   text-slate-800"
          >
            Lượt khám theo chuyên khoa
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            Số lượt khám và tỷ lệ hoàn thành
          </p>
        </div>

        <select
          v-model="specialty"
          class="rounded-lg
                 border border-slate-200
                 bg-white px-3 py-2
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
        >
          <option value="all">
            Tất cả chuyên khoa
          </option>

          <option
            v-for="item in specialties"
            :key="item"
            :value="item"
          >
            {{ item }}
          </option>
        </select>
      </div>

      <div class="space-y-5">

        <div
          v-for="item in filteredStats"
          :key="item.name"
        >

          <div
            class="mb-2 flex items-center
                   justify-between"
          >
            <div>
              <span
                class="text-sm font-medium
                       text-slate-700"
              >
                {{ item.name }}
              </span>

              <span
                class="ml-2 text-xs
                       text-slate-400"
              >
                {{ item.visits }} lượt khám
              </span>
            </div>

            <span
              class="text-xs font-semibold
                     text-slate-600"
            >
              {{ item.pct }}%
            </span>
          </div>

          <div
            class="h-3 overflow-hidden
                   rounded-full bg-slate-100"
          >
            <div
              class="h-full rounded-full
                     bg-violet-600
                     transition-all"
              :style="{
                width: `${(item.visits / maxVisits) * 100}%`,
              }"
            />
          </div>

          <div
            class="mt-1 text-xs
                   text-slate-400"
          >
            Thời gian khám trung bình:
            {{ item.avg }} phút
          </div>

        </div>

      </div>
    </div>

    <!-- DETAIL TABLE -->

    <div
      class="rounded-xl border
             border-slate-200
             bg-white p-5"
    >

      <div class="mb-4">
        <h2
          class="text-sm font-semibold
                 text-slate-800"
        >
          Chi tiết theo chuyên khoa
        </h2>
      </div>

      <div
        class="overflow-hidden rounded-lg
               border border-slate-200"
      >
        <table class="w-full text-sm">

          <thead>
            <tr
              class="border-b border-slate-200
                     bg-slate-50"
            >
              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Chuyên khoa
              </th>

              <th
                class="px-4 py-3 text-right
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Lượt khám
              </th>

              <th
                class="px-4 py-3 text-right
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Thời gian TB
              </th>

              <th
                class="px-4 py-3 text-right
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Tỷ lệ hoàn thành
              </th>
            </tr>
          </thead>

          <tbody class="divide-y divide-slate-100">

            <tr
              v-for="item in filteredStats"
              :key="item.name"
              class="hover:bg-slate-50"
            >
              <td
                class="px-4 py-3
                       font-medium
                       text-slate-800"
              >
                {{ item.name }}
              </td>

              <td
                class="px-4 py-3 text-right
                       text-slate-600"
              >
                {{ item.visits }}
              </td>

              <td
                class="px-4 py-3 text-right
                       text-slate-600"
              >
                {{ item.avg }} phút
              </td>

              <td
                class="px-4 py-3 text-right"
              >
                <span
                  class="font-medium
                         text-green-600"
                >
                  {{ item.pct }}%
                </span>
              </td>
            </tr>

          </tbody>

        </table>
      </div>
    </div>

  </div>
</template>
