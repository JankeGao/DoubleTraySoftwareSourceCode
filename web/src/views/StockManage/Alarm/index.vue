<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <!-- 筛选栏 -->
        <div class="filter-container" style="margin-bottom:20px">
          <el-input
            v-model="listQuery.MaterialCode"
            placeholder="物料编码、物料名称、物料条码"
            class="filter-item"
            clearable
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />
          <el-input
            v-model="listQuery.WarehouseCode"
            placeholder="仓库编码、仓库名称"
            class="filter-item"
            clearable
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />
          <el-button
            v-waves
            class="filter-button"
            type="primary"
            icon="el-icon-search"
            @click="handleFilter"
          >查询</el-button>
          <el-button
            v-waves
            class="filter-button"
            type="primary"
            icon="el-icon-search"
            @click="handleCheck"
          >更新</el-button>
          <el-dropdown size="small" placement="bottom" trigger="click" @command="batchOperate">
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
        >
          <el-table-column type="index" width="50" />
          <el-table-column :label="'报警状态'" width="80" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0" type="warning">
                <span>{{ scope.row.StatusCaption }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===1" type="danger">
                <span>{{ scope.row.StatusCaption }}</span>
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'有效期到期时间'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LimitDate }}</span>
            </template>
          </el-table-column>

          <el-table-column :label="'物料名称'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'数量'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜编码'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'托盘编码'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.TrayCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位地址'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'条码'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库编码'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'生产日期'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ManufactureDateFormat }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'有效期（天）'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.ValidityPeriod }}</span>
            </template>
          </el-table-column>
          <!-- <el-table-column :label="'单位'" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialUnit }}</span>
            </template>
          </el-table-column>-->
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            :current-page="listQuery.Page"
            :page-sizes="[15,30,45,60]"
            :page-size="listQuery.Rows"
            :total="total"
            background
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </el-row>
  </div>
</template>
<script>
import { getPageRecords, getAlarmCheck } from '@/api/Alarm'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'Alarm', // 库存预警
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
        WarehouseCode: '',
        Status: undefined,
        Sort: 'id',
        MaterialName: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null
    }
  },
  watch: {

  },
  created() {
    this.getList()
  },
  methods: {

    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
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
    // 数据筛选
    handleCheck() {
      getAlarmCheck().then(response => {
        this.getList()
      })
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
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
      if (this.listQuery.WarehouseCode === '' && this.listQuery.MaterialCode === '') {
        this.$confirm('您没有输入导出条件，将导出共：' + this.total + '条, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          var url = window.PLATFROM_CONFIG.baseUrl + '/api/Alarm/DoDownLoadTemp'
          window.open(url)
          console.log(url)
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消'
          })
        })
      } else {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Alarm/DoDownLoadTemp?MaterialCode=' + this.listQuery.MaterialCode + '&WarehouseCode=' + this.listQuery.WarehouseCode
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
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Alarm/DoDownLoadTemp'
        window.open(url)
        // console.log(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    }
  }
}
</script>

