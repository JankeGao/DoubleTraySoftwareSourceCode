<template>
  <!-- left: component.style.position.x + 'px',
          top: component.style.position.y + 'px',
          width: component.style.position.w + 'px',
          height: component.style.position.h + 'px',
          backgroundColor: component.style.backColor,
          zIndex: component.style.zIndex,
          borderWidth: component.style.borderWidth + 'px',
          borderStyle: component.style.borderStyle,
          borderColor: component.style.borderColor,
  transform: component.style.transform? `rotate(${component.style.transform}deg)`:'rotate(0deg)',-->
  <div v-if="configData.layer" class="topo-render" :style="layerStyle">
    <template v-for="(component,index) in configData.components">
      <div
        v-show="component.style.visible == undefined? true:component.style.visible"
        :key="index"
        class="topo-render-wrapper"
        :class="{'topo-render-wrapper-clickable': component.action.length > 0 }"
        :style="{


                        left: component.style.position.x + 'px',
              top: component.style.position.y + 'px',
              width: component.style.position.w + 'px',
              height: component.style.position.h + 'px',
              'background-color':   component.style.backColor,
              zIndex: component.style.zIndex,
              borderWidth: component.style.borderWidth + 'px',
              borderStyle: component.style.borderStyle,
              borderColor: component.style.borderColor,
              'border-width':component.style.borderWidth + 'px',
              transform: component.style.transform? `rotate(${component.style.transform}deg)`:'rotate(0deg)',
        }"
        @click="doClickComponent(component)"
        @dblclick="doDbClickComponent(component)"
      >
        <component :is="parseView(component)" ref="spirit" :detail="component" />
      </div>
    </template>
  </div>
</template>

<script>
import ViewText from './control/ViewText'
import ViewImage from './control/ViewImage'
import ViewCircular from './control/canvas/ViewCircular'
import ViewLine from './control/canvas/ViewLine'
import ViewLineArrow from './control/canvas/ViewLineArrow'
import ViewLineWave from './control/canvas/ViewLineWave'
import ViewRect from './control/canvas/ViewRect'
import ViewTriangle from './control/canvas/ViewTriangle'

import ViewChart from './control/chart/ViewChart'
import ViewChartPie from './control/chart/ViewChartPie'
import ViewChartGauge from './control/chart/ViewChartGauge'

import ViewSvgImage from './control/svg/ViewSvgImage'

import topoUtil from './util/topo-util'
import { getTrayLayoutById } from '@/api/WareHouse'

import { getLocationStockByLayoutId } from '@/api/stock'

export default {
  name: 'TopoRender',
  components: {
    ViewText,
    ViewImage,
    ViewCircular,
    ViewLine,
    ViewLineArrow,
    ViewLineWave,
    ViewRect,
    ViewTriangle,
    ViewChart,
    ViewChartPie,
    ViewChartGauge,
    ViewSvgImage
  },
  props: {
    time: {
      type: String,
      required: false,
      default: '0'
    }
  },
  data() {
    return {
      initSuccess: false,
      configData: {
        name: '--',
        layer: {
          backColor: '#ADD8E6',
          backgroundImage: '',
          widthHeightRatio: '',
          width: 600,
          height: 300
        },
        components: []
      },
      WareHouseCode: '',
      wareHouseList: [],
      locationList: [],
      indexi: 0,
      color: ''
    }
  },
  computed: {
    layerStyle: function () {
      var styles = []
      if (this.configData.layer.backColor) {
        styles.push(`background-color: ${this.configData.layer.backColor}`)
      }
      if (this.configData.layer.backgroundImage) {
        styles.push(`background-image: url("${this.configData.layer.backgroundImage}")`)
      }
      if (this.configData.layer.width > 0) {
        // styles.push(`width: ${this.configData.layer.width}px`)
        styles.push(`width: ${this.configData.layer.width + 10}px`)
      }
      if (this.configData.layer.height > 0) {
        styles.push(`height: ${this.configData.layer.height + 10}px`)
      }
      var style = styles.join(';')
      return style
    }
  },
  watch: {

  },
  created() {
    // this.initSlot()
  },
  mounted() {
    this.initSlot()
  },
  methods: {
    initSlot() {
      setTimeout(function () {
        this.initSuccess = true
      }, 1000)
      // this.initSuccess = false
    },
    //     initSlot() {
    //   setTimeout(function() {
    //     this.initSuccess = true
    //   }, (Number(this.time || 5000)))
    //   this.initSuccess = false
    // },
    // 定时任务，改变组件样式以此来提示哪里有任务
    timer() {
      // return setInterval(() => {
      //   this.configData.components.forEach(component => {
      //     // console.log(component.name)
      //     if (component.name === '货架D') {
      //       if (component.style.url === 'locationD.svg') {
      //         component.style.url = 'locationC.svg'
      //         // console.log(1)
      //       } else {
      //         component.style.url = 'locationD.svg'
      //         // console.log(2)
      //       }
      //       // console.log('|*|*|*|*|*|*|*|' + component.style.url)
      //     }
      //   })
      // }, 1000)
    },
    onShowTray(trayId) {
      this.configData.components = []
      this.locationList = []
      getTrayLayoutById(trayId).then(response => {
        console.log(response)
        var result = JSON.parse(response.data.Content)
        console.log(result)
        var jsonData = JSON.parse(result.JsonLayout)
        if (jsonData === null || jsonData === '') {
          this.$message({
            title: '提示',
            message: '该托盘未生成储位信息',
            type: 'error',
            duration: 3000
          })
          return
        }
        this.layjson = jsonData
        this.locationList = result.LocationList
        // for (this.indexi = 0; this.indexi < jsonData.components.length; this.indexi++) {
        //   // 查找托盘
        //   if (jsonData.components[this.indexi].name === 'orginPos') {

        //   }
        // }
        setTimeout(() => {
          this.transcolor()
        }, 1 * 2000)
      })
    },
    transcolor() {
      // for (let index = 0; index < this.layjson.components; index++) {
      //   var element = this.layjson.components[index]
      //   var isExist = this.locationList.find((item) => (item.LayoutId === element.identifier))
      //   if (isExist) {
      //     console.log(isExist.Color)
      //     element.style.backColor = isExist.Color //'gray'
      //     element.style.borderColor = 'red'
      //   }
      //   else {
      //     element.style.borderColor = 'red'
      //   }
      // }
      for (var i = 0; i < this.locationList.length; i++) {
        var layout = this.locationList[i].LayoutId
        var component = this.layjson.components.find((element) => (element.identifier === layout))
        if (component) {
          component.style.backColor = this.locationList[i].Color //'gray'
          component.style.borderColor = 'red'
          //component.style.borderWidth = 5//component.style.position.w - 5;
          if (this.locationList[i].Color == '' || this.locationList[i].Color == null || this.locationList[i].Color == 'null') {
            component.style.backColor = 'White'
          }
        }
      }
      this.configData = this.layjson
    },
    parseView(component) {
      return topoUtil.parseViewName(component)
    },
    doClickComponent(component) {
      this.$emit('clickBox', component) // 通过子组件给父组件传递值
      for (var i = 0; i < component.action.length; i++) {
        var action = component.action[i]
        if (action.type === 'click') {
          this.handleComponentActuib(action)
        }
      }
    },
    doDbClickComponent(component) {
      for (var i = 0; i < component.action.length; i++) {
        var action = component.action[i]
        if (action.type === 'dblclick') {
          this.handleComponentActuib(action)
        }
      }
    },
    handleComponentActuib(action) {
      var _this = this
      if (action.action === 'visible') {
        if (action.showItems.length > 0) {
          action.showItems.forEach(identifier => {
            _this.showComponent(identifier, true)
          })
        }
        if (action.hideItems.length > 0) {
          action.hideItems.forEach(identifier => {
            _this.showComponent(identifier, false)
          })
        }
      } else if (action.action === 'service') {
        _this.sendFun(action)
      }
    },
    showComponent(identifier, visible) {
      console.log('show:' + identifier + ',visible:' + visible)
      var spirits = this.$refs['spirit']
      for (var i = 0; i < spirits.length; i++) {
        console.log('****************')
        console.log(spirits)
        var spirit = spirits[i]
        if (spirit.detail.identifier === identifier) {
          spirit.detail.style.visible = visible
          break
        }
      }
    }
  }
}
</script>

<style lang="scss">
.topo-render {
  overflow: auto;
  background-color: white;
  background-clip: padding-box;
  background-origin: padding-box;
  background-repeat: no-repeat;
  //background-size: 100% 100%;
  position: relative;
  height: 80%;

  // .topo-render-wrapper {
  //   position: absolute;
  // }

  .topo-render-wrapper-clickable {
    cursor: pointer;
  }
  .topo-render-wrapper {
    position: absolute;
    height: 100px;
    width: 100px;
    background-color: #999;
    cursor: move;
    border: #ccc solid 1px;
  }
}
</style>

