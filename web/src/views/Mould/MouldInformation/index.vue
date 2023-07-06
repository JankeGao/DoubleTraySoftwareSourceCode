<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <!-- 筛选栏 -->
        <div class="filter-container" style="margin-bottom:20px">
          <el-select
            v-model="listQuery.MouldState"
            :multiple="false"
            filterable
            remote
            reserve-keyword
            @change="handleFilter"
          >
            <el-option
              v-for="item in statusList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
          <el-input
            v-model="listQuery.MaterialLabel"
            placeholder="模具名称"
            class="filter-item"
            clearable
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />
          <el-button
            v-waves
            class="filter-button"
            type="primary"
            icon="el-icon-search"
            @click="handleFilter"
          >查询</el-button>
        </div>
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
          <el-table-column :label="'状态'" width="100" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.MouldState===0" type="primary">
                <span>{{ scope.row.StateDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.MouldState===1" type="warning">
                <span>{{ scope.row.StateDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.MouldState===2" type="info">
                <span>{{ scope.row.StateDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.MouldState===3" type="info">
                <span>{{ scope.row.StateDescription }}</span>
              </el-tag>
              <el-tag v-if="scope.row.MouldState===4" type="success">
                <span>{{ scope.row.StateDescription }}</span>
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'货柜'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'层号'" width="80" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.TrayCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'储位'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LocationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'模具编码'" width="130" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'模具名称'" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.MaterialName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上次领用人'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LastTimeReceiveName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上次领用时间'" width="150" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.LastTimeReceiveDatetime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上次领用类别'" width="120" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ReceiveTypeDescription }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上次归还人'" width="100" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LastTimeReturnName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'上次归还时间'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.LastTimeReturnDatetime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'备注'" width="130" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.Remarks }}</span>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            :current-page="listQuery.Page"
            :page-sizes="[15,30,45,60]"
            :page-size="listQuery.Rows"
            :total="total"
            background
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </el-row>

    <!--  添加 ,编辑-->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      :width="'40%'"
      :close-on-click-modal="false"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="MouldInformation"
        class="dialog-form"
        label-width="130px"
        label-position="left"
      >
        <el-form-item :label="'模具编码'" prop="MaterialLabel">
          <el-input
            v-model="MouldInformation.Code"
            clearable
            class="dialog-input"
            placeholder="请输入模具编码"
          />
        </el-form-item>
        <el-form-item :label="'仓库编码'" prop="MaterialLabel">
          <el-input
            v-model="MouldInformation.MaterialLabel"
            :disabled="dialogStatus==='update'"
            clearable
            class="dialog-input"
            placeholder="请输入仓库编码"
          />
        </el-form-item>
        <el-form-item :label="'上次领用人'" prop="LastTimeReceiveName">
          <el-input
            v-model="MouldInformation.LastTimeReceiveName"
            clearable
            class="dialog-input"
            type="text"
            placeholder="请输入领用人"
          />
        </el-form-item>
        <el-form-item :label="'上次归还人'" prop="LastTimeReturnName">
          <el-input
            v-model="MouldInformation.LastTimeReturnName"
            clearable
            class="dialog-input"
            type="text"
            placeholder="请输入归还人"
          />
        </el-form-item>
        <el-form-item :label="'领用时长'" prop="ReceiveTime">
          <el-input
            v-model="MouldInformation.ReceiveTime"
            clearable
            class="dialog-input"
            type="text"
            placeholder="请输入领用时长"
          />
        </el-form-item>
        <el-form-item :label="'状态'" prop="ReceiveTime">
          <el-select
            v-model="MouldInformation.StateDescription"
            :multiple="false"
            filterable
            remote
            reserve-keyword
            clearable
            class="dialog-input"
          >
            <el-option
              v-for="item in statusList"
              :key="item.Code"
              :label="item.Name"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="'上次领用时间'" prop="LastTimeReceiveDatetime">
          <el-date-picker
            v-model="MouldInformation.LastTimeReceiveDatetime"
            type="date"
            class="dialog-input"
            placeholder="选择领用时间"
          />
        </el-form-item>
        <el-form-item :label="'上次归还时间'" prop="LastTimeReturnDatetime">
          <el-date-picker
            v-model="MouldInformation.LastTimeReturnDatetime"
            type="date"
            class="dialog-input"
            placeholder="选择归还时间"
          />
        </el-form-item>
        <el-form-item :label="'备注'">
          <el-input
            v-model="MouldInformation.Remarks"
            :autosize="{ minRows: 2, maxRows: 4}"
            type="textarea"
            placeholder="模具信息备注"
            class="dialog-input"
          />
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
import { getPageRecords, getMouldInformation, postMouldInformation, deleteMouldInformation } from '@/api/Mould/MouldInformation'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'MouldInformation',
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      statusList: [
        {
          Code: undefined, Name: '全部'
        },
        {
          Code: 0, Name: '生产'
        },
        {
          Code: 1, Name: '修模'
        },
        {
          Code: 2, Name: '注销'
        },
        {
          Code: 3, Name: '领用锁定'
        },
        {
          Code: 4, Name: '在库中'
        }
      ],
      dialogFormVisible: false,

      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        MaterialLabel: '', // 模具信息编码
        MouldState: undefined,
        Sort: 'id',
        Code: ''
      },
      TableKey: 0,
      list: null,
      // 模具信息实体
      MouldInformation: {
        ID: undefined,
        Code: '',
        MaterialLabel: '',
        MouldState: '', // 状态
        LastTimeReceiveName: '', // 领用人
        LastTimeReceiveDatetime: '', // 领用时间
        ReceiveTime: '', // 领用时长
        IsDelete: '',
        LastTimeReturnName: '', // 归还人
        LastTimeReturnDatetime: '', // 归还时间
        Remarks: '' // 备注
      },

      // 编辑/创建所用
      textMap: {
        update: '编辑模具信息',
        create: '创建模具信息'
      },
      dialogStatus: '',
      // 输入规则
      rules: {
        MaterialCode: [{ required: true, message: '仓库编码', trigger: 'blur' }],
        MaterialLabel: [{ required: true, message: '模具编码', trigger: 'blur' }],
        LastTimeReceiveName: [{ required: true, message: '请输入领用人', trigger: 'blur' }],
        MaterialType: [{ required: true, message: '请输入领用种类', trigger: 'blur' }],
        ReceiveTime: [{ required: true, message: '请输入领用时长', trigger: 'blur' }],
        LastTimeReturnName: [{ required: true, message: '仓库编码', trigger: 'blur' }]
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
    // 添加
    handleCreate() {
      this.resetMouldInformation()
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 创建
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          getMouldInformation(this.MouldInformation).then((res) => {
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

    // 编辑模具信息
    handleUpdate(row) {
      this.MouldInformation = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    // 编辑
    updateData(row) {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const MouldInformationData = Object.assign({}, this.MouldInformation)
          console.log(MouldInformationData)
          postMouldInformation(MouldInformationData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.MouldInformation.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.MouldInformation)
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
    // 数据筛选
    handleCheck() {
      getMouldInformation().then(response => {
        this.getList()
      })
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
    },
    resetMouldInformation() {
      this.MouldInformation = {
        ID: undefined,
        Code: '',
        MouldState: '', // 状态
        CreatedTime: undefined,
        CreatedUserName: '',
        IsDelete: false,
        AlarmDescribe: '', // 报警描述
        CreatedUserCode: '',
        ContainerCode: '', // 设备编码
        IsDeleted: false,
        UpdatedUserCode: '',
        UpdatedUserName: '',
        UpdatedTime: undefined,
        WarehouseCode: '' // 仓库编码
      }
    },
    handleDelete(row) {
      this.$confirm('此操作将永久删除该载具箱, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.MouldInformation = Object.assign({}, row) // copy obj
        console.log(this.MouldInformation)
        this.deleteData(this.MouldInformation)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    // 删除
    deleteData(data) {
      deleteMouldInformation(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: '成功',
            message: '删除成功',
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: '成功',
            message: '删除失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    }
  }
}
</script>

