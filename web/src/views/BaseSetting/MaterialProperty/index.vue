<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Code" :placeholder=" $t('basicInformation.nameOfTheMaterialPropertyGroup') " class="filter-item" clearable @clear="handleFilter" @keyup.enter.native="handleFilter" />
        <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">{{ $t('baseBtn.queryBtn') }}</el-button>
        <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">{{ $t('baseBtn.addBtn') }}</el-button>
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
        <el-table-column :label="$t('basicInformation.attributeGroupName')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.largestInventory')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MaxNum }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.theMinimumInventory')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.MinNum }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.firstInFirstOut')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.FIFOTypeCaption }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.precision')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.FIFOAccuracyCaption }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.batchesOfMaterial')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.IsBatch "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.storeLocked')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.IsNeedBlock "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.singlePackageBarCode')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.IsPackage "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.whetherTheMixedBatchOf')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.IsMaxBatch "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.agingTime')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.ValidityPeriod }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.agingTime')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.AgeingPeriod }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.costCenter')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.CostCenter }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.toEnableThe')" width="100px" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.Enabled "
              disabled
            />
          </template>
        </el-table-column>
        <el-table-column :label="$t('headerInformation.specificDescription')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('basicInformation.toEnableThe')" align="center" width="230" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button size="mini" @click="handleUpdate(scope.row)">{{ $t('baseBtn.editBtn') }}</el-button>
            <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{ $t('baseBtn.delete') }}</el-button>
          </template>
        </el-table-column>

      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>

    <!-- 创建/编辑 弹出框 -->
    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" :width="'70%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="MaterialProperty" class="dialog-form" label-width="100px" label-position="left">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item :label="$t('basicInformation.attributeGroupName')" prop="Name">
              <el-input v-model="MaterialProperty.Name" class="dialog-input" :placeholder=" $t('basicInformation.pleaseEnterTheMaterialPropertyGroupName') " />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.largestInventory')">
              <el-input v-model="MaterialProperty.MaxNum" :min="MaterialProperty.MinNum" class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g.,'')" :placeholder=" $t('basicInformation.pleaseInputTheMaximumStockOfMaterials') " />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.theMinimumInventory')">
              <el-input v-model="MaterialProperty.MinNum" :max="MaterialProperty.MaxNum" class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g,'')" :placeholder=" $t('basicInformation.pleaseEnterTheMinimumInventoryOfMaterials') " />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.batchesOfMaterial')">
              <el-switch
                v-model="MaterialProperty.IsBatch"
                active-color="#13ce66"
                inactive-color="#ff4949"
              />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.storeLocked')">
              <el-switch
                v-model="MaterialProperty.IsNeedBlock"
                active-color="#13ce66"
                inactive-color="#ff4949"
              />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.whetherTheMixedBatchOf')">
              <el-switch
                v-model="MaterialProperty.IsMaxBatch"
                active-color="#13ce66"
                inactive-color="#ff4949"
              />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.singlePackageBarCode')">
              <el-switch
                v-model="MaterialProperty.IsPackage"
                active-color="#13ce66"
                inactive-color="#ff4949"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('basicInformation.firstInFirstOut')">
              <el-select v-model="MaterialProperty.FIFOType" size="small" class="dialog-input" :placeholder="$t('basicInformation.pleaseSelectFifO')">
                <el-option
                  v-for="item in options"
                  :key="item.value"
                  :label="item.label"
                  :value="parseInt(item.value)"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="$t('basicInformation.precision')" prop="IntervalType">
              <el-select v-model="MaterialProperty.FIFOAccuracy" placeholder="请选择先进先出精度" class="dialog-input" :disabled="MaterialProperty.FIFOType===0">
                <el-option
                  v-for="item in timeoptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="$t('basicInformation.expiryDateDays')">
              <el-input v-model="MaterialProperty.ValidityPeriod" class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g,'')" :placeholder=" $t('basicInformation.pleaseEnterTheValidityPeriod') " />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.agingTimeDays')">
              <el-input v-model="MaterialProperty.AgeingPeriod" class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g,'')" :placeholder="$t('basicInformation.pleaseEnterAgingTime')" /></el-form-item>
            <el-form-item :label="$t('basicInformation.costCenter')">
              <el-input v-model="MaterialProperty.CostCenter" class="dialog-input" :placeholder="$t('basicInformation.pleaseEnterMaterialCostCenter')" />
            </el-form-item>
            <el-form-item :label="$t('basicInformation.toEnableThe')">
              <el-switch
                v-model="MaterialProperty.Enabled"
                active-color="#13ce66"
                inactive-color="#ff4949"
              />
            </el-form-item>
            <el-form-item :label="$t('headerInformation.specificDescription')">
              <el-input v-model="MaterialProperty.Remark" :autosize="{ minRows: 2, maxRows: 4}" type="textarea" :placeholder=" $t('basicInformation.attributeGroup') " class="dialog-input" />
            </el-form-item>
          </el-col>
        </el-row>
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
import { getPageRecords, createMaterialProperty, editMaterialProperty, deleteMaterialProperty } from '@/api/MaterialProperty'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
// import { cleanParams } from '@/utils'

export default {
  name: 'MaterialProperty', // 职位管理
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

      // 物料属性组实体
      MaterialProperty: {
        Id: undefined,
        Code: '',
        Name: '',
        Remark: '',
        Enabled: true
      },

      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑物料属性组',
        create: '创建物料属性组'
      },

      // 输入规则
      rules: {
        type: [{ required: true, message: 'type is required', trigger: 'change' }],
        timestamp: [{ type: 'date', required: true, message: 'timestamp is required', trigger: 'change' }],
        Name: [{ required: true, message: '请输入物料属性组名称', trigger: 'blur' }]
      },
      // 先进先出枚举
      options: [{
        value: '0',
        label: '无'
      }, {
        value: '1',
        label: '入库时间'
      }, {
        value: '2',
        label: '生产日期'
      }, {
        value: '3',
        label: '库存保质期'
      }],
      // 时间间隔-后端枚举
      timeoptions: [
        {
          value: '0',
          label: '无'
        }, {
          value: '1',
          label: '秒'
        }, {
          value: '2',
          label: '分钟'
        }, {
          value: '3',
          label: '小时'
        }, {
          value: '4',
          label: '天'
        }]
    }
  },
  watch: {
    // 授权面板关闭，清空原物料属性组查询权限
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
    // 获取列表数据
    getList() {
      this.listLoading = true
      getPageRecords(this.listQuery).then(response => {
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
    // 物料属性组创建
    handleCreate() {
      this.resetMaterialProperty(this.MaterialProperty)
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createMaterialProperty(this.MaterialProperty).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.list.unshift(this.MaterialProperty)
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
    // 物料属性组编辑
    handleUpdate(row) {
      this.MaterialProperty = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const MaterialPropertyData = Object.assign({}, this.MaterialProperty)
          editMaterialProperty(MaterialPropertyData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.MaterialProperty.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.MaterialProperty)
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
    // 物料属性组删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该物料属性组, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.MaterialProperty = Object.assign({}, row) // copy obj
        this.deleteData(this.MaterialProperty)
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
      deleteMaterialProperty(data).then((res) => {
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

    // 重置
    resetMaterialProperty() {
      this.MaterialProperty = {
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
