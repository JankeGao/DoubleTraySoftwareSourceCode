import { getMenu } from '@/api/getMenu'

const menuList = {
  state: {
    token: '',
    menuList: []
  },

  mutations: {
    SET_MENU: (state, data) => {
      state.menuList = data
    }
  },

  /**
  * 根据token 获取动态角色路由
  * @param {*} token
  */
  actions: {
    // 登录
    GetMenu({ commit }, token) {
      console.log(token)
      return new Promise((resolve, reject) => {
        getMenu(token).then(response => {
          var usersData = JSON.parse(response.data) // 将json字符串转换为json对象
          commit('SET_MENU', usersData)
          resolve(response)
        }).catch(error => {
          reject(error)
        })
      })
    }
  }
}

export default menuList
