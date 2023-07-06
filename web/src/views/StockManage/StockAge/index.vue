<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-row>
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
            <!-- multiple: 是否可以多选 ； filterable: 是否可以搜索（默认false）； remote: 是否为远程搜索（默认false）； reserve-keyword: 多选且可搜索时，是否在选中一个选项后保留当前的搜索关键词(默认false)-->
            <el-option
              v-for="item in WareHouseList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
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
          <el-input v-model="listQuery.SupplierCode" placeholder="供应商编码、供应商名称" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
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
          @selection-change="CheckedFun"
          @sort-change="sortChange"
        >
          <el-table-column type="index" width="50" align="center" />
          <el-table-column :label="'物料名称'" width="200" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库龄'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.StockAge }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'供应商名称'" width="120" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.SupplierName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'批次'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.BatchCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'总数量'" width="90" align="center">
            <template slot-scope="scope">
              <span style="color:green">{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'单位'" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialUnit }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料条码'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column sortable="custom" :label="'上架时间'" prop="CreatedTime" width="200" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedTime }}</span>
            </template>
          </el-table-column>
          <!-- <el-table-column :label="'锁定数量'" width="90" align="center">
            <template slot-scope="scope">
              <span style="color:red">{{ scope.row.LockedQuantity }}</span>
            </template>
          </el-table-column> -->
          <!-- <el-table-column :label="'仓库编码'" width="90" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseCode }}</span>
            </template>
          </el-table-column> -->
          <el-table-column :label="'仓库名称'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位地址'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
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
import { getWarehouseList, ouLoadStockInfo, getPageRecords, LightStock, OffLightStock, DeleteStockArray } from '@/api/stock'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'StockAge', // 库龄报表
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
        WareHouseName: '',
        SupplierCode: '',
        Status: undefined,
        //   Sort: 'id',
        MaterialName: '',
        begin: '',
        end: ''
        // Order: ''
      },
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
    // 授权面板关闭，清空原角色查询权限
    // dialogTreeVisible(value) {
    //   if (!value) {
    //     this.resetModuleAuthData()
    //   }
    // }
  },
  created() {
    this.getList()
    this.GetWareHouseList()
  },
  methods: {

    getList() {
      this.listLoading = true
      var date = new Date()
      var year = date.getFullYear()
      var month = date.getMonth() + 1
      var day = date.getDate()
      var hours = date.getHours()
      var minutes = date.getMinutes()
      var seconds = date.getSeconds()
      var endTime = year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds

      if (this.Value.length > 0) {
        this.listQuery.begin = this.Value[0]
        this.listQuery.end = this.Value[1]
      }
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        usersData.rows.forEach(item => {
          if (item.CreatedTime !== null && item.CreatedTime !== 0) {
            this.StockAge = this.dayDiff(item.CreatedTime, endTime)
            item.StockAge = this.StockAge
          }
        })
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
    beforeUpload(file) {
      // console.log('beforeUpload')
      // console.log(file.type)
      const isText = file.type === 'application/vnd.ms-excel'
      const isTextComputer = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      return (isText | isTextComputer)
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，请删除后继续上传`)
    },
    // 上传文件
    uploadFile(item) {
    //  console.log(item)
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      ouLoadStockInfo(form).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '导入成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '导入失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownLoadTemp'
      window.open(url)
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
      if (this.listQuery.MaterialCode === '' && this.listQuery.WareHouseCode === '' && this.listQuery.SupplierCode === '' &&
      this.listQuery.begin === '' && this.listQuery.end === '') {
        this.$confirm('您没有选择导出条件，将导出全部数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownStockAge'
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
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownStockAge?MaterialCode=' + this.listQuery.MaterialCode + '&WareHouseCode=' + this.listQuery.WareHouseCode +
        '&SupplierCode=' + this.listQuery.SupplierCode + '&begin=' + this.listQuery.begin + '&end=' + this.listQuery.end
        window.open(url)
      }
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据共' + this.total + '条，是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Stock/DoDownStockAge'
        window.open(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
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
    },
    handleLight() {
      if (this.checkBoxData.length > 0) {
        this.$confirm('确定点亮勾选的库存?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          LightStock(this.checkBoxData).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '点亮成功',
                type: 'success',
                duration: 2000
              })
              this.dialogFormSort = false
              // this.getList()
            } else {
              this.$message({
                title: '失败',
                message: '点亮失败' + resData.Message,
                type: 'error',
                duration: 5000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选拣货单',
          type: 'error',
          duration: 5000
        })
      }
    },
    handleOffLight() {
      this.$confirm('确定点亮勾选的库存?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        OffLightStock(this.checkBoxData).then(res => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            this.$message({
              title: '成功',
              message: '熄灭成功',
              type: 'success',
              duration: 2000
            })
            this.dialogFormSort = false
            // this.getList()
          } else {
            this.$message({
              title: '失败',
              message: '熄灭失败' + resData.Message,
              type: 'error',
              duration: 5000
            })
          }
        })
      })
    },
    handleShelfDwon() {
      if (this.checkBoxData.length > 0) {
        this.$confirm('确定下架勾选的条码?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          DeleteStockArray(this.checkBoxData).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '直接下架条码成功',
                type: 'success',
                duration: 2000
              })
              this.dialogFormSort = false
              this.getList()
            } else {
              this.$message({
                title: '失败',
                message: '直接下架失败' + resData.Message,
                type: 'error',
                duration: 5000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选拣货单',
          type: 'error',
          duration: 5000
        })
      }
    }
  }

}
</script>

