<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate()">添加</el-button>
      </div>
    </el-card>
    <el-card>
      <tree-table :key="key" :data="tableData" :columns="columns" :default-expand-all="false" :header-cell-style="{background:'#F5F7FA'}" border row-key="Id" highlight-current-row>
        <template slot="Enabled" slot-scope="{scope}">
          <el-switch
            v-model="scope.row.Enabled "
            disabled
          />
        </template>
        <template slot="operation" slot-scope="{scope}">
          <el-button v-if="scope.row.ParentCode===''||scope.row.ParentCode===null" type="primary" size="mini" @click="handleCreate(scope.row.Code)">添加</el-button>
          <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
          <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </tree-table>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="module" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'模块编码'" prop="Code">
          <el-input v-model="module.Code" :disabled="dialogStatus==='update'" class="dialog-input" placeholder="请输入模块编码" />
        </el-form-item>
        <el-form-item :label="'模块名称'" prop="Name">
          <el-input v-model="module.Name" class="dialog-input" placeholder="请输入模块名称" />
        </el-form-item>
        <el-form-item :label="'上级编码'">
          <el-input v-model="module.ParentCode" class="dialog-input" placeholder="请输入模块上级编码" />
        </el-form-item>
        <el-form-item :label="'地址'" prop="Address">
          <el-input v-model="module.Address" class="dialog-input" placeholder="请输入模块地址" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input v-model="module.Sort" class="dialog-input" placeholder="请输入模块排序" />
        </el-form-item>
        <el-form-item label="图标">
          <span>
            <el-input v-model="module.Icon" class="dialog-input" placeholder="请选择模块图标" />
          </span>
          <span>
            <el-button icon="el-icon-search" circle type="primary" @click="dialogIconVisible = true" />
          </span>
        </el-form-item>
        <el-form-item :label="'启用'">
          <el-switch
            v-model="module.Enabled"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData(module)">确认</el-button>
      </div>
    </el-dialog>

    <!-- 选择图标 -->
    <el-dialog v-el-drag-dialog title="选择图标" :visible.sync="dialogIconVisible" :width="'80%'" :close-on-click-modal="false">
      <div class="icons-container">
        <div v-for="item of svgIcons" :key="item" @click="handleClipboard(item)">
          <div class="icon-item">
            <svg-icon :icon-class="item" class-name="disabled" />
            <span>{{ item }}</span>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getModuleTreeList, createModule, editModule, deleteModule } from '@/api/SysManage/Module'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { articleType } from '@/utils/enum'
import treeTable from '@/components/TreeTable'
import svgIcons from './svg-icons'
import elementIcons from './element-icons'

export default {
  name: 'Module', // 模块管理
  components: { treeTable },
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
      svgIcons,
      elementIcons,
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

      // 模块实体
      module: {
        Code: '',
        Name: '',
        ParentCode: '',
        Address: '',
        Sort: 1,
        Enabled: true,
        Target: 'Page',
        AuthType: 'Authorization',
        Type: 'Menu'
      },
      articleType: [],
      selectType: 0,
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑模块',
        create: '创建模块'
      },

      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入模块编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入模块名称', trigger: 'blur' }],
        Address: [{ required: true, message: '请输入模块地址', trigger: 'blur' }]
      },
      imagecropperShow: false,
      imagecropperKey: 0,
      showCheckbox: true,
      key: 1,
      columns: [
        {
          label: '编码',
          width: 200,
          key: 'Code',
          align: 'left'
        },
        {
          label: '名称',
          key: 'Name',
          width: 200
        },
        {
          label: '地址',
          key: 'Address'
        },
        {
          label: '排序',
          width: 100,
          key: 'Sort'
        },
        {
          label: '启用',
          width: 100,
          key: 'Enabled'
        },
        {
          label: '图标',
          width: 100,
          key: 'Icon'
        },
        {
          label: '操作',
          width: 280,
          key: 'operation'
        }
      ],
      tableData: '',
      //
      dialogIconVisible: false
    }
  },
  watch: {
    // 授权面板关闭，清空原文章查询权限
    dialogTreeVisible(value) {
      if (!value) {
        this.resetModuleAuthData()
      }
    }
  },
  created() {
    this.getList()
    // 科属
    this.articleType = articleType()
  },
  methods: {
    getList() {
      this.listLoading = true
      getModuleTreeList().then(response => {
        var usersData = JSON.parse(response.data.Content)
        console.log(usersData)
        this.tableData = usersData
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

    // 模块创建
    handleCreate(data) {
      this.module.ParentCode = data
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createModule(this.module).then((res) => {
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
              this.resetModule()
              //   window.location.reload()
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

    // 模块编辑
    handleUpdate(row) {
      this.module = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData(row) {
      const data = row
      var postModule = {
        Id: data.Id,
        Code: data.Code,
        Name: data.Name,
        ParentCode: data.ParentCode,
        Icon: data.Icon,
        Address: data.Address,
        Target: data.Target,
        Sort: data.Sort,
        Enabled: data.Enabled,
        Type: data.Type
      }
      editModule(postModule).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '编辑成功',
            type: 'success',
            duration: 2000
          })
          window.location.reload()
        } else {
          this.$message({
            title: '成功',
            message: '编辑失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 模块删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该模块、子模块以及用户授权, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'error'
      }).then(() => {
        this.module = Object.assign({}, row) // copy obj
        this.deleteData(this.module)
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
      deleteModule(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
          window.location.reload()
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

    handelSelect(value) {
      //    console.log(value)
    },
    generateIconCode(symbol) {
      return `<svg-icon icon-class="${symbol}" />`
    },
    generateElementIconCode(symbol) {
      return `<i class="el-icon-${symbol}" />`
    },
    handleClipboard(text) {
      this.dialogIconVisible = false
      this.module.Icon = text
    },
    // 重置
    resetModule() {
      this.module = {
        Code: '',
        Name: '',
        Icon: '',
        ParentCode: '',
        Address: '',
        Sort: 1,
        Enabled: true,
        Target: 'Page',
        AuthType: 'Authorization',
        Type: 'Menu'
      }
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
// 单页面样式
  .pro-picture {
    width: 80px;
    height: 50px;
    border-radius: 10px;
  }

  .icons-container {
  margin: 10px 20px 0;
  overflow: hidden;

  .icon-item {
    margin: 10px;
    height: 85px;
    text-align: center;
    width: 100px;
    float: left;
    font-size: 30px;
    color: #24292e;
    cursor: pointer;
  }

  span {
    display: block;
    font-size: 16px;
    margin-top: 10px;
  }

  .disabled {
    pointer-events: none;
  }
}
</style>
