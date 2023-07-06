import request from '@/utils/request'

export function fetchList(query) {
  return request({
    url: 'api/CodeRule/GetCodeRuleList',
    method: 'get',
    params: query
  })
}

export function getCodeRuleList() {
  return request({
    url: 'api/CodeRule/GetRuleAssemblys',
    method: 'get',
    params: {}
  })
}

export function getResetAssemblys() {
  return request({
    url: 'api/CodeRule/GetResetAssemblys',
    method: 'get',
    params: {}
  })
}

export function getItems(data) {
  return request({
    url: 'api/CodeRule/GetItems',
    method: 'get',
    params: { codeRuleId: data }
  })
}

export function getEntityInfo() {
  return request({
    url: 'api/EntityInfo/GetEntityInfo',
    method: 'get',
    params: {}
  })
}

export function createCodeRule(data) {
  return request({
    url: 'api/CodeRule/PostDoCreate',
    method: 'post',
    data
  })
}

export function editCodeRule(data) {
  return request({
    url: 'api/CodeRule/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteCodeRule(data) {
  return request({
    url: 'api/CodeRule/PostDoRemove',
    method: 'post',
    data
  })
}

