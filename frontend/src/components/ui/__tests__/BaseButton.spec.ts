import { describe, expect, it } from 'vitest'
import { mount } from '@vue/test-utils'
import BaseButton from '../BaseButton.vue'

describe('BaseButton', () => {
  it('renders slot content', () => {
    const wrapper = mount(BaseButton, { slots: { default: 'Click me' } })
    expect(wrapper.text()).toContain('Click me')
  })

  it('emits click', async () => {
    const wrapper = mount(BaseButton, { slots: { default: 'Go' } })
    await wrapper.trigger('click')
    expect(wrapper.emitted('click')).toHaveLength(1)
  })

  it('is disabled while loading', () => {
    const wrapper = mount(BaseButton, { props: { loading: true } })
    expect(wrapper.attributes('disabled')).toBeDefined()
  })
})
