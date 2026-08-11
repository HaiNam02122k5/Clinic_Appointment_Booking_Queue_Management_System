<script setup lang="ts">
import { ref } from 'vue'

const activeChart = ref<'week' | 'month'>('week')
</script>

<template>
  <div class="space-y-5">

    <!-- Page title -->
    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Tổng quan hệ thống
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Tổng quan hoạt động phòng khám
      </p>
    </div>

    <!-- =========================
         STAT CARDS
    ========================== -->

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">

      <!-- Tổng bệnh nhân -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50">
          <span class="text-lg">👥</span>
        </div>

        <p class="text-sm text-slate-500">
          Tổng số bệnh nhân
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>
      </div>

      <!-- Tổng bác sĩ -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-green-50">
          <span class="text-lg">🩺</span>
        </div>

        <p class="text-sm text-slate-500">
          Tổng số bác sĩ
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>
      </div>

      <!-- Lịch hẹn -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-purple-50">
          <span class="text-lg">📅</span>
        </div>

        <p class="text-sm text-slate-500">
          Lịch hẹn hôm nay
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>
      </div>

      <!-- Đang chờ -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-orange-50">
          <span class="text-lg">⏳</span>
        </div>

        <p class="text-sm text-slate-500">
          Đang chờ khám
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>
      </div>

      <!-- Thời gian chờ -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-yellow-50">
          <span class="text-lg">🕐</span>
        </div>

        <p class="text-sm text-slate-500">
          Thời gian chờ TB
        </p>

        <p class="mt-1 text-2xl font-bold text-slate-800">
          -
        </p>

        <p class="mt-2 text-xs text-slate-400">
          Chưa có dữ liệu
        </p>
      </div>

      <!-- Tỷ lệ huỷ -->
      <div class="rounded-xl border border-slate-200 bg-white p-5">
        <div class="mb-4 flex h-10 w-10 items-center justify-center rounded-lg bg-red-50">
          <span class="text-lg">↩</span>
        </div>

        <p class="text-sm text-slate-500">
          Tỷ lệ huỷ lịch
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
         CHARTS
    ========================== -->

    <div class="grid grid-cols-1 gap-5 lg:grid-cols-5">

      <!-- Appointment chart -->
      <section class="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-3">

        <div class="mb-6 flex items-start justify-between">

          <div>
            <h2 class="text-sm font-semibold text-slate-800">
              Lượt khám theo ngày
            </h2>

            <p class="mt-1 text-xs text-slate-400">
              Số bệnh nhân được khám
            </p>
          </div>

          <div class="flex rounded-lg bg-slate-100 p-1">

            <button
              class="rounded-md px-3 py-1.5 text-xs font-medium"
              :class="
                activeChart === 'week'
                  ? 'bg-white text-slate-800 shadow-sm'
                  : 'text-slate-500'
              "
              @click="activeChart = 'week'"
            >
              Tuần
            </button>

            <button
              class="rounded-md px-3 py-1.5 text-xs font-medium"
              :class="
                activeChart === 'month'
                  ? 'bg-white text-slate-800 shadow-sm'
                  : 'text-slate-500'
              "
              @click="activeChart = 'month'"
            >
              Tháng
            </button>

          </div>

        </div>

        <!-- Chart placeholder -->
        <div class="flex h-64 items-center justify-center rounded-lg bg-slate-50">

          <div class="text-center">

            <div class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-white shadow-sm">
              <span class="text-xl">📊</span>
            </div>

            <p class="text-sm font-medium text-slate-600">
              Chưa có dữ liệu
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Dữ liệu lượt khám sẽ được hiển thị khi kết nối API
            </p>

          </div>

        </div>

      </section>


      <!-- Appointment status -->
      <section class="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-2">

        <div class="mb-6">

          <h2 class="text-sm font-semibold text-slate-800">
            Lịch hẹn theo trạng thái
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            Hôm nay
          </p>

        </div>

        <div class="flex h-64 items-center justify-center rounded-lg bg-slate-50">

          <div class="text-center">

            <div class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-white shadow-sm">
              <span class="text-xl">◔</span>
            </div>

            <p class="text-sm font-medium text-slate-600">
              Chưa có dữ liệu
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Thống kê trạng thái lịch hẹn sẽ được hiển thị tại đây
            </p>

          </div>

        </div>

      </section>

    </div>


    <!-- =========================
         TODAY APPOINTMENTS
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white p-6">

      <div class="mb-5 flex items-center justify-between">

        <div>
          <h2 class="text-sm font-semibold text-slate-800">
            Lịch hẹn hôm nay
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            Danh sách lịch hẹn trong ngày
          </p>
        </div>

        <button
          class="text-sm font-medium text-brand-600 hover:underline"
        >
          Xem tất cả
        </button>

      </div>


      <!-- Search + filter -->

      <div class="mb-5 flex flex-wrap gap-3">

        <div class="relative">

          <input
            type="text"
            placeholder="Tìm bệnh nhân, bác sĩ..."
            class="h-10 w-64 rounded-lg border border-slate-200 px-3 text-sm outline-none focus:border-brand-500"
          />

        </div>

        <select
          class="h-10 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-600 outline-none focus:border-brand-500"
        >
          <option>
            Tất cả trạng thái
          </option>

          <option>
            Hoàn thành
          </option>

          <option>
            Đang chờ
          </option>

          <option>
            Đã hủy
          </option>

          <option>
            Không đến
          </option>
        </select>

      </div>


      <!-- Table -->

      <div class="overflow-x-auto rounded-lg border border-slate-200">

        <table class="w-full min-w-[900px]">

          <thead>
            <tr class="border-b border-slate-200 bg-slate-50">

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                #
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Bệnh nhân
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Bác sĩ
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Chuyên khoa
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Giờ hẹn
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Trạng thái
              </th>

              <th class="px-4 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Thao tác
              </th>

            </tr>
          </thead>

          <tbody>

            <tr>
              <td
                colspan="7"
                class="py-12 text-center"
              >

                <div class="flex flex-col items-center">

                  <div class="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
                    <span class="text-xl">📋</span>
                  </div>

                  <p class="text-sm font-medium text-slate-600">
                    Chưa có lịch hẹn
                  </p>

                  <p class="mt-1 text-xs text-slate-400">
                    Dữ liệu sẽ được tải từ hệ thống
                  </p>

                </div>

              </td>
            </tr>

          </tbody>

        </table>

      </div>


      <!-- Pagination -->

      <div class="mt-4 flex items-center justify-between">

        <span class="text-xs text-slate-400">
          Hiển thị 0 kết quả
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

    </section>

  </div>
</template>
