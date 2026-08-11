<script setup lang="ts">
import { ref } from 'vue'

const search = ref('')
const roleFilter = ref('all')
const statusFilter = ref('all')
</script>

<template>
  <div class="space-y-5">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">
          Quản lý tài khoản
        </h1>

        <p class="mt-1 text-sm text-slate-500">
          Quản lý tài khoản người dùng trong hệ thống
        </p>
      </div>

      <button
        class="rounded-lg bg-brand-600 px-4 py-2.5 text-sm font-medium text-white hover:bg-brand-700"
      >
        + Thêm tài khoản
      </button>
    </div>


    <!-- Filters -->
    <div class="rounded-xl border border-slate-200 bg-white p-4">

      <div class="flex flex-wrap gap-3">

        <!-- Search -->
        <div class="flex-1">
          <input
            v-model="search"
            type="text"
            placeholder="Tìm theo tên, email..."
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm outline-none focus:border-brand-500"
          />
        </div>

        <!-- Role -->
        <select
          v-model="roleFilter"
          class="h-10 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-600 outline-none focus:border-brand-500"
        >
          <option value="all">
            Tất cả vai trò
          </option>

          <option value="patient">
            Bệnh nhân
          </option>

          <option value="doctor">
            Bác sĩ
          </option>

          <option value="receptionist">
            Lễ tân
          </option>

          <option value="admin">
            Quản trị viên
          </option>
        </select>

        <!-- Status -->
        <select
          v-model="statusFilter"
          class="h-10 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-600 outline-none focus:border-brand-500"
        >
          <option value="all">
            Tất cả trạng thái
          </option>

          <option value="active">
            Hoạt động
          </option>

          <option value="inactive">
            Không hoạt động
          </option>
        </select>

      </div>

    </div>


    <!-- Table -->
    <div class="rounded-xl border border-slate-200 bg-white">

      <div class="overflow-x-auto">

        <table class="w-full min-w-[900px]">

          <thead>
            <tr class="border-b border-slate-200 bg-slate-50">

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Người dùng
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Email
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Vai trò
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Ngày tạo
              </th>

              <th
                class="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Trạng thái
              </th>

              <th
                class="px-5 py-3 text-right text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Thao tác
              </th>

            </tr>
          </thead>

          <tbody>

            <!-- Empty state -->
            <tr>
              <td
                colspan="6"
                class="px-5 py-16 text-center"
              >

                <div class="flex flex-col items-center">

                  <div
                    class="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100"
                  >
                    <span class="text-xl">
                      👥
                    </span>
                  </div>

                  <p class="text-sm font-medium text-slate-600">
                    Chưa có dữ liệu tài khoản
                  </p>

                  <p class="mt-1 text-xs text-slate-400">
                    Danh sách tài khoản sẽ được tải từ API
                  </p>

                </div>

              </td>
            </tr>

          </tbody>

        </table>

      </div>


      <!-- Pagination -->
      <div
        class="flex items-center justify-between border-t border-slate-200 px-5 py-4"
      >

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

    </div>

  </div>
</template>
