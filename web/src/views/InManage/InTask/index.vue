<template>
  <div class="app-container">
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input
          v-model="listQuery.Code"
          placeholder="入库任务号"
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
        <el-button
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleFilter"
        >查询</el-button>
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
        <el-table-column :label="'状态'" width="110" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===1">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===2" type="success">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===3" type="info">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'入库任务号'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'仓库'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseCode }}-{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'货柜'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ContainerCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'入库单据号'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.InCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'入库类型'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.InDictDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'下发人'" width="120" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.CreatedUserName }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'下发时间'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="'操作'"
          align="center"
          width="100px"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <el-button size="mini" type="info" @click="handlePrint(scope.row)">单据</el-button>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          :current-page="listQuery.Page"
          :page-sizes="[6,12,18,24]"
          :page-size="listQuery.Rows"
          :total="total"
          background
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
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
            <el-tag v-if="scope.row.Status===0" size="mini" type="warning">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===1" size="mini">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===2" size="mini" type="success">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="info">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'待上架托盘'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SuggestTrayCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待上架储位'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SuggestLocation }}</span>
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
        <el-table-column :label="'X轴灯号'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.XLight }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'Y轴灯号'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.YLight }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料描述'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'类别'" width="60" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'供应商名称'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SupplierName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'批次'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BatchCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待上架数量'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已上架数量'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RealInQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'上架时间'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ShelfTime }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="'操作'"
          align="center"
          width="150px"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <el-button size="mini" type="primary" @click="handleConfirmFinish(scope.row)">执行</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="'物料入库'"
      :visible.sync="dialogFormVisible"
      :width="'70%'"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="Label"
        class="dialog-form"
        label-width="100px"
        label-position="left"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item
              v-if="InTaskMaterial.IsPackage===true"
              :label="'物料条码'"
              prop="MaterialLabel"
            >
              <span>
                <el-input
                  v-model="Label.Code"
                  class="dialog-input"
                  clearable
                  placeholder="请扫描或输入物料条码"
                  @keyup.enter.native="getInMaterialByLabel"
                />
              </span>
              <span>
                <el-button
                  class="filter-button"
                  type="primary"
                  icon="el-icon-search"
                  @click="getInMaterialByLabel"
                />
              </span>
            </el-form-item>
            <el-form-item :label="'入库数量'" prop="MaterialLabel">
              <el-input
                v-model="Label.Quantity"
                :disabled="InTaskMaterial.IsPackage===true"
                class="dialog-input"
                width="200px"
                placeholder="请输入入库数量"
              />
            </el-form-item>
            <el-form-item :label="'物料编码'">
              <span class="dialog-input">{{ InTaskMaterial.MaterialCode }}</span>
            </el-form-item>
            <el-form-item :label="'物料名称'">
              <span class="dialog-input">{{ InTaskMaterial.MaterialName }}</span>
            </el-form-item>
            <el-form-item :label="'批次'">
              <span class="dialog-input">{{ InTaskMaterial.BatchCode }}</span>
            </el-form-item>
            <el-form-item :label="'单位'">
              <span class="dialog-input">{{ InTaskMaterial.MaterialUnit }}</span>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'货柜'">
              <span class="dialog-input">{{ InTaskMaterial.SuggestContainerCode }}</span>
            </el-form-item>
            <el-form-item :label="'托盘'">
              <span class="dialog-input">{{ InTaskMaterial.SuggestTrayCode }}</span>

              <span
                class="dialog-input"
              >{{ '---X轴:'+InTaskMaterial.XLight +'---Y轴:'+InTaskMaterial.YLight }}</span>
            </el-form-item>
            <el-form-item :label="'待上架数量'">
              <span class="dialog-input">{{ InTaskMaterial.Quantity-InTaskMaterial.RealInQuantity }}</span>
            </el-form-item>
            <el-form-item :label="'上架储位'" :prop="'LocationCode'">
              <el-select
                v-model="InTaskMaterial.LocationCode"
                filterable
                disabled
                placeholder="请输入库位编码"
                style="width:300px"
                @change="handleChangeLocation"
              >
                <el-option
                  v-for="item in locationList"
                  :key="item.Code"
                  :label="item.Code"
                  :value="item.Code"
                >
                  <span style="float: left">托盘:{{ item.TrayCode }}</span>
                  <span style="float: right; color: #8492a6; font-size: 13px">储位：{{ item.Code }}</span>
                </el-option>
              </el-select>
            </el-form-item>
            <el-button type="primary" @click="handleStartContainer">启动货柜</el-button>
            <el-button type="primary" @click="handleRestoreContainer">存入货柜</el-button>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="shelfInTaskMaterialItem">确认</el-button>
      </div>
    </el-dialog>

    <!-- 单据打印 -->
    <div style="visibility:hidden;height:250px; position: absolute;right:5px;top: 5px">
      <el-button id="printBtn" v-print="'#printMe'" type="text">打印</el-button>
      <span id="printMe" style="width:188px">
        <el-row :gutter="20">
          <el-col :span="17">
            <div style="font-size:20px;margin-bottom:20px">货柜-{{ PrintCode.ContainerCode }}-入库任务</div>
            <div style="font-size:12px;">
              <span>制单日期：{{ printDate }}</span>
              <span style="margin-left:20px">{{ PrintCode.InDictDescription }}</span>
              <span>{{ PrintCode.BillCode }}</span>
            </div>
          </el-col>
          <el-col :span="7">
            <barcode
              :value="PrintCode.InCode"
              height="20"
              font="12"
              width="1px"
              margin="0px"
            >Show this if the rendering fails.</barcode>
          </el-col>
        </el-row>
        <hr style="margin:10px 0px">
        <div style="font-size:12px">
          <div style="font-size:16px;margin:20px 0px">
            <el-row>
              <el-col :span="10">
                <span>名称</span>
              </el-col>
              <el-col :span="2">
                <span>数量</span>
              </el-col>
              <el-col :span="4">
                <span>批次</span>
              </el-col>
              <el-col :span="8">
                <span>物料编码</span>
              </el-col>
            </el-row>
          </div>
          <div v-for="item in tempPrintlist" :key="item.Id" style="margin-bottom:30px">
            <el-row>
              <el-col :span="10" style="margin-top:15px">
                <span>{{ item.MaterialName }}</span>
              </el-col>
              <el-col :span="2" style="margin-top:15px">
                <span>{{ item.Quantity }}</span>
              </el-col>
              <el-col :span="4" style="margin-top:15px">
                <span>{{ item.BatchCode }}</span>
              </el-col>
              <el-col :span="8">
                <barcode
                  :value="item.MaterialCode"
                  height="20px"
                  width="1px"
                  font="8"
                  display-value="false"
                >Show this if the rendering fails.</barcode>
              </el-col>
            </el-row>
          </div>
        </div>
      </span>
    </div>
  </div>
</template>
<script>
import { shelfInTaskMaterial, getLocationList, handleSendReceiptOrder, getInDictTypeList, getInList, getInTaskMaterialList, postDoStartContrainer, postRestoreContrainer } from '@/api/ReceiptManage/ReceiptTask'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import QRCode from 'qrcode'
import getLodop from '@/utils/LodopFuncs.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { getLabelByCode } from '@/api/Label'
import { getDoCheckAuth } from '@/api/WareHouse'
import VueBarcode from 'vue-barcode'

export default {
  name: 'InTaskBill', // 入库任务单据
  components: { 'barcode': VueBarcode },
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
      if (!isFloat(this.InTaskMaterial.Quantity)) {
        callback(new Error('请输入正确的数字'))
        return
      } else {
        callback()
      }
    }
    return {
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
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
      InTask: {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: '',
        AddMaterial: [],
        Status: undefined
      },
      InTaskMaterial: {
        MaterialCode: '',
        MaterialName: '',
        SupplierCode: '',
        SupplierName: '',
        RealInQuantity: 0,
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
        Quantity: null,
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
          Code: 0, Name: '待执行'
        },
        {
          Code: 1, Name: '执行中'
        },
        {
          Code: 2, Name: '已完成'
        },
        {
          Code: 3, Name: '已作废'
        }
      ],
      locationList: [],
      confirmEnable: false
    }
  },
  watch: {
    // 创建面板关闭，清空数据
    dialogFormVisible(value) {
      if (!value) {
        this.resstIn()
      }
    }
  },
  created() {
    this.getList()
    this.getInDictTypeList()
  },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    handleChangeLocation(data) {
      var tempLocation = this.locationList.find(a => a.Code === data)
      this.InTaskMaterial.LocationCode = data
      this.InTaskMaterial.TrayId = tempLocation.TrayId
      this.InTaskMaterial.TrayCode = tempLocation.TrayCode
    },
    // 获取扫描的物料条码信息
    getInMaterialByLabel() {
      if (this.Label.Code !== null && this.Label.Code !== '') {
        getLabelByCode(this.Label.Code).then((response) => {
          if (response.status === 200) {
            console.log(response)
            if (response.data.Data === null || response.data.Data === '') {
              this.$message({
                title: '失败',
                message: '未获取到条码信息',
                type: 'error',
                duration: 4000
              })
              this.Label.Code = ''
              this.Label.Quantity = ''
              return
            }
            var data = JSON.parse(response.data.Content)
            this.Label = data

            if (this.Label.MaterialCode !== this.InTaskMaterial.MaterialCode) {
              this.$message({
                title: '失败',
                message: '此物料不属于本行项目入库的物料',
                type: 'error',
                duration: 4000
              })
              this.Label.Code = ''
              this.Label.Quantity = ''
              return
            }
            if (this.InTaskMaterial.IsBatch === true) {

            }

            // 获取本物料在此货柜可存放的储位
            this.InTaskMaterial.MaterialLabel = this.Label.Code

            getLocationList(this.InTaskMaterial).then((response) => {
              var data = JSON.parse(response.data.Content)
              this.locationList = data.Data
              console.log(this.locationList)
            })
          } else {
            this.$message({
              title: '失败',
              message: '获取上架条码信息失败',
              type: 'error',
              duration: 2000
            })
          }
        })
      }
    },
    getInDictTypeList() {
      getInDictTypeList('InType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    // 获取入库单信息
    getList() {
      this.listLoading = true
      getInList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        if (this.list.length > 0) {
          this.handleRowClick(this.list[0])
        }
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
    // 上架执行
    handleConfirmFinish(row) {
      this.$confirm('是否开始执行', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.InTaskMaterial = Object.assign({}, row) // copy obj
        if (this.InTaskMaterial.Status === 3 || this.InTaskMaterial.Status === 2) {
          this.$message({
            title: '失败',
            message: '状态:[' + row.StatusCaption + ']不可执行',
            type: 'error',
            duration: 2000
          })
          return
        }
        this.InTaskMaterial.LocationCode = this.InTaskMaterial.SuggestLocation
        this.InTaskMaterial.MaterialLabel = this.Label.Code
        // 核验权限
        getDoCheckAuth(this.InTaskMaterial.SuggestTrayId).then(response => {
          var resData = JSON.parse(response.data.Content)
          if (resData === 0) {
            this.$message({
              title: '失败',
              message: '抱歉，您没有操作该托盘的权限',
              type: 'error',
              duration: 2000
            })
          } else {
            this.dialogFormVisible = true
            this.confirmEnable = false
          }
        })
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消手动执行'
        })
      })
    },
    // 添加收货物料编码
    shelfInTaskMaterialItem() {
      if (this.Label.Quantity === '' || this.Label.Quantity === null) {
        this.$message({
          type: 'error',
          message: '未获取到正确的标签数量，无法入库'
        })
        return
      }
      if (this.Label.Quantity > (this.InTaskMaterial.Quantity - this.InTaskMaterial.RealInQuantity)) {
        this.$message({
          type: 'error',
          message: '入库数量超过应收数量!'
        })
        return
      }

      this.InTaskMaterial.InTaskMaterialQuantity = this.Label.Quantity
      console.log(this.InTaskMaterial)
      shelfInTaskMaterial(this.InTaskMaterial).then(response => {
        var resData = JSON.parse(response.data.Content)
        console.log(resData)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.handleRowClick(this.InTask)
          this.getList()
          this.$message({
            title: '成功',
            message: '执行成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '执行失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 生成二维码
    createBarCode(data) {
      var JsBarcode = require('jsbarcode')
      const canvas = document.createElement('canvas')
      var settings = {
        format: this.format,
        height: 40,
        width: 1,
        margin: 0,
        displayValue: false
      }
      JsBarcode(canvas, data, settings)
      this.barCode = canvas.toDataURL('image/jpeg')
    },
    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      getInTaskMaterialList(row.Code).then(response => {
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
    },
    // 选择一行
    handleRowClick(row) {
      this.InTask = Object.assign({}, row) // copy obj
      getInTaskMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.listMaterial = usersData
      })
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
      getInTaskMaterialList(row.Code).then(response => {
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
          // var LODOP = getLodop()
          // LODOP.PREVIEW()
          this.controls = []
        })
      })
    },
    // 清空数据面板
    dialogInMaterialVisibleOpen() {
      this.dialogInMaterialVisible = true
      this.InTaskMaterial.Quantity = 0
      this.resstIn()
      this.restLabel()
    },
    dialogInMaterialVisibleClose() {
      this.dialogInMaterialVisible = false
      this.resstIn()
    },
    handleSendReceipt(row) {
      this.InTask = Object.assign({}, row) // copy obj
      if (this.InTask.Status !== 0) {
        this.$message({
          title: '失败',
          message: '入库单' + this.InTask.Code + '状态:[' + row.StatusCaption + ']不为待上架',
          type: 'error',
          duration: 2000
        })
        return
      }
      handleSendReceiptOrder(this.InTask).then((res) => {
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
      this.InTask = {
        Code: '',
        BillCode: '',
        WareHouseCode: '',
        InDict: undefined,
        Remark: ''
      }
      this.Label = {
        Code: null,
        SupplyCode: null,
        MaterialCode: null,
        ProductionDate: null,
        Batchcode: null
      }
      this.InTaskMaterial = {
        MaterialCode: '',
        MaterialName: '',
        SupplierCode: '',
        SupplierName: '',
        RealInQuantity: 0,
        BatchCode: '',
        Quantity: 0,
        MaterialLabel: '',
        LocationCode: '',
        Id: 0
      }
      this.addMaterial = []
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    },
    handleStartContainer() {
      postDoStartContrainer(this.InTaskMaterial).then(response => {
        var resData = JSON.parse(response.data.Content)
        console.log(resData)
        if (resData.Success) {
          this.confirmEnable = true
          this.$message({
            title: '成功',
            message: '执行成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '执行失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleRestoreContainer() {
      postRestoreContrainer(this.InTaskMaterial).then(response => {
        var resData = JSON.parse(response.data.Content)
        console.log(resData)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '执行成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '执行失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    }
  }
}
</script>

