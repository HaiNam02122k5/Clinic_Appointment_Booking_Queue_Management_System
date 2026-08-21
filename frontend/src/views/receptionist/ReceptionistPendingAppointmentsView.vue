<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { http } from '@/lib/api/http'

type AppointmentItem = {
  id: string
  patientId: string
  doctorId?: string | null
  patientName?: string | null
  doctorName?: string | null
  date?: string | null
  timeSlot?: string | { hours?: number; minutes?: number; seconds?: number } | null
  reason?: string | null
  status?: string | number | null
}

type PaginationEnvelope<T> = {
  items?: T[]
  totalCount?: number
  pageNumber?: number
  pageSize?: number
}

function unwrapApiResult<T>(payload: unknown): T | null {
  if (!payload || typeof payload !== 'object') {
    return payload as T | null
  }

  const maybeEnvelope = payload as { result?: T; data?: T; items?: T }
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

const appointments = ref<AppointmentItem[]>([])
const pageNumber = ref(1)
const pageSize = ref(10)
const total = ref(0)
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

function formatDate(value?: string | null): string {
  if (!value) return '—'
  const parsed = new Date(value.includes('T') ? value : `${value}T00:00:00`)
  if (Number.isNaN(parsed.getTime())) return value
  return parsed.toLocaleDateString('vi-VN')
}

function formatTime(value: unknown): string {
  if (!value) return '—'
  if (typeof value === 'string') return value.includes(':') ? value.slice(0, 5) : value
  if (typeof value === 'object' && value && 'hours' in value) {
    const v = value as { hours?: number; minutes?: number }
    return `${String(v.hours ?? 0).padStart(2, '0')}:${String(v.minutes ?? 0).padStart(2, '0')}`
  }
  return String(value)
}

async function loadPending() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    const { data } = await http.get('/receptionist/appointments/pending', {
      params: {
        pageNumber: pageNumber.value,
        pageSize: pageSize.value,
        sortBy: 'date',
        orderBy: 'asc',
      },
    })

    const payload = unwrapApiResult<PaginationEnvelope<AppointmentItem> | AppointmentItem[] | null>(data)
    const nextItems = Array.isArray(payload)
      ? payload
      : Array.isArray(payload?.items)
        ? payload.items
        : []

    appointments.value = nextItems
    total.value = Array.isArray(payload)
      ? payload.length
      : Number(payload?.totalCount ?? nextItems.length)
  } catch (err: unknown) {
  // surface server message when possible
  try {
    const msg = (err as any)?.response?.data?.message || (err as any)?.response?.data?.errorMessages?.join(', ')
    errorMessage.value = msg || 'Không thể tải danh sách chờ xác nhận.'
  } catch {
    errorMessage.value = 'Không thể tải danh sách chờ xác nhận.'
  }
  } finally {
  isLoading.value = false
  }
}

async function confirmAppointment(id: string) {
  errorMessage.value = ''
  successMessage.value = ''
  isLoading.value = true
  try {
    await http.post(`/appointments/${id}/confirm`)
    successMessage.value = 'Đã xác nhận cuộc hẹn.'
    // remove from list as it's no longer pending
    appointments.value = appointments.value.filter((a) => a.id !== id)
  } catch (err: unknown) {
    const status = typeof err === 'object' && err !== null && 'response' in err
      ? Number((err as any).response?.status)
      : undefined

    if (status === 400) {
      errorMessage.value = 'Cuộc hẹn không hợp lệ để xác nhận.'
    } else if (status === 404) {
      errorMessage.value = 'Không tìm thấy cuộc hẹn.'
    } else if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền xác nhận cuộc hẹn.'
    } else {
      errorMessage.value = 'Xác nhận thất bại. Vui lòng thử lại.'
    }
  } finally {
    isLoading.value = false
  }
}

function prevPage() {
  if (pageNumber.value > 1) {
    pageNumber.value--
    void loadPending()
  }
}

function nextPage() {
  if (pageNumber.value * pageSize.value < total.value) {
    pageNumber.value++
    void loadPending()
  }
}

onMounted(() => {
  void loadPending()
})
</script>

<template>
  <div class="max-w-4xl">
    <h1 class="text-xl font-semibold text-slate-800 mb-4">Danh sách lịch hẹn chờ xác nhận</h1>

    <div v-if="errorMessage" class="mb-3 rounded border border-red-200 bg-red-50 p-3 text-sm text-red-600">
      {{ errorMessage }}
    </div>
    <div v-if="successMessage" class="mb-3 rounded border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">
      {{ successMessage }}
    </div>

    <div class="rounded-lg border bg-white p-4">
      <table class="w-full table-auto text-sm">
        <thead>
          <tr class="text-left text-xs text-slate-500">
            <th class="py-2">Ngày</th>
            <th class="py-2">Giờ</th>
            <th class="py-2">Bệnh nhân</th>
            <th class="py-2">Bác sĩ</th>
            <th class="py-2">Lý do</th>
            <th class="py-2">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="a in appointments" :key="a.id" class="border-t">
            <td class="py-2">{{ formatDate(a.date) }}</td>
            <td class="py-2">{{ formatTime(a.timeSlot) }}</td>
            <td class="py-2">{{ a.patientName ?? '—' }}</td>
            <td class="py-2">{{ a.doctorName ?? '—' }}</td>
            <td class="py-2">{{ a.reason ?? '—' }}</td>
            <td class="py-2">
              <button
                class="rounded bg-violet-600 px-3 py-1 text-xs font-medium text-white hover:bg-violet-700"
                :disabled="isLoading"
                @click="confirmAppointment(a.id)"
              >
                Xác nhận
              </button>
            </td>
          </tr>
          <tr v-if="!appointments.length && !isLoading">
            <td colspan="6" class="py-3 text-center text-sm text-slate-500">Không có lịch hẹn chờ xác nhận.</td>
          </tr>
        </tbody>
      </table>

      <div class="mt-4 flex items-center justify-between">
        <div class="text-sm text-slate-600">Tổng: {{ total }}</div>
        <div class="space-x-2">
          <button class="rounded border px-3 py-1 text-sm" @click="prevPage" :disabled="pageNumber === 1">Trước</button>
          <button class="rounded border px-3 py-1 text-sm" @click="nextPage" :disabled="pageNumber * pageSize >= total">Sau</button>
        </div>
      </div>
    </div>
  </div>
</template>
