<template>
  <div class="dashboard-editor-container">

    <panel-group />

    <el-row style="background:#fff;padding:16px 16px 0;margin-bottom:32px;">
      <line-chart :chart-data="lineChartData" />
    </el-row>

    <el-row :gutter="32">
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <PieChartOut :chart-data="topOutMaterials" />
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <pie-chart :chart-data="topInMaterials" />
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <bar-chart />
        </div>
      </el-col>
    </el-row>

  </div>
</template>

<script>
import PanelGroup from './components/PanelGroup'
import LineChart from './components/LineChart'
import PieChart from './components/PieChart'
import PieChartOut from './components/PieChartOut'
import BarChart from './components/BarChart'
import { getWeekIns, getTopOutMaterials, getTopInMaterials } from '@/api/dashboard'

const lineChartData = {
  purchases: {
    expectedData: [80, 100, 121, 104, 105, 90, 100],
    actualData: [0, 0, 0, 0, 0, 0, 0]
  }
}

export default {
  name: 'DashboardAdmin',
  components: {
    // GithubCorner,
    PanelGroup,
    LineChart,
    PieChart,
    PieChartOut,
    BarChart
  },
  data() {
    return {
      lineChartData: lineChartData.purchases,
      topOutMaterials: [],
      topInMaterials: []
    }
  },
  created() {
    this.getWeekIns()
    this.getTopOutMaterials()
    this.getTopInMaterials()
  },
  methods: {
    getWeekIns() {
      getWeekIns().then(res => {
        var data = JSON.parse(res.data.Content)
        this.lineChartData.expectedData = data
      })
    },
    getTopOutMaterials() {
      getTopOutMaterials().then(res => {
        var salesData = JSON.parse(res.data.Content)
        this.topOutMaterials = salesData
      })
    },
    getTopInMaterials() {
      getTopInMaterials().then(res => {
        var salesData = JSON.parse(res.data.Content)
        this.topInMaterials = salesData
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.dashboard-editor-container {
  padding: 20px;
  background-color: rgb(240, 242, 245);
  .chart-wrapper {
    background: #fff;
    padding: 16px 16px 0;
    margin-bottom: 32px;
  }
}
</style>
