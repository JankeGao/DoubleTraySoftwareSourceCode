<template>
  <div class="app-container">
    <el-row>
      <el-card overflow:auto>
        <div class="filter-container" style="margin-bottom:10px">
          <el-input v-model="listQuery.MaterialLabel" placeholder="物料编码、物料名称、物料条码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-input v-model="listQuery.Code" placeholder="出库单号" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
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
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        </div>
        <el-table
          :key="TableKey"
          v-loading="listLoading"
          :data="list"
          :header-cell-style="{background:'#F5F7FA'}"
          border
          size="mini"
          fit
          highlight-current-row
          style="width:100%;min-height:100%;"
        >
          <el-table-column type="expand">
            <template slot-scope="props">
              <el-form label-position="left" inline class="demo-table-expand">
                <el-form-item label="拣选时间">
                  <span>{{ props.row.PickedTime }}</span>
                </el-form-item>
                <el-form-item label="复核时间">
                  <span>{{ props.row.CheckedTime }}</span>
                </el-form-item>
                <el-form-item label="拣选人">
                  <span>{{ props.row.Operator }}</span>
                </el-form-item>
                <el-form-item label="复核人">
                  <span>{{ props.row.Checker }}</span>
                </el-form-item>
              </el-form>
            </template>
          </el-table-column>
          <el-table-column type="index" width="50" />
          <el-table-column :label="'状态'" width="110" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0" type="info"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===2"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===3"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===4" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===6" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'出库单号'" width="160" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.OutCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="130" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料名称'" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'拣选数量'" width="100" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>

          <el-table-column min-width="120" label="已拣数量" width="100" align="center">
            <template slot-scope="{row}">
              <template v-if="row.edit">
                <el-input v-model="row.RealPickedQuantity" style="width:100px" class="edit-input" size="mini" />
                <el-button
                  class="cancel-btn"
                  size="mini"
                  icon="el-icon-refresh"
                  type="warning"

                  @click="cancelEdit(row)"
                >
                  取消
                </el-button>
              </template>
              <span v-else>{{ row.RealPickedQuantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'复核数量'" width="100" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.CheckedQuantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'单位'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialUnit }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位码'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'区域名称'" width="120" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.AreaName }}</span>
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
import { getOutLabelList, HandShelfDown } from '@/api/PickManage/HandPick'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'HandPick', // 手动下架
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
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      // 物料实体
      OutMaterialLabel: {
        Id: undefined,
        OutCode: '',
        MaterialCode: '',
        Quantity: '',
        BatchCode: '',
        Status: undefined,
        IsDeleted: false,
        BillCode: '',
        LocationCode: '',
        ItemNo: '',
        OutMaterialId: 0,
        RealPickedQuantity: 0,
        PickedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        MaterialLabel: '',
        AreaCode: '',
        WareHouseCode: '',
        Operator: '',
        CheckedTime: undefined,
        Checker: ''
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入物料编码', trigger: 'blur' }],
        //  timestamp: [{ type: 'date', required: true, message: 'timestamp is required', trigger: 'change' }],
        Name: [{ required: true, message: '请输入物料名称', trigger: 'blur' }],
        Unit: [{ required: true, message: '请输入物料单位', trigger: 'blur' }]
      },
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 1, Name: '待发送'
        },
        {
          Code: 2, Name: '已发送'
        },
        {
          Code: 3, Name: '执行中'
        },
        {
          Code: 4, Name: '手动下架'
        },
        {
          Code: 5, Name: '已下架'
        },
        {
          Code: 6, Name: '已复核'
        }
      ]

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
      getOutLabelList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        const items = usersData.rows
        this.total = usersData.total
        this.list = items.map(v => {
          this.$set(v, 'edit', false)
          this.$set(v, 'OriginalPickedQuantity', 0)
          v.OriginalPickedQuantity = v.RealPickedQuantity
          return v
        })
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

    cancelEdit(row) {
      row.RealPickedQuantity = row.OriginalPickedQuantity
      row.edit = false
      this.$message({
        message: '取消操作',
        type: 'warning'
      })
    },
    confirmEdit(row) {
      row.edit = false
      // row.originalTitle = row.title
      // this.$message({
      //   message: 'The title has been edited',
      //   type: 'success'
      // })
      var value = parseFloat(row.RealPickedQuantity)
      if (isNaN(value) || value === 'Nan') {
        this.$message({
          message: '填写的已拣选数量必须为数值',
          type: 'error'
        })
        row.RealPickedQuantity = row.OriginalPickedQuantity
        row.edit = true
        return
      }

      if (value === 0) {
        this.$message({
          message: '拣选数量不能为0',
          type: 'error'
        })
        row.RealPickedQuantity = row.OriginalPickedQuantity
        row.edit = true
        return
      }
      if (value > row.Quantity) {
        this.$message({
          message: '拣选数量不能大于需求数量',
          type: 'error'
        })
        row.RealPickedQuantity = row.OriginalPickedQuantity
        row.edit = true
        return
      }
      // this.OutMaterialLabel = Object.assign({}, row) // copy obj
      HandShelfDown(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          // this.list.unshift(this.Role)
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '下架成功',
            type: 'success',
            duration: 2000
          })
        } else {
          row.RealPickedQuantity = row.OriginalPickedQuantity
          row.edit = true
          this.$message({
            title: '失败',
            message: '下架失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    HandEdit(row) {
      // 手动下架
      if (row.Status === 1) {
        row.edit = !row.edit
      }
      // 亮灯任务
      if (row.Status === 2) {
        row.edit = !row.edit
      }
    }
  }

}
</script>

<style>
  .demo-table-expand {
    font-size: 0;
  }
  .demo-table-expand label {
    width: 90px;
    color: #99a9bf;
  }
  .demo-table-expand .el-form-item {
    margin-right: 0;
    margin-bottom: 0;
    width: 50%;
  }
</style>
