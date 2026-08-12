import { http } from '@/lib/api/http'

import type {
  CheckInRequest,
  CheckInResponse,
  QueueResponse,
  QueueStatusResponse,
  CallQueueResponse,
  SkipQueueResponse,
  SetPriorityRequest,
  SetPriorityResponse,
} from './queue.types'

export const queueApi = {
  // =========================
  // Check-in
  // POST /queue-tickets/check-in
  // =========================

  async checkIn(
    data: CheckInRequest,
  ): Promise<CheckInResponse> {
    const response =
      await http.post<CheckInResponse>(
        '/queue-tickets/check-in',
        data,
      )

    return response.data
  },


  // =========================
  // Queue status
  // GET /queue-tickets/{id}/status
  // =========================

  async getQueueStatus(
    id: string,
  ): Promise<QueueStatusResponse> {
    const response =
      await http.get<QueueStatusResponse>(
        `/queue-tickets/${id}/status`,
      )

    return response.data
  },


  // =========================
  // Get queue
  // GET /doctors/{doctorId}/queue
  // Doctor | Receptionist
  // =========================

  async getQueue(
    doctorId: number,
  ): Promise<QueueResponse> {
    const response =
      await http.get<QueueResponse>(
        `/doctors/${doctorId}/queue`,
      )

    return response.data
  },


  // =========================
  // Call
  // PATCH /queue-tickets/{id}/call
  // Receptionist
  // =========================

  async call(
    id: string,
  ): Promise<CallQueueResponse> {
    const response =
      await http.patch<CallQueueResponse>(
        `/queue-tickets/${id}/call`,
      )

    return response.data
  },


  // =========================
  // Skip
  // PATCH /queue-tickets/{id}/skip
  // Receptionist
  // =========================

  async skip(
    id: string,
  ): Promise<SkipQueueResponse> {
    const response =
      await http.patch<SkipQueueResponse>(
        `/queue-tickets/${id}/skip`,
      )

    return response.data
  },


  // =========================
  // Priority
  // PATCH /queue-tickets/{id}/priority
  // Receptionist
  // =========================

  async setPriority(
    id: string,
    data: SetPriorityRequest,
  ): Promise<SetPriorityResponse> {
    const response =
      await http.patch<SetPriorityResponse>(
        `/queue-tickets/${id}/priority`,
        data,
      )

    return response.data
  },
}
