import { defineStore } from 'pinia'
import { ref } from 'vue'

import { queueApi } from '@/features/queue/queue.api'
import type {
  QueueItem,
  CheckInRequest,
} from '@/features/queue/queue.types'

export const useReceptionistStore = defineStore(
  'receptionist',
  () => {
    const queue = ref<QueueItem[]>([])
    const loading = ref(false)
    const error = ref<string | null>(null)

    // =========================
    // Load queue
    // =========================

    async function loadQueue(doctorId: number) {
      loading.value = true
      error.value = null

      try {
        const response = await queueApi.getQueue(doctorId)

        queue.value = response.items
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể tải hàng đợi'
      } finally {
        loading.value = false
      }
    }

    // =========================
    // Check-in
    // =========================

    async function checkIn(
      data: CheckInRequest,
    ) {
      loading.value = true
      error.value = null

      try {
        const result = await queueApi.checkIn(data)

        return result
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể check-in bệnh nhân'

        throw err
      } finally {
        loading.value = false
      }
    }

    // =========================
    // Call
    // =========================

    async function call(queueTicketId: string) {
      loading.value = true
      error.value = null

      try {
        const result =
          await queueApi.callQueueTicket(queueTicketId)

        return result
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể gọi bệnh nhân'

        throw err
      } finally {
        loading.value = false
      }
    }

    // =========================
    // Skip
    // =========================

    async function skip(queueTicketId: string) {
      loading.value = true
      error.value = null

      try {
        const result =
          await queueApi.skipQueueTicket(queueTicketId)

        return result
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể bỏ lượt'

        throw err
      } finally {
        loading.value = false
      }
    }

    // =========================
    // Priority
    // =========================

    async function setPriority(
      queueTicketId: string,
      priority: boolean,
    ) {
      loading.value = true
      error.value = null

      try {
        const result =
          await queueApi.setPriority(
            queueTicketId,
            {
              priority,
            },
          )

        return result
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể cập nhật ưu tiên'

        throw err
      } finally {
        loading.value = false
      }
    }

    return {
      queue,
      loading,
      error,

      loadQueue,
      checkIn,
      call,
      skip,
      setPriority,
    }
  },
)
