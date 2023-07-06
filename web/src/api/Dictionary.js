import request from '@/utils/request'
export function GetDictionaryList(query) {
  return request({
    url: 'api/Dictionary/GetDictionaryTypeList',
    method: 'get',
    params: query
  })
}
export function createDictionaryType(data) {
  return request({
    url: 'api/Dictionary/PostDoCreateType',
    method: 'post',
    data
  })
}

export function editDictionaryType(data) {
  return request({
    url: 'api/Dictionary/PostDoEditType',
    method: 'post',
    data
  })
}

export function deleteDictionaryType(data) {
  return request({
    url: 'api/Dictionary/PostDodDeleteType',
    method: 'post',
    data
  })
}
export function getDictionaryTypeTree(query) {
  return request({
    url: 'api/Dictionary/GetDictionaryTreeList',
    method: 'get',
    params: query
  })
}

export function createDictionary(data) {
  return request({
    url: 'api/Dictionary/PostDoCreate',
    method: 'post',
    data
  })
}

export function editDictionary(data) {
  return request({
    url: 'api/Dictionary/PostDoEdit',
    method: 'post',
    data
  })
}

export function deleteDictionary(data) {
  return request({
    url: 'api/Dictionary/PostDodDelete',
    method: 'post',
    data
  })
}
export function getPageRecords(query) {
  return request({
    url: 'api/Dictionary/GetDictionaryList',
    method: 'get',
    params: query
  })
}
