import request from '@/utils/request'
// 查询领用单信息
export function getRecipientsOrdersList(query) {
  return request({
    url: 'api/Receive/GetPageRecords',
    method: 'get',
    params: query
  })
}

// 领用明细
export function getReceiveMaterialList(data) {
  return request({
    url: 'api/Receive/GetReceiveMaterialList',
    method: 'get',
    params: { Code: data }
  })
}
// 添加领用单
export function createReceive(data) {
  return request({
    url: 'api/Receive/PostDoCreate',
    method: 'post',
    data
  })
}
// 获取仓库列表信息
export function getWarehouseList() {
  return request({
    url: 'api/Receive/GetWareHouseList',
    method: 'get'
  })
}
// 获取用户列表信息
export function geUserList() {
  return request({
    url: 'api/User/GetUserInfos',
    method: 'get'
  })
}

// 删除
export function deleteRecipientsOrders(data) {
  return request({
    url: 'api/Receive/PostDoDelete',
    method: 'post',
    data: data
  })
}

// 编辑
export function editReceive(data) {
  return request({
    url: 'api/Receive/PostDoUpdate',
    method: 'post',
    data: data
  })
}

// 作废
export function postDoCancellatione(data) {
  return request({
    url: 'api/Receive/PostDoCancellatione',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/Receive/GetEditMaterialList',
    method: 'get',
    params: { inCode: data }
  })
}

// 领用单导入
export function ouLoadInInfo(data) {
  return request({
    url: 'api/Receive/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function getInterfaceReceive(data) {
  return request({
    url: 'api/Receive/GetInterfaceReceive',
    method: 'get'
  })
}

