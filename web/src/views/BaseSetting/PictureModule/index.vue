
<template>
  <div class="app-container">
    <!-- 筛选栏 -->

    <!-- card list -->
    <el-card>
      <el-row :gutter="20">
        <el-col v-for="file in list" :key="file.Id" :span="3" style="margin-bottom: 20px;">
          <el-card :body-style="{ padding: '0px' }" class="image-card">
            <img :src="BaseUrl+file.FilePath" class="image">
            <div style="padding: 10px;">
              <div>
                <span style="float:right">
                  <el-button type="text" @click="handleDelete(file)"><i class="el-icon-delete" /></el-button>
                </span>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination :current-page="listQuery.Page" :page-sizes="[16,32,40]" :page-size="listQuery.Rows" :total="total" background layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handleCurrentChange" />
      </div>
    </el-card>

  </div>
</template>

<script>
import { PageRecordsPicture, deleteOneselfPicture } from '@/api/FileLibrary'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'FileInfo',
  // components: { Dropzone },
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {

      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址

      list: null,

      // 分页显示总查询数据
      total: null,
      listLoading: true,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 16,
        FileName: undefined,
        type: undefined,
        Sort: 'id'
      },
      // 图片实体
      FileInfo: {
        Id: undefined,
        Code: '',
        FileName: '',
        ExtensionName: '',
        ContentType: '',
        Size: '',
        FilePath: '',
        LabelId: ''
      },

      uploadFilename: null,
      uploadFiles: [],
      dialogVisible: false,

      uploadFileLibrary: 'http://127.0.0.1:30025/api/FileLibrary/PostDoFileLibraryUpload'
    }
  },
  created() {
    this.initData()
  },
  methods: {
    // 初始化
    initData() {
      this.getList()
    },
    // 获取列表数据
    getList() {
      this.listLoading = true
      PageRecordsPicture(this.listQuery).then(response => {
        var FileInfosData = JSON.parse(response.data.Content)
        this.list = FileInfosData.rows
        this.total = FileInfosData.total

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

    uploadSuccess(data) {
      var resData = JSON.parse(data.Content)
      if (resData.Success) {
        this.$message({
          title: '成功',
          message: '文件上传成功',
          type: 'success',
          duration: 2000
        })
        this.getList()
      } else {
        this.$message({
          title: '成功',
          message: '上传失败：' + resData.Message,
          type: 'error',
          duration: 2000
        })
      }
    },
    // 图片删除
    handleDelete(file) {
      this.$confirm('此操作将永久删除该图片, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.deleteData(file)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        })
      })
    },
    deleteData(file) {
      deleteOneselfPicture(file).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
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
    resetFileInfo() {
      this.FileInfo = {
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
// small card
.image-card{
  widows: 95%;
  height: 180px;
  .image{
    width:100%;
    height: 140px;
    display: block;
  }
  .bottom {
    line-height: 12px;
  }
  .title{
    // font-weight:bold;
    font-size: 15px;
    color: #303133;
  }
  .label{
    font-size: 12px;
    color: #909399;
  }
  .link{
    font-size: 12px;
  }
}

.item-iconcard:hover{
  width:250px;
  height: 100px;
  text-align:center;
  background: #F2F6FC;
}
.el-card__body {
    padding-top: 0px;
    padding-bottom: 0px;
    padding-right: 0px;
    padding-left: 0px
}
</style>

