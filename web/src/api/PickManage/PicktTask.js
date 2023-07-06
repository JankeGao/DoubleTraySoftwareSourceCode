import request from '@/utils/request'
export function getOutList(query) {
  return request({
    url: 'api/OutTask/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function getOutTaskMaterialList(data) {
  return request({
    url: 'api/OutTask/GetOutTaskMaterialList',
    method: 'get',
    params: { OutTaskCode: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/OutTask/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createOutTask(data) {
  return request({
    url: 'api/OutTask/PostDoCreate',
    method: 'post',
    data: data
  })
}
export function getWarehouseList() {
  return request({
    url: 'api/OutTask/GetWareHouseList',
    method: 'get'
  })
}

export function deleteOutTask(data) {
  return request({
    url: 'api/OutTask/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editOut(data) {
  return request({
    url: 'api/OutTask/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/OutTask/GetEditMaterialList',
    method: 'get',
    params: { OutCode: data }
  })
}

export function getOutDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}
export function ouLoadOutOutfo(data) {
  return request({
    url: 'api/OutTask/DoUpLoadOutOutfo',
    method: 'post',
    data: data
  })
}

export function handleSendReceiptOrder(data) {
  return request({
    url: 'api/OutTask/PostDoSendOrder',
    method: 'post',
    data: data
  })
}

export function getLocationList(data) {
  return request({
    url: 'api/OutTask/GetLocationList',
    method: 'get',
    params: data
  })
}

export function shelfOutTaskMaterial(data) {
  return request({
    url: 'api/OutTask/ConfirmHandPicked',
    method: 'post',
    data: data
  })
}

export function postTaskMaterialLabelList(data) {
  return request({
    url: 'api/OutTask/PostTaskMaterialLabelList',
    method: 'post',
    data: data
  })
}

export function postDoStartContrainer(data) {
  return request({
    url: 'api/OutTask/PostDoStartContrainer',
    method: 'post',
    data: data
  })
}

//存入货柜
export function postRestoreContrainer(data) {
  return request({
    url: 'api/OutTask/PostRestoreContrainer',
    method: 'post',
    data: data
  })
}
