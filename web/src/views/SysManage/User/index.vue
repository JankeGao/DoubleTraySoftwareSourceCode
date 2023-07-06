
<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input
          v-model="listQuery.Name"
          placeholder="姓名"
          clearable
          class="filter-item"
          @clear="handleFilter"
          @keyup.enter.native="handleFilter"
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
    </el-card>

    <!-- 表格 -->
    <el-card>
      <el-table
        :key="tableKey"
        v-loading="listLoading"
        :data="list"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        fit
        highlight-current-row
        style="width: 100%;min-height:100%;"
      >
        <el-table-column type="index" width="50" />
        <el-table-column :label="'用户名'" width="150px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'姓名'" width="100px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'用户组'" align="left" show-overflow-tooltip>
          <template slot-scope="scope">
            <span v-for="Role of scope.row.Role" :key="Role.Id" style="margin:3px">
              <el-tag :type="'success'">{{ Role.Name }}</el-tag>
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="'员工卡号'" width="180px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.RFIDCode }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'手机'" width="180px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Mobilephone }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'性别'" width="100px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.SexCaption }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'启用'" width="100px" align="center">
          <template slot-scope="scope">
            <el-switch v-model="scope.row.Enabled " disabled />
          </template>
        </el-table-column>
        <el-table-column :label="'创建人'" width="150px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'创建日期'" width="200px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
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
            <el-row :gutter="10">
              <el-col :span="8">
                <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
              </el-col>
              <el-col :span="8">
                <el-button size="mini" type="primary" @click="handleAuthorization(scope.row)">授权</el-button>
              </el-col>
              <el-col :span="8">
                <el-button size="mini" type="danger" @click="handleDelete(scope.row,'deleted')">删除</el-button>
              </el-col>
            </el-row>
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

    <!-- 创建 -->
    <el-dialog
      v-el-drag-dialog
      :visible.sync="dialogFormVisible"
      width="60%"
      :close-on-click-modal="false"
      :title="textMap[dialogStatus]"
    >
      <el-form
        ref="dataForm"
        :model="User"
        :rules="rules"
        status-icon
        label-width="100px"
        class="page-form"
        label-position="right"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="用户名:" prop="Code" class="item">
              <el-input
                v-model="User.Code"
                placeholder="请输入用户名"
                class="dialog-input"
                :disabled="dialogStatus==='update'"
              />
            </el-form-item>
            <el-form-item label="员工卡号:" class="item">
              <el-input
                v-model="User.RFIDCode"
                placeholder="请输入员工卡号"
                class="dialog-input"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                maxlength="11"
              />
            </el-form-item>
            <el-form-item label="手机:" class="item">
              <el-input
                v-model="User.Mobilephone"
                placeholder="请输入手机号码"
                class="dialog-input"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                maxlength="11"
              />
            </el-form-item>
            <el-form-item label="密码:" prop="Password" class="item">
              <div v-if="dialogStatus=== 'create'">
                <el-tooltip effect="light" content="默认密码为：123456" placement="right">
                  <el-input
                    v-model="User.Password"
                    type="password"
                    auto-complete="off"
                    class="dialog-input"
                  />
                </el-tooltip>
              </div>
              <div v-else>
                <el-input
                  v-model="User.Password"
                  type="password"
                  auto-complete="off"
                  class="dialog-input"
                  @change="changePassword"
                />
              </div>
            </el-form-item>
            <el-form-item label="确认密码:" prop="checkPass" class="item">
              <el-input
                v-model="User.checkPass"
                type="password"
                auto-complete="off"
                class="dialog-input"
              />
            </el-form-item>
            <el-form-item label="用户组:" prop="Role" class="item">
              <template>
                <div>
                  <el-checkbox-group v-model="User.Roles" size="medium" @change="selectedRole">
                    <el-checkbox-button
                      v-for="role in roleList"
                      :key="role.Code"
                      :label="role.Code"
                    >{{ role.Name }}</el-checkbox-button>
                  </el-checkbox-group>
                </div>
              </template>
            </el-form-item>
            <el-form-item label="姓名:" prop="Name" class="item">
              <el-input v-model="User.Name" class="dialog-input" placeholder="请输入姓名" />
            </el-form-item>
            <el-form-item label="性别:" class="item">
              <el-radio-group v-model="User.Sex" @change="selectedSex">
                <el-radio label="1">男</el-radio>
                <el-radio label="2">女</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="启用:" class="item">
              <el-switch v-model="User.Enabled" active-color="#13ce66" inactive-color="#ff4949" />
            </el-form-item>
          </el-col>
          <el-col v-if="dialogStatus=='update'" :span="12">
            <img
              :src="faceUrl"
              class="image"
              style="height:100%;width:100%; display: block;"
              fit="fit"
            />
            <div style="margin:20px 0px">人脸照片需小于200k</div>
            <el-upload
              :show-file-list="false"
              :on-success="uploadingPicture"
              :action="uploadFileLibrary"
              :before-upload="PictureStatus"
              :data="{Id:User.Id}"
              class="filter-button"
            >
              <el-button style="margin-bottom: 70px;">上传人脸图片</el-button>
            </el-upload>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createUser">确认</el-button>
        <el-button v-else type="primary" @click="updateUser">确认</el-button>
      </div>
    </el-dialog>

    <!-- 用户授权 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="'用户授权--'+User.Name"
      :visible.sync="dialogTreeVisible"
      :close-on-click-modal="false"
      :fullscreen="true"
    >
      <div style="height:75vh;">
        <el-tabs v-model="activeName" type="card" @tab-click="handleClickTab">
          <el-tab-pane label="功能权限" name="first" style="height:65vh;">
            <el-scrollbar style="height:100%;">
              <el-form ref="treeDataForm">
                <el-row :gutter="20">
                  <el-col :span="24">
                    <el-card class="box-card">
                      <!-- <el-input :placeholder="'角色：'+ Role.Name" :disabled="true" style="width:100%;margin-bottom:5px;" /> -->
                      <el-tree
                        ref="tree"
                        :props="treeProps"
                        :data="treeData"
                        :default-checked-keys="moduleAuthData"
                        :check-strictly="true"
                        node-key="Code"
                        show-checkbox
                        :default-expand-all="true"
                      />
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </el-scrollbar>
          </el-tab-pane>
          <el-tab-pane label="仓库权限" name="second" style="height:70vh;">
            <el-scrollbar style="height:100%;">
              <el-row :gutter="10">
                <el-col :span="24">
                  <div>
                    <span>
                      <!-- :title="textMap[dialogStatus]" -->
                      <div style="display: inline-block;">
                        <h4>库存管理区域</h4>
                      </div>
                    </span>
                  </div>
                  <hr class="line" />
                  <div>
                    <el-tree
                      ref="treeTest"
                      :data="warehouseTreeData"
                      :expand-on-click-node="false"
                      style="font-size:20px;"
                      node-key="Id"
                      highlight-current
                      show-checkbox
                      default-expand-all
                      @check-change="handleCheckStockChange"
                    />
                  </div>
                </el-col>
              </el-row>
            </el-scrollbar>
          </el-tab-pane>
        </el-tabs>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogTreeVisible = false">取消</el-button>
        <el-button type="primary" @click="setAuthorization">确认</el-button>
      </span>
    </el-dialog>

    <!-- 用户授权 弹出框 -->
    <!-- <el-dialog
      v-el-drag-dialog
      :title="'用户授权--'+User.Name"
      :visible.sync="dialogTreeVisible"
      :width="'30%'"
      :close-on-click-modal="false"
    >
      <div style="height:600px">
        <el-scrollbar style="height:100%;">
          <el-form ref="treeDataForm">
            <el-row :gutter="20">
              <el-col :span="24">
                <el-card class="box-card">
                  <el-tree
                    ref="tree"
                    :props="treeProps"
                    :data="treeData"
                    :default-checked-keys="moduleAuthData"
                    :check-strictly="true"
                    node-key="Code"
                    show-checkbox
                    default-expand-all
                  />
                </el-card>
              </el-col>
            </el-row>
          </el-form>
        </el-scrollbar>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogTreeVisible = false">取消</el-button>
        <el-button type="primary" @click="setAuthorization">确认</el-button>
      </span>
    </el-dialog>-->
  </div>
</template>

<script>
import { userList, createUser, editUser, deleteUser, editUserArea } from '@/api/SysManage/User'
import { getWareHouseTreeDataByUserCode } from '@/api/WareHouse'
import { getRoleList } from '@/api/SysManage/Role'
import md5 from 'js-md5'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import { cleanParams } from '@/utils'
import { getModuleList, setAuthorization, getModuleUserRoleAuth } from '@/api/SysManage/Authorization'
import { getWareHouseTreeData } from '@/api/WareHouse'
export default {
  name: 'User', // 员工管理
  directives: {
    elDragDialog,
    waves
  },
  data() {
    var validatePass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入密码'))
      } else if (value.length < 5) {
        callback(new Error('密码不能小于5位'))
      } else {
        callback()
      }
    }
    var validatePass2 = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.User.Password) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    var validateRole = (rule, value, callback) => {
      if (this.User.Roles.length === 0) {
        callback(new Error('请至少选择一个角色'))
      } else {
        callback()
      }
    }
    var validateMobilephone = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入手机号码'))
      } else {
        if (this.User.Mobilephone.length < 6) {
          callback(new Error('请输入正确手机号码'))
        }
        callback()
      }
    }
    return {
      warehouseTreeData: [],
      tableKey: 0,
      // table 列表数据
      list: null,

      // 分页显示总查询数据
      total: null,
      listLoading: true,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 12,
        Name: undefined,
        type: undefined,
        Sort: 'id'
      },

      isChangePassword: false,
      // 用户实体
      User: {
        Id: undefined,
        Code: '',
        Name: '',
        Password: '123456',
        checkPass: '123456',
        Sex: '1',
        Mobilephone: '',
        Roles: [],
        WeXin: '',
        Remark: '',
        Header: window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png',
        Enabled: true
      },
      roleList: [],
      rules: {
        Code: [{ required: true, message: '请输入用户编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入用户姓名', trigger: 'blur' }],
        Role: [{ required: true, validator: validateRole, trigger: 'change' }],
        Mobilephone: [{ required: true, validator: validateMobilephone, trigger: 'blur', min: 6, max: 11 }],
        Password: [{ validator: validatePass, trigger: 'blur' }],
        checkPass: [{ required: true, validator: validatePass2, trigger: 'blur' }]
      },
      // 授权弹出框
      dialogTreeVisible: false,
      // 当前角色的权限
      moduleAuthData: [1],
      treeProps: {
        label: 'Name',
        children: 'children'
      },
      // 获取当前角色Tree列表
      treeData: [],
      ModuleAuths: {
        arr: [],
        typeCode: '',
        type: 1
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑用户',
        create: '创建用户'
      },
      src: '/logo.png',
      faceUrl: '', // '/logo.png',
      // PostDoFaceLibraryUpload
      // 访问的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFaceLibraryUpload1',
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      activeName: 'first'
    }
  },
  watch: {
    // 创建编辑面板关闭
    dialogFormVisible(value) {
      if (!value) {
        this.resetUser()
      }
    },
    // 创建编辑面板关闭
    dialogTreeVisible(value) {
      if (!value) {
        this.resetUser()
      }
    }
  },
  created() {
    this.initData()
  },
  methods: {
    // 初始化
    initData() {
      this.getList()
      this.getRoleList()
    },
    // 获取列表数据
    getList() {
      this.listLoading = true
      userList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 获取角色
    getRoleList() {
      getRoleList().then((res) => {
        if (res.status === 200) {
          var resData = JSON.parse(res.data.Content)
          resData.forEach(item => {
            this.roleList.push(item)
          })
        } else {
          this.$message({
            title: '失败',
            message: '获取角色信息失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
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
    getWareHouse() {
      // RawMaterial
      // getWareHouseTreeDataByUserCode('', this.User.Code).then(response => {
      //   var usersData = JSON.parse(response.data.Content)
      //   this.warehouseTreeData = this.convertTreeData1(usersData)
      // })
      getWareHouseTreeData(1).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.warehouseTreeData = this.convertTreeData2(usersData)
      })
    },
    convertTreeData1(data) {
      const treedataList = []
      var entity = {
        Code: 'WareHouse',
        Name: '仓库信息'
      }

      var first = this.generateRouter1(entity, true, 0)
      var firstchildren = []
      data.forEach(item => {
        var parent = this.generateRouter1(item, true, 1)
        var areaList = []
        var twoChild = []
        areaList = item.ContainerList
        var channelList = []
        channelList = item.LocationList
        areaList.forEach(area => {
          var threeChild = []
          channelList.forEach(channel => {
            if (channel.AreaCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              // threeChild.push(this.generateRouter(channel, false, 3))
            }
          })
          var twoParent = this.generateRouter1(area, false, 2)
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
    handleClickTab(e) {
      // 如果是数据权限
      if (e.name === 'second') {
        this.setCheckedStockNodes(this.User.StockArea)
      }
    },
    // 设置库存管理区域
    setCheckedStockNodes(data) {
      if (data == null) {
        return
      }
      var tarra = data.split(',')
      console.log(tarra)
      this.$nextTick(() => {
        var checkNodes = []
        tarra.forEach(element => {
          if (element != '') {
            checkNodes.push(element)
          }
        })
        this.$refs['treeTest'].setCheckedKeys(checkNodes)
      })

      // this.$nextTick(() => {
      //   var checkNodes = []
      //   const wareHouseArray = this.warehouseTreeData[0].children
      //   wareHouseArray.forEach(element => {
      //     element.children.forEach(area => {
      //       area.children.forEach(tray => {
      //         if (tray.Level === 3) {
      //           const id = tray.Id.toString()
      //           tarra.forEach(check => {
      //             if (check == id) {
      //               console.log('---')
      //               checkNodes.push(id)
      //             }
      //           })
      //         }
      //       })
      //     })
      //   })
      //   this.$refs['treeTest'].setCheckedKeys(checkNodes)
      // })
    },
    // 库存管理区域
    handleCheckStockChange(data, checked, indeterminate) {
      var array = this.$refs['treeTest'].getCheckedNodes()
      var areaCodes = ''
      this.$nextTick(() => {
        array.forEach(element => {
          if (element.Level === 3) {
            areaCodes = areaCodes + element.Id + ','
          }
        })

        this.User.StockArea = areaCodes // JSON.stringify(array).toString()
      })
    },
    // 员工信息创建
    handleCreate() {
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createUser() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.User.Password = md5(this.User.Password)
          this.User.Roles = JSON.stringify(this.User.Roles)
          this.User.Header = '/Assets/images/3.png'
          createUser(this.User).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '用户创建成功',
                type: 'success',
                duration: 2000
              })
              this.getList()
              this.dialogFormVisible = false
            } else {
              this.$message({
                title: '失败',
                message: '用户创建失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 会员信息编辑
    handleUpdate(row) {
      const user = Object.assign({}, row)
      user.checkPass = user.Password
      user.Roles = []
      this.User = user // copy obj
      const imageData = 'data:image/png;base64,' + this.User.Remark // this.BaseUrl + this.User.PictureUrl
      this.faceUrl = imageData.replace(/[\r\n]/g, '')
      if (this.User.Sex === 2) {
        this.User.Sex = '2'
      } else {
        this.User.Sex = '1'
      }
      this.User.Role.forEach(item => {
        this.User.Roles.push(item.Code)
      })
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateUser() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          if (this.isChangePassword) {
            this.User.Password = md5(this.User.Password)
          }
          this.User.Roles = JSON.stringify(this.User.Roles)
          editUser(this.User).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '用户编辑成功',
                type: 'success',
                duration: 2000
              })
              this.getList()
              this.dialogFormVisible = false
            } else {
              this.$message({
                title: '失败',
                message: '用户编辑失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 用户删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该用户, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.User = Object.assign({}, row) // copy obj
        this.deleteData(this.User)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(data) {
      deleteUser(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
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
    // 选择角色信息
    selectedRole(val) {
      this.User.Roles = val
    },
    selectedSex(val) {
      this.User.Sex = val
    },
    changePassword() {
      this.isChangePassword = true
    },
    // 是否创建了图片实例
    PictureStatus(datas) {
      this.JudgeFileLibraty = true
    },
    // 上传图片
    uploadingPicture(data) {
      var resData = JSON.parse(data.Content)
      if (resData.Success) {
        this.User.PictureUrl = resData.Data.FilePath
        const imageData = 'data:image/png;base64,' + resData.Data.Remark // this.BaseUrl + this.User.PictureUrl
        this.faceUrl = imageData.replace(/[\r\n]/g, '')
        this.User.Remark = resData.Data.Remark
        this.User.FileID = resData.Data.Id
      } else {
        this.$message({
        })
      }
    },
    // 角色授权
    handleAuthorization(row) {
      this.User = Object.assign({}, row) // copy obj
      this.getWareHouseTreeData()
      this.getModuleList() // 获取权限列表
      this.getModuleAuth(this.User.Code) // 获取当前用户权限
      // this.getModuleUserAuth(this.User.Code) // 获取当前用户权限
      this.dialogTreeVisible = true
      this.$nextTick(() => {
        this.$refs['treeDataForm'].clearValidate()
      })
    },
    // 获取Tree模块列表
    getModuleList() {
      getModuleList().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = this.convertTreeData(usersData)

        // Just to simulate the time of the request
        setTimeout(() => {
        }, 1 * 500)
      })
    },
    // Tree数组转换
    convertTreeData(arr) {
      const treedataList = [] // 返回的路由数组
      var routerData = arr // data中的值为数组
      if (JSON.stringify(routerData) !== '[]') {
        routerData.forEach(item => {
          if (item.ParentCode === null || item.ParentCode === '') { // 如果不存在上级，则为1级菜单，此部分可根据后端返回的数据重新定义完善
            var parent = this.generateRouter(item, true)
            var children = []
            routerData.forEach(child => {
              if (child.ParentCode === item.Code) { // 查找该父级路由的子级路由
                children.push(this.generateRouter(child, false))
                parent.children = children
              }
            })
            treedataList.push(parent)
          }
        })
      }
      return treedataList
    },
    generateRouter(item, isParent) {
      var treeData = {
        Code: item.Code,
        Name: item.Name
      }
      return treeData
    },
    // 获取当前用户下的角色模块权限
    getModuleAuth(data) {
      getModuleUserRoleAuth(data, 1).then(response => {
        var usersData = JSON.parse(response.data.Content)
        const defaultTreeNode = []
        const list = this.treeData // 全部权限模块
        usersData.forEach(item => {
          // 判断如果为角色权限，则无法更改
          list.forEach(it => {
            // 父级菜单
            if (it.Code === item.Code && item.Type === 1) {
              console.log(it.Code)
              it.disabled = true
            }
            // 子菜单
            it.children.forEach(i => {
              if (item.Code === i.Code && item.Type === 1) {
                i.disabled = true
              }
            })
          })
          defaultTreeNode.push(item.Code)
        })
        this.moduleAuthData = defaultTreeNode
      })
    },
    // 更新权限模块
    setAuthorization() {
      // 获取当前选中的Key值
      const nodelist = this.$refs.tree.getCheckedKeys()
      for (let i = 0; i < nodelist.length; i++) {
        var ModuleAuth = Object.create(null)
        ModuleAuth.Type = 1
        ModuleAuth.TypeCode = this.User.Code
        ModuleAuth.ModuleCode = nodelist[i]
        this.ModuleAuths.arr.push(ModuleAuth)
      }
      this.ModuleAuths.typeCode = this.User.Code
      this.ModuleAuths.type = 5
      setAuthorization(this.ModuleAuths).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          // resetTemp(this.ModuleAuths)
          // this.dialogTreeVisible = false
          // window.location.reload()
          editUserArea(this.User).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '用户授权成功',
                type: 'success',
                duration: 2000
              })
              this.dialogTreeVisible = false
              window.location.reload()
            } else {
              this.$message({
                title: '失败',
                message: '用户授权失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
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
    },
    // 重置
    resetUser() {
      this.User = {
        Id: undefined,
        Code: '',
        Name: '',
        Password: '123456',
        checkPass: '123456',
        Sex: '1',
        Mobilephone: '',
        Roles: [],
        WeXin: '',
        Remark: '',
        Header: window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png',
        Enabled: true
      }
      this.moduleAuthData = []
      this.treeData = []
    },
    getWareHouseTreeData() {
      getWareHouseTreeData(1).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.warehouseTreeData = this.convertTreeData2(usersData)
        console.log(this.warehouseTreeData)
      })
    },
    convertTreeData2(data) {
      const treedataList = []
      var entity = {
        Code: 'WareHouse',
        Name: '仓库信息'
      }
      var first = this.generateRouter2(entity, true, 0)
      var firstchildren = []
      data.forEach(item => {
        var parent = this.generateRouter2(item, true, 1)
        var containerList = []
        var twoChild = []
        containerList = item.ContainerList
        var trayList = []
        trayList = item.TrayList
        containerList.forEach(area => {
          var threeChild = []
          trayList.forEach(channel => {
            if (channel.ContainerCode === area.Code && channel.WareHouseCode === area.WareHouseCode) {
              threeChild.push(this.generateRouter2(channel, false, 3))
            }
          })
          var twoParent = this.generateRouter2(area, false, 2)
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
    generateRouter2(item, isParent, level) {
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
        Id: level + '-' + item.Id
      }
      return treeData
    },
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
// 单页面样式
.pro-picture {
  width: 50px;
  height: 50px;
  border-radius: 10px;
}
</style>
