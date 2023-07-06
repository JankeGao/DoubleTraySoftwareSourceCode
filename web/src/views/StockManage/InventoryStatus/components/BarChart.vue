<template>
  <div :class="className" :style="{height:height,width:width}" />
</template>

<script>
import echarts from 'echarts'
require('echarts/theme/macarons') // echarts theme
import { debounce } from '@/utils'
const animationDuration = 6000

export default {
  props: {
    className: {
      type: String,
      default: 'chart'
    },
    width: {
      type: String,
      default: '100%'
    },
    height: {
      type: String,
      default: '300px'
    },
    chartData: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      chart: null,
      salesData: []
    }
  },
  watch: {
    chartData: {
      deep: true,
      handler(val) {
        this.salesData = val
        this.initChart()
      }
    }
  },
  mounted() {
    this.initChart()
    this.__resizeHanlder = debounce(() => {
      if (this.chart) {
        this.chart.resize()
      }
    }, 100)
    window.addEventListener('resize', this.__resizeHanlder)
  },
  beforeDestroy() {
    if (!this.chart) {
      return
    }
    window.removeEventListener('resize', this.__resizeHanlder)
    this.chart.dispose()
    this.chart = null
  },
  methods: {
    initChart() {
      var salesData = this.salesData
      var data = []
      var titleData = ''
      salesData.forEach(item => {
        titleData = '库存：' + item.MaterialName + '状态详情' + '                                       ' + '单位：' + item.MaterialUnit
        data.push(item.Quantity)
        data.push(item.NotShelfQuantity)
      })
      console.log(data)
      this.chart = echarts.init(this.$el, 'macarons')
      this.chart.setOption({
        title: {
          text: titleData,
          x: 'center',
          y: '10px',
          textStyle: {// 主标题文本样式{"fontSize": 18,"fontWeight": "bolder","color": "#333"}
            fontFamily: 'Arial, Verdana, sans...',
            fontWeight: 'bolder',
            color: '#333',
            fontSize: 20
          }
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: { // 坐标轴指示器，坐标轴触发有效
            type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
          }
        },
        grid: {
          top: '10%',
          left: '2%',
          right: '2%',
          bottom: '3%',
          containLabel: true
        },
        xAxis: [{
          type: 'category',
          data: ['已上架数量', '未上架数量'],
          axisTick: {
            alignWithLabel: true
          }
        }],
        yAxis: [{
          type: 'value',
          data: data,
          minInterval: 5, // 标值的最小间隔
          axisTick: {
            show: false
          }
        }],
        series: [{
          name: '',
          type: 'bar',
          stack: 'vistors',
          barWidth: '60%',
          data: data,
          itemStyle: {
            normal: {
              // 随机显示
              // color: function(d) { return '#' + Math.floor(Math.random() * (256 * 256 * 256 - 1)).toString(16) }

              // 定制显示（按顺序）
              // color: function(params) {
              //   var colorList = ['#C33531', '#EFE42A', '#64BD3D', '#EE9201', '#29AAE3', '#B74AE5', '#0AAF9F', '#E89589', '#16A085', '#4A235A', '#C39BD3 ', '#F9E79F', '#BA4A00', '#ECF0F1', '#616A6B', '#EAF2F8', '#4A235A', '#3498DB']
              //   return colorList[params.dataIndex]
              // }
              // 颜色随机且渐变
              color: function(params) {
                var colorList = []
                for (var i = 0; i < 10; i++) {
                  var colorStr = Math.floor(Math.random() * 0xffffff).toString(16)
                  // 如果颜色值是五位，则补零
                  if (colorStr.length < 6) {
                    colorStr += '0'
                  }
                  if (colorStr === '005094') {
                    i--
                    continue
                  }
                  colorList.push('#' + colorStr)
                }
                return new echarts.graphic.LinearGradient(0, 1, 0, 0, [
                  { offset: 0, color: colorList[params.dataIndex] },
                  { offset: 1, color: colorList[params.dataIndex + 1].slice(0, 6) + '0' }
                ])
              }

            }
          },
          animationDuration
        }]
      })
    }
  }
}
</script>
