<template>
    <header class="sticky top-0 z-50 w-full py-6 bg-white/80 backdrop-blur-md border-b border-gray-100 transition-all select-none">
        <div class="container mx-auto px-4">

            <div class="relative flex items-center justify-center">

                <div
                    class="relative cursor-pointer py-2 z-20"
                    @mouseenter="isMenuOpen = true"
                    @mouseleave="isMenuOpen = false"
                >
                   <span class="bg-green-100 text-green-800 px-4 py-1 rounded-full text-2xl font-bold select-none">
                        Edu-space
                    </span>

                    <Transition
                        enter-active-class="transition duration-300 ease-out"
                        enter-from-class="transform scale-95 opacity-0 translate-y-2"
                        enter-to-class="transform scale-100 opacity-100 translate-y-0"
                        leave-active-class="transition duration-200 ease-in"
                        leave-from-class="transform scale-100 opacity-100 translate-y-0"
                        leave-to-class="transform scale-95 opacity-0 translate-y-2"
                    >
                        <div
                            v-if="isMenuOpen"
                            class="absolute top-full left-1/2 -translate-x-1/2 pt-4"
                        >
                            <nav
                                class="bg-white shadow-xl rounded-lg border border-gray-100 p-4 min-w-[200px]"
                            >
                                <ul class="flex flex-col p-1">
                                    <li>
                                        <a href="#" class="block px-4 py-2 rounded-lg text-gray-600 font-medium
                                       hover:bg-green-50 hover:text-green-700
                                       active:bg-green-100 transition-colors duration-200">
                                            Courses
                                        </a>
                                    </li>
                                    <li>
                                        <a href="#" class="block px-4 py-2 rounded-lg text-gray-600 font-medium
                                       hover:bg-green-50 hover:text-green-700
                                       active:bg-green-100 transition-colors duration-200">
                                            Mentors
                                        </a>
                                    </li>
                                    <li>
                                        <a href="#" class="block px-4 py-2 rounded-lg text-gray-600 font-medium
                                       hover:bg-green-50 hover:text-green-700
                                       active:bg-green-100 transition-colors duration-200">
                                            Pricing
                                        </a>
                                    </li>
                                </ul>
                            </nav>
                        </div>
                    </Transition>
                </div>

                <div class="absolute right-0 top-1/2 -translate-y-1/2 hidden md:block">
                    <button
                        class="bg-black text-white px-6 py-2 rounded-full font-medium
                        hover:bg-gray-800 hover:shadow-lg hover:-translate-y-0.5
                        active:scale-95 transition-all duration-200 ease-in-out"
                        @click="handleClick"
                    >
                        Get Started
                    </button>
                </div>

            </div>
        </div>
    </header>
</template>
<script setup lang="ts">
import "../style/style.css"
import {ref} from "vue";
const isMenuOpen = ref(false);
function handleClick() {
    console.log('clicked');
    if (isMenuOpen.value) {

    }
    console.log('clicked1');
}
</script>