export interface Doctor {
  id: number
  fullName: string
  specialty?: string
}

export interface RecentCheckin {
  ticket: string
  name: string
  doc: string
  time: string
}
