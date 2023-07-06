<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <div class="filter-container" style="margin-bottom:10px">
          <el-input v-model="listQuery.Code" placeholder="盘点单号" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-input v-model="listQuery.WarehouseCode" placeholder="仓库编码、仓库名称" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-input v-model="listQuery.ContainerCode" placeholder="货柜编码" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <!-- <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button> -->
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
          :height="tableHeight"
        >
          <el-table-column type="index" />
          <el-table-column :label="'状态'" width="100" align="center">
            <!-- <template slot-scope="scope">
              <span>{{ scope.row.StatusCaption }}</span>
            </template> -->
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0" type="primary"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===2" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===3" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===4" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===6" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===7" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库编码'" width="90" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库名称'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.WareHouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜编码'" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点单号'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点类型'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CheckTypeDescription }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'创建人'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedUserName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'开始时间'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.StartTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'结束时间'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.EndTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'备注'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'操作'" align="center" width="220" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <!-- <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">编辑</el-button> -->
              <!-- <el-button size="mini" type="danger" @click="handleCancel(scope.row)">作废</el-button> -->
              <el-button size="mini" type="info" @click="handleDetail(scope.row)">详情</el-button>
              <!-- <el-button size="mini" type="primary" @click="handleSend(scope.row)">指引</el-button> -->
              <!-- <el-button size="mini" type="warning" @click="handleAgain(scope.row)">重盘</el-button> -->
              <el-button size="mini" type="success" @click="handleSubmit(scope.row)">提交</el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
        </div>
      </el-card>
    </el-row>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Check" class="dialog-form" label-width="100px" label-position="left">
        <!-- <el-form-item :label="'盘点编码'" prop="Code">
          <el-input v-model="Check.Code" class="dialog-input" :disabled="dialogStatus=='update'" placeholder="请输入盘点单编码" />
        </el-form-item> -->
        <el-form-item :label="'仓库编码'" prop="WareHouseCode">
          <el-select
            v-model="Check.WareHouseCode"
            :multiple="false"
            reserve-keyword
            style="width:300px"
            :disabled="dialogStatus=='update'"
            @change="wareHouseCodeChange"
          >
            <el-option
              v-for="item in WareHouseList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="'区域编码'">
          <v-select
            key="Code"
            v-model="Check.AreaCodes"
            :multiple="true"
            reserve-keyword
            style="width:300px"
            :disabled="dialogStatus=='update'"
            :clearable="true"
            :options="areaList"
            label="Name"
            value="Code"
            @change="areaChange"
          />
        </el-form-item>
        <el-form-item :label="'盘点类型'">
          <el-select
            v-model="Check.CheckDict"
            :multiple="false"
            reserve-keyword
            style="width:300px"
          >
            <el-option
              v-for="item in dictionaryList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="'备注'">
          <el-input v-model="Check.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" placeholder="备注" class="dialog-input" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 盘点单详情 -->
    <el-dialog v-el-drag-dialog :title="'盘点单详情'" :visible.sync="dialogDetial" :fullscreen="true" :close-on-click-modal="false">
      <div class="filter-container" style="margin-bottom:20px">
        <el-input v-model="listDetailQuery.MaterialCode" placeholder="物料编码" style="width:200px" clearable class="filter-item" @keyup.enter.native="handleDetailFilter" @clear="handleDetailFilter" />
        <el-input v-model="listDetailQuery.MaterialLabel" placeholder="物料条码" style="width:200px" clearable class="filter-item" @keyup.enter.native="handleDetailFilter" @clear="handleDetailFilter" />
        <el-select
          v-model="listDetailQuery.LocationCode"
          :multiple="false"
          filterable
          remote
          reserve-keyword
          placeholder="请输入库位编码"
          :remote-method="remoteMethod"
          :loading="locationLoading"
          clearable
          @clear="handleDetailFilter"
          @change="locationSelectChange"
        >
          <el-option
            v-for="item in locationList"
            :key="item.Code"
            :label="item.Code"
            :value="item.Code"
          />
        </el-select>
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleDetailFilter">查询</el-button>
        <el-button v-if="(Check.Status!==6&&Check.Status!=7)" class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreateDetail">添加</el-button>
        <el-button class="filter-button" type="primary" icon="el-icon-search" @click="handleConfirmFinish">盘点完成</el-button>
      </div>
      <el-row>
        <el-table
          :key="TableKey"
          v-loading="false"
          :data="detailList"
          height="488px"
          :header-cell-style="{background:'#F5F7FA'}"
          border
          fit
          size="mini"
          highlight-current-row
          style="width:100%;min-height:100%;"
        >
          <el-table-column type="index" width="50" />
          <el-table-column :label="'状态'" width="100" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0" type="primary"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===2" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===3" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===4" type="primary"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===6" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===7" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'托盘号'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.TrayCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'库位码'" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料条码'" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料编码'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'物料名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'数量'" width="80" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点数量'" width="180" align="center">
            <template slot-scope="{row}">
              <template v-if="row.edit">
                <el-input v-model="row.CheckedQuantity" style="width:80px" class="edit-input" size="mini" />
                <el-button
                  class="cancel-btn"
                  size="mini"
                  type="warning"
                  @click="cancelDetailEdit(row)"
                >
                  取消
                </el-button>
              </template>
              <span v-else>{{ row.CheckedQuantity }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点人'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Checker }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点时间'" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CheckedTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'操作'" align="center" width="120" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="{row}">
              <el-button
                v-if="row.edit && (row.Status==0||row.Status==5)"
                type="success"
                size="small"
                icon="el-icon-circle-check-outline"
                @click="confirmDetailEdit(row)"
              >
                确认
              </el-button>
              <el-button
                v-if="(row.Status==0||row.Status==5) && row.edit==false"
                type="primary"
                size="small"
                @click="HandDetailEdit(row)"
              >
                手动盘点
              </el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listDetailQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listDetailQuery.Rows" :total="detailTotal" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleDetailSizeChange" @current-change="handleDetailCurrentChange" />
        </div>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="closeHandCheck">关闭</el-button>
      </div>
    </el-dialog>
    <el-dialog v-el-drag-dialog :title="'新增盘点条码'" :visible.sync="dialogFormDetail" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="detailDataForm" :rules="detailRules" :model="CheckDetail" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'物料名称'" prop="MaterialCode">
          <el-select
            v-model="CheckDetail.MaterialCode"
            :multiple="false"

            filterable
            remote
            reserve-keyword
            placeholder="请输入关键词(物料编码或名称)"
            :remote-method="remoteMaterialMethod"
            :loading="materialLoading"
            style="width:300px"
          >
            <el-option
              v-for="item in materialList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="'物料编码'">
          <el-input v-model="CheckDetail.MaterialCode" disabled class="dialog-input" />
        </el-form-item>
        <el-form-item :label="'物料条码'" prop="MaterialLabel">
          <el-input v-model="CheckDetail.MaterialLabel" clearable class="dialog-input" />
        </el-form-item>
        <el-form-item :label="'批次'">
          <el-input v-model="CheckDetail.BatchCode" clearable class="dialog-input" />
        </el-form-item>
        <el-form-item :label="'数量'" prop="Quantity">
          <el-input v-model="CheckDetail.Quantity" clearable class="dialog-input" />
        </el-form-item>
        <el-form-item :label="'盘点数量'" prop="CheckedQuantity">
          <el-input v-model="CheckDetail.CheckedQuantity" clearable class="dialog-input" />
        </el-form-item>
        <el-form-item :label="'库位码'" prop="LocationCode">
          <el-select
            v-model="CheckDetail.LocationCode"
            :multiple="false"
            filterable
            remote
            reserve-keyword
            placeholder="请输入库位编码"
            :remote-method="remoteMethod"
            :loading="locationLoading"
            clearable
            style="width:300px"
          >
            <el-option
              v-for="item in locationList"
              :key="item.Code"
              :label="item.Code"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="'生产日期'">
          <template>
            <!-- <el-input v-if="scope.row.Status==0" v-model="scope.row.ManufactrueDate" /> -->
            <el-date-picker
              v-model="CheckDetail.ManufactrueDate"
              type="date"
              placeholder="选择日期"
              style="width:300px"
            />
          </template>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormDetail = false">取消</el-button>
        <el-button type="primary" @click="createDetailData">确认</el-button>
      </div>
    </el-dialog>

    <el-dialog v-el-drag-dialog :title="'区域详情'" :visible.sync="dialogFormArea" :width="'60%'" :close-on-click-modal="false">
      <!-- 筛选栏 -->
      <el-card class="search-card">
        <div class="filter-container">
          <el-button size="mini" type="primary" @click="handleAreaStart()">启动</el-button>
          <el-button size="mini" type="primary" @click="handleAreaFinish()">熄灭</el-button>
        </div>
      </el-card>
      <el-card>
        <el-table
          :key="TableKey"
          v-loading="false"
          :data="checkAreaList"
          :header-cell-style="{background:'#F5F7FA'}"
          border
          size="mini"
          fit
          highlight-current-row
          style="width:100%;min-height:100%;"

          @selection-change="CheckAreaStart"
        >
          <el-table-column type="selection" width="30" :selectable="CheckAreaEnable" />
          <el-table-column type="index" width="50" />
          <el-table-column :label="'状态'" width="110" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0" type="primary"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===1" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===2" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===3" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===4" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===6" type="success"><span>{{ scope.row.StatusCaption }}</span></el-tag>
              <el-tag v-if="scope.row.Status===7" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'仓库编码'" width="100" align="center">
            <template slot-scope="scope">
              <el-tooltip class="item" effect="dark" :content="scope.row.MaterialName " placement="top">
                <span>{{ scope.row.WareHouseCode }}</span>
              </el-tooltip>
            </template>
          </el-table-column>
          <el-table-column :label="'区域编码'" width="100" align="center">
            <template slot-scope="scope">
              <el-tooltip class="item" effect="dark" :content="scope.row.AreaCode " placement="top">
                <span>{{ scope.row.AreaCode }}</span>
              </el-tooltip>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点单号'" width="180" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.CheckCode }}</span>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormArea = false">取消</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { PostPDACheckComplete, GetCheckAreaList, CheckTaskDoFinish, CheckTaskDoStart, getPageRecords, getCheckDetailPageRecords, createCheck, editCheck, cancelCheck, submitCheck, handCheck, getCheckDictTypeList, getLocationList, getWarehouseList, checkAgain, getMaterialList, createCheckDetail } from '@/api/Inventory'
import { GetWareHouseAreaList } from '@/api/Inventory'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { isFloat } from '@/utils/validate.js'

export default {
  name: 'Check', // 库存盘点
  directives: {
    elDragDialog,
    waves
    // elPopover
  },
  data() {
    var validateNumber = (rule, value, callback) => {
      if (!isFloat(value)) {
        callback(new Error('请输入正确格式的数字（包含两位小数的数字或者不包含小数的数字）'))
      } else {
        callback()
      }
    }
    return {
      tableHeight: window.innerHeight - 350,

      // 分页显示总查询数据
      total: null,
      detailTotal: null,
      listLoading: false,
      listDetailLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 10,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      listDetailQuery: {
        Page: 1,
        Rows: 10,
        MaterialCode: '',
        Status: undefined,
        Sort: 'id',
        MaterialLabel: '',
        LocationCode: '',
        CheckCode: '',
        WareHouseCode: '',
        TrayId: '',
        AreaCode: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      detailList: null,
      dialogFormVisible: false,
      dialogFormArea: false,
      dialogStatus: '',
      textMap: {
        update: '编辑盘点任务',
        create: '创建盘点任务'
      },
      checkAreaList: [],
      // 物料实体
      Check: {
        Id: undefined,
        Code: '',
        CheckDict: '',
        WareHouseCode: '',
        IsDeleted: false,
        StartTime: 0,
        EndTime: '',
        Status: '',
        Remark: '',
        CreatedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: '',
        ContainerCode: '',
        AreaCodes: undefined
      },
      CheckDetail: {
        Id: undefined,
        CheckCode: '',
        IsDeleted: false,
        Quantity: 0,
        CheckedQuantity: 0,
        MaterialCode: '',
        WareHouseCode: '',
        AreaCode: '',
        LocationCode: '',
        MaterialLabel: '',
        BatchCode: '',
        Checker: '',
        CheckedTime: undefined,
        Status: 4,
        ManufactureDate: ''

      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入盘点单号', trigger: 'blur' }],
        WareHouseCode: [{ required: true, message: '请选择仓库', trigger: 'blur' }]
      },
      detailRules: {
        MaterialLabel: [{ required: true, message: '请输入盘点条码', trigger: 'blur' }],
        LocaitonCode: [{ required: true, message: '请输入盘点单号', trigger: 'blur' }],
        MaterialCode: [{ required: true, message: '请输入盘点单号', trigger: 'blur' }],
        Quantity: [{ required: true, validator: validateNumber, trigger: 'blur' }],
        CheckedQuantity: [{ required: true, validator: validateNumber, trigger: 'blur' }]
      },
      dictionaryList: [],
      WareHouseList: [],
      dialogDetial: false,
      locationList: null,
      locationLoading: false,
      dialogFormDetail: false,
      materialList: [],
      materialLoading: false,
      areaList: [],
      CheckedAreaList: []
    }
  },
  watch: {

  },
  created() {
    this.getList()
    this.getCheckDictTypeList()
    this.GetWareHouseList()
    this.timer()
  },
  destroyed() {
    if (this.timer) {
      clearInterval(this.timer)
    }
  },
  methods: {
    handleConfirmFinish() {
      PostPDACheckComplete(this.Check).then(response => {
        var resData = JSON.parse(response.data.Content)
        if (resData.Success) {
          this.getDetailList()
          this.$message({
            title: '成功',
            message: '盘点成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '盘点失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        this.WareHouseList = data
      })
    },
    getCheckDictTypeList() {
      getCheckDictTypeList('CheckType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },

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
    // 详情页获取数据
    getDetailList() {
      this.listDetailLoading = true
      getCheckDetailPageRecords(this.listDetailQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.detailTotal = usersData.total
        this.detailList = usersData.rows.map(v => {
          this.$set(v, 'edit', false)
          this.$set(v, 'OriginalCheckedQuantity', 0)
          v.OriginalCheckedQuantity = v.CheckedQuantity
          return v
        })
        // Just to simulate the time of the request
        setTimeout(() => {
          this.listDetailLoading = false
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
    handleDetailFilter() {
      this.listDetailQuery.Page = 1
      this.getDetailList()
    },
    // 切换分页数据-行数据
    handleDetailSizeChange(val) {
      this.listDetailQuery.Rows = val
      this.getDetailList()
    },
    // 切换分页-列数据
    handleDetailCurrentChange(val) {
      this.listDetailQuery.Page = val
      this.getDetailList()
    },
    handleCreate() {
      this.resetCheck()
      this.areaList = []
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.$message({
            title: '成功',
            message: this.Check.AreaCodes,
            type: 'success',
            duration: 2000
          })
          createCheck(this.Check).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
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

    // 物料编辑
    handleUpdate(row) {
      this.Check = Object.assign({}, row) // copy obj
      if (this.Check.Status !== 0) {
        this.$message({
          title: '失败',
          message: '盘点单' + this.Check.Code + '执行中或已完成',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const checkData = Object.assign({}, this.Check)
          editCheck(checkData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Check.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Check)
                  break
                }
              }
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '更新成功',
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: '成功',
                message: '创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    handleCancel(row) {
      this.$confirm('此操作将永久作废该盘点单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Check = Object.assign({}, row) // copy obj
        this.canCelData(this.Check)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消作废'
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    canCelData(data) {
      if (data.Status === 7) {
        this.$message({
          title: '提示',
          message: '该盘点单' + data.Code + '，已作废。',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (data.Status === 6) {
        this.$message({
          title: '提示',
          message: '该盘点单' + data.Code + '，已提交。',
          type: 'warning',
          duration: 2000
        })
        return
      }
      cancelCheck(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '作废成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '作废失败' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    resetCheck() {
      this.Check = {
        Id: undefined,
        Code: '',
        CheckDict: '',
        WareHosueCode: '',
        IsDeleted: false,
        StartTime: 0,
        EndTime: '',
        Status: '',
        Remark: '',
        CreatedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: '',
        AreaCodes: undefined
      }
    },
    cancelDetailEdit(row) {
      row.CheckedQuantity = row.OriginalCheckedQuantity
      row.edit = false
    },
    confirmDetailEdit(row) {
      row.edit = false
      var value = parseFloat(row.CheckedQuantity)
      if (isNaN(value) || value === 'Nan') {
        this.$message({
          message: '盘点数量必须为数值',
          type: 'error'
        })
        row.CheckedQuantity = row.OriginalCheckedQuantity
        row.edit = true
        return
      }

      handCheck(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getDetailList()
          this.getList()
          this.$message({
            title: '成功',
            message: '盘点成功',
            type: 'success',
            duration: 2000
          })
        } else {
          row.CheckdQuantity = row.OriginalPickedQuantity
          row.edit = true
          this.$message({
            title: '失败',
            message: '盘点失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    HandDetailEdit(row) {
      if (row.Status === 0 || row.Status === 5) {
        row.edit = !row.edit
      }
    },
    closeHandCheck() {
      this.dialogDetial = false
      this.getList()
    },
    handleDetail(row) {
      this.Check = Object.assign({}, row) // copy obj
      this.dialogDetial = true
      this.listDetailQuery.CheckCode = row.Code
      this.listDetailQuery.WareHouseCode = row.WareHouseCode
      this.getDetailList()
    },
    handleSubmit(row) {
      submitCheck(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          this.$message({
            title: '成功',
            message: '提交盘点单成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '提交盘点单失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    remoteMethod(query) {
      if (query !== '') {
        this.locationLoading = true
        getLocationList(query, this.listDetailQuery.WareHouseCode).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.locationList = data
        })
        setTimeout(() => {
          this.locationLoading = false
        }, 200)
      } else {
        this.locationList = []
      }
    },
    locationSelectChange(value) {
      this.listDetailQuery.LocationCode = value
      this.getDetailList()
    },

    handleCreateDetail() {
      this.restCheckDetail()
      this.dialogFormDetail = true
      this.$nextTick(() => {
        this.$refs['detailDataForm'].clearValidate()
      })
    },
    createDetailData() {
      if (this.CheckDetail.Quantity === 0 || this.CheckDetail.Quantity === '0' || this.CheckDetail.Quantity === '0.0' || this.CheckDetail.Quantity === '0.00' || this.CheckDetail.Quantity === '00' || this.CheckDetail.Quantity === '000' || this.CheckDetail.Quantity === '0000' || this.CheckDetail.Quantity === '00000') {
        this.$message({
          title: '成功',
          message: '请输入数量',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (this.CheckDetail.CheckedQuantity === 0 || this.CheckDetail.CheckedQuantity === '0' || this.CheckDetail.CheckedQuantity === '0.0' || this.CheckDetail.CheckedQuantity === '0.00' || this.CheckDetail.CheckedQuantity === '00' || this.CheckDetail.CheckedQuantity === '000' || this.CheckDetail.CheckedQuantity === '0000' || this.CheckDetail.CheckedQuantity === '00000') {
        this.$message({
          title: '成功',
          message: '请输入盘点数量',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (!isFloat(this.CheckDetail.Quantity)) {
        this.$message({
          title: '成功',
          message: '请输入正确格式的数量（包含两位小数的数字或者不包含小数的数字）',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (!isFloat(this.CheckDetail.CheckedQuantity)) {
        this.$message({
          title: '成功',
          message: '请输入正确格式的盘点数量（包含两位小数的数字或者不包含小数的数字）',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.$refs['detailDataForm'].validate((valid) => {
        if (valid) {
          createCheckDetail(this.CheckDetail).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
              this.dialogFormDetail = false
              this.getDetailList()
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

    // 重盘  盘点完成后
    handleAgain(row) {
      if (row.Status !== 4) {
        this.$message({
          title: '失败',
          message: '盘点完成的盘点单才允许重盘',
          type: 'error',
          duration: 2000
        })
        return
      }
      checkAgain(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          this.$message({
            title: '成功',
            message: '重盘成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '重盘失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    restCheckDetail() {
      this.CheckDetail = {
        Id: undefined,
        CheckCode: this.Check.Code,
        IsDeleted: false,
        Quantity: 0,
        CheckedQuantity: 0,
        MaterialCode: '',
        WareHouseCode: this.Check.WareHouseCode,
        AreaCode: '',
        LocationCode: '',
        MaterialLabel: '',
        BatchCode: '',
        Checker: '',
        CheckedTime: undefined,
        Status: 4,
        ManufactureDate: ''
      }
    },
    remoteMaterialMethod(query) {
      if (query !== '') {
        this.materialLoading = true
        getMaterialList(query).then((response) => {
          var data = JSON.parse(response.data.Content)
          this.materialList = data
        })
        setTimeout(() => {
          this.materialLoading = false
        }, 200)
      } else {
        this.materialList = []
      }
    },
    getCheckAreaList(val) {
      GetCheckAreaList(val).then(res => {
        var data = JSON.parse(res.data.Content)
        this.checkAreaList = data
      })
    },
    handleSend(row) {
      this.getCheckAreaList(row.Code)
      this.dialogFormArea = true
    },
    wareHouseCodeChange(value) {
      GetWareHouseAreaList(value).then(res => {
        var data = JSON.parse(res.data.Content)
        this.areaList = data
      })
    },
    areaChange(value) {
      this.$set(this.Check, 'AreaCodes', value)
      this.$message({
        title: '失败',
        message: value,
        type: 'error',
        duration: 2000
      })
    },
    timer() {

    },
    CheckAreaStart(val) {
      this.CheckedAreaList = val
    },
    CheckAreaEnable(row, index) {
      if (row.Status !== 6 && row.Status !== 7) {
        return true
      }
      return false
    },
    handleAreaStart() {
      if (this.CheckedAreaList.length > 0) {
        this.$confirm('确定启动勾选的区域?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          CheckTaskDoStart(this.CheckedAreaList).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '区域启动成功',
                type: 'success',
                duration: 2000
              })
              var element = this.CheckedAreaList[0]
              this.getCheckAreaList(element.CheckCode)
            } else {
              this.$message({
                title: '失败',
                message: '区域启动失败' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选区域',
          type: 'error',
          duration: 2000
        })
      }
    },
    handleAreaFinish() {
      if (this.CheckedAreaList.length > 0) {
        this.$confirm('确定完成勾选的区域?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          CheckTaskDoFinish(this.CheckedAreaList).then(res => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '区域完成成功',
                type: 'success',
                duration: 2000
              })
              var element = this.CheckedAreaList[0]
              this.getCheckAreaList(element.CheckCode)
            } else {
              this.$message({
                title: '失败',
                message: '区域完成失败' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        })
      } else {
        this.$message({
          title: '失败',
          message: '尚未勾选区域',
          type: 'error',
          duration: 2000
        })
      }
    }
  }
}
</script>

