<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Name" placeholder="定时对象编码" class="filter-item" clearable @clear="handleFilter" @keyup.enter.native="handleFilter" />
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
        <el-table-column :label="'对象编码'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'契约'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.NamespaceClass }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'方法'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Method }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'程序集'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.AssemblyName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="230" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
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
      <el-form ref="dataForm" :rules="rules" :model="JobObject" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'对象编码'" prop="Code">
          <el-input v-model="JobObject.Code" class="dialog-input" placeholder="请输入任务对象编码" />
        </el-form-item>
        <el-form-item :label="'契约'" prop="NamespaceClass">
          <el-input v-model="JobObject.NamespaceClass" class="dialog-input" placeholder="请输入任务契约" />
        </el-form-item>
        <el-form-item :label="'方法'" prop="Method">
          <el-input v-model="JobObject.Method" class="dialog-input" placeholder="请输入需要定时启动的方法" />
        </el-form-item>
        <el-form-item :label="'程序集'" prop="AssemblyName">
          <el-input v-model="JobObject.AssemblyName" class="dialog-input" placeholder="请输入程序集名称" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { fetchList, createJobObject, editJobObject, deleteJobObject } from '@/api/SysManage/JobObject'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import { cleanParams } from '@/utils'

export default {
  name: 'JobObject', // 定时对象
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

      // 任务对象实体
      JobObject: {
        Id: undefined,
        Code: '',
        NamespaceClass: '',
        Method: '',
        AssemblyName: ''
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑任务对象',
        create: '创建任务对象'
      },
      // 授权弹出框
      dialogTreeVisible: false,
      // 当前任务对象的权限
      moduleAuthData: [],
      treeProps: {
        label: 'Name',
        children: 'children'
      },
      // 获取当前任务对象Tree列表
      treeData: [],

      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入任务对象编码', trigger: 'blur' }],
        NamespaceClass: [{ required: true, message: '请输入任务对象契约', trigger: 'blur' }],
        Method: [{ required: true, message: '请输入定时启动方法', trigger: 'blur' }],
        AssemblyName: [{ required: true, message: '请输入定时启动程序集', trigger: 'blur' }]
      }
    }
  },
  watch: {
    // 授权面板关闭，清空原任务对象查询权限
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
    // 任务对象创建
    handleCreate() {
      this.resetJobObject(this.JobObject)
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createJobObject(this.JobObject).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.list.unshift(this.JobObject)
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
    // 任务对象编辑
    handleUpdate(row) {
      this.JobObject = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const JobObjectData = Object.assign({}, this.JobObject)
          editJobObject(JobObjectData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.JobObject.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.JobObject)
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
    // 任务对象删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该任务对象, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.JobObject = Object.assign({}, row) // copy obj
        this.deleteData(this.JobObject)
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
      deleteJobObject(data).then((res) => {
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
    resetJobObject() {
      this.JobObject = {
        Id: undefined,
        Code: '',
        Name: '',
        Remark: '',
        Enabled: true
      }
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
