<template>
    <TheHeader />

    <main class="min-h-screen bg-gray-100">
        <router-view />
    </main>
</template>

<script setup lang="ts">
import TheHeader from "./components/Theheader.vue"
</script>