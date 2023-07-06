<template>
  <div class="app-container">
    <el-card>
      <div class="filter-container" style="margin-bottom:10px">
        <el-select
          v-model="listQuery.WareHouseCode"
          :multiple="false"
          filterable
          remote
          reserve-keyword
          placeholder="全部仓库"
          clearable
          @clear="handleFilter"
          @change="handleFilter"
        >
          <el-option
            v-for="item in WareHouseList"
            :key="item.Code"
            :label="item.Name"
            :value="item.Code"
          />
        </el-select>
        <el-input v-model="listQuery.MaterialCode" placeholder="物料编码、物料名称、物料条码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
      </div>
      <el-table
        :key="TableKey"
        v-loading="listLoading"
        :data="list"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        size="mini"
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleRowClick"
      >
        <el-table-column type="index" width="50" align="center" />
        <el-table-column :label="'仓库编码'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'仓库名称'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料名称'" width="200" show-overflow-tooltip align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已上架数量'" width="180" show-overflow-tooltip align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'未上架数量'" width="180" show-overflow-tooltip align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.NotShelfQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料单位'" show-overflow-tooltip align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialUnit }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[15,30,45,60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>
    <!-- 行项目表单 -->
    <el-card class="search-card" style="margin-top:20px">
      <div class="dashboard-editor-container">
        <el-row :gutter="3">
          <!-- 柱状图 -->
          <el-col :span="12">
            <div class="chart-wrapper">
              <bar-chart :chart-data="pieMaterialStatus" />
            </div>
          </el-col>
          <!-- 饼状图 -->
          <el-col :span="12">
            <div class="chart-wrapper">
              <pie-chart :chart-data="pieMaterialStatus" />
            </div>
          </el-col>
        </el-row>
      </div>
    </el-card>

  </div>
</template>
<script>

import PieChart from './components/PieChart'
import BarChart from './components/BarChart'
import { getInventoryStatusPageRecords, getWarehouseList } from '@/api/stock'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'InventoryStatus', // 库存状态报表
  directives: {
    elDragDialog,
    waves
  },
  components: {
    PieChart,
    BarChart
  },
  data() {
    return {
      pieMaterialStatus: [],
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        MaterialCode: '',
        MaterialName: '',
        WareHouseCode: '',
        WareHouseName: ''
      },
      listMaterial: [],
      downloadLoading: false,
      TableKey: 0,
      list: null,
      WareHouseList: [],
      checkBoxData: []
    }
  },
  watch: {
  },
  created() {
    this.getList()
    this.GetWareHouseList()
  },
  methods: {
    getList() {
      this.listLoading = true

      getInventoryStatusPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 数据筛选
    handleFilter() {
      this.listQuery.Page = 1
      this.getList()
    },
    // 切换分页数据-行数据
    handleSizeChange(val) {
      this.listQuery.Rows = val
      this.getList()
    },
    // 切换分页-列数据
    handleCurrentChange(val) {
      this.listQuery.Page = val
      this.getList()
    },
    handlePtl(row) {

    },
    // 获取仓库列表（选择仓库按钮功能）
    GetWareHouseList() {
      getWarehouseList(this.chosedWareHouseCode).then(res => {
        var wareHouseData = JSON.parse(res.data.Content)
        this.WareHouseList = wareHouseData
      })
    },
    handleRowClick(row, column, event) {
      console.log(row)
      // getMaterialStatusList(row.MaterialCode).then(res => {
      //   var resData = JSON.parse(res.data.Content)
      //   this.pieMaterialStatus = resData
      //   console.log(this.pieMaterialStatus)
      // })
      this.pieMaterialStatus = []
      this.pieMaterialStatus.push(row)
      console.log(this.pieMaterialStatus)
    },
    CheckedFun(val) {
      this.checkBoxData = val
    },
    sortChange(column, prop, order) {
      this.listQuery.Sort = column.prop
      if (column.order === 'ascending') {
        this.listQuery.Order = 'asc'
      } else if (column.order === 'descending') {
        this.listQuery.Order = 'desc'
      } else {
        this.listQuery.Order = null
      }
      this.getList()
    }
  }

}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.dashboard-editor-container {
  padding-left: 20px;
  padding-right: 20px;
  padding-top: 30px;
  background-color: rgb(240, 242, 245);
  .chart-wrapper {
    background: #fff;
    padding: 16px 16px 0;
    margin-bottom: 32px;
  }
}
</style>

