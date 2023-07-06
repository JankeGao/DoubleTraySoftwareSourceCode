<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input v-model="listQuery.Name" placeholder="定时任务名称" class="filter-item" clearable @clear="handleFilter" @keyup.enter.native="handleFilter" />
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
        <el-table-column :label="'任务Id'" align="center" width="80px">
          <template slot-scope="scope">
            <span>{{ scope.row.Id }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'任务名称'" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.JobName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'任务对象'" align="center" width="180px" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.JobObject }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'任务状态'" align="center" width="80px">
          <template slot-scope="scope">
            <el-tag v-if="scope.row.Status===1" type="info">{{ scope.row.StatusCaption }}</el-tag>
            <el-tag v-if="scope.row.Status===2" type="success">{{ scope.row.StatusCaption }}</el-tag>
            <el-tag v-if="scope.row.Status===3" type="warning">{{ scope.row.StatusCaption }}</el-tag>
            <el-tag v-if="scope.row.Status===4" type="danger">{{ scope.row.StatusCaption }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="'开始时间'" align="center" width="200px">
          <template slot-scope="scope">
            <span>{{ scope.row.StartTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'重复次数'" align="center" width="80px">
          <template slot-scope="scope">
            <span>{{ scope.row.RepeatCount }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'下次执行时间'" align="center" width="200px">
          <template slot-scope="scope">
            <span>{{ scope.row.NextExecuteTime }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="'创建信息'" width="200px" align="center">
          <template slot-scope="scope">
            <div>{{ scope.row.CreatedUserName }}</div>
            <div>{{ scope.row.CreatedTime }}</div>
          </template>
        </el-table-column>
        <el-table-column :label="'操作'" align="center" width="260" class-name="small-padding fixed-width" fixed="right">
          <template slot-scope="scope">
            <el-button icon="el-icon-edit" circle @click="handleUpdate(scope.row)" />
            <el-button type="info" icon="el-icon-delete" circle @click="handleDelete(scope.row)" />
            <el-button size="mini" type="primary" @click="handleStart(scope.row)">启动</el-button>
            <el-button size="mini" type="danger" @click="handleStop(scope.row)">停止</el-button>
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
      <el-form ref="dataForm" :rules="rules" :model="JobScheduler" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'任务名称'" prop="JobName">
          <el-input v-model="JobScheduler.JobName" class="dialog-input" placeholder="请输入定时任务名称" />
        </el-form-item>
        <el-form-item :label="'任务对象'" prop="JobObject">
          <el-select v-model="JobScheduler.JobObject" placeholder="请选择任务对象" class="dialog-input">
            <el-option
              v-for="item in objectList"
              :key="item.Code"
              :label="item.Code"
              :value="item.Code"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="'开始时间'" prop="StartTime">
          <el-date-picker
            v-model="JobScheduler.StartTime"
            class="dialog-input"
            type="datetime"
            placeholder="选择日期时间"
            value-format="yyyy-MM-dd HH:mm:ss"
            @change="handleChange(JobScheduler.StartTime)"
          />
        </el-form-item>
        <el-form-item
          :label="'间隔时间'"
          prop="IntervalTime"
          :rules="[
            { required: true, message: '间隔时间不能为空'},
            { type: 'number', message: '间隔时间必须为数字值'}
          ]"
        >
          <el-input v-model.number="JobScheduler.IntervalTime" class="dialog-input" placeholder="请输入任务启动的间隔时间" />
        </el-form-item>
        <el-form-item :label="'间隔类型'" prop="IntervalType">
          <el-select v-model="JobScheduler.IntervalType" placeholder="请选择间隔类型" class="dialog-input">
            <el-option
              v-for="item in options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          :label="'重复次数'"
          prop="RepeatCount"
          :rules="[
            { required: true, message: '重复次数不能为空'},
            { type: 'number', message: '重复次数必须为数字值'}
          ]"
        >
          <el-input v-model.number="JobScheduler.RepeatCount" placeholder="请输入定时任务的重复次数，0 表示无限次" class="dialog-input" />
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
import { fetchList, createJobScheduler, editJobScheduler, deleteJobScheduler, startJobScheduler, stopJobScheduler } from '@/api/SysManage/Job'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
import { getJobObjects } from '@/api/SysManage/JobObject'

export default {
  name: 'Job', // 定时任务
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

      // 定时任务实体
      JobScheduler: {
        Id: undefined,
        JobName: '',
        JobObject: '',
        StartTime: undefined,
        IntervalTime: undefined,
        IntervalType: undefined,
        RepeatCount: 1
      },

      // 创建弹出框
      dialogFormVisible: false,
      dialogStatus: '',
      textMap: {
        update: '编辑定时任务',
        create: '创建定时任务'
      },

      // 输入规则
      rules: {
        StartTime: [{ required: true, message: '请输入开始时间', trigger: 'change' }],
        JobName: [{ required: true, message: '请输入定时名称', trigger: 'blur' }],
        JobObject: [{ required: true, message: '请选择定时任务对象', trigger: 'blur' }],
        IntervalType: [{ required: true, message: '请选择时间类型', trigger: 'blur' }]
      },
      // 任务对象
      objectList: [],
      // 时间间隔-后端枚举
      options: [{
        value: 'hours',
        label: '小时'
      }, {
        value: 'minutes',
        label: '分钟'
      }, {
        value: 'seconds',
        label: '秒'
      }]
    }
  },
  watch: {
    // 授权面板关闭，清空原定时任务查询权限
    dialogFormVisible(value) {
      if (!value) {
        this.resetJobScheduler()
      }
    }
  },
  created() {
    this.getList()
    this.getJobObjects()
  },
  methods: {
    handleChange(data) {
      console.log(data)
    },
    // 获取列表数据
    getJobObjects() {
      getJobObjects(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.objectList = usersData
      })
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
    // 定时任务创建
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
          console.log(this.JobScheduler)
          createJobScheduler(this.JobScheduler).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.list.unshift(this.JobScheduler)
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
    // 定时任务编辑
    handleUpdate(row) {
      this.JobScheduler = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const JobSchedulerData = Object.assign({}, this.JobScheduler)
          editJobScheduler(JobSchedulerData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.JobScheduler.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.JobScheduler)
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
    // 定时任务删除
    handleDelete(row) {
      this.$confirm('此操作将永久删除该定时任务, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.deleteData(row)
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
      deleteJobScheduler(data).then((res) => {
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
    // 定时任务启动
    handleStart(row) {
      startJobScheduler(row).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '启动成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '启动失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 定时任务停止
    handleStop(row) {
      stopJobScheduler(row).then(res => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.$message({
            title: '成功',
            message: '停止成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '成功',
            message: '停止失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    // 重置
    resetJobScheduler() {
      this.JobScheduler = {
        Id: undefined,
        JobName: '',
        JobObject: '',
        StartTime: undefined,
        IntervalTime: undefined,
        IntervalType: undefined,
        RepeatCount: 1
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
