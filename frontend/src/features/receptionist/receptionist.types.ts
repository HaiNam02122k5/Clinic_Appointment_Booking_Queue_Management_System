export type QueueStatus =
  | 'in-progress'
  | 'waiting'
  | 'completed'
  | 'skipped'

export interface Doctor {
  id: number
  name: string
  specialty: string
  room: string
  prefix: string
  color: 'blue' | 'violet' | 'emerald'
}

export interface QueueItem {
  ticket: string
  name: string
  docId: number
  time: string
  status: QueueStatus
  wait: number
  urgent: boolean
}

export interface RecentCheckin {
  ticket: string
  name: string
  doc: string
  time: string
}
