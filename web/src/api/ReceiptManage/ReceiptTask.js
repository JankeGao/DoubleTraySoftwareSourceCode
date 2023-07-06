import request from '@/utils/request'
export function getInList(query) {
  return request({
    url: 'api/InTask/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getInTaskMaterialList(data) {
  return request({
    url: 'api/InTask/GetInTaskMaterialList',
    method: 'get',
    params: { inTaskCode: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/InTask/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createInTask(data) {
  return request({
    url: 'api/InTask/PostDoCreate',
    method: 'post',
    data: data
  })
}
export function getWarehouseList() {
  return request({
    url: 'api/InTask/GetWareHouseList',
    method: 'get'
  })
}

export function deleteInTask(data) {
  return request({
    url: 'api/InTask/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editIn(data) {
  return request({
    url: 'api/InTask/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/InTask/GetEditMaterialList',
    method: 'get',
    params: { inCode: data }
  })
}

export function getInDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}
export function ouLoadInInfo(data) {
  return request({
    url: 'api/InTask/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function handleSendReceiptOrder(data) {
  return request({
    url: 'api/InTask/PostDoSendOrder',
    method: 'post',
    data: data
  })
}

export function getLocationList(data) {
  return request({
    url: 'api/InTask/GetLocationList',
    method: 'get',
    params: data
  })
}

export function shelfInTaskMaterial(data) {
  return request({
    url: 'api/InTask/PostDoHandShelf',
    method: 'post',
    data: data
  })
}


export function postDoStartContrainer(data) {
  return request({
    url: 'api/InTask/PostDoStartContrainer',
    method: 'post',
    data: data
  })
}
//存入货柜
export function postRestoreContrainer(data) {
  return request({
    url: 'api/InTask/PostRestoreContrainer',
    method: 'post',
    data: data
  })
}
