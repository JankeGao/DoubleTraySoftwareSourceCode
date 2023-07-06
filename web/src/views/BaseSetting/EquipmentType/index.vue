<template>
  <div class="app-container">
    <!-- 筛选栏 -->
    <el-card class="search-card">
      <div class="filter-container">
        <el-input
          v-model="listQuery.Code"
          :placeholder="$t('EquipmentType.inputEquipmentCode')"
          class="filter-item"
          clearable
          @keyup.enter.native="handleFilter"
          @clear="handleFilter"
        />
        <el-select
          v-model="listQuery.Type"
          :clearable="true"
          :placeholder="$t('EquipmentType.choiceEquipmentType')"
          @change="handleFilter"
        >
          <el-option
            v-for="item in SelectTypeList"
            :key="item.Code"
            :label="item.Name"
            :value="item.Code"
          />
        </el-select>
        <el-select
          v-model="listQuery.Brand"
          :placeholder="$t('EquipmentType.choiceEquipmentBrand')"
          :clearable="true"
          @change="handleFilter"
        >
          <el-option
            v-for="item in SelectBrandList"
            :key="item.Code"
            :label="item.Name"
            :value="item.Code"
          />
        </el-select>
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
          @click="handleCreate"
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
        <el-button
          class="filter-button"
          type="primary"
          icon="el-icon-download"
          @click="ExportEquipmentType()"
        >{{ $t('baseBtn.exportBtn') }}</el-button>
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
        <el-table-column
          :label="$t('EquipmentType.Code')"
          width="120"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.Code }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('EquipmentType.Picture')"
          align="center"
          width="80"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <div class="image-picture">
              <img
                :src="BaseUrl+scope.row.PictureUrl "
                style="height:100%;width:100%; display: block;"
                fit="fit"
              >
            </div>
          </template>
        </el-table-column>
        <el-table-column :label="$t('EquipmentType.Remark')" align="center" show-overflow-tooltip>
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('EquipmentType.brand')"
          width="140"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.BrandDescription }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('EquipmentType.Type')"
          width="120"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.TypeDescription }}</span>
          </template>
        </el-table-column>

        <el-table-column
          :label="$t('EquipmentType.CreatedUserName')"
          width="150"
          align="center"
          show-overflow-tooltip
        >
          <template slot-scope="scope">
            <span>{{ scope.row.CreatedUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="$t('EquipmentType.CreatedTime')"
          align="center"
          show-overflow-tooltip
          width="280"
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
              @click="handleUpdate(scope.row)"
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
            :model="EquipmentType"
            class="dialog-form"
            label-width="135px"
            label-position="left"
          >
            <el-form-item :label="$t('EquipmentType.Code')" prop="Code">
              <el-input
                v-model="EquipmentType.Code"
                clearable
                class="dialog-input"
                :placeholder="$t('EquipmentType.inputEquipmentCode')"
                :disabled="dialogStatus=='update'"
              />
            </el-form-item>
            <el-form-item :label="$t('EquipmentType.Remark')" prop="Remark">
              <el-input
                v-model="EquipmentType.Remark"
                clearable
                class="dialog-input"
                :placeholder="$t('EquipmentType.inputEquipmentRemark')"
              />
            </el-form-item>
            <el-form-item :label="$t('EquipmentType.brand')">
              <el-select
                v-model="EquipmentType.Brand"
                :multiple="false"
                reserve-keyword
                :placeholder="$t('EquipmentType.choiceEquipmentBrand')"
                style="width:300px"
              >
                <el-option
                  v-for="item in BrandList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="$t('EquipmentType.Type')">
              <el-select
                v-model="EquipmentType.Type"
                :multiple="false"
                reserve-keyword
                :placeholder="$t('EquipmentType.choiceEquipmentType')"
                style="width:300px"
              >
                <el-option
                  v-for="item in TypeList"
                  :key="item.Code"
                  :label="item.Name"
                  :value="item.Code"
                />
              </el-select>
            </el-form-item>
          </el-form>
        </el-col>
        <el-col :span="12">
          <div class="block">
            <img
              :src="src"
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
import { Uploading, PageRecordsEquipmentType, createEquipmentType, editEquipmentType, deleteEquipmentType } from '@/api/EquipmentType.js'
import waves from '@/directive/waves' // 水波纹指令
import { deletePicture } from '@/api/FileLibrary'
import elDragDialog from '@/directive/el-dragDialog' // base on element-ui

export default {
  name: 'Box', // 设备型号管理
  directives: {
    elDragDialog,
    waves
  },
  data() {
    return {
      urls: null,
      src: '/logo.png',
      fits: 'contain',
      // 访问图片的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFileLibraryUpload',
      // 分页显示总查询数据
      total: null,
      listLoading: false,
      JudgeFileLibraty: false,
      // 分页查询
      listQuery: {
        Page: 1,
        Rows: 15,
        Code: '',
        Status: undefined,
        Sort: 'id',
        Name: '',
        Brand: '',
        Type: ''
      },
      downloadLoading: false,
      TableKey: 0,
      list: null,
      dialogFormVisible: false,
      dialogStatus: '',
      // 设备型号实体
      EquipmentType: {
        ID: undefined, // 表的编号
        Code: '', // 设备型号
        Remark: '', // 设备描述
        Brand: '', // 品牌
        Type: '', // 种类
        CreatedTime: '', // 创建时间
        CreatedUserName: '', // 创建人
        PictureUrl: 'Assets\images\blue.jpg', // 图片地址
        FileID: ''// 图片id
      },
      BaseUrl: window.PLATFROM_CONFIG.baseUrl // 服务默认地址
    }
  },
  computed: {
    // 输入规则
    rules() {
      return {
        Code: [{ required: true, message: this.$t('EquipmentType.inputEquipmentCode'), trigger: 'blur' }],
        Remark: [{ required: true, message: this.$t('EquipmentType.inputEquipmentRemark'), trigger: 'blur' }]
      }
    },
    // 编辑创建
    textMap() {
      return {
        update: this.$t('EquipmentType.update'),
        create: this.$t('EquipmentType.create')
      }
    },
    SelectTypeList() {
      return [
        {
          Code: undefined, Name: this.$t('EquipmentType.whole')
        },
        {
          Code: 0, Name: this.$t('EquipmentType.GoUpDecline')
        },
        {
          Code: 1, Name: this.$t('EquipmentType.Rotation')
        }
      ]
    },
    TypeList() {
      return [
        {
          Code: 0, Name: this.$t('EquipmentType.GoUpDecline')
        },
        {
          Code: 1, Name: this.$t('EquipmentType.Rotation')
        }
      ]
    },
    SelectBrandList() {
      return [
        {
          Code: undefined, Name: this.$t('EquipmentType.whole')
        },
        {
          Code: 0, Name: this.$t('EquipmentType.NamgyaiVoluntarily')
        },
        {
          Code: 1, Name: this.$t('EquipmentType.Kardex')
        },
        {
          Code: 2, Name: this.$t('EquipmentType.Hanel')
        }
      ]
    },
    BrandList() {
      return [
        {
          Code: 0, Name: this.$t('EquipmentType.NamgyaiVoluntarily')
        },
        {
          Code: 1, Name: this.$t('EquipmentType.Kardex')
        },
        {
          Code: 2, Name: this.$t('EquipmentType.Hanel')
        }
      ]
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
  },
  methods: {
    getList() { // 获取数据库中的所有数据
      this.listLoading = true
      PageRecordsEquipmentType(this.listQuery).then(response => {
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
          createEquipmentType(this.EquipmentType).then((res) => {
            var resData = JSON.parse(res.data.Content)
            // console.log(resData)
            if (resData.Success) {
              this.dialogFormVisible = false
              this.getList()
              this.$message({
                title: this.$t('messageTips.Succeed'),
                message: this.$t('messageTips.messageSucceed'),
                type: 'success',
                duration: 2000
              })
            } else {
              this.$message({
                title: this.$t('messageTips.Failure'),
                message: this.$t('messageTips.messageFailure') + '：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },

    // 设备型号编辑
    handleUpdate(row) {
      this.EquipmentType = Object.assign({}, row) // copy obj
      // console.log(this.EquipmentType)
      this.dialogStatus = 'update'
      this.src = this.BaseUrl + row.PictureUrl
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 设备型号添加
    handleCreate() {
      this.resetEquipmentType()
      this.src = '/logo.png'
      this.dialogStatus = 'create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },

    // 设备型号编辑
    updateData() {
      this.$refs['dataForm'].validate((valid) => {
        if (valid) {
          const EquipmentTypeData = Object.assign({}, this.EquipmentType)
          editEquipmentType(EquipmentTypeData).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              for (const v of this.list) {
                if (v.Id === this.EquipmentType.Id) {
                  const index = this.list.indexOf(v)
                  this.list.splice(index, 1, this.EquipmentType)
                  // 判断是否更改了图片更改了则删除原来图片
                  if (v.FileID !== this.EquipmentType.FileID) {
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
              this.getList()
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
      // console.log(item)
      const fileObj = item.file
      // FormData 对象
      const form = new FormData()
      // 文件对象
      form.append('file', fileObj)
      form.append('comId', this.comId)
      Uploading(form).then((res) => {
        console.log(res)
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
            message: this.$t('messageTips.ImportFailure') + ':' + resData.Message,
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
    // 添加时选择图片后不确认添加
    creatCancel() {
      // 删除图片
      if (this.JudgeFileLibraty) {
        deletePicture(this.EquipmentType).then((res) => {
          var resDatas = JSON.parse(res.data.Content)
          if (!resDatas.Success) {
            // this.$message({
            //   title: this.$t('messageTips.Succeed'),
            //   type: 'success',
            //   message: '图片未删除！',
            //   duration: 2000
            // })
          }
        })
      }

      this.dialogFormVisible = false
    },

    // 图片上传
    uploadingPicture(res) {
      var resData = JSON.parse(res.Content)
      if (resData.Success) {
        // 不选择图片时显示的默认图片
        this.EquipmentType.PictureUrl = resData.Data.FilePath
        this.src = this.BaseUrl + this.EquipmentType.PictureUrl
        this.EquipmentType.FileID = resData.Data.Id
        this.$message({
          title: this.$t('messageTips.Succeed'),
          message: this.$t('messageTips.ImportSucceed'),
          type: 'success',
          duration: 2000
        })
      } else {
        this.$message({
          title: this.$t('messageTips.Failure'),
          message: this.$t('messageTips.ImportFailure') + ':' + resData.Message,
          type: 'error',
          duration: 4000
        })
      }
    },

    // 模板下载
    handleDownUpload() {
      var url = window.PLATFROM_CONFIG.baseUrl + '/api/EquipmentType/DoDownLoadTemp'
      window.open(url)
    },

    // 设备型号数据导出
    ExportEquipmentType() {
      this.$confirm(this.$t('messageTips.Export'), this.$t('messageTips.Tips'), {
        confirmButtonText: this.$t('baseBtn.confirm'),
        cancelButtonText: this.$t('baseBtn.cancel'),
        type: 'warning'
      }).then(() => {
        var url = window.PLATFROM_CONFIG.baseUrl + '/api/EquipmentType/ExportInformation'
        window.open(url)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.messageCancel')
        })
      })
    },
    handleDelete(row) {
      this.$confirm(this.$t('messageTips.deleteTips'), this.$t('messageTips.Tips'), {
        confirmButtonText: this.$t('baseBtn.confirm'),
        cancelButtonText: this.$t('baseBtn.cancel'),
        type: 'warning'
      }).then(() => {
        this.EquipmentType = Object.assign({}, row) // copy obj
        this.deleteData(this.EquipmentType)
      }).catch(() => {
        this.$message({
          type: 'info',
          message: this.$t('messageTips.messageCancel')
        })
      })
    },

    // 删除
    deleteData(data) {
      deleteEquipmentType(data).then((res) => {
        var resData = JSON.parse(res.data.Content)
        if (resData.Success) {
          this.dialogFormVisible = false
          // 调用图片接口删除图片信息
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
    resetEquipmentType() {
      this.EquipmentType = {
        ID: undefined,
        Code: '',
        Remark: '', // 设备描述
        Brand: '', // 品牌
        Type: '', // 种类
        CreatedUserName: '', // 创建人
        CreatedUserCode: '',
        CreatedTime: undefined, // 创建时间
        IsDeleted: false,
        PictureUrl: '',
        FileID: ''// 图片id
      }
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.image-picture {
  width: 40px;
  height: 40px;
}
.block {
  width: 500px;
  height: 500px;
}
</style>
