<script setup lang="ts">
import { ref } from 'vue'

const clinicName = ref('')
const clinicAddress = ref('')
const clinicPhone = ref('')
const clinicEmail = ref('')

const openingTime = ref('')
const closingTime = ref('')

const advanceBookingDays = ref<number | null>(null)
const appointmentDuration = ref<number | null>(null)

const onlineBooking = ref(false)
const autoQueueNumber = ref(false)
const smsNotification = ref(false)
const autoReport = ref(false)

function saveSettings() {
  // TODO: Gọi API cập nhật cài đặt khi BE hoàn thiện
  console.log('Save settings')
}
</script>

<template>
  <div class="space-y-5">

    <!-- =========================
         PAGE HEADER
    ========================== -->

    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Cài đặt hệ thống
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Quản lý thông tin và cấu hình hoạt động của phòng khám
      </p>
    </div>


    <!-- =========================
         CLINIC INFORMATION
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white">

      <div class="border-b border-slate-200 px-6 py-5">

        <h2 class="text-sm font-semibold text-slate-800">
          Thông tin phòng khám
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Thông tin cơ bản được sử dụng trong hệ thống
        </p>

      </div>


      <div class="grid gap-5 p-6 md:grid-cols-2">

        <!-- Clinic name -->
        <div>
          <label
            for="clinic-name"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Tên phòng khám
          </label>

          <input
            id="clinic-name"
            v-model="clinicName"
            type="text"
            placeholder="Nhập tên phòng khám"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>


        <!-- Phone -->
        <div>
          <label
            for="clinic-phone"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Số điện thoại
          </label>

          <input
            id="clinic-phone"
            v-model="clinicPhone"
            type="tel"
            placeholder="Nhập số điện thoại"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>


        <!-- Address -->
        <div>
          <label
            for="clinic-address"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Địa chỉ
          </label>

          <input
            id="clinic-address"
            v-model="clinicAddress"
            type="text"
            placeholder="Nhập địa chỉ phòng khám"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>


        <!-- Email -->
        <div>
          <label
            for="clinic-email"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Email
          </label>

          <input
            id="clinic-email"
            v-model="clinicEmail"
            type="email"
            placeholder="Nhập email phòng khám"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>

      </div>

    </section>


    <!-- =========================
         WORKING HOURS
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white">

      <div class="border-b border-slate-200 px-6 py-5">

        <h2 class="text-sm font-semibold text-slate-800">
          Thời gian hoạt động
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Cấu hình giờ hoạt động và thời gian đặt lịch
        </p>

      </div>


      <div class="grid gap-5 p-6 md:grid-cols-2 lg:grid-cols-4">

        <!-- Opening -->
        <div>
          <label
            for="opening-time"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Giờ mở cửa
          </label>

          <input
            id="opening-time"
            v-model="openingTime"
            type="time"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
          />
        </div>


        <!-- Closing -->
        <div>
          <label
            for="closing-time"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Giờ đóng cửa
          </label>

          <input
            id="closing-time"
            v-model="closingTime"
            type="time"
            class="h-10 w-full rounded-lg border border-slate-200 px-3 text-sm text-slate-700 outline-none focus:border-brand-500"
          />
        </div>


        <!-- Advance booking -->
        <div>
          <label
            for="advance-booking"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Đặt lịch trước tối đa
          </label>

          <div class="relative">

            <input
              id="advance-booking"
              v-model="advanceBookingDays"
              type="number"
              min="0"
              placeholder="Nhập số ngày"
              class="h-10 w-full rounded-lg border border-slate-200 px-3 pr-14 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500"
            />

            <span
              class="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-slate-400"
            >
              ngày
            </span>

          </div>
        </div>


        <!-- Appointment duration -->
        <div>
          <label
            for="appointment-duration"
            class="mb-1.5 block text-sm font-medium text-slate-700"
          >
            Thời lượng mỗi ca
          </label>

          <div class="relative">

            <input
              id="appointment-duration"
              v-model="appointmentDuration"
              type="number"
              min="0"
              placeholder="Nhập thời lượng"
              class="h-10 w-full rounded-lg border border-slate-200 px-3 pr-16 text-sm text-slate-700 outline-none placeholder:text-slate-400 focus:border-brand-500"
            />

            <span
              class="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-slate-400"
            >
              phút
            </span>

          </div>
        </div>

      </div>

    </section>


    <!-- =========================
         SYSTEM OPTIONS
    ========================== -->

    <section class="rounded-xl border border-slate-200 bg-white">

      <div class="border-b border-slate-200 px-6 py-5">

        <h2 class="text-sm font-semibold text-slate-800">
          Tùy chọn hệ thống
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Bật hoặc tắt các chức năng của hệ thống
        </p>

      </div>


      <div class="divide-y divide-slate-100">

        <!-- Online booking -->
        <div class="flex items-center justify-between gap-5 px-6 py-5">

          <div>
            <p class="text-sm font-medium text-slate-700">
              Đặt lịch trực tuyến
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Cho phép bệnh nhân tự đặt lịch khám trực tuyến
            </p>
          </div>

          <button
            type="button"
            role="switch"
            :aria-checked="onlineBooking"
            class="relative h-6 w-11 shrink-0 rounded-full transition"
            :class="
              onlineBooking
                ? 'bg-brand-600'
                : 'bg-slate-200'
            "
            @click="onlineBooking = !onlineBooking"
          >
            <span
              class="absolute top-1 h-4 w-4 rounded-full bg-white shadow transition"
              :class="
                onlineBooking
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>

        </div>


        <!-- Auto queue -->
        <div class="flex items-center justify-between gap-5 px-6 py-5">

          <div>
            <p class="text-sm font-medium text-slate-700">
              Tự động cấp số thứ tự
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Tự động cấp số khi bệnh nhân check-in
            </p>
          </div>

          <button
            type="button"
            role="switch"
            :aria-checked="autoQueueNumber"
            class="relative h-6 w-11 shrink-0 rounded-full transition"
            :class="
              autoQueueNumber
                ? 'bg-brand-600'
                : 'bg-slate-200'
            "
            @click="autoQueueNumber = !autoQueueNumber"
          >
            <span
              class="absolute top-1 h-4 w-4 rounded-full bg-white shadow transition"
              :class="
                autoQueueNumber
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>

        </div>


        <!-- SMS -->
        <div class="flex items-center justify-between gap-5 px-6 py-5">

          <div>
            <p class="text-sm font-medium text-slate-700">
              Thông báo SMS
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Gửi SMS nhắc lịch và thông báo cho bệnh nhân
            </p>
          </div>

          <button
            type="button"
            role="switch"
            :aria-checked="smsNotification"
            class="relative h-6 w-11 shrink-0 rounded-full transition"
            :class="
              smsNotification
                ? 'bg-brand-600'
                : 'bg-slate-200'
            "
            @click="smsNotification = !smsNotification"
          >
            <span
              class="absolute top-1 h-4 w-4 rounded-full bg-white shadow transition"
              :class="
                smsNotification
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>

        </div>


        <!-- Auto report -->
        <div class="flex items-center justify-between gap-5 px-6 py-5">

          <div>
            <p class="text-sm font-medium text-slate-700">
              Báo cáo tự động
            </p>

            <p class="mt-1 text-xs text-slate-400">
              Tự động tổng hợp và gửi báo cáo định kỳ
            </p>
          </div>

          <button
            type="button"
            role="switch"
            :aria-checked="autoReport"
            class="relative h-6 w-11 shrink-0 rounded-full transition"
            :class="
              autoReport
                ? 'bg-brand-600'
                : 'bg-slate-200'
            "
            @click="autoReport = !autoReport"
          >
            <span
              class="absolute top-1 h-4 w-4 rounded-full bg-white shadow transition"
              :class="
                autoReport
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>

        </div>

      </div>

    </section>


    <!-- =========================
         SAVE
    ========================== -->

    <div class="flex justify-end gap-3">

      <button
        type="button"
        class="rounded-lg border border-slate-200 px-5 py-2.5 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
      >
        Hủy
      </button>

      <button
        type="button"
        class="rounded-lg bg-brand-600 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-brand-700"
        @click="saveSettings"
      >
        Lưu cài đặt
      </button>

    </div>

  </div>
</template>
