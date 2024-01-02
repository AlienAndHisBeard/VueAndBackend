<script setup lang="js">
import UserContentItem from './UserContentItem.vue'
import DocumentationIcon from './icons/IconDocumentation.vue'
import ToolingIcon from './icons/IconTooling.vue'
import SupportIcon from './icons/IconSupport.vue'
</script>

<template>
  

  <UserContentItem v-if="userId">
    <template #icon>
      <ToolingIcon />
    </template>
    <template #heading>Account</template>
    {{ userId }}
  </UserContentItem>

  <UserContentItem v-if="userId">
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

    export default defineComponent({
        data() {
            return {
                loading: false,
                busStops: null
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        computed: {
            userId: {
                get () {
                    return this.$store.state.userId
                },
            }
        },
        methods: {
          fetchData() {
            if (this.$store.state.userId == null) return;
            this.busStops = null;
            this.loading = true;
            fetch('/api/Users/'+this.$store.state.userId+'/BusStops')
                .then(r => r.json())
                .then(json => {
                    this.busStops = json;
                    this.loading = false;
                    return;
                });
          }
        },
    });
</script>