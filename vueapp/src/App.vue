<template>
  <nav>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="35" height="35" />
    <router-link to="/">Home</router-link> |
    <router-link to="/Logout" v-if="user">Logout</router-link>
    <router-link to="/Login" v-else>Login/register</router-link>
  </nav>
  <router-view/>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        data() {
            return {
                user: localStorage.getItem("user")
            };
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            fetchData() {
                this.user = localStorage.getItem("user");
            }
        },
    });
</script>