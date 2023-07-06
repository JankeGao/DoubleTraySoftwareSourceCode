import request from '@/utils/request'

export function getRoleList() {
  return request({
    url: 'api/Role/GetRole',
    method: 'get',
    params: {}
  })
}

export function fetchList(query) {
  return request({
    url: 'api/Role/GetRoleList',
    method: 'get',
    params: query
  })
}

export function createRole(data) {
  return request({
    url: 'api/Role/PostDoCreate',
    method: 'post',
    data
  })
}

export function editRole(data) {
  return request({
    url: 'api/Role/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteRole(data) {
  return request({
    url: 'api/Role/PostDoRemove',
    method: 'post',
    data
  })
}

