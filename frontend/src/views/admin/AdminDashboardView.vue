<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'

import AdminAppointmentsTable from '@/features/admin/components/AdminAppointmentsTable.vue'
import AdminBarChart from '@/features/admin/components/AdminBarChart.vue'
import AdminDonutChart from '@/features/admin/components/AdminDonutChart.vue'
import AdminStatCard from '@/features/admin/components/AdminStatCard.vue'

import { adminApi } from '@/features/admin/admin.api'

import type {
  DashboardData,
  StatisticsData,
  AdminAppointment,
} from '@/features/admin/admin.types'

const tab = ref<'week' | 'month'>('week')

const dashboard = ref<DashboardData | null>(null)
const statistics = ref<StatisticsData | null>(null)
const appointments = ref<AdminAppointment[]>([])

/* =========================
   DATE
========================= */

// Dùng để gửi API: 2026-08-20
const getToday = () => {
  const today = new Date()

  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

// Dùng để hiển thị: 20/08/2026
const todayText = computed(() =>
  new Date().toLocaleDateString('vi-VN'),
)

/* =========================
   DASHBOARD API
========================= */

const fetchDashboard = async () => {
  try {
    const response = await adminApi.getDashboard()

    if (response.isSuccess) {
      dashboard.value = response.result
    }
  } catch (error) {
    console.error('Failed to fetch dashboard:', error)
  }
}

/* =========================
   STATISTICS API
========================= */

const fetchStatistics = async () => {
  try {
    const period =
      tab.value === 'week'
        ? 'Week'
        : 'Month'

    const response =
      await adminApi.getStatistics(period)

    if (response.isSuccess) {
      statistics.value = response.result
    }
  } catch (error) {
    console.error(
      'Failed to fetch statistics:',
      error,
    )
  }
}

/* =========================
   APPOINTMENTS API
========================= */

const fetchAppointments = async () => {
  try {
    const response =
      await adminApi.getAppointments(
        getToday(),
        1,
        5,
      )

    if (response.isSuccess) {
      appointments.value =
        response.result.items
    }
  } catch (error) {
    console.error(
      'Failed to fetch appointments:',
      error,
    )
  }
}

/* =========================
   INITIAL LOAD
========================= */

onMounted(() => {
  fetchDashboard()
  fetchStatistics()
  fetchAppointments()
})

/* =========================
   WATCH TAB
========================= */

watch(tab, () => {
  fetchStatistics()
})

/* =========================
   STAT CARDS
========================= */

const stats = computed(() => [
  {
    label: 'Tổng lịch hẹn',

    value: (
      dashboard.value
        ?.totalAppointments.value ?? 0
    ).toLocaleString(),

    sub: `${
      dashboard.value
        ?.totalAppointments.change ?? 0
    }% so với tháng trước`,
  },

  {
    label: 'Bệnh nhân',

    value: (
      dashboard.value
        ?.totalPatients.value ?? 0
    ).toLocaleString(),

    sub: `${
      dashboard.value
        ?.totalPatients.change ?? 0
    }% so với tháng trước`,
  },

  {
    label: 'Bác sĩ',

    value: (
      dashboard.value
        ?.totalDoctors.value ?? 0
    ).toLocaleString(),

    sub: `${
      dashboard.value
        ?.totalDoctors.change ?? 0
    }% so với tháng trước`,
  },

  {
    label: 'Bác sĩ đang hoạt động',

    value: (
      dashboard.value
        ?.activeDoctor.value ?? 0
    ).toLocaleString(),

    sub: `${
      dashboard.value
        ?.activeDoctor.change ?? 0
    }% thay đổi`,
  },

  {
    label: 'Tỷ lệ hoàn thành',

    value: `${
      dashboard.value
        ?.completionRate.value ?? 0
    }%`,

    sub: `${
      dashboard.value
        ?.completionRate.change ?? 0
    }% thay đổi`,
  },

  {
    label: 'Thời gian chờ TB',

    value: `${
      dashboard.value
        ?.averageWaitingMinute.value ?? 0
    } phút`,

    sub: `${
      dashboard.value
        ?.averageWaitingMinute.change ?? 0
    } phút thay đổi`,
  },
])

/* =========================
   BAR CHART DATA
========================= */

const chartData = computed(() =>
  statistics.value?.appointmentsByDay.map(
    (item) => ({
      day: new Date(
        item.date,
      ).toLocaleDateString('vi-VN', {
        weekday: 'short',
      }),

      value: item.count,
    }),
  ) ?? [],
)

/* =========================
   BAR CHART MAX VALUE
========================= */

const chartMaxValue = computed(() => {
  const max = Math.max(
    ...chartData.value.map(
      (item) => item.value,
    ),
    0,
  )

  return max === 0
    ? 10
    : Math.ceil(max / 10) * 10
})

/* =========================
   DONUT CHART COLORS
========================= */

const statusColors: Record<string, string> = {
  completed: '#00A878',
  confirmed: '#0E4D92',
  pending: '#F59E0B',
  cancelled: '#EF4444',
  waiting: '#0E4D92',
  absent: '#F59E0B',
}

/* =========================
   DONUT CHART DATA
========================= */

const donutData = computed(() =>
  statistics.value?.appointmentsByStatus.map(
    (item) => ({
      label: item.status,

      value: item.count,

      color:
        statusColors[
          item.status.toLowerCase()
        ] ?? '#94A3B8',
    }),
  ) ?? [],
)
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         STATISTICS
    ========================== -->

    <div
      class="grid grid-cols-2 gap-4 lg:grid-cols-3 xl:grid-cols-6"
    >
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

    <div
      class="grid grid-cols-1 gap-4 lg:grid-cols-5"
    >

      <!-- =========================
           BAR CHART
      ========================== -->

      <div
        class="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-3"
      >
        <div
          class="mb-5 flex items-center justify-between"
        >
          <div>
            <h3
              class="text-sm font-semibold text-slate-800"
            >
              Lượt khám theo ngày
            </h3>

            <p
              class="mt-0.5 text-xs text-slate-400"
            >
              Số bệnh nhân được khám
            </p>
          </div>

          <!-- TAB -->

          <div
            class="flex items-center gap-1 rounded-lg bg-slate-100 p-1"
          >
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
          :data="chartData"
          :max-val="chartMaxValue"
        />
      </div>

      <!-- =========================
           DONUT CHART
      ========================== -->

      <div
        class="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-2"
      >
        <div class="mb-5">
          <h3
            class="text-sm font-semibold text-slate-800"
          >
            Lịch hẹn theo trạng thái
          </h3>

          <p
            class="mt-0.5 text-xs text-slate-400"
          >
            Hôm nay, {{ todayText }}
          </p>
        </div>

        <AdminDonutChart
          :data="donutData"
        />
      </div>
    </div>

    <!-- =========================
         APPOINTMENTS
    ========================== -->

    <div
      class="rounded-xl border border-slate-200 bg-white p-6"
    >
      <div
        class="mb-5 flex items-center justify-between"
      >
        <div>
          <h3
            class="text-sm font-semibold text-slate-800"
          >
            Lịch hẹn hôm nay
          </h3>

          <p
            class="mt-0.5 text-xs text-slate-400"
          >
            Danh sách lịch hẹn ngày {{ todayText }}
          </p>
        </div>

        <button
          class="text-xs font-medium text-[#0E4D92] hover:underline"
        >
          Xem tất cả
        </button>
      </div>

      <AdminAppointmentsTable
        :data="appointments"
      />
    </div>

  </div>
</template>
