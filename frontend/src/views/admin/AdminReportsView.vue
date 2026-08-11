<script setup lang="ts">
import { ref } from 'vue'

const period = ref('month')
const specialty = ref('all')
const fromDate = ref('')
const toDate = ref('')
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         PAGE HEADER
    ========================== -->

    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Báo cáo thống kê
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Theo dõi và phân tích hoạt động của phòng khám
      </p>
    </div>


    <!-- =========================
         FILTER
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white p-5">

      <div class="flex flex-wrap items-end gap-4">

        <!-- Period -->
        <div>
          <label
            class="mb-1.5 block text-xs font-medium text-slate-600"
          >
            Thời gian
          </label>

          <select
            v-model="period"
            class="h-10 min-w-[150px] rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
          >
            <option value="week">
              Tuần
            </option>

            <option value="month">
              Tháng
            </option>

            <option value="quarter">
              Quý
            </option>

            <option value="year">
              Năm
            </option>

            <option value="custom">
              Tùy chọn
            </option>
          </select>
        </div>


        <!-- From date -->
        <div v-if="period === 'custom'">
          <label
            class="mb-1.5 block text-xs font-medium text-slate-600"
          >
            Từ ngày
          </label>

          <input
            v-model="fromDate"
            type="date"
            class="h-10 rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
          />
        </div>


        <!-- To date -->
        <div v-if="period === 'custom'">
          <label
            class="mb-1.5 block text-xs font-medium text-slate-600"
          >
            Đến ngày
          </label>

          <input
            v-model="toDate"
            type="date"
            class="h-10 rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
          />
        </div>


        <!-- Specialty -->
        <div>
          <label
            class="mb-1.5 block text-xs font-medium text-slate-600"
          >
            Chuyên khoa
          </label>

          <select
            v-model="specialty"
            class="h-10 min-w-[180px] rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
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
        </div>


        <!-- Apply -->
        <button
          class="h-10 rounded-lg bg-brand-600 px-5 text-sm font-medium text-white transition hover:bg-brand-700"
        >
          Áp dụng
        </button>

      </div>

    </section>


    <!-- =========================
         SUMMARY CARDS
    ========================== -->

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">

      <!-- Total appointments -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">

        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50">
          <span class="text-lg">
            📅
          </span>
        </div>

        <p class="text-sm text-slate-500">
          Tổng lượt khám
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>

      </div>


      <!-- Completion -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">

        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-green-50">
          <span class="text-lg">
            ✓
          </span>
        </div>

        <p class="text-sm text-slate-500">
          Tỷ lệ hoàn thành
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>

      </div>


      <!-- Waiting -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">

        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-orange-50">
          <span class="text-lg">
            🕐
          </span>
        </div>

        <p class="text-sm text-slate-500">
          Thời gian chờ trung bình
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>

      </div>


      <!-- Cancel -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">

        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-red-50">
          <span class="text-lg">
            ↩
          </span>
        </div>

        <p class="text-sm text-slate-500">
          Tỷ lệ hủy lịch
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>

      </div>

    </div>


    <!-- =========================
         CHART 1
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white p-6">

      <div class="mb-5">
        <h2 class="text-sm font-semibold text-slate-800">
          Lượt khám theo thời gian
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Thống kê số lượt khám trong khoảng thời gian đã chọn
        </p>
      </div>

      <div class="flex h-72 items-center justify-center rounded-lg bg-slate-50">

        <div class="text-center">

          <div
            class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-white shadow-sm"
          >
            <span class="text-xl">
              📊
            </span>
          </div>

          <p class="text-sm font-medium text-slate-600">
            Chưa có dữ liệu
          </p>

          <p class="mt-1 text-xs text-slate-400">
            Biểu đồ sẽ được hiển thị khi kết nối API
          </p>

        </div>

      </div>

    </section>


    <!-- =========================
         CHART 2 + 3
    ========================== -->

    <div class="grid grid-cols-1 gap-5 lg:grid-cols-2">

      <!-- Specialty chart -->
      <section class="rounded-xl border border-slate-200 bg-white p-6">

        <div class="mb-5">

          <h2 class="text-sm font-semibold text-slate-800">
            Lượt khám theo chuyên khoa
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            So sánh số lượt khám giữa các chuyên khoa
          </p>

        </div>

        <div class="flex h-64 items-center justify-center rounded-lg bg-slate-50">

          <div class="text-center">

            <div
              class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-white shadow-sm"
            >
              <span class="text-xl">
                📈
              </span>
            </div>

            <p class="text-sm font-medium text-slate-600">
              Chưa có dữ liệu
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Dữ liệu sẽ được tải từ API
            </p>

          </div>

        </div>

      </section>


      <!-- Status chart -->
      <section class="rounded-xl border border-slate-200 bg-white p-6">

        <div class="mb-5">

          <h2 class="text-sm font-semibold text-slate-800">
            Thống kê trạng thái lịch hẹn
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            Tỷ lệ hoàn thành, hủy và không đến
          </p>

        </div>

        <div class="flex h-64 items-center justify-center rounded-lg bg-slate-50">

          <div class="text-center">

            <div
              class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-white shadow-sm"
            >
              <span class="text-xl">
                ◔
              </span>
            </div>

            <p class="text-sm font-medium text-slate-600">
              Chưa có dữ liệu
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Dữ liệu sẽ được tải từ API
            </p>

          </div>

        </div>

      </section>

    </div>


    <!-- =========================
         REPORT TABLE
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white p-6">

      <div class="mb-5 flex items-center justify-between">

        <div>
          <h2 class="text-sm font-semibold text-slate-800">
            Báo cáo chi tiết
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            Chi tiết thống kê theo chuyên khoa
          </p>
        </div>

        <button
          class="rounded-lg border border-slate-200 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
        >
          Xuất báo cáo
        </button>

      </div>


      <div class="overflow-x-auto rounded-lg border border-slate-200">

        <table class="w-full min-w-[800px]">

          <thead>

            <tr class="border-b border-slate-200 bg-slate-50">

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Chuyên khoa
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Tổng lượt khám
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Hoàn thành
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Đã hủy
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Không đến
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Thời gian chờ TB
              </th>

            </tr>

          </thead>


          <tbody>

            <tr>

              <td
                colspan="6"
                class="px-5 py-14 text-center"
              >

                <div class="flex flex-col items-center">

                  <div
                    class="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100"
                  >
                    <span class="text-xl">
                      📋
                    </span>
                  </div>

                  <p class="text-sm font-medium text-slate-600">
                    Chưa có dữ liệu báo cáo
                  </p>

                  <p class="mt-1 text-xs text-slate-400">
                    Dữ liệu thống kê sẽ được tải từ API
                  </p>

                </div>

              </td>

            </tr>

          </tbody>

        </table>

      </div>

    </section>

  </div>
</template>
