<script setup lang="ts">
import { computed, ref } from 'vue'

import BaseCard from '@/components/ui/BaseCard.vue'
import BaseButton from '@/components/ui/BaseButton.vue'

import {
  DOCTORS,
  RECENT_CHECKINS,
} from '@/features/receptionist/receptionist.data'

const name = ref('')
const phone = ref('')
const bookingCode = ref('')
const docId = ref('')
const urgent = ref(false)

const done = ref(false)
const issued = ref('')

const selectedDoctor = computed(() => {
  return DOCTORS.find(
    (doctor) => doctor.id === Number(docId.value),
  )
})

const currentTime = computed(() => {
  return new Date().toLocaleTimeString('vi-VN', {
    hour: '2-digit',
    minute: '2-digit',
  })
})

function handleCheckin() {
  const doctor = selectedDoctor.value

  if (!doctor) return
  if (!name.value.trim()) return

  issued.value = `${doctor.prefix}00${
    Math.floor(Math.random() * 9) + 5
  }`

  done.value = true
}

function resetCheckin() {
  done.value = false
  name.value = ''
  phone.value = ''
  bookingCode.value = ''
  docId.value = ''
  urgent.value = false
  issued.value = ''
}
</script>

<template>
  <!-- =========================
       PHIẾU XẾP HÀNG
       ========================= -->
  <div
    v-if="done"
    class="max-w-sm mx-auto mt-4"
  >
    <BaseCard class="p-8 text-center">

      <p
        class="text-xs text-slate-400 uppercase tracking-widest mb-3 font-semibold"
      >
        Phiếu xếp hàng
      </p>

      <!-- Ticket number -->
      <div
        class="w-32 h-32 rounded-full mx-auto mb-4 flex items-center justify-center"
        :class="
          urgent
            ? 'bg-red-500'
            : 'bg-[#0E4D92]'
        "
      >
        <div>
          <p
            class="text-3xl font-bold text-white"
          >
            {{ issued }}
          </p>

          <p
            v-if="urgent"
            class="text-[10px] text-red-200 font-bold"
          >
            ƯU TIÊN
          </p>
        </div>
      </div>

      <!-- Patient -->
      <p
        class="font-bold text-slate-800 text-lg mb-1"
      >
        {{ name }}
      </p>

      <!-- Doctor -->
      <p
        class="text-sm text-slate-400 mb-1"
      >
        {{ selectedDoctor?.name }}
      </p>

      <!-- Time -->
      <p
        class="text-xs text-slate-400 mb-6"
      >
        {{ currentTime }}
      </p>

      <!-- New check-in -->
      <BaseButton
        class="w-full"
        @click="resetCheckin"
      >
        Check-in tiếp theo
      </BaseButton>

    </BaseCard>
  </div>

  <!-- =========================
       FORM CHECK-IN
       ========================= -->
  <div
    v-else
    class="max-w-4xl mx-auto space-y-5"
  >

    <!-- Header -->
    <div>
      <h1
        class="text-2xl font-bold text-slate-800"
      >
        Check-in bệnh nhân
      </h1>

      <p
        class="text-sm text-slate-400 mt-1"
      >
        Xác nhận và cấp số thứ tự
      </p>
    </div>

    <div
      class="grid grid-cols-1 md:grid-cols-5 gap-5"
    >

      <!-- =========================
           CHECK-IN FORM
           ========================= -->
      <BaseCard
        class="md:col-span-3 p-5 space-y-4"
      >

        <h3
          class="font-semibold text-slate-700"
        >
          Thông tin check-in
        </h3>

        <div
          class="grid grid-cols-1 sm:grid-cols-2 gap-4"
        >

          <!-- Họ tên -->
          <div>
            <label
              class="block text-sm font-medium text-slate-700 mb-1.5"
            >
              Họ tên bệnh nhân
              <span class="text-red-500">
                *
              </span>
            </label>

            <input
              v-model="name"
              type="text"
              placeholder="Nguyễn Văn A"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
            />
          </div>

          <!-- Số điện thoại -->
          <div>
            <label
              class="block text-sm font-medium text-slate-700 mb-1.5"
            >
              Số điện thoại
            </label>

            <input
              v-model="phone"
              type="tel"
              placeholder="0912 345 678"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
            />
          </div>

          <!-- Mã đặt lịch -->
          <div>
            <label
              class="block text-sm font-medium text-slate-700 mb-1.5"
            >
              Mã đặt lịch (nếu có)
            </label>

            <input
              v-model="bookingCode"
              type="text"
              placeholder="VD: A001"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
            />
          </div>

          <!-- Bác sĩ -->
          <div>
            <label
              class="block text-sm font-medium text-slate-700 mb-1.5"
            >
              Bác sĩ phụ trách
              <span class="text-red-500">
                *
              </span>
            </label>

            <select
              v-model="docId"
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-[#0E4D92]"
            >
              <option value="">
                Chọn bác sĩ…
              </option>

              <option
                v-for="doctor in DOCTORS"
                :key="doctor.id"
                :value="doctor.id"
              >
                {{ doctor.name }}
                —
                {{ doctor.specialty }}
              </option>
            </select>
          </div>

        </div>

        <!-- =========================
             ƯU TIÊN KHẨN CẤP
             ========================= -->
        <label
          class="flex items-center gap-3 p-3 border-2 border-dashed border-red-200 bg-red-50 rounded-xl cursor-pointer hover:bg-red-100 transition-colors"
        >

          <input
            v-model="urgent"
            type="checkbox"
            class="w-4 h-4"
          />

          <div>
            <p
              class="text-sm font-semibold text-red-700"
            >
              Ưu tiên khẩn cấp
            </p>

            <p
              class="text-xs text-red-500"
            >
              Đưa lên đầu hàng đợi của bác sĩ
            </p>
          </div>

        </label>

        <!-- =========================
             CẤP SỐ
             ========================= -->
        <BaseButton
          class="w-full"
          :disabled="
            !name.trim() || !docId
          "
          @click="handleCheckin"
        >
          Cấp số thứ tự
        </BaseButton>

      </BaseCard>

      <!-- =========================
           CHECK-IN GẦN ĐÂY
           ========================= -->
      <BaseCard
        class="md:col-span-2 p-5"
      >

        <h3
          class="font-semibold text-slate-700 mb-4"
        >
          Check-in gần đây
        </h3>

        <div class="space-y-2">

          <div
            v-for="item in RECENT_CHECKINS"
            :key="item.ticket"
            class="flex items-center gap-3 p-2.5 bg-slate-50 rounded-xl"
          >

            <!-- Ticket -->
            <span
              class="text-xs font-bold text-[#0E4D92] w-10"
            >
              {{ item.ticket }}
            </span>

            <!-- Patient -->
            <div
              class="flex-1 min-w-0"
            >
              <p
                class="text-sm font-medium text-slate-800 truncate"
              >
                {{ item.name }}
              </p>

              <p
                class="text-[10px] text-slate-400 truncate"
              >
                {{ item.doc }}
              </p>
            </div>

            <!-- Time -->
            <span
              class="text-xs text-slate-400 shrink-0"
            >
              {{ item.time }}
            </span>

          </div>

        </div>

        <!-- =========================
             TÓM TẮT HÔM NAY
             ========================= -->
        <div
          class="mt-5 pt-4 border-t border-slate-100"
        >

          <p
            class="text-xs text-slate-400 font-medium uppercase tracking-wide mb-2"
          >
            Tóm tắt hôm nay
          </p>

          <div
            class="grid grid-cols-2 gap-2"
          >

            <div
              class="bg-blue-50 rounded-xl p-2.5 text-center"
            >
              <p
                class="font-bold text-[#0E4D92] text-xl"
              >
                23
              </p>

              <p
                class="text-xs text-slate-400"
              >
                Check-in
              </p>
            </div>

            <div
              class="bg-emerald-50 rounded-xl p-2.5 text-center"
            >
              <p
                class="font-bold text-emerald-600 text-xl"
              >
                18
              </p>

              <p
                class="text-xs text-slate-400"
              >
                Đã khám
              </p>
            </div>

          </div>

        </div>

      </BaseCard>

    </div>
  </div>
</template>
