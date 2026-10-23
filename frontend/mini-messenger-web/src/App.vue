
<script setup>
import { ref } from 'vue'
import { login } from './lib/auth.api'

const result = ref('')

async function testApi() {
  try {
    const { data } = await login({ username: 'alex', password: 'secret123' })
    result.value = 'OK : connecté en tant que ' + data.username
  } catch (e) {
    result.value = 'Erreur : ' + (e.response?.status ?? e.message)
  }
}
</script>

<template>
  <button @click="testApi">Tester l'API</button>
  <p>{{ result }}</p>
</template>