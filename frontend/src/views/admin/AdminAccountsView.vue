<script setup lang="ts">
import {
  computed,
  onMounted,
  ref,
  watch,
} from 'vue'

import AdminPagination from '@/features/admin/components/AdminPagination.vue'
import AdminRoleBadge from '@/features/admin/components/AdminRoleBadge.vue'

import { usersApi } from '@/features/users/users.api'

import type {
  User,
  AccountRole,
} from '@/features/users/users.types'

// ================================
// STATE
// ================================

const accounts = ref<User[]>([])

const loading = ref(false)
const error = ref('')

const search = ref('')
const roleFilter = ref<'all' | string>('all')
const statusFilter = ref<'all' | 'active' | 'inactive'>('all')

const currentPage = ref(1)

const pageSize = 5

const totalPages = ref(1)
const totalCount = ref(0)

// ================================
// LOAD API
// ================================

async function loadUsers() {
  loading.value = true
  error.value = ''

  try {
    const response = await usersApi.list({
      search: search.value || undefined,
      pageNumber: currentPage.value,
      pageSize,
    })

    accounts.value = response.items

    totalPages.value = response.totalPages
    totalCount.value = response.totalCount

    console.log('Users from API:', response)
  } catch (err: unknown) {
    console.error('Failed to load users:', err)

    error.value = 'Không thể tải danh sách tài khoản.'
  } finally {
    loading.value = false
  }
}

// ================================
// FILTER
// ================================

const filteredAccounts = computed(() => {
  return accounts.value.filter((account) => {
    const accountRole =
      account.roles[0]?.toLowerCase() ?? ''

    const matchesRole =
      roleFilter.value === 'all' ||
      accountRole === roleFilter.value.toLowerCase()

    // API chưa trả về status
    const matchesStatus =
      statusFilter.value === 'all'

    return matchesRole && matchesStatus
  })
})

// ================================
// WATCH
// ================================

watch(
  [search, roleFilter, statusFilter],
  () => {
    currentPage.value = 1
    loadUsers()
  },
)

watch(
  currentPage,
  () => {
    loadUsers()
  },
)

// ================================
// HELPERS
// ================================

function statusText(
  status: 'active' | 'inactive',
) {
  return status === 'active'
    ? 'Hoạt động'
    : 'Không hoạt động'
}

function statusClass(
  status: 'active' | 'inactive',
) {
  return status === 'active'
    ? 'bg-green-50 text-green-700'
    : 'bg-slate-100 text-slate-500'
}

function getAccountRole(
  account: User,
): AccountRole {
  const role =
    account.roles[0]?.toLowerCase()

  if (
    role === 'admin' ||
    role === 'doctor' ||
    role === 'receptionist' ||
    role === 'patient'
  ) {
    return role
  }

  return 'patient'
}

function getAccountStatus(): 'active' | 'inactive' {
  // API hiện chưa trả về trạng thái
  return 'active'
}

function getAccountCreated() {
  // API hiện chưa trả về createdAt
  return '-'
}

function getAccountName(
  account: User,
) {
  return (
    account.fullName ||
    account.username ||
    'Chưa có tên'
  )
}

function getAccountEmail(
  account: User,
) {
  return account.email ?? '-'
}


// ================================
// RESET
// ================================

function resetFilters() {
  search.value = ''
  roleFilter.value = 'all'
  statusFilter.value = 'all'
  currentPage.value = 1
}

// ================================
// INIT
// ================================

onMounted(() => {
  loadUsers()
})
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
      <div
        class="flex flex-col gap-3
               lg:flex-row lg:items-center"
      >

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

        <!-- RESET -->

        <button
          class="rounded-lg border border-slate-200
                 px-4 py-2.5 text-sm font-medium
                 text-slate-600
                 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <!-- ADD -->

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
            {{ totalCount }} tài khoản
          </p>
        </div>
      </div>

      <div
        class="overflow-hidden rounded-lg
               border border-slate-200"
      >

        <!-- LOADING -->

        <div
          v-if="loading"
          class="py-12 text-center
                 text-sm text-slate-400"
        >
          Đang tải danh sách tài khoản...
        </div>

        <!-- ERROR -->

        <div
          v-else-if="error"
          class="py-12 text-center
                 text-sm text-red-500"
        >
          {{ error }}
        </div>

        <!-- TABLE -->

        <table
          v-else
          class="w-full text-sm"
        >

          <thead>
            <tr
              class="border-b border-slate-200
                     bg-slate-50"
            >

              <!-- NUMBER -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                #
              </th>

              <!-- USER -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Người dùng
              </th>

              <!-- EMAIL -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Email
              </th>

              <!-- ROLE -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Vai trò
              </th>

              <!-- STATUS -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Trạng thái
              </th>

              <!-- CREATED -->

              <th
                class="px-4 py-3 text-left
                       text-xs font-semibold
                       uppercase tracking-wide
                       text-slate-500"
              >
                Ngày tạo
              </th>

              <!-- ACTION -->

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

            <!-- ACCOUNT -->

            <tr
              v-for="(account, index) in filteredAccounts"
              :key="account.id"
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

              <!-- USER -->

              <td class="px-4 py-3">
                <div
                  class="font-medium
                         text-slate-800"
                >
                  {{ getAccountName(account) }}
                </div>

                <div
                  class="mt-0.5 text-xs
                         text-slate-400"
                >
                  @{{ account.username }}
                </div>
              </td>

              <!-- EMAIL -->

              <td
                class="px-4 py-3
                       text-slate-500"
              >
                {{ getAccountEmail(account) }}
              </td>

              <!-- ROLE -->

              <td class="px-4 py-3">
                <AdminRoleBadge
                  :role="getAccountRole(account)"
                />
              </td>

              <!-- STATUS -->

              <td class="px-4 py-3">
                <span
                  class="rounded-md px-2.5 py-1
                         text-xs font-medium"
                  :class="statusClass(getAccountStatus())"
                >
                  {{ statusText(getAccountStatus()) }}
                </span>
              </td>

              <!-- CREATED -->

              <td
                class="px-4 py-3
                       text-xs text-slate-500"
              >
                {{ getAccountCreated() }}
              </td>

              <!-- ACTIONS -->

              <td class="px-4 py-3">
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

            <tr
              v-if="filteredAccounts.length === 0"
            >
              <td
                colspan="7"
                class="px-4 py-12
                       text-center text-sm
                       text-slate-400"
              >
                Không tìm thấy tài khoản phù hợp.
              </td>
            </tr>

          </tbody>
        </table>

      </div>

      <!-- =========================
           PAGINATION
      ========================== -->

      <AdminPagination
        v-model:current-page="currentPage"
        :total-pages="totalPages"
      />

    </div>

  </div>
</template>
