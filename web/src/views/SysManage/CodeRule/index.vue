<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Name" placeholder="编码名称" class="filter-item" clearable @clear="handleFilter" @keyup.enter.native="handleFilter" />
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
        <el-table-column label="编码" width="200px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Id }}</span>
          </template>
        </el-table-column>
        <el-table-column label="名称" width="120px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column label="重置规则" width="200px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Reset }}</span>
          </template>
        </el-table-column>
        <el-table-column label="步长" width="80px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Step }}</span>
          </template>
        </el-table-column>
        <el-table-column label="当前序号" width="100px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CurrentNo }}</span>
          </template>
        </el-table-column>
        <el-table-column label="当前编码" width="200px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CurrentCode }}</span>
          </template>
        </el-table-column>
        <el-table-column label="实体映射" width="100px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.EntityFullName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="具体描述" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column label="创建信息" width="200px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <div>{{ scope.row.CreatedUserName }}</div>
            <div>{{ scope.row.CreatedTime }}</div>
          </template>
        </el-table-column>

        <el-table-column label="操作" align="center" width="230" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button size="mini" @click="handleUpdate(scope.row)">编辑</el-button>
            <el-button size="mini" type="primary" @click="handleDetail(scope.row)">详情</el-button>
            <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>

      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[8,16,24,36]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'68%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="CodeRule" class="dialog-form" label-width="100px" label-position="left" style="width:90%">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="'编码'" prop="Id">
              <el-input v-model="CodeRule.Id" class="dialog-input" placeholder="输入编码" />
            </el-form-item>
            <el-form-item :label="'名称'" prop="Name">
              <el-input v-model="CodeRule.Name" class="dialog-input" placeholder="输入名称" />
            </el-form-item>
            <el-form-item :label="'步长'" prop="Step">
              <el-input v-model="CodeRule.Step" class="dialog-input" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="'重置规则'" prop="Reset">
              <el-select v-model="CodeRule.Reset" placeholder="重置规则" style="width:300px">
                <el-option
                  v-for="item in ResetAssemblys"
                  :key="item.Value"
                  :label="item.Key"
                  :value="item.Value"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'实体映射'" prop="EntityFullName">
              <el-select v-model="CodeRule.EntityFullName" placeholder="实体映射" style="width:300px" filterable>
                <el-option
                  v-for="item in EntityInfo"
                  :key="item.Id"
                  :label="item.Name"
                  :value="item.Id"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="'备注'">
              <el-input v-model="CodeRule.Remark" :autosize="{ minRows: 1, maxRows: 2}" type="textarea" placeholder="备注说明" class="dialog-input" />
            </el-form-item>
          </el-col>
        </el-row>
        <h4 class="page-title-h4">规则项目</h4>
        <hr class="line-sub">
        <el-row>
          <el-form-item
            v-for="(item, index) in itemList"
            :key="item.Id"
            :label="'规则' + (index+1)"
            style="width:100%"
          >
            <el-select v-model="item.Name" placeholder="规则程序集" style="width:250px">
              <el-option
                v-for="ruleItem in CodeRuleList"
                :key="ruleItem.Value"
                :label="ruleItem.Key"
                :value="ruleItem.Value"
              />
            </el-select>
            <el-input v-model="item.Value" style="width:150px" placeholder="值" />
            <el-input v-model="item.Sort" style="width:80px" placeholder="排序" />
            <el-select v-model="item.PaddingSide" style="width:150px" placeholder="填充模式">
              <el-option
                v-for="item2 in options"
                :key="item2.value"
                :label="item2.label"
                :value="item2.value"
              />
            </el-select>
            <el-input v-model="item.PaddingWidth" style="width:100px" placeholder="填充长度" />
            <el-input v-model="item.PaddingChar" style="width:100px" placeholder="填充字符" />
            <el-button style="float:right" @click.prevent="removeNormalDomain(item)">删除</el-button></el-form-item>
          <el-form-item>
            <el-button type="danger" @click="addNormalDomain">新增规则</el-button>
            <el-button @click="resetForm('dataForm')">重置</el-button>
          </el-form-item>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" :disabled="disabled" @click="createData">确认</el-button>
        <el-button v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>

    <!-- 详情 弹出框 -->
    <el-dialog v-el-drag-dialog title="详情" :visible.sync="dialogDetailVisible" width="40%" :close-on-click-modal="false">
      <el-table
        key="1"
        v-loading="listLoading"
        :data="detailList"
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
        <el-table-column label="规则程序集" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column label="值" width="80px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Value }}</span>
          </template>
        </el-table-column>
        <el-table-column label="排序" width="100px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.Sort }}</span>
          </template>
        </el-table-column>
        <el-table-column label="填充模式" width="200px" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.PaddingSide }}</span>
          </template>
        </el-table-column>
      </el-table>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogDetailVisible = false">取消</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>
import { fetchList, getCodeRuleList, getResetAssemblys, getEntityInfo, createCodeRule, editCodeRule, deleteCodeRule, getItems } from '@/api/SysManage/CodeRule'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import { debounce } from '@/utils'

export default {
  name: 'CodeRule', // 编码规则
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
        Rows: 8,
        Name: undefined,
        type: undefined,
        Sort: 'id'
      },

      // 实体
      CodeRule: {
        Id: undefined,
        Name: '',
        Delimiter: '',
        Reset: '',
        RuleJson: '',
        Step: 1,
        Remark: '',
        EntityFullName: ''
      },
      // 实体
      CodeRuleItem: {
        Id: undefined,
        CodeRuleId: '',
        Name: '',
        Value: '',
        Sort: 1,
        PaddingSide: '',
        PaddingWidth: undefined,
        PaddingChar: ''
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑编码规则',
        create: '创建编码规则'
      },
      // 详情弹出框
      dialogDetailVisible: false,
      detailList: null,
      // 输入规则
      rules: {
        Id: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入编码规则名称', trigger: 'blur' }],
        Reset: [{ required: true, message: '请选择重置程序集', trigger: 'blur' }],
        Step: [{ required: true, message: '请输入步长', trigger: 'blur' }],
        EntityFullName: [{ required: true, message: '请选择映射实体', trigger: 'blur' }]
      },
      disabled: false,
      itemList: [],
      options: [{
        value: 'None',
        label: '不填充'
      }, {
        value: 'Left',
        label: '左边'
      }, {
        value: 'Right',
        label: '右边'
      }],
      value: '',
      ResetAssemblys: [],
      CodeRuleList: [],
      EntityInfo: []
    }
  },
  watch: {
    // 授权面板关闭，清空原编码规则查询权限
    dialogTreeVisible(value) {
      if (!value) {
        this.resetModuleAuthData()
      }
    },
    // 授权面板关闭，清空原编码规则查询权限
    dialogFormVisible(value) {
      if (!value) {
        this.disabled = false
        this.resetCodeRule()
      }
    }
  },
  created() {
    this.initData()
  },
  methods: {
    initData() {
      this.getList()
      this.getCodeRuleList()
      this.getEntityInfo()
      this.getResetAssemblys()
      this.itemList.push(this.CodeRuleItem)
    },
    // 获取列表数据
    getList() {
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.total = usersData.total

        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    // 获取规则列表
    getCodeRuleList() {
      getCodeRuleList().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.CodeRuleList = usersData
      })
    },
    // 获取重置程序集
    getResetAssemblys() {
      getResetAssemblys().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.ResetAssemblys = usersData
      })
    },
    // 获取实体信息
    getEntityInfo() {
      getEntityInfo().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.EntityInfo = usersData
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
    // 编码规则创建
    handleCreate() {
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.disabled = true
          this.CodeRule.RuleJson = JSON.stringify(this.itemList)
          createCodeRule(this.CodeRule).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.list.unshift(this.CodeRule)
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
            this.disabled = false
          })
        }
      })
    },
    // 编码规则编辑
    handleUpdate(row) {
      this.CodeRule = Object.assign({}, row) // copy obj
      getItems(row.Id).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.itemList = usersData
      })
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          this.CodeRule.RuleJson = JSON.stringify(this.itemList)
          editCodeRule(this.CodeRule).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.CodeRule.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.CodeRule)
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
    // 编码规则删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该编码规则, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.CodeRule = Object.assign({}, row) // copy obj
        this.deleteData(this.CodeRule)
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
      deleteCodeRule(data).then((res) => {
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
    handleDetail(row) {
      this.dialogDetailVisible = true
      getItems(row.Id).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.detailList = usersData
      })
    },
    // 增加项目
    addNormalDomain() {
      const id = this.itemList.length + 1
      this.itemList.push({
        CodeRuleId: '',
        Name: '',
        Value: '',
        Sort: id,
        PaddingSide: '',
        PaddingWidth: undefined,
        PaddingChar: ''
      })
    },
    // 删除项目
    removeNormalDomain(item) {
      var index = this.itemList.indexOf(item)
      if (index !== -1) {
        this.itemList.splice(index, 1)
      }
    },
    // 重置
    resetCodeRule() {
      this.CodeRule = {
        Id: undefined,
        Name: '',
        Delimiter: '',
        Reset: '',
        RuleJson: '',
        Step: 1,
        Remark: '',
        EntityFullName: ''
      }
      this.itemList = []
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
