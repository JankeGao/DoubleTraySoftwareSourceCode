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
        titleData = '物料：' + item.MaterialName + '状态详情' + '                                       ' + '单位：' + item.MaterialUnit
        data.push(item.Quantity)
        data.push((item.Quantity - item.LockedQuantity))
        data.push(item.LockedQuantity)
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
          data: ['库存数量', '正常数量', '锁定数量'],
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
            //  随机显示
              // color: function(d) { return '#' + Math.floor(Math.random() * (256 * 256 * 256 - 1)).toString(16) }
              color: function(params) {
                // 自定义颜色
                var colorList = [
                  '#E6A23C', '#67C23A', '#F56C6C'
                ]
                return colorList[params.dataIndex]
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
