import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/CheckList/GetPageRecords',
    method: 'get',
    params: query
  })
}
// 作废
export function cancelCheck(data) {
  return request({
    url: 'api/CheckList/PostDoCancellatione',
    method: 'post',
    data
  })
}

// 盘点明细
export function getCheckListMaterialList(data) {
  return request({
    url: 'api/CheckList/GetCheckListDetailPageRecords',
    method: 'get',
    params: data
  })
}
export function getCheckListDetailList(data) {
  return request({
    url: 'api/CheckList/GetCheckListDetailList',
    method: 'get',
    params: { Code: data }
  })
}

// 获取货柜
export function getCheckListContainerList(data) {
  return request({
    url: 'api/CheckList/GetCheckListContainerList',
    method: 'get',
    params: { WareHosueCode: data }
  })
}

export function handCheck(data) {
  return request({
    url: 'api/Check/PostDoHandCheck',
    method: 'post',
    data
  })
}

// 添加盘点单
export function postDoCreate(data) {
  return request({
    url: 'api/CheckList/PostDoCreate',
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

// 获取托盘编码
export function getLocationList(query, WareHouseCode) {
  return request({
    url: 'api/CheckList/GetLocationList',
    method: 'get',
    params: { query: query, WareHouseCode: WareHouseCode }
  })
}

// 获取仓库信息
export function getWarehouseList(query) {
  return request({
    url: 'api/CheckList/GetWareHouseList',
    method: 'get',
    params: query
  })
}
// 获取货柜信息
export function getContainerList(query) {
  return request({
    url: 'api/CheckList/GetContainerList',
    method: 'get',
    params: query
  })
}
