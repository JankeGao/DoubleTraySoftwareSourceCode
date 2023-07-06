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
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleInterfaceCreate">同步</el-button>
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
        <el-table-column :label="'状态'" width="100" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2" type="success"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" type="info"><span>{{ scope.row.StateDescription }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'领用单号'" width="140" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用类型'" width="70" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用人'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveUserName }}</span>
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
        <el-table-column :label="'来源单据'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BillCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
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
            <el-button size="mini" type="primary" style="width:50px" @click="chosePrint(scope.row)">下发</el-button>
            <el-button size="mini" type="info" style="width:50px" @click="cancellation(scope.row)">作废</el-button>
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
            <el-tag v-if="scope.row.Status===0" size="mini" type="warning"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1" size="mini"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===2" size="mini" type="success"><span>{{ scope.row.StateDescription }}</span></el-tag>
            <el-tag v-if="scope.row.Status===3" size="mini" type="info"><span>{{ scope.row.StateDescription }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'领用类型'" width="70" aligill-coden="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remarks }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'领用数量'" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具条码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具编码'" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'储位'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'x轴'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.XLight }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'Y轴'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.YLight }}</span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'80%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Receive" class="dialog-form" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="'仓库编码'" prop="WareHouseCode">
              <el-select
                v-model="Receive.WareHouseCode"
                :multiple="false"
                filterable
                style="width:300px"
              >
                <el-option
                  v-for="item in WareHouseList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'领用类型'" :prop="'ReceiveType'">
              <el-select
                v-model="Receive.ReceiveType"
                :multiple="false"
                reserve-keyword
                style="width:300px"
              >
                <el-option
                  v-for="item in typeList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'来源单号'" prop="DesignCode">
              <el-input v-model="Receive.DesignCode" class="dialog-input" clearable placeholder="请输入来源单据号" />
            </el-form-item>
            <el-form-item :label="'具体描述'">
              <el-input v-model="Receive.Remarks" :autosize="{ minRows: 1, maxRows: 1}" type="textarea" placeholder="领用描述" class="dialog-input" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="'领用人'" prop="LastTimeReceiveName">
              <el-select
                v-model="Receive.LastTimeReceiveName"
                :multiple="false"
                filterable
                style="width:300px"
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
          <el-col :span="12">
            <el-form-item :label="'领用时间'" prop="LastTimeReceiveDatetime">
              <el-date-picker
                v-model="Receive.LastTimeReceiveDatetime"
                type="datetime"
                placeholder="请选择领用时间"
                value-format="yyyy-MM-dd HH:mm:ss"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item :label="'预计归还时间'" prop="PredictReturnTime">
              <el-date-picker
                v-model="Receive.PredictReturnTime"
                type="datetime"
                placeholder="请选择预计归还时间"
                value-format="yyyy-MM-dd HH:mm:ss"
              />
            </el-form-item>
          </el-col>
        </el-row>

      </el-form>
      <div style="margin-bottom:20px" align="right">
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreateOutMaterial">添加</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="danger" icon="el-icon-delete" @click="handleDeleteOutMaterial">移除</el-button>
      </div>
      <el-table
        :key="TableKey"
        ref="addMaterialGrid"
        v-loading="false"
        :data="addMaterial"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%;"
        height="300px"
        @row-click="addMaterialGridClick"
      >
        <el-table-column type="index" width="50" :reserve-selection="true" />
        <el-table-column :label="'模具条码'" width="190" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'模具名称'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'储位'" width="180" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'上次领用人'" width="140" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'上次领用时间'" width="190" show-overflow-tooltip align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.LastTimeReceiveDatetime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'上次领用类别'" width="145" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ReceiveTypeDescription }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'数量'" width="145" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Quantity }}</span>
          </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData(addMaterial)">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 添加选择框 -->
    <el-dialog v-el-drag-dialog :visible.sync="addMould" :width="'95%'" :close-on-click-modal="false">
      <el-row>
        <el-card>
          <div class="filter-container" style="margin-bottom:20px">
            <el-input v-model="AddlistQuery.MaterialLabel" placeholder="模具名称" class="filter-item" clearable @keyup.enter.native="handleFilterAdd" @clear="handleFilterAdd" />
            <el-button class="filter-button" type="primary" icon="el-icon-search" @click="handleFilterAdd">查询</el-button>
          </div>
          <!-- 筛选栏 -->
          <el-table
            :key="TableKey"
            ref="multipleTable"
            v-loading="listLoading"
            :data="addList"
            :header-cell-style="{background:'#F5F7FA'}"
            size="mini"
            highlight-current-row
            style="width:100%;min-height:100%;"
            border
            fit
            :row-key="getRowKeys"
            @selection-change="changeFun"
          >
            <el-table-column type="selection" :reserve-selection="true" width="50" align="center" />
            <el-table-column type="index" width="50" />
            <el-table-column :label="'状态'" width="90" align="center">
              <template slot-scope="scope">
                <el-tag v-if="scope.row.MouldState===0" type="primary"><span>{{ scope.row.StateDescription }}</span></el-tag>
                <el-tag v-if="scope.row.MouldState===1" type="warning"><span>{{ scope.row.StateDescription }}</span></el-tag>
                <el-tag v-if="scope.row.MouldState===2" type="info"><span>{{ scope.row.StateDescription }}</span></el-tag>
                <el-tag v-if="scope.row.MouldState===3" type="info"><span>{{ scope.row.StateDescription }}</span></el-tag>
                <el-tag v-if="scope.row.MouldState===4" type="success"><span>{{ scope.row.StateDescription }}</span></el-tag>
              </template>
            </el-table-column>
            <el-table-column :label="'物料编码'" width="110" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialCode }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'模具条码'" width="140" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialLabel }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'模具名称'" width="110" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.MaterialName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上次领用人'" width="100" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.LastTimeReceiveName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上次领用时间'" width="150" show-overflow-tooltip align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.LastTimeReceiveDatetime }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上次领用类别'" width="110" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.ReceiveTypeDescription }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'备注'" width="130" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Remarks }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'领用时长'" width="80" align="center">
              <template slot-scope="scope">
                <span>{{ scope.row.ReceiveTime }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上次归还人'" width="100" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.LastTimeReturnName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="'上次归还时间'" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.LastTimeReturnDatetime }}</span>
              </template>
            </el-table-column>
          </el-table>
          <!-- 分页 -->
          <div class="pagination-container">
            <el-pagination :current-page="AddlistQuery.Page" :page-sizes="[5,10,15,20]" :page-size="AddlistQuery.Rows" :total="Addtotal" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChangeAdd" @current-change="handleCurrentChangeAdd" />
          </div>
          <div class="dialog-footer" align="right">
            <el-button @click="addMould = false ">取消</el-button>
            <el-button v-if="dialogStatus=='create'" type="primary" @click="toggleSelection">确认</el-button>
            <el-button v-else type="primary" @click="UpdatetoggleSelection">确认</el-button>
          </div>
        </el-card></el-row>
    </el-dialog>
  </div>
</template>
<script>
import { getInterfaceReceive, ouLoadInInfo, getRecipientsOrdersList, getReceiveMaterialList, getWarehouseList, deleteRecipientsOrders, createReceive, editReceive, postDoCancellatione } from '@/api/Mould/Receive'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import getLodop from '@/utils/LodopFuncs.js' // 引入附件的js文件
import { isFloat } from '@/utils/validate.js'
import PrintToLodop from '@/utils/PrintToLodop.js' // 引入附件的js文件
import { getLabelByCode } from '@/api/Label'
import { getUserInfos } from '@/api/SysManage/User'
// import { userList } from '@/api/SysManage/User'
import { issueReceiveTaskInfo } from '@/api/Mould/ReceiveTask'
import { getPageRecords } from '@/api/Mould/MouldInformation'

export default {
  name: 'Receive', // 入库单据
  directives: {
    elDragDialog
  },
  data() {
    var validateBarcode = (rule, value, callback) => {
      if (this.Label.Code === '' || this.Label.Code === null) {
        callback(new Error('请扫描条码'))
      } else {
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
      multipleSelection: [], // 多选
      SelectData: [], // 选中的数据，用来最后提交的
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
      // 添加-分页查询
      AddlistQuery: {
        Page: 1,
        Rows: 5,
        MaterialLabel: '', // 模具信息编码
        MouldState: 4, // 只显示在库中的模具信息
        Sort: 'id',
        Code: '',
        WareHouseCode: ''
      },
      OptionList: [],
      OptionObject: {
        Name: '',
        value: 0
      },
      dictionaryList: [],
      downloadLoading: false,
      dialogPrinterVisible: false,
      addMould: false, // 选择添加领用信息界面是否显示
      TableKey: 0,
      list: null,
      addList: null,
      listMaterial: null,
      addMaterial: [], // 添加选中的模具数据到领用单添加页面
      addMouldList: [],
      dialogFormVisible: false,
      printerList: [],
      dialogStatus: '',
      materialList: [],
      editMaterialList: [],
      loading: false,
      addMaterialGridCurrentRowIndex: undefined,
      textMap: {
        update: '编辑领用单',
        create: '创建领用单'
      },

      // 领用订单实体
      Receive: {
        Code: '', // 编码
        Quantity: '', // 数量
        Status: '', // 领用状态
        PredictReturnTime: '',
        ReceiveType: '', // 领用类型
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        ReceiveTime: '',
        WareHouseCode: '',
        Remarks: '', // 备注
        DesignCode: '', // 来源单号
        AddMaterial: []
      },
      // 领用清单实体
      ReceiveDetailed: {
        ReceiveCode: '', // 清单编码
        MaterialLabel: '', // 模具条码
        MaterialCode: '',
        MaterialName: '',
        Status: '',
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        LastTimeReturnName: '',
        LastTimeReturnDatetime: '',
        PredictReturnTime: '',
        LocationCode: '',
        Quantity: '' // 数量
      },
      PrintCode: '',
      controls: [],
      // 输入规则
      rules: {
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }],
        InputBarcode: [{ required: true, validator: validateBarcode, trigger: 'blur' }],
        InputNumber: [{ require: true, validator: validateFloat, trigger: 'blur' }]

      },
      // 选择仓库后，赋予addMarital=[]
      // changeWarehouse() {
      //   // this.addMaterial = []
      // },
      WareHouseList: [], // 仓库
      UserInfoList: [], // 员工
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
      typeList: [
        {
          Code: 0, Name: '生产'
        },
        {
          Code: 1, Name: '修模'
        },
        {
          Code: 2, Name: '注销'
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
        this.resstReceive()
        this.multipleSelection = []
        this.$refs.multipleTable.clearSelection()
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

    // 获取领用单信息
    getList() {
      this.listLoading = true
      getRecipientsOrdersList(this.listQuery).then(response => {
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
    // 数据筛选
    handleFilter() {
      this.listQuery.Page = 1
      this.getList()
    },
    // 添加-数据筛选
    handleFilterAdd() {
      this.AddlistQuery.Page = 1
      this.getAddList()
    },
    // 切换分页数据-行数据-添加
    handleSizeChangeAdd(val) {
      this.AddlistQuery.Rows = val
      this.getAddList()
    },
    // 切换分页-列数据 - 添加
    handleCurrentChangeAdd(val) {
      this.AddlistQuery.Page = val
      this.getAddList()
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
    // 同步
    handleInterfaceCreate() {
      getInterfaceReceive().then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.handleRowClick(this.Receive)
          this.$message({
            title: '成功',
            message: resData.Message,
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
    // 生成领用单
    handleCreate() {
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 多选
    changeFun(val) {
      this.multipleSelection = val
    },
    // 手动勾选时触发
    getRowKeys(row) {
      return row.Id
    },
    // 添加时
    toggleSelection() {
      // 更换数据时先清空之前保留的数据
      this.addMaterial = []
      // 获取的数据保存在list中
      this.multipleSelection.forEach(element => {
        var Receives = element
        Receives.ReceiveTypeDescription = element.MaterialTypeDescription
        this.addMaterial.push(Receives)
      })

      this.addMould = false
    },
    UpdatetoggleSelection() {
      // 获取的数据保存在list中
      this.addMouldList = []
      this.addMouldList = this.multipleSelection
      this.addMouldList.forEach(element => {
        this.addMaterial.push(element)
      })
      // this.addMaterial = this.multipleSelection
      this.addMould = false
    },

    // 打印单据
    handlePrint(row) {
      this.PrintCode = row
      this.printDate = this.getNowFormatDate()
      getReceiveMaterialList(row.Code).then(response => {
        var resData = JSON.parse(response.data.Content)
        this.tempPrintlist = resData
        var btn = document.getElementById('printBtn')
        btn.click()
      })
    },

    // 下发
    chosePrint(row) {
      this.rowData = row
      this.Receive = Object.assign({}, row) // copy obj
      if (this.Receive.Status === 1) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '进行中',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Receive.Status === 2) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Receive.Status === 3) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '已作废',
          type: 'error',
          duration: 2000
        })
        return
      }
      issueReceiveTaskInfo(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          getReceiveMaterialList(this.Receive.Code).then(response => {
            var usersData = JSON.parse(response.data.Content)
            this.listMaterial = usersData
          })
          this.$message({
            title: '成功',
            message: '下发成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '下发失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 作废
    cancellation(row) {
      this.Receive = Object.assign({}, row) // copy obj
      if (this.Receive.Status === 2) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      postDoCancellatione(this.Receive).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          this.$message({
            title: '成功',
            message: '作废成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '作废失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
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
        // console.log(this.OptionList[i])
      }

      const data = {}
      getReceiveMaterialList(row.Code).then(response => {
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
            // console.log(this.OptionObject.Name)
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
      this.Receive = Object.assign({}, row) // copy obj
      if (this.Receive.Status === 1) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '进行中',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Receive.Status === 2) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.Receive.Status === 3) {
        this.$message({
          title: '失败',
          message: '领用单' + this.Receive.Code + '已作废',
          type: 'error',
          duration: 2000
        })
        return
      }
      getReceiveMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.addMouldList = usersData
        this.addMaterial = this.addMouldList
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
          this.Receive.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.Receive.AddMaterial.push(element)
          })
          const inData = Object.assign({}, this.Receive)
          editReceive(inData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              getReceiveMaterialList(this.Receive.Code).then(response => {
                var usersData = JSON.parse(response.data.Content)
                this.listMaterial = usersData
              })
              this.$message({
                title: '成功',
                message: '更新成功',
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: '失败',
                message: '更新失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 领用单添加页面的确定按钮
    createData(addMaterial) {
      if (this.addMaterial.length === 0) {
        this.$message({
          title: '提示',
          message: '请添加需要领用的模具信息',
          type: 'warning',
          duration: 2000
        })
        return
      }
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.Receive.Status = 0
          this.Receive.AddMaterial = []
          this.addMaterial.forEach(element => {
            this.Receive.AddMaterial.push(element)
          })
          createReceive(this.Receive).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success && this.i === this.len) {
              this.dialogFormVisible = false
              this.getList()
              this.resstReceive()
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
    // 删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该入库单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Receive = Object.assign({}, row) // copy obj
        this.deleteData(this.Receive)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
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
          getReceiveMaterialList('').then(response => {
            var usersData = JSON.parse(response.data.Content)
            this.listMaterial = usersData
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
      // 清除数据
      this.resstReceive()
    },
    // 选择一行后的明细表信息
    handleRowClick(row, column, event) {
      getReceiveMaterialList(row.Code).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.listMaterial = usersData
      })
    },
    // 获取模具信息
    getAddList() {
      this.listLoading = true
      this.AddlistQuery.WareHouseCode = this.Receive.WareHouseCode
      getPageRecords(this.AddlistQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        // console.log(usersData)
        this.addList = usersData.rows
        this.Addtotal = usersData.Addtotal
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },

    // 创建领用单行项目
    handleCreateOutMaterial() {
      // 核查可用库存需要选择仓库
      if (this.Receive.WareHouseCode === '') {
        this.$message({
          title: '提示',
          message: '请选择仓库',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (this.Receive.ReceiveType === '') {
        this.$message({
          title: '提示',
          message: '请选择领用类型',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (this.Receive.LastTimeReceiveName === '') {
        this.$message({
          title: '提示',
          message: '请选择领用人',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (this.Receive.LastTimeReceiveDatetime === '') {
        this.$message({
          title: '提示',
          message: '请选择领用时间',
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
      // this.$message({
      //   title: '成功',
      //   message: row.MaterialCode,
      //   type: 'success',
      //   duration: 2000
      // })
      // addMaterialGridCurrentRowIndex = row.
      this.addMaterialGridCurrentRowIndex = row
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
    // 模板下载
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Receive/DoDownLoadTemp'
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
    // 扫描添加数据，清空数据面板
    dialogInMaterialVisibleOpen() {
      this.dialogInMaterialVisible = true
      this.InMaterial.Quantity = 0
      this.resstIn()
      this.restLabel()
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
    // 清空添加面板
    resstReceive() {
      this.Receive = {
        Code: '', // 编码
        Quantity: '', // 数量
        Status: '', // 领用状态
        PredictReturnTime: '',
        ReceiveType: '', // 领用类型
        LastTimeReceiveName: '',
        LastTimeReceiveDatetime: '',
        ReceiveTime: '',
        WareHouseCode: '',
        Remarks: '', // 备注
        DesignCode: '', // 来源单号
        AddMaterial: []
      }
      this.addMaterial = []
      this.$refs.addMaterialGrid.clearSelection()
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    }
  }
}
</script>

