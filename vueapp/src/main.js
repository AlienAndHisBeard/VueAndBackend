import './assets/main.css'

import { createApp } from 'vue';
import { createStore } from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import VueGoodTablePlugin from 'vue-good-table-next';
import router from './router'
import App from './App.vue';

import 'vue-good-table-next/dist/vue-good-table-next.css';

const store = createStore({
    plugins: [createPersistedState({
        storage: window.sessionStorage,
    })],
    state () {
        return {
            userId: null
        }
    },
    mutations: {
        updateUserId(state, id) {
            state.userId = id;
        }
    }
})

var app = createApp(App);
app.use(VueGoodTablePlugin);
app.use(store);
app.use(router);



app.mount('#app');
