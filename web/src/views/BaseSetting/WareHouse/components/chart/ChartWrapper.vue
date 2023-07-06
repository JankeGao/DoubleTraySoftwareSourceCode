<template>
  <div ref="xwin" style="height:100%;width:100%;" />
</template>

<script>
import echarts from 'echarts'

export default {
  name: 'ChartWrapper',
  data() {
    return {
      echart: null,
      isEmpty: true
    }
  },
  mounted() {

  },
  methods: {
    loadData(option) {
      if (this.echart) {
        this.echart.dispose()
      }
      const view = this.$refs.xwin
      this.echart = echarts.init(view)
      this.echart.setOption(option)
    },
    onResize(size) {
      if (this.echart) {
        this.echart.resize()
      }
    }
  }
}
</script>
