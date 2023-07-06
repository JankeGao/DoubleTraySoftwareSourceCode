<template>
  <div :class="className" :style="{height:height,width:width}" />
</template>

<script>
import echarts from 'echarts'
require('echarts/theme/macarons') // echarts theme
import { debounce } from '@/utils'

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
      chart: null
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
      var nameData = []
      var data = []
      var num = 5
      var titleData = ''
      if (salesData.length < num) {
        num = salesData.length
      }
      for (var i = 0; i < num; i++) {
        titleData = '物料：' + salesData[i].MaterialName + '状态详情' + '                                       ' + '单位：' + salesData[i].MaterialUnit
        nameData.push('库存数量')
        nameData.push('正常数量')
        nameData.push('锁定数量')
        data.push({ 'value': (salesData[i].Quantity - salesData[i].LockedQuantity), 'name': '正常数量' })
        data.push({ 'value': salesData[i].LockedQuantity, 'name': '锁定数量' })
        data.push({ 'value': salesData[i].Quantity, 'name': '库存数量' })
      }
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
          trigger: 'item',
          formatter: '{a} <br/>{b} : {c} ({d}%)'
        },
        legend: {
          left: 'center',
          bottom: '10',
          data: nameData,
          textStyle: {
            fontSize: 12, // 字体大小
            color: '#000'// 字体颜色
          }
        },
        calculable: true,

        series: [
          {
            name: '物料状态',
            type: 'pie',
            roseType: 'radius',
            center: ['50%', '45%'],
            radius: '50%', // 饼图的半径大小
            data: data,
            animationDuration: 2600,
            label: {
              normal: {
                textStyle: {
                  color: 'rgba(255, 255, 255, 0.3)'
                }
              }
            },
            labelLine: {
              normal: {
                lineStyle: {
                  color: 'rgba(255, 255, 255, 0.3)'
                },
                smooth: 0.2,
                length: 10,
                length2: 20
              }
            },
            itemStyle: {
              normal: {
                color: function(params) {
                  // 自定义颜色
                  var colorList = [
                    '#67C23A', '#F56C6C', '#E6A23C'
                  ]
                  return colorList[params.dataIndex]
                },
                shadowBlur: 200,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            },

            animationType: 'scale',
            animationEasing: 'elasticOut',
            animationDelay: function(idx) {
              return Math.random() * 200
            }
          }
        ]
      })
    }
  }
}
</script>
