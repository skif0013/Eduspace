<template>
  <TheHeader/>

  <main>
    <router-view />
  </main>
</template>

<script setup lang="ts">
import TheHeader from "./components/Theheader.vue"
</script>