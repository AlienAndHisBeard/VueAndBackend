<template>
    <main>

    
    <div class="post" >
        <div v-if="loading" class="loading">
            Loading...
        </div>

        <div v-else-if="userId>0">

        </div>

        <div v-else style="display:inline-flexbox; flex-direction:column!; width: 200px; height: 400px; align-content: center;">
            <p v-if="error">{{ error }}</p>
            <p>Enter login: </p>
            <input v-model="username" placeholder="username">
            <p>Enter password:</p>
            <input v-model="password" placeholder="password">
            <button @click="tryLogin" style="width: 100px;height: 30px; align-self: center; margin-left: 20%; margin-top: 3%;">Login</button>
            <button @click="tryRegister" style="width: 100px;height: 30px; align-self: center; margin-left: 20%; margin-top: 3%;">Register</button>
        </div>
        
    </div>
</main>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        data() {
            return {
                loading: false,
                post: null,
                username: null,
                password: null,
                error: null
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
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            if ( this.$store.state.userId )
            {
                sessionStorage.clear();
                this.$store.commit('updateUserId', null)
                sessionStorage.clear();
            }
        },
        methods: {
            tryLogin() {
                this.post = null;
                this.loading = true;
                if (this.username == null || this.password == null)
                {
                    this.error = "You need to fill out the credentials first.";
                    this.loading = false;
                    return;
                }
                const requestOptions = {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({id:0, login: this.username, password: this.password, busStops:"" })
                };
                fetch('/api/Users/login', requestOptions)
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        this.error = "Succesfully logged in";
                        this.userId = json['id'];
                        this.$router.push({ path: "/" });
                        return;
                    });
            },
            tryRegister() {
                this.post = null;
                this.loading = true;
                if (this.username == null || this.password == null)
                {
                    this.error = "You need to fill out the credentials first.";
                    this.loading = false;
                    return;
                }
                const requestOptions = {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({id:0, login: this.username, password: this.password, busStops:"" })
                };
                fetch('/api/Users', requestOptions)
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        this.error = "Succesfully logged in";
                        this.userId = json['id'];
                        this.$router.push({ path: "/" });
                        return;
                    });
            }
        },
    });
</script>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>