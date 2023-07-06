<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input
          v-model="listQuery.Code"
          :placeholder="$t('box.queryBack')"
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
        >{{ $t('baseBtn.queryBtn') }}</el-button>
        <el-button
          class="filter-button"
          style="margin-left: 10px;"
          type="primary"
          icon="el-icon-edit"
          @click="handleCreateBox"
        >{{ $t('baseBtn.addBtn') }}</el-button>
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

        <el-button
          v-waves
          class="filter-button"
          type="primary"
          @click="handleDownUpload"
        >{{ $t('baseBtn.templateBtn') }}</el-button>

        <el-upload
          ref="fileupload"
          style="display: inline; margin-left: 10px;margin-right: 10px;"
          action="#"
          :show-file-list="false"
          :http-request="uploadFile"
          :on-exceed="handleExceed"
        >
          <el-button
            class="filter-button"
            type="primary"
            icon="el-icon-download"
            @click="handleDownUploadBox()"
          >{{ $t('baseBtn.exportBtn') }}</el-button>
        </el-upload>
      </div>
    </el-card>
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
        <el-table-column type="index" width="40" />
        <el-table-column :label="$t('box.Code')" width="130" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('box.Picture')"
          width="120"
          align="center"
          show-overflow-tooltip
        >
          <!-- BaseUrl+scope.row.PictureUrl -->
          <template slot-scope="scope">
            <div>
              <span>
                <div class="block">
                  <img
                    :src="BaseUrl+scope.row.PictureUrl "
                    style="height:50%;width:50%; display: block;"
                    fit="fit"
                  >
                </div>
              </span>
            </div>
          </template>
        </el-table-column>
        <el-table-column :label="$t('box.Colour')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.BoxColour }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('box.Name')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('box.Type')" width="100" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Type }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('box.width')+'(mm)'"
          width="90"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.BoxLength }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('box.length')+'(mm)'"
          width="90"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.BoxWidth }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('box.IsVirtual')"
          width="80"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <el-switch v-model="scope.row.IsVirtual" disabled />
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('box.Introduce')"
          width="150"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.IntroduceBox }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('box.CreatedUserName')"
          width="100"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('box.CreatedTime')"
          align="center"
          width="150"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedTime }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('baseBtn.Operation')"
          align="center"
          width="210"
          class-name="small-padding fixed-width"
          fixed="right"
        >
          <template slot-scope="scope">
            <el-button
              size="mini"
              type="primary"
              @click="handleUpdateBox(scope.row)"
            >{{ $t('baseBtn.editBtn') }}</el-button>
            <el-button
              size="mini"
              type="danger"
              @click="handleDelete(scope.row)"
            >{{ $t('baseBtn.removeBtn') }}</el-button>
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
    <!-- 创建/编辑 弹出框 -->
    <el-dialog
      v-el-drag-dialog
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
      :width="'90%'"
      :close-on-click-modal="false"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form
            ref="dataForm"
            :rules="rules"
            :model="Box"
            class="dialog-form"
            label-width="140px"
            label-position="left"
          >
            <el-form-item v-if="dialogStatus=='update'" :label="$t('box.Code')" prop="Code">
              <el-input
                v-model="Box.Code"
                clearable
                class="dialog-input"
                :placeholder="$t('box.inputBoxCode')"
                :disabled="dialogStatus=='update'"
              />
            </el-form-item>
            <el-form-item :label="$t('box.Name')" prop="Name">
              <el-input
                v-model="Box.Name"
                clearable
                class="dialog-input"
                :placeholder="$t('box.inputBoxName')"
              />
            </el-form-item>
            <el-form-item :label="$t('box.width')+'(mm)'" prop="BoxWidth">
              <el-input
                v-model="Box.BoxLength"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                :placeholder="$t('box.inputBoxwidth')"
              />
            </el-form-item>
            <el-form-item :label="$t('box.length')+'(mm)'" prop="BoxLength">
              <el-input
                v-model="Box.BoxWidth"
                clearable
                class="dialog-input"
                type="text"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                :placeholder="$t('box.inputBoxlength')"
              />
            </el-form-item>

            <el-form-item :label="$t('box.Type')">
              <el-select
                v-model="Box.Type"
                :multiple="false"
                reserve-keyword
                :placeholder="$t('box.inputBoxType')"
                style="width:300px"
              >
                <el-option
                  v-for="item in dictionaryList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Name"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="$t('box.Colour')">
              <el-select
                v-model="Box.BoxColour"
                :multiple="false"
                reserve-keyword
                :placeholder="$t('box.inputBoxColour')"
                style="width:300px"
              >
                <el-option
                  v-for="item in BoxColourTypeList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Name"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="$t('box.IsVirtual')">
              <el-switch
                v-model="Box.IsVirtual"
                active-color="#13ce66"
                inactive-color="#ff4949"
                :active-text="$t('box.Yes')"
                :inactive-text="$t('box.No')"
              />
            </el-form-item>
            <el-form-item :label="$t('box.Introduce')">
              <el-input
                v-model="Box.IntroduceBox"
                :autosize="{ minRows: 2, maxRows: 4}"
                type="textarea"
                :placeholder="$t('box.inputBoxRemarks')"
                class="dialog-input"
              />
            </el-form-item>
          </el-form>
        </el-col>
        <el-col :span="12">
          <div class="block">
            <img
              :src="BaseUrl+Box.PictureUrl "
              style="width: 600px; height: 440px;object-fit:cover; display: block;"
              fit="fit"
            >
          </div>
          <el-upload
            :show-file-list="false"
            :on-success="uploadingPicture"
            :action="uploadFileLibrary"
            :before-upload="PictureStatus"
            class="filter-button"
          >
            <el-button style="margin-left: 10px;">{{ $t('baseBtn.SelectPicture') }}</el-button>
          </el-upload>
        </el-col>
      </el-row>

      <div slot="footer" class="dialog-footer">
        <el-button @click="creatCancel">{{ $t('baseBtn.cancel') }}</el-button>
        <el-button
          v-if="dialogStatus=='create'"
          type="primary"
          @click="createData"
        >{{ $t('baseBtn.confirm') }}</el-button>
        <el-button v-else type="primary" @click="updateData">{{ $t('baseBtn.confirm') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { UploadingBox, getBoxPageRecords, createBox, editBox, deleteBox, getBoxTypeList } from '@/api/Box'
import { deletePicture } from '@/api/FileLibrary'
import waves from '@/directive/waves' // 水波纹指令
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'Box', // 载具箱管理
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      urls: null,
      dictionaryList: [], // 颜色字典保存
      BoxColourTypeList: [], // 种类字典保存
      src: '/logo.png',
      fits: 'contain',
      // 访问的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFileLibraryUpload',
      // 分页显示总查询数据
      total: null,
      // 用来判断是否更换了图片
      JudgeFileLibraty: false,

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
      // 载具箱实体
      Box: {
        ID: undefined, // 表的编号
        Code: '', // 货物编码
        Name: '', // 名称
        Type: '', // 种类
        IntroduceBox: '', // 介绍
        BoxWidth: '', // 宽度
        BoxLength: '', // BoxLength: '', // 高度
        IsVirtual: true, // 是否是虚拟载具
        CreatedTime: '', // 创建时间
        CreatedUserName: '', // 创建人
        PictureUrl: '/logo.png', // 图片地址
        BoxColour: '', // 颜色
        FileID: ''// 图片id
      },
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址，
      imageUrl: 'http://172.20.10.3:81/Assets/uploads/FileLibrary/Picture/20201111/blue.jpg'
    }
  },
  computed: {
    // 输入规则
    rules() {
      return {
        Name: [{ required: true, message: this.$t('box.inputBoxName'), trigger: 'blur' }],
        BoxLength: [{ required: true, message: this.$t('box.inputBoxlength'), trigger: 'blur' }],
        BoxWidth: [{ required: true, message: this.$t('box.inputBoxwidth'), trigger: 'blur' }]
      }
    },
    // 编辑创建
    textMap() {
      return {
        update: this.$t('box.update'),
        create: this.$t('box.create')
      }
    }
  },
  watch: {
    // 面板关闭，清空属性
    dialogFormVisible(value) {
      if (!value) {
        this.JudgeFileLibraty = false
      }
    }
  },
  created() {
    this.getList()
    this.getBoxTypeList()
    this.getBoxColourTypeList()
  },
  methods: {
    // 种类下拉
    getBoxTypeList() {
      getBoxTypeList('Box').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.dictionaryList = usersData
      })
    },
    // 颜色下拉
    getBoxColourTypeList() {
      getBoxTypeList('BoxColour').then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.BoxColourTypeList = usersData
      })
    },
    getList() { // 获取数据库中的所有数据
      this.listLoading = true
      getBoxPageRecords(this.listQuery).then(response => {
        var usersData = JSON.parse(response.data.Content)
        this.list = usersData.rows
        this.urls = usersData.rows.PictureUrl
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

    // 创建
    createData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          createBox(this.Box).then((res) => {
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
              this.$confirm('是否跳转到物料信息界面', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
              }).then(() => {
                this.$router.push('/BaseSetting/MaterialSetting/index');
              }).catch(() => {
                this.$message({
                  type: 'info',
                  message: '已取消执行'
                })
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('messageTips.messageFailure') + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },

    // 载具箱编辑
    handleUpdateBox(row) {
      this.Box = Object.assign({}, row) // copy obj
      this.dialogStatus = 'update'
      this.src = this.BaseUrl + row.PictureUrl
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 载具箱添加
    handleCreateBox() {
      this.resetBox()
      this.dialogStatus = 'create'
      this.Box.PictureUrl = '/Assets/images/blue.jpg'
      //  this.src = this.BaseUrl + this.Box.PictureUrl
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 载具箱编辑
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const BoxData = Object.assign({}, this.Box)
          editBox(BoxData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.Box.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.Box)
                  // 判断是否更改图片
                  if (v.FileID !== this.Box.FileID) {
                    deletePicture(v)
                  }
                  break
                }
              }
              this.dialogFormVisible = false
              this.$message({
                title: this.$t('messageTips.Succeed'),
                message: this.$t('messageTips.editSucceed'),
                type: 'success',
                duration: 2000
              })
              this.$confirm('是否跳转到物料信息界面', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
              }).then(() => {
                this.$router.push('/BaseSetting/MaterialSetting/index');
              }).catch(() => {
                this.$message({
                  type: 'info',
                  message: '已取消执行'
                })
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('messageTips.editFailure') + '：' + resData.Message,
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
      this.$message.warning(this.$t('messageTips.ImportJudge'),)
    },
    // 上传文件
    uploadFile(item) {
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      // let formTwo = JSON.stringify(form)
      UploadingBox(form).then((res) => {
        var resData = typeof res.data === 'string' ? JSON.parse(res.data) : res.data
        // var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          this.getList()
          this.$message({
            title: this.$t('messageTips.Succeed'),
            message: this.$t('messageTips.ImportSucceed'),
            type: 'success',
            duration: 2000
          })
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('messageTips.ImportFailure') + resData.Message,
            type: 'error',
            duration: 4000
          })
        }
      })
    },
    // 是否创建了图片实例
    PictureStatus(datas) {
      this.JudgeFileLibraty = true
    },
    // 图片上传
    uploadingPicture(datas) {
      var resData = JSON.parse(datas.Content)
      // 默认图片地址
      this.Box.PictureUrl = resData.Data.FilePath
      this.src = this.BaseUrl + this.Box.PictureUrl
      this.Box.FileID = resData.Data.Id
      if (resData.Success) {
        console.log()
      }
    },
    // 添加时选择图片后不确认添加
    creatCancel() {
      // 删除图片
      if (this.JudgeFileLibraty) {
        deletePicture(this.Box).then((res) => {
          var resDatas = JSON.parse(res.data.Content)
          if (!resDatas.Success) {
            this.$message({
              title: '失败',
              type: 'error',
              message: '图片未删除！',
              duration: 2000
            })
          }
        })
      }
      this.dialogFormVisible = false
    },

    // 模板下载
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Box/DoDownLoadTempBox'
      window.open(url)
    },
    // 载具箱数据导出
    handleDownUploadBox() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/Box/DoDownLoadTemp'
      window.open(url)
    },
    handleDelete(row) {
      this.$confirm(this.$t('messageTips.deleteTips'), this.$t('messageTips.Tips'), {
        confirmButtonText: this.$t('baseBtn.confirm'),
        cancelButtonText: this.$t('baseBtn.cancel'),
        type: 'warning'
      }).then(() => {
        this.Box = Object.assign({}, row) // copy obj
        this.deleteData(this.Box)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.messageCancel')
        })
      })
    },

    // 删除
    deleteData(data) {
      deleteBox(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          // 删除图片
          deletePicture(data).then((res) => {
            var resDatas = JSON.parse(res.data.Content)
            if (resDatas.Success) {
              this.$message({
                title: this.$t('messageTips.Succeed'),
                message: this.$t('messageTips.deleteSucceed'),
                type: 'success',
                duration: 2000
              })
            }
          })
          this.getList()
        } else {
          this.$message({
            title: this.$t('messageTips.Failure'),
            message: this.$t('messageTips.deleteFailure') + ':' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    resetBox() {
      this.Box = {
        ID: undefined,
        Code: '',
        Name: '',
        Type: '',
        IntroduceBox: '',
        BoxWidth: '',
        BoxLength: '',
        CreatedUserCode: '',
        CreatedUserName: '',
        CreatedTime: undefined,
        IsDeleted: false,
        IsVirtual: true,
        PictureUrl: '',
        FileID: ''
      }
    }
  }
}
</script>
