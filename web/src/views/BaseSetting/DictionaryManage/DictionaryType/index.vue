<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Name" :placeholder=" $t('basicInformation.dictionaryCategoryCodeOrName') " class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }}</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
      </div>
    </el-card>
    <el-card>
      <tree-table
        :key="key"
        :data="tableData"
        :columns="columns"
        :default-expand-all="true"
        :header-cell-style="{background:'#F5F7FA'}"
        border
        row-key="Id"
        highlight-current-row
        @row-click="rowSelect"
      >

        <template slot="Enabled" slot-scope="{scope}">
          <el-switch
            v-model="scope.row.Enabled"
            active-color="#13ce66"
            inactive-color="#ff4949"
            disabled
          />
        </template>
        <template slot="operation" slot-scope="{scope}">
          <el-button v-if="scope.row.ParentCode===''||scope.row.ParentCode===null" type="primary" size="mini" @click="handleCreate(scope.row.Code)">{{ $t('baseBtn.addBtn') }}</el-button>
          <el-button size="mini" @click="handleUpdate(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
          <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{ $t('baseBtn.delete') }}</el-button>
        </template>
      </tree-table>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="DictionaryType" class="dialog-form" label-width="100px" label-position="left">
                <el-form-item :label=" $t('basicInformation.theCategoryCode') ">
                    <el-input v-model="DictionaryType.Code" :disabled="dialogStatus==='update'" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterTheCategoryCode') " />
                </el-form-item>
                <el-form-item :label=" $t('basicInformation.categoryName') ">
                    <el-input v-model="DictionaryType.Name" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterACategoryName') " />
                </el-form-item>
                <el-form-item :label=" $t('basicInformation.theSuperiorCoding') ">
                    <el-input v-model="DictionaryType.ParentCode" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterTheSuperiorCode') " />
                </el-form-item>
                <el-form-item :label=" $t('basicInformation.theSorting') ">
                    <el-input v-model="DictionaryType.Sort" class="dialog-input" />
                </el-form-item>
                <el-form-item :label=" $t('basicInformation.toEnableThe') ">
                    <el-switch v-model="DictionaryType.Enabled" active-color="#13ce66" inactive-color="#ff4949" />
                </el-form-item>
                <el-form-item :label=" $t('headerInformation.remarks') ">
                    <el-input v-model="DictionaryType.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" :placeholder=" $t('basicInformation.theDictionaryNote') " class="dialog-input" />
                </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button v-if="dialogStatus=='create'" type="primary" @click="createData">{{ $t('baseBtn.confirm') }}</el-button>
        <el-button v-else type="primary" @click="updateData(module)">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { GetDictionaryList, createDictionaryType, editDictionaryType, deleteDictionaryType, getDictionaryTree } from '@/api/Dictionary'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import treeTable from '@/components/TreeTable'
// import { cleanParams } from '@/utils'
// import data from './data'

export default {
  name: 'DictionaryType', // 字典类别
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
      currentRow: undefined,
      // 模块实体
      DictionaryType: {
        Code: '',
        Name: '',
        ParentCode: '',
        Sort: 1,
        Enabled: true,
        CreateUserCode: '',
        CreateUserName: '',
        CreatedTime: undefined
      },
      selectType: 0,
      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: this.$t('basicTips.editDictionaryCategory'),
        create: this.$t('basicTips.createDictionaryCategory')
      },

      // 输入规则
      rules: {
        Code: [{ required: true, message: 'type is required', trigger: 'blur' }],
        // timestamp: [{ type: 'date', required: true, message: 'timestamp is required', trigger: 'change' }],
        Name: [{ required: true, message: this.$t('basicTips.pleaseEnterTheArticleName'), trigger: 'blur' }]
      },
      imagecropperShow: false,
      imagecropperKey: 0,
      showCheckbox: true,
      key: 1,
      columns: [
        // {
        //   label: '',
        //   width: 120,
        //   checkbox: true
        // },
        {
          label: this.$t('basicInformation.coding'),
          width: 200,
          key: 'Code',
          align: 'left'
        },
        {
          label: this.$t('headerInformation.name') ,
          key: 'Name',
          width: 200
        },
        {
          label: this.$t('basicInformation.theSuperiorCoding'),
          key: 'ParentCode',
          width: 200
        },
        {
          label: this.$t('systemManagement.theSorting'),
          width: 100,
          key: 'Sort'
        },
        {
          label: this.$t('basicInformation.toEnableThe'),
          width: 100,
          key: 'Enabled'
        },
        {
          label: this.$t('headerInformation.remarks'),
          // width: 100,
          key: 'Remark'
        },
        {
          label: this.$t('headerInformation.operation'),
          width: 280,
          key: 'operation'
        }
      ],
      tableData: '',
      ParentCodes: []
    }
  },
  watch: {
    dialogFormVisible(value) {
      if (!value) {
        this.resetDictionaryType()
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      this.listLoading = true
      GetDictionaryList(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.tableData = usersData.rows
        this.total = usersData.total
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    getTreeData() {
      getDictionaryTree().then(response => {
        var usersData = JSON.parse(response.data.Content)

        this.treeData = this.convertTreeData(usersData)
      })
    },
    // Tree数组转换
    convertTreeData(arr) {
      const treedataList = [] // 返回的路由数组
      var routerData = arr // data中的值为数组
      var entity = {
        Code: '',
        Name: this.$t('basicTips.rootDirectory')
      }
      var first = this.generateRouter(entity, true, 0)
      var firstchildren = []
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
            // treedataList.push(parent)
            firstchildren.push(parent)
          }
        })
      }
      first.children = firstchildren
      treedataList.push(first)
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
      this.DictionaryType.ParentCode = data
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createDictionaryType(this.DictionaryType).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed'),
                message: this.$t('messageTips.messageSucceed'),
                type: 'success',
                duration: 2000
              })
              this.resetDictionaryType()
              // window.location.reload()
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('messageTips.messageFailure') +"：" + resData.Message,
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
      this.DictionaryType = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      editDictionaryType(this.DictionaryType).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('messageTips.editSucceed'),
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('messageTips.editFailure')+"：" + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 文章删除
    handleDelete(row) {
      this.$confirm(this.$t('basicTips.permanentlyDeleteTheDictionaryCategory'), this.$t('messageTips.Tips') , {
        confirmButtonText: this.$t('baseBtn.confirm') ,
        cancelButtonText: this.$t('baseBtn.cancel') ,
        type: 'warning'
      }).then(() => {
        this.DictionaryType = Object.assign({}, row) // copy obj
        this.deleteData(this.DictionaryType)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.cancelledDelete'),
        })
      })
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    deleteData(data) {
      deleteDictionaryType(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('messageTips.deleteSucceed'),
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('messageTips.deleteFailure')+"：" + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },

    handelSelect(value) {

    },

    // 头像控件
    cropSuccess(resData) {
      this.imagecropperShow = false
      this.imagecropperKey = this.imagecropperKey + 1
      this.Artical.Picture = resData
    },
    close() {
      this.imagecropperShow = false
    },
    // 重置
    resetDictionaryType() {
      this.DictionaryType = {
        Code: '',
        Name: '',
        ParentCode: '',
        Sort: 1,
        Enabled: true,
        CreateUserCode: '',
        CreateUserName: '',
        CreatedTime: undefined
      }
    },
    rowSelect(row, column, event) {
      this.currentRow = row
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
</style>
