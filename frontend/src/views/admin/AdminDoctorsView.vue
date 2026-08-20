<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import axios from 'axios'
import DoctorFormModal from '@/features/doctors/components/DoctorFormModal.vue'
import { doctorsApi } from '@/features/doctors/doctors.api'
import { specialtiesApi } from '@/features/specialties/specialties.api'

import type {
  CreateDoctorRequest,
  Doctor,
  DoctorDetail,
  DoctorStatus,
  Specialty,
  UpdateDoctorRequest,
} from '@/features/doctors/doctors.types'

const doctors = ref<Doctor[]>([])
const specialties = ref<Specialty[]>([])

const loadingDoctors = ref(false)
const loadingSpecialties = ref(false)
const submitting = ref(false)

const search = ref('')
const selectedStatus = ref<DoctorStatus | ''>('')

const pageNumber = ref(1)
const pageSize = ref(5)

const totalCount = ref(0)
const totalPages = ref(1)

const isModalOpen = ref(false)
const selectedDoctor = ref<DoctorDetail | null>(null)

const pageNumbers = computed(() => {
  const pages: number[] = []

  for (let i = 1; i <= totalPages.value; i += 1) {
    pages.push(i)
  }

  return pages
})

const loadDoctors = async () => {
  try {
    loadingDoctors.value = true

    const response = await doctorsApi.list({
      Search: search.value || undefined,
      Status: selectedStatus.value || undefined,
      PageNumber: pageNumber.value,
      PageSize: pageSize.value,
    })

    doctors.value = response.items

    totalCount.value = response.totalCount
    totalPages.value = response.totalPages || 1
  } catch (error) {
    console.error('Không thể tải danh sách bác sĩ:', error)

    doctors.value = []
    totalCount.value = 0
    totalPages.value = 1
  } finally {
    loadingDoctors.value = false
  }
}

const loadSpecialties = async () => {
  try {
    loadingSpecialties.value = true

    const response = await specialtiesApi.list({
      Page: 1,
      PageSize: 100,
    })

    specialties.value = response.items
  } catch (error) {
    console.error('Không thể tải danh sách chuyên khoa:', error)

    specialties.value = []
  } finally {
    loadingSpecialties.value = false
  }
}

const handleSearch = async () => {
  pageNumber.value = 1

  await loadDoctors()
}

const handleStatusChange = async () => {
  pageNumber.value = 1

  await loadDoctors()
}

const changePage = async (page: number) => {
  if (page < 1 || page > totalPages.value) {
    return
  }

  pageNumber.value = page

  await loadDoctors()
}

const openCreateModal = () => {
  selectedDoctor.value = null

  isModalOpen.value = true
}

const openEditModal = async (doctor: Doctor) => {
  try {
    submitting.value = true

    const doctorDetail = await doctorsApi.get(doctor.id)

    selectedDoctor.value = doctorDetail

    isModalOpen.value = true
  } catch (error) {
    console.error('Không thể lấy thông tin bác sĩ:', error)

    alert('Không thể tải thông tin bác sĩ')
  } finally {
    submitting.value = false
  }
}

const closeModal = () => {
  if (submitting.value) return

  isModalOpen.value = false

  selectedDoctor.value = null
}

const handleSubmitDoctor = async (payload: {
  mode: 'create' | 'edit'
  data: CreateDoctorRequest | UpdateDoctorRequest
}) => {
  try {
    submitting.value = true

    if (payload.mode === 'create') {
      await doctorsApi.create(
        payload.data as CreateDoctorRequest,
      )
    } else {
      if (!selectedDoctor.value) {
        return
      }

      const data = payload.data as UpdateDoctorRequest

      const updateData: UpdateDoctorRequest = {
        fullName: data.fullName,
        phoneNumber: data.phoneNumber,
        email: data.email,
        dateOfBirth: data.dateOfBirth,
        gender: data.gender,
        address: data.address,
        licenseNumber: data.licenseNumber,
        qualification: data.qualification,
        experienceYears: Number(data.experienceYears),
        biography: data.biography,
      }

      console.log('UPDATE DATA:', updateData)

      await doctorsApi.update(
        selectedDoctor.value.id,
        updateData,
      )
    }

    isModalOpen.value = false
    selectedDoctor.value = null

    await loadDoctors()
  } catch (error: unknown) {
  console.error('UPDATE ERROR:', error)

  if (axios.isAxiosError(error)) {
    console.error('RESPONSE:', error.response?.data)

    const responseData = error.response?.data as {
      detail?: string
      errorMessages?: string[]
    }

    alert(
      responseData.detail ||
      responseData.errorMessages?.join(', ') ||
      'Không thể cập nhật bác sĩ',
    )
  } else {
    alert('Không thể cập nhật bác sĩ')
  }
} finally {
  submitting.value = false
}
}

const getGenderText = (gender: string) => {
  switch (gender) {
    case 'Male':
      return 'Nam'

    case 'Female':
      return 'Nữ'

    default:
      return 'Khác'
  }
}

const getStatusText = (status: DoctorStatus) => {
  return status === 'Active'
    ? 'Đang hoạt động'
    : 'Ngừng hoạt động'
}

onMounted(async () => {
  await Promise.all([
    loadDoctors(),
    loadSpecialties(),
  ])
})
</script>

<template>
  <div class="doctors-page">
    <div class="page-header">
      <div>
        <h1>Quản lý bác sĩ</h1>

        <p>
          Quản lý thông tin và tài khoản của các bác sĩ
        </p>
      </div>

      <button
        class="btn-add"
        type="button"
        @click="openCreateModal"
      >
        + Thêm bác sĩ
      </button>
    </div>

    <div class="filter-card">
      <div class="search-group">
        <input
          v-model="search"
          type="text"
          placeholder="Tìm kiếm tên, email hoặc số điện thoại..."
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
    </div>

    <div class="table-card">
      <div class="table-header">
        <div>
          <h2>Danh sách bác sĩ</h2>

          <p>
            Tổng cộng {{ totalCount }} bác sĩ
          </p>
        </div>
      </div>

      <div
        v-if="loadingDoctors"
        class="loading-state"
      >
        Đang tải danh sách bác sĩ...
      </div>

      <div
        v-else-if="doctors.length === 0"
        class="empty-state"
      >
        Chưa có bác sĩ nào.
      </div>

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
              <td>
                <div class="doctor-info">
                  <div class="avatar">
                    {{
                      doctor.fullName
                        .charAt(0)
                        .toUpperCase()
                    }}
                  </div>

                  <div>
                    <strong>
                      {{ doctor.fullName }}
                    </strong>

                    <span>
                      {{ getGenderText(doctor.gender) }}
                    </span>
                  </div>
                </div>
              </td>

              <td>
                <div class="contact-info">
                  <span>{{ doctor.email }}</span>

                  <span>{{ doctor.phoneNumber }}</span>
                </div>
              </td>

              <td>
                <div class="professional-info">
                  <strong>
                    {{ doctor.currentSpecialty }}
                  </strong>

                  <span>
                    {{ doctor.qualification }}
                  </span>
                </div>
              </td>

              <td>
                {{ doctor.experienceYears }} năm
              </td>

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

              <td>
                <button
                  class="btn-edit"
                  type="button"
                  :disabled="submitting"
                  @click="openEditModal(doctor)"
                >
                  Chỉnh sửa
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

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

  color: white;

  font-weight: 600;

  cursor: pointer;
}

.btn-add:hover {
  background: #1d4ed8;
}

.filter-card,
.table-card {
  background: white;

  border: 1px solid #e2e8f0;
  border-radius: 12px;

  box-shadow: 0 2px 8px rgb(15 23 42 / 4%);
}

.filter-card {
  display: flex;
  gap: 16px;

  padding: 18px;

  margin-bottom: 24px;
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

  outline: none;
}

.btn-search {
  padding: 0 20px;

  border: none;
  border-radius: 0 8px 8px 0;

  background: #334155;

  color: white;

  cursor: pointer;
}

.status-filter {
  min-width: 190px;

  padding: 0 12px;

  border: 1px solid #cbd5e1;
  border-radius: 8px;

  background: white;
}

.table-header {
  display: flex;
  justify-content: space-between;

  padding: 20px 24px;

  border-bottom: 1px solid #e2e8f0;
}

.table-header h2 {
  margin: 0;

  font-size: 18px;
}

.table-header p {
  margin: 5px 0 0;

  font-size: 14px;

  color: #64748b;
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
  font-weight: 700;

  color: #64748b;

  text-transform: uppercase;
}

td {
  padding: 18px 24px;

  border-top: 1px solid #f1f5f9;

  font-size: 14px;
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

.btn-edit:hover {
  background: #dbeafe;
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

  background: white;

  cursor: pointer;
}

.pagination button.active {
  border-color: #2563eb;

  background: #2563eb;

  color: white;
}

.pagination button:disabled {
  cursor: not-allowed;

  opacity: 0.4;
}

@media (max-width: 768px) {
  .doctors-page {
    padding: 16px;
  }

  .page-header,
  .filter-card {
    flex-direction: column;
  }

  .page-header {
    gap: 16px;
  }

  .filter-card {
    align-items: stretch;
  }

  .status-filter {
    min-width: auto;
    min-height: 42px;
  }
}
</style>
