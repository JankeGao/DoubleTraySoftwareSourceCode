<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Code" :placeholder="$t('Supply.SupplierSearch')" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }}</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
        <el-button v-waves :loading="downloadLoading" class="filter-button" type="primary" @click="handleDownUpload">{{ $t('baseBtn.templateBtn') }}</el-button>
        <el-upload
          ref="fileupload"
          style="display: inline; margin-left: 10px;margin-right: 10px;"
          action="#"
          :show-file-list="false"
          :http-request="uploadFile"

          :on-exceed="handleExceed"
        >
          <el-button type="primary">{{ $t('baseBtn.importBtn') }}</el-button>
        </el-upload>
        <el-dropdown
          size="small"
          placement="bottom"
          trigger="click"
          @command="batchOperate"
        >
          <el-button class="filter-button" style="margin-left: 10px;" type="primary">
            {{ $t('baseBtn.exportBtn') }}
            <i class="el-icon-arrow-down el-icon--right" />
          </el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item command="All_Export"> {{ $t('baseBtn.exportBtnWhole') }}</el-dropdown-item>
            <el-dropdown-item command="Condition_Export"> {{ $t('baseBtn.exportBtnCondition') }}</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </div>
    </el-card>
    <el-row>
      <el-card>
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
        >
          <el-table-column type="index" width="50" />
          <el-table-column :label="$t('Supply.Code')" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('Supply.Name')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Name }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('Supply.Linkman')" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Linkman }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('Supply.Phone')" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Phone }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('Supply.Address')" width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Address }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('Supply.Remark')" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'操作'" align="center" width="250" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">编辑</el-button>
              <el-button size="mini" type="danger" @click="handleDelete(scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listQuery.Page" :page-sizes="[15,30,45,60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
        </div>

      </el-card>
    </el-row>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Supply" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'供应商编码'" prop="Code">
          <el-input v-model="Supply.Code" clearable :disabled="dialogStatus == 'update'" class="dialog-input" placeholder="请输入供应商编码" />
        </el-form-item>
        <el-form-item :label="'供应商名称'" prop="Name">
          <el-input v-model="Supply.Name" clearable class="dialog-input" placeholder="请输入供应商名称" />
        </el-form-item>
        <el-form-item :label="'联系人'">
          <el-input v-model="Supply.Linkman" clearable class="dialog-input" placeholder="请输入联系人" />
        </el-form-item>
        <el-form-item :label="'联系方式'" prop="Unit">
          <el-input v-model="Supply.Phone" clearable class="dialog-input" placeholder="请输入联系方式" />
        </el-form-item>
        <el-form-item :label="'地址'">
          <el-input v-model="Supply.Address" clearable class="dialog-input" placeholder="请输入供应商地址" />
        </el-form-item>
        <el-form-item :label="'备注'">
          <el-input v-model="Supply.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" placeholder="供应商备注" class="dialog-input" />
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
import { ouLoadSupplyInfo, getPageRecords, createSupply, editSupply, deleteSupply } from '@/api/Supply'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'Supply', // 供应商管理
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
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑供应商',
        create: '创建供应商'
      },
      // 供应商实体
      Supply: {
        Id: undefined,
        Code: '',
        Name: '',
        Linkman: '',
        Phone: 0,
        Remark: '',
        Address: ''
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '请输入供应商编码', trigger: 'blur' }],
        Name: [{ required: true, message: '请输入供应商名称', trigger: 'blur' }]
      }
    }
  },
  watch: {

  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
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
    handleCreate() {
      this.resetSupply()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createSupply(this.Supply).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
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

    // 供应商编辑
    handleUpdate(row) {
      this.Supply = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const SupplyData = Object.assign({}, this.Supply)
          editSupply(SupplyData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Supply.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Supply)
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
                message: '更新失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 上传文件个数超过定义的数量
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，请删除后继续上传`)
    },
    // 上传文件
    uploadFile(item) {
    //  console.log(item)
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      // let formTwo = JSON.stringify(form)
      ouLoadSupplyInfo(form).then((res) => {
        var resData = typeof res.data === 'string' ? JSON.parse(res.data) : res.data
        console.log(typeof resData)
        // var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '导入成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '失败',
            message: '导入失败:' + resData.Message,
            type: 'error',
            duration: 4000
          })
        }
      })
    },
    // 导出选择
    batchOperate(command) {
      switch (command) {
        case 'All_Export':
          this.All_ExportExcel()
          break
        case 'Condition_Export':
          this.Condition_ExportExcel()
          break
      }
    },
    // 条件导出
    Condition_ExportExcel() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Supply/DoDownLoadTempSupply?Code=' + this.listQuery.Code
      // console.log(this.listQuery.begin)
      // console.log(this.listQuery.end)
      window.open(url)
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm('此操作将导出全部数据，共：' + this.total + '条, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Supply/DoDownLoadTempSupply'
        window.open(url)
        console.log(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },
    handleDownUpload() {
      // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Supply/DoDownLoadTemp'
      var url = window.PLATFROM_CONFIG.baseUrl + '/Assets/themes/Excel/供应商导入模版.xlsx'
      window.open(url)
    },
    handleDelete(row) {
      this.$confirm('此操作将永久删除该供应商, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.Supply = Object.assign({}, row) // copy obj
        this.deleteData(this.Supply)
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
      deleteSupply(data).then((res) => {
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
    resetSupply() {
      this.Supply = {
        Id: undefined,
        Code: '',
        Name: '',
        ShortName: '',
        MinNum: 0,
        MaxNum: 0,
        Remark: '',
        Unit: '',
        PackageUnit: '',
        PackageQuantity: 0,
        IsQuality: false,
        IsBatch: false,
        IsDeleted: false,
        MinOutQuantity: 0,
        MinOutUnit: '',
        CreatedTime: undefined,
        CreatedUserCode: '',
        CreatedUserName: ''
      }
    }
  }

}
</script>

