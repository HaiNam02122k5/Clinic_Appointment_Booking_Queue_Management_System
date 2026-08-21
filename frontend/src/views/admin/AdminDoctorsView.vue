<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

import DoctorFormModal from '@/features/doctors/components/DoctorFormModal.vue'

import { doctorsApi } from '@/features/doctors/doctors.api'
import { specialtiesApi } from '@/features/specialties/specialties.api'

import type {
  CreateDoctorRequest,
  Doctor,
  DoctorDetail,
  DoctorStatus,
  UpdateDoctorRequest,
} from '@/features/doctors/doctors.types'

import type { Specialty } from '@/features/specialties/specialties.types'

// ==============================
// STATE
// ==============================

const doctors = ref<Doctor[]>([])
const specialties = ref<Specialty[]>([])

const loadingDoctors = ref(false)
const loadingSpecialties = ref(false)
const submitting = ref(false)

const errorMessage = ref('')

const search = ref('')
const selectedStatus = ref<DoctorStatus | ''>('')

const pageNumber = ref(1)
const pageSize = ref(5)

const totalCount = ref(0)
const totalPages = ref(1)

const isModalOpen = ref(false)

const selectedDoctor = ref<DoctorDetail | null>(null)

// ==============================
// PAGINATION
// ==============================

const pageNumbers = computed<number[]>(() => {
  const pages: number[] = []

  for (let i = 1; i <= totalPages.value; i += 1) {
    pages.push(i)
  }

  return pages
})

// ==============================
// LOAD DOCTORS
// ==============================

async function loadDoctors() {
  try {
    loadingDoctors.value = true
    errorMessage.value = ''

    const response = await doctorsApi.list({
      Search: search.value.trim() || undefined,
      Status: selectedStatus.value || undefined,
      PageNumber: pageNumber.value,
      PageSize: pageSize.value,
    })

    console.log('DOCTORS RESPONSE:', response)

    doctors.value = response?.items ?? []
    totalCount.value = response?.totalCount ?? 0
    totalPages.value = response?.totalPages ?? 1
  } catch (error) {
    console.error('FETCH DOCTORS ERROR:', error)

    doctors.value = []
    totalCount.value = 0
    totalPages.value = 1

    errorMessage.value = 'Không thể tải danh sách bác sĩ.'
  } finally {
    loadingDoctors.value = false
  }
}

// ==============================
// LOAD SPECIALTIES
// ==============================

async function loadSpecialties() {
  try {
    loadingSpecialties.value = true

    const response = await specialtiesApi.list({
      Page: 1,
      PageSize: 100,
    })

    console.log('SPECIALTIES RESPONSE:', response)

    specialties.value = response?.items ?? []
  } catch (error) {
    console.error('FETCH SPECIALTIES ERROR:', error)

    specialties.value = []
  } finally {
    loadingSpecialties.value = false
  }
}

// ==============================
// SEARCH
// ==============================

async function handleSearch() {
  pageNumber.value = 1

  await loadDoctors()
}

// ==============================
// STATUS FILTER
// ==============================

async function handleStatusChange() {
  pageNumber.value = 1

  await loadDoctors()
}

// ==============================
// RESET FILTERS
// ==============================

async function resetFilters() {
  search.value = ''
  selectedStatus.value = ''
  pageNumber.value = 1

  await loadDoctors()
}

// ==============================
// PAGINATION
// ==============================

async function changePage(page: number) {
  if (page < 1 || page > totalPages.value) {
    return
  }

  pageNumber.value = page

  await loadDoctors()
}

// ==============================
// CREATE DOCTOR
// ==============================

function openCreateModal() {
  selectedDoctor.value = null
  isModalOpen.value = true
}

// ==============================
// EDIT DOCTOR
// ==============================

async function openEditModal(doctor: Doctor) {
  try {
    submitting.value = true

    const doctorDetail = await doctorsApi.get(doctor.id)

    selectedDoctor.value = doctorDetail
    isModalOpen.value = true
  } catch (error) {
    console.error('GET DOCTOR ERROR:', error)

    alert('Không thể tải thông tin bác sĩ.')
  } finally {
    submitting.value = false
  }
}

// ==============================
// CLOSE MODAL
// ==============================

function closeModal() {
  if (submitting.value) {
    return
  }

  isModalOpen.value = false
  selectedDoctor.value = null
}

// ==============================
// SUBMIT DOCTOR
// ==============================

async function handleSubmitDoctor(payload: {
  mode: 'create' | 'edit'
  data: CreateDoctorRequest | UpdateDoctorRequest
}) {
  try {
    submitting.value = true

    if (payload.mode === 'create') {
      const createData = payload.data as CreateDoctorRequest

      await doctorsApi.create(createData)
    } else {
      if (!selectedDoctor.value) {
        throw new Error('Không tìm thấy bác sĩ cần cập nhật.')
      }

      const data = payload.data as UpdateDoctorRequest

      const updateData: UpdateDoctorRequest = {
        fullName: data.fullName.trim(),
        phoneNumber: data.phoneNumber.trim(),
        email: data.email.trim(),

        // BE yêu cầu DateOnly, không được gửi ""
        dateOfBirth: data.dateOfBirth?.trim() || '2000-01-01',

        gender: data.gender,
        address: data.address?.trim() || 'Chưa cập nhật',
        licenseNumber: data.licenseNumber.trim(),
        qualification: data.qualification.trim(),
        experienceYears: Number(data.experienceYears),
        biography: data.biography?.trim() || '',
      }

      console.log('UPDATE DOCTOR PAYLOAD:', updateData)

      await doctorsApi.update(
        selectedDoctor.value.id,
        updateData,
      )
    }

    isModalOpen.value = false
    selectedDoctor.value = null

    await loadDoctors()
  } catch (error) {
    console.error('SAVE DOCTOR ERROR:', error)

    alert('Không thể lưu thông tin bác sĩ.')
  } finally {
    submitting.value = false
  }
}

// ==============================
// HELPERS
// ==============================

function getGenderText(gender?: string) {
  if (gender === 'Male') {
    return 'Nam'
  }

  if (gender === 'Female') {
    return 'Nữ'
  }

  return 'Khác'
}

function getStatusText(status?: string) {
  if (status === 'Active') {
    return 'Đang hoạt động'
  }

  return 'Ngừng hoạt động'
}

// ==============================
// INITIAL LOAD
// ==============================

onMounted(async () => {
  await Promise.all([
    loadDoctors(),
    loadSpecialties(),
  ])
})
</script>

<template>
  <div class="doctors-page">
    <!-- HEADER -->
    <div class="page-header">
      <div>
        <h1>Quản lý bác sĩ</h1>
        <p>Quản lý thông tin và danh sách bác sĩ trong hệ thống</p>
      </div>

      <button
        type="button"
        class="btn-add"
        @click="openCreateModal"
      >
        + Thêm bác sĩ
      </button>
    </div>

    <!-- FILTER -->
    <div class="filter-card">
      <div class="search-group">
        <input
          v-model="search"
          type="text"
          placeholder="Tìm kiếm theo tên, email..."
          @keyup.enter="handleSearch"
        >

        <button
          type="button"
          class="btn-search"
          @click="handleSearch"
        >
          Tìm kiếm
        </button>
      </div>

      <select
        v-model="selectedStatus"
        class="status-filter"
        @change="handleStatusChange"
      >
        <option value="">
          Tất cả trạng thái
        </option>

        <option value="Active">
          Đang hoạt động
        </option>

        <option value="Inactive">
          Ngừng hoạt động
        </option>
      </select>

      <button
        type="button"
        class="btn-reset"
        @click="resetFilters"
      >
        Đặt lại
      </button>
    </div>

    <!-- ERROR -->
    <div
      v-if="errorMessage"
      class="error-message"
    >
      {{ errorMessage }}
    </div>

    <!-- TABLE -->
    <div class="table-card">
      <div class="table-header">
        <div>
          <h2>Danh sách bác sĩ</h2>

          <p>
            Tổng cộng {{ totalCount }} bác sĩ
          </p>
        </div>
      </div>

      <!-- LOADING -->
      <div
        v-if="loadingDoctors"
        class="loading-state"
      >
        Đang tải danh sách bác sĩ...
      </div>

      <!-- EMPTY -->
      <div
        v-else-if="doctors.length === 0"
        class="empty-state"
      >
        Chưa có bác sĩ nào.
      </div>

      <!-- TABLE CONTENT -->
      <div
        v-else
        class="table-wrapper"
      >
        <table>
          <thead>
            <tr>
              <th>Bác sĩ</th>
              <th>Liên hệ</th>
              <th>Chuyên môn</th>
              <th>Kinh nghiệm</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="doctor in doctors"
              :key="doctor.id"
            >
              <!-- DOCTOR -->
              <td>
                <div class="doctor-info">
                  <div class="avatar">
                    {{
                      doctor.fullName
                        ? doctor.fullName.charAt(0).toUpperCase()
                        : '?'
                    }}
                  </div>

                  <div>
                    <strong>
                      {{ doctor.fullName || 'Chưa cập nhật' }}
                    </strong>

                    <span>
                      {{ getGenderText(doctor.gender) }}
                    </span>
                  </div>
                </div>
              </td>

              <!-- CONTACT -->
              <td>
                <div class="contact-info">
                  <span>
                    {{ doctor.email || 'Chưa cập nhật' }}
                  </span>

                  <span>
                    {{ doctor.phoneNumber || 'Chưa cập nhật' }}
                  </span>
                </div>
              </td>

              <!-- SPECIALTY -->
              <td>
                <div class="professional-info">
                  <strong>
                    {{ doctor.currentSpecialty || 'Chưa cập nhật' }}
                  </strong>

                  <span>
                    {{ doctor.qualification || 'Chưa cập nhật' }}
                  </span>
                </div>
              </td>

              <!-- EXPERIENCE -->
              <td>
                {{ doctor.experienceYears ?? 0 }} năm
              </td>

              <!-- STATUS -->
              <td>
                <span
                  class="status-badge"
                  :class="{
                    active: doctor.status === 'Active',
                    inactive: doctor.status === 'Inactive',
                  }"
                >
                  {{ getStatusText(doctor.status) }}
                </span>
              </td>

              <!-- ACTION -->
              <td>
                <button
                  type="button"
                  class="btn-edit"
                  @click="openEditModal(doctor)"
                >
                  Chỉnh sửa
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- PAGINATION -->
      <div
        v-if="totalPages > 1"
        class="pagination"
      >
        <button
          type="button"
          :disabled="pageNumber === 1"
          @click="changePage(pageNumber - 1)"
        >
          ←
        </button>

        <button
          v-for="page in pageNumbers"
          :key="page"
          type="button"
          :class="{ active: page === pageNumber }"
          @click="changePage(page)"
        >
          {{ page }}
        </button>

        <button
          type="button"
          :disabled="pageNumber === totalPages"
          @click="changePage(pageNumber + 1)"
        >
          →
        </button>
      </div>
    </div>

    <!-- DOCTOR MODAL -->
    <DoctorFormModal
      :open="isModalOpen"
      :doctor="selectedDoctor"
      :specialties="specialties"
      :loading="submitting || loadingSpecialties"
      @close="closeModal"
      @submit="handleSubmitDoctor"
    />
  </div>
</template>

<style scoped>
.doctors-page {
  padding: 28px;
  color: #1e293b;
}

.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 24px;
}

.page-header h1 {
  margin: 0;
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
}

.page-header p {
  margin: 8px 0 0;
  color: #64748b;
}

.btn-add {
  padding: 11px 18px;
  border: none;
  border-radius: 8px;
  background: #2563eb;
  color: #ffffff;
  font-weight: 600;
  cursor: pointer;
}

.btn-add:hover {
  background: #1d4ed8;
}

.filter-card,
.table-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
}

.filter-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 18px;
  margin-bottom: 20px;
}

.search-group {
  display: flex;
  flex: 1;
}

.search-group input {
  flex: 1;
  padding: 11px 14px;
  border: 1px solid #cbd5e1;
  border-radius: 8px 0 0 8px;
  color: #1e293b;
  outline: none;
}

.btn-search {
  padding: 0 20px;
  border: none;
  border-radius: 0 8px 8px 0;
  background: #334155;
  color: #ffffff;
  cursor: pointer;
}

.status-filter {
  min-width: 180px;
  padding: 11px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background: #ffffff;
  color: #1e293b;
}

.btn-reset {
  padding: 11px 16px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  background: #ffffff;
  color: #475569;
  cursor: pointer;
}

.error-message {
  padding: 14px 18px;
  margin-bottom: 20px;
  border: 1px solid #fecaca;
  border-radius: 8px;
  background: #fef2f2;
  color: #dc2626;
}

.table-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
}

.table-header h2 {
  margin: 0;
  font-size: 18px;
}

.table-header p {
  margin: 5px 0 0;
  color: #64748b;
  font-size: 14px;
}

.table-wrapper {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th {
  padding: 14px 24px;
  background: #f8fafc;
  text-align: left;
  font-size: 12px;
  color: #64748b;
}

td {
  padding: 18px 24px;
  border-top: 1px solid #f1f5f9;
}

.doctor-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #dbeafe;
  color: #2563eb;
  font-weight: 700;
}

.doctor-info strong,
.professional-info strong {
  display: block;
  margin-bottom: 4px;
}

.doctor-info span,
.contact-info span,
.professional-info span {
  display: block;
  font-size: 13px;
  color: #64748b;
}

.contact-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.status-badge {
  display: inline-flex;
  padding: 6px 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 600;
}

.status-badge.active {
  background: #dcfce7;
  color: #15803d;
}

.status-badge.inactive {
  background: #fee2e2;
  color: #dc2626;
}

.btn-edit {
  padding: 8px 13px;
  border: 1px solid #bfdbfe;
  border-radius: 7px;
  background: #eff6ff;
  color: #2563eb;
  font-weight: 600;
  cursor: pointer;
}

.loading-state,
.empty-state {
  padding: 50px 20px;
  text-align: center;
  color: #64748b;
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 6px;
  padding: 20px;
}

.pagination button {
  min-width: 36px;
  height: 36px;
  padding: 0 10px;
  border: 1px solid #e2e8f0;
  border-radius: 7px;
  background: #ffffff;
  cursor: pointer;
}

.pagination button.active {
  border-color: #2563eb;
  background: #2563eb;
  color: #ffffff;
}

.pagination button:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .doctors-page {
    padding: 16px;
  }

  .page-header,
  .filter-card {
    flex-direction: column;
  }

  .filter-card {
    align-items: stretch;
  }

  .status-filter {
    width: 100%;
  }
}
</style>
