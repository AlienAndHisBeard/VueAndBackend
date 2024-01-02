import './assets/main.css'

import { createApp } from 'vue';
import { createStore } from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import VueGoodTablePlugin from 'vue-good-table-next';
import router from './router'
import App from './App.vue';

import 'vue-good-table-next/dist/vue-good-table-next.css';

const store = createStore({
    state () {
        return {
            key: 0
        }
    },
    mutations: {
        updateKey(state) {
            state.key++;
        }
    }
})

var app = createApp(App);
app.use(VueGoodTablePlugin);
app.use(store);
router.beforeEach((to, from) => {
    if (from.fullPath == "/Logout" || from.fullPath == "/Login") {
        
    }
})
app.use(router);



app.mount('#app');
