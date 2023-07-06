import request from '@/utils/request'

export function getModuleList() {
  return request({
    url: 'api/Authorization/GetModuleList',
    method: 'get'
  })
}
export function setAuthorization(data) {
  return request({
    url: 'api/Authorization/PostSetAuthorization',
    method: 'post',
    data: { ModuleAuthJson: JSON.stringify(data.arr), TypeCode: data.typeCode, Type: data.type }
  })
}

export function getModuleAuth(data, type) {
  return request({
    url: 'api/Authorization/GetModuleAuth',
    method: 'get',
    params: { typeCode: data, type: type }
  })
}

// 用户模块，获取相关角色权限
export function getModuleUserRoleAuth(data, type) {
  return request({
    url: 'api/Authorization/GetModuleUserRoleAuth',
    method: 'get',
    params: { typeCode: data, type: type }
  })
}

// 用户模块，获取该用户权限
export function getModuleUserAuth(data) {
  return request({
    url: 'api/Authorization/GetModuleUserAuth',
    method: 'get',
    params: { typeCode: data }
  })
}
