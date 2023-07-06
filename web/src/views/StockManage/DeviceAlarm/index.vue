<template>
  <div class="app-container">
    <el-row>
      <el-card>
        <!-- 筛选栏 -->
        <div class="filter-container" style="margin-bottom:20px">
          <el-select
            v-model="listQuery.Status"
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
          <el-input v-model="listQuery.ContainerCode" placeholder="设备信息" class="filter-item" clearable @keyup.enter.native="handleFilter" @clear="handleFilter" />
          <el-date-picker
            v-model="Value"
            type="daterange"
            range-separator="至 "
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            clearable
            format="yyyy 年 MM 月 dd 日"
            value-format="yyyy-MM-dd HH:mm:ss"
            @blur="handleFilter"
          />
          <el-button v-waves class="filter-button" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button class="filter-button" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
          <el-dropdown
            size="small"
            placement="bottom"
            trigger="click"
            @command="batchOperate"
          >
            <el-button class="filter-button" style="margin-left: 10px;" type="primary">
              导出
              <i class="el-icon-arrow-down el-icon--right" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item command="All_Export">全部导出</el-dropdown-item>
              <el-dropdown-item command="Condition_Export">按条件导出</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
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
          <el-table-column :label="'状态'" width="120" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.State===0" type="warning"><span>{{ scope.row.StateDescription }}</span></el-tag>
              <el-tag v-if="scope.row.State===1" type="danger"><span>{{ scope.row.StateDescription }}</span></el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="'设备'" width="150" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContainerCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'报警区域'" width="160" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.WarehouseName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'报警描述'" width="120" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.AlarmDescribeDescription }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'发生时间'" width="210" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.CreatedTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'结束时间'" width="210" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.UpdatedTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'延续时间'" width="160" align="center" show-overflow-tooltip>
            <template slot-scope="scope">
              <span>{{ scope.row.ContinueTime }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'操作'" show-overflow-tooltip align="center">
            <template slot-scope="scope">
              <el-button v-if="scope.row.State===1" v-waves disabled class="filter-button" type="danger">复位</el-button>
              <el-button v-if="scope.row.State===0" v-waves class="filter-button" type="danger" @click="UpdatDivice(scope.row)">复位</el-button>
            </template>
          </el-table-column>
        </el-table>
        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination :current-page="listQuery.Page" :page-sizes="[15,30,45,60]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
        </div>
      </el-card>
    </el-row>

    <!--  添加 -->
    <el-dialog v-el-drag-dialog :visible.sync="dialogFormVisible" :width="'40%'" :close-on-click-modal="false">
      <el-form ref="dataForm" :rules="rules" :model="DeviceAlarm" class="dialog-form" label-width="100px" label-position="left">
        <el-form-item :label="'设备报警编码'" prop="Code">
          <el-input v-model="DeviceAlarm.Code" clearable class="dialog-input" placeholder="请输入设备报警编码" />
        </el-form-item>
        <el-form-item :label="'设备编码'" prop="ContainerCode">
          <el-input v-model="DeviceAlarm.ContainerCode" clearable class="dialog-input" placeholder="请输入设备编码名称" />
        </el-form-item>
        <el-form-item :label="'状态'" prop="State">
          <el-input v-model="DeviceAlarm.State" clearable class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g,'')" placeholder="请输入设备状态" />
        </el-form-item>
        <el-form-item :label="'报警描述'" prop="AlarmDescribe">
          <el-input v-model="DeviceAlarm.AlarmDescribe" clearable class="dialog-input" type="text" onkeyup="value=value.replace(/[^\d]/g,'')" placeholder="请输入报警描述" />
        </el-form-item>
        <el-form-item :label="'仓库编码'" prop="WarehouseCode">
          <el-input v-model="DeviceAlarm.WarehouseCode" clearable class="dialog-input" placeholder="请输入仓库编码" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="createData">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { getPageRecords, getAlarmCheck, postDeviceAlarm } from '@/api/DeviceAlarm'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui
export default {
  name: 'Alarm', // 库存预警
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      statusList: [
        {
          Code: undefined, Name: '全部状态'
        },
        {
          Code: 0, Name: '进行中'
        },
        {
          Code: 1, Name: '已复位'
        }
      ],
      dialogFormVisible: false,
      Value: [],

      // 分页显示总查询数据
      total: null,
      listLoading: false,
      // 复位
      restoration: true,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        ContainerCode: '',
        Status: '',
        Sort: 'id',
        MaterialName: '',
        Code: '',
        begin: '',
        end: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      // 设备报警实体
      DeviceAlarm: {
        ID: undefined,
        Code: '',
        State: '', // 状态
        CreatedTime: '',
        CreatedUserName: '',
        IsDelete: '',
        AlarmDescribe: '', // 报警描述
        UpdatedTime: undefined
      },
      // 输入规则
      rules: {
        Code: [{ required: true, message: '设备报警编码', trigger: 'blur' }],
        ContainerCode: [{ required: true, message: '设备编码', trigger: 'blur' }],
        State: [{ required: true, message: '状态', trigger: 'blur' }],
        AlarmDescribe: [{ required: true, message: '报警描述', trigger: 'blur' }],
        WarehouseCode: [{ required: true, message: '仓库编码', trigger: 'blur' }]

      }
    }
  },
  watch: {
    // 授权面板关闭，清空原角色查询权限
    // dialogTreeVisible(value) {
    //   if (!value) {
    //     this.resetModuleAuthData()
    //   }
    // }
  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      this.listLoading = true

      if (this.Value.length > 0) {
        this.listQuery.begin = this.Value[0]
        this.listQuery.end = this.Value[1]
      }
      getPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        // console.log(usersData)
        this.list = usersData.rows
        this.total = usersData.total
        // Just to simulate the time of the request
        setTimeout(() => {
          this.listLoading = false
        }, 1 * 1000)
      })
    },
    handleNodeClick(data) {
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
    // 添加
    handleCreate() {
      this.resetDeviceAlarm()
      // this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 创建
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          getAlarmCheck(this.DeviceAlarm).then((res) => {
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

    // 复位
    UpdatDivice(row) {
      row.State = 1
      const DeviceAlarmData = Object.assign({}, row)
      // console.log(DeviceAlarmData)
      postDeviceAlarm(DeviceAlarmData).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.$message({
            title: '成功',
            message: '复位成功',
            type: 'success',
            duration: 2000
          })
          this.getList()
        } else {
          this.$message({
            title: '失败',
            message: '复位失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },

    // 数据筛选
    handleCheck() {
      getAlarmCheck().then(response => {
        this.getList()
      })
    },
    timer() {
      return setInterval(() => {
        this.getList()
      }, 10000)
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
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/DeviceAlarm/DoDownLoadTemp?ContainerCode=' + this.listQuery.ContainerCode + '&Status=' + this.listQuery.Status + '&begin=' + this.listQuery.begin + '&end=' + this.listQuery.end
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
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/DeviceAlarm/DoDownLoadTemp'
        window.open(url)
        console.log(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消'
        })
      })
    },

    resetDeviceAlarm() {
      this.DeviceAlarm = {
        ID: undefined,
        Code: '',
        State: '', // 状态
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
    }
  }
}
</script>

