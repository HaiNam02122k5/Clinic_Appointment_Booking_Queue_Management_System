<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { usePatientStore } from '@/stores/patient'

const patient = usePatientStore()

let timer: ReturnType<typeof setTimeout> | undefined
let isMounted = false

const isFetching = ref(false)
const isError = ref(false)
const lastUpdatedTime = ref<string | null>(null)

const notificationPermission = ref<NotificationPermission>('default')
const notifiedTicket = ref<string | null>(null)

// Format thời gian cập nhật
const formatTime = (date: Date) => {
  return date.toLocaleTimeString('vi-VN', {
    hour: '2-digit',
    minute: '2-digit',
  })
}

// Gửi Notification bằng Web Notification API
const sendNotification = (title: string, body: string) => {
  // Trình duyệt không hỗ trợ Notification
  if (!('Notification' in window)) {
    return
  }

  // Chưa được cấp quyền
  if (Notification.permission !== 'granted') {
    return
  }

  try {
    new Notification(title, {
      body,
      icon: '/favicon.ico',
    })
  } catch (error) {
    console.warn('Không thể gửi Notification:', error)
  }
}

// Kiểm tra vị trí hàng đợi và gửi thông báo
const checkAndSendNotification = () => {
  if (!patient.queue) return

  const { position, myTicket: ticket } = patient.queue

  // Sắp đến lượt: vị trí 1 hoặc 2
  if (
    position > 0 &&
    position <= 2 &&
    notifiedTicket.value !== ticket
  ) {
    sendNotification(
      'Sắp đến lượt khám!',
      `Bạn đang ở vị trí thứ #${position}. Vui lòng chuẩn bị vào phòng khám!`
    )

    // Đánh dấu vé này đã được thông báo
    notifiedTicket.value = ticket
  }

  // Reset trạng thái thông báo
  // khi vị trí > 2 hoặc đã kết thúc lượt khám
  if (
    (position > 2 || position <= 0) &&
    notifiedTicket.value === ticket
  ) {
    notifiedTicket.value = null
  }
}

// Lên lịch gọi API tiếp theo sau 30 giây
const scheduleNextFetch = () => {
  // Xóa timer cũ nếu có
  if (timer) {
    clearTimeout(timer)
    timer = undefined
  }

  // Component đã bị unmount thì không tạo timer mới
  if (!isMounted) return

  timer = setTimeout(() => {
    fetchQueue()
  }, 30000)
}

// Gọi API lấy thông tin hàng đợi
const fetchQueue = async () => {
  // Không gọi API nếu component đã unmount
  // hoặc đang có request khác
  if (!isMounted || isFetching.value) return

  isFetching.value = true
  isError.value = false

  try {
    await patient.loadQueue()

    // Component có thể đã bị unmount trong lúc chờ API
    if (!isMounted) return

    // Store báo lỗi
    if (patient.queueError) {
      isError.value = true
      return
    }

    // API thành công
    isError.value = false
    lastUpdatedTime.value = formatTime(new Date())

    // Kiểm tra xem có cần gửi notification không
    checkAndSendNotification()
  } catch (error) {
    if (!isMounted) return

    isError.value = true
    console.error('Lỗi khi tải hàng đợi:', error)
  } finally {
    // Luôn kết thúc trạng thái loading
    isFetching.value = false

    // Chỉ tiếp tục polling khi component còn tồn tại
    if (isMounted) {
      scheduleNextFetch()
    }
  }
}

// Xin quyền gửi Notification
const requestNotificationPermission = async () => {
  // Trình duyệt không hỗ trợ Notification
  if (!('Notification' in window)) {
    notificationPermission.value = 'denied'
    return
  }

  try {
    const permission = await Notification.requestPermission()

    notificationPermission.value = permission

    // Nếu người dùng cho phép notification
    // thì kiểm tra ngay trạng thái hàng đợi hiện tại
    if (permission === 'granted') {
      checkAndSendNotification()
    }
  } catch (error) {
    console.error('Lỗi xin quyền thông báo:', error)
  }
}

// Đếm số người đang Waiting
const waitingCount = computed(() => {
  return (
    patient.queue?.entries.filter(
      (entry) => entry.status === 'Waiting'
    ).length ?? 0
  )
})

// Component được mount
onMounted(async () => {
  isMounted = true

  // Kiểm tra trạng thái Notification hiện tại
  if ('Notification' in window) {
    notificationPermission.value = Notification.permission
  } else {
    notificationPermission.value = 'denied'
  }

  // Lấy dữ liệu hàng đợi ngay khi mở trang
  await fetchQueue()
})

// Component bị unmount
onUnmounted(() => {
  isMounted = false

  // Hủy timer để không tiếp tục gọi API
  if (timer) {
    clearTimeout(timer)
    timer = undefined
  }
})

// Expose a small API for tests and external callers
defineExpose({
  isError,
  isFetching,
  lastUpdatedTime,
  waitingCount,
  requestNotificationPermission,
  fetchQueue,
})
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-5">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Trạng thái hàng đợi</h1>
        <p class="text-sm text-slate-500" v-if="lastUpdatedTime">Cập nhật: {{ lastUpdatedTime }}</p>
      </div>
      <div class="text-right">
        <p class="text-sm text-slate-500">Đang chờ: <span class="font-semibold">{{ waitingCount }}</span></p>
      </div>
    </div>

    <div v-if="isFetching" class="py-6 text-center text-sm text-slate-400">Đang tải hàng đợi...</div>

    <div v-else-if="isError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">
      Không thể tải hàng đợi. Vui lòng thử lại sau.
    </div>

    <div v-else-if="!patient.queue">
      <div class="rounded-xl border border-slate-200 bg-white p-6 text-center text-sm text-slate-400">Hiện không có thông tin hàng đợi.</div>
    </div>

    <div v-else class="space-y-4">
      <div class="rounded-2xl border border-blue-200 bg-blue-50 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-600">Số của bạn</p>
            <p class="text-3xl font-bold text-[#0E4D92]">{{ patient.queue.myTicket }}</p>
          </div>
          <div class="text-right">
            <p class="text-sm text-slate-600">Vị trí</p>
            <p class="text-xl font-bold">#{{ patient.queue.position }}</p>
          </div>
        </div>
      </div>

      <div>
        <h2 class="font-semibold text-slate-700">Danh sách hàng đợi</h2>
        <div v-for="entry in patient.queue.entries" :key="entry.ticket" class="mt-3 rounded-xl border p-3 bg-white">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-semibold">{{ entry.patientName }}</p>
              <p class="text-xs text-slate-400">{{ entry.doctorName }} — {{ entry.appointmentTime }}</p>
            </div>
            <div class="text-right">
              <p class="text-sm">{{ entry.ticket }}</p>
              <p class="text-xs text-slate-400">Vị trí: {{ entry.position }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="mt-4">
        <button @click="requestNotificationPermission" class="px-4 py-2 rounded bg-[#0E4D92] text-white">Cho phép thông báo</button>
      </div>
    </div>
  </div>
</template>
