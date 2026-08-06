import { reactive, ref } from 'vue'

export function useFormValidation<T extends Record<string, any>>(initialValues: T) {
  const formData = reactive<T>({ ...initialValues })
  const errors = reactive<Record<string, string>>({})
  const touched = reactive<Record<string, boolean>>({})
  const isSubmitting = ref(false)

  function touch(field: keyof T) {
    touched[field as string] = true
  }

  function setError(field: keyof T, message: string) {
    errors[field as string] = message
  }

  function clearError(field: keyof T) {
    errors[field as string] = ''
  }

  function clearAllErrors() {
    Object.keys(errors).forEach((key) => {
      errors[key] = ''
    })
  }

  return {
    formData,
    errors,
    touched,
    isSubmitting,
    touch,
    setError,
    clearError,
    clearAllErrors,
  }
}