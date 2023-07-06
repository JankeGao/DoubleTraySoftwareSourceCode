import request from '@/utils/request'

export function getModuleList() {
  return request({
    url: 'api/Authorization/GetModuleList',
    method: 'get',
    params: {}
  })
}

export function getModuleTreeList() {
  return request({
    url: 'api/Authorization/GetModuleTreeList',
    method: 'get',
    params: {}
  })
}

export function fetchList(query) {
  return request({
    url: 'api/ArticleInfo/GetModuleList',
    method: 'get',
    params: query
  })
}

export function createModule(data) {
  return request({
    url: 'api/Authorization/PostDoCreate',
    method: 'post',
    data
  })
}

export function editModule(data) {
  return request({
    url: 'api/Authorization/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteModule(data) {
  return request({
    url: 'api/Authorization/PostDoRemove',
    method: 'post',
    data
  })
}

