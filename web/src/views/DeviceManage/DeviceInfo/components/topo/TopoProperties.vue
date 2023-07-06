<template>
  <div class="topo-properties">
    <div class="topo-properties-nav">
      <!-- <el-select v-model="curComponent" :options="componentOptions" @input="changeComponent" style="margin-right:0px;height:43px;border:none;" /> -->
      <template v-if="isLayer">
        <el-input v-model="topoData.name" style="margin-top:10px" />
      </template>
      <template v-if="configObject != null && isLayer == false">
        <!-- <el-input v-model="configObject.name" /> -->
        <el-select
          v-model="configObject.name"
          filterable
          allow-create
          default-first-option
          placeholder="请选择或输入货架编码"
        >
          <el-option
            v-for="item in shelfList"
            :key="item"
            :label="item"
            :value="item"
          />
        </el-select>
      </template>
    </div>
    <template v-if="configObject != null && isLayer == false">
      <div class="topo-properties-tabs">
        <div class="topo-properties-tab" :class="{'topo-properties-tab-active': tabIndex == 0}" @click="changeTab(0)">样式</div>
        <div class="topo-properties-tab" :class="{'topo-properties-tab-active': tabIndex == 1}" @click="changeTab(1)">数据</div>
        <div class="topo-properties-tab" :class="{'topo-properties-tab-active': tabIndex == 2}" @click="changeTab(2)">行为</div>
      </div>
      <div class="topo-properties-table">
        <div v-show="tabIndex == 0">
          <table style="display: none">
            <tr>
              <td width="50%" style="padding:5px 0px;background-color:#eee;">属性</td>
              <td width="50%" style="padding:5px 0px;background-color:#eee;">值</td>
            </tr>
          </table>
          <el-collapse>
            <el-collapse-item title="位置" name="1">
              <el-row class="item">
                <el-col :span="8">
                  X :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.position.x" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  Y :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.position.y" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  宽：
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.position.w" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  高：
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.position.h" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
            </el-collapse-item>
            <el-collapse-item title="边框" name="2">
              <el-row class="item">
                <el-col :span="8">
                  线宽 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.borderWidth" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row class="item">
                <div v-show="colorShow" style="margin-right:20px;z-index: 10000;">
                  <sketch-picker v-model="color" @input="updateValue" />
                </div>
                <el-col :span="8">
                  颜色 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model="configObject.style.borderColor" placeholder="请选择颜色" class="input-with-select">
                    <el-button slot="append" icon="el-icon-search" @click="handleShowColor" />
                  </el-input>
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  样式 :
                </el-col>
                <el-col :span="16">
                  <el-select v-model="configObject.style.borderStyle" placeholder="请选择">
                    <el-option
                      v-for="item in borderStyleOptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value"
                    />
                  </el-select>
                </el-col>
              </el-row>
            </el-collapse-item>
            <el-collapse-item title="基本" name="3">
              <el-row class="item">
                <el-col :span="8">
                  可见 :
                </el-col>
                <el-col :span="16">
                  <el-select v-model="configObject.style.visible" placeholder="请选择">
                    <el-option
                      v-for="item in visibleStyleOptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value"
                    />
                    <!-- <el-option label="显示" value="true" />
                    <el-option label="隐藏" value="false" /> -->
                  </el-select>
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  图层 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.zIndex" type="number" />
                </el-col>
              </el-row>
              <el-row class="item">
                <el-col :span="8">
                  旋转 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.transform" type="number" suffix="deg" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.url != undefined && configObject.style.url != null" class="item">
                <el-col :span="8">
                  URL :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.url" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.text != undefined" class="item">
                <el-col :span="8">
                  文字内容 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.text" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.textAlign != undefined" class="item">
                <el-col :span="8">
                  文字属性 :
                </el-col>
                <el-col :span="16">
                  <el-select v-model="configObject.style.textAlign" placeholder="请选择">
                    <el-option
                      v-for="item in textAlignOptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value"
                    />
                  </el-select>
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.fontSize != undefined" class="item">
                <el-col :span="8">
                  字号 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.fontSize" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.radius != undefined" class="item">
                <el-col :span="8">
                  半径 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.radius" type="number" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.lineWidth != undefined" class="item">
                <el-col :span="8">
                  线宽 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model.lazy="configObject.style.lineWidth" type="number" suffix="px" style="padding-right:5px;" />
                </el-col>
              </el-row>
              <el-row v-if="configObject.style.fontFamily != undefined" class="item">
                <el-col :span="8">
                  字体 :
                </el-col>
                <el-col :span="16">
                  <el-select v-model="configObject.style.fontFamily" placeholder="请选择">
                    <el-option
                      v-for="item in fontFamilyOptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value"
                    />
                  </el-select>
                </el-col>
              </el-row>
              <el-row class="item">
                <div v-show="colorBackshow" style="margin-right:20px;z-index: 10000;">
                  <sketch-picker v-model="color" @input="updateBackValue" />
                </div>
                <el-col :span="8">
                  背景颜色 :
                </el-col>
                <el-col :span="16">
                  <el-input v-model="configObject.style.backColor" placeholder="请选择颜色" class="input-with-select">
                    <el-button slot="append" icon="el-icon-search" @click="colorBackshow=true" />
                  </el-input>
                </el-col>
              </el-row>
            </el-collapse-item>
          </el-collapse>
        </div>
        <div v-show="tabIndex == 1">
          <div class="not-surpport">根据实际系统设计</div>
        </div>
        <div v-show="tabIndex == 2">
          <template v-if="configObject && configObject.action">
            <template v-for="(event,index) in configObject.action">
              <div :key="index" style="margin-top:10px;">
                <div style="padding:5px;color:#fff;font-size:13px;background-color:#212121">
                  交互-{{ index+1 }}
                  <i class="el-icon-delete" style="float:right;cursor:pointer;" @click.native="removeAction(index)" />
                </div>
                <el-row class="item">
                  <el-col :span="8" style="margin-top:8px">
                    事件 :
                  </el-col>
                  <el-col :span="16">
                    <el-select v-model="event.type" placeholder="请选择">
                      <el-option
                        v-for="item in eventOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value"
                      />
                    </el-select>
                  </el-col>
                </el-row>
                <el-row class="item">
                  <el-col :span="8" style="margin-top:8px">
                    动作 :
                  </el-col>
                  <el-col :span="16">
                    <el-select v-model="event.action" placeholder="请选择">
                      <el-option
                        v-for="item in eventActionOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value"
                      />
                    </el-select>
                  </el-col>
                </el-row>
                <el-row v-if="event.action == 'visible'" class="item">
                  <el-col :span="8" style="margin-top:8px">
                    点击出现 :
                  </el-col>
                  <el-col :span="16">
                    <el-select v-model="event.showItems" placeholder="请选择">
                      <el-option
                        v-for="item in componentOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value"
                      />
                    </el-select>
                  </el-col>
                </el-row>
                <el-row v-if="event.action == 'visible'" class="item">
                  <el-col :span="8" style="margin-top:8px">
                    点击隐藏 :
                  </el-col>
                  <el-col :span="16">
                    <el-select v-model="event.hideItems" placeholder="请选择">
                      <el-option
                        v-for="item in componentOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value"
                      />
                    </el-select>
                  </el-col>
                </el-row>
              </div>
            </template>
            <div style="width:100%;padding:10px 10px 10px 10px;">
              <el-button size="mini" style="width:100%;" @click="addAction">添加</el-button>
            </div>
          </template>
        </div>
      </div>
    </template>
    <template v-if="isLayer">
      <table style="margin: 10px;font-size:14px;border:none">
        <tr>
          <td width="35%">
            背景颜色
          </td>
          <td>
            <el-input v-model.lazy="topoData.layer.backColor" />
          </td>
        </tr>
        <tr>
          <td>
            背景图片
          </td>
          <td>
            <el-input v-model.lazy="topoData.layer.backgroundImage" />
          </td>
        </tr>
        <tr>
          <td>
            分辨率
          </td>
          <td>
            <el-select v-model="layerWH" :options="whOptions" />
          </td>
        </tr>
        <tr v-if="layerWH == 'custom'">
          <td>
            W
          </td>
          <td>
            <el-input v-model.lazy="topoData.layer.width" />
          </td>
        </tr>
        <tr v-if="layerWH == 'custom'">
          <td>
            H
          </td>
          <td>
            <el-input v-model.lazy="topoData.layer.height" />
          </td>
        </tr>
      </table>

    </template>
  </div>
</template>

<script>

// import { mapActions, mapGetters, mapState, mapMutations } from 'vuex'
import { mapState } from 'vuex'
import { Sketch } from 'vue-color'

export default {
  name: 'TopoProperties',
  components: {
    'sketch-picker': Sketch
  },
  data() {
    return {
      tabIndex: 0,
      whOptions: ['1024x768', '1366x768', '1280x800', '1440x900', '1600x900', '1920x1080', 'custom'],
      layerWHTemp: '',
      componentOptions: [],
      eventActionOptions: [{
        value: 'link',
        label: '打开链接'
      }, {
        value: 'val',
        label: '赋值变量'
      }, {
        value: 'visible',
        label: '展示隐藏'
      }, {
        value: 'service',
        label: '调用服务'
      }, {
        value: '',
        label: ''
      }],
      // 事件类型
      eventOptions: [{
        value: 'click',
        label: '点击'
      }, {
        value: 'dblclick',
        label: '双击'
      }, {
        value: 'mouseenter',
        label: '鼠标移入'
      }, {
        value: 'mouseleave',
        label: '鼠标双击'
      }, {
        value: '',
        label: ''
      }],
      fontFamilyOptions: [{
        value: 'Arial',
        label: 'Arial'
      }, {
        value: 'Helvetica',
        label: 'Helvetica'
      }, {
        value: 'sans-serif',
        label: 'sans-serif'
      },
      {
        value: '宋体',
        label: '宋体'
      },
      {
        value: '黑体',
        label: '黑体'
      }, {
        value: '微软雅黑',
        label: '微软雅黑'
      }, {
        value: '',
        label: ''
      }],

      borderStyleOptions: [{
        value: 'solid',
        label: '实线'
      }, {
        value: 'dashed',
        label: '虚线'
      }, {
        value: 'dotted',
        label: '点线'
      }, {
        value: '',
        label: ''
      }],
      visibleStyleOptions: [{
        value: true,
        label: '显示'
      }, {
        value: false,
        label: '隐藏'
      }],
      textAlignOptions: [{
        value: 'left',
        label: '靠左'
      }, {
        value: 'right',
        label: '靠右'
      }, {
        value: 'center',
        label: '居中'
      }, {
        value: '',
        label: ''
      }],
      value: '',
      color: '#000',
      // 颜色选择器
      colorShow: false,
      colorBackshow: false,
      colors: {
        hex: '#194d33',
        hsl: { h: 150, s: 0.5, l: 0.2, a: 1 },
        hsv: { h: 150, s: 0.66, v: 0.30, a: 1 },
        rgba: { r: 25, g: 77, b: 51, a: 1 },
        a: 1
      }
    }
  },
  computed: {
    layerWH: {
      get: function() {
        // this.topoData
        /* eslint-disable */
        if (!this.topoData.layer.width || !this.topoData.layer.height) {
          this.topoData.layer.width = 1600
          this.topoData.layer.height = 900
        }
        if (this.layerWHTemp === '') {
          var wh = this.topoData.layer.width + 'x' + this.topoData.layer.height
          if (this.whOptions.indexOf(wh, 0) === -1) {
            this.layerWHTemp = 'custom'
          } else {
            this.layerWHTemp = wh
          }
        } else {
          wh = this.topoData.layer.width + 'x' + this.topoData.layer.height
          if (this.whOptions.indexOf(wh, 0) === -1) {
            this.layerWHTemp = 'custom'
          }
        }
        return this.layerWHTemp
      },
      set: function(val) {
        this.layerWHTemp = val
        if (val === 'custom') {
          console.log()
        } else {
          var wh = val.split('x')
          this.topoData.layer.width = parseInt(wh[0])
          this.topoData.layer.height = parseInt(wh[1])
        }
      }
    },
    shelfList: {
      get:function(){
        console.log(this.$store.getters.shelfList)
        return this.$store.getters.shelfList
      }
    },
    ...mapState({
      topoData: state => state.topoEditor.topoData,
      selectedComponents: state => state.topoEditor.selectedComponents,
      selectedComponentMap: state => state.topoEditor.selectedComponentMap,
      isLayer: state => state.topoEditor.selectedIsLayer,
      configObject: state => state.topoEditor.selectedComponent,
      // shelfList: state => state.shelfList
    }),
    // ...mapGetters(['shelfList']) // 动态计算属性，相当于this.$store.getters.shelfList
  },
  mounted() {
  },
  methods: {
    initPage(configData) {
      this.configData = configData
    },
    changeTab(tabIndex) {
      this.tabIndex = tabIndex
    },
    bindData(configObject, index, isLayer) {
      this.configObject = configObject
      this.isLayer = isLayer
      if (isLayer === false) {
        console.log()
      }
    },
    generateTargetComponentOptions() {
      var options = []
      this.topoData.components.forEach(component => {
        if (this.configObjectconfigObject.identifier !== component.identifier) {
          options.push({
            label: component.name || component.type,
            value: component.identifier
          })
        }
      })
      this.componentOptions=options
    },
    removeAction(index) {
      this.configObject.action.splice(index, 1)
    },
    addAction() {
      var action = {
        type: 'click',
        action: 'visible',
        showItems: [],
        hideItems: []
      }
      this.configObject.action.push(action)
      this.generateTargetComponentOptions() 
    },
      // 颜色选择器
    handleShowColor () {
      if (this.colorShow) {
        this.colorShow = false
      } else {
        this.colorShow = true
      }
    },
    updateValue (val) {
      this.configObject.style.borderColor = val.hex
      this.colorShow=false
    },
    // 背景选择器
    handleShowBackColor () {
      if (this.colorBackshow) {
        this.colorBackshow = false
      } else {
        this.colorBackshow = true
      }
    },
    updateBackValue(val){
      this.configObject.style.backColor = val.hex
      console.log(val.hex)
      this.colorBackshow=false
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
  /deep/ .el-collapse-item__wrap{
      background-color: #2F2F2F;
      border: none;
  }

  /deep/ .el-collapse-item__content{
  background-color: #2F2F2F;
  border: none;
}
  /deep/ .el-collapse-item__header{
  background-color:#212121;
  color:#fff;
  width:100%;
  padding:5px;
  height:35px;
  border: none;
  padding:10px;
}

.topo-properties-nav{
  margin:1px 10px;
}

.topo-properties {
   // border: #ccc solid 1px;
   // background-color: white;
    background-color: #2F2F2F;
    height: 100%;
    color: #fff;
    

    .topo-properties-tabs {
        height: 35px;
        display: flex;
        border-bottom: #ccc solid 1px;
        background-color: #2F2F2F;
        font-size:13px;
        .topo-properties-tab {
            height: 35px;
            text-align: center;
            line-height: 35px;
            flex: 1;
            color: #C0C4CC;
        }

        // .topo-properties-tab+.topo-properties-tab {
        //    // border-left: #ccc solid 1px;
        // }

        .topo-properties-tab:hover {
            cursor: pointer;
        }

        .topo-properties-tab-active {
            color: #fff;
            border-bottom: #FFCC33 solid 2px;
            font-weight: bold;
        }
    }

    .topo-properties-table {
        overflow-x: hidden;
        overflow-y: auto;
        height: calc(100% - 92px);

        table {
            width: 100%;
            border-collapse: collapse;

            td {
                text-align: center;
                padding: 0px;
                border: #ccc solid 1px;
            }

            .el-input {
                border: none;
            }

            .el-select {
                border: none;
                margin-right: 0px;
            }
        }
    }

    .not-surpport {
        margin: 20px auto;
        text-align: center;
    }
}

.item{
  color:#fff;
  text-align:center;
  margin:6px;
  font-size: 14px;
  text-align: center;
}
</style>
