<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ApiError } from '@/lib/api/http'
import { doctorsApi } from '@/features/doctors/doctors.api'
import type { DoctorSchedule, WorkSchedule } from '@/features/doctors/doctors.types'

function isoDate(date: Date) { return date.toISOString().slice(0, 10) }
const today = new Date()
const startDate = ref(isoDate(today))
const end = new Date(today); end.setDate(end.getDate() + 13)
const endDate = ref(isoDate(end))
const data = ref<DoctorSchedule<WorkSchedule> | null>(null)
const loading = ref(false)
const error = ref('')

const activeSchedules = computed(() => data.value?.schedules.filter((s) => s.status === 'Active') ?? [])
function msg(e: unknown) { return e instanceof ApiError ? e.message : e instanceof Error ? e.message : 'Không thể tải lịch.' }
function formatDate(v: string) { return new Intl.DateTimeFormat('vi-VN', { weekday: 'short', day: '2-digit', month: '2-digit' }).format(new Date(`${v}T00:00:00`)) }
async function load() { loading.value = true; error.value = ''; try { data.value = await doctorsApi.getOwnSchedule(startDate.value, endDate.value) } catch (e) { error.value = msg(e) } finally { loading.value = false } }
onMounted(load)
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-end"><div><p class="text-sm font-medium text-violet-600">Bác sĩ</p><h1 class="text-2xl font-bold text-slate-900">Lịch làm việc</h1><p class="mt-1 text-sm text-slate-500">Lịch đã được phòng khám phê duyệt.</p></div><div class="flex flex-wrap gap-2"><input v-model="startDate" type="date" class="rounded-lg border border-slate-200 px-3 py-2 text-sm"><input v-model="endDate" type="date" class="rounded-lg border border-slate-200 px-3 py-2 text-sm"><button class="rounded-lg bg-violet-600 px-4 py-2 text-sm font-semibold text-white disabled:opacity-50" :disabled="loading" @click="load">Xem lịch</button></div></div>
    <div v-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">{{ error }}</div>
    <div class="grid gap-4 sm:grid-cols-3"><div class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"><p class="text-sm text-slate-500">Tổng ca</p><p class="mt-2 text-3xl font-bold text-slate-900">{{ data?.schedules.length ?? 0 }}</p></div><div class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"><p class="text-sm text-slate-500">Ca hoạt động</p><p class="mt-2 text-3xl font-bold text-emerald-600">{{ activeSchedules.length }}</p></div><div class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"><p class="text-sm text-slate-500">Tên bác sĩ</p><p class="mt-2 truncate text-lg font-bold text-slate-900">{{ data?.doctorName || '—' }}</p></div></div>
    <section class="rounded-2xl border border-slate-200 bg-white shadow-sm"><div class="border-b border-slate-100 p-5"><h2 class="font-semibold">Danh sách ca khám</h2></div><div class="overflow-x-auto"><table class="min-w-full text-sm"><thead class="bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500"><tr><th class="px-5 py-3">Ngày</th><th class="px-5 py-3">Thời gian</th><th class="px-5 py-3">Số bệnh nhân tối đa</th><th class="px-5 py-3">Trạng thái</th></tr></thead><tbody class="divide-y divide-slate-100"><tr v-for="shift in data?.schedules" :key="shift.id"><td class="px-5 py-4 font-medium">{{ formatDate(shift.date) }}</td><td class="px-5 py-4">{{ shift.startTime.slice(0,5) }} – {{ shift.endTime.slice(0,5) }}</td><td class="px-5 py-4">{{ shift.patientLimit }}</td><td class="px-5 py-4"><span class="rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-semibold text-emerald-700">{{ shift.status }}</span></td></tr><tr v-if="!loading && !data?.schedules.length"><td colspan="4" class="px-5 py-12 text-center text-slate-500">Không có ca trong khoảng thời gian này.</td></tr></tbody></table></div></section>
  </div>
</template>
