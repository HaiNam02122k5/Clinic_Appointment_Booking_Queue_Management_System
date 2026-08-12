// =========================
// Check-in
// =========================

export interface CheckInRequest {
  appointmentId: string
  priority: boolean
}

export interface CheckInResponse {
  id: string
  queueNumber: number
  status: string
  currentPosition: number
  estimatedWaitMinutes: number
}


// =========================
// Queue status
// =========================

export interface QueueStatusResponse {
  id: string
  queueNumber: number
  status: string
  currentPosition: number
  estimatedWaitMinutes: number
}


// =========================
// Queue list
// GET /doctors/{doctorId}/queue
// =========================

export interface QueuePatient {
  id: number
  name: string
  age: number
}

export interface QueueItem {
  queueNumber: number
  patient: QueuePatient
  reason: string
  status: string
}

export interface QueueResponse {
  items: QueueItem[]
}


// =========================
// Call
// =========================

export interface CallQueueResponse {
  id: string
  status: string
  calledAt: string
}


// =========================
// Skip
// =========================

export interface SkipQueueResponse {
  id: string
  status: string
}


// =========================
// Priority
// =========================

export interface SetPriorityRequest {
  priority: boolean
}

export interface SetPriorityResponse {
  id: string
  priority: boolean
}
