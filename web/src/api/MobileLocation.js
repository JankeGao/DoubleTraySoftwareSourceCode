import request from '@/utils/request'
export function getWarehouseList() {
  return request({
    url: 'api/MobileLocation/GetWareHouseList',
    method: 'get'
  })
}

export function getMaterialLabelListByWareHouseCode(code) {
  return request({
    url: 'api/MobileLocation/GetMaterialLabelListByWareHouseCode',
    method: 'get',
    params: { WareHouseCode: code }
  })
}

export function getAreaTreeData(code) {
  return request({
    url: 'api/MobileLocation/GetAreaTreeData',
    method: 'get',
    params: { WareHouseCode: code }
  })
}

export function getLocationPageRecords(query) {
  return request({
    url: 'api/MobileLocation/GetLocationPageRecords',
    method: 'get',
    params: query
  })
}

export function getPageRecords(query) {
  return request({
    url: 'api/MobileLocation/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function createMobileLocation(data) {
  return request({
    url: 'api/MobileLocation/CreateMobileLocation',
    method: 'post',
    data: data
  })
}

export function GetMaterialList(data) {
  return request({
    url: 'api/MobileLocation/GetMaterialList',
    method: 'get',
    params: { KeyValue: data }
  })
}
