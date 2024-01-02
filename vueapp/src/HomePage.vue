<script setup>
import BusStopsContent from './components/BusStopsContent.vue'
import UserContent from './components/UserContent.vue'
</script>

<template>
  <header>
    <div class="wrapper">
      <BusStopsContent msg="You did it!" />
    </div>
  </header>

  <main >
    <UserContent />
  </main>
</template>

<script>
</script>

<style scoped>
header {
  line-height: 1.5;
  height: 80%;
}

@media (min-width: 1180px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
    width: 57%;
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

  main {
    width: 40%;
    align-self:flex-end;
    height: 85vh;
  }
}
</style>