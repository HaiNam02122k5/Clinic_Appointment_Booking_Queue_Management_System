<script setup lang="ts">
import { reactive, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseCard from '@/components/ui/BaseCard.vue'
import type { CreateUserInput, User } from './users.types'

const props = defineProps<{
  open: boolean
  initial?: User | null
  submitting?: boolean
}>()

const emit = defineEmits<{
  close: []
  submit: [value: CreateUserInput]
}>()

const { t } = useI18n()
const form = reactive<CreateUserInput>({ name: '', username: '', email: '' })

watch(
  () => [props.open, props.initial],
  () => {
    form.name = props.initial?.name ?? ''
    form.username = props.initial?.username ?? ''
    form.email = props.initial?.email ?? ''
  },
  { immediate: true },
)
</script>

<template>
  <div v-if="open" class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
    <BaseCard>
      <form
        class="flex w-96 max-w-full flex-col gap-4 p-6"
        @submit.prevent="emit('submit', { ...form })"
      >
        <h2 class="text-lg font-semibold">
          {{ initial ? t('users.edit') : t('users.create') }}
        </h2>
        <BaseInput v-model="form.name" :label="t('users.name')" required />
        <BaseInput v-model="form.username" :label="t('users.username')" required />
        <BaseInput v-model="form.email" :label="t('users.email')" type="email" required />
        <div class="flex justify-end gap-2">
          <BaseButton type="button" variant="secondary" @click="emit('close')">
            {{ t('users.cancel') }}
          </BaseButton>
          <BaseButton type="submit" :loading="submitting">
            {{ t('users.save') }}
          </BaseButton>
        </div>
      </form>
    </BaseCard>
  </div>
</template>
