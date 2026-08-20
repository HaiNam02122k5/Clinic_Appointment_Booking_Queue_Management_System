<script setup lang="ts">
import { computed, reactive, watch } from 'vue'

import type {
  CreateDoctorRequest,
  Doctor,
  DoctorGender,
  DoctorStatus,
  Specialty,
  UpdateDoctorRequest,
} from '../doctors.types'

interface Props {
  open: boolean
  doctor?: Doctor | null
  specialties: Specialty[]
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  doctor: null,
  loading: false,
})

const emit = defineEmits<{
  close: []

  submit: [
    payload: {
      mode: 'create' | 'edit'
      data: CreateDoctorRequest | UpdateDoctorRequest
    },
  ]
}>()

const isEditMode = computed(() => !!props.doctor)

const form = reactive({
  fullName: '',
  phoneNumber: '',
  email: '',
  gender: 'Male' as DoctorGender,
  dateOfBirth: '',
  address: '',

  hireDate: '',
  licenseNumber: '',
  qualification: '',
  experienceYears: 0,
  status: 'Active' as DoctorStatus,
  specialtyId: '',

  username: '',
  password: '',

  biography: '',
})

const resetForm = () => {
  form.fullName = ''
  form.phoneNumber = ''
  form.email = ''
  form.gender = 'Male'
  form.dateOfBirth = ''
  form.address = ''

  form.hireDate = ''
  form.licenseNumber = ''
  form.qualification = ''
  form.experienceYears = 0
  form.status = 'Active'
  form.specialtyId = ''

  form.username = ''
  form.password = ''

  form.biography = ''
}

watch(
  () => props.open,
  (open) => {
    if (!open) return

    resetForm()

    if (props.doctor) {
      form.fullName = props.doctor.fullName
      form.phoneNumber = props.doctor.phoneNumber
      form.email = props.doctor.email
      form.gender = props.doctor.gender

      form.licenseNumber = props.doctor.licenseNumber
      form.qualification = props.doctor.qualification
      form.experienceYears = props.doctor.experienceYears
      form.status = props.doctor.status

      form.biography = props.doctor.biography ?? ''
    }
  },
)

const handleSubmit = () => {
  if (isEditMode.value) {
    const updateData: UpdateDoctorRequest = {
      fullName: form.fullName,
      phoneNumber: form.phoneNumber,
      email: form.email,
      dateOfBirth: form.dateOfBirth,
      gender: form.gender,
      address: form.address,

      licenseNumber: form.licenseNumber,
      qualification: form.qualification,
      experienceYears: Number(form.experienceYears),
      biography: form.biography,
    }

    emit('submit', {
      mode: 'edit',
      data: updateData,
    })

    return
  }

  const createData: CreateDoctorRequest = {
    hireDate: form.hireDate,
    licenseNumber: form.licenseNumber,
    qualification: form.qualification,
    experienceYears: Number(form.experienceYears),
    status: form.status,
    specialtyId: form.specialtyId,

    email: form.email,
    address: form.address,
    username: form.username,
    password: form.password,
    fullName: form.fullName,
    phoneNumber: form.phoneNumber,
    dateOfBirth: form.dateOfBirth,
    gender: form.gender,
    biography: form.biography,
  }

  emit('submit', {
    mode: 'create',
    data: createData,
  })
}

const handleClose = () => {
  if (props.loading) return

  emit('close')
}
</script>

<template>
  <Teleport to="body">
    <div
      v-if="open"
      class="modal-backdrop"
      @click.self="handleClose"
    >
      <div class="doctor-modal">
        <div class="modal-header">
          <div>
            <h2>
              {{
                isEditMode
                  ? 'Chỉnh sửa bác sĩ'
                  : 'Thêm bác sĩ'
              }}
            </h2>

            <p>
              {{
                isEditMode
                  ? 'Cập nhật thông tin bác sĩ'
                  : 'Nhập thông tin để tạo tài khoản bác sĩ mới'
              }}
            </p>
          </div>

          <button
            class="close-button"
            type="button"
            :disabled="loading"
            @click="handleClose"
          >
            ×
          </button>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <!-- THÔNG TIN CÁ NHÂN -->
            <section class="form-section">
              <h3>Thông tin cá nhân</h3>

              <div class="form-grid">
                <div class="form-group">
                  <label>Họ và tên *</label>

                  <input
                    v-model="form.fullName"
                    type="text"
                    placeholder="Nhập họ và tên"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Số điện thoại *</label>

                  <input
                    v-model="form.phoneNumber"
                    type="tel"
                    placeholder="Ví dụ: 0912345678"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Email *</label>

                  <input
                    v-model="form.email"
                    type="email"
                    placeholder="doctor@example.com"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Giới tính *</label>

                  <select v-model="form.gender">
                    <option value="Male">
                      Nam
                    </option>

                    <option value="Female">
                      Nữ
                    </option>

                    <option value="Other">
                      Khác
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>Ngày sinh</label>

                  <input
                    v-model="form.dateOfBirth"
                    type="date"
                  >
                </div>

                <div class="form-group">
                  <label>Địa chỉ</label>

                  <input
                    v-model="form.address"
                    type="text"
                    placeholder="Nhập địa chỉ"
                  >
                </div>
              </div>
            </section>

            <!-- THÔNG TIN CHUYÊN MÔN -->
            <section class="form-section">
              <h3>Thông tin chuyên môn</h3>

              <div class="form-grid">
                <!-- CREATE ONLY -->
                <div
                  v-if="!isEditMode"
                  class="form-group"
                >
                  <label>Ngày tuyển dụng *</label>

                  <input
                    v-model="form.hireDate"
                    type="date"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Số giấy phép *</label>

                  <input
                    v-model="form.licenseNumber"
                    type="text"
                    placeholder="Ví dụ: BS-HN-2026-001"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Trình độ *</label>

                  <input
                    v-model="form.qualification"
                    type="text"
                    placeholder="Ví dụ: Bác sĩ đa khoa"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Số năm kinh nghiệm *</label>

                  <input
                    v-model.number="form.experienceYears"
                    type="number"
                    min="0"
                    required
                  >
                </div>

                <!-- CREATE ONLY -->
                <div
                  v-if="!isEditMode"
                  class="form-group"
                >
                  <label>Chuyên khoa *</label>

                  <select
                    v-model="form.specialtyId"
                    :disabled="loading"
                    required
                  >
                    <option value="">
                      Chọn chuyên khoa
                    </option>

                    <option
                      v-for="specialty in props.specialties"
                      :key="specialty.id"
                      :value="specialty.id"
                    >
                      {{ specialty.name }}
                    </option>
                  </select>
                </div>

                <!-- CREATE ONLY -->
                <div
                  v-if="!isEditMode"
                  class="form-group"
                >
                  <label>Trạng thái *</label>

                  <select v-model="form.status">
                    <option value="Active">
                      Hoạt động
                    </option>

                    <option value="Inactive">
                      Không hoạt động
                    </option>
                  </select>
                </div>
              </div>
            </section>

            <!-- TÀI KHOẢN -->
            <section
              v-if="!isEditMode"
              class="form-section"
            >
              <h3>Tài khoản đăng nhập</h3>

              <div class="form-grid">
                <div class="form-group">
                  <label>Tên đăng nhập *</label>

                  <input
                    v-model="form.username"
                    type="text"
                    placeholder="doctor.username"
                    required
                  >
                </div>

                <div class="form-group">
                  <label>Mật khẩu *</label>

                  <input
                    v-model="form.password"
                    type="password"
                    placeholder="Nhập mật khẩu"
                    required
                  >
                </div>
              </div>
            </section>

            <!-- GIỚI THIỆU -->
            <section class="form-section">
              <h3>Giới thiệu</h3>

              <div class="form-group">
                <label>Tiểu sử</label>

                <textarea
                  v-model="form.biography"
                  rows="4"
                  placeholder="Nhập thông tin giới thiệu về bác sĩ..."
                />
              </div>
            </section>
          </div>

          <div class="modal-footer">
            <button
              type="button"
              class="btn-cancel"
              :disabled="loading"
              @click="handleClose"
            >
              Hủy
            </button>

            <button
              type="submit"
              class="btn-submit"
              :disabled="loading"
            >
              {{
                loading
                  ? 'Đang xử lý...'
                  : isEditMode
                    ? 'Lưu thay đổi'
                    : 'Thêm bác sĩ'
              }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  z-index: 1000;

  display: flex;
  align-items: center;
  justify-content: center;

  padding: 24px;

  background: rgb(15 23 42 / 50%);
}

.doctor-modal {
  width: min(900px, 100%);
  max-height: calc(100vh - 48px);

  overflow-y: auto;

  background: #ffffff;

  border-radius: 16px;

  box-shadow: 0 25px 60px rgb(15 23 42 / 25%);
}

.modal-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;

  padding: 24px 28px;

  border-bottom: 1px solid #e2e8f0;
}

.modal-header h2 {
  margin: 0;

  font-size: 22px;
  font-weight: 700;

  color: #0f172a;
}

.modal-header p {
  margin: 6px 0 0;

  font-size: 14px;

  color: #64748b;
}

.close-button {
  width: 36px;
  height: 36px;

  border: none;
  border-radius: 8px;

  background: #f1f5f9;

  font-size: 24px;

  color: #64748b;

  cursor: pointer;
}

.close-button:hover {
  background: #e2e8f0;
}

.modal-body {
  padding: 28px;
}

.form-section {
  margin-bottom: 32px;
}

.form-section:last-child {
  margin-bottom: 0;
}

.form-section h3 {
  margin: 0 0 18px;

  font-size: 16px;
  font-weight: 700;

  color: #1e293b;
}

.form-grid {
  display: grid;

  grid-template-columns: repeat(2, minmax(0, 1fr));

  gap: 18px;
}

.form-group {
  display: flex;

  flex-direction: column;

  gap: 7px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;

  color: #334155;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;

  box-sizing: border-box;

  padding: 11px 13px;

  border: 1px solid #cbd5e1;
  border-radius: 8px;

  outline: none;

  font-size: 14px;

  color: #1e293b;

  background: #ffffff;

  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

.form-group textarea {
  resize: vertical;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  border-color: #2563eb;

  box-shadow: 0 0 0 3px rgb(37 99 235 / 12%);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;

  gap: 12px;

  padding: 20px 28px;

  border-top: 1px solid #e2e8f0;
}

.btn-cancel,
.btn-submit {
  padding: 10px 20px;

  border-radius: 8px;

  font-size: 14px;
  font-weight: 600;

  cursor: pointer;
}

.btn-cancel {
  border: 1px solid #cbd5e1;

  background: #ffffff;

  color: #475569;
}

.btn-submit {
  border: none;

  background: #2563eb;

  color: #ffffff;
}

.btn-submit:hover:not(:disabled) {
  background: #1d4ed8;
}

.btn-cancel:hover:not(:disabled) {
  background: #f8fafc;
}

.btn-cancel:disabled,
.btn-submit:disabled,
.close-button:disabled {
  cursor: not-allowed;

  opacity: 0.6;
}

@media (max-width: 640px) {
  .modal-backdrop {
    padding: 12px;
  }

  .modal-body,
  .modal-header,
  .modal-footer {
    padding: 20px;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
