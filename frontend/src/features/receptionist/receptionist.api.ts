import type {
  QueueItem,
  RecentCheckin,
} from './receptionist.types'

export const receptionistApi = {
  async getQueue(): Promise<QueueItem[]> {
    return []
  },

  async getRecentCheckins(): Promise<RecentCheckin[]> {
    return []
  },
}
