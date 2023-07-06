<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="4">
        <el-card>
          <div>
            <span>
              <div style="display: inline-block;"><h4>{{ $t('basicInformation.theDictionaryClass') }}</h4> </div>
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
              :default-expand-all="false"
              highlight-current
              :props="defaultProps"
              @node-click="handleNodeClick"
            />
          </div>
        </el-card>
      </el-col>
      <el-col :span="18" />
      <!-- 筛选栏 -->
      <el-card class="search-card">
        <div class="filter-container">
          <el-input v-model="listQuery.Name" clearable :placeholder=" $t('basicInformation.dictionaryCodeOrName') " class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }}</el-button>
          <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
        </div>
      </el-card>
      <el-card>
        <div>
          <el-table
            :key="0"
            v-loading="false"
            :data="list"
            :header-cell-style="{background:'#F5F7FA'}"
            border
            fit
            highlight-current-row
            style="width:100%;min-height:100%;"
          >
            <el-table-column type="index" width="50" />
            <el-table-column :label=" $t('basicInformation.theDictionaryCoding') " width="150" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Code }}</span>
              </template>
            </el-table-column>
            <el-table-column :label=" $t('basicInformation.nameOfTheDictionary') " width="200" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Name }}</span>
              </template>
            </el-table-column>
            <el-table-column :label=" $t('basicInformation.extensionValue') " align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.Value }}</span>
              </template>
            </el-table-column>
            <el-table-column :label=" $t('basicInformation.theCategoryCode') " width="150" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.TypeCode }}</span>
              </template>
            </el-table-column>
            <el-table-column v-if="Level==0" :label=" $t('basicInformation.toEnableThe') " width="100" align="center">
              <template slot-scope="scope">
                <el-switch
                  v-model="scope.row.Enabled"
                  active-color="#13ce66"
                  inactive-color="#ff4949"
                  disabled
                />
              </template>
            </el-table-column>
            <el-table-column :label=" $t('printBar.founder') " width="150" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.CreatedUserName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label=" $t('printBar.createTime') " width="200" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                <span>{{ scope.row.CreatedTime }}</span>
              </template>
            </el-table-column>
            <el-table-column :label=" $t('headerInformation.operation') " align="center" width="180" class-name="small-padding fixed-width" fixed="right">
              <template slot-scope="scope">
                <el-button size="mini" type="primary" @click="handleUpdate(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
                <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{ $t('baseBtn.delete') }}</el-button>
              </template>
            </el-table-column>
          </el-table>
          <!-- 分页 -->
          <div class="pagination-container">
            <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
          </div>
        </div>
      </el-card>
    </el-row>
    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="Dictionary" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label=" $t('basicInformation.theDictionaryCoding') " prop="Code">
          <el-input v-model="Dictionary.Code" :disabled="dialogStatus=='update'" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterTheDictionaryCode') " />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.nameOfTheDictionary') " prop="Name">
          <el-input v-model="Dictionary.Name" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterADictionaryName') " />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.extensionValue') " prop="Value">
          <el-input v-model="Dictionary.Value" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterADictionaryExtensionValue') " />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.theSorting') " prop="Sort">
          <el-input v-model="Dictionary.Sort" class="dialog-input" />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.theCategoryCode') ">
          <el-input v-model="Dictionary.TypeCode" class="dialog-input" disabled :placeholder=" $t('basicInformation.pleaseSelectADictionaryCategory') " />
        </el-form-item>
        <el-form-item :label=" $t('basicInformation.toEnableThe') ">
          <el-switch
            v-model="Dictionary.Enabled"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </el-form-item>
        <el-form-item :label=" $t('headerInformation.specificDescription') ">
          <el-input v-model="Dictionary.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" :placeholder=" $t('basicInformation.theDictionaryNote') " class="dialog-input" />
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
import { getDictionaryTypeTree, getPageRecords, createDictionary, editDictionary, deleteDictionary } from '@/api/Dictionary'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'Dictionary', // 通用字典
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      defaultProps: {
        label: 'Name'
      //  Id: 'Id'
      },
      treeData: [],
      list: undefined,
      // 分页显示总查询数据
      total: null,
      listLoading: true,
      loading: false,
      materialList: [],
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 10,
        Name: '',
        Code: '',
        Sort: 'Id',
        TypeCode: ''
      },
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: this.$t('basicTips.editDictionary'),
        create: this.$t('basicTips.createDictionary')
      },
      Level: 0,
      // 输入规则
      rules: {
        Code: [{ required: true, message: this.$t('basicTips.pleaseEnterTheCode'), trigger: 'blur' }],
        Name: [{ required: true, message: this.$t('basicTips.pleaseEnterAName'), trigger: 'blur' }],
        Sort: [{ required: true, message: this.$t('basicTips.pleaseEnterSorting'), trigger: 'blur' }],
        Value: [{ required: true, message: this.$t('basicTips.pleaseEnterAnExtendedValue'), trigger: 'blur' }]
      },
      Dictionary: {
        Id: undefined,
        Code: '',
        Name: '',
        Value: '',
        Enabled: true,
        Sort: 1,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        parentCode: '',
        Remark: '',
        TypeCode: ''
      }
    }
  },
  watch: {

  },
  created() {
    this.getTreeData()
    this.handleFilter()
  },
  methods: {
    getList() {
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
    getTreeData() {
      getDictionaryTypeTree().then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.treeData = usersData// this.convertTreeData(usersData)
      })
    },
    // Tree数组转换
    convertTreeData(arr) {
      const treedataList = [] // 返回的路由数组
      var routerData = arr // data中的值为数组
      if (JSON.stringify(routerData) !== '[]') {
        routerData.forEach(item => {
          if (item.ParentCode === null || item.ParentCode === '' || item.ParentCode === 'null') { // 如果不存在上级，则为1级菜单，此部分可根据后端返回的数据重新定义完善
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
    generateRouter(item, isParent, level) {
      var treeData = {
        label: item.Name,
        Code: item.Code,
        Name: item.Name,
        Level: level,
        WareHouseCode: item.WareHouseCode,
        AreaCode: item.AreaCode
      }
      return treeData
    },
    handleCreate() {
      this.resetDictionary()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createDictionary(this.Dictionary).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              // this.list.unshift(this.Role)
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('messageTips.messageSucceed') ,
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('messageTips.messageFailure') +'：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 编辑字典
    handleUpdate(row) {
      this.Dictionary = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const data = Object.assign({}, this.Dictionary)
          editDictionary(data).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('presentationOfWarehousingDocuments.updateSuccessful'),
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Succeed') ,
                message: this.$t('messageTips.messageFailure') +'：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    handleDelete(row) {
      this.$confirm(this.$t('basicTips.thisOperationWillPermanentlyDeleteThisRecord'), this.$t('messageTips.Tips'), {
        confirmButtonText: this.$t('baseBtn.confirm'),
        cancelButtonText: this.$t('baseBtn.cancel'),
        type: 'warning'
      }).then(() => {
        this.Dictionary = Object.assign({}, row) // copy obj
        this.deleteData(this.Dictionary)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.cancelledDelete')
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    deleteData(data) {
      deleteDictionary(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message: this.$t('messageTips.deleteSucceed') ,
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: this.$t('messageTips.Succeed') ,
            message:  this.$t('messageTips.deleteFailure') +'：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    handleNodeClick(data) {
      this.listQuery.TypeCode = data.Code

      this.Dictionary.TypeCode = data.Code
      this.getList()
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
    resetDictionary() {
      this.Dictionary = {
        Id: undefined,
        Code: '',
        Name: '',
        Value: '',
        Enabled: true,
        Sort: 1,
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        parentCode: '',
        Remark: '',
        TypeCode: this.Dictionary.TypeCode
      }
    }
  }
}
</script>

