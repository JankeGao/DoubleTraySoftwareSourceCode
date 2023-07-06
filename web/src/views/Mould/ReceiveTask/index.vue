<template>
  <div class="app-container">
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input
          v-model="listQuery.Code"
          placeholder="领用任务单号"
          class="filter-item"
          clearable
          @keyup.enter.native="handleFilter"
          @clear="handleFilter"
        />
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
        <el-button class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        <!-- <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button> -->
        <!-- <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="dialogInMaterialVisibleOpen()">扫码添加</el-button> -->
        <!-- <el-button :loading="downloadLoading" class="filter-button" type="primary" icon="el-icon-download" @click="handleDownUpload">模板</el-button> -->
      </div>
      <el-table
        :key="TableKey"
        v-loading="listLoading"
        :data="list"
        :header-cell-style="{background:'#F5F7FA'}"
        size="mini"
        height="331"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
        @row-click="handleRowClick"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'状态'" width="90" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2" type="success"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="info"><span>{{ scope.row.StatusDescription }}</span></el-tag>
          </template></el-table-column>
        <el-table-column :label="'任务编码'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'货柜'" width="120" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.ContainerCode }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'领用类型'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTypeEnumDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用人'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用时间'" width="155" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveDatetime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'预计归还时间'" width="135" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.PredictReturnTime }}</div>
          </template>
        </el-table-column>
        <!-- <el-table-column :label="'使用时间'" width="90" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTime }}</span>
          </template>
        </el-table-column> -->
        <el-table-column :label="'领用备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加时间'" width="135" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="100px" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button size="mini" type="info" @click="handlePrint(scope.row)">单据</el-button>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[6,12,18,24]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>
    <!-- 行项目 -->
    <el-card>
      <el-table
        :key="TableKey"
        v-loading="false"
        :data="listMaterial"
        :header-cell-style="{background:'#F5F7FA'}"
        :height="300"
        size="mini"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'状态'" width="100" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" size="mini" type="warning"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1" size="mini"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2" size="mini" type="success"><span>{{ scope.row.StatusDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="info"><span>{{ scope.row.StatusDescription }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'领用类型'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'载具名称'" align="center" show-overflow-tooltip>
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
        <el-table-column :label="'领用数量'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用人'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用时间'" width="135" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.LastTimeReceiveDatetime }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'归还人'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'归还时间'" width="135" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.LastTimeReturnDatetime }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'使用时间(min)'" width="110" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具条码'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具编码'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MouldRemarks }}</span>
          </template>
        </el-table-column>

        <el-table-column :label="'储位'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'x轴'" align="center" width="100" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.XLight }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'Y轴'" align="center" width="100" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.YLight }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="160" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-button size="mini" type="success" style="width:55px" @click="execute(scope.row)">执行</el-button>
            <el-button size="mini" type="warning" style="width:55px" @click="giveBacks(scope.row)">归还</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <el-dialog v-el-drag-dialog :visible.sync="dialogFormVisible" :width="'45%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="ReceiveTaskDetail" class="dialog-form" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item :label="'归还人'" prop="Name">
              <el-select
                v-model="TaskName.LastTimeReturnName"
                :multiple="false"
                placeholder="请选择归还人"
                filterable
                style="width:250px"
                :remote-method="remoteMethod"
                :loading="loading"
              >
                <el-option
                  v-for="item in UserInfoList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item :label="'归还时间'" prop="Name">
              <el-date-picker
                v-model="TaskName.LastTimeReturnDatetime"
                type="datetime"
                style="width:250px"
                placeholder="请选择领用时间"
                value-format="yyyy-MM-dd HH:mm:ss"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <div style="margin-bottom:10px" align="right">
          <el-button class="filter-button" style="margin-left: 10px;" type="primary" @click="dialogFormVisible = false">取消</el-button>
          <el-button class="filter-button" style="margin-left: 10px;" type="primary" @click="giveBack">确认</el-button>
        </div>
      </el-form>
    </el-dialog>
    <!-- 单据打印 -->
    <div style="visibility:hidden;height:250px; position: absolute;right:5px;top: 5px">
      <el-button id="printBtn" v-print="'#printMe'" type="text">打印</el-button>
      <span id="printMe" style="width:188px">
        <el-row :gutter="20">
          <el-col :span="17">
            <div style="font-size:20px;margin-bottom:20px">货柜-{{ PrintCode.ContainerCode }}-领用任务</div>
            <div style="font-size:12px;">
              <span>制单日期：{{ printDate }}</span>
              <span style="margin-left:20px">{{ PrintCode.OutDictDescription }}</span>
              <span>{{ PrintCode.BillCode }}</span>
            </div>
          </el-col>
          <el-col :span="7">
            <barcode
              :value="PrintCode.Code"
              height="20"
              font="12"
              width="1px"
              margin="0px"
            >
              Show this if the rendering fails.
            </barcode>
          </el-col>
        </el-row>
        <hr style="margin:10px 0px">
        <div style="font-size:12px">
          <div style="font-size:16px;margin:20px 0px">
            <el-row>
              <el-col :span="10">
                <span>模具名称</span>
              </el-col>
              <el-col :span="4">
                <span>容器</span>
              </el-col>
              <el-col :span="3">
                <span>数量</span>
              </el-col>
              <el-col :span="5">
                <span>模具编码</span>
              </el-col>
            </el-row>
          </div>
          <div v-for="item in tempPrintlist" :key="item.Id" style="margin-bottom:30px">
            <el-row>
              <el-col :span="10" style="margin-top:15px">
                <span>{{ item.MaterialName }}</span>
              </el-col>
              <el-col :span="4" style="margin-top:15px">
                <span>{{ item.BoxName }}</span>
              </el-col>
              <el-col :span="2" style="margin-top:15px">
                <span> {{ item.Quantity }}</span>
              </el-col>
              <el-col :span="6">
                <barcode
                  :value="item.MaterialLabel"
                  height="20px"
                  width="1px"
                  font="8"
                  display-value="false"
                >
                  Show this if the rendering fails.
                </barcode>
              </el-col>
            </el-row>
          </div>
        </div>
      </span>
    </div>
  </div>
</template>
<script>
import { getReceiveTaskList, getReceiveTaskDetailList, getWarehouseList, deleteReceiveTask, postHandShelf, postReturnReceiveTask } from '@/api/Mould/ReceiveTask'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getUserInfos, getUserlInfoList } from '@/api/SysManage/User'
import VueBarcode from 'vue-barcode'
export default {
  name: 'ReceiveTask', // 领用任务
  directives: {
    elDragDialog
  },
  components: { 'barcode': VueBarcode },
  data() {
    return {
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      multipleSelection: [], // 多选
      rowData: null,
      // 分页显示总查询数据
      barcodeValue: 'test',
      total: null,
      Addtotal: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 6,
        Code: '',
        Status: undefined,
        Sort: 'id'
      },
      TaskName: {
        LastTimeReturnName: '',
        LastTimeReturnDatetime: ''
      },
      downloadLoading: false,
      dialogPrinterVisible: false,
      TableKey: 0,
      list: null,
      listMaterial: null,
      addMaterial: [],
      UserInfoList: [],
      dialogFormVisible: false,
      dialogStatus: '',
      LastTimeReturnName: '',
      LastTimeReturnDatetime: '',
      loading: false,
      addMaterialGridCurrentRowIndex: undefined,
      textMap: {
        update: '编辑领用单',
        create: '创建领用单'
      },
      PrintCode: '',
      // 打印时间
      printDate: null,
      // 领用任务实体
      ReceiveTask: {
        Code: '', // 编码
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        LastTimeReturnName: '',
        LastTimeReturnDatetime: '',
        PredictReturnTime: '',
        ReceiveTime: '',
        ReceiveCode: '',
        WareHouseCode: '',
        Remarks: '', // 备注
        Status: '',
        ReceiveType: ''
      },
      // 领用任务明细实体
      ReceiveTaskDetail: {
        TaskCode: '', // 编码
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        LastTimeReturnName: '',
        LastTimeReturnDatetime: '',
        PredictReturnTime: '',
        WareHouseCode: '',
        ReceiveCode: '',
        Remarks: '', // 备注
        Status: '',
        ReceiveType: '',
        MaterialLabel: '',
        ContainerCode: ''
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入领用单号', trigger: 'blur' }],
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }]
      },
      WareHouseList: [],
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '待执行'
        },
        {
          Code: 1, Name: '进行中'
        },
        {
          Code: 2, Name: '已完成'
        },
        {
          Code: 3, Name: '已作废'
        }
      ],
      MouldList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '生产'
        },
        {
          Code: 1, Name: '修模'
        },
        {
          Code: 2, Name: '注销'
        },
        {
          Code: 3, Name: '领用锁定'
        },
        {
          Code: 4, Name: '在库中'
        }
      ],
      tempPrintlist: []
    }
  },
  watch: {
    // 创建面板关闭，清空数据
    dialogFormVisible(value) {
      if (!value) {
        this.resstReceiveTaskDetail()
      }
    }

  },
  created() {
    this.getList()
    this.GetWareHouseList()
    this.GetUserInfoList()
  },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    // 获取仓库列表
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        // console.log(data)
        this.WareHouseList = data
      })
    },
    // 获取领用单信息
    getList() {
      this.listLoading = true
      getReceiveTaskList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        if (this.list.length > 0) {
          this.handleRowClick(this.list[0])
        }
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },

    // 获取员工列表
    GetUserInfoList() {
      getUserInfos().then(response => {
        var data = JSON.parse(response.data.Content)
        data.forEach(element => {
          if (element.Code === 'admin') {
            return
          }
          this.UserInfoList.push(element)
        })
      })
    },
    remoteMethod(query) {
      if (query !== '') {
        this.loading = true
        getUserlInfoList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.UserInfoList = data
        })
        setTimeout(() => {
          this.loading = false
        }, 200)
      } else {
        this.UserInfoList = []
        this.editMaterialList.forEach(element => {
          this.UserInfoList.push(element)
        })
      }
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

    // 生成领用单
    handleCreate() {
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 执行
    execute(row) {
      this.$confirm('是否对此物料执行手动领用？手动执行将不驱动货柜运转。', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.ReceiveTaskDetail = row
        // console.log(this.rowData)
        postHandShelf(this.ReceiveTaskDetail).then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            this.$message({
              title: '成功',
              message: '执行成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
            getReceiveTaskDetailList(this.ReceiveTaskDetail.TaskCode).then(response => {
              var usersData = JSON.parse(response.data.Content)
              this.listMaterial = usersData
            })
          } else {
            this.$message({
              title: '成功',
              message: '执行失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消手动执行'
        })
      })
    },
    giveBacks(row) {
      this.dialogFormVisible = true
      this.ReceiveTaskDetail = row
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 归还
    giveBack() {
      this.$confirm('是否对此物料执行手动归还？手动执行将不驱动货柜运转。', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.ReceiveTaskDetail.LastTimeReturnName = this.TaskName.LastTimeReturnName
        this.ReceiveTaskDetail.LastTimeReturnDatetime = this.TaskName.LastTimeReturnDatetime
        postReturnReceiveTask(this.ReceiveTaskDetail).then((res) => {
          var resData = JSON.parse(res.data.Content)
          if (resData.Success) {
            this.dialogFormVisible = false
            this.$message({
              title: '成功',
              message: '归还成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
            getReceiveTaskDetailList(this.ReceiveTaskDetail.TaskCode).then(response => {
              var usersData = JSON.parse(response.data.Content)
              // console.log(usersData)
              this.listMaterial = usersData
            })
          } else {
            this.$message({
              title: '成功',
              message: '归还失败：' + resData.Message,
              type: 'error',
              duration: 2000
            })
          }
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消手动执行'
        })
      })
    },

    // 删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该入库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.ReceiveTask = Object.assign({}, row) // copy obj
        this.deleteData(this.ReceiveTask)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(data) {
      deleteReceiveTask(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '删除失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 选择一行后的明细表信息
    handleRowClick(row, column, event) {
      getReceiveTaskDetailList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        // console.log(usersData)
        this.listMaterial = usersData
      })
    },
    // 创建领用单行项目
    handleCreateOutMaterial() {
      // 核查可用库存需要选择仓库
      if (this.RecipientsOrders.WareHouseCode === '') {
        this.$message({
          title: '提示',
          message: '请选择仓库',
          type: 'warning',
          duration: 2000
        })
        return
      }
      this.addMould = true
      this.getAddList()
    },
    // 移除已添加的物料
    handleDeleteOutMaterial() {
      if (this.addMaterialGridCurrentRowIndex !== undefined) {
        this.addMaterial.splice(this.addMaterial.indexOf(this.addMaterialGridCurrentRowIndex), 1)
      }
    },
    addMaterialGridClick(row, column, event) {
      this.addMaterialGridCurrentRowIndex = row
    },
    resstReceiveTaskDetail() {
      this.ReceiveTaskDetail = {
        TaskCode: '', // 编码
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        LastTimeReturnName: '',
        LastTimeReturnDatetime: '',
        PredictReturnTime: '',
        WareHouseCode: '',
        ReceiveCode: '',
        Remarks: '', // 备注
        Status: '',
        ReceiveType: '',
        MaterialLabel: '',
        ContainerCode: ''
      }
      this.addMaterial = []
      this.TaskName = {
        LastTimeReturnName: '',
        LastTimeReturnDatetime: ''
      }
    },
    // 上传核查
    beforeUpload(file) {
      const isText = file.type === 'application/vnd.ms-excel'
      const isTextComputer = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      return (isText | isTextComputer)
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，请删除后继续上传`)
    },

    // 模板下载
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/RecipientsOrders/DoDownLoadTemp'
      window.open(url)
    },
    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      getReceiveTaskDetailList(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        this.tempPrintlist = resData
        var btn = document.getElementById('printBtn')
        btn.click()
      })
    },
    getNowFormatDate() {
      var date = new Date()
      var seperator1 = '-'
      var year = date.getFullYear()
      var month = date.getMonth() + 1
      var strDate = date.getDate()
      if (month >= 1 && month <= 9) {
        month = '0' + month
      }
      if (strDate >= 0 && strDate <= 9) {
        strDate = '0' + strDate
      }
      var currentdate = year + seperator1 + month + seperator1 + strDate
      return currentdate
    }
  }
}
</script>

