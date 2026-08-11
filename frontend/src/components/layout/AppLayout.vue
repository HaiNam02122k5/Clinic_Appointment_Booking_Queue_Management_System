<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { supportedLocales, setLocale } from '@/lib/i18n'
import BaseButton from '@/components/ui/BaseButton.vue'

const { t, locale } = useI18n()
const router = useRouter()
const auth = useAuthStore()

function onLocaleChange(e: Event) {
  setLocale((e.target as HTMLSelectElement).value)
}

function logout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="flex min-h-screen bg-gray-50 dark:bg-gray-950">

    <!-- ================= SIDEBAR ================= -->
    <aside
      class="flex h-screen w-64 shrink-0 flex-col
             border-r border-gray-200 bg-white
             dark:border-gray-800 dark:bg-gray-900"
    >

      <!-- Logo -->
      <div class="border-b border-gray-200 px-5 py-5 dark:border-gray-800">
        <div class="text-lg font-bold text-brand-600">
          {{ t('app.name') }}
        </div>

        <div class="mt-1 text-xs text-gray-400">
          <template v-if="auth.hasRole(['Admin'])">
            Quản trị hệ thống
          </template>

          <template v-else-if="auth.hasRole(['Receptionist'])">
            Lễ tân
          </template>

          <template v-else-if="auth.hasRole(['Doctor'])">
            Bác sĩ
          </template>

          <template v-else-if="auth.hasRole(['Patient'])">
            Bệnh nhân
          </template>
        </div>
      </div>

      <!-- MENU -->
      <nav class="flex-1 overflow-y-auto p-3">

        <!-- ADMIN -->
        <template v-if="auth.hasRole(['Admin'])">
          <div class="space-y-1">

            <RouterLink
              to="/admin"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Tổng quan
            </RouterLink>

            <RouterLink
              to="/admin/accounts"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Quản lý tài khoản
            </RouterLink>

            <RouterLink
              to="/admin/doctors"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Quản lý bác sĩ
            </RouterLink>

            <RouterLink
              to="/admin/specialties"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Chuyên khoa
            </RouterLink>

            <RouterLink
              to="/admin/schedule"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Lịch làm việc
            </RouterLink>

            <RouterLink
              to="/admin/reports"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Báo cáo
            </RouterLink>

            <RouterLink
              to="/admin/settings"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Cài đặt
            </RouterLink>

          </div>
        </template>

        <!-- RECEPTIONIST -->
        <template v-else-if="auth.hasRole(['Receptionist'])">
          <div class="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-gray-400">
            Lễ tân
          </div>

          <div class="space-y-1">

            <RouterLink
              to="/reception/queue"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Bảng điều khiển
            </RouterLink>

            <RouterLink
              to="/reception/checkin"
              class="block rounded-lg px-3 py-2.5 text-sm font-medium
                     text-gray-600 hover:bg-gray-100
                     dark:text-gray-300 dark:hover:bg-gray-800"
              active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >
              Check-in bệnh nhân
            </RouterLink>

          </div>
        </template>

        <!-- DOCTOR -->
        <template v-else-if="auth.hasRole(['Doctor'])">
          <div class="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-gray-400">
            Bác sĩ
          </div>

          <!-- Menu bác sĩ thêm sau -->
        </template>

        <!-- PATIENT -->
        <template v-else-if="auth.hasRole(['Patient'])">
          <div class="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-gray-400">
            Bệnh nhân
          </div>

          <!-- Menu bệnh nhân thêm sau -->
        </template>

      </nav>
    </aside>


    <!-- ================= PHẦN BÊN PHẢI ================= -->
    <div class="flex min-w-0 flex-1 flex-col">

      <!-- HEADER -->
      <header
        class="flex h-16 shrink-0 items-center justify-end gap-4
               border-b border-gray-200 bg-white px-6
               dark:border-gray-800 dark:bg-gray-900"
      >

        <!-- Language -->
        <select
          aria-label="Language"
          :value="locale"
          class="h-9 rounded-lg border border-gray-300 bg-white px-2 text-sm
                 dark:border-gray-700 dark:bg-gray-900"
          @change="onLocaleChange"
        >
          <option
            v-for="l in supportedLocales"
            :key="l"
            :value="l"
          >
            {{ l.toUpperCase() }}
          </option>
        </select>

        <!-- User -->
        <span
          v-if="auth.user"
          class="text-sm text-gray-500"
        >
          {{ auth.user.name }}
        </span>

        <!-- Logout -->
        <BaseButton
          variant="ghost"
          size="sm"
          @click="logout"
        >
          {{ t('nav.logout') }}
        </BaseButton>

      </header>


      <!-- CONTENT -->
      <main class="flex-1 overflow-y-auto p-6">
        <RouterView />
      </main>

    </div>

  </div>
</template>
