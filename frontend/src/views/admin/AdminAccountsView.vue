<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import AdminPagination from '@/features/admin/components/AdminPagination.vue'
import AdminRoleBadge from '@/features/admin/components/AdminRoleBadge.vue'

import {
  ACCOUNTS,
  type AccountRole,
} from '@/features/admin/admin.mock'

// ================================
// STATE
// ================================

const search = ref('')
const roleFilter = ref<'all' | AccountRole>('all')
const statusFilter = ref<'all' | 'active' | 'inactive'>('all')
const currentPage = ref(1)

const pageSize = 5

// ================================
// FILTER
// ================================

const filteredAccounts = computed(() => {
  const keyword = search.value.trim().toLowerCase()

  return ACCOUNTS.filter((account) => {
    const matchesSearch =
      !keyword ||
      account.name.toLowerCase().includes(keyword) ||
      account.email.toLowerCase().includes(keyword)

    const matchesRole =
      roleFilter.value === 'all' ||
      account.role === roleFilter.value

    const matchesStatus =
      statusFilter.value === 'all' ||
      account.status === statusFilter.value

    return matchesSearch && matchesRole && matchesStatus
  })
})

// ================================
// PAGINATION
// ================================

const totalPages = computed(() => {
  return Math.max(
    1,
    Math.ceil(filteredAccounts.value.length / pageSize),
  )
})

const pagedAccounts = computed(() => {
  const start = (currentPage.value - 1) * pageSize

  return filteredAccounts.value.slice(
    start,
    start + pageSize,
  )
})

watch(
  [search, roleFilter, statusFilter],
  () => {
    currentPage.value = 1
  },
)

// ================================
// HELPERS
// ================================

function statusText(status: 'active' | 'inactive') {
  return status === 'active'
    ? 'Hoạt động'
    : 'Không hoạt động'
}

function statusClass(status: 'active' | 'inactive') {
  return status === 'active'
    ? 'bg-green-50 text-green-700'
    : 'bg-slate-100 text-slate-500'
}

function resetFilters() {
  search.value = ''
  roleFilter.value = 'all'
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
        Quản lý tài khoản
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Quản lý tài khoản người dùng trong hệ thống
      </p>
    </div>

    <!-- =========================
         TOOLBAR
    ========================== -->

    <div
      class="rounded-xl border border-slate-200
             bg-white p-5"
    >
      <div class="flex flex-col gap-3 lg:flex-row lg:items-center">

        <!-- SEARCH -->

        <div class="relative flex-1">
          <input
            v-model="search"
            type="text"
            placeholder="Tìm kiếm theo tên hoặc email..."
            class="w-full rounded-lg border border-slate-200
                   bg-white px-3 py-2.5 pl-10
                   text-sm text-slate-800
                   placeholder:text-slate-400
                   focus:border-violet-500
                   focus:outline-none"
          />

          <svg
            class="absolute left-3 top-1/2 h-4 w-4
                   -translate-y-1/2 text-slate-400"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <circle cx="11" cy="11" r="7" />
            <path d="m20 20-3.5-3.5" />
          </svg>
        </div>

        <!-- ROLE -->

        <select
          v-model="roleFilter"
          class="rounded-lg border border-slate-200
                 bg-white px-3 py-2.5
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
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

        <!-- STATUS -->

        <select
          v-model="statusFilter"
          class="rounded-lg border border-slate-200
                 bg-white px-3 py-2.5
                 text-sm text-slate-600
                 focus:border-violet-500
                 focus:outline-none"
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

        <button
          class="rounded-lg border border-slate-200
                 px-4 py-2.5 text-sm font-medium
                 text-slate-600
                 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <button
          class="rounded-lg bg-violet-600
                 px-4 py-2.5 text-sm font-semibold
                 text-white
                 hover:bg-violet-700"
        >
          + Thêm tài khoản
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

      <div class="mb-4 flex items-center justify-between">
        <div>
          <h2 class="text-sm font-semibold text-slate-800">
            Danh sách tài khoản
          </h2>

          <p class="mt-1 text-xs text-slate-400">
            {{ filteredAccounts.length }} tài khoản
          </p>
        </div>
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
                #
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Người dùng
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Email
              </th>

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Vai trò
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
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Ngày tạo
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

          <tbody class="divide-y divide-slate-100">

            <tr
              v-for="(account, index) in pagedAccounts"
              :key="account.id"
              class="hover:bg-slate-50"
            >

              <!-- NUMBER -->

              <td class="px-4 py-3 text-xs text-slate-400">
                {{ (currentPage - 1) * pageSize + index + 1 }}
              </td>

              <!-- USER -->

              <td class="px-4 py-3">
                <div class="font-medium text-slate-800">
                  {{ account.name }}
                </div>
              </td>

              <!-- EMAIL -->

              <td class="px-4 py-3 text-slate-500">
                {{ account.email }}
              </td>

              <!-- ROLE -->

              <td class="px-4 py-3">
                <AdminRoleBadge :role="account.role" />
              </td>

              <!-- STATUS -->

              <td class="px-4 py-3">
                <span
                  class="rounded-md px-2.5 py-1
                         text-xs font-medium"
                  :class="statusClass(account.status)"
                >
                  {{ statusText(account.status) }}
                </span>
              </td>

              <!-- CREATED -->

              <td
                class="px-4 py-3 text-xs
                       text-slate-500"
              >
                {{ account.created }}
              </td>

              <!-- ACTIONS -->

              <td class="px-4 py-3">
                <div class="flex justify-end gap-2">

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

            <tr v-if="pagedAccounts.length === 0">
              <td
                colspan="7"
                class="px-4 py-12 text-center
                       text-sm text-slate-400"
              >
                Không tìm thấy tài khoản phù hợp.
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
