<script setup lang="ts">
import { ref } from 'vue'

import AdminAppointmentsTable from '@/features/admin/components/AdminAppointmentsTable.vue'
import AdminBarChart from '@/features/admin/components/AdminBarChart.vue'
import AdminDonutChart from '@/features/admin/components/AdminDonutChart.vue'
import AdminStatCard from '@/features/admin/components/AdminStatCard.vue'

import {
  APPOINTMENTS,
  DONUT_DATA,
  MONTH_DATA,
  WEEK_DATA,
} from '@/features/admin/admin.mock'

const tab = ref<'week' | 'month'>('week')

const stats = [
  {
    label: 'Tổng lịch hẹn',
    value: '1,248',
    sub: '+8.4% so với tháng trước',
  },
  {
    label: 'Bệnh nhân',
    value: '892',
    sub: '+5.2% so với tháng trước',
  },
  {
    label: 'Bác sĩ',
    value: '24',
    sub: '22 đang hoạt động',
  },
  {
    label: 'Lượt khám hôm nay',
    value: '156',
    sub: '+12.5% so với hôm qua',
  },
  {
    label: 'Tỷ lệ hoàn thành',
    value: '94.2%',
    sub: '+2.1% so với tháng 7',
  },
  {
    label: 'Thời gian chờ TB',
    value: '18 phút',
    sub: 'Giảm 3 phút',
  },
]
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         STATISTICS
    ========================== -->
    <div class="grid grid-cols-2 gap-4 lg:grid-cols-3 xl:grid-cols-6">
      <AdminStatCard
        v-for="stat in stats"
        :key="stat.label"
        :label="stat.label"
        :value="stat.value"
        :sub="stat.sub"
      />
    </div>

    <!-- =========================
         CHARTS
    ========================== -->
    <div class="grid grid-cols-1 gap-4 lg:grid-cols-5">

      <!-- BAR CHART -->
      <div
        class="rounded-xl border border-slate-200
               bg-white p-6 lg:col-span-3"
      >
        <div class="mb-5 flex items-center justify-between">

          <div>
            <h3 class="text-sm font-semibold text-slate-800">
              Lượt khám theo ngày
            </h3>

            <p class="mt-0.5 text-xs text-slate-400">
              Số bệnh nhân được khám
            </p>
          </div>

          <div class="flex items-center gap-1 rounded-lg bg-slate-100 p-1">

            <button
              class="rounded-md px-3 py-1 text-xs font-medium transition"
              :class="
                tab === 'week'
                  ? 'bg-white text-slate-800 shadow-sm'
                  : 'text-slate-500 hover:text-slate-700'
              "
              @click="tab = 'week'"
            >
              Tuần
            </button>

            <button
              class="rounded-md px-3 py-1 text-xs font-medium transition"
              :class="
                tab === 'month'
                  ? 'bg-white text-slate-800 shadow-sm'
                  : 'text-slate-500 hover:text-slate-700'
              "
              @click="tab = 'month'"
            >
              Tháng
            </button>

          </div>
        </div>

        <AdminBarChart
          :data="tab === 'week' ? WEEK_DATA : MONTH_DATA"
          :max-val="tab === 'week' ? 60 : 600"
        />
      </div>

      <!-- DONUT -->
      <div
        class="rounded-xl border border-slate-200
               bg-white p-6 lg:col-span-2"
      >
        <div class="mb-5">
          <h3 class="text-sm font-semibold text-slate-800">
            Lịch hẹn theo trạng thái
          </h3>

          <p class="mt-0.5 text-xs text-slate-400">
            Hôm nay, 11/08/2026
          </p>
        </div>

        <AdminDonutChart :data="DONUT_DATA" />
      </div>
    </div>

    <!-- =========================
         APPOINTMENTS
    ========================== -->
    <div
      class="rounded-xl border border-slate-200
             bg-white p-6"
    >
      <div class="mb-5 flex items-center justify-between">

        <div>
          <h3 class="text-sm font-semibold text-slate-800">
            Lịch hẹn hôm nay
          </h3>

          <p class="mt-0.5 text-xs text-slate-400">
            Danh sách lịch hẹn ngày 11/08/2026
          </p>
        </div>

        <button
          class="text-xs font-medium text-[#0E4D92]
                 hover:underline"
        >
          Xem tất cả
        </button>

      </div>

      <AdminAppointmentsTable
        :data="APPOINTMENTS.slice(0, 5)"
      />
    </div>

  </div>
</template>
