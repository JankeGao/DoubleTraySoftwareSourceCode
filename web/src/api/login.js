import request from '@/utils/request'

export function LoginByUsername(username, password) {
  return request({
    url: '/api/login/PostLogin',
    method: 'post',
    data: {
      Code: username,
      Password: password
    }
  })
}

export function getUserInfo() {
  return request({
    url: 'api/login/GetUserInfo',
    method: 'get',
    params: {}
  })
}

export function logout() {
  return request({
    url: '/api/login/Postlogout',
    method: 'post'
  })
}
