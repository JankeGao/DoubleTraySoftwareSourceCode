<template>
  <div id="User">
    <el-form
      ref="UserForm"
      :model="User"
      :rules="rules"
      status-icon
      label-width="100px"
      class="page-form"
      label-position="right"
    >
      <el-card>
        <h4 class="page-title-h4">个人中心</h4>
        <hr class="line-sub">
        <el-row :gutter="20">
          <el-col :span="10">
            <el-form-item label="手机:" prop="Mobilephone" class="item">
              <el-input
                v-model="User.Mobilephone"
                placeholder="请输入手机号码"
                class="dialog-input"
                onkeyup="value=value.replace(/[^\d]/g,'')"
                maxlength="11"
              />
            </el-form-item>
            <el-form-item label="密码:" prop="Password" class="item">
              <div>
                <el-input
                  v-model="User.Password"
                  type="password"
                  auto-complete="off"
                  class="dialog-input"
                  @change="changePassword"
                />
              </div>
            </el-form-item>
            <el-form-item label="确认密码:" prop="checkPass" class="item">
              <el-input
                v-model="User.checkPass"
                type="password"
                auto-complete="off"
                class="dialog-input"
              />
            </el-form-item>
            <el-form-item label="姓名:" prop="Name" class="item">
              <el-input v-model="User.Name" class="dialog-input" placeholder="请输入姓名" />
            </el-form-item>
            <el-form-item label="性别:" class="item">
              <el-radio-group v-model="User.Sex" @change="selectedSex">
                <el-radio label="1">男</el-radio>
                <el-radio label="2">女</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="14">
            <img :src="faceUrl" class="image">
            <div style="margin:20px 0px">人脸照片需小于200k</div>
            <el-upload
              :show-file-list="false"
              :on-success="uploadingPicture"
              :action="uploadFileLibrary"
              :before-upload="PictureStatus"
              :data="{Id:User.Id}"
              class="filter-button"
            >
              <el-button style="margin-bottom: 70px;">上传人脸图片</el-button>
            </el-upload>
          </el-col>
        </el-row>

        <div style="padding:20px 115px">
          <el-button type="primary" @click="updateUser">确定</el-button>
          <router-link to="/">
            <el-button>取消</el-button>
          </router-link>
        </div>
      </el-card>
    </el-form>
  </div>
</template>

<script>
import { editUserCenter, getUserInfobyCode } from '@/api/SysManage/User'
import md5 from 'js-md5'
import { isNull } from '@/utils'
import store from '../../../store'
import { deletePicture } from '@/api/FileLibrary'

export default {
  data() {
    var validatePass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入密码'))
      } else {
        if (this.User.checkPass !== '') {
          this.$refs.UserForm.validateField('checkPass')
        }
        callback()
      }
    }
    var validatePass2 = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.User.Password) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    return {
      isCreate: 1,
      imagecropperShow: false,
      imagecropperKey: 0,
      image: window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png',
      isChangePassword: false,
      // 角色实体
      User: {
        Id: undefined,
        Code: '',
        Name: '',
        Password: '123456',
        checkPass: '123456',
        Sex: '1',
        Mobilephone: '',
        WeXin: '',
        Remark: '',
        Header: window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png',
        Enabled: true
      },
      rules: {
        Name: [{ required: true, message: '请输入员工姓名', trigger: 'blur' }],
        Mobilephone: [{ required: true, message: '请输入正确的手机号码', trigger: 'blur', min: 6, max: 11 }],
        Password: [{ validator: validatePass, trigger: 'blur' }],
        checkPass: [{ required: true, validator: validatePass2, trigger: 'blur' }]
      },
      textMap: {
        1: '新建员工',
        2: '编辑员工'
      },
      src: '/logo.png',
      // faceUrl: '/logo.png',
      faceUrl: '',
      // 访问的服务器地址
      uploadFileLibrary: window.PLATFROM_CONFIG.baseUrl + '/api/FileLibrary/PostDoFaceLibraryUpload1',
      BaseUrl: window.PLATFROM_CONFIG.baseUrl, // 服务默认地址
      // 用来判断是否更换了图片
      JudgeFileLibraty: false
    }
  },
  created() {
    this.initData()
  },
  methods: {
    // 获取后台权限数据
    initData() {
      getUserInfobyCode(store.getters.code).then((res) => {
        var resData = JSON.parse(res.data.Content)
        console.log(resData)
        if (res.status === 200) {
          this.User = {
            Id: resData.Id,
            Code: resData.Code,
            Name: resData.Name,
            Password: resData.Password,
            checkPass: resData.Password,
            Sex: '1',
            Mobilephone: resData.Mobilephone,
            WeXin: resData.WeXin,
            Remark: resData.Remark,
            Header: resData.Header,
            PictureUrl: resData.PictureUrl,
            Enabled: resData.Enabled
          }
          const imageData = 'data:image/png;base64,' + this.User.Remark // this.BaseUrl + this.User.PictureUrl
          this.faceUrl = imageData.replace(/[\r\n]/g, '')
          // this.faceUrl = //this.BaseUrl + this.User.PictureUrl
          if (resData.Sex === 2) this.User.Sex = '2'
          if (isNull(resData.Header)) this.User.Header = window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png'
        } else {
          this.$message({
            title: '失败',
            message: '获取用户信息失败：' + resData.Message,
            type: 'error',
            duration: 2000
          })
        }
      })
    },
    updateUser() {
      this.$refs['UserForm'].validate((valid) => {
        if (valid) {
          if (this.isChangePassword) {
            this.User.Password = md5(this.User.Password)
            this.User.checkPass = md5(this.User.checkPass)
          }
          editUserCenter(this.User).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.$message({
                title: '成功',
                message: '个人信息编辑成功',
                type: 'success',
                duration: 2000
              })
              this.resetUser()
              this.$router.push({ path: '/' })
            } else {
              this.$message({
                title: '失败',
                message: '个人信息编辑失败：' + resData.Message,
                type: 'error',
                duration: 2000
              })
            }
          })
        }
      })
    },
    // 是否创建了图片实例
    PictureStatus(datas) {
      this.JudgeFileLibraty = true
    },
    // 上传图片
    uploadingPicture(data) {
      var resData = JSON.parse(data.Content)
      if (resData.Success) {
        this.User.PictureUrl = resData.Data.FilePath
        // this.faceUrl = this.BaseUrl + this.User.PictureUrl
        const imageData = 'data:image/png;base64,' + resData.Data.Remark // this.BaseUrl + this.User.PictureUrl
        this.faceUrl = imageData.replace(/[\r\n]/g, '')
        this.User.Remark = resData.Data.Remark
        this.User.FileID = resData.Data.Id
      } else {
        this.$message({
        })
      }
    },
    // 添加时选择图片后不确认添加
    creatCancel() {
      // 删除图片
      if (this.JudgeFileLibraty) {
        deletePicture(this.EquipmentType).then((res) => {
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
    // 头像控件
    cropSuccess(resData) {
      this.imagecropperShow = false
      this.imagecropperKey = this.imagecropperKey + 1
      this.User.Header = resData
    },
    close() {
      this.imagecropperShow = false
    },
    changePassword() {
      this.isChangePassword = true
    },
    selectedSex(val) {
      this.User.Sex = val
    },
    // 重置
    resetUser() {
      this.User = {
        Id: undefined,
        Code: '',
        Name: '',
        Password: '123456',
        checkPass: '123456',
        Sex: '1',
        Mobilephone: '',
        Roles: [],
        WeXin: '',
        Remark: '',
        Header: window.PLATFROM_CONFIG.baseUrl + '/Assets/images/3.png',
        Enabled: true
      }
    }
  }
}
</script>

