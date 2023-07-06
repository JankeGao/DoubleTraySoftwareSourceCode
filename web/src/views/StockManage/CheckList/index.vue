<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <div class="filter-container" style="margin-bottom:10px">
          <el-input
            v-model="listQuery.Code"
            placeholder="盘点单号"
            class="filter-item"
            clearable
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />
          <el-input
            v-model="listQuery.WarehouseCode"
            placeholder="仓库编码、仓库名称"
            class="filter-item"
            clearable
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />
          <el-button
            v-waves
            class="filter-button"
            type="primary"
            icon="el-icon-search"
            @click="handleFilter"
          >查询</el-button>
          <el-button
            class="filter-button"
            style="margin-left: 10px;"
            type="primary"
            icon="el-icon-edit"
            @click="handleCreate"
          >添加</el-button>
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
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status===0">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===1" type="primary">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===2" type="primary">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===3" type="success">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===4" type="warning">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===6" type="warning">
                <span>{{ scope.row.StatusDescription }}</span>
              </el-tag>
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
          <el-table-column :label="'盘点单号'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点类型'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CheckDictDescription }}</span>
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
          <el-table-column
            :label="'操作'"
            align="center"
            width="220"
            class-name="small-padding fixed-width"
            fixed="right"
          >
            <template slot-scope="scope">
              <el-button size="mini" type="danger" @click="handleCancel(scope.row)">作废</el-button>
              <el-button size="mini" type="info" @click="handleDetail(scope.row)">详情</el-button>
              <el-button size="mini" type="success" @click="handleSubmit(scope.row)">下发</el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            :current-page="listQuery.Page"
            :page-sizes="[10,20,30, 50]"
            :page-size="listQuery.Rows"
            :total="total"
            background
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </el-row>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      :fullscreen="true"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="CheckList"
        class="dialog-form"
        label-width="130px"
        label-position="left"
      >
        <el-form-item :label="'盘点类型'">
          <el-select
            v-model="CheckList.CheckDict"
            placeholder="请选择盘点类型"
            :multiple="false"
            reserve-keyword
            style="width:300px"
          >
            <el-option
              v-for="item in CheckTypeList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="'选择盘点货柜'" style="width:300px" prop="Tray">
          <el-cascader-multi
            ref="CheckAddr"
            v-model="CheckPreserve"
            :data="options2"
            :show-all-levels="false"
            clearable
            style="width:600px"
            @change="Change"
          />
        </el-form-item>
        <el-form-item :label="'备注'">
          <el-input
            v-model="CheckList.Remark"
            :autosize="{ minRows: 2, maxRows: 4}"
            type="textarea"
            placeholder="备注"
            class="dialog-input"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="handleChange">确认</el-button>
      </div>
    </el-dialog>

    <!-- 盘点单详情 -->
    <el-dialog
      v-el-drag-dialog
      :title="'盘点单详情'"
      :visible.sync="dialogDetial"
      :fullscreen="true"
      :close-on-click-modal="false"
    >
      <div class="filter-container" style="margin-bottom:20px">
        <el-input
          v-model="listDetailQuery.MaterialCode"
          placeholder="物料编码"
          style="width:150px"
          clearable
          class="filter-item"
          @keyup.enter.native="handleDetailFilter"
          @clear="handleDetailFilter"
        />
        <el-input
          v-model="listDetailQuery.MaterialLabel"
          placeholder="物料条码"
          style="width:150px"
          clearable
          class="filter-item"
          @keyup.enter.native="handleDetailFilter"
          @clear="handleDetailFilter"
        />
        <el-select
          v-model="listDetailQuery.ContainerCode"
          :multiple="false"
          reserve-keyword
          :clearable="true"
          placeholder="请选择货柜"
          @change="handleDetailFilter"
        >
          <el-option
            v-for="item in areaList"
            :key="item.Code"
            :label="item.Name"
            :value="item.Code"
          />
        </el-select>
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
        <el-button
          v-waves
          class="filter-button"
          type="primary"
          icon="el-icon-search"
          @click="handleDetailFilter"
        >查询</el-button>
      </div>
      <el-row>
        <el-table
          :key="TableKey"
          v-loading="false"
          :data="detailList"
          height="450px"
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
              <el-tag v-if="scope.row.Status===0">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===1" type="success">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===2" type="primary">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===3" type="success">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===4" type="warning">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===5" type="warning">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.Status===6" type="warning">
                <span>{{ scope.row.StatusDetailDescription }}</span>
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜码'" width="140" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'储位码'" width="140" align="center" show-overflow-tooltip>
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
          <el-table-column :label="'盘点数量'" width="100" align="center">
            <template slot-scope="{row}">
              <template v-if="row.edit">
                <el-input
                  v-model="row.CheckedQuantity"
                  style="width:80px"
                  class="edit-input"
                  size="mini"
                />
                <el-button
                  class="cancel-btn"
                  size="mini"
                  type="warning"
                  @click="cancelDetailEdit(row)"
                >取消</el-button>
              </template>
              <span v-else>{{ row.CheckedQuantity }}</span>
            </template>
          </el-table-column>

          <el-table-column :label="'盘点人'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Checker }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'盘点时间'" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CheckedTime }}</span>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            :current-page="listDetailQuery.Page"
            :page-sizes="[10,20,30, 50]"
            :page-size="listDetailQuery.Rows"
            :total="detailTotal"
            background
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleDetailSizeChange"
            @current-change="handleDetailCurrentChange"
          />
        </div>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="closeHandCheck">关闭</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { getPageRecords, cancelCheck, handCheck, getCheckDictTypeList, getLocationList, getWarehouseList, getContainerList, postDoCreate, getCheckListMaterialList, getCheckListContainerList } from '@/api/Mould/CheckList'
import { postDoCreateCheck } from '@/api/Inventory'
import { getUserInfos } from '@/api/SysManage/User'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { isFloat } from '@/utils/validate.js'
export default {
  name: 'CheckList', // 库存盘点
  directives: {
    elDragDialog,
    waves
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

      CheckPreserve: [],
      Containers: [],
      Trays: [],
      Locations: [],
      options2: [],
      valueList: [],
      props: {
        value: 'value',
        label: 'label',
        children: 'children',
        multiple: true,
        emitPath: false
      },
      addContainers: [],
      addCheckListDetail: [],
      // 盘点人
      UserInfoList: [],
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
        Sort: 'id',
        MaterialLabel: '',
        LocationCode: '',
        CheckCode: '',
        WareHouseCode: '',
        ContainerCode: ''
      },

      downloadLoading: false,
      TableKey: 0,
      list: null,
      detailList: null,
      dialogFormVisible: false,
      dialogFormArea: false,
      dialogStatus: '',
      textMap: {
        update: '编辑盘点单',
        create: '创建盘点单'
      },
      checkAreaList: [],
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
      // 盘点单实体
      CheckList: {
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
        AddCheckListDetails: [],
        AreaCodes: undefined
      },
      // 盘点单详情实体
      CheckListDetail: {
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
        ContainerCode: '',
        BatchCode: '',
        Checker: '',
        CheckedTime: undefined,
        Status: 4,
        ManufactureDate: '',
        TrayId: ''
      },
      // 输入规则
      rules: {

      },
      detailRules: {
        MaterialLabel: [{ required: true, message: '请输入盘点条码', trigger: 'blur' }],
        LocaitonCode: [{ required: true, message: '请输入盘点单号', trigger: 'blur' }],
        MaterialCode: [{ required: true, message: '请输入盘点单号', trigger: 'blur' }],
        Quantity: [{ required: true, validator: validateNumber, trigger: 'blur' }],
        CheckedQuantity: [{ required: true, validator: validateNumber, trigger: 'blur' }]
      },
      dictionaryList: [],
      // 表格显示实体
      showEntity: {
        wareHouse: '',
        Container: '',
        Tray: ''
      },
      showList: [],
      zhongjian: [],
      WareHouseList: [],
      ContainerList: [],
      dialogDetial: false,
      locationList: null,
      locationLoading: false,
      dialogFormDetail: false,
      materialList: [],
      materialLoading: false,
      areaList: [],
      CheckedAreaList: [],

      CheckTypeList: [
        {
          Code: 0, Name: '月度盘点'
        },
        {
          Code: 1, Name: '年度盘点'
        },
        {
          Code: 2, Name: '周期盘点'
        }
      ]
    }
  },
  watch: {
    // 面板关闭，清空数据
    dialogFormVisible(value) {
      if (!value) {
        this.resstCheckListDetail()
        this.resstCheckList()
        this.showList = []
        this.CheckPreserve = []
      }
    }
  },
  created() {
    this.getList()
    this.getCheckDictTypeList()
    this.GetWareHouseList()
    this.GetUserInfoList()
    this.GetContacitList()
  },
  methods: {
    Change(value) {
      console.log(this.CheckPreserve)
      // 每次进来清空保存选中的数据
      this.showList = []

      //  用来显示
      for (var i = 0; i < this.CheckPreserve.length; i++) {
        var tablelist = this.CheckPreserve[i]
        // console.log(tablelist.length)
        if (tablelist.length <= 2) {
          continue
        }
        this.showEntity.wareHouse = tablelist[0]
        this.showEntity.Container = tablelist[1]
        this.showEntity.Tray = tablelist[2]
        this.showList.push(this.showEntity)
        this.showEntity = {}
      }
      console.log(this.showList)
    },

    // 最后选择的数据
    handleChange(value) {
      if (this.CheckPreserve.length <= 0) {
        this.$message({
          title: '提示',
          message: '请选择盘点区域',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (this.CheckList.CheckDict === '') {
        this.$message({
          title: '提示',
          message: '请选择盘点类型',
          type: 'warning',
          duration: 2000
        })
        return
      }

      this.CheckList.AddCheckListDetails = []
      // 保存获取的托盘编码
      this.showList.forEach(element => {
        this.CheckListDetail = {
          TrayId: element.Tray
        }
        // this.addCheckListDetail.push(this.CheckListDetail)
        this.CheckList.AddCheckListDetails.push(this.CheckListDetail)
        // 添加进后清空盘点明细供后面使用
        // this.resstCheckListDetail()
      })

      // 添加盘点单
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          postDoCreate(this.CheckList).then((res) => {
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
    // 获取仓库信息
    GetWareHouseList() {
      getWarehouseList().then(response => {
        var data = JSON.parse(response.data.Content)
        this.WareHouseList = data
      })
    },
    // 获取仓库托盘库位信息
    GetContacitList() {
      getContainerList().then(response => {
        var data = JSON.parse(response.data)
        this.ContainerList = data
        // 循环仓库
        this.ContainerList.forEach(element => {
          var warehouse = {}
          warehouse.value = element.Code
          warehouse.label = '仓库：' + element.Name
          // 循环每个仓库下的货柜
          element.children.forEach(element2 => {
            var container = {}
            container.value = element2.Code
            container.label = '货柜：' + element2.Code
            // 循环每个货柜下的托盘
            element2.children.forEach(element3 => {
              var tray = {}
              tray.value = element3.Id
              tray.label = '托盘：' + element3.Code
              this.Trays.push(tray)
            })
            container.children = this.Trays
            // 清空保存托盘数据
            this.Trays = []
            // 添加
            this.Containers.push(container)
            warehouse.children = this.Containers
          })
          this.options2.push(warehouse)
          // 每循环一次清空保存的数据
          this.Containers = []
        })
      })
    },
    // 获取盘点类型
    getCheckDictTypeList() {
      getCheckDictTypeList('CheckType').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    // 获取盘点单信息
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 获取盘点单详情
    getDetailList() {
      this.listDetailLoading = true
      getCheckListMaterialList(this.listDetailQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.detailTotal = usersData.total
        this.detailList = usersData.rows
        console.log(this.detailTotal)
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
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.CheckPreserve = []
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleCancel(row) {
      if (row.Status === 6) {
        this.$message({
          title: '提示',
          message: '该盘点单' + row.Code + '，已作废。',
          type: 'warning',
          duration: 2000
        })
        return
      }
      if (row.Status === 5) {
        this.$message({
          title: '提示',
          message: '该盘点单' + row.Code + '，已提交。',
          type: 'warning',
          duration: 2000
        })
        return
      }
      this.$confirm('此操作将永久作废该盘点单, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.CheckList = Object.assign({}, row) // copy obj
        this.canCelData(this.CheckList)
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
    // 详情
    handleDetail(row) {
      this.dialogDetial = true
      this.listDetailQuery.CheckCode = row.Code
      this.listDetailQuery.WareHouseCode = row.WareHouseCode
      // 获取仓库下的货柜
      this.getDetailList()
      getCheckListContainerList(row.WareHouseCode).then(res => {
        var list = JSON.parse(res.data.Content)
        this.areaList = list
      })
      // 获取盘点单详情
      this.getDetailList()
    },
    handleSubmit(row) {
      if (row.Status !== 0) {
        this.$message({
          title: '提示',
          message: '该盘点单' + row.Code + '，不是初始状态。',
          type: 'warning',
          duration: 2000
        })
        return
      }
      postDoCreateCheck(row).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.getList()
          this.$message({
            title: '成功',
            message: '下发盘点单任务成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '下发盘点单任务失败:' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },

    // 选择时提示获取的托盘编码
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

    // 查询盘点单详情
    locationSelectChange(value) {
      this.listDetailQuery.LocationCode = value
      this.getDetailList()
    },
    // 清空盘点单详情信息
    resstCheckListDetail() {
      this.CheckListDetail = {
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
      }
    },
    // 清空盘点单信息
    resstCheckList() {
      this.CheckList = {
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
        AddCheckListDetails: [],
        AreaCodes: undefined
      }
      // 添加面板关闭后清空数据
      this.$refs.CheckAddr.$refs.panel.clearCheckedNodes()
      this.CheckPreserve = []
    }
  }
}
</script>
<style scoped>
/* .el-form /deep/ .el-form-item__label
{ width: 150px !important;  } */
</style>

