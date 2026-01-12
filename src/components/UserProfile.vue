<template>
    <div class="bg-white rounded-xl shadow-md p-6">
        <div class="flex items-center space-x-4">
            <img
                class="h-20 w-20 rounded-full object-cover border-2 border-indigo-500"
                :src="user.avatarUrl"
                alt="Avatar"
            >
            <div>
                <h2 class="text-xl font-bold text-gray-900">{{ user.fullName }}</h2>
                <p class="text-indigo-600 font-medium">@{{ user.username }}</p>
            </div>
        </div>

        <div class="mt-4">
            <p class="text-gray-600">{{ user.bio }}</p>
        </div>

        <div class="mt-6 flex justify-around border-t pt-4">
            <div class="text-center">
                <span class="block font-bold text-lg text-gray-800">{{ user.stats.posts }}</span>
                <span class="text-xs text-gray-500">Posts</span>
            </div>
            <div class="text-center">
                <span class="block font-bold text-lg text-gray-800">{{ user.stats.followers }}</span>
                <span class="text-xs text-gray-500">Followers</span>
            </div>
            <div class="text-center">
                <span class="block font-bold text-lg text-gray-800">{{ user.stats.following }}</span>
                <span class="text-xs text-gray-500">Following</span>
            </div>
        </div>
    </div>
</template>


<script setup lang="ts">
export interface User {
    username: string;
    fullName: string;
    avatarUrl: string;
    bio: string;
    stats: {
        followers: number;
        following: number;
        posts: number;
    };
}
defineProps<{
    user: User
}>();
</script>

