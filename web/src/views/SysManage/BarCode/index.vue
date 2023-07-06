<template>
  <div id="User">
    <el-form ref="UserForm" :model="User" :rules="rules" status-icon label-width="100px" class="page-form" label-position="right">
      <el-card>
        <!-- <div style="width:300px; ">
          <div style="display: inline-block;"><h4 >个人中心</h4> </div>
          <div style="display: inline-block">
            <router-link to="/">
              <span class="el-icon-close"/>
            </router-link>
          </div>
        </div>
        <hr class="line" > -->
        <h4 class="page-title-h4">个人中心</h4>
        <hr class="line-sub">
        <el-row :gutter="20">
          <el-col :span="10">
            <input v-model="barcodeValue"><br>
            <barcode :value="barcodeValue">
              Show this if the rendering fails.
            </barcode>
          </el-col>
          <el-col :span="14">
            <div class="components-container">
              <div> <pan-thumb :image="User.Header" /></div>
              <div style="margin:20px 25px">
                <el-button type="primary" icon="upload" @click="imagecropperShow=true">上传头像
                </el-button></div>
              <image-cropper
                v-show="imagecropperShow"
                :key="imagecropperKey"
                :width="300"
                :height="300"
                lang-type="zh"
                @close="close"
                @crop-success="cropSuccess"
              />
            </div>
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
import ImageCropper from '@/components/ImageCropper'
import PanThumb from '@/components/PanThumb'
import { editUserCenter, getUserInfobyCode } from '@/api/SysManage/User'
import md5 from 'js-md5'
import { isNull } from '@/utils'
import store from '../../../store'
import VueBarcode from 'vue-barcode'

export default {
  components: { ImageCropper, PanThumb, 'barcode': VueBarcode },
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
      barcodeValue: 'test',
      image: 'https://wpimg.wallstcn.com/577965b9-bb9e-4e02-9f0c-095b41417191',
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
        Header: 'https://wpimg.wallstcn.com/577965b9-bb9e-4e02-9f0c-095b41417191',
        Enabled: true
      },
      rules: {
        Name: [{ required: true, message: '请输入员工姓名', trigger: 'blur' }],
        Mobilephone: [{ required: true, message: '请输入手机号码', trigger: 'blur' }],
        Password: [{ validator: validatePass, trigger: 'blur' }],
        checkPass: [{ required: true, validator: validatePass2, trigger: 'blur' }]
      },
      textMap: {
        1: '新建员工',
        2: '编辑员工'
      }
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
            Enabled: resData.Enabled
          }
          if (resData.Sex === 2) this.User.Sex = '2'
          if (isNull(resData.Header)) this.User.Header = 'https://wpimg.wallstcn.com/577965b9-bb9e-4e02-9f0c-095b41417191'
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
    // 头像控件
    cropSuccess(resData) {
      this.imagecropperShow = false
      this.imagecropperKey = this.imagecropperKey + 1
      // this.User.Header = resData.files.avatar
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
        Header: 'https://wpimg.wallstcn.com/577965b9-bb9e-4e02-9f0c-095b41417191',
        Enabled: true
      }
    }
  }
}
</script>

