<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-row>
      <el-card>
        <div class="filter-container" style="margin-bottom:10px">
          <!-- 仓库查询 -->
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
            <!-- multiple: 是否可以多选 ； filterable: 是否可以搜索（默认false）； remote: 是否为远程搜索（默认false）； reserve-keyword: 多选且可搜索时，是否在选中一个选项后保留当前的搜索关键词(默认false)-->
            <el-option
              v-for="item in WareHouseList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
          <!-- 天数查询 -->
          <el-select
            v-model="listQuery.InactiveDays"
            :multiple="false"
            filterable
            remote
            reserve-keyword
            placeholder="呆滞天数"
            clearable
            @clear="handleFilter"
            @change="handleFilter"
          >
            <!-- multiple: 是否可以多选 ； filterable: 是否可以搜索（默认false）； remote: 是否为远程搜索（默认false）； reserve-keyword: 多选且可搜索时，是否在选中一个选项后保留当前的搜索关键词(默认false)-->
            <el-option
              v-for="item in dictionaryList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
          <!-- 日期查询 -->
          <el-date-picker
            v-model="Value"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            clearable
            format="yyyy 年 MM 月 dd 日"
            value-format="yyyy-MM-dd HH:mm:ss"
            @blur="handleFilter"
          />
          <el-input v-model="listQuery.MaterialCode" placeholder="物料编码、物料名称、物料条码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-dropdown
            size="small"
            placement="bottom"
            trigger="click"
            @command="batchOperate"
          >
            <el-button class="filter-button" style="margin-left: 10px;" type="primary">
              导出
              <i class="el-icon-arrow-down el-icon--right" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item command="All_Export">全部导出</el-dropdown-item>
              <el-dropdown-item command="Condition_Export">按条件导出</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
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
          @selection-change="CheckedFun"
          @sort-change="sortChange"
        >
          <el-table-column type="index" width="50" align="center" />
          <el-table-column :label="'状态'" width="100" align="center" show-overflow-tooltip>
            <template>
              <el-tag type="warning"><span>{{ "呆滞物料" }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'呆滞数量'" prop="CreatedTime" width="90" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'最近一次出库日期'" prop="OutTime" width="200" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.OutTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'最近一次入库日期'" prop="InTime" width="200" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.InTime }}</span>
            </template>
          </el-table-column>
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
          <el-table-column :label="'物料名称'" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listQuery.Page" :page-sizes="[15,30,45,60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
        </div>
      </el-card>
    </el-row>

  </div>
</template>
<script>
import { getInactiveStockPageRecords, getInactiveStockDictTypeList, getWarehouseList } from '@/api/stock'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'InactiveStock', // 呆滞料报表
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
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
        WareHouseName: '',
        InactiveDays: '',
        Status: undefined,
        begin: '',
        end: ''
      },
      dictionaryList: [],
      Value: '',
      downloadLoading: false,
      TableKey: 0,
      list: null,
      StockAge: 0,
      WareHouseList: [],
      checkBoxData: []
    }
  },
  watch: {
  },
  created() {
    this.getList()
    this.GetWareHouseList()
    this.getInactiveStockDictTypeList()
  },
  methods: {

    getList() {
      this.listLoading = true

      if (this.Value.length > 0) {
        this.listQuery.begin = this.Value[0]
        this.listQuery.end = this.Value[1]
      }
      getInactiveStockPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        this.Value = ''
        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    dayDiff(beginTime, endTime) {
      var dateBegin = new Date(beginTime)
      var dateEnd = new Date(endTime)
      var dateDiff = dateEnd.getTime() - dateBegin.getTime()// 时间差的毫秒数
      var dayDiff = Math.floor(dateDiff / (24 * 3600 * 1000))// 计算出相差天数
      var leave1 = dateDiff % (24 * 3600 * 1000) // 计算天数后剩余的毫秒数
      var hours = Math.floor(leave1 / (3600 * 1000))// 计算出小时数
      // // 计算相差分钟数
      // var leave2 = leave1 % (3600 * 1000) // 计算小时数后剩余的毫秒数
      // var minutes = Math.floor(leave2 / (60 * 1000)) // 计算相差分钟数
      // // 计算相差秒数
      // var leave3 = leave2 % (60 * 1000) // 计算分钟数后剩余的毫秒数
      // var seconds = Math.round(leave3 / 1000)
      return dayDiff + '天' + hours + '小时'
    },
    getInactiveStockDictTypeList() {
      getInactiveStockDictTypeList('InactiveStock').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
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
    batchOperate(command) {
      switch (command) {
        case 'All_Export':
          this.All_ExportExcel()
          break
        case 'Condition_Export':
          this.Condition_ExportExcel()
          break
      }
    },

    // 条件导出
    Condition_ExportExcel() {
      if (this.listQuery.MaterialCode === '' && this.listQuery.WareHouseCode === '' && this.listQuery.InactiveDays === '' &&
      this.listQuery.begin === '' && this.listQuery.end === '') {
        this.$confirm('您没有选择导出条件，将导出全部数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoInactiveStock'
          window.open(url)
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消'
          })
        })
      } else {
        if (this.Value.length > 0) {
          this.listQuery.begin = this.Value[0]
          this.listQuery.end = this.Value[1]
        }
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoInactiveStock?MaterialCode=' + this.listQuery.MaterialCode + '&WareHouseCode=' + this.listQuery.WareHouseCode +
        '&begin=' + this.listQuery.begin + '&end=' + this.listQuery.end + '&InactiveDays=' + this.listQuery.InactiveDays
        window.open(url)
      }
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据，共：' + this.total + '条, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api//Stock/DoInactiveStock'
        window.open(url)
        console.log(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },
    // 获取仓库列表（选择仓库按钮功能）
    GetWareHouseList() {
      getWarehouseList(this.chosedWareHouseCode).then(res => {
        var wareHouseData = JSON.parse(res.data.Content)
        this.WareHouseList = wareHouseData
      })
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

