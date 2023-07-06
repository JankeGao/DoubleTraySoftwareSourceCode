<template>
  <div>
    <el-table
      :key="tableKey"
      v-loading="loading"
      :header-cell-style="{background:'#F2F6FC'}"
      :data="list"
      border
      fit
      highlight-current-row
      style="width: 100%;min-height:100%;"
    >
      <el-table-column width="120px" align="center" label="操作人">
        <template slot-scope="scope">
          <span style="font-weight:bold">{{ scope.row.CreatedUserName }}</span>
        </template>
      </el-table-column>

      <el-table-column width="200px" align="center" label="操作名称">
        <template slot-scope="scope">
          <span>{{ scope.row.Name }}</span>
        </template>
      </el-table-column>

      <el-table-column width="220px" align="center" label="操作时间" prop="CreatedTime" sortable>
        <template slot-scope="scope">
          <span>{{ scope.row.CreatedTime | parseTime('{y}-{m}-{d} {h}:{i}') }}</span>
        </template>
      </el-table-column>

      <el-table-column width="150px" align="center" label="开始时间" prop="BeginTime" sortable>
        <template slot-scope="scope">
          <span>{{ scope.row.BeginTime | parseTime('{y}-{m}-{d} {h}:{i}') }}</span>
        </template>
      </el-table-column>

      <el-table-column width="150px" align="center" label="结束时间" prop="EndTime" sortable>
        <template slot-scope="scope">
          <span>{{ scope.row.EndTime | parseTime('{y}-{m}-{d} {h}:{i}') }}</span>
        </template>
      </el-table-column>

      <el-table-column width="150px" align="center" label="持续时间" prop="TotalMilliseconds" show-overflow-tooltip sortable>
        <template slot-scope="scope">
          <span>{{ scope.row.TotalMilliseconds }}</span>
        </template>
      </el-table-column>

      <el-table-column label="Url" show-overflow-tooltip>
        <template slot-scope="scope">
          <span>{{ scope.row.Url }}</span>
        </template>
      </el-table-column>
    </el-table>
    <!-- 分页 -->
    <div class="pagination-container">
      <el-pagination :current-page="listQuery.Page" :page-sizes="[10,20,30, 50]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
    </div>
  </div>

</template>

<script>
import { fetchList } from '@/api/SysManage/Logs'

export default {
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
  props: {
    type: {
      type: String,
      default: '1'
    }
  },
  data() {
    return {
      list: null,
      tableKey: 1,
      total: null,
      listLoading: true,
      listQuery: {
        Page: 1,
        Rows: 10,
        Name: undefined,
        Sort: 'Id',
        Type: this.type
      },
      loading: false
    }
  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      this.loading = true
      fetchList(this.listQuery).then(response => {
        if (response.status === 200) {
          var resData = JSON.parse(response.data.Content)
          this.list = resData.rows
        } else {
          this.$message({
            title: '失败',
            message: '获取日志信息失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
        this.loading = false
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
    }
  }
}
</script>

