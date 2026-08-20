<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { supportedLocales, setLocale } from '@/lib/i18n'

const { locale } = useI18n()
const router = useRouter()
const auth = useAuthStore()

function onLocaleChange(event: Event) {
  const value = (event.target as HTMLSelectElement).value
  setLocale(value)
}

function logout() {
  auth.logout()
  router.push('/login')
}

</script>

<template>
  <div class="flex min-h-screen bg-slate-50">

    <!-- ================================================= -->
    <!-- SIDEBAR -->
    <!-- ================================================= -->
    <aside
      class="flex h-screen w-64 shrink-0 flex-col
             border-r border-slate-200 bg-white"
    >

      <!-- Logo -->
      <div
        class="flex h-16 items-center
               border-b border-slate-200 px-5"
      >
        <div
          class="flex h-9 w-9 items-center justify-center
                 rounded-lg bg-violet-600
                 text-sm font-bold text-white"
        >
          C
        </div>

        <div class="ml-3">
          <div class="text-sm font-bold text-slate-800">
            Clinic
          </div>

          <div class="mt-0.5 text-xs text-slate-400">
            <template v-if="auth.hasRole(['Receptionist'])">
              Lễ tân
            </template>

            <template v-else-if="auth.hasRole(['Admin'])">
              Quản trị hệ thống
            </template>

            <template v-else-if="auth.hasRole(['Doctor'])">
              Bác sĩ
            </template>

            <template v-else-if="auth.hasRole(['Patient'])">
              Bệnh nhân
            </template>

            <template v-else>
              Clinic System
            </template>
          </div>
        </div>
      </div>


      <!-- ================================================= -->
      <!-- MENU -->
      <!-- ================================================= -->
      <nav class="flex-1 overflow-y-auto p-4">

        <!-- ================= RECEPTIONIST ================= -->
        <template v-if="auth.hasRole(['Receptionist'])">

          <div
            class="mb-2 px-3 text-xs font-semibold
                   uppercase tracking-wider text-slate-400"
          >
          </div>

          <div class="space-y-1">

            <!-- Tổng quan -->
            <RouterLink
              to="/reception"
              class="flex items-center gap-3
                     rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              exact-active-class="bg-violet-50 text-violet-700"
            >
              <!-- Grid icon -->
              <svg
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
              >
                <rect x="3" y="3" width="7" height="7" />
                <rect x="14" y="3" width="7" height="7" />
                <rect x="3" y="14" width="7" height="7" />
                <rect x="14" y="14" width="7" height="7" />
              </svg>

              <span>Tổng quan</span>
            </RouterLink>


            <!-- Quản lý hàng đợi -->
            <RouterLink
              to="/reception/queue"
              class="flex items-center gap-3
                     rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              <!-- Users icon -->
              <svg
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
              >
                <path
                  d="M16 21v-2a4 4 0 0 0-4-4H6
                     a4 4 0 0 0-4 4v2"
                />
                <circle
                  cx="9"
                  cy="7"
                  r="4"
                />

                <path
                  d="M22 21v-2a4 4 0 0 0-3-3.87"
                />

                <path
                  d="M16 3.13a4 4 0 0 1 0 7.75"
                />
              </svg>

              <span>Quản lý hàng đợi</span>
            </RouterLink>


            <!-- Check-in -->
            <RouterLink
              to="/reception/checkin"
              class="flex items-center gap-3
                     rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              <!-- Badge icon -->
              <svg
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
              >
                <rect
                  x="3"
                  y="4"
                  width="18"
                  height="16"
                  rx="2"
                />

                <circle
                  cx="8"
                  cy="10"
                  r="2"
                />

                <path d="M12 9h5" />
                <path d="M12 13h5" />
              </svg>

              <span>Check-in bệnh nhân</span>
            </RouterLink>

          </div>
        </template>


        <!-- ================= ADMIN ================= -->
        <template v-else-if="auth.hasRole(['Admin'])">

          <div
            class="mb-2 px-3 text-xs font-semibold
                   uppercase tracking-wider text-slate-400"
          >
          </div>

          <div class="space-y-1">

            <RouterLink
              to="/admin"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              exact-active-class="bg-violet-50 text-violet-700"
            >
              Tổng quan
            </RouterLink>

            <RouterLink
              to="/admin/accounts"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Quản lý tài khoản
            </RouterLink>

            <RouterLink
              to="/admin/doctors"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Quản lý bác sĩ
            </RouterLink>

            <RouterLink
              to="/admin/specialties"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Chuyên khoa
            </RouterLink>

            <RouterLink
              to="/admin/schedule"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Lịch làm việc
            </RouterLink>

            <RouterLink
              to="/admin/reports"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Báo cáo
            </RouterLink>

            <RouterLink
              to="/admin/settings"
              class="block rounded-lg px-3 py-2.5
                     text-sm font-medium
                     text-slate-600
                     transition-colors
                     hover:bg-slate-50"
              active-class="bg-violet-50 text-violet-700"
            >
              Cài đặt
            </RouterLink>

          </div>
        </template>


        <!-- ================= DOCTOR ================= -->
        <template v-else-if="auth.hasRole(['Doctor'])">

          <div
            class="mb-2 px-3 text-xs font-semibold
                   uppercase tracking-wider text-slate-400"
          >
            Bác sĩ
          </div>

          <div class="space-y-1">
            <!-- Menu bác sĩ thêm sau -->
          </div>

        </template>


        <!-- ================= PATIENT ================= -->
        <template v-else-if="auth.hasRole(['Patient'])">

          <div
            class="mb-2 px-3 text-xs font-semibold
                   uppercase tracking-wider text-slate-400"
          >
            Bệnh nhân
          </div>

          <div class="space-y-1">
            <!-- Menu bệnh nhân thêm sau -->
          </div>

        </template>

      </nav>

    </aside>


    <!-- ================================================= -->
    <!-- MAIN CONTENT -->
    <!-- ================================================= -->
    <div class="flex min-w-0 flex-1 flex-col">

      <!-- ================= HEADER ================= -->
      <header
        class="flex h-16 shrink-0 items-center
               justify-end gap-4
               border-b border-slate-200
               bg-white px-6"
      >

        <!-- Language -->
        <select
          aria-label="Language"
          :value="locale"
          class="h-9 rounded-lg
                 border border-slate-200
                 bg-white px-2
                 text-sm text-slate-600
                 outline-none
                 focus:border-violet-500"
          @change="onLocaleChange"
        >
          <option
            v-for="language in supportedLocales"
            :key="language"
            :value="language"
          >
            {{ language.toUpperCase() }}
          </option>
        </select>


          <!-- USER -->
          <div
            v-if="auth.user"
            class="flex items-center gap-3"
          >

            <!-- Avatar -->
            <div
              class="flex h-9 w-9 shrink-0
                    items-center justify-center
                    rounded-full
                    bg-violet-100
                    text-sm font-semibold
                    text-violet-700"
            >
              {{ auth.user.name?.charAt(0)?.toUpperCase() || 'U' }}
            </div>

            <!-- Name + role -->
            <div class="min-w-0">

              <div
                class="max-w-32 truncate
                      text-sm font-medium
                      text-slate-800"
              >
                {{ auth.user.name || 'Demo User' }}
              </div>

              <div class="text-xs text-slate-400">
                <template v-if="auth.hasRole(['Receptionist'])">
                  Lễ tân
                </template>

                <template v-else-if="auth.hasRole(['Admin'])">
                  Quản trị viên
                </template>

                <template v-else-if="auth.hasRole(['Doctor'])">
                  Bác sĩ
                </template>

                <template v-else-if="auth.hasRole(['Patient'])">
                  Bệnh nhân
                </template>
              </div>

            </div>

          </div>


        <!-- Logout -->
        <button
          type="button"
          class="text-sm font-medium
                 text-slate-500
                 transition-colors
                 hover:text-red-600"
          @click="logout"
        >
          Đăng xuất
        </button>

      </header>


      <!-- ================= PAGE CONTENT ================= -->
      <main class="min-h-0 flex-1 overflow-y-auto p-6">
        <RouterView />
      </main>

    </div>

  </div>
</template>
