import { http } from '@/lib/api/http'

import type {
  CheckInRequest,
  CheckInResponse,
  QueueStatusResponse,
  QueueResponse,
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
    const response = await http.post<CheckInResponse>(
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
    queueTicketId: string,
  ): Promise<QueueStatusResponse> {
    const response = await http.get<QueueStatusResponse>(
      `/queue-tickets/${queueTicketId}/status`,
    )

    return response.data
  },

  // =========================
  // Get current queue
  // GET /doctors/{doctorId}/queue
  // Doctor | Receptionist
  // =========================

  async getQueue(
    doctorId: number,
  ): Promise<QueueResponse> {
    const response = await http.get<QueueResponse>(
      `/doctors/${doctorId}/queue`,
    )

    return response.data
  },

  // =========================
  // Call queue ticket
  // PATCH /queue-tickets/{id}/call
  // Receptionist
  // =========================

  async callQueueTicket(
    queueTicketId: string,
  ): Promise<CallQueueResponse> {
    const response = await http.patch<CallQueueResponse>(
      `/queue-tickets/${queueTicketId}/call`,
    )

    return response.data
  },

  // =========================
  // Skip queue ticket
  // PATCH /queue-tickets/{id}/skip
  // Receptionist
  // =========================

  async skipQueueTicket(
    queueTicketId: string,
  ): Promise<SkipQueueResponse> {
    const response = await http.patch<SkipQueueResponse>(
      `/queue-tickets/${queueTicketId}/skip`,
    )

    return response.data
  },

  // =========================
  // Set priority
  // PATCH /queue-tickets/{id}/priority
  // Receptionist
  // =========================

  async setPriority(
    queueTicketId: string,
    data: SetPriorityRequest,
  ): Promise<SetPriorityResponse> {
    const response = await http.patch<SetPriorityResponse>(
      `/queue-tickets/${queueTicketId}/priority`,
      data,
    )

    return response.data
  },
}
