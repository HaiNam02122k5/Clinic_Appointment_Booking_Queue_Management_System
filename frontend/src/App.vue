<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <!-- Layout is applied per-route (see router) so /login can render full-screen. -->
  <RouterView />
</template>
