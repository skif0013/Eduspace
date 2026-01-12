<script setup lang="ts">
import UserProfile from '../components/UserProfile.vue';

const currentUser = {
    username: "real_user_123",
    fullName: "Иван Иванов",
    avatarUrl: "https://i.pravatar.cc/150?u=a042581f4e29026704d",
    bio: "Разработчик на Vue 3 + TypeScript",
    stats: {
        followers: 500,
        following: 100,
        posts: 10
    }
};
</script>

<template>
    <div class="p-10">
        <h1 class="text-2xl font-bold mb-6">Мой профиль</h1>
        <UserProfile :user="currentUser" />
    </div>
</template>