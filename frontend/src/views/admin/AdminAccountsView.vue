<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { usersApi } from '@/features/users/users.api'
import type { CreateUserInput, UpdateUserInput, User } from '@/features/users/users.types'
import AdminFilterToolbar from '@/features/admin/components/AdminFilterToolbar.vue'
import AdminPagination from '@/features/admin/components/AdminPagination.vue'
import AdminRoleBadge from '@/features/admin/components/AdminRoleBadge.vue'
import BaseStatusBadge from '@/components/ui/BaseStatusBadge.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseModal from '@/components/ui/BaseModal.vue'
import UserFormDialog from '@/features/users/UserFormDialog.vue'

const accounts = ref<User[]>([])
const loading = ref(false)
const error = ref('')
const successMessage = ref('')

const search = ref('')
const roleFilter = ref('all')
const currentPage = ref(1)
const pageSize = 8
const totalPages = ref(1)
const totalCount = ref(0)

// Dialogs
const formDialogOpen = ref(false)
const editingUser = ref<User | null>(null)
const submitting = ref(false)

const viewModalOpen = ref(false)
const viewingUser = ref<User | null>(null)

const deleteConfirmOpen = ref(false)
const deletingUser = ref<User | null>(null)
const deleting = ref(false)

async function loadUsers() {
  loading.value = true
  error.value = ''

  try {
    const res = await usersApi.list({
      search: search.value.trim() || undefined,
      pageNumber: currentPage.value,
      pageSize,
    })

    const rawItems = res?.items || (Array.isArray(res as any) ? (res as any) : [])
    let items: User[] = Array.isArray(rawItems) ? rawItems : []

    if (roleFilter.value !== 'all') {
      items = items.filter((u) =>
        u.roles?.some((r) => r.toLowerCase() === roleFilter.value.toLowerCase()),
      )
    }

    accounts.value = items
    totalPages.value = res?.totalPages || 1
    totalCount.value = res?.totalCount ?? items.length
  } catch (err: any) {
    accounts.value = []
    error.value = err?.response?.data?.message || err?.message || 'Không thể tải danh sách tài khoản từ máy chủ.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadUsers()
})

watch([search, roleFilter], () => {
  currentPage.value = 1
  loadUsers()
})

watch(currentPage, () => {
  loadUsers()
})

function resetFilters() {
  search.value = ''
  roleFilter.value = 'all'
  currentPage.value = 1
  loadUsers()
}

function openCreate() {
  editingUser.value = null
  formDialogOpen.value = true
}

function openEdit(user: User) {
  editingUser.value = user
  formDialogOpen.value = true
}

function openView(user: User) {
  viewingUser.value = user
  viewModalOpen.value = true
}

function confirmDelete(user: User) {
  deletingUser.value = user
  deleteConfirmOpen.value = true
}

async function handleFormSubmit(payload: CreateUserInput | UpdateUserInput) {
  submitting.value = true
  error.value = ''

  try {
    if (editingUser.value) {
      await usersApi.update(editingUser.value.id, payload as UpdateUserInput)
      successMessage.value = `Đã cập nhật thông tin tài khoản ${payload.fullName || payload.name} thành công!`
    } else {
      await usersApi.create(payload as CreateUserInput)
      successMessage.value = `Đã tạo tài khoản ${payload.username} thành công!`
    }

    formDialogOpen.value = false
    await loadUsers()

    setTimeout(() => {
      successMessage.value = ''
    }, 4000)
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Thao tác không thành công.'
  } finally {
    submitting.value = false
  }
}

async function handleDelete() {
  if (!deletingUser.value) return

  deleting.value = true
  try {
    await usersApi.remove(deletingUser.value.id)
    successMessage.value = `Đã vô hiệu hóa tài khoản @${deletingUser.value.username}!`
    deleteConfirmOpen.value = false
    await loadUsers()

    setTimeout(() => {
      successMessage.value = ''
    }, 4000)
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Không thể khóa tài khoản.'
  } finally {
    deleting.value = false
  }
}
</script>

<template>
  <div class="space-y-5 font-sans">
    <!-- Header Title -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-slate-800 tracking-tight">Quản lý Tài khoản</h1>
        <p class="mt-1 text-sm text-slate-500">
          Quản lý phân quyền, tài khoản nhân viên và người dùng trong toàn hệ thống
        </p>
      </div>

      <BaseButton
        type="button"
        variant="primary"
        size="md"
        @click="openCreate"
      >
        + Thêm Tài khoản
      </BaseButton>
    </div>

    <!-- Alert thông báo -->
    <div v-if="successMessage">
      <BaseAlert type="success" :message="successMessage" dismissible @dismiss="successMessage = ''" />
    </div>
    <div v-if="error">
      <BaseAlert type="error" :message="error" dismissible @dismiss="error = ''" />
    </div>

    <!-- Filter Toolbar -->
    <AdminFilterToolbar
      v-model:search="search"
      search-placeholder="Tìm theo họ tên, username, email, SĐT..."
      add-button-label="+ Thêm Tài khoản"
      :show-add-button="false"
      @reset="resetFilters"
      @add="openCreate"
    >
      <template #filters>
        <!-- Role Filter -->
        <select
          v-model="roleFilter"
          class="rounded-xl border border-slate-200 bg-slate-50/50 px-3 py-2 text-xs font-medium text-slate-700 focus:border-[#0E4D92] focus:bg-white focus:outline-none transition-colors"
        >
          <option value="all">Tất cả vai trò</option>
          <option value="Admin">Quản trị viên (Admin)</option>
          <option value="Doctor">Bác sĩ (Doctor)</option>
          <option value="Receptionist">Lễ tân (Receptionist)</option>
          <option value="Patient">Bệnh nhân (Patient)</option>
        </select>
      </template>
    </AdminFilterToolbar>

    <!-- Table Container -->
    <div class="rounded-2xl border border-slate-200 bg-white p-5 shadow-2xs">
      <div class="mb-4 flex items-center justify-between">
        <div>
          <h2 class="text-base font-bold text-slate-800">Danh sách Tài khoản</h2>
          <p class="text-xs text-slate-400 mt-0.5">Tất cả tài khoản định danh trong hệ thống</p>
        </div>
      </div>

      <div class="overflow-x-auto rounded-xl border border-slate-100">
        <!-- Loading State -->
        <div v-if="loading" class="py-16 text-center text-slate-400 text-sm">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-3 border-[#0E4D92] border-t-transparent mb-2"></div>
          <p>Đang tải danh sách tài khoản…</p>
        </div>

        <!-- Table -->
        <table v-else class="w-full text-left text-sm border-collapse">
          <thead>
            <tr class="border-b border-slate-100 bg-slate-50/75 text-xs font-semibold uppercase tracking-wider text-slate-500">
              <th class="px-4 py-3.5">#</th>
              <th class="px-4 py-3.5">Người dùng</th>
              <th class="px-4 py-3.5">Email / Số điện thoại</th>
              <th class="px-4 py-3.5">Vai trò</th>
              <th class="px-4 py-3.5">Trạng thái</th>
              <th class="px-4 py-3.5">Ngày tạo</th>
              <th class="px-4 py-3.5 text-right">Thao tác</th>
            </tr>
          </thead>

          <tbody class="divide-y divide-slate-100">
            <tr
              v-for="(account, index) in accounts"
              :key="account.id"
              class="hover:bg-slate-50/80 transition-colors"
            >
              <td class="px-4 py-3.5 text-xs text-slate-400 font-mono">
                {{ (currentPage - 1) * pageSize + index + 1 }}
              </td>

              <!-- Người dùng -->
              <td class="px-4 py-3.5">
                <div class="font-bold text-slate-800">
                  {{ account.fullName || account.name || account.username }}
                </div>
                <div class="text-xs text-slate-400 mt-0.5 font-mono">
                  @{{ account.username }}
                </div>
              </td>

              <!-- Email / SĐT -->
              <td class="px-4 py-3.5 text-xs text-slate-600">
                <div>{{ account.email || '—' }}</div>
                <div class="text-slate-400 mt-0.5">{{ account.phoneNumber || '—' }}</div>
              </td>

              <!-- Role -->
              <td class="px-4 py-3.5">
                <AdminRoleBadge
                  :role="account.roles?.[0] || 'Patient'"
                />
              </td>

              <!-- Status -->
              <td class="px-4 py-3.5">
                <BaseStatusBadge
                  :status="account.isActive === false ? 'inactive' : 'active'"
                />
              </td>

              <!-- CreatedAt -->
              <td class="px-4 py-3.5 text-xs text-slate-500 font-mono">
                {{ account.createdAt ? new Date(account.createdAt).toLocaleDateString('vi-VN') : '—' }}
              </td>

              <!-- Actions -->
              <td class="px-4 py-3.5 text-right">
                <div class="inline-flex items-center gap-1.5">
                  <button
                    type="button"
                    class="rounded-lg px-2.5 py-1 text-xs font-semibold text-slate-600 hover:bg-slate-100 transition-colors cursor-pointer"
                    @click="openView(account)"
                  >
                    Xem
                  </button>

                  <button
                    type="button"
                    class="rounded-lg px-2.5 py-1 text-xs font-semibold text-[#0E4D92] hover:bg-blue-50 transition-colors cursor-pointer"
                    @click="openEdit(account)"
                  >
                    Sửa
                  </button>

                  <button
                    type="button"
                    class="rounded-lg px-2.5 py-1 text-xs font-semibold text-rose-600 hover:bg-rose-50 transition-colors cursor-pointer"
                    @click="confirmDelete(account)"
                  >
                    Khóa
                  </button>
                </div>
              </td>
            </tr>

            <!-- Empty State -->
            <tr v-if="!accounts || accounts.length === 0">
              <td colspan="7" class="px-4 py-12 text-center text-slate-400 text-sm">
                Không tìm thấy tài khoản phù hợp.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <AdminPagination
        v-model:current-page="currentPage"
        :total-pages="totalPages"
        :total-count="totalCount"
      />
    </div>

    <!-- User Form Dialog -->
    <UserFormDialog
      :open="formDialogOpen"
      :initial="editingUser"
      :submitting="submitting"
      @close="formDialogOpen = false"
      @submit="handleFormSubmit"
    />

    <!-- User Detail Modal -->
    <BaseModal
      :open="viewModalOpen"
      title="Chi tiết Tài khoản"
      size="md"
      icon="👤"
      @close="viewModalOpen = false"
    >
      <div v-if="viewingUser" class="space-y-4 text-sm">
        <div class="flex items-center gap-3.5 pb-4 border-b border-slate-100">
          <div class="w-12 h-12 rounded-full bg-slate-100 text-slate-700 flex items-center justify-center font-bold text-lg">
            {{ (viewingUser.fullName || viewingUser.name || viewingUser.username || 'U').charAt(0).toUpperCase() }}
          </div>
          <div>
            <h3 class="text-base font-bold text-slate-800">{{ viewingUser.fullName || viewingUser.name }}</h3>
            <p class="text-xs text-slate-500 font-mono">@{{ viewingUser.username }}</p>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3 text-xs">
          <div>
            <span class="text-slate-400 block">Email:</span>
            <span class="font-medium text-slate-700">{{ viewingUser.email || '—' }}</span>
          </div>
          <div>
            <span class="text-slate-400 block">Số điện thoại:</span>
            <span class="font-medium text-slate-700">{{ viewingUser.phoneNumber || '—' }}</span>
          </div>
          <div>
            <span class="text-slate-400 block">Vai trò:</span>
            <AdminRoleBadge :role="viewingUser.roles?.[0] || 'Patient'" />
          </div>
          <div>
            <span class="text-slate-400 block">Trạng thái:</span>
            <BaseStatusBadge :status="viewingUser.isActive === false ? 'inactive' : 'active'" />
          </div>
        </div>
      </div>
    </BaseModal>

    <!-- Delete Confirmation Modal -->
    <BaseModal
      :open="deleteConfirmOpen"
      title="Xác nhận vô hiệu hóa"
      size="sm"
      icon="🔒"
      @close="deleteConfirmOpen = false"
    >
      <p class="text-sm text-slate-600">
        Bạn có chắc chắn muốn khóa tài khoản <strong class="text-slate-800">@{{ deletingUser?.username }}</strong> không?
      </p>
      <template #footer>
        <BaseButton variant="outline" size="sm" @click="deleteConfirmOpen = false">Hủy</BaseButton>
        <BaseButton variant="danger" size="sm" :loading="deleting" @click="handleDelete">Xác nhận</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>
