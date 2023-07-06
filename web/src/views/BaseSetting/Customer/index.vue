<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Code" placeholder="$t('basicInformation.customerCodeAndCustomerName')" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }} </el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" @click="handleDownUpload">{{ $t('baseBtn.templateBtn') }}</el-button>
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
            <el-dropdown-item command="All_Export">{{ $t('baseBtn.allExport') }}</el-dropdown-item>
            <el-dropdown-item command="Condition_Export">{{ $t('baseBtn.queryExport') }}</el-dropdown-item>
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
          <!-- @row-click="handleRowClick" -->
          <el-table-column type="index" width="50" />
          <el-table-column :label=" $t('basicInformation.customerCodeAndCustomerName') " width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Code }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('basicInformation.theCustomerName') " align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Name }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('headerInformation.theContact') " width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Linkman }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('headerInformation.contact') " width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Phone }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('headerInformation.address') " width="180" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Address }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('headerInformation.remarks') " align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remark }}</span>
            </template>
          </el-table-column>
          <el-table-column :label=" $t('headerInformation.operation') " align="center" width="250" class-name="small-padding fixed-width" fixed="right">
            <template slot-scope="scope">
              <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
              <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
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
      <el-form ref="dataForm" :rules="rules" :model="Customer" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label=" $t('basicInformation.customerCodeAndCustomerName') " prop="Code">
          <el-input v-model="Customer.Code" clearable class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterTheCustomerCode') " />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.theCustomerName') " prop="Name">
          <el-input v-model="Customer.Name" clearable class="dialog-input" placeholder=" $t('basicInformation.pleaseEnterACustomerName') " />
        </el-form-item>
        <el-form-item :label=" $t('headerInformation.theContact') ">
          <el-input v-model="Customer.Linkman" clearable class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterContacts') " />
        </el-form-item>
        <el-form-item :label=" $t('headerInformation.contact') " prop="Unit">
          <el-input v-model="Customer.Phone" clearable class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterContactInformation') " />
        </el-form-item>
        <el-form-item :label=" $t('headerInformation.address') ">
          <el-input v-model="Customer.Address" clearable class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterCustomerAddress') " />
        </el-form-item>
        <el-form-item :label=" $t('headerInformation.remarks') ">
          <el-input v-model="Customer.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" :placeholder=" $t('basicInformation.customerRemarks') " class="dialog-input" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">{{ $t('baseBtn.confirm') }}</el-button>
        <el-button v-else type="primary" @click="updateData">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { ouLoadCustomerInfo, getPageRecords, createCustomer, editCustomer, deleteCustomer } from '@/api/Customer'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'Customer', // 客户管理
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
        //    Sort: 'id',
        Name: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: this.$t('basicTips.editCustome'),
        create: this.$t('basicTips.createCustome')
      },
      // 客户实体
      Customer: {
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
        Code: [{ required: true, message: this.$t('basicInformation.pleaseEnterTheCustomerCode'), trigger: 'blur' }],
        Name: [{ required: true, message: this.$t('basicInformation.pleaseEnterACustomerName'), trigger: 'blur' }]
      }
    }
  },
  watch: {

  },
  created() {
    this.getList()
  },
  methods: {
    // 获取数据库中的所有数据
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
      this.resetCustomer()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createCustomer(this.Customer).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('messageTips.messageSucceed'),
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure') ,
                message: this.$t('messageTips.messageFailure')+'：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },

    // 客户编辑
    handleUpdate(row) {
      this.Customer = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const CustomerData = Object.assign({}, this.Customer)
          editCustomer(CustomerData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Customer.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Customer)
                  break
                }
              }
              this.dialogFormVisible = false
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('presentationOfWarehousingDocuments.updateSuccessful'),
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message:this.$t('messageTips.messageFailure')  +'：' + resData.Message,
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
      this.$message.warning(this.$t('messageTips.ImportJudge'))
    },
    // 上传文件
    uploadFile(item) {
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      ouLoadCustomerInfo(form).then(res => {
        var resData = typeof res.data === 'string' ? JSON.parse(res.data) : res.data
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message: this.$t('messageTips.ImportSucceed'),
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: this.$t('messageTips.Failure') ,
            message: this.$t('messageTips.ImportFailure')+':' + resData.Message,
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
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Customer/ExportClientData?Code=' + this.listQuery.Code
      window.open(url)
    },

    // 全部导出
    All_ExportExcel() {
      this.$confirm( this.$t('deviceTips.thisOperationWillExportAllDataIncluding') + this.total + this.$t('deviceTips.articleContinue'),  this.$t('messageTips.Tips'), {
        confirmButtonText: this.$t('baseBtn.confirm') ,
        cancelButtonText: this.$t('baseBtn.cancel') ,
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/Customer/ExportClientData'
        window.open(url)
        console.log(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.messageCancel')
        })
      })
    },

    // 模板下载
    handleDownUpload() {
      // var url = window.PLATFROM_CONFIG.baseUrl + '/api/Customer/DoDownLoadTemp'
      var url = window.PLATFROM_CONFIG.baseUrl + '/Assets/themes/Excel/客户导入模板.xlsx'
      window.open(url)
    },
    handleDelete(row) {
      this.$confirm(this.$t('basicTips.permanentlyDeleteTheCustomer'), this.$t('messageTips.Tips') , {
        confirmButtonText: this.$t('baseBtn.confirm') ,
        cancelButtonText: this.$t('baseBtn.cancel') ,
        type: 'warning'
      }).then(() => {
        this.Customer = Object.assign({}, row) // copy obj
        this.deleteData(this.Customer)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.cancelledDelete') 
        })
      })
      // this.$nextTick(() => {
      //   this.$refs['dataForm'].clearValidate()
      // })
    },
    deleteData(data) {
      deleteCustomer(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message: this.$t('messageTips.deleteSucceed'),
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message: this.$t('messageTips.deleteFailure')+'：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    resetCustomer() {
      this.Customer = {
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

