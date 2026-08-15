<script setup lang="ts">
import { storeToRefs } from 'pinia'

import QueueTable from '@/features/receptionist/components/QueueTable.vue'
import { useReceptionistStore } from '@/stores/receptionist'

const receptionistStore = useReceptionistStore()

const { queue } = storeToRefs(receptionistStore)

function callPatient(no: string) {
  receptionistStore.callPatient(no)
}

function completePatient(no: string) {
  receptionistStore.completePatient(no)
}
</script>

<template>
  <div class="space-y-5 max-w-6xl">

    <!-- TITLE -->
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Quản lý hàng đợi
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Theo dõi và quản lý bệnh nhân đang chờ khám
      </p>
    </div>

    <!-- TABLE CARD -->
    <div
      class="bg-white border border-slate-200
             rounded-xl p-6"
    >

      <h2
        class="text-sm font-semibold
               text-slate-800 mb-4"
      >
        Danh sách hàng đợi
      </h2>

      <QueueTable
        :queue="queue"
        @call="callPatient"
        @complete="completePatient"
      />

    </div>

  </div>
</template>
