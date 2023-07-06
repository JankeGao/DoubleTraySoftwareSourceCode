<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <!-- 筛选栏 -->
        <div class="filter-container" style="margin-bottom:20px">
          <el-input v-model="listQuery.QueryCondition" placeholder="单号、操作人、物料" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-date-picker
            v-model="Value"
            type="daterange"
            range-separator="至 "
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            clearable
            format="yyyy 年 MM 月 dd 日"
            value-format="yyyy-MM-dd HH:mm:ss"
            @blur="handleFilter"
          />
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-dropdown
            size="small"
            placement="bottom"
            trigger="click"
            @command="batchOperate"
          >
            <el-button :loading="downloadLoading" class="filter-button" type="primary">
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
          <el-table-column :label="'入库单号'" width="145" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.InCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料名称'" width="110" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上架托盘'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.TrayCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上架储位'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'载具名称'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.BoxName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'图片'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <div class="image_box">
                <el-image
                  :src="BaseUrl+scope.row.BoxUrl"
                  fit="contain"
                  :preview-src-list="[BaseUrl+scope.row.BoxUrl]"
                  style="width: 50px; height: 50px"
                >
                  <div slot="error" class="image-slot">
                    <i class="el-icon-picture-outline" />
                  </div>
                </el-image>
              </div>
            </template>
          </el-table-column>
          <el-table-column :label="'物料条码'" width="145" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'数量'" width="70" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'单位'" width="65" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Unit }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'价格'" width="65" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Remark2 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="145" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>

          <el-table-column :label="'仓库编码'" align="center" width="70" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WarehouseCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" align="center" width="110" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WarehouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'入库时间'" align="center" width="145" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.InWarehouseTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'操作人'" align="center" width="145" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.OperatorName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'备注信息'" align="center" width="145" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark }}</span>
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
import { getPageRecords, getMouldInformation, deleteMouldInformation } from '@/api/HistoryIn'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'MouldInformation',
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      dialogFormVisible: false,
      downloadLoading: false,
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        MaterialLabel: '', // 模具信息编码
        QueryCondition: '', // 查询条件
        MouldState: undefined,
        Sort: 'id',
        Code: '',
        begin: '',
        end: ''
      },
      Value: '',
      TableKey: 0,
      list: null,

      dialogStatus: ''

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
      if (this.Value !== null) {
        if (this.Value.length > 0) {
          this.listQuery.begin = this.Value[0]
          this.listQuery.end = this.Value[1]
        }
      } else {
        this.listQuery.begin = ''
        this.listQuery.end = ''
      }
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        console.log(usersData)
        this.list = usersData.rows
        this.total = usersData.total
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    clearTime() {
      this.getList()
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
      getMouldInformation().then(response => {
        this.getList()
      })
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
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
      if (this.listQuery.QueryCondition === '' && this.listQuery.begin === '' && this.listQuery.end === '') {
        this.$confirm('您没有选择导出条件，将导出全部数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          var url = process.env.VUE_APP_B
          this.downloadLoading = trueASE_API + '/api/HistoryIn/DoDownLoadTemp'
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
        this.downloadLoading = true
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/HistoryIn/DoDownLoadTemp?QueryCondition=' + this.listQuery.QueryCondition + '&begin=' + this.listQuery.begin + '&end=' + this.listQuery.end
        window.open(url)
      }
      setTimeout(() => {
        this.downloadLoading = false
      }, 1 * 2000)
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据共' + this.total + '条，是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/HistoryIn/DoDownLoadTemp'
        window.open(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },

    handleDelete(row) {
      this.$confirm('此操作将永久删除该载具箱, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.MouldInformation = Object.assign({}, row) // copy obj
        this.deleteData(this.MouldInformation)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    // 删除
    deleteData(data) {
      deleteMouldInformation(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '成功',
            message: '删除失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    }
  }
}
</script>

