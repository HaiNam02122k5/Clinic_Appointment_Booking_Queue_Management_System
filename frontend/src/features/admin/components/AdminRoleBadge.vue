<script setup lang="ts">
import type { AccountRole } from '@/features/admin/admin.mock'

defineProps<{
  role: AccountRole
}>()

const roleText: Record<AccountRole, string> = {
  patient: 'Bệnh nhân',
  doctor: 'Bác sĩ',
  receptionist: 'Lễ tân',
  admin: 'Quản trị viên',
}

const roleClass: Record<AccountRole, string> = {
  patient: 'bg-slate-100 text-slate-600',
  doctor: 'bg-blue-50 text-blue-700',
  receptionist: 'bg-violet-50 text-violet-700',
  admin: 'bg-amber-50 text-amber-700',
}
</script>

<template>
  <span
    class="inline-flex rounded-md px-2.5 py-1 text-xs font-medium"
    :class="roleClass[role]"
  >
    {{ roleText[role] }}
  </span>
</template>
