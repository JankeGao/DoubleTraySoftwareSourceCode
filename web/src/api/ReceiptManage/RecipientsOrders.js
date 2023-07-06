import request from '@/utils/request'
// 查询领用单信息
export function getRecipientsOrdersList(query) {
  return request({
    url: 'api/RecipientsOrders/GetPageRecords',
    method: 'get',
    params: query
  })
}

// 导入模板下载
export function getRecipientsOrdersMaterialList(data) {
  return request({
    url: 'api/RecipientsOrders/GetRecipientsOrdersMaterialList',
    method: 'get',
    params: { InCode: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/RecipientsOrders/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createIn(data) {
  return request({
    url: 'api/RecipientsOrders/PostDoCreate',
    method: 'post',
    data: data
  })
}

// 获取仓库列表信息
export function getWarehouseList() {
  return request({
    url: 'api/RecipientsOrders/GetWareHouseList',
    method: 'get'
  })
}

export function deleteRecipientsOrders(data) {
  return request({
    url: 'api/RecipientsOrders/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editIn(data) {
  return request({
    url: 'api/RecipientsOrders/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/RecipientsOrders/GetEditMaterialList',
    method: 'get',
    params: { inCode: data }
  })
}

// 获取添加的类型选择
export function getInDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}

// 领用单导入
export function ouLoadInInfo(data) {
  return request({
    url: 'api/RecipientsOrders/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function handleSendReceiptOrder(data) {
  return request({
    url: 'api/RecipientsOrders/PostDoSendOrder',
    method: 'post',
    data: data
  })
}
