<script setup lang="ts">
import {
  computed,
  onMounted,
  ref,
  reactive,
} from 'vue'

import { shiftsApi } from '@/features/admin/shifts/shifts.api'

import type {
  DoctorSchedule,
  Shift,
} from '@/features/admin/shifts/shifts.types'

const DAYS = [
  'Thứ 2',
  'Thứ 3',
  'Thứ 4',
  'Thứ 5',
  'Thứ 6',
  'Thứ 7',
  'Chủ nhật',
]

interface ScheduleSlot {
  shiftId: string
  date: string
  label: 'Sáng' | 'Chiều' | 'Cả ngày'
  startTime: string
  endTime: string
  patientLimit: number
  status: string
}

interface ScheduleRow {
  doctorId: string
  name: string
  slots: (ScheduleSlot | null)[]
}

const schedules = ref<DoctorSchedule[]>([])

const scheduleRows = ref<ScheduleRow[]>([])

const loading = ref(false)

const error = ref('')

const search = ref('')

const selectedSpecialty = ref('all')

const currentWeekStart = ref(getMonday(new Date()))

const showScheduleModal = ref(false)

const scheduleModalMode =
  ref<'create' | 'view' | 'edit'>('create')

const selectedDoctorId =
  ref<string | null>(null)

const selectedDoctorName =
  ref('')

const selectedShiftId =
  ref<string | null>(null)

    const scheduleForm = reactive({
  doctorId: '',
  date: '',
  startTime: '08:00',
  endTime: '12:00',
  patientLimit: 20,
})

const selectedWeek = computed(() => {
  const start = currentWeekStart.value
  const end = new Date(start)

  end.setDate(start.getDate() + 6)

  return `${formatDate(start)} - ${formatDate(end)}`
})

const filteredRows = computed(() => {
  const keyword = search.value
    .trim()
    .toLowerCase()

  return scheduleRows.value.filter((doctor) => {
    return (
      !keyword ||
      doctor.name
        .toLowerCase()
        .includes(keyword)
    )
  })
})

function formatDate(date: Date) {
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()

  return `${day}/${month}/${year}`
}

function formatApiDate(date: Date) {
  const year = date.getFullYear()

  const month = String(
    date.getMonth() + 1,
  ).padStart(2, '0')

  const day = String(
    date.getDate(),
  ).padStart(2, '0')

  return `${year}-${month}-${day}`
}

function getMonday(date: Date) {
  const current = new Date(date)

  const day = current.getDay()

  const diff =
    day === 0
      ? -6
      : 1 - day

  current.setDate(
    current.getDate() + diff,
  )

  current.setHours(0, 0, 0, 0)

  return current
}

function getSlotLabel(
  shift: Shift,
): ScheduleSlot['label'] {
  const startHour = Number(
    shift.startTime.split(':')[0],
  )

  const endHour = Number(
    shift.endTime.split(':')[0],
  )

  if (
    startHour < 12 &&
    endHour >= 17
  ) {
    return 'Cả ngày'
  }

  if (startHour < 12) {
    return 'Sáng'
  }

  return 'Chiều'
}

function createScheduleRows(
  data: DoctorSchedule[],
): ScheduleRow[] {
  const weekDates = Array.from(
    { length: 7 },
    (_, index) => {
      const date = new Date(
        currentWeekStart.value,
      )

      date.setDate(
        date.getDate() + index,
      )

      return formatApiDate(date)
    },
  )

  return data.map((doctor) => {
    const slots = weekDates.map(
      (date) => {
        const shift =
          doctor.schedules.find(
            item =>
              item.date === date &&
              item.status === 'Active',
          )

        if (!shift) {
          return null
        }

        return {
          shiftId: shift.id,
          date: shift.date,
          label: getSlotLabel(shift),
          startTime: shift.startTime,
          endTime: shift.endTime,
          patientLimit:
            shift.patientLimit,
          status: shift.status,
        }
      },
    )

    return {
      doctorId: doctor.doctorId,
      name: doctor.doctorName,
      slots,
    }
  })
}

async function fetchSchedules() {
  loading.value = true
  error.value = ''

  try {
    const startDate =
      formatApiDate(
        currentWeekStart.value,
      )

    const endDate = new Date(
      currentWeekStart.value,
    )

    endDate.setDate(
      endDate.getDate() + 6,
    )

    const data =
      await shiftsApi.getAll({
        StartDate: startDate,
        EndDate:
          formatApiDate(endDate),
      })

    console.log(
      'SCHEDULE DATA:',
      data,
    )

    console.log(
      'IS ARRAY:',
      Array.isArray(data),
    )

    schedules.value = data

    scheduleRows.value =
      createScheduleRows(data)
  }
  catch (err) {
    console.error(
      'Không thể tải lịch:',
      err,
    )

    error.value =
      'Không thể tải lịch làm việc'

    schedules.value = []

    scheduleRows.value = []
  }
  finally {
    loading.value = false
  }
}

function slotClass(
  slot: ScheduleSlot | null,
) {
  if (!slot) {
    return ''
  }

  if (slot.label === 'Cả ngày') {
    return 'bg-violet-50 text-violet-700 border-violet-100'
  }

  if (slot.label === 'Sáng') {
    return 'bg-blue-50 text-blue-700 border-blue-100'
  }

  return 'bg-amber-50 text-amber-700 border-amber-100'
}

function resetFilters() {
  search.value = ''
  selectedSpecialty.value = 'all'
}

async function previousWeek() {
  const previous = new Date(
    currentWeekStart.value,
  )

  previous.setDate(
    previous.getDate() - 7,
  )

  currentWeekStart.value = previous

  await fetchSchedules()
}

async function nextWeek() {
  const next = new Date(
    currentWeekStart.value,
  )

  next.setDate(
    next.getDate() + 7,
  )

  currentWeekStart.value = next

  await fetchSchedules()
}

function viewSchedule(
  slot: ScheduleSlot | null,
) {
  if (!slot) {
    return
  }

  console.log(
    'Xem shift:',
    slot,
  )
}
function openCreateModal() {
  scheduleModalMode.value = 'create'

  selectedDoctorId.value = null
  selectedDoctorName.value = ''
  selectedShiftId.value = null

  scheduleForm.doctorId = ''
  scheduleForm.date = ''
  scheduleForm.startTime = '08:00'
  scheduleForm.endTime = '12:00'
  scheduleForm.patientLimit = 20

  showScheduleModal.value = true
}
function viewDoctorSchedule(
  doctor: ScheduleRow,
) {
  scheduleModalMode.value = 'view'

  selectedDoctorId.value =
    doctor.doctorId

  selectedDoctorName.value =
    doctor.name

  showScheduleModal.value = true
}
function editDoctorSchedule(
  doctor: ScheduleRow,
) {
  const shift =
    doctor.slots.find(
      slot => slot !== null,
    )

  if (!shift) {
    alert(
      'Bác sĩ này chưa có lịch để sửa',
    )

    return
  }

  scheduleModalMode.value = 'edit'

  selectedDoctorId.value =
    doctor.doctorId

  selectedDoctorName.value =
    doctor.name

  selectedShiftId.value =
    shift.shiftId

  scheduleForm.doctorId =
    doctor.doctorId

  scheduleForm.date =
    shift.date

  scheduleForm.startTime =
    shift.startTime

  scheduleForm.endTime =
    shift.endTime

  scheduleForm.patientLimit =
    shift.patientLimit

  showScheduleModal.value = true
}
const submitting = ref(false)
async function submitSchedule() {
  try {
    submitting.value = true

    if (
      scheduleModalMode.value ===
      'create'
    ) {
      await shiftsApi.create(
        scheduleForm.doctorId,
        {
          date: scheduleForm.date,
          startTime:
            scheduleForm.startTime,
          endTime:
            scheduleForm.endTime,
          patientLimit:
            Number(
              scheduleForm.patientLimit,
            ),
        },
      )
    }

    if (
      scheduleModalMode.value ===
        'edit' &&
      selectedShiftId.value
    ) {
      await shiftsApi.update(
        selectedShiftId.value,
        {
          date: scheduleForm.date,
          startTime:
            scheduleForm.startTime,
          endTime:
            scheduleForm.endTime,
          patientLimitPerSlot:
            Number(
              scheduleForm.patientLimit,
            ),
        },
      )
    }

    showScheduleModal.value = false

    await fetchSchedules()
  }
  catch (error) {
    console.error(
      'Không thể lưu lịch:',
      error,
    )

    alert(
      'Không thể lưu lịch làm việc',
    )
  }
  finally {
    submitting.value = false
  }
}

onMounted(() => {
  fetchSchedules()
})
</script>

<template>
  <div class="space-y-5">
    <!-- TITLE -->
    <div>
      <h1
        class="text-xl font-semibold text-slate-800"
      >
        Lịch làm việc
      </h1>

      <p
        class="mt-1 text-sm text-slate-500"
      >
        Quản lý lịch làm việc của bác sĩ
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
            placeholder="Tìm kiếm bác sĩ..."
            class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 pl-10 text-sm text-slate-800 placeholder:text-slate-400 focus:border-violet-500 focus:outline-none"
          >

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

            <path
              d="m20 20-3.5-3.5"
            />
          </svg>
        </div>

        <!-- RESET -->
        <button
          class="rounded-lg border border-slate-200 px-4 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          @click="resetFilters"
        >
          Đặt lại
        </button>

        <!-- ADD -->
        <button
          type="button"
          class="rounded-lg bg-violet-600 px-5 py-2.5 font-medium text-white hover:bg-violet-700"
          @click="openCreateModal"
        >
          + Thêm lịch
        </button>
      </div>
    </div>

    <!-- WEEK NAVIGATION -->
    <div
      class="flex items-center justify-between rounded-xl border border-slate-200 bg-white px-5 py-4"
    >
      <button
        class="rounded-lg border border-slate-200 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        @click="previousWeek"
      >
        ← Tuần trước
      </button>

      <div class="text-center">
        <div
          class="text-sm font-semibold text-slate-800"
        >
          Tuần làm việc
        </div>

        <div
          class="mt-1 text-xs text-slate-500"
        >
          {{ selectedWeek }}
        </div>
      </div>

      <button
        class="rounded-lg border border-slate-200 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        @click="nextWeek"
      >
        Tuần sau →
      </button>
    </div>

    <!-- ERROR -->
    <div
      v-if="error"
      class="rounded-xl border border-red-200 bg-red-50 px-5 py-4 text-sm text-red-600"
    >
      {{ error }}
    </div>

    <!-- SCHEDULE TABLE -->
    <div
      class="overflow-hidden rounded-xl border border-slate-200 bg-white"
    >
      <div
        class="border-b border-slate-200 p-5"
      >
        <h2
          class="text-sm font-semibold text-slate-800"
        >
          Lịch làm việc bác sĩ
        </h2>

        <p
          class="mt-1 text-xs text-slate-500"
        >
          <span v-if="loading">
            Đang tải...
          </span>

          <span v-else>
            {{ filteredRows.length }} bác sĩ
          </span>
        </p>
      </div>

      <div class="overflow-x-auto">
        <table
          class="w-full min-w-[1000px] text-sm"
        >
          <thead>
            <tr
              class="border-b border-slate-200 bg-slate-50"
            >
              <th
                class="w-64 px-5 py-4 text-left text-xs font-semibold uppercase tracking-wide text-slate-600"
              >
                Bác sĩ
              </th>

              <th
                v-for="day in DAYS"
                :key="day"
                class="px-3 py-4 text-center text-xs font-semibold text-slate-600"
              >
                {{ day }}
              </th>

              <th
                class="px-4 py-4 text-right text-xs font-semibold uppercase tracking-wide text-slate-600"
              >
                Thao tác
              </th>
            </tr>
          </thead>

          <tbody
            class="divide-y divide-slate-100"
          >
            <tr
              v-for="doctor in filteredRows"
              :key="doctor.doctorId"
              class="hover:bg-slate-50"
            >
              <!-- DOCTOR -->
              <td class="px-5 py-4">
                <div
                  class="font-medium text-slate-800"
                >
                  {{ doctor.name }}
                </div>
              </td>

              <!-- SLOTS -->
              <td
                v-for="(slot, index) in doctor.slots"
                :key="`${doctor.doctorId}-${index}`"
                class="px-2 py-3 text-center"
              >
                <button
                  v-if="slot"
                  class="mx-auto rounded-md border px-2 py-2 text-xs font-medium"
                  :class="slotClass(slot)"
                  @click="viewSchedule(slot)"
                >
                  {{ slot.label }}
                </button>

                <span
                  v-else
                  class="text-slate-400"
                >
                  —
                </span>
              </td>

              <!-- ACTIONS -->
              <td class="px-4 py-5">
                <div class="flex items-center justify-end gap-6">
                  <button
                    type="button"
                    class="text-sm font-medium text-slate-700 hover:text-violet-600"
                    @click="viewDoctorSchedule(doctor)"
                  >
                    Xem
                  </button>

                  <button
                    type="button"
                    class="text-sm font-medium text-violet-600 hover:text-violet-800"
                    @click="editDoctorSchedule(doctor)"
                  >
                    Sửa
                  </button>
                </div>
              </td>
            </tr>

            <!-- EMPTY -->
            <tr
              v-if="
                !loading &&
                filteredRows.length === 0
              "
            >
              <td
                :colspan="DAYS.length + 2"
                class="px-5 py-12 text-center text-sm text-slate-500"
              >
                Không tìm thấy lịch làm việc phù hợp.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- LEGEND -->
    <div
      class="flex flex-wrap items-center gap-5 rounded-xl border border-slate-200 bg-white px-5 py-4"
    >
      <span
        class="text-xs font-semibold text-slate-700"
      >
        Chú thích:
      </span>

      <div class="flex items-center gap-2">
        <span
          class="h-3 w-3 rounded bg-blue-100"
        />

        <span
          class="text-xs text-slate-600"
        >
          Sáng
        </span>
      </div>

      <div class="flex items-center gap-2">
        <span
          class="h-3 w-3 rounded bg-amber-100"
        />

        <span
          class="text-xs text-slate-600"
        >
          Chiều
        </span>
      </div>

      <div class="flex items-center gap-2">
        <span
          class="h-3 w-3 rounded bg-violet-100"
        />

        <span
          class="text-xs text-slate-600"
        >
          Cả ngày
        </span>
      </div>
    </div>

    <!-- SCHEDULE MODAL -->
    <div
      v-if="showScheduleModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
    >
      <div
        class="w-full max-w-lg rounded-xl bg-white shadow-xl"
      >
        <!-- HEADER -->
        <div
          class="flex items-center justify-between border-b border-slate-200 px-6 py-4"
        >
          <h2
            class="text-lg font-semibold text-slate-800"
          >
            {{
              scheduleModalMode === 'create'
                ? 'Thêm lịch làm việc'
                : scheduleModalMode === 'edit'
                  ? 'Sửa lịch làm việc'
                  : 'Xem lịch làm việc'
            }}
          </h2>

          <button
            type="button"
            class="text-xl font-medium text-slate-600 hover:text-slate-900"
            @click="showScheduleModal = false"
          >
            ×
          </button>
        </div>

        <!-- BODY -->
        <div class="space-y-4 p-6">
          <!-- DOCTOR -->
<div>
  <label
    class="mb-1 block text-sm font-medium text-slate-800"
  >
    Doctor
  </label>

  <select
    v-if="scheduleModalMode === 'create'"
    v-model="scheduleForm.doctorId"
    class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 focus:border-violet-500 focus:outline-none"
  >
    <option value="">
      Select a doctor
    </option>

    <option
      v-for="doctor in scheduleRows"
      :key="doctor.doctorId"
      :value="doctor.doctorId"
    >
      {{ doctor.name }}
    </option>
  </select>

  <input
    v-else
    :value="selectedDoctorName"
    type="text"
    disabled
    class="w-full rounded-lg border border-slate-300 bg-slate-100 px-3 py-2.5 text-sm text-slate-800 disabled:opacity-100"
  >
</div>

          <!-- DATE -->
          <div>
            <!-- DATE -->
<label class="mb-1 block text-sm font-medium text-slate-800">
  Working Date
</label>

            <input
              v-model="scheduleForm.date"
              type="date"
              :disabled="scheduleModalMode === 'view'"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 focus:border-violet-500 focus:outline-none disabled:bg-slate-100 disabled:text-slate-800 disabled:opacity-100"
            >
          </div>

          <!-- START TIME -->
          <div>
            <label
              class="mb-1 block text-sm font-medium text-slate-800"
            >
              Start Time
            </label>

            <input
              v-model="scheduleForm.startTime"
              type="time"
              :disabled="scheduleModalMode === 'view'"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 focus:border-violet-500 focus:outline-none disabled:bg-slate-100 disabled:text-slate-800 disabled:opacity-100"
            >
          </div>

          <!-- END TIME -->
          <div>
            <label
              class="mb-1 block text-sm font-medium text-slate-800"
            >
              End Time
            </label>

            <input
              v-model="scheduleForm.endTime"
              type="time"
              :disabled="scheduleModalMode === 'view'"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 focus:border-violet-500 focus:outline-none disabled:bg-slate-100 disabled:text-slate-800 disabled:opacity-100"
            >
          </div>

          <!-- PATIENT LIMIT -->
          <div>
            <label
              class="mb-1 block text-sm font-medium text-slate-800"
            >
              Patient Limit
            </label>

            <input
              v-model.number="scheduleForm.patientLimit"
              type="number"
              min="1"
              :disabled="scheduleModalMode === 'view'"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 focus:border-violet-500 focus:outline-none disabled:bg-slate-100 disabled:text-slate-800 disabled:opacity-100"
            >
          </div>
        </div>

        <!-- FOOTER -->
        <div
          class="flex justify-end gap-3 border-t border-slate-200 px-6 py-4"
        >
          <button
            type="button"
            class="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
            @click="showScheduleModal = false"
          >
            Đóng
          </button>

          <button
            v-if="scheduleModalMode !== 'view'"
            type="button"
            :disabled="submitting"
            class="rounded-lg bg-violet-600 px-4 py-2 text-sm font-medium text-white hover:bg-violet-700 disabled:cursor-not-allowed disabled:opacity-60"
            @click="submitSchedule"
          >
            {{
              submitting
                ? 'Đang lưu...'
                : scheduleModalMode === 'create'
                  ? 'Thêm lịch'
                  : 'Lưu thay đổi'
            }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
