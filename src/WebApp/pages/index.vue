<script setup>
const { loggedIn, login, logout, user } = useOidcAuth()
</script>

<template>
  <div v-if="loggedIn">
    {{ user.providerInfo.name }}
    <button @click="() => logout('oidc')">Sair</button>
  </div>
  <div v-else>
    <button @click="() => login('oidc')">Loga ai porra</button>
  </div>
</template>

<style scoped>

</style>