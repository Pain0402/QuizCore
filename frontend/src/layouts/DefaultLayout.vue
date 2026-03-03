<template>
  <div class="app-layout">
    <AppSidebar />
    <div class="main-content">
      <AppTopbar :title="pageTitle" />
      <main class="page-container fade-in">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from '@/components/layout/AppSidebar.vue'
import AppTopbar from '@/components/layout/AppTopbar.vue'

const route = useRoute()
const pageTitle = computed(() => route.meta.title || 'QuizCore')
</script>
