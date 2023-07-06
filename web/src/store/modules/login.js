import { logout } from '@/api/login'
import { getToken, setToken, removeToken } from '@/utils/auth'
import router, { resetRouter } from '@/router'
import { LoginByUsername, getUserInfo } from '@/api/login'
import md5 from 'js-md5'

//  token: getToken(),
const state = {
  token: getToken(),
  name: '',
  avatar: '',
  introduction: '',
  roles: []
}

const mutations = {
  SET_TOKEN: (state, token) => {
    state.token = token
  },
  SET_INTRODUCTION: (state, introduction) => {
    state.introduction = introduction
  },
  SET_NAME: (state, name) => {
    state.name = name
  },
  SET_AVATAR: (state, avatar) => {
    state.avatar = avatar
  },
  SET_ROLES: (state, roles) => {
    state.roles = roles
  },
  SET_USERCODE: (state, code) => {
    state.code = code
  }
}

const actions = {

  // 登录
  LoginByUsername({ commit }, userInfo) {
    const username = userInfo.username.trim()
    return new Promise((resolve, reject) => {
      LoginByUsername(username, md5(userInfo.password)).then(response => {
        var resData = JSON.parse(response.data.Content)
        setToken(resData.Data)
        commit('SET_TOKEN', resData.Data)
        resolve(response)
      }).catch(error => {
        reject(error)
      })
    })
  },

  // get user info
  GetUserInfo({ commit, state }) {
    return new Promise((resolve, reject) => {
      getUserInfo(state.token).then(response => {
        var usersData = JSON.parse(response.data.Content) // 将json字符串转换为json对象
        if (usersData.RoleIds && usersData.RoleIds.length > 0) { // 验证返回的roles是否是一个非空数组
          const header = window.PLATFROM_CONFIG.baseUrl + usersData.Header
          commit('SET_ROLES', usersData.RoleIds)
          commit('SET_NAME', usersData.Name)
          commit('SET_USERCODE', usersData.Code)
          commit('SET_AVATAR', header)
          resolve(response)
          // resolve(data)
        } else {
          resolve()
          reject('获取用户角色信息失败')
        }
      }).catch(error => {
        reject(error)
      })
    })
  },

  // 前端 登出
  FedLogOut({ commit }) {
    return new Promise(resolve => {
      commit('SET_TOKEN', '')
      removeToken()
      resolve()
    })
  },

  // user logout
  logout({ commit, state }) {
    return new Promise((resolve, reject) => {
      logout(state.token).then(() => {
        commit('SET_TOKEN', '')
        commit('SET_ROLES', [])
        removeToken()
        resetRouter()
        resolve()
      }).catch(error => {
        reject(error)
      })
    })
  },

  // remove token
  resetToken({ commit }) {
    return new Promise(resolve => {
      commit('SET_TOKEN', '')
      commit('SET_ROLES', [])
      removeToken()
      resolve()
    })
  },

  // dynamically modify permissions
  changeRoles({ commit, dispatch }, role) {
    return new Promise(async resolve => {
      const token = role + '-token'

      commit('SET_TOKEN', token)
      setToken(token)

      const { roles } = await dispatch('getInfo')

      resetRouter()

      // generate accessible routes map based on roles
      const accessRoutes = await dispatch('permission/generateRoutes', roles, { root: true })

      // dynamically add accessible routes
      router.addRoutes(accessRoutes)

      // reset visited views and cached views
      dispatch('tagsView/delAllViews', null, { root: true })

      resolve()
    })
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
