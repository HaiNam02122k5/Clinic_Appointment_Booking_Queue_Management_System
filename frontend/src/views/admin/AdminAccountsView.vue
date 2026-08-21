<script setup lang="ts">
import {
  onMounted,
  ref,
  watch,
} from 'vue'

import AdminPagination from '@/features/admin/components/AdminPagination.vue'
import AdminRoleBadge from '@/features/admin/components/AdminRoleBadge.vue'

import UserFormDialog from '@/features/users/UserFormDialog.vue'
import { usersApi } from '@/features/users/users.api'

import type {
  AccountRole,
  User,
  CreateAccountForm,
} from '@/features/users/users.types'

// =================================
// STATE
// =================================

const selectedUser = ref<User | null>(null)

const showUserDetail = ref(false)

const loadingUser = ref(false)

const accounts = ref<User[]>([])

const loading = ref(false)

const error = ref('')

const search = ref('')

const roleFilter = ref<
  'all' | AccountRole
>('all')

const statusFilter = ref<
  'all' | 'active' | 'inactive'
>('all')

const currentPage = ref(1)

const pageSize = 5

const totalPages = ref(1)

const totalCount = ref(0)

// =================================
// CREATE DIALOG STATE
// =================================

const dialogOpen = ref(false)

const submitting = ref(false)

// =================================
// VIEW USER
// =================================

async function handleViewUser(
  id: string,
) {
  try {
    loadingUser.value = true

    const user =
      await usersApi.get(id)

    selectedUser.value = user

    showUserDetail.value = true
  } catch (err) {
    console.error(
      'Không thể lấy thông tin tài khoản:',
      err,
    )

    error.value =
      'Không thể lấy thông tin tài khoản.'
  } finally {
    loadingUser.value = false
  }
}
function getRoleParam() {
  if (roleFilter.value === 'all') {
    return undefined
  }

  const roleMap: Record<AccountRole, string> = {
    admin: 'Admin',
    doctor: 'Doctor',
    receptionist: 'Receptionist',
    patient: 'Patient',
  }

  return roleMap[roleFilter.value]
}
// =================================
// LOAD USERS
// =================================

async function loadUsers() {
  loading.value = true

  error.value = ''

  try {
    const response =
      await usersApi.list({
        Search:
          search.value.trim() || undefined,

        Role:
          getRoleParam(),

        IsActive:
          statusFilter.value === 'all'
            ? undefined
            : statusFilter.value === 'active',

        PageNumber:
          currentPage.value,

        PageSize:
          pageSize,
      })

    accounts.value =
      response.items ?? []

    totalPages.value =
      response.totalPages > 0
        ? response.totalPages
        : 1

    totalCount.value =
      response.totalCount ?? 0
  } catch (err: unknown) {
    console.error(
      'Failed to load users:',
      err,
    )

    accounts.value = []

    totalPages.value = 1

    totalCount.value = 0

    error.value =
      'Không thể tải danh sách tài khoản.'
  } finally {
    loading.value = false
  }
}

// =================================
// CREATE ACCOUNT
// =================================

function openCreateDialog() {
  dialogOpen.value = true
}

function closeCreateDialog() {
  if (!submitting.value) {
    dialogOpen.value = false
  }
}

async function handleCreateAccount(
  form: CreateAccountForm,
) {
  submitting.value = true

  error.value = ''

  try {
    // =================================
    // CREATE DOCTOR
    // =================================

    if (
      form.role === 'Doctor'
    ) {
      await usersApi.createDoctor({
        hireDate:
          form.hireDate ?? '',

        licenseNumber:
          form.licenseNumber ?? '',

        qualification:
          form.qualification ?? '',

        experienceYears: Number(
          form.experienceYears ?? 0,
        ),

        status: 'Active',

        specialtyId:
          form.specialtyId ?? '',

        email:
          form.email ?? '',

        address:
          form.address ?? '',

        username:
          form.username ?? '',

        password:
          form.password ?? '',

        fullName:
          form.fullName ?? '',

        phoneNumber:
          form.phoneNumber ?? '',

        dateOfBirth:
          form.dateOfBirth ?? '',

        gender:
          form.gender,

        biography:
          form.biography || undefined,
      })
    }

    // =================================
    // CREATE RECEPTIONIST
    // =================================

    else if (
      form.role === 'Receptionist'
    ) {
      await usersApi.createReceptionist({
        username:
          form.username ?? '',

        password:
          form.password ?? '',

        fullName:
          form.fullName ?? '',

        phoneNumber:
          form.phoneNumber ?? '',

        email:
          form.email ?? '',

        dateOfBirth:
          form.dateOfBirth ?? '',

        gender:
          form.gender,

        address:
          form.address ?? '',

        hireDate:
          form.hireDate ?? '',

        roles: [
          'Receptionist',
        ],
      })
    }

    dialogOpen.value = false

    // Nếu đang ở page khác thì quay về page 1
    if (
      currentPage.value !== 1
    ) {
      currentPage.value = 1
    } else {
      await loadUsers()
    }
  } catch (err: unknown) {
    console.error(
      'Failed to create account:',
      err,
    )

    error.value =
      'Không thể tạo tài khoản.'
  } finally {
    submitting.value = false
  }
}

// =================================
// WATCH SEARCH
// =================================

watch(
  search,
  () => {
    if (
      currentPage.value !== 1
    ) {
      currentPage.value = 1
    } else {
      loadUsers()
    }
  },
)

// =================================
// WATCH ROLE + STATUS
// =================================

watch(
  [
    roleFilter,
    statusFilter,
  ],
  () => {
    if (
      currentPage.value !== 1
    ) {
      currentPage.value = 1
    } else {
      loadUsers()
    }
  },
)

// =================================
// WATCH PAGINATION
// =================================

watch(
  currentPage,
  () => {
    loadUsers()
  },
)

// =================================
// HELPERS
// =================================

function statusText(
  isActive: boolean,
) {
  return isActive
    ? 'Hoạt động'
    : 'Không hoạt động'
}

function statusClass(
  isActive: boolean,
) {
  return isActive
    ? 'bg-green-50 text-green-700'
    : 'bg-slate-100 text-slate-500'
}

function getAccountRole(
  account: User,
): AccountRole {
  const roles = account.roles.map(
    (role) => role.toLowerCase(),
  )

  if (roles.includes('admin')) {
    return 'admin'
  }

  if (roles.includes('doctor')) {
    return 'doctor'
  }

  if (roles.includes('receptionist')) {
    return 'receptionist'
  }

  return 'patient'
}
function formatCreatedAt(
  createdAt?: string,
) {
  if (!createdAt) {
    return '-'
  }

  return new Date(
    createdAt,
  ).toLocaleDateString(
    'vi-VN',
    {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    },
  )
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
  return (
    account.email ||
    '-'
  )
}

// =================================
// RESET FILTER
// =================================

function resetFilters() {
  search.value = ''

  roleFilter.value = 'all'

  statusFilter.value = 'all'

  if (
    currentPage.value !== 1
  ) {
    currentPage.value = 1
  } else {
    loadUsers()
  }
}
function getDisplayRoles(
  roles: string[],
) {
  const nonPatientRoles = roles.filter(
    (role) =>
      role.toLowerCase() !== 'patient',
  )

  return nonPatientRoles.length > 0
    ? nonPatientRoles
    : roles
}

// =================================
// INIT
// =================================

onMounted(() => {
  loadUsers()
})
</script>

<template>
  <div class="space-y-5">

    <!-- TITLE -->

    <div>
      <h1
        class="text-xl font-semibold text-slate-800"
      >
        Quản lý tài khoản
      </h1>

      <p
        class="mt-1 text-sm text-slate-500"
      >
        Quản lý tài khoản người dùng trong hệ thống
      </p>
    </div>

    <!-- TOOLBAR -->

    <div
      class="rounded-xl border border-slate-200 bg-white p-5"
    >
      <div
        class="flex flex-col gap-3 lg:flex-row lg:items-center"
      >

        <!-- SEARCH -->

        <div class="relative flex-1">
          <input
            v-model="search"
            type="text"
            placeholder="Tìm kiếm theo tên hoặc email..."
            class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 pl-10 text-sm text-slate-800 placeholder:text-slate-400 focus:border-violet-500 focus:outline-none"
          />

          <svg
            class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <circle
              cx="11"
              cy="11"
              r="7"
            />

            <path d="m20 20-3.5-3.5" />
          </svg>
        </div>


        <!-- STATUS FILTER -->

        <select
          v-model="statusFilter"
          class="rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-600 focus:border-violet-500 focus:outline-none"
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
          type="button"
          class="rounded-lg border border-slate-200 px-4 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <!-- ADD -->

        <button
          type="button"
          class="rounded-lg bg-violet-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-violet-700"
          @click="openCreateDialog"
        >
          + Thêm tài khoản
        </button>

      </div>
    </div>

    <!-- TABLE CARD -->

    <div
      class="rounded-xl border border-slate-200 bg-white p-5"
    >
      <div class="mb-4">
        <h2
          class="text-sm font-semibold text-slate-800"
        >
          Danh sách tài khoản
        </h2>

        <p
          class="mt-1 text-xs text-slate-400"
        >
          {{ totalCount }} tài khoản
        </p>
      </div>

      <div
        class="overflow-x-auto rounded-lg border border-slate-200"
      >

        <!-- LOADING -->

        <div
          v-if="loading"
          class="py-12 text-center text-sm text-slate-400"
        >
          Đang tải danh sách tài khoản...
        </div>

        <!-- ERROR -->

        <div
          v-else-if="error"
          class="py-12 text-center text-sm text-red-500"
        >
          {{ error }}
        </div>

        <!-- TABLE -->

        <table
          v-else
          class="w-full min-w-[900px] text-sm"
        >
          <thead>
            <tr
              class="border-b border-slate-200 bg-slate-50"
            >
              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                #
              </th>

              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Người dùng
              </th>

              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Email
              </th>

              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Vai trò
              </th>

              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Trạng thái
              </th>

              <th
                class="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Ngày tạo
              </th>

              <th
                class="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Thao tác
              </th>
            </tr>
          </thead>

          <tbody
            class="divide-y divide-slate-100"
          >
            <tr
              v-for="(account, index) in accounts"
              :key="account.id"
              class="hover:bg-slate-50"
            >
              <td
                class="px-4 py-3 text-xs text-slate-400"
              >
                {{
                  (currentPage - 1) *
                    pageSize +
                  index +
                  1
                }}
              </td>

              <!-- USER -->

              <td
                class="px-4 py-3"
              >
                <div
                  class="font-medium text-slate-800"
                >
                  {{
                    getAccountName(account)
                  }}
                </div>

                <div
                  class="mt-0.5 text-xs text-slate-400"
                >
                  @{{ account.username }}
                </div>
              </td>

              <!-- EMAIL -->

              <td
                class="px-4 py-3 text-slate-500"
              >
                {{
                  getAccountEmail(account)
                }}
              </td>

              <!-- ROLE -->

              <td
                class="px-4 py-3"
              >
                <AdminRoleBadge
                  :role="getAccountRole(account)"
                />
              </td>

              <!-- STATUS -->

              <td
                class="px-4 py-3"
              >
                <span
                  class="rounded-md px-2.5 py-1 text-xs font-medium"
                  :class="statusClass(account.isActive)"
                >
                  {{
                    statusText(account.isActive)
                  }}
                </span>
              </td>

              <!-- CREATED AT -->

              <td
                class="px-4 py-3 text-xs text-slate-500"
              >
                {{
                  formatCreatedAt(
                    account.createdAt,
                  )
                }}
              </td>

              <!-- ACTION -->

              <td
                class="px-4 py-3"
              >
                <div
                  class="flex justify-end"
                >
                  <button
                    type="button"
                    class="rounded-md px-2.5 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-100 hover:text-violet-600"
                    @click="handleViewUser(account.id)"
                  >
                    Xem
                  </button>
                </div>
              </td>
            </tr>

            <!-- EMPTY -->

            <tr
              v-if="accounts.length === 0"
            >
              <td
                colspan="7"
                class="px-4 py-12 text-center text-sm text-slate-400"
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

    <!-- CREATE ACCOUNT DIALOG -->

    <UserFormDialog
      :open="dialogOpen"
      :submitting="submitting"
      @close="closeCreateDialog"
      @submit="handleCreateAccount"
    />

    <!-- VIEW USER DETAIL DIALOG -->

    <div
      v-if="showUserDetail && selectedUser"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
      @click.self="showUserDetail = false"
    >
      <div
        class="w-full max-w-lg rounded-2xl bg-white p-6 shadow-xl"
      >

        <!-- HEADER -->

        <div
          class="mb-6 flex items-center justify-between"
        >
          <h2
            class="text-xl font-semibold text-slate-800"
          >
            Thông tin tài khoản
          </h2>

          <button
            type="button"
            class="flex h-8 w-8 items-center justify-center rounded-lg text-xl text-slate-400 hover:bg-slate-100 hover:text-slate-700"
            @click="showUserDetail = false"
          >
            ×
          </button>
        </div>

        <!-- USER INFO -->

        <div
          class="space-y-4"
        >
          <div>
            <p
              class="text-sm text-slate-500"
            >
              Họ và tên
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                selectedUser.fullName
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Tên đăng nhập
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                selectedUser.username
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Email
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                selectedUser.email
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Số điện thoại
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                selectedUser.phoneNumber
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Giới tính
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                selectedUser.gender
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Vai trò
            </p>

              <div
                class="mt-2 flex flex-wrap gap-2"
              >
                <span
                  v-for="role in getDisplayRoles(selectedUser.roles)"
                  :key="role"
                  class="rounded-full bg-violet-50 px-3 py-1 text-sm font-medium text-violet-700"
                >
                  {{ role }}
                </span>
              </div>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Trạng thái
            </p>

            <p
              class="mt-1 font-medium"
              :class="
                selectedUser.isActive
                  ? 'text-emerald-600'
                  : 'text-red-600'
              "
            >
              {{
                selectedUser.isActive
                  ? 'Hoạt động'
                  : 'Đã khóa'
              }}
            </p>
          </div>

          <div>
            <p
              class="text-sm text-slate-500"
            >
              Ngày tạo
            </p>

            <p
              class="mt-1 font-medium text-slate-800"
            >
              {{
                formatCreatedAt(
                  selectedUser.createdAt,
                )
              }}
            </p>
          </div>
        </div>

        <!-- FOOTER -->

        <div
          class="mt-6 flex justify-end"
        >
          <button
            type="button"
            class="rounded-lg bg-violet-600 px-5 py-2.5 text-sm font-medium text-white hover:bg-violet-700"
            @click="showUserDetail = false"
          >
            Đóng
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
