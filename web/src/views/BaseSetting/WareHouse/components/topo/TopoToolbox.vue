<template>
  <div class="topo-toolbox">
    <el-collapse v-model="activeNames">
      <el-collapse-item v-for=" (group,index1) in toolbox" :key="index1" :name="group.title">
        <template slot="title">
          <span style="margin-left:10px">
            <i :class="'header-icon '+ group.icon" />
          </span>
          <span style="margin-left:10px">
            {{ group.title }}
          </span>
        </template>
        <div class="toolbox-group">
          <div v-for="(value,index) in group.items" :key="index">
            <div :key="index" class="toolbox-item" draggable="true" @dragstart="onDragstart($event,value)">
              <!-- 判断是不是字体图标 -->
              <div v-if="value.isFontIcon === true">
                <div class="toolbox-item-icon">
                  <i :class="value.icon" />
                </div>
                <div class="toolbox-item-text">{{ value.text }}</div>
              </div>
              <div v-else>
                <div class="toolbox-item-icon">
                  <!-- <img class="topo-dom" src="./static/office/embellish1.png" style="width: 40px;height: 40px;"> -->
                  <img class="topo-dom" :src="getImg(value.icon)" style="width: 40px;height: 40px;">
                </div>
                <div class="toolbox-item-text">{{ value.text }}</div>
              </div>
              <!-- <div v-show="group.items.length === (index+1)">
                <div class="toolbox-item-icon">
                  <img class="topo-dom" :src="getImg(value.icon)" style="width: 40px;height: 40px;">
                </div>
                <div class="toolbox-item-text">{{ value.text }}</div>
              </div> -->
            </div>
          </div>
        </div>
      </el-collapse-item>
    </el-collapse>
  </div>
</template>

<script>
import jsonBase from './data-toolbox/base.json'
import jsonChart from './data-toolbox/chart.json'
import jsonOffice from './data-toolbox/office.json'
import jsonSvg from './data-toolbox/svg.json'
// import jsonSvgDianli from './data-toolbox/svg-dianli.json'
export default {
  name: 'TopoToolbox',
  data() {
    return {
      toolbox: [],
      box: [],
      selectedIndex: -1,
      activeNames: ['1']
    }
  },
  mounted() {
    this.toolbox = []
    this.toolbox.push(jsonBase)
    this.toolbox.push(jsonChart)
    this.toolbox.push(jsonOffice)
    this.toolbox.push(jsonSvg)
    // this.handleChange()
    // this.toolbox.push(jsonSvgDianli)
  },
  methods: {
    onDragstart(event, info) {
      console.log(event)
      console.log(info)
      var infoJson = JSON.stringify(info.info)
      event.dataTransfer.setData('my-info', infoJson) // 设置自定义的数据格式
    },
    getImg(imgName) {
      if (imgName.search('svg') !== -1) {
        return require('./static/svg/' + imgName)
      }
      return require('./static/office/' + imgName)
    },
    handleChange() {
      // for (var i = 0; i < this.toolbox.length; i++) {
      //   if (this.toolbox[i].items.isFontIcon === false) {
      //     for (var j = 0; j < this.toolbox[i].items.length; j++) {
      //       this.box.push(require(this.toolbox[i].items[j].icon))
      //       // this.toolbox[i].items[j].icon = require(this.toolbox[i].items[j].icon)
      //       // console.log(this.toolbox[i].items[j].icon)
      //     }
      //   }
      // }
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
  /deep/ .el-collapse-item__content{
  background-color: #2F2F2F;
}
  /deep/ .el-collapse-item__header{
  background-color:#212121;
  color:#fff;
  width:100%;
  padding:5px;
  height:35px;
  border: none;
}
.topo-toolbox {
    //border: #ccc solid 1px;
    background-color: #212121;
    overflow-y: auto;
    overflow-x: hidden;

    .toolbox-group {
        display: flex;
        flex-wrap: wrap;
        justify-content: flex-start;
        align-content: space-between;
        background-color: #2F2F2F;
        .toolbox-item {
            width: 60px;
            margin: 10px 5px;
            padding: 5px;
            color: #C0C4CC;
            border: transparent solid 1px;

            .toolbox-item-icon {
                text-align: center;
            }

            .toolbox-item-text {
                margin-top: 10px;
                text-align: center;
            }
        }

        .toolbox-item:hover {
            border: #ccc solid 1px;
            background: #ccc;
            color: #3388ff;
            border-radius: 6px;
            cursor: pointer;
        }
    }
}
</style>
