<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseCard from '@/components/ui/BaseCard.vue'

const { t } = useI18n()
const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

const email = ref('demo@example.com')
const password = ref('password')

async function onSubmit() {
  try {
    await auth.login({ email: email.value, password: password.value })
    const redirect = (route.query.redirect as string) || '/'
    router.replace(redirect)
  } catch {
    /* error surfaced via store */
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center p-4">
    <BaseCard>
      <form class="flex w-80 flex-col gap-4 p-6" @submit.prevent="onSubmit">
        <div>
          <h1 class="text-xl font-semibold">{{ t('auth.login') }}</h1>
          <p class="mt-1 text-sm text-gray-500">{{ t('auth.signInToContinue') }}</p>
        </div>
        <BaseInput v-model="email" :label="t('auth.email')" type="email" required />
        <BaseInput v-model="password" :label="t('auth.password')" type="password" required />
        <p v-if="auth.error" class="text-sm text-red-600">
          {{ t('auth.invalidCredentials') }}
        </p>
        <BaseButton type="submit" :loading="auth.status === 'loading'">
          {{ t('auth.login') }}
        </BaseButton>
        <p class="text-center text-xs text-gray-400">demo@example.com / password</p>
      </form>
    </BaseCard>
  </div>
</template>
