<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <!-- 主表单 -->
    <el-card class="search-card">
      <div class="filter-container" style="margin-bottom:10px">
        <el-input v-model="Query.Code" placeholder="库位移动单号" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
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
        @row-click="handleRowClick"
      >
        <el-table-column type="index" width="50" align="center" />
        <el-table-column :label="'状态'" width="120" align="center">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===0" type="warning"><span>{{ scope.row.StatusCaption }}</span></el-tag>
            <el-tag v-if="scope.row.Status===1" type="danger"><span>{{ scope.row.StatusCaption }}</span></el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'库位移动单号'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
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
        <el-table-column :label="'目标库位'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.NewLocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'创建人'" width="150" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column sortable="custom" :label="'添加时间'" prop="CreatedTime" width="200" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'备注'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
      </el-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="Query.Page" :page-sizes="[15,30,45,60]" :page-size="Query.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>
    <!-- 行项目表单 -->
    <el-card class="search-card">
      <el-table
        :key="TableKey"
        v-loading="false"
        :data="listMaterial"
        :header-cell-style="{background:'#f5f7fa'}"
        :height="300"
        size="mini"
        border
        fit
        highlight-current-row
        style="width:100%;min-height:100%"
      >
        <el-table-column type="index" width="50" align="center" />
        <el-table-column :label="'物料条码'" width="200" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLable }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="200" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料名称'" width="200" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'原始库位'" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.OldLocationCode }}</span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    <!-- 创建 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'85%'" style="padding-bottom: 50px" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="MobileLocation" class="dialog-form" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item :label="'目标库位'">
              <el-input v-model="MobileLocation.NewLocationCode" style="width:100%" placeholder="目标库位" class="dialog-input">
                <el-select
                  slot="prepend"
                  v-model="MobileLocation.WareHouseCode"
                  :multiple="false"
                  filterable
                  style="width:120px;"
                  placeholder="请选择仓库"
                  @change="changeWarehouse"
                >
                  <el-option
                    v-for="item in WareHouseList"
                    :key="item.Code"
                    :label="item.Name"
                    :value="item.Code"
                  />
                </el-select>
                <el-button slot="append" icon="el-icon-search" @click="choseLocation">选择库位</el-button>
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="20">
            <el-form-item :label="'具体描述'">
              <el-input v-model="MobileLocation.Remark" style="width:100%;" :autosize="{ minRows: 1, maxRows: 1}" type="textarea" placeholder="出库备注" class="dialog-input" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
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
        height="350"
        @current-change="chosedColumn"
      >
        <el-table-column type="index" width="50" align="center" />
        <!-- <el-table-column type="selection" width="50" align="center" :selectable="CheckEnable" /> -->
        <el-table-column :label="'物料条码'" width="250" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialLabel }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'原始库位'" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.LocationCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料编码'" width="250" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'物料名称'" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.MaterialName }}</span>
          </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <!-- <el-button v-else type="primary" @click="updateData">确认</el-button> -->
      </div>
    </el-dialog>
    <!-- 选择库位 弹出框 -->
    <el-dialog v-el-drag-dialog title="选择库位" :visible.sync="dialogFormChose" :width="'85%'" style="padding-bottom: 50px" :close-on-click-modal="false">
      <template>
        <el-row :gutter="20">
          <el-col :span="5">
            <!-- 区域巷道信息 -->
            <el-card>
              <div>
                <span>
                  <!-- :title="textMap[dialogStatus]" -->
                  <div style="display: inline-block;"><h4>库位信息</h4> </div>
                </span>
              </div>
              <hr class="line">
              <div>
                <el-tree
                  ref="treeTest"
                  :data="treeData"
                  :expand-on-click-node="false"
                  style="font-size:20px;"
                  node-key="Id"
                  :default-expand-all="false"
                  highlight-current
                  @node-click="handleNodeClick"
                />
              </div>
            </el-card>
          </el-col>
          <!-- 库位信息 -->
          <el-card>
            <div>
              <el-table
                :key="0"
                v-loading="listLoading"
                :data="locationList"
                :header-cell-style="{background:'#F5F7FA'}"
                border
                fit
                size="mini"
                highlight-current-row
                style="width:100%;min-height:100%;"
                @current-change="chosedLocation"
              >
                <el-table-column v-if="Level==2" type="index" width="50" align="center" />
                <!-- <el-table-column v-if="Level==2" type="selection" width="50" align="center" :selectable="CheckEnable" /> -->
                <!-- 库位列表 -->
                <el-table-column v-if="Level==2" :label="'区域编码'" width="80" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.AreaCode }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'货架编码'" width="80" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.ShelfCode }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'库位编码'" width="120" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.Code }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'行号'" width="80" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.Row }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'列号'" width="80" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.Column }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'物料编码'" width="150" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.SuggestMaterialCode }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'物料名称'" align="center" show-overflow-tooltip>
                  <template slot-scope="scope">
                    <span>{{ scope.row.SuggestMaterialName }}</span>
                  </template>
                </el-table-column>
                <el-table-column v-if="Level==2" :label="'启用'" width="70" align="center">
                  <template slot-scope="scope">
                    <el-switch
                      v-model="scope.row.Enabled"
                      active-color="#13ce66"
                      inactive-color="#ff4949"
                      disabled
                    />
                  </template>
                </el-table-column>
              </el-table>
              <!-- 分页 -->
              <div class="pagination-container">
                <el-pagination v-if="Level==2" :current-page="listQuery.Page" :page-sizes="[15,30,45, 60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleLocationSizeChange" @current-change="handleLocationCurrentChange" />
              </div>
            </div>
          </el-card>
        </el-row>
        <div slot="footer" class="dialog-footer">
          <el-button @click="dialogFormChose = false">取消</el-button>
          <el-button type="primary" @click="handleChose">确认</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>
<script>
// import { ouLoadStockInfo, getLocationPageRecords, LightStock, OffLightStock, DeleteStockArray } from '@/api/stock'
import { GetMaterialList, createMobileLocation, getPageRecords, getLocationPageRecords, getWarehouseList, getMaterialLabelListByWareHouseCode, getAreaTreeData } from '@/api/MobileLocation'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'MobileLocation', // 库位移动
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
        Level: 0,
        Status: undefined,
        WareHouseCode: '',
        Sort: 'Id',
        ChannelId: '',
        Code: ''
      },
      Query: {
        Page: 1,
        Rows: 15,
        MaterialCode: '',
        WareHouseCode: '',
        Code: ''
      },
      Level: 0,
      treeData: [],
      chosedWareHouseCode: '',
      MobileLocation: {
        Code: '',
        Status: undefined,
        WareHouseCode: '',
        NewLocationCode: '',
        OldLocationCode: '',
        MaterialLable: '',
        MaterialCode: '',
        Remark: ''
      },
      addMaterial: [],
      WareHouseList: [],
      listMaterial: [],
      TableKey: 0,
      list: null,
      locationList: null,
      checkBoxData: [],
      dialogStatus: '',
      dialogFormVisible: false,
      dialogFormChose: false,
      textMap: {
        update: '编辑移库单',
        create: '创建移库单'
      },
      // 输入规则
      rules: {

      }
    }
  },
  watch: {

  },
  created() {
    this.getList()
    this.GetWareHouseList()
  },
  methods: {
    handleCreate() {
      this.dialogStatus = 'create'
      this.addMaterial = []
      this.chosedWareHouseCode = ''
      this.MobileLocation.WareHouseCode = ''
      this.MobileLocation.NewLocationCode = ''
      this.MobileLocation.OldLocationCode = ''
      this.MobileLocation.MaterialLable = ''
      this.MobileLocation.MaterialCode = ''
      this.dialogFormVisible = true
    },
    createData() {
      if (this.MobileLocation.WareHouseCode === '') {
        this.$message({
          title: '失败',
          message: '请选择仓库',
          type: 'error',
          druation: 2000
        })
        return
      }
      if (this.MobileLocation.NewLocationCode === '') {
        this.$message({
          title: '失败',
          message: '请选择或输入目标仓库',
          type: 'error',
          druation: 2000
        })
        return
      }
      if (this.MobileLocation.MaterialLable === '') {
        this.$message({
          title: '失败',
          message: '请选择要移动库位的物料条码',
          type: 'error',
          druation: 2000
        })
        return
      }
      console.log(this.MobileLocation)
      createMobileLocation(this.MobileLocation).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '库位移动成功',
            type: 'success',
            druation: 2000
          })
          this.getList()
          this.dialogFormVisible = false
        } else {
          this.$message({
            title: '失败',
            message: '库位移动失败' + resData.Message,
            type: 'error',
            druation: 2000
          })
        }
      })
    },
    getList() {
      this.listLoading = true
      console.log(this.Query)
      getPageRecords(this.Query).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    getLocationList() {
      this.listLoading = true
      getLocationPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.locationList = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 获取仓库列表（选择仓库按钮功能）
    GetWareHouseList() {
      getWarehouseList(this.chosedWareHouseCode).then(res => {
        var wareHouseData = JSON.parse(res.data.Content)
        this.WareHouseList = wareHouseData
      })
    },
    // 获取已选择仓库下的所有条码库存
    changeWarehouse(data) {
      this.chosedWareHouseCode = data // 将选择的仓库编码值保存下来（供选择库位使用）
      this.MobileLocation.WareHouseCode = data
      getMaterialLabelListByWareHouseCode(data).then(res => {
        var materialData = JSON.parse(res.data.Content)
        this.addMaterial = materialData
      })
    },
    handleNodeClick(data) {
      console.log(data)
      this.Level = data.Level
      this.listQuery.Level = this.Level + 1
      this.listQuery.Page = 1
      this.listQuery.WareHouseCode = data.WareHouseCode
      this.listQuery.Code = data.Id
      this.listQuery.ChannelId = data.Id
      this.getLocationList()
    },
    // 数据筛选
    handleFilter() {
      this.Query.Page = 1
      this.getList()
    },
    // 切换分页数据-行数据
    handleSizeChange(val) {
      this.Query.Rows = val
      this.getList()
    },
    // 切换分页-列数据
    handleCurrentChange(val) {
      this.Query.Rows = val
      this.getList()
    },
    handleLocationSizeChange(val) {
      this.listQuery.Rows = val
      this.getLocationList()
    },
    // 切换分页-列数据
    handleLocationCurrentChange(val) {
      this.listQuery.Page = val
      this.getLocationList()
    },
    // 获取已选择仓库下的区域巷道库位
    choseLocation() {
      if (this.chosedWareHouseCode === '') {
        this.$message({
          title: '失败',
          message: '请先选择仓库，然后再选择目标库位！',
          type: 'error',
          druation: 2000
        })
        return
      }
      this.MobileLocation.NewLocationCode = ''
      this.Level = 0
      this.locationList = null
      this.dialogFormChose = true
      getAreaTreeData(this.chosedWareHouseCode).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData(usersData)
      })
    },
    convertTreeData(data) {
      const treedataList = []
      data.forEach(item => {
        var areaList = []
        var parent = []
        areaList = item.AreaList
        var channelList = []
        channelList = item.ChannelList
        areaList.forEach(area => {
          var Child = []
          channelList.forEach(channel => {
            if (channel.AreaCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              Child.push(this.generateRouter(channel, false, 2))
            }
          })
          parent = this.generateRouter(area, false, 1)
          parent.children = Child
          treedataList.push(parent)
        })
      })
      return treedataList
    },
    generateRouter(item, isParent, level) {
      if (level === 1) {
        item.Name = item.Name + '(区域)'
      }
      if (level === 2) {
        item.Name = item.Code + '(巷道)'
      }
      var treeData = {
        label: item.Name,
        Code: item.Code,
        Name: item.Name,
        Level: level,
        WareHouseCode: item.WareHouseCode,
        AreaCode: item.AreaCode,
        Id: item.Id
      }
      return treeData
    },
    // 确定选择的库位
    handleChose() {
      if (this.MobileLocation.NewLocationCode === '') {
        this.$message({
          tilt: '失败',
          message: '请选择目标库位',
          type: 'error',
          druation: 2000
        })
        return
      }
      this.dialogFormChose = false
    },
    // 选择物料信息
    chosedColumn(val) {
      console.log(val)
      this.MobileLocation.OldLocationCode = val.LocationCode
      this.MobileLocation.MaterialCode = val.MaterialCode
      this.MobileLocation.MaterialLable = val.MaterialLabel
      console.log(this.MobileLocation)
    },
    // 选择库位信息
    chosedLocation(val) {
      this.MobileLocation.NewLocationCode = val.Code
    },
    handleRowClick(row, column, event) {
      console.log(event)
      GetMaterialList(row.Code).then(res => {
        var resData = JSON.parse(res.data.Content)
        this.listMaterial = resData
      })
    }
  }

}
</script>

