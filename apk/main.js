import Vue from 'vue'
import App from './App'
import api from './config.js'

Vue.config.productionTip = false
Vue.prototype.api = api
App.mpType = 'app'
Vue.prototype.apiUrl = 'http://localhost:40005/api/';  
Vue.prototype.now = Date.now || function () {  
    return new Date().getTime();  
};  
Vue.prototype.isArray = Array.isArray || function (obj) {  
    return obj instanceof Array;  
};


const app = new Vue({
    ...App
})
app.$mount()
