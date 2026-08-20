<script setup lang="ts">
import {
  onMounted,
  reactive,
  ref,
} from 'vue'
import { AxiosError } from 'axios'
import { specialtiesApi } from '@/features/specialties/specialties.api'

import type {
  Specialty,
} from '@/features/specialties/specialties.types'

// =================================
// LIST STATE
// =================================

const search = ref('')

const currentPage = ref(1)

const pageSize = 5

const specialties = ref<Specialty[]>([])

const totalPages = ref(1)

const totalCount = ref(0)

const loading = ref(false)

const error = ref('')

// =================================
// CREATE MODAL STATE
// =================================

const isCreateModalOpen = ref(false)

const submitting = ref(false)

const createError = ref('')

const createForm = reactive({
  name: '',
  description: '',
  establishedDate: '',
})

// =================================
// VIEW MODAL STATE
// =================================

const isViewModalOpen = ref(false)

const viewing = ref(false)

const selectedSpecialty = ref<Specialty | null>(null)

// =================================
// EDIT MODAL STATE
// =================================

const isEditModalOpen = ref(false)

const updating = ref(false)

const editError = ref('')

const editForm = reactive({
  name: '',
  description: '',
  establishedDate: '',
})

// =================================
// FETCH SPECIALTIES
// =================================

async function fetchSpecialties() {
  try {
    loading.value = true
    error.value = ''

    const response = await specialtiesApi.list({
      Search: search.value.trim() || undefined,
      Page: currentPage.value,
      PageSize: pageSize,
    })

    specialties.value = response?.items ?? []

    totalPages.value = response?.totalPages ?? 1

    totalCount.value = response?.totalCount ?? 0

    // Tránh trường hợp đang ở trang không còn tồn tại
    if (
      currentPage.value > totalPages.value &&
      totalPages.value > 0
    ) {
      currentPage.value = totalPages.value

      await fetchSpecialties()
    }
  } catch (err) {
    console.error(err)

    specialties.value = []

    totalPages.value = 1

    totalCount.value = 0

    error.value =
      'Không thể tải danh sách chuyên khoa.'
  } finally {
    loading.value = false
  }
}

// =================================
// SEARCH
// =================================

function handleSearch() {
  currentPage.value = 1

  fetchSpecialties()
}

// =================================
// RESET FILTER
// =================================

function handleReset() {
  search.value = ''

  currentPage.value = 1

  fetchSpecialties()
}

// =================================
// PAGINATION
// =================================

function goToPreviousPage() {
  if (currentPage.value <= 1) return

  currentPage.value--

  fetchSpecialties()
}

function goToNextPage() {
  if (currentPage.value >= totalPages.value) return

  currentPage.value++

  fetchSpecialties()
}

// =================================
// CREATE MODAL
// =================================

function openCreateModal() {
  createError.value = ''

  createForm.name = ''
  createForm.description = ''
  createForm.establishedDate = ''

  isCreateModalOpen.value = true
}

function closeCreateModal() {
  if (submitting.value) return

  isCreateModalOpen.value = false
}

// =================================
// CREATE SPECIALTY
// =================================

async function handleCreateSpecialty() {
  createError.value = ''

  if (!createForm.name.trim()) {
    createError.value =
      'Vui lòng nhập tên chuyên khoa.'

    return
  }

  if (!createForm.description.trim()) {
    createError.value =
      'Vui lòng nhập mô tả.'

    return
  }

  if (!createForm.establishedDate) {
    createError.value =
      'Vui lòng chọn ngày thành lập.'

    return
  }

  try {
    submitting.value = true

    await specialtiesApi.create({
      name: createForm.name.trim(),
      description: createForm.description.trim(),
      establishedDate: createForm.establishedDate,
    })

    isCreateModalOpen.value = false

    currentPage.value = 1

    await fetchSpecialties()
  } catch (err) {
    console.error(err)

    createError.value =
      'Không thể thêm chuyên khoa. Vui lòng thử lại.'
  } finally {
    submitting.value = false
  }
}

// =================================
// VIEW SPECIALTY
// =================================

async function handleView(id: string) {
  try {
    viewing.value = true

    const specialty =
      await specialtiesApi.get(id)

    selectedSpecialty.value = specialty

    isViewModalOpen.value = true
  } catch (err) {
    console.error(err)

    alert(
      'Không thể tải thông tin chuyên khoa.',
    )
  } finally {
    viewing.value = false
  }
}

function closeViewModal() {
  isViewModalOpen.value = false

  selectedSpecialty.value = null
}

// =================================
// EDIT SPECIALTY
// =================================

async function handleEdit(id: string) {
  try {
    editError.value = ''

    const specialty =
      await specialtiesApi.get(id)

    selectedSpecialty.value = specialty

    editForm.name = specialty.name ?? ''

    editForm.description =
      specialty.description ?? ''

    editForm.establishedDate =
      specialty.establishedDate ?? ''

    isEditModalOpen.value = true
  } catch (err) {
    console.error(err)

    alert(
      'Không thể tải thông tin chuyên khoa.',
    )
  }
}

function closeEditModal() {
  if (updating.value) return

  isEditModalOpen.value = false

  editError.value = ''

  selectedSpecialty.value = null
}

// =================================
// UPDATE SPECIALTY
// =================================

async function handleUpdateSpecialty() {
  if (!selectedSpecialty.value) return

  editError.value = ''

  if (!editForm.name.trim()) {
    editError.value =
      'Vui lòng nhập tên chuyên khoa.'

    return
  }

  if (!editForm.description.trim()) {
    editError.value =
      'Vui lòng nhập mô tả.'

    return
  }

  if (!editForm.establishedDate) {
    editError.value =
      'Vui lòng chọn ngày thành lập.'

    return
  }

  try {
    updating.value = true

    await specialtiesApi.update(
      selectedSpecialty.value.id,
      {
        name: editForm.name.trim(),
        description: editForm.description.trim(),
        establishedDate: editForm.establishedDate,
      },
    )

    isEditModalOpen.value = false

    selectedSpecialty.value = null

    await fetchSpecialties()
  } catch (err: unknown) {
    console.error(
      'UPDATE SPECIALTY ERROR:',
      err,
    )

    if (err instanceof AxiosError) {
      console.error(
        'STATUS:',
        err.response?.status,
      )

      console.error(
        'RESPONSE DATA:',
        err.response?.data,
      )

      if (err.response?.status === 401) {
        editError.value =
          'Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn.'
      } else if (err.response?.status === 404) {
        editError.value =
          'Không tìm thấy chuyên khoa.'
      } else if (err.response?.status === 400) {
        editError.value =
          'Dữ liệu cập nhật không hợp lệ.'
      } else {
        editError.value =
          'Không thể cập nhật chuyên khoa.'
      }
    } else {
      editError.value =
        'Đã xảy ra lỗi không xác định.'
    }
  } finally {
    updating.value = false
  }
}

// =================================
// FORMAT DATE
// =================================

function formatDate(date: string) {
  if (!date) return '-'

  const value = new Date(date)

  if (Number.isNaN(value.getTime())) {
    return date
  }

  return value.toLocaleDateString('vi-VN')
}

// =================================
// INITIAL LOAD
// =================================

onMounted(() => {
  fetchSpecialties()
})
</script>

<template>
  <div class="space-y-6">

    <!-- ========================= -->
    <!-- PAGE HEADER -->
    <!-- ========================= -->

    <div>
      <h1
        class="
          text-2xl
          font-semibold
          text-slate-800
        "
      >
        Quản lý chuyên khoa
      </h1>

      <p
        class="
          mt-1
          text-sm
          text-slate-500
        "
      >
        Quản lý danh sách các chuyên khoa
        trong hệ thống
      </p>
    </div>

    <!-- ========================= -->
    <!-- FILTER -->
    <!-- ========================= -->

    <div
      class="
        rounded-xl
        border
        border-slate-200
        bg-white
        p-6
        shadow-sm
      "
    >
      <div
        class="
          flex
          flex-col
          gap-4
          lg:flex-row
          lg:items-center
          lg:justify-between
        "
      >
        <!-- SEARCH -->

        <div
          class="
            relative
            w-full
            lg:max-w-md
          "
        >
          <svg
            class="
              absolute
              left-3
              top-1/2
              h-5
              w-5
              -translate-y-1/2
              text-slate-400
            "
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke-width="2"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="m21 21-4.35-4.35m2.1-5.4a7.5 7.5 0 1 1-15 0 7.5 7.5 0 0 1 15 0Z"
            />
          </svg>

          <input
            v-model="search"
            type="text"
            placeholder="Tìm kiếm chuyên khoa..."
            class="
              w-full
              rounded-lg
              border
              border-slate-300
              bg-white
              py-2.5
              pl-10
              pr-4
              text-sm
              text-slate-800
              placeholder:text-slate-400
              outline-none
              focus:border-violet-500
              focus:ring-2
              focus:ring-violet-100
            "
            @keyup.enter="handleSearch"
          >
        </div>

        <!-- ACTIONS -->

        <div class="flex items-center gap-3">

          <button
            type="button"
            class="
              rounded-lg
              border
              border-slate-300
              bg-white
              px-5
              py-2.5
              text-sm
              font-medium
              text-slate-600
              transition
              hover:bg-slate-50
            "
            @click="handleReset"
          >
            Đặt lại
          </button>

          <button
            type="button"
            class="
              rounded-lg
              bg-violet-600
              px-5
              py-2.5
              text-sm
              font-medium
              text-white
              transition
              hover:bg-violet-700
            "
            @click="openCreateModal"
          >
            + Thêm chuyên khoa
          </button>

        </div>
      </div>
    </div>

    <!-- ========================= -->
    <!-- TABLE -->
    <!-- ========================= -->

    <div
      class="
        overflow-hidden
        rounded-xl
        border
        border-slate-200
        bg-white
        shadow-sm
      "
    >
      <!-- TABLE HEADER -->

      <div
        class="
          border-b
          border-slate-200
          px-6
          py-5
        "
      >
        <h2
          class="
            text-lg
            font-semibold
            text-slate-800
          "
        >
          Danh sách chuyên khoa
        </h2>

        <p
          class="
            mt-1
            text-sm
            text-slate-500
          "
        >
          Tổng cộng {{ totalCount }} chuyên khoa
        </p>
      </div>

      <div class="overflow-x-auto">

        <table class="w-full">

          <thead>

            <tr
              class="
                border-b
                border-slate-200
                bg-slate-50
              "
            >

              <th
                class="
                  px-5
                  py-4
                  text-left
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-500
                "
              >
                #
              </th>

              <th
                class="
                  px-5
                  py-4
                  text-left
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-500
                "
              >
                Chuyên khoa
              </th>

              <th
                class="
                  px-5
                  py-4
                  text-left
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-500
                "
              >
                Mô tả
              </th>

              <th
                class="
                  px-5
                  py-4
                  text-left
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-500
                "
              >
                Ngày thành lập
              </th>

              <th
                class="
                  px-5
                  py-4
                  text-right
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-500
                "
              >
                Thao tác
              </th>

            </tr>

          </thead>

          <tbody
            class="
              divide-y
              divide-slate-100
            "
          >

            <!-- LOADING -->

            <tr v-if="loading">

              <td
                colspan="5"
                class="
                  px-5
                  py-12
                  text-center
                  text-sm
                  text-slate-400
                "
              >
                Đang tải danh sách chuyên khoa...
              </td>

            </tr>

            <!-- ERROR -->

            <tr v-else-if="error">

              <td
                colspan="5"
                class="
                  px-5
                  py-12
                  text-center
                  text-sm
                  text-red-500
                "
              >
                {{ error }}
              </td>

            </tr>

            <!-- EMPTY -->

            <tr
              v-else-if="
                specialties.length === 0
              "
            >

              <td
                colspan="5"
                class="
                  px-5
                  py-12
                  text-center
                  text-sm
                  text-slate-400
                "
              >
                Không tìm thấy chuyên khoa phù hợp.
              </td>

            </tr>

            <!-- DATA -->

            <tr
              v-for="
                (specialty, index)
                in specialties
              "
              :key="specialty.id"
              class="hover:bg-slate-50"
            >

              <td
                class="
                  px-5
                  py-4
                  text-sm
                  text-slate-500
                "
              >
                {{
                  (currentPage - 1) *
                    pageSize +
                  index +
                  1
                }}
              </td>

              <td
                class="
                  px-5
                  py-4
                  font-medium
                  text-slate-800
                "
              >
                {{ specialty.name }}
              </td>

              <td
                class="
                  max-w-md
                  px-5
                  py-4
                  text-sm
                  text-slate-600
                "
              >
                {{ specialty.description }}
              </td>

              <td
                class="
                  px-5
                  py-4
                  whitespace-nowrap
                  text-sm
                  text-slate-600
                "
              >
                {{
                  formatDate(
                    specialty.establishedDate,
                  )
                }}
              </td>

              <!-- ACTIONS -->

              <td class="px-5 py-4">

                <div
                  class="
                    flex
                    justify-end
                    gap-2
                  "
                >

                  <button
                    type="button"
                    class="
                      rounded-md
                      px-3
                      py-1.5
                      text-sm
                      font-medium
                      text-blue-600
                      transition
                      hover:bg-blue-50
                    "
                    @click="
                      handleView(
                        specialty.id,
                      )
                    "
                  >
                    Xem
                  </button>

                  <button
                    type="button"
                    class="
                      rounded-md
                      px-3
                      py-1.5
                      text-sm
                      font-medium
                      text-violet-600
                      transition
                      hover:bg-violet-50
                    "
                    @click="
                      handleEdit(
                        specialty.id,
                      )
                    "
                  >
                    Sửa
                  </button>

                </div>

              </td>

            </tr>

          </tbody>

        </table>

      </div>

      <!-- ========================= -->
      <!-- PAGINATION -->
      <!-- ========================= -->

      <div
        v-if="
          !loading &&
          !error &&
          totalPages > 1
        "
        class="
          flex
          items-center
          justify-between
          border-t
          border-slate-200
          px-6
          py-4
        "
      >

        <button
          type="button"
          class="
            rounded-lg
            border
            border-slate-300
            bg-white
            px-4
            py-2
            text-sm
            text-slate-700
            disabled:cursor-not-allowed
            disabled:opacity-50
          "
          :disabled="
            currentPage === 1
          "
          @click="goToPreviousPage"
        >
          Trước
        </button>

        <span
          class="
            text-sm
            text-slate-600
          "
        >
          Trang
          {{ currentPage }}
          /
          {{ totalPages }}
        </span>

        <button
          type="button"
          class="
            rounded-lg
            border
            border-slate-300
            bg-white
            px-4
            py-2
            text-sm
            text-slate-700
            disabled:cursor-not-allowed
            disabled:opacity-50
          "
          :disabled="
            currentPage >= totalPages
          "
          @click="goToNextPage"
        >
          Sau
        </button>

      </div>

    </div>

    <!-- ========================= -->
    <!-- CREATE MODAL -->
    <!-- ========================= -->

    <Teleport to="body">

      <div
        v-if="isCreateModalOpen"
        class="
          fixed
          inset-0
          z-50
          flex
          items-center
          justify-center
          bg-black/40
          px-4
        "
        @click.self="closeCreateModal"
      >

        <div
          class="
            w-full
            max-w-lg
            rounded-xl
            bg-white
            shadow-xl
          "
        >

          <div
            class="
              flex
              items-center
              justify-between
              border-b
              border-slate-200
              px-6
              py-4
            "
          >

            <div>

              <h2
                class="
                  text-lg
                  font-semibold
                  text-slate-800
                "
              >
                Thêm chuyên khoa
              </h2>

              <p
                class="
                  mt-1
                  text-sm
                  text-slate-500
                "
              >
                Thêm chuyên khoa mới
                vào hệ thống
              </p>

            </div>

            <button
              type="button"
              class="
                text-xl
                text-slate-500
                hover:text-slate-800
              "
              @click="closeCreateModal"
            >
              ×
            </button>

          </div>

          <form
            class="
              space-y-5
              px-6
              py-6
            "
            @submit.prevent="
              handleCreateSpecialty
            "
          >

            <!-- NAME -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Tên chuyên khoa
              </label>

              <input
                v-model="createForm.name"
                type="text"
                placeholder="Ví dụ: Nội khoa"
                class="
                  w-full
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  placeholder:text-slate-400
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              >

            </div>

            <!-- DESCRIPTION -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Mô tả
              </label>

              <textarea
                v-model="
                  createForm.description
                "
                rows="4"
                placeholder="
                  Nhập mô tả chuyên khoa...
                "
                class="
                  w-full
                  resize-none
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  placeholder:text-slate-400
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              />

            </div>

            <!-- DATE -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Ngày thành lập
              </label>

              <input
                v-model="
                  createForm.establishedDate
                "
                type="date"
                class="
                  w-full
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              >

            </div>

            <!-- ERROR -->

            <div
              v-if="createError"
              class="
                rounded-lg
                bg-red-50
                px-4
                py-3
                text-sm
                text-red-600
              "
            >
              {{ createError }}
            </div>

            <!-- BUTTONS -->

            <div
              class="
                flex
                justify-end
                gap-3
                border-t
                border-slate-100
                pt-5
              "
            >

              <button
                type="button"
                class="
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-5
                  py-2.5
                  text-sm
                  font-medium
                  text-slate-700
                  hover:bg-slate-50
                "
                @click="closeCreateModal"
              >
                Hủy
              </button>

              <button
                type="submit"
                :disabled="submitting"
                class="
                  rounded-lg
                  bg-violet-600
                  px-5
                  py-2.5
                  text-sm
                  font-medium
                  text-white
                  hover:bg-violet-700
                  disabled:opacity-50
                "
              >
                {{
                  submitting
                    ? 'Đang thêm...'
                    : 'Thêm chuyên khoa'
                }}
              </button>

            </div>

          </form>

        </div>

      </div>

    </Teleport>

    <!-- ========================= -->
    <!-- VIEW MODAL -->
    <!-- ========================= -->

    <Teleport to="body">

      <div
        v-if="
          isViewModalOpen &&
          selectedSpecialty
        "
        class="
          fixed
          inset-0
          z-50
          flex
          items-center
          justify-center
          bg-black/40
          px-4
        "
        @click.self="closeViewModal"
      >

        <div
          class="
            w-full
            max-w-lg
            rounded-xl
            bg-white
            shadow-xl
          "
        >

          <!-- HEADER -->

          <div
            class="
              flex
              items-center
              justify-between
              border-b
              border-slate-200
              px-6
              py-5
            "
          >

            <div>

              <h2
                class="
                  text-lg
                  font-semibold
                  text-slate-800
                "
              >
                Thông tin chuyên khoa
              </h2>

              <p
                class="
                  mt-1
                  text-sm
                  text-slate-500
                "
              >
                Chi tiết chuyên khoa
              </p>

            </div>

            <button
              type="button"
              class="
                text-xl
                text-slate-500
                hover:text-slate-800
              "
              @click="closeViewModal"
            >
              ×
            </button>

          </div>

          <!-- CONTENT -->

          <div
            class="
              space-y-6
              px-6
              py-6
            "
          >

            <div>

              <p
                class="
                  mb-1
                  text-sm
                  text-slate-500
                "
              >
                Tên chuyên khoa
              </p>

              <p
                class="
                  font-medium
                  text-slate-800
                "
              >
                {{ selectedSpecialty.name }}
              </p>

            </div>

            <div>

              <p
                class="
                  mb-1
                  text-sm
                  text-slate-500
                "
              >
                Mô tả
              </p>

              <p
                class="
                  leading-6
                  text-slate-800
                "
              >
                {{
                  selectedSpecialty.description ||
                  'Chưa có mô tả'
                }}
              </p>

            </div>

            <div>

              <p
                class="
                  mb-1
                  text-sm
                  text-slate-500
                "
              >
                Ngày thành lập
              </p>

              <p
                class="
                  font-medium
                  text-slate-800
                "
              >
                {{
                  formatDate(
                    selectedSpecialty.establishedDate,
                  )
                }}
              </p>

            </div>

          </div>

          <!-- FOOTER -->

          <div
            class="
              flex
              justify-end
              border-t
              border-slate-200
              px-6
              py-4
            "
          >

            <button
              type="button"
              class="
                rounded-lg
                border
                border-slate-300
                px-5
                py-2.5
                text-sm
                font-medium
                text-slate-700
                hover:bg-slate-50
              "
              @click="closeViewModal"
            >
              Đóng
            </button>

          </div>

        </div>

      </div>

    </Teleport>

    <!-- ========================= -->
    <!-- EDIT MODAL -->
    <!-- ========================= -->

    <Teleport to="body">

      <div
        v-if="isEditModalOpen"
        class="
          fixed
          inset-0
          z-50
          flex
          items-center
          justify-center
          bg-black/40
          px-4
        "
        @click.self="closeEditModal"
      >

        <div
          class="
            w-full
            max-w-lg
            rounded-xl
            bg-white
            shadow-xl
          "
        >

          <!-- HEADER -->

          <div
            class="
              flex
              items-center
              justify-between
              border-b
              border-slate-200
              px-6
              py-4
            "
          >

            <div>

              <h2
                class="
                  text-lg
                  font-semibold
                  text-slate-800
                "
              >
                Chỉnh sửa chuyên khoa
              </h2>

              <p
                class="
                  mt-1
                  text-sm
                  text-slate-500
                "
              >
                Cập nhật thông tin chuyên khoa
              </p>

            </div>

            <button
              type="button"
              class="
                text-xl
                text-slate-500
                hover:text-slate-800
              "
              @click="closeEditModal"
            >
              ×
            </button>

          </div>

          <!-- FORM -->

          <form
            class="
              space-y-5
              px-6
              py-6
            "
            @submit.prevent="
              handleUpdateSpecialty
            "
          >

            <!-- NAME -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Tên chuyên khoa
              </label>

              <input
                v-model="editForm.name"
                type="text"
                class="
                  w-full
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              >

            </div>

            <!-- DESCRIPTION -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Mô tả
              </label>

              <textarea
                v-model="
                  editForm.description
                "
                rows="4"
                class="
                  w-full
                  resize-none
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              />

            </div>

            <!-- DATE -->

            <div>

              <label
                class="
                  mb-2
                  block
                  text-sm
                  font-medium
                  text-slate-700
                "
              >
                Ngày thành lập
              </label>

              <input
                v-model="
                  editForm.establishedDate
                "
                type="date"
                class="
                  w-full
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-3
                  py-2.5
                  text-sm
                  text-slate-800
                  outline-none
                  focus:border-violet-500
                  focus:ring-2
                  focus:ring-violet-100
                "
              >

            </div>

            <!-- ERROR -->

            <div
              v-if="editError"
              class="
                rounded-lg
                bg-red-50
                px-4
                py-3
                text-sm
                text-red-600
              "
            >
              {{ editError }}
            </div>

            <!-- ACTIONS -->

            <div
              class="
                flex
                justify-end
                gap-3
                border-t
                border-slate-100
                pt-5
              "
            >

              <button
                type="button"
                class="
                  rounded-lg
                  border
                  border-slate-300
                  bg-white
                  px-5
                  py-2.5
                  text-sm
                  font-medium
                  text-slate-700
                  hover:bg-slate-50
                "
                :disabled="updating"
                @click="closeEditModal"
              >
                Hủy
              </button>

              <button
                type="submit"
                :disabled="updating"
                class="
                  rounded-lg
                  bg-violet-600
                  px-5
                  py-2.5
                  text-sm
                  font-medium
                  text-white
                  hover:bg-violet-700
                  disabled:opacity-50
                "
              >
                {{
                  updating
                    ? 'Đang lưu...'
                    : 'Lưu thay đổi'
                }}
              </button>

            </div>

          </form>

        </div>

      </div>

    </Teleport>

  </div>
</template>
