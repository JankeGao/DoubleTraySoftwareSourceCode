<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card>
          <div>
            <span>
              <div style="display: inline-block;"><h4>智能监控设备</h4> </div>
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
      <el-col :span="18">
        <el-card class="search-card">
          <div class="filter-container">
            <span style="font-size:15px;margin:0px 20px">{{ areaName }}</span>
            <el-select v-model="listQuery.AreaId" placeholder="请选择" @change="handleStatusChange">
              <el-option
                v-for="item in options"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </div>
        </el-card>
        <el-card>
          <el-row :gutter="20">
            <el-col v-for="item in videoList" :key="item.Code" :span="6" style="margin-bottom: 20px;">
              <el-card :body-style="{ padding: '0px'}" style="background:#F2F6FC" class="item-card" shadow="hover">
                <div slot="header" class="clearfix">
                  <span class="title" type="index">{{ item.Sort }}</span>
                  <span class="title">设备信息</span>
                  <el-button type="text" icon="el-icon-edit" style="margin-left:10px" @click="handleEdit(item)" /><!--编辑-->
                  <span v-if="item.Status=='1'" style="float:right;height:20px;margin-top:5px">
                    <div class="right" />
                  </span>
                  <span v-else style="float:right;height:20px;margin-top:5px">
                    <div class="error" />
                  </span>
                </div>
                <el-row :gutter="5" style="margin:10px">
                  <el-col :span="12">
                    <div style="margin:10px 0px">
                      <div class="title" style="margin:10px">ID</div>
                      <div class="label" style="margin:10px">{{ item.Id }}</div>
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
                      <div class="title" style="margin:10px">标题</div>
                      <div class="label" style="margin:10px">本机</div>
                    </div>
                  </el-col>
                </el-row>
              </el-card>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <div class="pagination-container">
              <el-pagination :current-page="listQuery.Page" :page-sizes="[8,16,24, 32]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
            </div>
          </el-row>
        </el-card>
      </el-col>
    </el-row>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="DeviceEntity" class="dialog-form" label-width="120px" label-position="right">
        <el-form-item label="设备编码" prop="Code">
          <el-input v-model="DeviceEntity.Code" class="dialog-input" />
        </el-form-item>
        <el-form-item label="设备名称" prop="Name">
          <el-input v-model="DeviceEntity.Remark" class="dialog-input" />
        </el-form-item>
        <el-form-item label="设备地址" prop="Address">
          <el-input v-model="DeviceEntity.Id" class="dialog-input" />
        </el-form-item>
        <el-form-item label="设备端口" prop="Port">
          <el-input v-model="DeviceEntity.Port" class="dialog-input" />
        </el-form-item>
        <el-form-item label="服务器地址" prop="ServerAddress">
          <el-input v-model="DeviceEntity.Ip" class="dialog-input" />
        </el-form-item>
        <el-form-item label=" 设备描述">
          <el-input v-model="DeviceEntity.EquipmentType" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" placeholder="请描述下设备具体功能" class="dialog-input" />
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
import { createVideo, fetchList, editVideo, deleteVideo, getWareHouseIDData } from '@/api/DeviceInfo'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  directives: {
    elDragDialog
  },
  data() {
    return {
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址

      areaName: '',
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
        SearchCode: ''
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      currentAreaId: 0,
      textMap: {
        update: '编辑模块',
        create: '创建模块'
      },
      // 输入规则
      rules: {

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
      Level: 1
    }
  },
  created() {
    this.getTreeData()
    this.handleFilter()
  },
  methods: {
    getTreeData() {
      getWareHouseIDData(1).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData(usersData)
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
        var areaList = []
        var twoChild = []
        areaList = item.ContainerList
        var channelList = []
        channelList = item.TrayList
        areaList.forEach(area => {
          var threeChild = []
          channelList.forEach(channel => {
            if (channel.AreaCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
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
        item.Name = item.Code + '(货柜)'
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
        AreaCode: item.AreaCode,
        Id: item.Id
      }
      return treeData
    },
    handleNodeClick(data) {
      this.Level = data.Level
      console.log(data)
      this.listQuery.Level = this.Level
      // this.listQuery.Code = data.Code
      this.listQuery.Page = 1
      this.listQuery.AreaId = data.Id
      if (data.Level === 1) {
        // this.AllEntity.WareHouseCode = data.Code
        this.listQuery.Level = 1
      }
      if (data.Level === 2) {
        this.AllEntity.AreaCode = data.Code
        this.AllEntity.WareHouseCode = data.WareHouseCode
        this.listQuery.WareHouseCode = data.WareHouseCode
      }
      if (data.Level === 3) {
        this.AllEntity.AreaCode = data.AreaCode
        this.AllEntity.WareHouseCode = data.WareHouseCode
        this.listQuery.WareHouseCode = data.WareHouseCode
        this.AllEntity.ChannelId = data.Id
        this.listQuery.Code = data.Id
      }
      this.getList()
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
    }
  }

}
</script>
<style  rel="stylesheet/scss" lang="scss" scoped>
.rowItemClass{
 width :80px;
 height:30px;
 margin-top: 35px;
 margin-left: 30px;
}
.img-xys {
  display: block;
  height: auto;
  width:100%;
  overflow: hidden;
  margin:5% 0%;
  }
  .clearfix:before,
  .clearfix:after {
    display: table;
    content: "";
  }
  .clearfix:after {
    clear: both
  }
</style>

