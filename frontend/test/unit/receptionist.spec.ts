import { beforeEach, describe, expect, it } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'

import { useReceptionistStore } from '@/stores/receptionist'

describe('Receptionist Store - Luồng lễ tân', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  // ================================
  // 1. TRA CỨU LỊCH HẸN
  // ================================

  it('tìm được lịch hẹn bằng mã APT-001', () => {
    const store = useReceptionistStore()

    const appointment = store.findAppointment('APT-001')

    expect(appointment).not.toBeNull()
    expect(appointment?.appointmentId).toBe('APT-001')
  })

  it('không tìm thấy lịch hẹn không tồn tại', () => {
    const store = useReceptionistStore()

    const appointment = store.findAppointment('APT-999')

    expect(appointment).toBeNull()
  })

  // ================================
  // 2. CHECK-IN
  // ================================

  it('check-in bệnh nhân thành công', () => {
    const store = useReceptionistStore()

    const result = store.checkIn('APT-001')

    expect(result).not.toBeNull()
    expect(result?.status).toBe('waiting')
  })

  it('check-in sinh STT tiếp theo là A-029', () => {
    const store = useReceptionistStore()

    const result = store.checkIn('APT-001')

    expect(result?.no).toBe('A-029')
  })

  it('bệnh nhân sau check-in được thêm vào queue', () => {
    const store = useReceptionistStore()

    store.checkIn('APT-001')

    const patient = store.queue.find(
      (item) => item.no === 'A-029'
    )

    expect(patient).toBeDefined()
    expect(patient?.name).toBe('Nguyễn Văn An')
    expect(patient?.status).toBe('waiting')
  })

  it('không cho bệnh nhân check-in lần hai', () => {
    const store = useReceptionistStore()

    const firstCheckIn = store.checkIn('APT-001')
    const secondCheckIn = store.checkIn('APT-001')

    expect(firstCheckIn).not.toBeNull()
    expect(secondCheckIn).toBeNull()
  })

  // ================================
  // 3. QUẢN LÝ HÀNG ĐỢI
  // ================================

  it('gọi số chuyển trạng thái waiting → examining', () => {
    const store = useReceptionistStore()

    store.checkIn('APT-001')
    store.callPatient('A-029')

    const patient = store.queue.find(
      (item) => item.no === 'A-029'
    )

    expect(patient?.status).toBe('examining')
  })

  it('hoàn thành bệnh nhân chuyển examining → completed', () => {
    const store = useReceptionistStore()

    store.checkIn('APT-001')
    store.callPatient('A-029')
    store.completePatient('A-029')

    const patient = store.queue.find(
      (item) => item.no === 'A-029'
    )

    expect(patient?.status).toBe('completed')
  })

  it('không thể hoàn thành bệnh nhân đang chờ', () => {
    const store = useReceptionistStore()

    store.checkIn('APT-001')
    store.completePatient('A-029')

    const patient = store.queue.find(
      (item) => item.no === 'A-029'
    )

    expect(patient?.status).toBe('waiting')
  })

  // ================================
  // 4. THỐNG KÊ DASHBOARD
  // ================================

  it('waitingCount tăng sau khi check-in', () => {
    const store = useReceptionistStore()

    const before = store.waitingCount

    store.checkIn('APT-001')

    expect(store.waitingCount).toBe(before + 1)
  })

  it('completedCount tăng sau khi hoàn thành khám', () => {
    const store = useReceptionistStore()

    store.checkIn('APT-001')
    store.callPatient('A-029')
    store.completePatient('A-029')

    expect(store.completedCount).toBe(1)
  })
})
