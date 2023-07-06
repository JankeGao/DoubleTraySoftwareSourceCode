<template>
  <div class="topo-main">
    <vue-ruler-tool :parent="true" :is-scale-revise="true">
      <div
        id="surface-edit-layer"
        tabindex="0"
        class="topo-layer"
        :class="{'topo-layer-view-selected': selectedIsLayer}"
        :style="layerStyle"
        @click="onLayerClick($event)"
        @mouseup="onLayerMouseup($event)"
        @mousemove="onLayerMousemove($event)"
        @mousedown="onLayerMousedown($event)"
        @dragover.prevent
        @drop="onDrop"
      >
        <template v-for="(component,index) in Comment.components">
          <div
            :key="index"
            tabindex="0"
            class="topo-layer-view"
            :class="{'topo-layer-view-selected': selectedComponentMap[component.identifier] == undefined? false:true }"
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
            @click.stop="clickComponent(index,component,$event)"
            @mousedown.stop="controlMousedown(component,$event,index)"
            @keyup.delete="removeItem(index,component)"
            @keydown.up.exact.prevent="moveItems('up')"
            @keydown.right.exact.prevent="moveItems('right')"
            @keydown.down.exact.prevent="moveItems('down')"
            @keydown.left.exact.prevent="moveItems('left')"
            @keydown.ctrl.67="copyItem(index,component)"
            @keydown.ctrl.86="pasteItem()"
          >
            <component
              :is="parseView(component)"
              :ref="'comp' + index"
              :detail="component"
              :edit-mode="true"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-left-top"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-lt')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-left-center"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-lc')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-left-bottom"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-lb')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-right-top"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-rt')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-right-center"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-rc')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-right-bottom"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-rb')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-center-top"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-ct')"
            />
            <div
              v-show="selectedComponentMap[component.identifier]"
              class="resize-center-bottom"
              @mousedown.stop="resizeMousedown(component,$event,index,'resize-cb')"
            />
          </div>
        </template>
        <div
          class="topo-frame-selection"
          :style="{width: frameSelectionDiv.width + 'px',height: frameSelectionDiv.height + 'px',top: frameSelectionDiv.top + 'px',left: frameSelectionDiv.left + 'px'}"
        />
      </div>
    </vue-ruler-tool>
  </div>
</template>

<script>
import VueRulerTool from './ruler'

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
import {
  deepCopy
} from '@/assets/libs/utils'

import { createWareHousePlan } from './TopoControls'
import { checkByRectCollisionDetection } from '@/assets/libs/topo'
import TopoRender from './TopoRender'
import { mapMutations, mapState } from 'vuex'
import { v1 as uuidv1 } from 'uuid'
// const uuidv1 = require('uuid/v1')
import { deletelocationByLayoutId } from '@/api/WareHouse'

export default {
  name: 'TopoMain',
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
    ViewSvgImage,
    VueRulerTool,
    TopoRender
  },
  props: [],
  data() {
    return {
      moveItem: {
        startX: 0,
        startY: 0
      }, // 移动组件 相关变量
      resizeItem: {
        startPx: 0,
        startPy: 0,
        x: 0,
        y: 0,
        w: 0,
        h: 0
      }, // resize组件 相关变量
      frameSelectionDiv: {
        width: 0,
        height: 0,
        top: 10,
        left: 10,
        startX: 0,
        startY: 0,
        startPageX: 0,
        startPageY: 0
      },
      Plan: {
        content: '',
        WareHouseCode: ''
      },
      WareHouseList: [],
      flag: '', // 当前操作标志位
      curControl: null,
      curIndex: -1,
      selectedValue: 100,
      showFullScreen: false,
      Comment: {}
    }
  },
  computed: {
    configData: {
      get() {
        // console.log(this.$store.state.topoEditor.topoData)
        return this.$store.state.topoEditor.topoData
      },
      set(newValue) {
        this.Comment = newValue
      }
    },
    ...mapState({
      selectedComponents: state => state.topoEditor.selectedComponents,
      selectedComponentMap: state => state.topoEditor.selectedComponentMap,
      // configData: state => state.topoEditor.topoData,
      selectedIsLayer: state => state.topoEditor.selectedIsLayer,
      copyFlag: state => state.topoEditor.copyFlag,
      copyCount: state => state.topoEditor.copyCount
      // shelfList: state => state.topoEditor.shelfList
    }),
    layerStyle: function () {

      var scale = this.selectedValue / 100
      var styles = [`transform:scale(${scale})`]
      if (this.Comment.layer.backColor) {
        styles.push(`background-color: ${this.Comment.layer.backColor}`)
      }
      if (this.Comment.layer.backgroundImage) {
        styles.push(`background-image: url("${this.Comment.layer.backgroundImage}")`)
      }
      if (this.Comment.layer.width > 0) {
        styles.push(`width: ${this.Comment.layer.width}px`)
      }
      if (this.Comment.layer.height > 0) {
        styles.push(`height: ${this.Comment.layer.height}px`)
      }
      if (this.Comment.layer.borderWidth) {
        styles.push(`border-width: ${this.Comment.layer.borderWidth}`)
      }
      var style = styles.join(';')
      return style
    }
  },
  mounted() {
  },
  created() {
    this.Comment = this.configData
  },
  methods: {
    ...mapMutations('topoEditor', [
      'topoEditor/setLayerSelected'
    ]),
    setLayerSelected(val) {
      this.$store.dispatch('topoEditor/setLayerSelected', val)
    },
    setSelectedComponent(component) {
      this.$store.dispatch('topoEditor/setSelectedComponent', component)
    },
    removeSelectedComponent(component) {
      this.$store.dispatch('topoEditor/removeSelectedComponent', component)
    },
    addSelectedComponent(component) {
      this.$store.dispatch('topoEditor/addSelectedComponent', component)
    },
    format(percentage) {
      return percentage === 100 ? '满' : `${percentage}%`
    },
    controlMousedown(component, event, index) {
      if (event.ctrlKey) {
        return
      }

      this.flag = 'move'
      this.curControl = component
      this.clickItem(component, index)
      this.moveItem.startX = event.pageX
      this.moveItem.startY = event.pageY
      // 记录初始信息--move
      for (var key in this.selectedComponentMap) {
        component = this.selectedComponentMap[key]
        // console.log(component)
        component.style.temp = {}
        component.style.temp.position = {}
        component.style.temp.position.x = component.style.position.x
        component.style.temp.position.y = component.style.position.y
      }
    },
    // 缩放
    increase() {
      this.selectedValue += 10
      if (this.selectedValue > 100) {
        this.selectedValue = 100
      }
    },
    decrease() {
      this.selectedValue -= 10
      if (this.selectedValue < 0) {
        this.selectedValue = 0
      }
    },
    resizeMousedown(component, $event, index, flag) { // resize-鼠标按下事件
      this.flag = flag
      this.curControl = component
      this.curIndex = index
      this.clickItem(component, index)
      var dom = event.currentTarget
      // console.log(dom)
      this.resizeItem.startPx = event.pageX
      this.resizeItem.startPy = event.pageY
      // 记录初始信息-resize
      this.resizeItem.x = this.curControl.style.position.x
      this.resizeItem.y = this.curControl.style.position.y
      this.resizeItem.w = this.curControl.style.position.w
      this.resizeItem.h = this.curControl.style.position.h
    },
    onLayerMousemove(event) {
      if (event.which !== 1) {
        this.flag = ''
        return
      }
      if (this.flag === '') { return }
      if (this.flag.startsWith('resize')) {
        var dx = event.pageX - this.resizeItem.startPx
        var dy = event.pageY - this.resizeItem.startPy
        switch (this.flag) {
          case 'resize-lt':
            this.curControl.style.position.x = this.resizeItem.x + dx
            this.curControl.style.position.y = this.resizeItem.y + dy
            dx = -dx
            dy = -dy
            break
          case 'resize-lc':
            this.curControl.style.position.x = this.resizeItem.x + dx
            dy = 0
            dx = -dx
            break
          case 'resize-lb':
            this.curControl.style.position.x = this.resizeItem.x + dx
            dx = -dx
            break
          case 'resize-rt':
            this.curControl.style.position.y = this.resizeItem.y + dy
            dy = -dy
            break
          case 'resize-rc':
            dy = 0
            break
          case 'resize-rb':
            break
          case 'resize-ct':
            this.curControl.style.position.y = this.resizeItem.y + dy
            dx = 0
            dy = -dy
            break
          case 'resize-cb':
            dx = 0
            break
        }
        if ((this.resizeItem.w + dx) > 20) {
          this.curControl.style.position.w = this.resizeItem.w + dx
        }
        if ((this.resizeItem.h + dy) > 20) {
          this.curControl.style.position.h = this.resizeItem.h + dy
        }
        // canvas组件需要重新渲染
        // DOM 还没有更新
        this.$nextTick(function () {
          // DOM 更新了
          var a = this.$refs['comp' + this.curIndex][0]
          a.onResize()
        })
      } else if (this.flag === 'move') {
        // 移动组件
        dx = event.pageX - this.moveItem.startX
        dy = event.pageY - this.moveItem.startY
        for (var key in this.selectedComponentMap) {
          var component = this.selectedComponentMap[key]
          component.style.position.x = component.style.temp.position.x + dx
          component.style.position.y = component.style.temp.position.y + dy
        }
        if (component.name === 'orginPos') {
          this.$emit('changeOrginPos', component) // 通过子组件给父组件传递值
        } else {
          // 重新计算灯号
          this.$emit('moveItem') // 通过子组件给父组件传递值
        }
      } else if (this.flag === 'frame_selection') {
        this.onFrameSelection(event)
      }
    },
    onLayerMouseup(event) {
      if (this.flag.startsWith('resize')) {
        var a = this.$refs['comp' + this.curIndex][0]
        a.onResize()
      } else if (this.flag === 'frame_selection') {
        // 由于和onLayerClick冲突，这里模拟下点击空白区域
        this.onFrameSelection(event)
        this.frameSelectionDiv.width = 0
        this.frameSelectionDiv.height = 0
        this.frameSelectionDiv.top = 0
        this.frameSelectionDiv.left = 0
      }
      this.flag = ''
    },
    onLayerMousedown($event) {
      this.flag = 'frame_selection'
      this.frameSelectionDiv.startX = event.offsetX
      this.frameSelectionDiv.startY = event.offsetY
      this.frameSelectionDiv.startPageX = event.pageX
      this.frameSelectionDiv.startPageY = event.pageY
    },
    onLayerClick() {
      // 清除当前选中组件
      this.$emit('clearComp')
    },
    onFrameSelection(event) {
      var dx = event.pageX - this.frameSelectionDiv.startPageX
      var dy = event.pageY - this.frameSelectionDiv.startPageY
      this.frameSelectionDiv.width = Math.abs(dx)
      this.frameSelectionDiv.height = Math.abs(dy)
      if (dx > 0 && dy > 0) {
        this.frameSelectionDiv.top = this.frameSelectionDiv.startY
        this.frameSelectionDiv.left = this.frameSelectionDiv.startX
      } else if (dx > 0 && dy < 0) {
        this.frameSelectionDiv.top = this.frameSelectionDiv.startY + dy
        this.frameSelectionDiv.left = this.frameSelectionDiv.startX
      } else if (dx < 0 && dy > 0) {
        this.frameSelectionDiv.top = this.frameSelectionDiv.startY
        this.frameSelectionDiv.left = this.frameSelectionDiv.startX + dx
      } else if (dx < 0 && dy < 0) {
        this.frameSelectionDiv.top = this.frameSelectionDiv.startY + dy
        this.frameSelectionDiv.left = this.frameSelectionDiv.startX + dx
      }
      // 判断各个组件是否在方框内
      var _this = this
      var rect = {
        x: this.frameSelectionDiv.left,
        y: this.frameSelectionDiv.top,
        width: this.frameSelectionDiv.width,
        height: this.frameSelectionDiv.height
      }
      var components = this.Comment.components
      components.forEach(component => {
        var itemRect = {
          x: component.style.position.x,
          y: component.style.position.y,
          width: component.style.position.w,
          height: component.style.position.h
        }
        if (checkByRectCollisionDetection(rect, itemRect)) {
          _this.addSelectedComponent(component)
        } else {
          _this.removeSelectedComponent(component)
        }
      })
      if (this.selectedComponents.length > 0) {
        this.setLayerSelected(false)
      } else {
        this.setLayerSelected(true)
      }
    },
    onAddBox(component) {
      this.Comment.components.push(component)
      // 默认选中，并点击
      // this.clickItem(component, this.Comment.components.length - 1)
    },
    onDrop(event) {
      console.log('rrrrrr')
      event.preventDefault() // 阻止事件的默认行为
      var infoJson = event.dataTransfer.getData('my-info') // 获取自定义的数据格式
      var component = JSON.parse(infoJson)
      if (this.checkAddComponent(component) === false) {
        return
      }
      // 判断当前着陆点是不是layer
      if (event.target.id === 'surface-edit-layer') {
        component.style.position.x = event.offsetX // offsetX设置或获取鼠标指针位置相对于触发事件的对象的 x 坐标。
        component.style.position.y = event.offsetY
      } else {
        // 解决着陆灯不是layer的情形
        var layer = event.currentTarget // 获取事件绑定的元素（）
        var position = layer.getBoundingClientRect() // 获取元素上下左右分别相对浏览器窗口的位置
        var x = event.clientX - position.left
        var y = event.clientY - position.top
        component.style.position.x = x
        component.style.position.y = y
      }
      // 处理默认值
      var fuid = uuidv1
      component.identifier = fuid()
      component.name = component.type + this.Comment.components.length
      component.style.visible = true
      component.style.transform = 0
      component.style.borderWidth = component.style.borderWidth ? component.style.borderWidth : 0
      component.style.borderStyle = component.style.borderStyle ? component.style.borderStyle : 'solid'
      component.style.borderColor = component.style.borderColor ? component.style.borderColor : '#ccccccff'
      // component.style.fontFamily = "Arial";
      this.Comment.components.push(component)
      // 默认选中，并点击
      this.clickItem(component, this.Comment.components.length - 1)
    },
    moveItems(direction) {
      var dx = 0; var dy = 0
      if (direction === 'up') {
        dy = -1
      } else if (direction === 'right') {
        dx = 1
      } else if (direction === 'down') {
        dy = 1
      } else if (direction === 'left') {
        dx = -1
      }
      for (var key in this.selectedComponentMap) {
        var component = this.selectedComponentMap[key]
        component.style.position.x = component.style.position.x + dx
        component.style.position.y = component.style.position.y + dy
      }
    },
    checkAddComponent(info) {
      if (info == null) {
        this.$q.notify({
          type: 'negative',
          position: 'bottom-right',
          message: 'This component not surpport.'
        })
        return false
      }
      return true
    },
    parseView(component) {
      return topoUtil.parseViewName(component)
    },
    clickItem(component, index) {
      console.log(component)
      // 如果不是原点
      if (component.name !== 'orginPos') {
        this.$emit('clickBox', component) // 通过子组件给父组件传递值
      }
      this.setLayerSelected(false)
      if (this.selectedComponentMap[component.identifier] === undefined) {
        this.setSelectedComponent(component)
      } else {
        // 如果已经选中，则不做任何处理
      }
    },
    clickComponent(index, component, event) { // 点击组件
      // console.log(event)
      if (event.ctrlKey === true) { // 点击了ctrl
        this.setLayerSelected(false)
        if (this.selectedComponentMap[component.identifier] === undefined) {
          this.addSelectedComponent(component)
        } else {
          this.removeSelectedComponent(component)
        }
      } else {
        // 这里不再处理点击事件，改为鼠标左键
        // this.clickItem(component,index);
      }
    },
    copyItem(index, component) { // 设定复制源
      // console.log(index)
      // console.log(component)
      // this.setCopyFlag(true)
    },
    pasteItem() {
      // console.log(this.copyFlag)
      if (this.copyFlag) {
        var fuid = uuidv1
        for (var key in this.selectedComponentMap) {
          var s = this.selectedComponentMap[key]
          var component = deepCopy(s)
          component.identifier = fuid()
          component.name = component.type + this.Comment.components.length
          component.style.visible = true
          component.style.position.x = component.style.position.x + 25 * (this.copyCount + 1)
          component.style.position.y = component.style.position.y + 25 * (this.copyCount + 1)
          this.Comment.components.push(component)
        }
        this.increaseCopyCount()
      }
    },
    removeItem(index, component) { // 移除组件
      this.$confirm('此操作将永久删除该库位, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var keys = []
        for (var i = 0; i < this.Comment.components.length; i++) {
          // 原点和灯不可删除
          if (this.Comment.components[i].name !== 'orginPos' && this.Comment.components[i].name !== 'light') {
            var identifier = this.Comment.components[i].identifier

            if (this.selectedComponentMap[identifier] !== undefined) {
              keys.push(i)
            }
          }
        }
        // 排序
        keys.sort((a, b) => { return a - b })

        const entity = {
          LayoutId: component.identifier
        }
        deletelocationByLayoutId(entity).then(response => {
          var resData = JSON.parse(response.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            // 逆向循环删除
            for (var j = keys.length - 1; j >= 0; j--) {
              this.Comment.components.splice(keys[j], 1)
            }
            this.$message({
              title: '成功',
              message: '删除成功',
              type: 'success',
              duration: 2000
            })
          } else {
            this.$message({
              title: '成功',
              message: '删除失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
        this.setLayerSelected(true)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    fullScreen() {
      console.log(typeof (this.configData))
      console.log('*******************')
      localStorage.setItem('topoData', JSON.stringify(this.configData))
      this.showFullScreen = true
    },
    save() {
      if (this.Plan.WareHouseCode === '') {
        this.$message({
          title: '失败',
          message: '请选择要创建的仓库名称',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.Plan.content = JSON.stringify(this.Comment)
      createWareHousePlan(this.Plan).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: resData.Message,
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    printData() {
      var json = JSON.stringify(this.configData)
      console.log(json)
      alert(json)
    }
  }

}
</script>

<style lang="scss">
.topo-main {
  background-color: white;
  border: #ccc solid 1px;
  position: relative;
  overflow-x: hidden;
  overflow-y: hidden;

  .topo-layer {
    width: 100%;
    height: 100%;
    position: absolute;
    transform-origin: left top;
    overflow: auto;
    background-color: #fff;
    background-clip: padding-box;
    background-origin: padding-box;
    background-repeat: no-repeat;
    background-size: 100% 100%;

    .topo-frame-selection {
      background-color: #8787e7;
      opacity: 0.3;
      border: #0000ff solid 1px;
      position: absolute;
      z-index: 999;
    }

    .topo-layer-view {
      position: absolute;
      height: 100px;
      width: 100px;
      background-color: #999;
      cursor: move;
      border: #ccc solid 1px;

      .resize-left-top {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        left: -5px;
        top: -5px;
        cursor: nwse-resize;
      }

      .resize-left-center {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        left: -5px;
        top: 50%;
        margin-top: -5px;
        cursor: ew-resize;
      }

      .resize-left-bottom {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        left: -5px;
        bottom: -5px;
        cursor: nesw-resize;
      }

      .resize-right-top {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        right: -5px;
        top: -5px;
        cursor: nesw-resize;
      }

      .resize-right-center {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        right: -5px;
        top: 50%;
        margin-top: -5px;
        cursor: ew-resize;
      }

      .resize-right-bottom {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        right: -5px;
        bottom: -5px;
        cursor: nwse-resize;
      }

      .resize-center-top {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        top: -5px;
        left: 50%;
        margin-left: -5px;
        cursor: ns-resize;
      }

      .resize-center-bottom {
        position: absolute;
        height: 10px;
        width: 10px;
        background-color: white;
        border: 1px solid #0cf;
        bottom: -5px;
        left: 50%;
        margin-left: -5px;
        cursor: ns-resize;
      }
    }

    .topo-layer-view-selected {
      outline: 1px solid #0cf;
    }
  }
}
</style>
