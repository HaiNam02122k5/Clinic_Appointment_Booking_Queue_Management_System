<script setup lang="ts">
import { ref } from 'vue'

import CheckInResult from '@/features/receptionist/components/CheckInResult.vue'
import { http } from '@/lib/api/http'

type PatientSearchItem = {
  id: string
  fullName: string
  phoneNumber?: string | null
  email?: string | null
  insuranceNumber?: string | null
}

type AppointmentItem = {
  id: string
  patientId: string
  doctorId: string
  patientName: string
  doctorName: string
  date?: string | null
  timeSlot?: string | { hours?: number; minutes?: number; seconds?: number } | null
  reason?: string | null
  status?: string | number | null
}

type QueueItem = {
  appointmentId?: string | null
  id?: string | null
  queueNumber?: number | string | null
}

/**
 * Lấy dữ liệu thực tế từ response API.
 *
 * API có thể trả về:
 * {
 *   result: ...
 * }
 *
 * hoặc:
 * {
 *   data: ...
 * }
 *
 * hoặc:
 * {
 *   items: ...
 * }
 */
function unwrapApiResult<T>(payload: unknown): T | null {
  if (!payload || typeof payload !== 'object') {
    return payload as T | null
  }

  const maybeEnvelope = payload as {
    result?: T
    data?: T
    items?: T
  }

  if (maybeEnvelope.result !== undefined) {
    return maybeEnvelope.result
  }

  if (maybeEnvelope.data !== undefined) {
    return maybeEnvelope.data
  }

  if (maybeEnvelope.items !== undefined) {
    return maybeEnvelope.items
  }

  return payload as T
}

/**
 * Lấy message lỗi từ response của Axios/API.
 */
function extractErrorMessage(error: unknown): string {
  try {
    const resp = (error as any)?.response

    if (resp && resp.data) {
      const data = resp.data

      if (
        Array.isArray(data.errorMessages) &&
        data.errorMessages.length
      ) {
        return String(data.errorMessages.join(', '))
      }

      if (
        Array.isArray(data.errors) &&
        data.errors.length
      ) {
        return String(data.errors.join(', '))
      }

      if (
        typeof data.message === 'string' &&
        data.message
      ) {
        return data.message
      }

      if (
        data.result &&
        Array.isArray((data.result as any).errorMessages) &&
        (data.result as any).errorMessages.length
      ) {
        return String(
          (data.result as any).errorMessages.join(', ')
        )
      }

      if (
        typeof data === 'string' &&
        data
      ) {
        return data
      }
    }

    if ((error as any)?.message) {
      return (error as any).message
    }
  } catch {
    // Bỏ qua lỗi khi đọc error
  }

  return 'Đã xảy ra lỗi. Vui lòng thử lại.'
}

/* =========================
   STATE
========================= */

const searchValue = ref('')

const searchResults = ref<PatientSearchItem[]>([])

const selectedPatient = ref<PatientSearchItem | null>(null)

const upcomingAppointments = ref<AppointmentItem[]>([])

const selectedAppointmentId = ref<string | null>(null)

const errorMessage = ref('')

const successMessage = ref('')

const queueNumber = ref<string | null>(null)

const isLoading = ref(false)

/**
 * Loading riêng cho từng dòng lịch hẹn.
 */
const rowLoading = ref<Record<string, boolean>>({})

/* =========================
   FORMAT
========================= */

function formatDate(value?: string | null): string {
  if (!value) {
    return '—'
  }

  const normalized = String(value).trim()

  if (!normalized) {
    return '—'
  }

  const parsed = new Date(
    normalized.includes('T')
      ? normalized
      : `${normalized}T00:00:00`,
  )

  if (Number.isNaN(parsed.getTime())) {
    return normalized
  }

  return parsed.toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

function formatTime(value: unknown): string {
  if (!value) {
    return '—'
  }

  if (typeof value === 'string') {
    const text = value.trim()

    if (!text) {
      return '—'
    }

    return text.includes(':')
      ? text.slice(0, 5)
      : text
  }

  if (
    typeof value === 'object' &&
    value &&
    'hours' in value
  ) {
    const nextValue = value as {
      hours?: number
      minutes?: number
      seconds?: number
    }

    const hours = String(
      nextValue.hours ?? 0,
    ).padStart(2, '0')

    const minutes = String(
      nextValue.minutes ?? 0,
    ).padStart(2, '0')

    return `${hours}:${minutes}`
  }

  return String(value)
}

function patientDisplayName(
  patient: PatientSearchItem,
): string {
  return patient.fullName || 'Bệnh nhân'
}

function formatQueueNumber(
  value: number | string | null | undefined,
): string | null {
  if (
    value === null ||
    value === undefined ||
    value === ''
  ) {
    return null
  }

  const number = Number(value)

  if (Number.isNaN(number)) {
    return null
  }

  return `A-${String(number).padStart(3, '0')}`
}

function appointmentStatusLabel(
  status: unknown,
): string {
  if (
    status === null ||
    status === undefined ||
    status === ''
  ) {
    return 'Chưa rõ'
  }

  const normalized = String(status)
    .trim()
    .toLowerCase()
    .replace(/[_\s-]+/g, '')

  if (['pending', '0'].includes(normalized)) {
    return 'Chờ xác nhận'
  }

  if (['confirmed', '1'].includes(normalized)) {
    return 'Đã xác nhận'
  }

  if (['checkedin', '2'].includes(normalized)) {
    return 'Đã check-in'
  }

  if (['completed', '3'].includes(normalized)) {
    return 'Hoàn thành'
  }

  if (
    ['cancelled', 'canceled', '4'].includes(normalized)
  ) {
    return 'Đã hủy'
  }

  if (['noshow', '5'].includes(normalized)) {
    return 'Không đến'
  }

  return String(status)
}

/* =========================
   TÌM BỆNH NHÂN
========================= */

async function searchPatient() {
  const keyword = searchValue.value.trim()

  errorMessage.value = ''
  successMessage.value = ''
  queueNumber.value = null

  searchResults.value = []

  /**
   * Khi tìm kiếm lại thì bỏ bệnh nhân đang chọn.
   * Vì vậy lịch hẹn cũng sẽ biến mất.
   */
  selectedPatient.value = null
  upcomingAppointments.value = []
  selectedAppointmentId.value = null

  if (!keyword) {
    errorMessage.value =
      'Vui lòng nhập tên, email, số điện thoại hoặc số bảo hiểm của bệnh nhân.'
    return
  }

  isLoading.value = true

  try {
    const { data } = await http.get('/patients', {
      params: {
        search: keyword,
        pageNumber: 1,
        pageSize: 10,
        sortBy: 'fullName',
        orderBy: 'asc',
      },
    })

    const payload = unwrapApiResult<
      { items?: PatientSearchItem[] }
      | PatientSearchItem[]
      | null
    >(data)

    const patients = Array.isArray(payload)
      ? payload
      : Array.isArray(payload?.items)
        ? payload.items
        : []

    if (!patients.length) {
      errorMessage.value =
        'Không tìm thấy bệnh nhân phù hợp.'
      return
    }

    searchResults.value = patients.map((patient) => ({
      id: String(patient.id),
      fullName: patient.fullName ?? 'Bệnh nhân',
      phoneNumber: patient.phoneNumber ?? '',
      email: patient.email ?? '',
      insuranceNumber:
        patient.insuranceNumber ?? '',
    }))
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    if (status === 401 || status === 403) {
      errorMessage.value =
        'Bạn không có quyền tìm bệnh nhân.'
      return
    }

    errorMessage.value =
      'Không thể tìm bệnh nhân. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}

/* =========================
   LOAD LỊCH HẸN
========================= */

async function loadPatientAppointments(
  patientId: string,
) {
  isLoading.value = true

  try {
    const { data } = await http.get(
      `/patients/${patientId}/appointments`,
    )

    const payload = unwrapApiResult<
      AppointmentItem[] | null
    >(data)

    const appointments = Array.isArray(payload)
      ? payload
      : []

    upcomingAppointments.value = appointments
      .filter(
        (appointment) =>
          appointment &&
          appointment.id,
      )
      .sort((a, b) => {
        const dateA = new Date(
          `${a.date ?? '2000-01-01'}T00:00:00`,
        ).getTime()

        const dateB = new Date(
          `${b.date ?? '2000-01-01'}T00:00:00`,
        ).getTime()

        return dateA - dateB
      })

    /**
     * Không set errorMessage ở đây.
     *
     * Vì "không có lịch hẹn" không phải lỗi API.
     * Template sẽ tự hiển thị thông báo
     * "Không có lịch hẹn sắp tới".
     */
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    if (status === 401 || status === 403) {
      errorMessage.value =
        'Bạn không có quyền xem lịch hẹn của bệnh nhân này.'
      return
    }

    if (status === 404) {
      errorMessage.value =
        'Không tìm thấy bệnh nhân hoặc lịch hẹn của bệnh nhân.'
      return
    }

    errorMessage.value =
      'Không thể tải lịch hẹn của bệnh nhân.'
  } finally {
    isLoading.value = false
  }
}

/* =========================
   CHỌN BỆNH NHÂN
========================= */

/**
 * QUAN TRỌNG:
 *
 * Chỉ khi click vào ô bệnh nhân
 * thì hàm này mới được gọi.
 *
 * Sau đó mới gọi API:
 * /patients/{patientId}/appointments
 */
function selectPatient(
  patient: PatientSearchItem,
) {
  selectedPatient.value = patient

  selectedAppointmentId.value = null

  queueNumber.value = null

  successMessage.value = ''

  errorMessage.value = ''

  /**
   * Chỉ ở đây mới load lịch hẹn.
   */
  void loadPatientAppointments(patient.id)
}

/* =========================
   CHỌN LỊCH HẸN
========================= */

function selectAppointment(
  appointmentId: string,
) {
  selectedAppointmentId.value =
    appointmentId

  errorMessage.value = ''

  successMessage.value = ''

  queueNumber.value = null
}

/* =========================
   CONFIRM APPOINTMENT
========================= */

async function confirmAppointmentPerRow(
  id: string,
) {
  errorMessage.value = ''
  successMessage.value = ''

  const appointment =
    upcomingAppointments.value.find(
      (a) => a.id === id,
    )

  if (!appointment) {
    errorMessage.value =
      'Không tìm thấy cuộc hẹn để xác nhận.'
    return
  }

  if (
    !appointment.doctorId ||
    !appointment.date ||
    !appointment.patientId ||
    !appointment.patientName ||
    !appointment.doctorName
  ) {
    errorMessage.value =
      'Cuộc hẹn thiếu dữ liệu cần thiết (bác sĩ/ngày/bệnh nhân tên). Vui lòng chỉnh sửa cuộc hẹn trước khi xác nhận.'
    return
  }

  const statusText = String(
    appointment.status ?? '',
  )
    .trim()
    .toLowerCase()

  const isPending =
    statusText === 'pending' ||
    statusText === '0'

  if (!isPending) {
    errorMessage.value =
      'Chỉ có cuộc hẹn ở trạng thái Chờ xác nhận mới có thể xác nhận.'
    return
  }

  rowLoading.value[id] = true
  isLoading.value = true

  try {
    await http.post(
      `/appointments/${id}/confirm`,
    )

    successMessage.value =
      'Đã xác nhận cuộc hẹn.'

    if (selectedPatient.value) {
      await loadPatientAppointments(
        selectedPatient.value.id,
      )
    }
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    const msg = extractErrorMessage(error)

    if (status === 400) {
      errorMessage.value =
        msg ||
        'Cuộc hẹn không hợp lệ để xác nhận.'
    } else if (status === 404) {
      errorMessage.value =
        msg || 'Không tìm thấy cuộc hẹn.'
    } else if (
      status === 401 ||
      status === 403
    ) {
      errorMessage.value =
        msg ||
        'Bạn không có quyền xác nhận cuộc hẹn.'
    } else if (status === 409) {
      if (selectedPatient.value) {
        await loadPatientAppointments(
          selectedPatient.value.id,
        )
      }

      errorMessage.value =
        msg ||
        'Xung đột: cuộc hẹn không thể được xác nhận.'
    } else {
      errorMessage.value =
        msg ||
        'Xác nhận thất bại. Vui lòng thử lại.'
    }
  } finally {
    rowLoading.value[id] = false
    isLoading.value = false
  }
}

/* =========================
   CHECK-IN
========================= */

async function checkInPerRow(
  id: string,
) {
  errorMessage.value = ''
  successMessage.value = ''

  const appointment =
    upcomingAppointments.value.find(
      (a) => a.id === id,
    )

  if (!appointment) {
    errorMessage.value =
      'Không tìm thấy cuộc hẹn để check-in.'
    return
  }

  const statusText = String(
    appointment.status ?? '',
  )
    .trim()
    .toLowerCase()

  const isConfirmed =
    statusText === 'confirmed' ||
    statusText === '1'

  const isCheckedIn =
    statusText === 'checkedin' ||
    statusText === '2'

  if (!isConfirmed) {
    errorMessage.value =
      'Chỉ có cuộc hẹn đã được xác nhận mới có thể check-in.'
    return
  }

  if (isCheckedIn) {
    errorMessage.value =
      'Cuộc hẹn đã được check-in.'
    return
  }

  if (appointment.date) {
    const apptDate = new Date(
      appointment.date,
    )

    const today = new Date()

    if (
      apptDate.getFullYear() !==
        today.getFullYear() ||
      apptDate.getMonth() !==
        today.getMonth() ||
      apptDate.getDate() !==
        today.getDate()
    ) {
      errorMessage.value =
        'Không thể check-in vì ngày cuộc hẹn không phải hôm nay.'
      return
    }
  }

  if (!appointment.doctorId) {
    errorMessage.value =
      'Cuộc hẹn chưa gán bác sĩ, không thể check-in.'
    return
  }

  rowLoading.value[id] = true
  isLoading.value = true

  try {
    await http.post(
      `/appointments/${id}/check-in`,
    )

    if (appointment.doctorId) {
      try {
        const { data } =
          await http.get(
            `/doctors/${appointment.doctorId}/queue`,
          )

        const payload =
          unwrapApiResult<
            | QueueItem[]
            | {
                items?: QueueItem[]
                data?: QueueItem[]
                result?: QueueItem[]
              }
            | null
          >(data)

        const queueList = Array.isArray(payload)
          ? payload
          : Array.isArray(payload?.items)
            ? payload.items
            : Array.isArray(payload?.data)
              ? payload.data
              : Array.isArray(payload?.result)
                ? payload.result
                : []

        const matchedQueue =
          queueList.find(
            (item) =>
              String(
                item.appointmentId ??
                  item.id,
              ) === String(id),
          )

        queueNumber.value =
          formatQueueNumber(
            matchedQueue?.queueNumber ??
              null,
          )
      } catch {
        queueNumber.value = 'A-001'
      }
    }

    successMessage.value =
      'Check-in bệnh nhân thành công.'

    if (selectedPatient.value) {
      await loadPatientAppointments(
        selectedPatient.value.id,
      )
    }
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    const msg = extractErrorMessage(error)

    if (status === 400) {
      errorMessage.value =
        msg ||
        'Lịch hẹn này không thể check-in ở thời điểm hiện tại.'
    } else if (status === 404) {
      errorMessage.value =
        msg ||
        'Không tìm thấy lịch hẹn để check-in.'
    } else if (
      status === 401 ||
      status === 403
    ) {
      errorMessage.value =
        msg ||
        'Bạn không có quyền check-in bệnh nhân.'
    } else if (status === 409) {
      if (selectedPatient.value) {
        await loadPatientAppointments(
          selectedPatient.value.id,
        )
      }

      errorMessage.value =
        msg ||
        'Xung đột: hành động check-in không thể thực hiện.'
    } else {
      errorMessage.value =
        msg ||
        'Không thể check-in bệnh nhân này.'
    }
  } finally {
    rowLoading.value[id] = false
    isLoading.value = false
  }
}

/* =========================
   ACTION NÚT BÊN DƯỚI
========================= */

async function performAppointmentAction(
  appointmentId?: string | null,
) {
  if (!appointmentId) {
    errorMessage.value =
      'Vui lòng chọn một cuộc hẹn để thực hiện hành động.'
    return
  }

  const appointment =
    upcomingAppointments.value.find(
      (item) => item.id === appointmentId,
    )

  if (!appointment) {
    errorMessage.value =
      'Không tìm thấy cuộc hẹn đã chọn.'
    return
  }

  const statusText = String(
    appointment.status ?? '',
  )
    .trim()
    .toLowerCase()

  const isPending =
    statusText === 'pending' ||
    statusText === '0'

  const isConfirmed =
    statusText === 'confirmed' ||
    statusText === '1'

  errorMessage.value = ''
  successMessage.value = ''
  isLoading.value = true

  try {
    if (isPending) {
      await http.post(
        `/appointments/${appointment.id}/confirm`,
      )

      appointment.status = 'confirmed'

      successMessage.value =
        'Đã xác nhận cuộc hẹn.'

      return
    }

    if (isConfirmed) {
      await http.post(
        `/appointments/${appointment.id}/check-in`,
      )

      if (appointment.doctorId) {
        try {
          const { data } =
            await http.get(
              `/doctors/${appointment.doctorId}/queue`,
            )

          const payload =
            unwrapApiResult<
              | QueueItem[]
              | {
                  items?: QueueItem[]
                  data?: QueueItem[]
                  result?: QueueItem[]
                }
              | null
            >(data)

          const queueList =
            Array.isArray(payload)
              ? payload
              : Array.isArray(payload?.items)
                ? payload.items
                : Array.isArray(payload?.data)
                  ? payload.data
                  : Array.isArray(payload?.result)
                    ? payload.result
                    : []

          const matchedQueue =
            queueList.find(
              (item) =>
                String(
                  item.appointmentId ??
                    item.id,
                ) ===
                String(appointment.id),
            )

          queueNumber.value =
            formatQueueNumber(
              matchedQueue?.queueNumber ??
                null,
            )
        } catch {
          queueNumber.value = 'A-001'
        }
      }

      appointment.status = 'checkedin'

      successMessage.value =
        'Check-in bệnh nhân thành công.'

      return
    }

    errorMessage.value =
      'Cuộc hẹn hiện không thể thực hiện hành động.'
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    const msg = extractErrorMessage(error)

    if (status === 400) {
      errorMessage.value =
        msg ||
        'Hành động không hợp lệ cho cuộc hẹn này.'
      return
    }

    if (status === 404) {
      errorMessage.value =
        msg ||
        'Không tìm thấy cuộc hẹn.'
      return
    }

    if (
      status === 401 ||
      status === 403
    ) {
      errorMessage.value =
        msg ||
        'Bạn không có quyền thực hiện hành động này.'
      return
    }

    if (status === 409) {
      errorMessage.value =
        msg ||
        'Xung đột khi thực hiện hành động.'
      return
    }

    errorMessage.value =
      msg ||
      'Đã xảy ra lỗi khi thực hiện hành động. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}

/* =========================
   CHECK-IN CŨ
========================= */

async function checkIn() {
  if (!selectedAppointmentId.value) {
    errorMessage.value =
      'Vui lòng chọn một cuộc hẹn để check-in.'
    return
  }

  const appointment =
    upcomingAppointments.value.find(
      (item) =>
        item.id ===
        selectedAppointmentId.value,
    )

  if (!appointment) {
    errorMessage.value =
      'Không tìm thấy cuộc hẹn đã chọn.'
    return
  }

  isLoading.value = true

  try {
    await http.post(
      `/appointments/${appointment.id}/check-in`,
    )

    if (appointment.doctorId) {
      try {
        const { data } =
          await http.get(
            `/doctors/${appointment.doctorId}/queue`,
          )

        const payload =
          unwrapApiResult<
            | QueueItem[]
            | {
                items?: QueueItem[]
                data?: QueueItem[]
                result?: QueueItem[]
              }
            | null
          >(data)

        const queueList =
          Array.isArray(payload)
            ? payload
            : Array.isArray(payload?.items)
              ? payload.items
              : Array.isArray(payload?.data)
                ? payload.data
                : Array.isArray(payload?.result)
                  ? payload.result
                  : []

        const matchedQueue =
          queueList.find(
            (item) =>
              String(
                item.appointmentId ??
                  item.id,
              ) ===
              String(appointment.id),
          )

        queueNumber.value =
          formatQueueNumber(
            matchedQueue?.queueNumber ??
              null,
          )
      } catch {
        queueNumber.value = 'A-001'
      }
    }

    successMessage.value =
      'Check-in bệnh nhân thành công.'

    errorMessage.value = ''
  } catch (error: unknown) {
    const status =
      typeof error === 'object' &&
      error !== null &&
      'response' in error
        ? Number(
            (
              error as {
                response?: {
                  status?: number
                }
              }
            ).response?.status,
          )
        : undefined

    if (status === 400) {
      errorMessage.value =
        'Lịch hẹn này không thể check-in ở thời điểm hiện tại.'
      return
    }

    if (status === 404) {
      errorMessage.value =
        'Không tìm thấy lịch hẹn để check-in.'
      return
    }

    if (
      status === 401 ||
      status === 403
    ) {
      errorMessage.value =
        'Bạn không có quyền check-in bệnh nhân.'
      return
    }

    errorMessage.value =
      'Không thể check-in bệnh nhân này.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="max-w-2xl space-y-5">
    <!-- ==================== TIÊU ĐỀ ==================== -->
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Check-in bệnh nhân
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Tìm bệnh nhân theo tên, email, số điện thoại hoặc số bảo hiểm để check-in.
      </p>
    </div>

    <!-- ==================== TÌM BỆNH NHÂN ==================== -->
    <div class="rounded-xl border border-slate-200 bg-white p-6">
      <h2 class="mb-4 text-sm font-semibold text-slate-800">
        Tìm bệnh nhân
      </h2>

      <label class="mb-1.5 block text-xs font-medium text-slate-600">
        Tên, email, SĐT hoặc số bảo hiểm
      </label>

      <input
        v-model="searchValue"
        type="text"
        placeholder="Nhập thông tin bệnh nhân..."
        class="w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm text-slate-900 focus:border-violet-600 focus:outline-none focus:ring-1 focus:ring-violet-600"
        @keyup.enter="searchPatient"
      />

      <!-- Thông báo lỗi -->
      <div
        v-if="errorMessage"
        class="mt-3 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600"
      >
        {{ errorMessage }}
      </div>

      <!-- Thông báo thành công -->
      <div
        v-if="successMessage"
        class="mt-3 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700"
      >
        {{ successMessage }}
      </div>

      <button
        type="button"
        class="mt-4 w-full rounded-lg bg-violet-600 px-3 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-violet-700 disabled:cursor-not-allowed disabled:bg-violet-400"
        :disabled="isLoading"
        @click="searchPatient"
      >
        {{ isLoading ? 'Đang tìm...' : 'Tìm bệnh nhân' }}
      </button>
    </div>

    <!-- ====================================================== -->
    <!-- DANH SÁCH BỆNH NHÂN TÌM ĐƯỢC                          -->
    <!-- ====================================================== -->
    <div
      v-if="searchResults.length"
      class="rounded-xl border border-slate-200 bg-white p-6"
    >
      <h2 class="mb-4 text-sm font-semibold text-slate-800">
        Chọn bệnh nhân
      </h2>

      <div class="space-y-3">

        <!-- ==================== 1 BỆNH NHÂN ==================== -->
        <div
          v-for="patient in searchResults"
          :key="patient.id"
        >

          <!-- Ô THÔNG TIN BỆNH NHÂN -->
          <button
            type="button"
            class="w-full rounded-lg border px-4 py-3 text-left transition-colors"
            :class="
              selectedPatient?.id === patient.id
                ? 'border-violet-500 bg-violet-50'
                : 'border-slate-200 bg-white hover:border-violet-200 hover:bg-violet-50/50'
            "
            @click="selectPatient(patient)"
          >
            <div class="flex items-center justify-between gap-3">
              <span class="text-sm font-semibold text-slate-800">
                {{ patientDisplayName(patient) }}
              </span>

              <span class="text-xs text-slate-500">
                {{ patient.insuranceNumber || 'Không có BHYT' }}
              </span>
            </div>

            <div class="mt-2 space-y-1 text-xs text-slate-600">
              <div v-if="patient.email">
                Email: {{ patient.email }}
              </div>

              <div v-if="patient.phoneNumber">
                SĐT: {{ patient.phoneNumber }}
              </div>
            </div>
          </button>

          <!-- ================================================= -->
          <!-- PHẦN NÀY CHỈ HIỆN KHI BẤM VÀO BỆNH NHÂN NÀY       -->
          <!-- ================================================= -->

          <div
            v-if="
              selectedPatient?.id === patient.id &&
              !isLoading
            "
            class="mt-3"
          >

            <!-- ==================== KHÔNG CÓ LỊCH ==================== -->
            <div
              v-if="!upcomingAppointments.length"
              class="rounded-lg border border-slate-200 bg-slate-50 p-4 text-sm text-slate-600"
            >
              Bệnh nhân chưa có lịch hẹn sắp tới.
            </div>

            <!-- ==================== CÓ LỊCH HẸN ==================== -->
            <div
              v-else
              class="rounded-lg border border-slate-200 bg-white p-4"
            >

              <h3 class="mb-3 text-sm font-semibold text-slate-800">
                Lịch hẹn sắp tới
              </h3>

              <div class="space-y-3">

                <!-- MỖI LỊCH HẸN -->
                <div
                  v-for="appointment in upcomingAppointments"
                  :key="appointment.id"
                  role="button"
                  class="flex w-full items-start justify-between rounded-lg border px-4 py-3 text-left transition-colors"
                  :class="
                    selectedAppointmentId === appointment.id
                      ? 'border-violet-500 bg-violet-50'
                      : 'border-slate-200 bg-white hover:border-violet-200 hover:bg-violet-50/50'
                  "
                  @click="selectAppointment(appointment.id)"
                >

                  <!-- THÔNG TIN LỊCH HẸN -->
                  <div class="min-w-0">

                    <div class="flex items-center justify-between gap-3">

                      <span class="text-sm font-semibold text-slate-800">
                        {{ formatDate(appointment.date) }}
                      </span>

                      <span
                        class="rounded-full bg-violet-100 px-2 py-1 text-[10px] font-medium text-violet-700"
                      >
                        {{ appointmentStatusLabel(appointment.status) }}
                      </span>

                    </div>

                    <div class="mt-2 space-y-1 text-xs text-slate-600">

                      <div>
                        Giờ:
                        {{ formatTime(appointment.timeSlot) }}
                      </div>

                      <div>
                        Bác sĩ:
                        {{ appointment.doctorName || 'Chưa xác định' }}
                      </div>

                      <div v-if="appointment.reason">
                        Lý do:
                        {{ appointment.reason }}
                      </div>

                    </div>
                  </div>

                  <!-- ==================== NÚT ACTION ==================== -->
                  <div class="ml-4 flex-shrink-0">

                    <!-- NÚT XÁC NHẬN -->
                    <button
                      v-if="
                        String(appointment.status ?? '')
                          .toLowerCase()
                          .includes('pending') ||
                        String(appointment.status ?? '') === '0'
                      "
                      type="button"
                      class="rounded bg-amber-500 px-3 py-1 text-xs font-medium text-white hover:bg-amber-600 disabled:cursor-not-allowed disabled:opacity-50"
                      :disabled="
                        isLoading ||
                        !!rowLoading[appointment.id]
                      "
                      @click.stop="
                        confirmAppointmentPerRow(appointment.id)
                      "
                    >
                      {{
                        rowLoading[appointment.id]
                          ? 'Đang xử lý...'
                          : 'Xác nhận'
                      }}
                    </button>

                    <!-- NÚT CHECK-IN -->
                    <button
                      v-else-if="
                        String(appointment.status ?? '')
                          .toLowerCase()
                          .includes('confirmed') ||
                        String(appointment.status ?? '') === '1'
                      "
                      type="button"
                      class="rounded bg-violet-600 px-3 py-1 text-xs font-medium text-white hover:bg-violet-700 disabled:cursor-not-allowed disabled:opacity-50"
                      :disabled="
                        isLoading ||
                        !!rowLoading[appointment.id]
                      "
                      @click.stop="
                        checkInPerRow(appointment.id)
                      "
                    >
                      {{
                        rowLoading[appointment.id]
                          ? 'Đang xử lý...'
                          : 'Check-in'
                      }}
                    </button>

                    <!-- TRẠNG THÁI KHÁC -->
                    <span
                      v-else
                      class="text-xs text-slate-400"
                    >
                      —
                    </span>

                  </div>
                </div>

              </div>

              <!-- ================================================= -->
              <!-- NÚT ACTION CHO LỊCH HẸN ĐƯỢC CHỌN                -->
              <!-- ================================================= -->

              <button
                v-if="selectedAppointmentId"
                type="button"
                class="mt-5 w-full rounded-lg bg-violet-600 px-3 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-violet-700 disabled:cursor-not-allowed disabled:bg-violet-400"
                :disabled="isLoading"
                @click="
                  void performAppointmentAction(
                    selectedAppointmentId
                  )
                "
              >
                {{
                  isLoading
                    ? 'Đang xử lý...'
                    : (
                        upcomingAppointments.find(
                          a => a.id === selectedAppointmentId
                        )?.status &&
                        ['pending', '0'].includes(
                          String(
                            upcomingAppointments.find(
                              a => a.id === selectedAppointmentId
                            )?.status
                          ).toLowerCase()
                        )
                          ? 'Xác nhận'
                          : 'Check-in'
                      )
                }}
              </button>

            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ==================== KẾT QUẢ CHECK-IN ==================== -->
    <CheckInResult
      v-if="queueNumber && selectedPatient"
      :queue-number="queueNumber"
      :patient-name="patientDisplayName(selectedPatient)"
    />
  </div>
</template>