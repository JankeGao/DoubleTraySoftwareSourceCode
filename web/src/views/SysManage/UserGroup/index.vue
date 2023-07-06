<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Name" placeholder="角色名称" class="filter-item" clearable @clear="handleFilter" @keyup.enter.native="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
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
        <el-table-column
          type="index"
          width="50"
        />
        <el-table-column :label="'角色名称'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'启用'" width="100px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.Enabled "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="'具体描述'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
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

        <el-table-column :label="'操作'" align="center" width="230" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
            <el-button size="mini" type="primary" @click="handleAuthorization(scope.row)">授权</el-button>
            <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>

      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Role" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'角色名称'" prop="Name">
          <el-input v-model="Role.Name" class="dialog-input" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item :label="'启用'">
          <el-switch
            v-model="Role.Enabled"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </el-form-item>
        <el-form-item :label="'具体描述'">
          <el-input v-model="Role.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" placeholder="请说明下角色的具体功能" class="dialog-input" />
        </el-form-item>
      </el-form>
      <div class="block">
        <span class="demonstration">默认</span>
        <el-image :src="src" />
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>

    <!-- 用户授权 弹出框 -->
    <el-dialog v-el-drag-dialog :title="'角色授权--'+Role.Name" :visible.sync="dialogTreeVisible" :width="'30%'" :close-on-click-modal="false">
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
    </el-dialog>

  </div>
</template>

<script>
import { fetchList, createRole, editRole, deleteRole } from '@/api/SysManage/Role'
import { getModuleList, setAuthorization, getModuleAuth } from '@/api/SysManage/Authorization'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'Role', // 职位管理
  directives: {
    elDragDialog,
    waves
  },
  filters: {
    statusFilter(status) {
      const statusMap = {
        published: 'success',
        draft: 'info',
        deleted: 'danger'
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      tableKey: 0,
      // table 列表数据
      list: null,
      src: 'https://cube.elemecdn.com/6/94/4d3ea53c084bad6931a56d5158a48jpeg.jpeg',
      // 分页显示总查询数据
      total: null,
      listLoading: true,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 10,
        Name: undefined,
        type: undefined,
        Sort: 'id'
      },

      // 角色实体
      Role: {
        Id: undefined,
        Code: '',
        Name: '',
        Remark: '',
        Enabled: true
      },
      // 模块权限实体
      ModuleAuths: {
        arr: [],
        typeCode: '',
        type: 1
      },

      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑角色',
        create: '创建角色'
      },
      // 授权弹出框
      dialogTreeVisible: false,
      // 当前角色的权限
      moduleAuthData: [],
      treeProps: {
        label: 'Name',
        children: 'children'
      },
      // 获取当前角色Tree列表
      treeData: [],

      // 输入规则
      rules: {
        type: [{ required: true, message: 'type is required', trigger: 'change' }],
        timestamp: [{ type: 'date', required: true, message: 'timestamp is required', trigger: 'change' }],
        Name: [{ required: true, message: '请输入角色名称', trigger: 'blur' }]
      }
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    dialogTreeVisible(value) {
      if (!value) {
        this.resetModuleAuthData()
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    printBtn() {
      const newstr = document.getElementsByClassName('dialogCare')[0].innerHTML
      window.document.body.innerHTML = newstr
      const oldstr = window.document.body.innerHTML
      window.print()
      window.location.reload() // 解决打印之后按钮失效的问题
      window.document.body.innerHTML = oldstr; return false
    },
    // 获取列表数据
    getList() {
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        console.log(response)
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
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
    // 角色创建
    handleCreate() {
      this.resetRole(this.Role)
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createRole(this.Role).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.list.unshift(this.Role)
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
    // 角色编辑
    handleUpdate(row) {
      this.Role = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const roleData = Object.assign({}, this.Role)
          editRole(roleData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Role.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Role)
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
    // 角色删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该角色, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Role = Object.assign({}, row) // copy obj
        this.deleteData(this.Role)
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
      deleteRole(data).then((res) => {
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
    // 角色授权
    handleAuthorization(row) {
      this.Role = Object.assign({}, row) // copy obj
      this.dialogTreeVisible = true
      this.getModuleList() // 获取权限列表
      this.getModuleAuth(this.Role.Code) // 获取当前角色权限
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
    // 获取当前角色模块权限
    getModuleAuth(data) {
      getModuleAuth(data).then(response => {
        var usersData = JSON.parse(response.data.Content)
        const defaultTreeNode = []
        usersData.forEach(item => {
          defaultTreeNode.push(item.Code)
        })
        this.moduleAuthData = defaultTreeNode
        // Just to simulate the time of the request
        setTimeout(() => {
        }, 1 * 500)
      })
    },
    // 更新权限模块
    setAuthorization() {
      // 获取当前选中的Key值
      const nodelist = this.$refs.tree.getCheckedKeys()
      for (let i = 0; i < nodelist.length; i++) {
        var ModuleAuth = Object.create(null)
        ModuleAuth.Type = 1
        ModuleAuth.TypeCode = this.Role.Code
        ModuleAuth.ModuleCode = nodelist[i]
        this.ModuleAuths.arr.push(ModuleAuth)
      }
      this.ModuleAuths.typeCode = this.Role.Code
      setAuthorization(this.ModuleAuths).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          // resetTemp(this.ModuleAuths)
          this.resetModuleAuths()
          this.resetModuleAuthData()
          this.dialogTreeVisible = false
          window.location.reload()
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
    // 重置
    resetRole() {
      this.Role = {
        Id: undefined,
        Code: '',
        Name: '',
        Remark: '',
        Enabled: true
      }
    },
    resetModuleAuths() {
      this.ModuleAuths = {
        arr: [],
        typeCode: '',
        type: 1
      }
    },
    resetModuleAuthData() {
      this.moduleAuthData = []
      this.treeData = []
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
  .el-scrollbar__view {
    height: calc(100% + 15px);
  }
  .el-scrollbar .el-scrollbar__wrap .el-scrollbar__view{
   white-space: nowrap;
   }
</style>
