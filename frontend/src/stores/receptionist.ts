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
    // GET /doctors/{doctorId}/queue
    // =========================

    async function loadQueue(
      doctorId: number,
    ) {
      loading.value = true
      error.value = null

      try {
        const response =
          await queueApi.getQueue(doctorId)

        queue.value = response.items
      } catch (err) {
        error.value =
          err instanceof Error
            ? err.message
            : 'Không thể tải hàng đợi'

        throw err
      } finally {
        loading.value = false
      }
    }

    // =========================
    // Check-in
    // POST /queue-tickets/check-in
    // =========================

    async function checkIn(
      data: CheckInRequest,
    ) {
      loading.value = true
      error.value = null

      try {
        return await queueApi.checkIn(data)
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
    // PATCH /queue-tickets/{id}/call
    // =========================

    async function call(
      queueTicketId: string,
    ) {
      loading.value = true
      error.value = null

      try {
        return await queueApi.call(
          queueTicketId,
        )
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
    // PATCH /queue-tickets/{id}/skip
    // =========================

    async function skip(
      queueTicketId: string,
    ) {
      loading.value = true
      error.value = null

      try {
        return await queueApi.skip(
          queueTicketId,
        )
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
    // PATCH /queue-tickets/{id}/priority
    // =========================

    async function setPriority(
      queueTicketId: string,
      priority: boolean,
    ) {
      loading.value = true
      error.value = null

      try {
        return await queueApi.setPriority(
          queueTicketId,
          {
            priority,
          },
        )
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
