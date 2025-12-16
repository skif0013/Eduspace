<template>
    <section class="w-full py-12 md:py-24 bg-white overflow-hidden select-none">
        <div class="container mx-auto px-4">

            <div class="grid grid-cols-1 md:grid-cols-2 gap-12 items-center">

                <div class="space-y-6">
                    <div class="inline-block bg-green-100 text-green-700 px-4 py-1.5 rounded-full text-sm font-semibold">
                        🎓 #1 Education Platform
                    </div>

                    <h1 class="text-4xl md:text-6xl font-bold text-gray-900 leading-tight">
                        Grow your skills <br>
                        <span class="text-transparent bg-clip-text bg-gradient-to-r from-green-600 to-teal-500">
                            with expert mentors
                        </span>
                    </h1>

                    <p class="text-lg text-gray-600 max-w-lg">
                        Find the perfect mentor or course to boost your career.
                        Access over 1,000+ courses in design and development.
                    </p>

                    <div class="flex flex-col sm:flex-row gap-4 pt-4">
                        <button class="bg-black text-white px-8 py-3 rounded-full font-medium text-lg hover:bg-gray-800 transition active:scale-95">
                            Start Learning
                        </button>
                        <button class="px-8 py-3 rounded-full font-medium text-lg border border-gray-200 hover:border-gray-900 transition active:scale-95">
                            Learn More
                        </button>
                    </div>

                    <div class="flex gap-8 pt-6 border-t border-gray-100 mt-8">
                        <div>
                            <p class="text-2xl font-bold text-gray-900">12390123</p>
                            <p class="text-sm text-gray-500">Active Students</p>
                        </div>
                        <div>
                            <p class="text-2xl font-bold text-gray-900">123123123</p>
                            <p class="text-sm text-gray-500">Expert Mentors</p>
                        </div>
                    </div>
                </div>

                <div class="relative">
                    <div class="absolute -top-10 -right-10 w-64 h-64 bg-green-200 rounded-full blur-3xl opacity-30"></div>

                    <img
                        src="https://images.unsplash.com/photo-1522202176988-66273c2fd55f?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"
                        alt="Students learning"
                        class="relative rounded-3xl shadow-2xl z-10 w-full object-cover transform hover:scale-[1.02] transition duration-500"
                    />
                </div>

            </div>
        </div>
    </section>
</template>
<script setup lang="ts">

</script>