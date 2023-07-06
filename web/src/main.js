import Vue from 'vue'

import Cookies from 'js-cookie'

import 'normalize.css/normalize.css' // a modern alternative to CSS resets

import Element from 'element-ui'
import './styles/element-variables.scss'

import '@/styles/index.scss' // global css
import '@/assets/icon/iconfont.css'
import 'vue-select/dist/vue-select.css'
import 'vue-select/src/scss/vue-select.scss'
import App from './App'
import store from './store'
import router from './router'
import vSelect from 'vue-select'
import './icons' // icon
import './permission' // permission control
import './utils/error-log' // error log
import i18n from './lang' // internationalization

import * as filters from './filters' // global filters
import Print from 'vue-print-nb'
// 需要按需引入，先引入vue并引入element-ui
import AFTableColumn from 'af-table-column'
// import BarTenderPrintClient from './views/common/BarTenderPrintClient'
import 'vue-select/dist/vue-select.css'

import dataV from '@jiaminghi/data-view'
Vue.use(dataV)

Vue.component('v-select', vSelect)
Vue.use(Print)
Vue.use(AFTableColumn)
Vue.component('v-select', vSelect)

import simpleEventBus from './assets/libs/simpleEventBus.js'
import elCascaderMulti from 'el-cascader-multi'
Vue.use(elCascaderMulti)

Vue.use(Element, {
  size: Cookies.get('size') || 'medium', // set element-ui default s
  i18n: (key, value) => i18n.t(key, value)
})
// Vue.use(Print) // 注册
// register global utility filters
Object.keys(filters).forEach(key => {
  Vue.filter(key, filters[key])
})

Vue.config.productionTip = false
Vue.prototype.SimpleEventBus = simpleEventBus
// 设置全局变量Task，EmptyTask属性默认为false。 true：有任务，false：无任务
Vue.prototype.Task = { EmptyTask: false }

new Vue({
  el: '#app',
  router,
  store,
  i18n,
  render: h => h(App)
})
