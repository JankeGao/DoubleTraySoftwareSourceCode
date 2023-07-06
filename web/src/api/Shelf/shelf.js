import request from '@/utils/request'
export function GetPageRecords(query) {
  return request({
    url: 'api/Shelf/GetPageRecords',
    method: 'get',
    params: query
  })
}
export function GetShelfDetailList(data) {
  return request({
    url: 'api/Shelf/GetShelfDetailList',
    method: 'get',
    params: { code: data }
  })
}

export function GetLabelInfoByLabel(data) {
  return request({
    url: 'api/Shelf/GetLabelInfoByLabel',
    method: 'get',
    params: { label: data }
  })
}

export function WebConfirmShelf(data) {
  return request({
    url: 'api/Shelf/WebConfirmShelf',
    method: 'post',
    data: data
  })
}

export function CompelFinishedReplenishOrder(data) {
  return request({
    url: 'api/Shelf/CompelFinishedReplenishOrder',
    method: 'post',
    data: data
  })
}

export function GetLocationList() {
  return request({
    url: 'api/Shelf/GetLocationList',
    method: 'get'
  })
}
export function GetMaterialList(data) {
  return request({
    url: 'api/Shelf/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}
export function GetSupplierList(data) {
  return request({
    url: 'api/Shelf/GetSupplierList',
    method: 'get',
    params: { KeyValue: data }
  })
}

export function GenerateLabel() {
  return request({
    url: 'api/Shelf/GenerateLabel',
    method: 'post'
  })
}
