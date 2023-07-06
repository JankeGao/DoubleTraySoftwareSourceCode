import request from '@/utils/request'

export function queryList(query) {
  return request({
    url: 'api/Department/GetList',
    method: 'get',
    params: query
  })
}

export function getDepartmentInfos() {
  return request({
    url: 'api/Department/GetDepartmentInfos',
    method: 'get'
  })
}

export function createDepartment(data) {
  return request({
    url: 'api/Department/PostDoCreate',
    method: 'post',
    data
  })
}

export function editDepartment(data) {
  return request({
    url: 'api/Department/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteDepartment(data) {
  return request({
    url: 'api/Department/PostDoRemove',
    method: 'post',
    data
  })
}

export function getDepartmentTreeData() {
  return request({
    url: 'api/Department/GetDepartmentTreeData',
    method: 'get'
  })
}
