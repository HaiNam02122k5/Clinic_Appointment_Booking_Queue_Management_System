<script setup lang="ts">
import { ref } from 'vue'

const clinicName = ref('Clinic')
const clinicEmail = ref('admin@clinic.vn')
const clinicPhone = ref('024 1234 5678')
const address = ref('Hà Nội, Việt Nam')

const allowBooking = ref(true)
const allowCancel = ref(true)
const allowCheckin = ref(true)
const queueNotification = ref(true)

const saved = ref(false)

function saveSettings() {
  saved.value = true

  setTimeout(() => {
    saved.value = false
  }, 2000)
}
</script>

<template>
  <div class="max-w-5xl space-y-5">

    <!-- TITLE -->

    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Cài đặt
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Quản lý thông tin và cấu hình hệ thống phòng khám
      </p>
    </div>

    <!-- =========================
         CLINIC INFORMATION
    ========================== -->

    <section
      class="rounded-xl border border-slate-200
             bg-white p-6"
    >
      <div class="mb-5">
        <h2 class="text-sm font-semibold text-slate-800">
          Thông tin phòng khám
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Thông tin cơ bản được hiển thị trong hệ thống
        </p>
      </div>

      <div class="grid grid-cols-1 gap-5 md:grid-cols-2">

        <!-- NAME -->

        <div>
          <label
            class="mb-1.5 block text-xs
                   font-medium text-slate-600"
          >
            Tên phòng khám
          </label>

          <input
            v-model="clinicName"
            type="text"
            class="w-full rounded-lg
                   border border-slate-200
                   px-3 py-2.5 text-sm
                   text-slate-800
                   focus:border-violet-500
                   focus:outline-none"
          />
        </div>

        <!-- EMAIL -->

        <div>
          <label
            class="mb-1.5 block text-xs
                   font-medium text-slate-600"
          >
            Email
          </label>

          <input
            v-model="clinicEmail"
            type="email"
            class="w-full rounded-lg
                   border border-slate-200
                   px-3 py-2.5 text-sm
                   text-slate-800
                   focus:border-violet-500
                   focus:outline-none"
          />
        </div>

        <!-- PHONE -->

        <div>
          <label
            class="mb-1.5 block text-xs
                   font-medium text-slate-600"
          >
            Số điện thoại
          </label>

          <input
            v-model="clinicPhone"
            type="text"
            class="w-full rounded-lg
                   border border-slate-200
                   px-3 py-2.5 text-sm
                   text-slate-800
                   focus:border-violet-500
                   focus:outline-none"
          />
        </div>

        <!-- ADDRESS -->

        <div>
          <label
            class="mb-1.5 block text-xs
                   font-medium text-slate-600"
          >
            Địa chỉ
          </label>

          <input
            v-model="address"
            type="text"
            class="w-full rounded-lg
                   border border-slate-200
                   px-3 py-2.5 text-sm
                   text-slate-800
                   focus:border-violet-500
                   focus:outline-none"
          />
        </div>

      </div>
    </section>

    <!-- =========================
         SYSTEM SETTINGS
    ========================== -->

    <section
      class="rounded-xl border border-slate-200
             bg-white p-6"
    >
      <div class="mb-5">
        <h2 class="text-sm font-semibold text-slate-800">
          Cấu hình hệ thống
        </h2>

        <p class="mt-1 text-xs text-slate-400">
          Thiết lập các chức năng hoạt động của phòng khám
        </p>
      </div>

      <div class="divide-y divide-slate-100">

        <!-- BOOKING -->

        <div
          class="flex items-center
                 justify-between py-4"
        >
          <div>
            <div
              class="text-sm font-medium
                     text-slate-800"
            >
              Cho phép đặt lịch
            </div>

            <div
              class="mt-1 text-xs
                     text-slate-400"
            >
              Bệnh nhân có thể đặt lịch khám trực tuyến
            </div>
          </div>

          <button
            type="button"
            class="relative h-6 w-11
                   rounded-full transition"
            :class="
              allowBooking
                ? 'bg-violet-600'
                : 'bg-slate-300'
            "
            @click="allowBooking = !allowBooking"
          >
            <span
              class="absolute top-1 h-4 w-4
                     rounded-full bg-white
                     transition-all"
              :class="
                allowBooking
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>
        </div>

        <!-- CANCEL -->

        <div
          class="flex items-center
                 justify-between py-4"
        >
          <div>
            <div
              class="text-sm font-medium
                     text-slate-800"
            >
              Cho phép hủy lịch
            </div>

            <div
              class="mt-1 text-xs
                     text-slate-400"
            >
              Bệnh nhân có thể hủy lịch hẹn trước giờ khám
            </div>
          </div>

          <button
            type="button"
            class="relative h-6 w-11
                   rounded-full transition"
            :class="
              allowCancel
                ? 'bg-violet-600'
                : 'bg-slate-300'
            "
            @click="allowCancel = !allowCancel"
          >
            <span
              class="absolute top-1 h-4 w-4
                     rounded-full bg-white
                     transition-all"
              :class="
                allowCancel
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>
        </div>

        <!-- CHECKIN -->

        <div
          class="flex items-center
                 justify-between py-4"
        >
          <div>
            <div
              class="text-sm font-medium
                     text-slate-800"
            >
              Cho phép check-in
            </div>

            <div
              class="mt-1 text-xs
                     text-slate-400"
            >
              Lễ tân có thể check-in bệnh nhân tại quầy
            </div>
          </div>

          <button
            type="button"
            class="relative h-6 w-11
                   rounded-full transition"
            :class="
              allowCheckin
                ? 'bg-violet-600'
                : 'bg-slate-300'
            "
            @click="allowCheckin = !allowCheckin"
          >
            <span
              class="absolute top-1 h-4 w-4
                     rounded-full bg-white
                     transition-all"
              :class="
                allowCheckin
                  ? 'left-6'
                  : 'left-1'
              "
            />
          </button>
        </div>

        <!-- NOTIFICATION -->

        <div
          class="flex items-center
                 justify-between py-4"
        >
          <div>
            <div
              class="text-sm font-medium
                     text-slate-800"
            >
              Thông báo hàng đợi
            </div>

            <div
              class="mt-1 text-xs
                     text-slate-400"
            >
              Hiển thị thông báo khi đến lượt khám
            </div>
          </div>

          <button
            type="button"
            class="relative h-6 w-11
                   rounded-full transition"
            :class="
              queueNotification
                ? 'bg-violet-600'
                : 'bg-slate-300'
            "
            @click="
              queueNotification =
                !queueNotification
            "
          >
            <span
              class="absolute top-1 h-4 w-4
                     rounded-full bg-white
                     transition-all"
              :class="
                queueNotification
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

    <div
      class="flex items-center
             justify-end gap-3"
    >
      <span
        v-if="saved"
        class="text-sm font-medium
               text-green-600"
      >
        Đã lưu thay đổi
      </span>

      <button
        class="rounded-lg bg-violet-600
               px-5 py-2.5
               text-sm font-semibold
               text-white
               hover:bg-violet-700"
        @click="saveSettings"
      >
        Lưu thay đổi
      </button>
    </div>

  </div>
</template>
