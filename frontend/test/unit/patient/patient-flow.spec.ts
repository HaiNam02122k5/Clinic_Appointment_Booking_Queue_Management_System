import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'

import { usePatientStore } from '@/stores/patient'
import { patientApi } from '@/features/patients/patient.api'

describe('patient unit flow - booking and appointments', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('loads doctors and keeps them in store', async () => {
    vi.spyOn(patientApi, 'getDoctors').mockResolvedValue([
      { id: 1, name: 'BS. Trần Minh Hùng', specialty: 'Nội tổng quát', room: 'P.101' },
      { id: 2, name: 'BS. Lê Hồng Đăng', specialty: 'Tim mạch', room: 'P.205' },
    ])

    const store = usePatientStore()
    await store.loadDoctors('Nội tổng quát')

    expect(store.doctors).toHaveLength(2)
    expect(store.doctors[0].name).toContain('BS. Trần Minh Hùng')
    expect(store.doctorsLoading).toBe(false)
  })

  it('creates an appointment and stores it as pending', async () => {
    vi.spyOn(patientApi, 'createAppointment').mockResolvedValue({
      id: 99,
      doctorId: 1,
      doctorName: 'BS. Trần Minh Hùng',
      specialty: 'Nội tổng quát',
      appointmentDate: '20/08/2026',
      appointmentTime: '08:00',
      status: 'Pending',
      queueNumber: 'A-12',
    } as any)

    const store = usePatientStore()
    const result = await store.createAppointment({
      doctorId: 1,
      workScheduleId: 10,
      appointmentDate: '2026-08-20',
      appointmentTime: '08:00',
      reason: 'Ho cảm',
    })

    expect(result.status).toBe('Pending')
    expect(store.appointments[0].id).toBe(99)
    expect(store.appointmentsLoading).toBe(false)
  })

  it('loads patient appointments and cancels one upcoming appointment', async () => {
    vi.spyOn(patientApi, 'getMyAppointments').mockResolvedValue([
      {
        id: 55,
        doctorId: 1,
        doctorName: 'BS. Trần Minh Hùng',
        specialty: 'Nội tổng quát',
        appointmentDate: '20/08/2026',
        appointmentTime: '08:00',
        status: 'Pending',
        queueNumber: 'A-10',
      },
    ] as any)

    const cancelSpy = vi.spyOn(patientApi, 'cancelAppointment').mockResolvedValue(undefined)

    const store = usePatientStore()
    await store.loadAppointments()
    await store.cancelAppointment(55)

    expect(store.appointments).toHaveLength(1)
    expect(cancelSpy).toHaveBeenCalledWith(55)
    expect(store.appointments[0].status).toBe('Cancelled')
  })

  it('loads queue status for the patient and preserves ticket info', async () => {
    vi.spyOn(patientApi, 'getMyQueue').mockResolvedValue({
      myTicket: 'A-12',
      position: 2,
      estimatedWaitMinutes: 15,
      doctorName: 'BS. Trần Minh Hùng',
      appointmentTime: '08:00',
      currentTicket: 'A-12',
      entries: [
        {
          ticket: 'A-12',
          patientName: 'Nguyễn Văn A',
          doctorId: 1,
          doctorName: 'BS. Trần Minh Hùng',
          appointmentTime: '08:00',
          status: 'Waiting',
          estimatedWaitMinutes: 15,
          position: 2,
          urgent: false,
        },
      ],
    })

    const store = usePatientStore()
    await store.loadQueue()

    expect(store.queue?.myTicket).toBe('A-12')
    expect(store.queue?.position).toBe(2)
    expect(store.queue?.doctorName).toContain('BS. Trần Minh Hùng')
  })

  it('loads medical history and patient profile', async () => {
    vi.spyOn(patientApi, 'getMedicalHistory').mockResolvedValue([
      {
        id: 1,
        examinationDate: '15/08/2026',
        doctorName: 'BS. Trần Minh Hùng',
        specialty: 'Nội tổng quát',
        diagnosis: 'Cảm cúm',
        prescription: 'Thuốc hạ sốt',
        note: 'Tái khám sau 3 ngày',
      },
    ])

    vi.spyOn(patientApi, 'getMyProfile').mockResolvedValue({
      id: 88,
      fullName: 'Nguyễn Văn A',
      email: 'a@example.com',
      phoneNumber: '0912345678',
      address: 'Hà Nội',
      dateOfBirth: '1998-05-10',
      dateOfBirthDisplay: '10/05/1998',
      gender: 0,
      insuranceNumber: '',
      emergencyContact: '',
    })

    const store = usePatientStore()
    await store.loadHistory()
    await store.loadProfile()

    expect(store.history).toHaveLength(1)
    expect(store.history[0].diagnosis).toBe('Cảm cúm')
    expect(store.profile?.fullName).toBe('Nguyễn Văn A')
    expect(store.profile?.email).toBe('a@example.com')
  })
})
