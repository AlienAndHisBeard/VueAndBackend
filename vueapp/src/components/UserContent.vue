<script setup lang="js">
import UserContentItem from './UserContentItem.vue'
import DocumentationIcon from './icons/IconDocumentation.vue'
import ToolingIcon from './icons/IconTooling.vue'
import SupportIcon from './icons/IconSupport.vue'
</script>

<template>
  

  <UserContentItem v-if="user">
    <template #icon>
      <ToolingIcon />
    </template>
    <template #heading>Account</template>
    {{ user }}
  </UserContentItem>

  <UserContentItem v-if="user">
    <template #icon>
      <SupportIcon @click="fetchData()"/>
    </template>
    <template #heading>Your favourite bus stops</template>
    {{ busStops }}
  </UserContentItem>

  <UserContentItem>
    <template #icon>
      <DocumentationIcon />
    </template>
    <template #heading>Official ZTM site</template>

    Ztm’s
    <a href="https://ztm.gda.pl/" target="_blank" rel="noopener">official site</a>
    provides you with all Gdańsk public transport information you need.
  </UserContentItem>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import VueJwtDecode from "vue-jwt-decode";

    export default defineComponent({
        data() {
            return {
                loading: false,
                busStops: null,
                user: null,
                list: [],
                timer: ''
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData',
            '$store.state.key': 'fetchData'
        },
        methods: {
            fetchData() {
                let userToken = localStorage.getItem("user");
                if (userToken == null) return;
                this.busStops = null;
                this.loading = true;
                this.user = VueJwtDecode.decode(userToken).sub;
                const requestOptions = {
                    method: "GET",
                    headers: { "Content-Type": "application/json", "Authorization": "Bearer " + userToken }
                };
                fetch('/api/Users/BusStops', requestOptions)
                    .then(r => r.json())
                    .then(json => {
                        this.busStops = json;
                        this.loading = false;
                        return;
                    });
            }
        }
    });
</script>