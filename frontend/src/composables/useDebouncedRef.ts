import { customRef } from 'vue'

/** A ref whose writes are debounced by `delay` ms. */
export function useDebouncedRef<T>(initial: T, delay = 300) {
  let timeout: ReturnType<typeof setTimeout>
  return customRef<T>((track, trigger) => {
    let value = initial
    return {
      get() {
        track()
        return value
      },
      set(newValue) {
        clearTimeout(timeout)
        timeout = setTimeout(() => {
          value = newValue
          trigger()
        }, delay)
      },
    }
  })
}
