import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { usersApi } from '@/features/users/users.api'
import type { CreateUserInput, UpdateUserInput, User } from '@/features/users/users.types'

export const useUsersStore = defineStore('users', () => {
  const items = ref<User[]>([])
  const status = ref<'idle' | 'loading' | 'error'>('idle')
  const error = ref<string | null>(null)
  const mutating = ref(false)
  const search = ref('')

  const filtered = computed(() => {
    const q = search.value.trim().toLowerCase()
    if (!q) return items.value
    return items.value.filter((u) =>
      [u.name, u.username, u.email].some((v) => v.toLowerCase().includes(q)),
    )
  })

  async function fetchAll() {
    status.value = 'loading'
    error.value = null
    try {
      items.value = await usersApi.list()
      status.value = 'idle'
    } catch (e) {
      status.value = 'error'
      error.value = e instanceof Error ? e.message : 'error'
    }
  }

  async function create(input: CreateUserInput) {
    mutating.value = true
    try {
      const created = await usersApi.create(input)
      // jsonplaceholder returns id 11; ensure uniqueness for the demo list.
      items.value.unshift({ ...created, id: created.id || Date.now() })
    } finally {
      mutating.value = false
    }
  }

  async function update(id: number, input: UpdateUserInput) {
    mutating.value = true
    try {
      const updated = await usersApi.update(id, input)
      const idx = items.value.findIndex((u) => u.id === id)
      if (idx !== -1) items.value[idx] = { ...items.value[idx]!, ...updated }
    } finally {
      mutating.value = false
    }
  }

  async function remove(id: number) {
    await usersApi.remove(id)
    items.value = items.value.filter((u) => u.id !== id)
  }

  return {
    items,
    filtered,
    status,
    error,
    mutating,
    search,
    fetchAll,
    create,
    update,
    remove,
  }
})
