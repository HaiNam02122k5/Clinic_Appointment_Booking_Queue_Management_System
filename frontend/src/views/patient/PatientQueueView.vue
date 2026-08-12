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
    if (patient.error) {
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
</script>

