import request from '@/utils/request'
export function getPickMainList(query) {
  return request({
    url: 'api/PickMain/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetPickMaterialList(data) {
  return request({
    url: 'api/PickMain/GetPickMaterialList',
    method: 'get',
    params: { Id: data }
  })
}

export function getMaterialList(data) {
  return request({
    url: 'api/PickMain/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function createOut(data) {
  return request({
    url: 'api/PickMain/PostDoCreate',
    method: 'post',
    data: data
  })
}
export function getWarehouseList() {
  return request({
    url: 'api/PickMain/GetWareHouseList',
    method: 'get'
  })
}

export function deleteOut(data) {
  return request({
    url: 'api/PickMain/PostDoDelete',
    method: 'post',
    data: data
  })
}

export function editOut(data) {
  return request({
    url: 'api/PickMain/PostDoUpdate',
    method: 'post',
    data: data
  })
}

export function getEditMaterialList(data) {
  return request({
    url: 'api/PickMain/GetEditMaterialList',
    method: 'get',
    params: { Id: data }
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
    url: 'api/PickMain/GetAvailableStock',
    method: 'get',
    params: { MaterialCode: MaterialCode, WareHouseCode: WareHouseCode }
  })
}

export function HandleSendPickOrder(data) {
  return request({
    url: 'api/PickMain/PostDoSendOrder',
    method: 'post',
    data: data
  })
}
export function HandleGenerateOutLabel(data) {
  return request({
    url: 'api/PickMain/PostDoGenerateOutLabel',
    method: 'post',
    data: data
  })
}
export function GetCombineOutMaterialList(data) {
  return request({
    url: 'api/PickMain/GetCombineOutMaterialList',
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
    url: 'api/PickMain/ConfirmHandPicked',
    method: 'post',
    data: data
  })
}
export function getWaiitingForCheckOrCheckedLabel(OutCode, Status, MaterialLabel) {
  return request({
    url: 'api/PickMain/GetWaiitingForCheckOrCheckedLabel',
    method: 'get',
    params: { OutCode: OutCode, Status: Status, MaterialLabel: MaterialLabel }
  })
}

export function confirmCheckLabel(data) {
  return request({
    url: 'api/PickMain/ConfirmCheckLabel',
    method: 'post',
    data: data
  })
}
export function ouLoadOutInfo(data) {
  return request({
    url: 'api/PickMain/DoUpLoadOutInfo',
    method: 'post',
    data: data
  })
}

export function downLoadTemp(data) {
  return request({
    url: 'api/PickMain/DoDownLoadTemp',
    method: 'get',
    params: data
  })
}

export function HandleCombinePickMainOrder(data) {
  return request({
    url: 'api/PickMain/HandleCombinePickMainOrder',
    method: 'post',
    data: data
  })
}
