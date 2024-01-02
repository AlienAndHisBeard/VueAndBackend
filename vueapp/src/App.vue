<template>
  <nav>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="35" height="35" />
    <router-link to="/">Home</router-link> |
    <router-link to="/Logout" v-if="userId">Logout</router-link>
    <router-link to="/Login" v-else>Login/register</router-link>
  </nav>
  <router-view/>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        data() {
            return {
            };
        },
        computed: {
            userId: {
                get () {
                    return this.$store.state.userId
                },
                set (value) {
                    this.$store.commit('updateUserId', value)
                }
            }
        },
    });
</script>