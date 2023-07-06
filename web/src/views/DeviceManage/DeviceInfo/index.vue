<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="4">
        <el-card>
          <div>
            <span>
              <div style="display: inline-block;">
                <h4>设备管理</h4>
              </div>
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
              default-expand-all
              highlight-current
              @node-click="handleNodeClick"
            />
          </div>
        </el-card>
      </el-col>
      <el-col :span="20">
        <el-card>
          <div v-if="Level==1">
            <el-row :gutter="20">
              <el-col
                v-for="item in videoList"
                :key="item.Code"
                :span="6"
                style="margin-bottom: 20px;"
              >
                <el-card
                  :body-style="{ padding: '0px'}"
                  style="background:#F2F6FC"
                  class="item-card"
                  shadow="hover"
                >
                  <div slot="header" class="clearfix">
                    <span class="title" type="index">{{ item.Sort }}</span>
                    <span class="title">设备信息</span>
                    <el-button
                      type="text"
                      icon="el-icon-edit"
                      style="margin-left:10px"
                      @click="handleEdit(item)"
                    />
                    <span v-if="item.Status===1" style="float:right;width:50px;margin:10px;">
                      <span style="font-size:14px;color:#909399">{{ item.StatusCaption }}</span>
                      <span class="right" />
                    </span>
                    <span v-else style="float:right;width:50px;margin:10px;">
                      <span style="font-size:14px;color:#909399">{{ item.StatusCaption }}</span>
                      <div class="error" />
                    </span>
                  </div>
                  <el-row :gutter="5" style="margin:10px">
                    <el-col :span="12">
                      <div style="margin:10px 0px">
                        <div class="title" style="margin:10px">ID</div>
                        <div class="label" style="margin:10px">{{ item.Code }}</div>
                      </div>
                      <div style="margin:10px 0px">
                        <div class="title" style="margin:10px">设备地址</div>
                        <div class="label" style="margin:10px">{{ item.Ip }}</div>
                      </div>
                      <div style="margin:10px 0px">
                        <div class="title" style="margin:10px">端口号</div>
                        <div class="label" style="margin:10px">{{ item.Port }}</div>
                      </div>
                      <div style="margin:10px 0px">
                        <div class="title" style="margin:10px">描述</div>
                        <div class="label" style="margin:10px">{{ item.Remark }}</div>
                      </div>
                    </el-col>
                    <el-col :span="12">
                      <el-card>
                        <img :src="BaseUrl+item.PictureUrl" class="img-xys">
                      </el-card>
                      <div style="margin:10px 0px">
                        <div class="title" style="margin:10px">品牌类别</div>
                        <div
                          class="label"
                          style="margin:10px"
                        >{{ item.BrandDescription }} - {{ item.EquipmentTypeDescription }}</div>
                      </div>
                    </el-col>
                  </el-row>
                </el-card>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <div class="pagination-container">
                <el-pagination
                  :current-page="listQuery.Page"
                  :page-sizes="[8,16,24, 32]"
                  :page-size="listQuery.Rows"
                  :total="total"
                  background
                  layout="total, sizes, prev, pager, next, jumper"
                  @size-change="handleSizeChange"
                  @current-change="handleCurrentChange"
                />
              </div>
            </el-row>
          </div>

          <div v-if="Level==2">
            <div class="filter-container" style="margin-bottom:10px">
              <el-input
                v-model="listQuery.LocationCode"
                placeholder="库位编码"
                class="filter-item"
                clearable
                @keyup.enter.native="handleFilter"
                @clear="handleFilter"
              />
              <el-input
                v-model="listQuery.MaterialCode"
                placeholder="物料编码、物料名称"
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
            </div>
            <el-table
              :key="TableKey"
              v-loading="listLoading"
              :data="videoList"
              :header-cell-style="{background:'#F5F7FA'}"
              border
              fit
              size="mini"
              highlight-current-row
              style="width:100%;min-height:100%;"
            >
              <el-table-column type="index" width="50" />
              <el-table-column :label="'托盘编码'" width="80" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.TrayCode }}</span>
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
              <el-table-column :label="'存储重量(kg)'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.Quantity* 2 }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'载具名称'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.BoxName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'物料名称'" width="200" show-overflow-tooltip align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialName }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'总数量'" width="80" align="center">
                <template slot-scope="scope">
                  <span style="color:green">{{ scope.row.Quantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'锁定数量'" width="80" align="center">
                <template slot-scope="scope">
                  <span style="color:red">{{ scope.row.LockedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'单位'" width="50" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialUnit }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'储位编码'" width="120" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.LocationCode }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'物料编码'" width="130" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialCode }}</span>
                </template>
              </el-table-column>
              <el-table-column :label="'物料条码'" width="140" align="center" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ scope.row.MaterialLabel }}</span>
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
            </el-table>
            <!-- 分页 -->
            <div class="pagination-container">
              <el-pagination
                :current-page="listQuery.Page"
                :page-sizes="[15,30,45,60]"
                :page-size="listQuery.Rows"
                :total="total"
                background
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="handleSizeChange"
                @current-change="handleCurrentChange"
              />
            </div>
          </div>

          <div v-if="Level==3">
            <div style="margin-bottom:20px">
              <span>
                <el-tag>
                  <span style="color:write">空</span>
                </el-tag>
              </span>
              <span>
                <el-tag type="success">少半</el-tag>
              </span>
              <span>
                <el-tag type="warning">多半</el-tag>
              </span>
              <span>
                <el-tag type="danger">已满</el-tag>
              </span>
            </div>
            <div>
              <TopoRender ref="render" time="1000" @clickBox="clickBoxLoaction" />
            </div>
            <div style="margin-top:20px;">
              <el-row :gutter="20">
                <el-col :span="8">
                  <el-card :body-style="{padding:'5px'}" align="middle" shadow="hover">
                    <div slot="header" class="clearfix">
                      <span>存放载具</span>
                    </div>
                    <!-- <el-image :src="src" fit="contain" style="width: 300px; height: 200px">
                      <div slot="error" class="image-slot">
                        <i class="el-icon-picture-outline" />
                      </div>
                    </el-image>-->
                    <img :src="src" fit="contain" style="width: 300px; height: 200px" />
                    <div style="padding:10px">
                      <div style="padding:10px">
                        <span>{{ Box.Name }}-{{ Box.Code }}</span>
                      </div>
                      <div style="padding:5px">
                        <span>长度(mm):</span>
                        <span>{{ Box.BoxLength }}</span>
                      </div>
                      <div style="padding:10px">
                        <span>宽度(mm):</span>
                        <span>{{ Box.BoxWidth }}</span>
                      </div>
                    </div>
                  </el-card>
                </el-col>
                <el-col
                  v-for="item in materialList"
                  :key="item.Code"
                  :span="8"
                  style="margin-bottom: 20px;"
                  >
                    <el-card :body-style="{padding:'5px'}" align="middle" shadow="hover">
                      <div slot="header" class="clearfix">
                        <span>当前存放物料</span>
                      </div>
                      <el-card :body-style="{padding:'5px'}" align="middle" shadow="hover">
                        <div id="printf" ref="print" align="middle">
                          <div style="padding:1px">
                            <el-image
                            :src="BaseUrl +item.PictureUrl"
                            fit="contain"
                            style="width: 300px; height: 200px"
                            >
                            <div slot="error" class="image-slot">
                              <i class="el-icon-picture-outline" />
                            </div>
                            </el-image>
                            <div style="padding:1px">
                              <span>{{ item.MaterialCode }}-{{ item.MaterialName }}</span>
                            </div>
                          </div>
                          <div style="padding:1px">
                            <span>当前存放数量:</span>
                            <span>{{ item.Quantity }}</span>
                          </div>
                      </div>
                      </el-card>
                        <div style="padding:10px">
                          <span>最多存放数量:</span>
                          <span>{{ item.BoxMaxCount }}</span>
                        </div>
                    </el-card>
                  <div align="center">
                    <el-button v-print="'#printf'" style="margin:0px;" @click="printMessage">打印</el-button>
                  </div>
                </el-col>
              </el-row>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="DeviceEntity"
        class="dialog-form"
        label-width="120px"
        label-position="right"
      >
        <el-form-item label="设备编码" prop="Code">
          <el-input
            v-model="DeviceEntity.Code"
            class="dialog-input"
            :disabled="dialogStatus=='update'"
          />
        </el-form-item>
        <el-form-item label="设备品牌">
          <el-input
            v-model="DeviceEntity.BrandDescription"
            class="dialog-input"
            :disabled="dialogStatus=='update'"
          />
        </el-form-item>
        <el-form-item label="设备类别">
          <el-input
            v-model="DeviceEntity.EquipmentTypeDescription"
            class="dialog-input"
            :disabled="dialogStatus=='update'"
          />
        </el-form-item>
        <el-form-item label="设备IP地址" prop="Address">
          <el-input v-model="DeviceEntity.Ip" class="dialog-input" placeholder="请输入设备运行驱动服务IP地址" />
        </el-form-item>
        <el-form-item label="设备端口" prop="Port">
          <el-input v-model="DeviceEntity.Port" class="dialog-input" placeholder="请输入设备运行驱动服务端口号" />
        </el-form-item>
        <el-form-item label=" 设备描述">
          <el-input
            v-model="DeviceEntity.Remark"
            :autosize="{ minRows: 2, maxRows: 4}"
            type="textarea"
            placeholder="请描述下设备具体功能"
            class="dialog-input"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="editData">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { createVideo, fetchList, editVideo, deleteVideo /* getWareHouseIDData*/ } from '@/api/DeviceInfo'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getWareHouseTreeData /* getTrayById */ } from '@/api/WareHouse'
import TopoRender from './components/topo/TopoRender'
import { /* getBoxMaterialMapByCode, getBoxPageRecords,*/ getBoxByCode } from '@/api/Box'
import { getLocationStockByLayoutId } from '@/api/stock'

export default {
  directives: {
    elDragDialog
  },
  components: {
    TopoRender
  },
  data() {
    return {
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址

      areaName: '',
      materialDetailList: null,
      // 获取当前Tree列表
      options: [{ value: null, label: '全部状态' }, { value: '0', label: '未启用' }, { value: '1', label: '运行中' }, { value: '2', label: '故障' }],
      treeData: [], // 用来保存仓库
      videoList: null,
      // 分页显示总查询数据
      total: null,
      listLoading: true,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        Name: '',
        Code: '',
        Sort: 'Id',
        Level: 1,
        WareHouseCode: '',
        SearchCode: '',
        LocationCode: '',
        MaterialCode: ''
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      currentAreaId: 0,
      textMap: {
        update: '编辑设备',
        create: '创建模块'
      },
      TableKey: 0,
      // 输入规则
      rules: {
        // Code: [{ required: true, message: '请输入设备编码', trigger: 'blur' }],
        // Name: [{ required: true, message: '请输入设备名称', trigger: 'blur' }],
        // Address: [{ required: true, message: '请输入设备地址', trigger: 'blur' }],
        // Port: [{ required: true, message: '请输入设备端口号', trigger: 'blur' }],
        // ServerAddress: [{ required: true, message: '请输入服务器地址', trigger: 'blur' }]
        // Channel: [{ required: true, message: '请输入设备通道', trigger: 'blur' }],
        // AreaId: [{ required: true, message: '请选择UWB属性', trigger: 'blur' }],
        // Description: [{ required: false, message: '', trigger: 'blur' }]
      },
      // 设备实体
      DeviceEntity: {
        Id: undefined,
        Name: '',
        Code: '',
        Address: '',
        Port: '',
        AreaId: undefined,
        ServerAddress: '',
        Channel: '',
        Description: '',
        AreaName: '',
        Remark: '',
        Ip: ''

      },
      // 编辑所用实体
      src: '/logo.png',
      Level: 1,
      materialList: [],
      Box: {}
    }
  },
  created() {
    this.getTreeData()
    this.handleFilter()
  },
  methods: {
    clickBoxLoaction(component) {
      getBoxByCode(component.dataBind.sn).then(response => {
        var result = JSON.parse(response.data.Content)
        this.Box = result
        // console.log(this.Box)
        this.src = this.BaseUrl + this.Box.PictureUrl
        getLocationStockByLayoutId(component.identifier).then(response => {
          var resData = JSON.parse(response.data.Content)
          this.materialList = resData
        })

        // 根据唯一码查询库存信息
        // 查找该载具所关联的物料
        // this.SuggestMaterial = JSON.parse(JSON.stringify(this.BoxMaterialMapList.find((element) => (element.Code === component.dataBind.biz))))
        // this.materialUrl = this.BaseUrl + this.SuggestMaterial.PictureUrl
      })
    },
    getTreeData() {
      getWareHouseTreeData(1).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData(usersData)
        console.log(this.treeData)
      })
    },
    // 获取设备信息
    getList() {
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.videoList = usersData.rows
        console.log(this.videoList)
        this.total = usersData.total
        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 数据筛选
    handleFilter() {
      this.listQuery.Page = 1
      // this.Level = 3
      this.listQuery.Level = this.Level // 查询库位码
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
    convertTreeData(data) {
      const treedataList = []
      var entity = {
        Code: 'WareHouse',
        Name: '仓库信息'
      }

      var first = this.generateRouter(entity, true, 0)
      var firstchildren = []
      data.forEach(item => {
        var parent = this.generateRouter(item, true, 1)
        var containerList = []
        var twoChild = []
        containerList = item.ContainerList
        var trayList = []
        trayList = item.TrayList
        containerList.forEach(area => {
          var threeChild = []
          trayList.forEach(channel => {
            if (channel.ContainerCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              threeChild.push(this.generateRouter(channel, false, 3))
            }
          })
          var twoParent = this.generateRouter(area, false, 2)
          twoParent.children = threeChild
          twoChild.push(twoParent)
        })

        parent.children = twoChild
        firstchildren.push(parent)
      })
      first.children = firstchildren
      treedataList.push(first)
      return treedataList
    },
    generateRouter(item, isParent, level) {
      if (level === 1) {
        item.Name = item.Name + '(仓库)'
      }
      if (level === 2) {
        item.Name = item.Code + '-' + item.Brand + '(货柜)'
      }
      if (level === 3) {
        item.Name = item.Code + '(托盘)'
      }
      var treeData = {
        label: item.Name,
        Code: item.Code,
        Name: item.Name,
        Level: level,
        WareHouseCode: item.WareHouseCode,
        ContainerCode: item.ContainerCode,
        Id: item.Id
      }
      return treeData
    },
    handleNodeClick(data) {
      this.Level = data.Level
      this.listQuery.Level = this.Level
      this.listQuery.Code = data.Code
      this.listQuery.Page = 1
      this.Box = []
      this.src = '/logo.png'
      if (data.Level === 0 || data.Level === 1) {
        // console.log(this.listQuery)
        this.getList()
      }
      if (data.Level === 2) {
        this.getList()
      }
      if (data.Level === 3) {
        const loading = this.$loading({
          lock: true,
          text: '加载库存中',
          spinner: 'el-icon-loading',
          background: 'rgba(255, 255, 255, 0.95)'
        })
        setTimeout(() => {
          this.$refs.render.onShowTray(data.Id) // 方法2:直接调用，添加延时，组件需要时间定义，否则报错误
        }, 100)
        setTimeout(() => {
          loading.close()
        }, 2000)
      }
    },
    handleStatusChange(val) {
      // this.$message({
      //   title: '失败',
      //   message: '创建失败：' + val,
      //   type: 'error',
      //   duration: 2000
      // })
      // this.listQuery.Status = data.value
      this.listQuery.Page = 1
      this.getList()
    },
    // 模块创建
    handleCreate() {
      var testData = this.$refs['treeTest'].getCurrentNode()
      if (testData === null || testData === undefined) {
        this.$message({
          title: '失败',
          message: '创建失败：未选择区域节点',
          type: 'error',
          duration: 2000
        })
        return
      }
      if (testData.Level !== 2) {
        this.$message({
          title: '失败',
          message: '请选择节点2区域',
          type: 'error',
          duration: 2000
        })
        return
      }
      this.DeviceEntity = {}
      this.DeviceEntity.AreaId = testData.Id
      this.DeviceEntity.AreaName = testData.Name
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 创建执行
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createVideo(this.DeviceEntity).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '创建成功',
                type: 'success',
                duration: 2000
              })
              this.DeviceEntity = {}
              this.getList()
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

    // 修改
    handleEdit(data) {
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.DeviceEntity = Object.assign({}, data)
      // this.Container = Object.assign({}, data)
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    editData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          editVideo(this.DeviceEntity).then((res) => {
            var resData = JSON.parse(res.data.Content)
            console.log(resData)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.$message({
                title: '成功',
                message: '编辑成功',
                type: 'success',
                duration: 2000
              })
              this.DeviceEntity = {}
              this.getList()
            } else {
              this.$message({
                title: '失败',
                message: '编辑失败' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    handleDelete(data) {
      this.$confirm('此操作将永久删除设备信息,是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.DeviceEntity = Object.assign({}, data)
        this.deleteData(this.DeviceEntity)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(data) {
      deleteVideo(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
          this.getTreeData()
          this.DeviceEntity = {}
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
    printMessage() {
      this.$print(this.$refs.print)
    }
  }

}
</script>
<style  rel="stylesheet/scss" lang="scss" scoped>
.rowItemClass {
  width: 80px;
  height: 30px;
  margin-top: 35px;
  margin-left: 30px;
}
.img-xys {
  display: block;
  height: auto;
  width: 100%;
  overflow: hidden;
  margin: 5% 0%;
}
.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
.clearfix:after {
  clear: both;
}

.right {
  border-radius: 50%;
  width: 15px;
  height: 15px;
  background-color: #67c23a;
  float: right;
  margin-left: 5px;
}
.error {
  border-radius: 50%;
  width: 15px;
  float: right;
  height: 15px;
  background-color: #f56c6c;
  margin-left: 5px;
}
</style>

