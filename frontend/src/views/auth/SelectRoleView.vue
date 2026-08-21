<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { UserRole } from '@/features/auth/auth.types'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

interface RoleOption {
  role: UserRole
  title: string
  desc: string
  emoji: string
  tag: string
  tagColor: 'blue' | 'violet' | 'green' | 'amber'
  features: string[]
}

const ROLE_META: Record<UserRole, Omit<RoleOption, 'role'>> = {
  Patient: {
    title: 'Bệnh nhân',
    desc: 'Đặt lịch và theo dõi hàng đợi trực tuyến',
    emoji: '🧑‍⚕️',
    tag: 'Dùng miễn phí',
    tagColor: 'blue',
    features: ['Đặt lịch khám 24/7', 'Nhận thông báo SMS', 'Xem lịch sử khám'],
  },
  Receptionist: {
    title: 'Lễ tân',
    desc: 'Quản lý check-in và điều phối hàng đợi',
    emoji: '🗂️',
    tag: 'Nội bộ',
    tagColor: 'violet',
    features: ['Gọi số thứ tự', 'Check-in bệnh nhân', 'Ưu tiên khẩn cấp'],
  },
  Doctor: {
    title: 'Bác sĩ',
    desc: 'Xem lịch và ghi kết quả khám bệnh',
    emoji: '👨‍⚕️',
    tag: 'Nội bộ',
    tagColor: 'green',
    features: ['Danh sách bệnh nhân', 'Ghi chẩn đoán', 'Quản lý lịch'],
  },
  Admin: {
    title: 'Quản trị viên',
    desc: 'Quản lý toàn hệ thống và thống kê',
    emoji: '📊',
    tag: 'Quản lý',
    tagColor: 'amber',
    features: ['Báo cáo thống kê', 'Quản lý bác sĩ', 'Cấu hình hệ thống'],
  },
}

const availableRoles = computed<UserRole[]>(() => auth.user?.roles?.length ? auth.user.roles : [auth.currentUserRole ?? 'Patient'])
const roles = computed<RoleOption[]>(() => availableRoles.value.map((role) => ({ role, ...ROLE_META[role] })))

function selectRole(role: UserRole) {
  auth.setActiveRole(role)

  const redirect = route?.query?.redirect as string | undefined
  const routeMap: Record<UserRole, string> = {
    Patient: '/patient',
    // Default receptionist entry should be the dashboard overview
    Receptionist: '/reception',
    Doctor: '/doctor/examination',
    Admin: '/admin/doctors',
  }

  if (router.replace) {
    router.replace(redirect || routeMap[role])
    return
  }

  if (router.push) {
    router.push(redirect || routeMap[role])
  }
}

function getTagClass(color: RoleOption['tagColor']) {
  const map = {
    blue: 'bg-blue-50 text-blue-700 border-blue-200',
    violet: 'bg-violet-50 text-violet-700 border-violet-200',
    green: 'bg-emerald-50 text-emerald-700 border-emerald-200',
    amber: 'bg-amber-50 text-amber-700 border-amber-200',
  }
  return map[color]
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 flex flex-col font-sans">
    <!-- Header -->
    <header class="bg-white/80 backdrop-blur border-b border-slate-200 h-16 flex items-center px-6 justify-between sticky top-0 z-10">
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 bg-[#0E4D92] rounded-xl flex items-center justify-center text-white shadow-sm font-bold text-lg">
          +
        </div>
        <div class="leading-tight">
          <p class="font-bold text-slate-800 text-[15px]">ClinicQueue</p>
          <p class="text-[10px] text-slate-400 hidden sm:block">
            Đặt lịch & Quản lý hàng đợi khám bệnh
          </p>
        </div>
      </div>
      <div class="flex items-center gap-2.5">
        <button
          type="button"
          @click="router.push('/register')"
          class="px-3.5 py-1.5 text-sm font-semibold rounded-xl text-slate-700 hover:bg-slate-100 transition-all"
        >
          Đăng ký
        </button>
        <button
          type="button"
          @click="router.push('/login')"
          class="px-3.5 py-1.5 text-sm font-semibold rounded-xl text-[#0E4D92] border-2 border-[#0E4D92] hover:bg-blue-50 transition-all active:scale-[0.98]"
        >
          Đăng nhập
        </button>
      </div>

    </header>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col items-center justify-center px-4 py-4 lg:py-5">
          <!-- Hero -->
      <div class="text-center mb-5 max-w-xl">
        <h1 class="text-2xl md:text-3xl font-bold text-slate-800 mb-2 leading-tight">
            Bạn đang sử dụng hệ thống<br />với vai trò nào?
        </h1>
        <p class="text-slate-500 text-base">
          Chọn vai trò để truy cập các chức năng phù hợp với bạn.
        </p>
      </div>

      <!-- Role Cards Grid -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 w-full max-w-2xl">
        <div
          v-for="item in roles"
          :key="item.role"
          @click="selectRole(item.role)"
          class="group bg-white border-2 border-slate-200 hover:border-[#0E4D92] rounded-2xl p-4 cursor-pointer transition-all duration-200 hover:shadow-xl hover:shadow-blue-100/60 flex flex-col gap-3 select-none"        >
          <div class="flex items-start justify-between">
            <div class="w-11 h-11 rounded-xl bg-slate-50 group-hover:bg-blue-50 flex items-center justify-center text-2xl transition-colors duration-200">
              {{ item.emoji }}
            </div>
            <span :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border', getTagClass(item.tagColor)]">
              {{ item.tag }}
            </span>
          </div>
          <div>
            <h3 class="font-bold text-lg text-slate-800 mb-1">
              {{ item.title }}
            </h3>
            <p class="text-sm text-slate-500 leading-relaxed mb-3">
              {{ item.desc }}
            </p>
            <ul class="space-y-1">
              <li v-for="f in item.features" :key="f" class="text-xs text-slate-500 flex items-center gap-2">
                <span class="text-[#00A878]">✓</span>
                {{ f }}
              </li>
            </ul>
          </div>
          <div class="w-full border-2 border-slate-200 group-hover:border-[#0E4D92] group-hover:bg-[#0E4D92] text-slate-500 group-hover:text-white rounded-xl py-2.5 text-sm font-semibold flex items-center justify-center gap-2 transition-all duration-200">
            Truy cập ➔
          </div>
        </div>
      </div>

      <!-- Trust Badges -->
    <div class="flex flex-wrap items-center justify-center gap-4 mt-5 text-xs text-slate-400">
        <span class="flex items-center gap-1.5">🔒 Bảo mật SSL</span>
        <span class="flex items-center gap-1.5">📱 Hỗ trợ mobile</span>
        <span class="flex items-center gap-1.5">⚡ Cập nhật thời gian thực</span>
      </div>
    </div>

    <footer class="text-center py-3 text-xs text-slate-400 border-t border-slate-200">
        © 2026 ClinicQueue · Hệ thống quản lý phòng khám · v1.0
    </footer>
  </div>
</template>
