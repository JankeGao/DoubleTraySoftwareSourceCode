
<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate('')">添加</el-button>
      </div>
    </el-card>

    <!-- 表格 -->
    <el-card>
      <tree-table :key="1" :data="tableData" :columns="columns" :default-expand-all="false" :header-cell-style="{background:'#F5F7FA'}" border row-key="Id" highlight-current-row>
        <template slot="operation" slot-scope="{scope}">
          <el-button v-if="scope.row.ParentCode===''||scope.row.ParentCode===null" type="primary" size="mini" @click="handleCreate(scope.row.Code)">添加</el-button>
          <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
          <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </tree-table>
    </el-card>

    <!-- 创建 -->
    <el-dialog v-el-drag-dialog :visible.sync="dialogFormVisible" width="30%" :close-on-click-modal="false" :title="textMap[dialogStatus]">
      <el-form ref="dataForm" :model="Department" :rules="rules" status-icon label-width="100px" class="page-form" label-position="right">
        <el-form-item label="部门编码:" prop="Code" class="item">
          <el-input v-model="Department.Code" placeholder="请输入部门编码" class="dialog-input" :disabled="dialogStatus==='update'" />
        </el-form-item>
        <el-form-item label="部门名称:" prop="Name" class="item">
          <el-input v-model="Department.Name" placeholder="请输入部门名称" class="dialog-input" />
        </el-form-item>
        <el-form-item label="上级编码:" class="item">
          <el-input v-model="Department.ParentCode" placeholder="请输入上级编码" class="dialog-input" />
        </el-form-item>
        <el-form-item label="负责人:" class="item">
          <el-input v-model="Department.Manager" class="dialog-input" placeholder="请输入负责人姓名" />
        </el-form-item>
        <el-form-item :label="'具体描述:'" class="item">
          <el-input v-model="Department.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" placeholder="备注信息" class="dialog-input" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createDepartment">确认</el-button>
        <el-button v-else type="primary" @click="updateDepartment">确认</el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import md5 from 'js-md5'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import { cleanParams } from '@/utils'
import { queryList, createDepartment, editDepartment, deleteDepartment } from '@/api/SysManage/Department'
import treeTable from '@/components/TreeTable'

export default {
  name: 'Department', // 员工管理
  components: { treeTable },
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      tableKey: 0,
      // table 列表数据
      list: null,
      tableData: '',
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
      // 部门实体
      Department: {
        Id: undefined,
        Code: '',
        Name: '',
        Manager: '',
        ParentCode: '',
        Remark: ''
      },
      roleList: [],
      rules: {
        Code: [{ required: true, message: '请输入部门编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入部门名称', trigger: 'blur' }]
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
        update: '编辑部门',
        create: '创建部门'
      },
      src: '/logo.png',
      faceUrl: '/logo.png',
      // 访问的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFaceLibraryUpload',
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      columns: [
        {
          label: '部门编码',
          width: 200,
          key: 'Code',
          align: 'left'
        },
        {
          label: '部门名称',
          key: 'Name',
          width: 200
        },
        {
          label: '部门负责人',
          key: 'Manager'
        },
        {
          label: '上级编码',
          width: 100,
          key: 'ParentCode'
        },
        {
          label: '操作',
          width: 280,
          key: 'operation'
        }
      ]
    }
  },
  watch: {
    // 创建编辑面板关闭
    dialogFormVisible(value) {
      if (!value) {
        this.resetDepartment()
      }
    },
    // 创建编辑面板关闭
    dialogTreeVisible(value) {
      if (!value) {
        this.resetDepartment()
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
      // this.getRoleList()
    },
    // 获取列表数据
    getList() {
      this.listLoading = true
      queryList(this.listQuery).then(response => {
        var DepartmentsData = JSON.parse(response.data.Content)
        console.log(DepartmentsData)
        this.tableData = DepartmentsData
        //  this.total = DepartmentsData.total

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
    // 员工信息创建
    handleCreate(data) {
      this.Department.ParentCode = data
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createDepartment() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createDepartment(this.Department).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '部门创建成功',
                type: 'success',
                duration: 2000
              })
              this.getList()
              this.dialogFormVisible = false
            } else {
              this.$message({
                title: '失败',
                message: '部门创建失败：' + resData.Message,
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
      const Department = Object.assign({}, row)
      this.Department = Department // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateDepartment() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          if (this.isChangePassword) {
            this.Department.Password = md5(this.Department.Password)
          }
          this.Department.Roles = JSON.stringify(this.Department.Roles)
          editDepartment(this.Department).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '部门编辑成功',
                type: 'success',
                duration: 2000
              })
              this.getList()
              this.dialogFormVisible = false
            } else {
              this.$message({
                title: '失败',
                message: '部门编辑失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 部门删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该部门, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Department = Object.assign({}, row) // copy obj
        this.deleteData(this.Department)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(data) {
      deleteDepartment(data).then((res) => {
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
    // 重置
    resetDepartment() {
      this.Department = {
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
    }
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
