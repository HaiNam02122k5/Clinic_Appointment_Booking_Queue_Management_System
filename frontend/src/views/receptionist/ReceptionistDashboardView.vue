<script setup lang="ts">
import { computed } from 'vue'
import { storeToRefs } from 'pinia'

import StatCard from '@/features/receptionist/components/StatCard.vue'
import QueuePreview from '@/features/receptionist/components/QueuePreview.vue'

import { useReceptionistStore } from '@/stores/receptionist'

const receptionistStore = useReceptionistStore()

const {
  queue,
  waitingCount,
  completedCount,
} = storeToRefs(receptionistStore)

const stats = computed(() => [
  {
    label: 'Tổng lịch hẹn',
    value: receptionistStore.appointments.length,
    color: '#7C3AED',
  },
  {
    label: 'Đã check-in',
    value: receptionistStore.appointments.filter(
      appointment => appointment.checkedIn
    ).length,
    color: '#0E4D92',
  },
  {
    label: 'Đang chờ',
    value: waitingCount.value,
    color: '#D97706',
  },
  {
    label: 'Đã khám xong',
    value: completedCount.value,
    color: '#00A878',
  },
])
</script>

<template>
  <div class="space-y-5 max-w-5xl">

    <!-- ================= TITLE ================= -->
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Tổng quan
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Tổng quan hoạt động khám bệnh trong ngày
      </p>
    </div>

    <!-- ================= STATISTICS ================= -->
    <div class="grid grid-cols-4 gap-4">

      <StatCard
        v-for="stat in stats"
        :key="stat.label"
        :label="stat.label"
        :value="stat.value"
        :color="stat.color"
      />

    </div>

    <!-- ================= CURRENT QUEUE ================= -->
    <QueuePreview :queue="queue" />

  </div>
</template>
