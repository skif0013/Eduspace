<template>
  <div class="min-h-screen flex items-center justify-center bg-[#FDFDFD] px-4 py-12">

    <div class="w-full max-w-md bg-white rounded-3xl shadow-2xl border border-gray-100 overflow-hidden">

      <div class="bg-gray-50 px-8 py-6 text-center border-b border-gray-100">
                <span class="bg-green-100 text-green-800 px-3 py-1 rounded-full text-xl font-bold">
                    Edu-space
                </span>
        <h2 class="mt-4 text-2xl font-bold text-gray-900">
          {{ isLogin ? 'Welcome Back!' : 'Create Account' }}
        </h2>
        <p class="text-gray-500 text-sm mt-1">
          {{ isLogin ? 'Enter your details to access your courses' : 'Start your learning journey today' }}
        </p>
      </div>

      <div class="p-8">
        <form @submit.prevent="handleSubmit" class="space-y-5">

          <div v-if="!isLogin" class="animate-fade-in-down">
            <label class="label-text">Full Name</label>
            <input
                type="text"
                placeholder="Alex Smith"
                class="input-primary"
            >
          </div>

          <div>
            <label class="label-text">Email Address</label>
            <input
                type="email"
                placeholder="alex@example.com"
                class="input-primary"
            >
          </div>

          <div>
            <label class="label-text">Password</label>
            <input
                type="password"
                placeholder="••••••••"
                class="input-primary"
            >
          </div>

          <button type="submit" class="btn-primary mt-6">
            {{ isLogin ? 'Sign In' : 'Sign Up' }}
          </button>
        </form>

        <div class="mt-6 text-center text-sm text-gray-600">
          {{ isLogin ? "Don't have an account?" : "Already have an account?" }}
          <span @click="toggleMode" class="link-toggle ml-1">
                        {{ isLogin ? 'Sign Up' : 'Log In' }}
                    </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const isLogin = ref(true);

function toggleMode() {
  isLogin.value = !isLogin.value;
}

function handleSubmit() {
  if (isLogin.value) {
    console.log('Логика входа...');
  } else {
    console.log('Логика регистрации...');
  }
}
</script>

<style scoped>
/* Простая анимация появления поля имени */
.animate-fade-in-down {
  animation: fadeInDown 0.3s ease-out;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>