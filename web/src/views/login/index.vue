<template>
  <div class="login-container" :style="{backgroundImage: 'url(' + bg2 + ')' }">
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" class="login-form" auto-complete="on" label-position="left">
      <el-row>
        <el-col :xs="1" :sm="3" :md="3" :lg="3" :xl="3">
          &nbsp;
        </el-col>
        <el-col :xs="22" :sm="18" :md="18" :lg="18" :xl="18">
          <h3 class="title">{{ $t('login.title') }}</h3>
          <el-form-item prop="username">
            <span class="svg-container svg-container_login">
              <svg-icon icon-class="user" />
            </span>
            <el-input v-model="loginForm.username" name="username" type="text" auto-complete="on" placeholder="用户名" />
          </el-form-item>
          <el-form-item prop="password">
            <span class="svg-container">
              <svg-icon icon-class="password" />
            </span>
            <el-input
              v-model="loginForm.password"
              :type="pwdType"
              name="password"
              auto-complete="on"
              placeholder="密码"
            />
            <span class="show-pwd" @click="showPwd">
              <svg-icon icon-class="eye" />
            </span>
          </el-form-item>
          <el-form-item>
            <el-button :loading="loading" type="primary" style="width:100%;" @click.native.prevent="handleLogin">
              {{ $t('login.logIn') }}
            </el-button>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script>
import LangSelect from '@/components/LangSelect'
import Bg2 from './background.png'
// import Bg2 from './background.png'
export default {
  name: 'Login',
  components: { LangSelect },
  data() {
    const validateUsername = (rule, value, callback) => {
      callback()
    }
    const validatePass = (rule, value, callback) => {
      if (value.length < 5) {
        callback(new Error('密码不能小于5位'))
      } else {
        callback()
      }
    }
    return {
      bg2: Bg2,
      loginForm: {
        username: '',
        password: ''
      },
      loginRules: {
        username: [{ required: true, trigger: 'blur', validator: validateUsername }],
        password: [{ required: true, trigger: 'blur', validator: validatePass }]
      },
      loading: false,
      pwdType: 'password'
    }
  },
  // 键盘监听事件，回车触发登录事件
  created() {
    var _self = this
    document.onkeydown = function(e) {
      var key = window.event.keyCode
      if (key === 13) {
        _self.handleLogin()
      }
    }
  },
  methods: {
    showPwd() {
      if (this.pwdType === 'password') {
        this.pwdType = ''
      } else {
        this.pwdType = 'password'
      }
    },
    handleLogin() {
      this.$refs.loginForm.validate(valid => {
        if (valid) {
          this.loading = true
          this.$store.dispatch('login/LoginByUsername', this.loginForm).then((res) => {
            var resData = JSON.parse(res.data.Content)
            if (resData.Success) {
              this.loginSuccess()
              this.loading = false
              this.$router.push({ path: '/' })
              window.location.reload()
            } else {
              this.loginFail(resData.Message)
              this.loading = false
            }
          }).catch(() => {
            this.loading = false
          })
        } else {
          console.log('用户名密码输入错误')
          return false
        }
      })
    },
    loginSuccess() {
      this.$message({
        message: '登录成功',
        type: 'success'
      })
    },
    loginFail(message) {
      this.$message({
        message: message,
        type: 'error'
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss">
$bg:#2d3a4b;
$light_gray:#eee;

/* reset element-ui css */
.login-container {
  .el-input {
    display: inline-block;
    height: 47px;
    width: 85%;
    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      color: $light_gray;
      height: 47px;
      &:-webkit-autofill {
        -webkit-box-shadow: 0 0 0px 1000px $bg inset !important;
        -webkit-text-fill-color: #fff !important;
      }
    }
  }
  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(0, 0, 0, 0.1);
    border-radius: 5px;
    color: #454545;
  }
}
  .master {
    position:absolute;
    left:47%;
    bottom:10px;
    text-align :center;
  }

</style>

<style rel="stylesheet/scss" lang="scss">
// $bg:#F2F6FC;
// $light_gray:#eee;

/* reset element-ui css */
.login-container {
 //   background-image: url('/background.jpg');
    background-repeat: no-repeat;
    background-size:100% 100%;
    -moz-background-size:100% 100%;
    // background-position: center left;
    // background-size: 36px;
  .el-input {
    display: inline-block;
    height: 47px;
    width: 85%;
    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      // color: $light_gray;
      height: 47px;
      // &:-webkit-autofill {
      //   -webkit-box-shadow: 0 0 0px 1000px $bg inset !important;
      //   -webkit-text-fill-color: #fff !important;
      // }
    }
  }
  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(0, 0, 0, 0.5);
    border-radius: 5px;
    // color: #454545;
  }
}
  .master {
    position:absolute;
    left:47%;
    bottom:10px;
    text-align :center;
  }

</style>

<style rel="stylesheet/scss" lang="scss" scoped>
$bg:#2d3a4b;
$dark_gray:#889aa4;
$light_gray:#eee;
.login-container {
  position: fixed;
  height: 100%;
  width: 100%;
  background-color: $bg;
  .login-form {
    background-color: $bg;
    border-radius:1%;
    background-color:rgba(0,0,0,0.5);
    position: absolute;
    left: 0;
    right: 0;
    min-width:300px;
    max-width: 520px;
    padding: 35px 35px 15px 35px;
    margin: 120px auto;
  }
  .tips {
    font-size: 14px;
    color: #fff;
    margin-bottom: 10px;
    span {
      &:first-of-type {
        margin-right: 16px;
      }
    }
  }
  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
    &_login {
      font-size: 20px;
    }
  }
  .title {
    font-size: 26px;
    font-weight: 400;
    color: $light_gray;
    margin: 0px auto 40px auto;
    text-align: center;
    font-weight: bold;
  }
  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }
    .set-language {
      color: #fff;
      position: absolute;
      top: 3px;
      font-size: 18px;
      right: 0px;
      cursor: pointer;
    }
}
</style>

