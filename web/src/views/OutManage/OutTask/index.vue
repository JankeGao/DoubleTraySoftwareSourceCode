<template>
  <div class="app-container">
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input
          v-model="listQuery.Code"
          placeholder="出库任务号"
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
        <el-table-column :label="'出库任务号'" width="160" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'货柜'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ContainerCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'仓库'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.WareHouseCode }}-{{ scope.row.WareHouseName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'出库单据号'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.OutCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'出库类型'" width="120" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.OutDictDescription }}</span>
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
        <el-table-column :label="'待拣货托盘'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.SuggestTrayCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待拣货储位'" width="100" align="center" show-overflow-tooltip>
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
        <el-table-column :label="'物料编码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'批次'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BatchCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待拣货数量'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已拣货数量'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RealPickedQuantity }}</span>
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
            <el-button size="mini" type="primary" @click="handleConfirmFinish(scope.row)">详情</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 推荐出库物料 -->
    <el-dialog
      v-el-drag-dialog
      :title="'出库物料'"
      :visible.sync="dialogFormLabelVisible"
      :fullscreen="true"
      :close-on-click-modal="false"
    >
      <el-table
        :key="TableKey"
        v-loading="false"
        :data="listLabelMaterial"
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
            <el-tag v-if="scope.row.Status===2" size="mini" type="danger">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="success">
              <span>{{ scope.row.StatusCaption }}</span>
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'建议物料'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待拣货托盘'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.TrayCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待拣货储位'" width="100" align="center" show-overflow-tooltip>
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
        <el-table-column :label="'物料编码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'批次'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BatchCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'待拣货数量'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'已拣货数量'" width="90" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.RealPickedQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'拣货时间'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.PickedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="'操作'"
          align="center"
          width="180px"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <el-button size="mini" type="primary" @click="handleConfirmLabel(scope.row)">执行</el-button>
            <el-button size="mini" type="primary" @click="handlePrintCode(scope.row)">打印</el-button>
            <!-- <el-button size="mini" type="primary" style="width:80px" @click="chosePrint(scope.row)">执行</el-button> -->
          </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormLabelVisible = false">取消</el-button>
      </div>
    </el-dialog>

    <!-- 执行出库 -->
    <el-dialog
      v-el-drag-dialog
      :title="'执行出库'"
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
            <el-form-item :label="'出库数量'" prop="MaterialLabel">
              <el-input
                v-model="OutTaskMaterial.OutTaskMaterialQuantity"
                :disabled="OutTaskMaterial.IsPackage===true"
                class="dialog-input"
                placeholder="请输入出库数量"
              />
            </el-form-item>
            <el-form-item :label="'物料条码'">
              <span class="dialog-input">{{ OutTaskMaterial.MaterialLabel }}</span>
            </el-form-item>
            <el-form-item :label="'物料编码'">
              <span class="dialog-input">{{ OutTaskMaterial.MaterialCode }}</span>
            </el-form-item>
            <el-form-item :label="'物料名称'">
              <span class="dialog-input">{{ OutTaskMaterial.MaterialName }}</span>
            </el-form-item>
            <el-form-item :label="'批次'">
              <span class="dialog-input">{{ OutTaskMaterial.BatchCode }}</span>
            </el-form-item>
            <el-form-item :label="'单位'">
              <span class="dialog-input">{{ OutTaskMaterial.MaterialUnit }}</span>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'货柜'">
              <span class="dialog-input">{{ OutTaskMaterial.SuggestContainerCode }}</span>
            </el-form-item>
            <el-form-item :label="'托盘'">
              <span class="dialog-input">{{ OutTaskMaterial.SuggestTrayCode }}</span>
              <span
                class="dialog-input"
              >{{ '---X轴:'+OutTaskMaterial.XLight +'---Y轴:'+OutTaskMaterial.YLight }}</span>
            </el-form-item>
            <el-form-item :label="'待拣货数量'">
              <span
                class="dialog-input"
              >{{ OutTaskMaterial.Quantity-OutTaskMaterial.RealPickedQuantity }}</span>
            </el-form-item>
            <el-form-item :label="'拣货储位'" :prop="'LocationCode'">
              <el-select
                v-model="OutTaskMaterial.SuggestLocation"
                filterable
                disabled
                placeholder="请输出库位编码"
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
        <!-- :disabled="confirmEnable==false" -->
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="shelfOutTaskMaterialItem">确认</el-button>
      </div>
    </el-dialog>

    <!-- 单据打印 -->
    <div style="visibility:hidden;height:250px; position: absolute;right:5px;top: 5px">
      <el-button id="printBtn" v-print="'#printMe'" type="text">打印</el-button>
      <span id="printMe" style="width:188px">
        <el-row :gutter="20">
          <el-col :span="17">
            <div style="font-size:20px;margin-bottom:20px">货柜-{{ PrintCode.ContainerCode }}-出库任务</div>
            <div style="font-size:12px;">
              <span>制单日期：{{ printDate }}</span>
              <span style="margin-left:20px">{{ PrintCode.OutDictDescription }}</span>
              <span>{{ PrintCode.BillCode }}</span>
            </div>
          </el-col>
          <el-col :span="7">
            <barcode
              :value="PrintCode.OutCode"
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
              <el-col :span="8">
                <span>名称</span>
              </el-col>
              <el-col :span="4">
                <span>容器</span>
              </el-col>
              <el-col :span="2">
                <span>数量</span>
              </el-col>
              <el-col :span="4">
                <span>批次</span>
              </el-col>
              <el-col :span="6">
                <span>物料编码</span>
              </el-col>
            </el-row>
          </div>
          <div v-for="item in tempPrintlist" :key="item.Id" style="margin-bottom:30px">
            <el-row>
              <el-col :span="8" style="margin-top:15px">
                <span>{{ item.MaterialName }}</span>
              </el-col>
              <el-col :span="4" style="margin-top:15px">
                <span>{{ item.BoxName }}</span>
              </el-col>
              <el-col :span="2" style="margin-top:15px">
                <span>{{ item.Quantity }}</span>
              </el-col>
              <el-col :span="4" style="margin-top:15px">
                <span>{{ item.BatchCode }}</span>
              </el-col>
              <el-col :span="6">
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
import { shelfOutTaskMaterial, postTaskMaterialLabelList, handleSendReceiptOrder, getInDictTypeList, getOutList, getOutTaskMaterialList, deleteOutTask, postDoStartContrainer, postRestoreContrainer } from '@/api/PickManage/PicktTask'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import QRCode from 'qrcode'
import getLodop from '@/utils/LodopFuncs.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { getLabelByCode } from '@/api/Label'
import { getDoCheckAuth } from '@/api/WareHouse'
import VueBarcode from 'vue-barcode'

export default {
  name: 'InTaskBill', // 出库任务单据
  directives: {
    elDragDialog
  },
  components: { 'barcode': VueBarcode },
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
      if (!isFloat(this.OutTaskMaterial.Quantity)) {
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
        update: '编辑出库单',
        create: '创建出库单'
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
      OutTaskMaterial: {
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
        Code: [{ required: true, message: '请输入出库单号', trigger: 'blur' }],
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
        Quantity: null,
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
          Code: 1, Name: '执行中 '
        },
        {
          Code: 2, Name: '已完成'
        },
        {
          Code: 3, Name: '已作废'
        }
      ],
      locationList: [],
      listLabelMaterial: [],
      dialogFormLabelVisible: false,
      confirmEnable: false
    }
  },
  watch: {
    // 创建面板关闭，清空数据
    dialogInMaterialVisible(value) {
      if (!value) {
        this.resstIn()
      }
    },
    dialogFormLabelVisible(value) {
      if (!value) {
        this.getList()
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
    // this.GetWareHouseList()
    this.getInDictTypeList()
    // this.timer()
  },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    handleChangeLocation(data) {
      var tempLocation = this.locationList.find(a => a.Code === data)
      this.OutTaskMaterial.LocationCode = data
      this.OutTaskMaterial.TrayId = tempLocation.TrayId
      this.OutTaskMaterial.TrayCode = tempLocation.TrayCode
    },
    // 获取扫描的物料条码信息
    getInMaterialByLabel() {
      if (this.Label.Code !== '') {
        getLabelByCode(this.Label.Code).then((response) => {
          if (response.status === 200) {
            var data = JSON.parse(response.data.Content)
            this.Label = data
            if (this.Label.MaterialCode !== this.OutTaskMaterial.MaterialCode) {
              this.$message({
                title: '失败',
                message: '此物料不属于本行项目出库的物料',
                type: 'error',
                duration: 4000
              })
              this.Label.Code = ''
              this.Label.Quantity = ''
              return
            }
            if (this.OutTaskMaterial.IsBatch === true) {
              if (this.OutTaskMaterial.BatchCode !== this.Label.Batchcode) {
                this.$message({
                  title: '失败',
                  message: '此条码批次与出库任务不符合',
                  type: 'error',
                  duration: 4000
                })
                this.Label.Code = ''
                this.Label.Quantity = ''
                return
              }
            }

            // 获取本物料在此货柜可存放的储位
            this.OutTaskMaterial.MaterialLabel = this.Label.Code
          } else {
            this.$message({
              title: '失败',
              message: '获取拣货条码信息失败',
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
    // 获取出库单信息
    getList() {
      this.listLoading = true
      getOutList(this.listQuery).then(response => {
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
    // 拣货执行
    handleConfirmFinish(row) {
      this.OutTaskMaterial = Object.assign({}, row) // copy obj
      // this.OutTaskMaterial.OutTaskMaterialQuantity = this.OutTaskMaterial.Quantity
      // 核验权限
      getDoCheckAuth(this.OutTaskMaterial.SuggestTrayId).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData === 0) {
          this.$message({
            title: '失败',
            message: '抱歉，您没有操作该托盘的权限',
            type: 'error',
            duration: 2000
          })
        } else {
          this.dialogFormLabelVisible = true
          this.GetTaskMaterialLabelList()
        }
      })
    },
    handleConfirmLabel(row) {
      this.$confirm('是否开始执行', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        const outLabel = Object.assign({}, row) // copy obj
        this.OutTaskMaterial.MaterialLabel = outLabel.MaterialLabel
        this.OutTaskMaterial.LocationCode = outLabel.LocationCode
        this.OutTaskMaterial.OutTaskMaterialQuantity = outLabel.Quantity
        this.dialogFormVisible = true
        this.confirmEnable = false
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消手动执行'
        })
      })
    },
    // 添加收货物料编码
    shelfOutTaskMaterialItem() {
      // if (this.Label.Quantity === '' || this.Label.Quantity === null) {
      //   this.$message({
      //     type: 'error',
      //     message: '未获取到正确的标签数量，无法出库'
      //   })
      //   return
      // }
      shelfOutTaskMaterial(this.OutTaskMaterial).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.GetTaskMaterialLabelList()
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
    GetTaskMaterialLabelList() {
      postTaskMaterialLabelList(this.OutTaskMaterial).then(response => {
        var resData = JSON.parse(response.data.Content)
        console.log(resData)
        this.listLabelMaterial = resData
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
    chosePrint(row) {
      this.rowData = row
      this.OptionObject.Name = ''
      this.dialogPrinterVisible = true
    },
    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      getOutTaskMaterialList(row.Code).then(response => {
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
          message: '请选择需要打印条码的出库单',
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
      this.createBarCode(row.MaterialLabel)
      // 物料编码
      this.controls.push({
        id: 111,
        type: 'atext',
        data: {
          value: row.MaterialCode,
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
          value: row.MaterialName,
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
          value: row.MaterialLabel,
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
    },

    // 删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该入任务单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.OutTask = Object.assign({}, row) // copy obj
        this.deleteData(this.OutTask)
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
      deleteOutTask(data).then((res) => {
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
      getOutTaskMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.listMaterial = usersData
        // console.log(this.listMaterial)
      })
    },
    // 创建出库单行项目
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
        ManufactrueDate: '',
        MaterialTypeDescription: ''
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
      this.OutTaskMaterial.Quantity = 0
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
          message: '出库单' + this.InTask.Code + '状态:[' + row.StatusCaption + ']不为待拣货',
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
      this.addMaterial = []
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    },
    handleStartContainer() {
      postDoStartContrainer(this.OutTaskMaterial).then(response => {
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
      postRestoreContrainer(this.OutTaskMaterial).then(response => {
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

