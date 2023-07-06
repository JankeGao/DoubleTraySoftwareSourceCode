<template>
  <div class="app-container">
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input
          v-model="listQuery.Code"
          placeholder="领用单号"
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
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="dialogInMaterialVisibleOpen()">扫码添加</el-button>
        <el-upload
          ref="fileupload"
          style="display: inline; margin-left: 10px;margin-right: 10px;"
          action="#"
          :show-file-list="false"
          :http-request="uploadFile"
          :before-upload="beforeUpload"
          :on-exceed="handleExceed"
        >
          <el-button type="primary"><i class="el-icon-upload" />  导入</el-button>
        </el-upload>
        <el-button :loading="downloadLoading" class="filter-button" type="primary" icon="el-icon-download" @click="handleDownUpload">模板</el-button>
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
            <el-tag v-if="scope.row.RecipientsOrdersState===0" type="warning"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
            <el-tag v-if="scope.row.RecipientsOrdersState===1"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
            <el-tag v-if="scope.row.RecipientsOrdersState===2" type="danger"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'领用单号'" width="140" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用类型'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RecipientsOrdersTypeEnum }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用人'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveName }}-{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用时间'" width="135" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveDatetime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'预计归还时间'" width="135" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.PredictReturnTime }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'归还人'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReturnName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'归还时间'" width="135" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.LastTimeReturnDatetime }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'使用时间'" width="90" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RecipientsOrdersRemarks }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'添加信息'" width="120" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.CreatedUserName }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'添加时间'" width="135" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="200px" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" />
            <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" />
            <el-button size="mini" type="info" style="width:80px" @click="chosePrint(scope.row)">打印条码</el-button>
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
            <el-tag v-if="scope.row.RecipientsOrdersState===0" size="mini" type="warning"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
            <el-tag v-if="scope.row.RecipientsOrdersState===1" size="mini"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
            <el-tag v-if="scope.row.RecipientsOrdersState===2" size="mini" type="danger"><span>{{ scope.row.RecipientsOrdersDescription }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'模具条码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具编码'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具描述'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用数量'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RecipientsOrdersQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'货柜/层号/储位/'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'x轴'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'Y轴'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
<script>
import { handleSendReceiptOrder, ouLoadInInfo, getInDictTypeList, getRecipientsOrdersList, getRecipientsOrdersMaterialList, getMaterialList, createIn, getWarehouseList, deleteRecipientsOrders, editIn, getEditMaterialList } from '@/api/ReceiptManage/RecipientsOrders'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import getLodop from '@/utils/LodopFuncs.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { getLabelByCode } from '@/api/Label'
import { getSupplierList } from '@/api/Supply'

export default {
  name: 'InBill', // 入库单据
  directives: {
    elDragDialog
  },
  data() {
    var validateBarcode = (rule, value, callback) => {
      if (this.Label.Code === '' || this.Label.Code === null) {
        callback(new Error('请扫描条码'))
      } else {
      //  this.handleCheckLabel()
        callback()
      }
    }

    var validateFloat = (rule, value, callback) => {
      if (!isFloat(this.InMaterial.Quantity)) {
        callback(new Error('请输入正确的数字'))
        return
      } else {
        callback()
      }
    }
    return {
      rowData: null,
      // 分页显示总查询数据
      barcodeValue: 'test',
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 6,
        Code: '',
        Status: undefined,
        Sort: 'id'
      },
      OptionList: [],
      OptionObject: {
        Name: '',
        value: 0
      },
      dictionaryList: [],
      downloadLoading: false,
      dialogPrinterVisible: false,
      TableKey: 0,
      list: null,
      listMaterial: null,
      addMaterial: [],
      dialogFormVisible: false,
      printerList: [],
      dialogStatus: '',
      materialList: [],
      editMaterialList: [],
      loading: false,
      addMaterialGridCurrentRowIndex: undefined,
      textMap: {
        update: '编辑入库单',
        create: '创建入库单'
      },

      // 领用订单实体
      RecipientsOrders: {
        Code: '', // 编码
        InformationCode: '', // 清单编码
        Remarks: '', // 备注
        RecipientsOrdersQuantity: '', // 数量
        RecipientsOrdersState: '', // 领用状态
        InCode: '',
        PredictReturnTime: '',
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        LastTimeReturnName: '',
        LastTimeReturnDatetime: '',
        ReceiveTime: ''
      },
      // 领用清单实体
      ReceiveDetailed: {
        InCode: '', // 清单编码
        MouldCode: '' // 模具编码

      },
      In: {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: '',
        AddMaterial: [],
        Status: undefined
      },
      InMaterial: {
        MaterialCode: '',
        MaterialName: '',
        SupplierCode: '',
        SupplierName: '',
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        LocationCode: '',
        Id: 0
      },
      PrintCode: '',
      controls: [],
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入入库单号', trigger: 'blur' }],
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }],
        InputBarcode: [{ required: true, validator: validateBarcode, trigger: 'blur' }],
        InputNumber: [{ require: true, validator: validateFloat, trigger: 'blur' }]

      },
      WareHouseList: [],
      // 条码打印
      tempPrintlist: [],
      // 打印时间
      printDate: null,
      page: {
        width: 520,
        height: 350,
        pagetype: '',
        intOrient: 1
      },
      barCode: '',
      // 料仓收料
      dialogInMaterialVisible: false,
      Label: {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      },
      supplierList: [],
      editSupplierList: [],
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '已完成'
        },
        {
          Code: 1, Name: '进行中'
        },
        {
          Code: 2, Name: '待执行'
        }
      ]
    }
  },
  watch: {
    // 创建面板关闭，清空数据
    dialogInMaterialVisible(value) {
      if (!value) {
        this.resstIn()
      }
    },
    // 创建面板关闭，清空数据
    dialogFormVisible(value) {
      if (!value) {
        this.resstIn()
      }
    }
  },
  created() {
    this.getList()
    this.GetWareHouseList()
    this.getInDictTypeList()
    // this.timer()
  },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    getInDictTypeList() {
      getInDictTypeList('InType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    // 获取领用单信息
    getList() {
      this.listLoading = true
      getRecipientsOrdersList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        // console.log(usersData)
        this.list = usersData.rows
        this.total = usersData.total
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
    // 生成入库单
    handleCreate() {
      this.resstIn()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      if (this.addMaterial.length <= 0) {
        this.$message({
          title: '失败',
          message: '请添加物料明细',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.In.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.In.AddMaterial.push(element)
          })
          // 验证物料行项目数据
          if (!this.checkItem(this.In.AddMaterial[this.In.AddMaterial.length - 1])) {
            return
          }
          createIn(this.In).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: '成功',
                message: '创建成功',
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: '失败',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      getRecipientsOrdersMaterialList(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        this.tempPrintlist = resData
        var btn = document.getElementById('printBtn')
        btn.click()
      })
    },
    chosePrint(row) {
      this.rowData = row
      this.OptionObject.Name = ''
      this.dialogPrinterVisible = true
    },
    UseDefualtPrint() {
      this.handlePrintCode(this.rowData)
    },
    UseDefinedPrint() {
      this.handlePrintCode(this.rowData)
    },
    // 打印标签
    handlePrintCode(row) {
      this.dialogPrinterVisible = false
      this.$message({
        title: '成功',
        message: '打印任务已发送，请稍等',
        type: 'success',
        duration: 2000
      })
      // console.log(typeof row)
      if (row === null) {
        this.$message({
          title: '失败',
          message: '请选择需要打印条码的入库单',
          type: 'error',
          duration: 2000
        })
        return
      }
      var LODOP = getLodop()

      var iPrinterCount = LODOP.GET_PRINTER_COUNT()

      for (var i = 0; i < iPrinterCount; i++) {
        this.OptionList[i] = LODOP.GET_PRINTER_NAME(i)
        console.log(this.OptionList[i])
      }

      const data = {}
      getRecipientsOrdersMaterialList(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        resData.forEach(element => {
          this.createBarCode(element.MaterialLabel)
          // 物料编码
          this.controls.push({
            id: 111,
            type: 'atext',
            data: {
              value: element.MaterialCode,
              width: 400,
              height: 20,
              x: 20,
              y: 10,
              itemtype: 0,
              databind: {
                id: '',
                text: ''
              },
              style: {
                color: '#000',
                fontFamily: '宋体',
                fontSize: '12px',
                fontSpacing: 0,
                fontWeight: 'normal',
                fontStyle: 'normal',
                textAlign: 'left',
                border: '',
                borderType: 0,
                HOrient: 0,
                VOrient: 0
              }
            }
          })
          // 物料名称
          this.controls.push({
            id: 111,
            type: 'atext',
            data: {
              value: element.MaterialName,
              width: 400,
              height: 20,
              x: 20,
              y: 30,
              itemtype: 0,
              databind: {
                id: '',
                text: ''
              },
              style: {
                color: '#000',
                fontFamily: '宋体',
                fontSize: '12px',
                fontSpacing: 0,
                fontWeight: 'normal',
                fontStyle: 'normal',
                textAlign: 'left',
                border: '',
                borderType: 0,
                HOrient: 0,
                VOrient: 0
              }
            }
          })
          // 物料条码
          this.controls.push({
            id: 111,
            type: 'atext',
            data: {
              value: element.MaterialLabel,
              width: 400,
              height: 20,
              x: 20,
              y: 50,
              itemtype: 0,
              databind: {
                id: '',
                text: ''
              },
              style: {
                color: '#000',
                fontFamily: '宋体',
                fontSize: '12px',
                fontSpacing: 0,
                fontWeight: 'normal',
                fontStyle: 'normal',
                textAlign: 'left',
                border: '',
                borderType: 0,
                HOrient: 0,
                VOrient: 0
              }
            }
          })
          // 一维码
          this.controls.push({
            id: 1,
            type: 'aimage',
            data: {
              x: 20,
              y: 75,
              width: 400,
              height: 60,
              itemtype: 0,
              databindtype: 0,
              databind: {
                id: '',
                text: ''
              },
              style: {
                backgroundSize: 0,
                defaultimgtype: 0,
                defaultimg: this.barCode,
                HOrient: 0,
                VOrient: 0
              }
            }
          })
          var printobj = new PrintToLodop(this.controls, data, this.page)
          if (this.OptionObject.Name !== '') {
            console.log(this.OptionObject.Name)
            printobj.definedPrint(this.OptionObject.Name)
            this.rowData = null
            return
          }
          printobj.print()
          this.rowData = null
          this.controls = []
        })
      })
    },
    // 物料编辑
    handleUpdate(row) {
      this.In = Object.assign({}, row) // copy obj
      if (this.In.Status !== 0) {
        this.$message({
          title: '失败',
          message: '入库单' + this.In.Code + '执行中或已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      getRecipientsOrdersMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.addMaterial = usersData
      })
      getEditMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.editMaterialList = usersData
        this.materialList = []
        this.editMaterialList.forEach(element => {
          this.materialList.push(element)
        })
      })
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.In.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.In.AddMaterial.push(element)
          })
          // 验证物料行项目数据
          if (!this.checkItem(this.In.AddMaterial[this.In.AddMaterial.length - 1])) {
            return
          }
          const inData = Object.assign({}, this.In)
          editIn(inData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: '成功',
                message: '更新成功',
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: '失败',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该入库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.In = Object.assign({}, row) // copy obj
        this.deleteData(this.In)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    deleteData(data) {
      deleteRecipientsOrders(data).then((res) => {
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
    // 选择一行
    handleRowClick(row, column, event) {
      getRecipientsOrdersMaterialList(row.InCode).then(response => {
        var usersData = JSON.parse(response.data.Content)
        console.log(usersData)
        this.listMaterial = usersData
      })
    },
    // 创建入库单行项目
    handleCreateInMaterial() {
      const i = this.addMaterial.length + 1
      if (i > 1) {
        var item = this.addMaterial[this.addMaterial.length - 1]
        if (!this.checkItem(item)) {
          return
        }
      }
      const inMaterial = {
        MaterialCode: '',
        MaterialName: '',
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        Id: i,
        Status: 0,
        ManufactrueDate: ''
      }
      this.addMaterial.push(inMaterial)
    },
    checkItem(item) {
      if (item.MaterialCode === '') {
        this.$message({
          title: '成功',
          message: '请选择物料',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (item.BatchCode === '') {
        this.$message({
          title: '成功',
          message: '请输入批次',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (item.Quantity === 0 || item.Quantity === '0' || item.Quantity === '0.0' || item.Quantity === '0.00') {
        this.$message({
          title: '成功',
          message: '请输入数量',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (item.ManufactrueDate === '') {
        this.$message({
          title: '成功',
          message: '请输入生产日期',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (!isFloat(item.Quantity)) {
        this.$message({
          title: '成功',
          message: '请输入正确的数字（包含两位小数的数字或者不包含小数的数字）',
          type: 'error',
          duration: 2000
        })
        return false
      }
      return true
    },
    remoteMethod(query) {
      if (query !== '') {
        this.loading = true
        getMaterialList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.materialList = data
        })
        setTimeout(() => {
          this.loading = false
        }, 200)
      } else {
        this.materialList = []
        this.editMaterialList.forEach(element => {
          this.materialList.push(element)
        })
      }
    },
    remoteSupplierMethod(query) {
      if (query !== '') {
        this.loading = true
        getSupplierList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.supplierList = data
        })
        setTimeout(() => {
          this.loading = false
        }, 200)
      } else {
        this.supplierList = []
        this.editSupplierList.forEach(element => {
          this.supplierList.push(element)
        })
      }
    },
    handleDeleteInMaterial() {
      if (this.addMaterialGridCurrentRowIndex !== undefined) {
        this.addMaterial.splice(this.addMaterial.indexOf(this.addMaterialGridCurrentRowIndex), 1)
      }
    },
    materialCodeChange(row) {

    },
    addMaterialGridClick(row, column, event) {
      this.addMaterialGridCurrentRowIndex = row
    },
    // 获取仓库列表
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        // console.log(data)
        this.WareHouseList = data
      })
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
    // 上传文件
    uploadFile(item) {
    //  console.log(item)
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      // let formTwo = JSON.stringify(form)
      ouLoadInInfo(form).then(res => {
        var resData = JSON.parse(res.data.Content)
        console.log(resData)
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
            message: '导入失败:' + resData.Message,
            type: 'error',
            duration: 4000
          })
        }
      })
    },
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/RecipientsOrders/DoDownLoadTemp'
      window.open(url)
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
    },
    next_id() {
      var current_id = 0
      return function() {
        return ++current_id
      }
    },
    // 料仓收料
    // 获取Lable信息
    handleCheckLabel() {
      if (this.Label.Code === '' || this.Label.Code == null) {
        this.$message({
          title: '失败 ',
          message: '请先扫描或输入条码',
          type: 'error',
          duration: 2000
        })
        return
      }
      getLabelByCode(this.Label.Code).then(response => {
        if (typeof (response.data.Content) !== 'undefined') {
          var result = JSON.parse(response.data.Content)
          if (result === '' || result === null) {
            this.restLabel()
          } else {
            this.Label = result
          }
        } else {
          this.$message({
            title: '失败',
            message: '未获取到该条码明细，请核查输入条码信息',
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handlEntereCheckLabel(e) {
      var keyCode = window.event ? e.keyCode : e.which
      if (keyCode === 13) {
        getLabelByCode(this.Label.Code).then(response => {
          if (typeof (response.data.Content) !== 'undefined') {
            var result = JSON.parse(response.data.Content)
            if (result === '' || result === null) {
              this.restLabel()
            } else {
              this.Label = result
            }
          }
        })
      }
    },
    // 清空数据面板
    dialogInMaterialVisibleOpen() {
      this.dialogInMaterialVisible = true
      this.InMaterial.Quantity = 0
      this.resstIn()
      this.restLabel()
    },
    dialogInMaterialVisibleClose() {
      this.dialogInMaterialVisible = false
      this.resstIn()
    },
    // 添加收货物料编码
    handleAddLabel() {
      if (this.Label.MaterialCode === '' || this.Label.MaterialCode === null) {
        this.$message({
          type: 'error',
          message: '未获取到正确的标签数据，无法收料，请先获取物料信息或者确认物料信息无误后再收料'
        })
        return
      }
      if (this.In.WareHouseCode === '' || this.In.WareHouseCode === null) {
        this.$message({
          type: 'error',
          message: '未获取到仓库信息，无法收料，请选择仓库或者确认仓库信息无误后再收料'
        })
        return
      }
      if (this.InMaterial.Quantity === 0 || this.InMaterial.Quantity === '0' || this.InMaterial.Quantity === '0.0' || this.InMaterial.Quantity === '0.00') {
        this.$message({
          title: '成功',
          message: '请输入数量',
          type: 'error',
          duration: 2000
        })
        return false
      }
      if (!isFloat(this.InMaterial.Quantity)) {
        //
      } else {
        const i = this.addMaterial.length + 1
        const inMaterial = {
          MaterialCode: this.Label.MaterialCode,
          MaterialName: this.Label.MaterialName,
          BatchCode: this.Label.Batchcode,
          Quantity: this.InMaterial.Quantity,
          MaterialLabel: this.Label.Code,
          LocationCode: this.InMaterial.LocationCode,
          SupplierCode: this.Label.SupplyCode,
          Id: i,
          Status: 0
        }
        this.addMaterial.push(inMaterial)
        this.Label = {
          Code: null,
          SupplyCode: null,
          MaterialCode: null,
          ProductionDate: null,
          Batchcode: null
        }
        this.InMaterial = {
          Quantity: null,
          LocationCode: null
        }
        this.$message({
          type: 'success',
          message: '收料成功'
        })
        // this.resstIn()
      }
    },
    createLabelData() {
      if (this.addMaterial.length <= 0) {
        this.$message({
          type: 'error',
          message: '未进行收料操作，无法创建入库单'
        })
        return
      }
      this.In.AddMaterial = []
      this.addMaterial.forEach(element => {
        this.In.AddMaterial.push(element)
      })
      createIn(this.In).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogInMaterialVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '创建成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '创建失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleSendReceipt(row) {
      this.In = Object.assign({}, row) // copy obj
      if (this.In.Status !== 0) {
        this.$message({
          title: '失败',
          message: '入库单' + this.In.Code + '状态:[' + row.StatusCaption + ']不为待上架',
          type: 'error',
          duration: 2000
        })
        return
      }
      handleSendReceiptOrder(this.In).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '亮灯任务发送成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '亮灯任务发送失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    restLabel() {
      this.Label = {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      }
    },
    resstIn() {
      this.In = {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: ''
      }
      this.addMaterial = []
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    }
  }
}
</script>

