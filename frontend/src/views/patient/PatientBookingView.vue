<script setup lang="ts">
import {computed, onMounted, ref, watch} from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'
import { areProtectedPatientEndpointsDisabled, enableProtectedPatientEndpoints } from '@/features/patients/patient.api'
import type { Appointment } from '@/features/patients/patient.types'

const router = useRouter()
const patient = usePatientStore()
const auth = useAuthStore()

// Quản lý các bước trong quy trình đặt lịch: 1 - Chọn bác sĩ, 2 - Thông tin bệnh nhân, 3 - Xác nhận
const step = ref(1)

const specialty = ref('')
const doctorId = ref<number | string | null>(null)
const selectedSlotId = ref<number | string | null>(null)
const appointmentDate = ref('')
const appointmentTime = ref('')
const symptoms = ref('')

// Trạng thái đặt lịch thành công và thông tin cuộc hẹn đã tạo
const success = ref(false)
const createdAppointment = ref<Appointment | null>(null)

// Trạng thái chờ kết nối tới backend để tải dữ liệu bác sĩ và khung giờ
const tryingToConnect = ref(false)

// Lấy ngày hiện tại theo định dạng yyyy-mm-dd để giới hạn ngày đặt lịch
const todayDate = new Date().toLocaleDateString('sv-SE')

// Hàm tìm và đồng bộ hóa khung giờ đã chọn dựa trên thời gian cuộc hẹn
function syncSelectedSlotFromTime() {
  if (!appointmentTime.value) {
    selectedSlotId.value = null
    return
  }

  const matched = patient.slots.find((slot) => slot.time === appointmentTime.value)
  if (matched) {
    selectedSlotId.value = matched.workScheduleId ?? matched.id
  }
}

// Theo dõi sự thay đổi chuyên khoa để reset các lựa chọn liên quan đến bác sĩ và khung giờ
watch(specialty, () => {
  doctorId.value = null
  selectedSlotId.value = null
  appointmentTime.value = ''
  patient.clearSlots()
})

// Theo dõi sự thay đổi bác sĩ để reset các lựa chọn liên quan đến khung giờ
watch(() => appointmentTime.value, () => {
  syncSelectedSlotFromTime()
})

// Theo dõi sự thay đổi khung giờ để đồng bộ hóa với thời gian cuộc hẹn
watch(
  () => patient.slots,
  () => {
    syncSelectedSlotFromTime()
  },
  { deep: true },
)


const specialties = [
  'Nội tổng quát',
  'Tim mạch',
  'Nhi khoa',
  'Da liễu',
  'Xương khớp',
  'Tai mũi họng',
]

// Tính toán danh sách bác sĩ dựa trên chuyên khoa đã chọn
const filteredDoctors = computed(() => {
  if (!specialty.value) return patient.doctors
  return patient.doctors.filter((doctor) => doctor.specialty === specialty.value)
})

// Lấy thông tin bác sĩ đã chọn dựa trên doctorId
const selectedDoctor = computed(() =>
  patient.doctors.find((doctor) => doctor.id === doctorId.value)
)

// Hàm tải danh sách bác sĩ khi component được mounted
onMounted(async () => {
  await patient.loadDoctors()
})

// Hàm thử bật lại endpoint và tải dữ liệu từ backend
async function connectToBackend() {
  tryingToConnect.value = true
  try {
    enableProtectedPatientEndpoints()
    // re-load current data: doctors, and slots if a doctor & date already selected
    await patient.loadDoctors()
    if (doctorId.value && appointmentDate.value) {
      await patient.loadSlots(doctorId.value, appointmentDate.value)
    }
  } finally {
    tryingToConnect.value = false
  }
}

// Hàm xử lý sự kiện khi người dùng chọn bác sĩ
async function selectDoctor(id: number | string) {
  doctorId.value = id
  selectedSlotId.value = null
  appointmentTime.value = ''
  patient.clearSlots()

  if (appointmentDate.value) {
    await patient.loadSlots(id, appointmentDate.value)
  }
}

// Hàm xử lý sự kiện khi người dùng thay đổi ngày đặt lịch
async function changeDate() {
  selectedSlotId.value = null
  appointmentTime.value = ''
  patient.clearSlots()

  if (doctorId.value && appointmentDate.value) {
    await patient.loadSlots(doctorId.value, appointmentDate.value)
  }
}

// Hàm xử lý sự kiện khi người dùng chọn khung giờ
function chooseSlot(slot: { id: number | string; workScheduleId?: number | string; time: string }) {
  selectedSlotId.value = slot.workScheduleId ?? slot.id
  appointmentTime.value = slot.time
}

// Hàm chuyển sang bước tiếp theo trong quy trình đặt lịch
function nextStep() {
  if (step.value === 1) {
    if (!doctorId.value || !appointmentDate.value || !appointmentTime.value || !selectedSlotId.value) {
      return
    }
  }
  step.value++
}

// Hàm quay lại bước trước trong quy trình đặt lịch
function previousStep() {
  if (step.value > 1) {
    step.value--
  }
}

// Hàm xác nhận đặt lịch, tạo cuộc hẹn mới dựa trên thông tin đã nhập
async function confirmBooking() {
const resolvedSlotId = selectedSlotId.value ??
  patient.slots.find((slot) => slot.time === appointmentTime.value)?.workScheduleId ??
  patient.slots.find((slot) => slot.time === appointmentTime.value)?.id ??
  null

if (!doctorId.value || !resolvedSlotId) return

// Gọi API để tạo cuộc hẹn mới
try {
  createdAppointment.value = await patient.createAppointment({
    doctorId: doctorId.value,
    appointmentDate: appointmentDate.value,
    appointmentTime: appointmentTime.value,
    symptoms: symptoms.value,
  })
  success.value = true
} catch {
  // Store xử lý lỗi
}
}

//Hàm reset lại trạng thái đặt lịch để người dùng có thể đặt lịch mới
function newBooking() {
  step.value = 1
  specialty.value = ''
  doctorId.value = null
  selectedSlotId.value = null
  appointmentDate.value = ''
  appointmentTime.value = ''
  symptoms.value = ''
  success.value = false
  createdAppointment.value = null
  patient.clearSlots()
}
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-5">

    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Đặt lịch khám
      </h1>

      <p class="mt-1 text-sm text-slate-400">
        Đặt lịch nhanh, nhận xác nhận ngay
      </p>
    </div>

    <!-- Nếu protected endpoints bị tắt, hiển thị banner cho phép bật lại -->
    <div v-if="areProtectedPatientEndpointsDisabled()" class="rounded-2xl border border-amber-200 bg-amber-50 p-4 text-sm text-amber-800">
      Kết nối tới API đặt lịch hiện đang bị tắt để tránh lỗi. Nếu backend đã sẵn sàng, bạn có thể thử bật lại.
      <div class="mt-3">
        <button @click="connectToBackend" :disabled="tryingToConnect" class="px-4 py-2 rounded bg-[#0E4D92] text-white">{{ tryingToConnect ? 'Đang kết nối...' : 'Kết nối lại với backend' }}</button>
      </div>
    </div>

    <!-- SUCCESS -->
    <section
      v-if="success"
      class="rounded-2xl border border-slate-200
             bg-white p-8 text-center shadow-sm"
    >
      <div
        class="mx-auto mb-4 flex h-16 w-16 items-center
               justify-center rounded-full bg-emerald-100
               text-2xl text-emerald-600"
      >
        ✓
      </div>

      <h2 class="text-2xl font-bold text-slate-800">
        Đặt lịch thành công!
      </h2>

      <p class="mt-2 text-sm text-slate-500">
        Vui lòng đến trước giờ khám để làm thủ tục
        check-in tại quầy lễ tân.
      </p>

      <div
        class="mt-6 rounded-2xl bg-gradient-to-br
               from-[#0E4D92] to-[#1a6bbf]
               p-5 text-left text-white"
      >
        <div class="flex justify-between">
          <span class="text-sm text-blue-200">
            Số thứ tự
          </span>

          <span class="text-2xl font-bold">
            {{ createdAppointment?.queueNumber || '---' }}
          </span>
        </div>

        <div class="my-3 h-px bg-white/20" />

        <div class="space-y-2 text-sm">

          <div class="flex justify-between">
            <span class="text-blue-200">
              Bác sĩ
            </span>

            <span>
              {{ selectedDoctor?.name }}
            </span>
          </div>

          <div class="flex justify-between">
            <span class="text-blue-200">
              Chuyên khoa
            </span>

            <span>
              {{ selectedDoctor?.specialty }}
            </span>
          </div>

          <div class="flex justify-between">
            <span class="text-blue-200">
              Ngày
            </span>

            <span>
              {{ appointmentDate }}
            </span>
          </div>

          <div class="flex justify-between">
            <span class="text-blue-200">
              Giờ
            </span>

            <span>
              {{ appointmentTime }}
            </span>
          </div>

        </div>
      </div>

      <div class="mt-5 flex gap-3">

        <button
          class="flex-1 rounded-xl border-2
                 border-[#0E4D92] py-3 text-sm
                 font-semibold text-[#0E4D92]"
          @click="newBooking"
        >
          Đặt lịch mới
        </button>

        <button
          class="flex-1 rounded-xl bg-[#00A878]
                 py-3 text-sm font-semibold text-white"
          @click="router.push('/patient/queue')"
        >
          Xem hàng đợi
        </button>

      </div>
    </section>

    <!-- STEPPER -->
    <template v-else>

      <div class="flex items-center">

        <div
          v-for="item in [
            'Chọn bác sĩ',
            'Thông tin',
            'Xác nhận',
          ]"
          :key="item"
          class="flex flex-1 items-center"
        >
          <!-- có thể bổ sung stepper UI sau -->
        </div>

      </div>

      <!-- STEP 1 -->
      <section v-if="step === 1" class="space-y-4">

        <!-- Specialty -->
        <div
          class="rounded-2xl border border-slate-200
                 bg-white p-4"
        >
          <label
            class="mb-2 block text-sm font-medium"
          >
            Lọc theo chuyên khoa
          </label>

          <div class="flex flex-wrap gap-2">

            <button
              class="rounded-xl border px-3.5 py-1.5
                     text-xs font-medium"
              :class="
                specialty === ''
                  ? 'border-[#0E4D92] bg-[#0E4D92] text-white'
                  : 'border-slate-200 text-slate-600'
              "
              @click="specialty = ''"
            >
              Tất cả
            </button>

            <button
              v-for="sp in specialties"
              :key="sp"
              class="rounded-xl border px-3.5 py-1.5
                     text-xs font-medium"
              :class="
                specialty === sp
                  ? 'border-[#0E4D92] bg-[#0E4D92] text-white'
                  : 'border-slate-200 text-slate-600'
              "
              @click="specialty = sp"
            >
              {{ sp }}
            </button>

          </div>
        </div>

        <!-- Date -->
        <div
          class="rounded-2xl border border-slate-200
                 bg-white p-4"
        >
          <label class="mb-2 block text-sm font-medium">
            Ngày khám
          </label>

          <input
            v-model="appointmentDate"
            type="date"
            :min="todayDate"
            class="w-full rounded-xl border border-slate-200
                   px-4 py-2.5 text-sm"
            @change="changeDate"
          />
        </div>

        <!-- Doctors -->
        <div class="space-y-3">

          <div
            v-for="doctor in filteredDoctors"
            :key="doctor.id"
            class="cursor-pointer rounded-2xl border-2
                   bg-white p-5 transition"
            :class="
              doctorId === doctor.id
                ? 'border-[#0E4D92] shadow-md'
                : 'border-slate-200'
            "
            @click="selectDoctor(doctor.id)"
          >

            <div class="flex items-center gap-3">

              <div
                class="flex h-12 w-12 items-center
                       justify-center rounded-xl
                       bg-blue-50 font-bold
                       text-[#0E4D92]"
              >
                BS
              </div>

              <div class="flex-1">
                <p class="font-bold text-slate-800">
                  {{ doctor.name }}
                </p>

                <p class="text-xs text-slate-400">
                  {{ doctor.specialty }}
                  <span v-if="doctor.room">
                    · {{ doctor.room }}
                  </span>
                </p>
              </div>

              <span
                v-if="doctorId === doctor.id"
                class="text-[#0E4D92]"
              >
                ✓
              </span>

            </div>

            <!-- Slots -->
            <div
              v-if="doctorId === doctor.id"
              class="mt-4"
            >
              <p
                class="mb-2 text-xs font-semibold
                       text-slate-500"
              >
                Chọn khung giờ
              </p>

              <div class="flex flex-wrap gap-2">

                <button
                  v-for="slot in patient.slots"
                  :key="slot.id"
                  :disabled="!slot.available"
                  class="rounded-xl border px-3.5 py-1.5
                         text-xs font-medium"
                  :class="
                    !slot.available
                      ? 'cursor-not-allowed border-slate-100 bg-slate-50 text-slate-300'
                      : appointmentTime === slot.time
                        ? 'border-[#0E4D92] bg-[#0E4D92] text-white'
                        : 'border-slate-200 text-slate-700'
                  "
                  @click.stop="chooseSlot(slot)"
                >
                  {{ slot.time }}
                </button>

              </div>
            </div>

          </div>

        </div>

        <button
          class="w-full rounded-xl bg-[#0E4D92]
                 py-3 text-sm font-semibold text-white
                 disabled:opacity-40"
          :disabled="
            !doctorId ||
            !appointmentDate ||
            !appointmentTime ||
            !selectedSlotId
          "
          @click="nextStep"
        >
          Tiếp theo →
        </button>

      </section>

      <!-- STEP 2 -->
      <section v-if="step === 2" class="space-y-4">

        <div
          class="rounded-2xl border border-slate-200
                 bg-white p-5"
        >
          <h2 class="mb-4 font-semibold text-slate-700">
            Thông tin bệnh nhân
          </h2>

          <div class="space-y-4">

            <div>
              <label class="mb-1 block text-sm">
                Họ và tên
              </label>

              <input
                :value="auth.user?.name"
                disabled
                class="w-full rounded-xl border
                       border-slate-200 bg-slate-50
                       px-4 py-2.5 text-sm"
              />
            </div>

            <div>
              <label class="mb-1 block text-sm">
                Lý do khám / Triệu chứng
              </label>

              <textarea
                v-model="symptoms"
                rows="4"
                placeholder="Mô tả ngắn gọn triệu chứng..."
                class="w-full resize-none rounded-xl
                       border border-slate-200 px-4 py-2.5
                       text-sm"
              />
            </div>

          </div>
        </div>

        <div class="flex gap-3">

          <button
            class="rounded-xl border-2 border-[#0E4D92]
                   px-5 py-3 text-sm font-semibold
                   text-[#0E4D92]"
            @click="previousStep"
          >
            ← Quay lại
          </button>

          <button
            class="flex-1 rounded-xl bg-[#0E4D92]
                   py-3 text-sm font-semibold text-white"
            @click="nextStep"
          >
            Tiếp theo →
          </button>

        </div>

      </section>

      <!-- STEP 3 -->
      <section v-if="step === 3" class="space-y-4">

        <div
          class="rounded-2xl border border-slate-200
                 bg-white p-5"
        >
          <h2 class="mb-4 font-semibold text-slate-700">
            Xác nhận thông tin
          </h2>

          <div class="grid grid-cols-2 gap-3">

            <div class="rounded-xl bg-blue-50 p-3">
              <p class="text-xs text-blue-600">
                Bác sĩ
              </p>
              <p class="text-sm font-bold">
                {{ selectedDoctor?.name }}
              </p>
            </div>

            <div class="rounded-xl bg-blue-50 p-3">
              <p class="text-xs text-blue-600">
                Chuyên khoa
              </p>
              <p class="text-sm font-bold">
                {{ selectedDoctor?.specialty }}
              </p>
            </div>

            <div class="rounded-xl bg-blue-50 p-3">
              <p class="text-xs text-blue-600">
                Ngày
              </p>
              <p class="text-sm font-bold">
                {{ appointmentDate }}
              </p>
            </div>

            <div class="rounded-xl bg-blue-50 p-3">
              <p class="text-xs text-blue-600">
                Giờ
              </p>
              <p class="text-lg font-bold text-[#0E4D92]">
                {{ appointmentTime }}
              </p>
            </div>

          </div>

          <div
            v-if="symptoms"
            class="mt-4 border-t border-slate-100 pt-4"
          >
            <p class="text-xs text-slate-400">
              Lý do khám
            </p>

            <p class="mt-1 text-sm">
              {{ symptoms }}
            </p>
          </div>

        </div>

        <div class="flex gap-3">

          <button
            class="rounded-xl border-2 border-[#0E4D92]
                   px-5 py-3 text-sm font-semibold
                   text-[#0E4D92]"
            @click="previousStep"
          >
            ← Quay lại
          </button>

          <button
            class="flex-1 rounded-xl bg-[#00A878]
                   py-3 text-sm font-semibold text-white
                   disabled:opacity-50"
            :disabled="patient.appointmentsLoading"
            @click="confirmBooking"
          >
            {{
              patient.appointmentsLoading
                ? 'Đang xử lý...'
                : '✓ Xác nhận đặt lịch'
            }}
          </button>

        </div>

        <p
          v-if="patient.appointmentsError"
          class="rounded-xl bg-red-50 p-3 text-sm
                 text-red-600"
        >
          {{ patient.appointmentsError }}
        </p>

      </section>

    </template>

  </div>
</template>