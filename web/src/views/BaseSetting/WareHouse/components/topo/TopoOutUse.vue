<template>
  <div v-if="configData.layer" class="topo-render" :style="layerStyle">
    <!-- <div class="filter-container" style="margin-bottom:20px">
      <el-select v-model="WareHouseCode" clearable placeholder="请选择仓库" @change="handleWareHouseChange">
        <el-option
          v-for="item in wareHouseList"
          :key="item.Code"
          :label="item.Name"
          :value="item.Code"
        />
      </el-select>
    </div> -->
    <template v-for="(component,index) in configData.components">
      <div
        v-show="component.style.visible == undefined? true:component.style.visible"
        :key="index"
        class="topo-render-wrapper"
        style="margin-top:60px"
        :class="{'topo-render-wrapper-clickable': component.action.length > 0 }"
        :style="{
          left: component.style.position.x + 'px',
          top: component.style.position.y + 'px',
          width: component.style.position.w + 'px',
          height: component.style.position.h + 'px',
          backgroundColor: component.style.backColor,
          zIndex: component.style.zIndex,
          borderWidth: component.style.borderWidth + 'px',
          borderStyle: component.style.borderStyle,
          borderColor: component.style.borderColor,
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

import { getWareHousePlan, getPickOrderAllMaterialsStatusCaption } from './TopoControls'

export default {
  name: 'TopoOutUse', // 供外部使用的组件
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
    locationview: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      configData: {

      },
      location: {},
      WareHouseCode: '',
      wareHouseList: [],
      shelf_List: [],
      a: undefined
    }
  },
  computed: {
    layerStyle: function() {
      var styles = []
      if (this.configData.layer.backColor) {
        styles.push(`background-color: ${this.configData.layer.backColor}`)
      }
      if (this.configData.layer.backgroundImage) {
        styles.push(`background-image: url("${this.configData.layer.backgroundImage}")`)
      }
      if (this.configData.layer.width > 0) {
        // styles.push(`width: ${this.configData.layer.width}px`)
        styles.push(`width: 100%`)
      }
      if (this.configData.layer.height > 0) {
        styles.push(`height: ${this.configData.layer.height}px`)
      }
      var style = styles.join(';')
      return style
    }
  },
  watch: {
    locationview: function(new_Value, old_Value) {
      this.location = new_Value
    }
  },
  mounted() {
    this.shelf_List = []
    // console.log(this.locationview)
    getWareHousePlan(this.locationview.wareHouse_Code).then((res) => {
      var da = JSON.parse(res.data.Content)
      var resData = JSON.parse(da.Message)
      this.configData = resData
      // this.configData.components.forEach(component => {
      //   this.locationview.shelf_Code.forEach(item => {
      //     if (component.name === item) {
      //       this.shelf_List.push(component.name)
      //     }
      //   })
      // })
      this.a = setInterval(() => {
        console.log(this.locationview.pickOrder_Code)
        getPickOrderAllMaterialsStatusCaption(this.locationview.pickOrder_Code).then(response => {
          var pick_Data = JSON.parse(response.data.Content)
          // console.log(pick_Data)
          pick_Data.forEach(element => {
            if (element.Status < 3) {
              this.configData.components.forEach(component => {
                if (element.LocationCode.search(component.name) > 0) {
                  // console.log()
                  // console.log(element.LocationCode + '|*******|' + component.name)
                  component.style.url = 'locationC.svg'
                }
              })
            } else {
              this.configData.components.forEach(component => {
                if (element.LocationCode.search(component.name)) {
                  component.style.url = 'locationD.svg'
                }
              })
            }
          })
        })
      }, 1000)
      // this.taskTimer()
    })
  },
  destroyed() {
    clearInterval(this.a)
  },
  methods: {
    taskTimer() {
      return setInterval(() => {
        getPickOrderAllMaterialsStatusCaption(this.locationview.pickOrder_Code).then(response => {
          var pick_Data = JSON.parse(response.data.Content)
          // console.log(pick_Data)
          pick_Data.forEach(element => {
            if (element.Status < 3) {
              this.configData.components.forEach(component => {
                if (element.LocationCode.search(component.name) > 0) {
                  // console.log()
                  // console.log(element.LocationCode + '|*******|' + component.name)
                  component.style.url = 'locationC.svg'
                }
              })
            } else {
              this.configData.components.forEach(component => {
                if (element.LocationCode.search(component.name)) {
                  component.style.url = 'locationD.svg'
                }
              })
            }
          })
        })
      }, 1000)
    },
    // 定时任务，改变组件样式以此来提示哪里有任务
    chanageTimer(val) {
      return setInterval((val) => {
        this.configData.components.forEach(component => {
          if (val.LocationCode.search(component.name)) {
            if (component.style.url === 'locationD.svg') {
              component.style.url = 'locationC.svg'
            } else {
              component.style.url = 'locationD.svg'
            }
          }
          // console.log(component.name)
          // console.log(this.shelf_List)
          // console.log(this.shelf_List.includes(component.name))
          // if (this.shelf_List.includes(component.name)) {
          //   if (component.style.url === 'locationD.svg') {
          //     component.style.url = 'locationC.svg'
          //   } else {
          //     component.style.url = 'locationD.svg'
          //   }
          // }
        })
      }, 1000)
    },
    parseView(component) {
      return topoUtil.parseViewName(component)
    },
    doClickComponent(component) {
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

        .topo-render-wrapper {
            position: absolute;
        }

        .topo-render-wrapper-clickable {
            cursor: pointer;
        }
    }
</style>

