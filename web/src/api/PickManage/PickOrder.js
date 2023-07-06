import request from '@/utils/request'
export function getOutList(query) {
  return request({
    url: 'api/Out/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetOutMaterialList(data) {
  return request({
    url: 'api/Out/GetOutMaterialList',
    method: 'get',
    params: { OutCode: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/Out/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createOut(data) {
  return request({
    url: 'api/Out/PostDoCreate',
    method: 'post',
    data: data
  })
}
export function getWarehouseList() {
  return request({
    url: 'api/Out/GetWareHouseList',
    method: 'get'
  })
}

export function deleteOut(data) {
  return request({
    url: 'api/Out/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editOut(data) {
  return request({
    url: 'api/Out/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/Out/GetEditMaterialList',
    method: 'get',
    params: { outCode: data }
  })
}

export function getOutDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}

export function getAvailableStock(MaterialCode, WareHouseCode) {
  return request({
    url: 'api/Out/GetAvailableStock',
    method: 'get',
    params: { MaterialCode: MaterialCode, WareHouseCode: WareHouseCode }
  })
}

export function HandleSendPickOrder(data) {
  return request({
    url: 'api/Out/PostDoSendOrder',
    method: 'post',
    data: data
  })
}
export function HandleGenerateOutLabel(data) {
  return request({
    url: 'api/Out/PostDoGenerateOutLabel',
    method: 'post',
    data: data
  })
}
export function GetCombineOutMaterialList(data) {
  return request({
    url: 'api/Out/GetCombineOutMaterialList',
    method: 'get',
    params: { OutCode: data }
  })
}

export function GetStockPageRecords(query) {
  return request({
    url: 'api/Stock/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function confirmHandPicked(data) {
  return request({
    url: 'api/Out/ConfirmHandPicked',
    method: 'post',
    data: data
  })
}
export function getWaiitingForCheckOrCheckedLabel(OutCode, Status, MaterialLabel) {
  return request({
    url: 'api/Out/GetWaiitingForCheckOrCheckedLabel',
    method: 'get',
    params: { OutCode: OutCode, Status: Status, MaterialLabel: MaterialLabel }
  })
}

export function confirmCheckLabel(data) {
  return request({
    url: 'api/Out/ConfirmCheckLabel',
    method: 'post',
    data: data
  })
}
export function ouLoadOutInfo(data) {
  return request({
    url: 'api/Out/DoUpLoadOutInfo',
    method: 'post',
    data: data
  })
}

export function downLoadTemp(data) {
  return request({
    url: 'api/Out/DoDownLoadTemp',
    method: 'get',
    params: data
  })
}

export function cancelOut(data) {
  return request({
    url: 'api/Out/PostDoCancel',
    method: 'post',
    data: data
  })
}

export function getInterfaceOut(data) {
  return request({
    url: 'api/Out/GetInterfaceOut',
    method: 'get'
  })
}
