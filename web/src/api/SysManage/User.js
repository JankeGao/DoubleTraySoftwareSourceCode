import request from '@/utils/request'

export function userList(query) {
  return request({
    url: 'api/User/GetUserList',
    method: 'get',
    params: query
  })
}

export function getUserInfos() {
  return request({
    url: 'api/User/GetUserInfos',
    method: 'get'
  })
}

export function createUser(data) {
  return request({
    url: 'api/User/PostDoCreate',
    method: 'post',
    data
  })
}

export function editUser(data) {
  return request({
    url: 'api/User/PostDoEdit',
    method: 'post',
    data
  })
}

export function editHeader(data) {
  return request({
    url: 'api/User/DoHeaderEdit',
    method: 'post',
    params: { code: data }
  })
}

export function deleteUser(data) {
  return request({
    url: 'api/User/PostDoRemove',
    method: 'post',
    data
  })
}

export function getUserInfobyCode(data) {
  return request({
    url: 'api/User/GetUserInfo',
    method: 'get',
    params: { code: data }
  })
}

export function editUserCenter(data) {
  return request({
    url: 'api/User/PostDoEditUserCenter',
    method: 'post',
    data
  })
}

export function getUserlInfoList(data) {
  return request({
    url: 'api/User/GetUserlInfoList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function getUserInfoByRole(data) {
  return request({
    url: 'api/User/GetUserInfoByRole',
    method: 'get',
    params: { role: data }
  })
}

export function editUserArea(data) {
  return request({
    url: 'api/User/PostDoEditUserArea',
    method: 'post',
    data
  })
}
