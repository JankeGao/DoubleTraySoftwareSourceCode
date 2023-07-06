<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <!-- 筛选栏 -->
        <div class="filter-container" style="margin-bottom:20px">
          <el-select
            v-model="listQuery.Status"
            :multiple="false"
            filterable
            remote
            reserve-keyword
            @change="handleFilter"
          >
            <el-option
              v-for="item in statusList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
          <el-input v-model="listQuery.MaterialCode" placeholder="物料编码、物料名称" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <!-- <el-input v-model="listQuery.WareHouseCode" placeholder="仓库编码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" /> -->
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleCheck">更新</el-button>
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
        >
          <el-table-column type="index" width="50" />
          <el-table-column :label="'报警状态'" width="150" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===1" type="info"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===2" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===3" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'库存上限'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaxNum }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库存下限'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MinNum }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库存数量'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span v-if="scope.row.Status===2" style="color:red">{{ scope.row.Quantity }}</span>
              <span v-else>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <!-- <el-table-column :label="'库存导出数量'" width="100" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Quantityes }}</span>
            </template>
          </el-table-column> -->
          <el-table-column :label="'物料编码'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上架储位'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <!-- <el-table-column :label="'仓库编码'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位地址'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column> -->
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
import { getPageRecords, getNumAlarmCheck } from '@/api/NumAlarm'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'NumAlarm', // 库存上下限预警
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
        WareHouseCode: '',
        Status: undefined,
        Sort: 'id',
        MaterialName: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '已达到最小库存'
        },
        {
          Code: 1, Name: '已达到最大库存'
        },
        {
          Code: 2, Name: '小于最小库存'
        },
        {
          Code: 3, Name: '大于最大库存'
        }
      ]
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    // dialogTreeVisible(value) {
    //   if (!value) {
    //     this.resetModuleAuthData()
    //   }
    // }
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
      this.listLoading = true
      getNumAlarmCheck().then(response => {
        this.getList()
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 导出选择
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
      if (this.listQuery.MaterialCode === '' && this.listQuery.WareHouseCode === '') {
        this.$confirm('没有选择导出条件，将导出共：' + this.total + '条数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          var url = window.PLATFROM_CONFIG.baseUrl + '/api/NumAlarm/DoDownLoadTemp'
          window.open(url)
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消'
          })
        })
      } else {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/NumAlarm/DoDownLoadTemp?MaterialCode=' + this.listQuery.MaterialCode + '&WareHouseCode=' + this.listQuery.WareHouseCode
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
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/NumAlarm/DoDownLoadTemp'
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

