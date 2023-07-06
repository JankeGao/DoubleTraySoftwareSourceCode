import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Check/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getCheckDetailPageRecords(query) {
  return request({
    url: 'api/Check/GetCheckDetailPageRecords',
    method: 'get',
    params: query
  })
}
export function createCheck(data) {
  return request({
    url: 'api/Check/PostDoCreate',
    method: 'post',
    data
  })
}

// 创建盘点单任务
export function postDoCreateCheck(data) {
  return request({
    url: 'api/Check/PostDoCreateCheck',
    method: 'post',
    data
  })
}

export function editCheck(data) {
  return request({
    url: 'api/Check/PostDoEdit',
    method: 'post',
    data
  })
}
export function cancelCheck(data) {
  return request({
    url: 'api/Check/PostDoCancel',
    method: 'post',
    data
  })
}
export function submitCheck(data) {
  return request({
    url: 'api/Check/PostDoSubmit',
    method: 'post',
    data
  })
}
export function handCheck(data) {
  return request({
    url: 'api/Check/PostDoHandCheck',
    method: 'post',
    data
  })
}
export function getCheckDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}
export function getLocationList(query, WareHouseCode) {
  return request({
    url: 'api/Check/GetLocationList',
    method: 'get',
    params: { query: query, WareHouseCode: WareHouseCode }
  })
}
export function getWarehouseList(query) {
  return request({
    url: 'api/Check/GetWareHouseList',
    method: 'get',
    params: query
  })
}
export function checkAgain(data) {
  return request({
    url: 'api/Check/PostDoCheckAgain',
    method: 'post',
    data: data
  })
}
export function getMaterialList(query) {
  return request({
    url: 'api/Check/GetMaterialList',
    method: 'get',
    params: { keyValue: query }
  })
}

export function createCheckDetail(data) {
  return request({
    url: 'api/Check/PostDoCreateDetail',
    method: 'post',
    data
  })
}
export function HandleSendPTL(data) {
  return request({
    url: 'api/Check/PostDoSendPTL',
    method: 'post',
    data: data
  })
}

export function GetWareHouseAreaList(query) {
  return request({
    url: 'api/Check/GetWareHouseAreaList',
    method: 'get',
    params: { WareHouseCode: query }
  })
}

export function GetCheckAreaList(query) {
  return request({
    url: 'api/Check/GetCheckAreaList',
    method: 'get',
    params: { CheckCode: query }
  })
}

export function CheckTaskDoFinish(data) {
  return request({
    url: 'api/Check/CheckTaskDoFinish',
    method: 'post',
    data: data
  })
}
export function CheckTaskDoStart(data) {
  return request({
    url: 'api/Check/CheckTaskDoStart',
    method: 'post',
    data: data
  })
}

export function PostPDACheckComplete(data) {
  return request({
    url: 'api/Check/PostPDACheckComplete',
    method: 'post',
    data: data
  })
}
