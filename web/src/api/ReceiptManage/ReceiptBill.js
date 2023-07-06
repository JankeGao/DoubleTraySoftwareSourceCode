import request from '@/utils/request'
export function getInList(query) {
  return request({
    url: 'api/In/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetInMaterialList(data) {
  return request({
    url: 'api/In/GetInMaterialList',
    method: 'get',
    params: { InCode: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/In/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createIn(data) {
  return request({
    url: 'api/In/PostDoCreate',
    method: 'post',
    data: data
  })
}
export function getWarehouseList() {
  return request({
    url: 'api/In/GetWareHouseList',
    method: 'get'
  })
}

export function deleteIn(data) {
  return request({
    url: 'api/In/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editIn(data) {
  return request({
    url: 'api/In/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function cancelIn(data) {
  return request({
    url: 'api/In/PostDoCancel',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/In/GetEditMaterialList',
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
    url: 'api/In/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function handleSendReceiptOrder(data) {
  return request({
    url: 'api/In/PostDoSendOrder',
    method: 'post',
    data: data
  })
}

export function getInterfaceIn(data) {
  return request({
    url: 'api/In/GetInterfaceIn',
    method: 'get'
  })
}
