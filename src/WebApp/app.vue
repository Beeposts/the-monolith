<template>
  <div>
   
    <button @click="test()">AAA</button>
  </div>
</template>



<script setup lang="ts">
function test() {
  console.log('test')
  let aaa = 123
  let bbb  = aaa + 1
  console.log(bbb)
}
</script>
