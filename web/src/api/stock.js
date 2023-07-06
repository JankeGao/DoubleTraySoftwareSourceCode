import request from '@/utils/request'
export function getPageRecords(query) {
  return request({
    url: 'api/Stock/GetPageRecords',
    method: 'get',
    params: query
  })
}

export function getStockLabel(query) {
  return request({
    url: 'api/Stock/GetStockLabel',
    method: 'get',
    params: { code: query }
  })
}

export function ouLoadStockInfo(data) {
  return request({
    url: 'api/Stock/DoUpLoadInInfo',
    method: 'post',
    data: data
  })
}

export function getMaterialPageRecords(query) {
  return request({
    url: 'api/Stock/GetMaterialPageRecords',
    method: 'get',
    params: query
  })
}
export function LightStock(data) {
  return request({
    url: 'api/Stock/LightStock',
    method: 'post',
    data: data
  })
}
export function OffLightStock() {
  return request({
    url: 'api/Stock/OffLightStock',
    method: 'post'
  })
}
export function DeleteStockArray(data) {
  return request({
    url: 'api/Stock/DeleteStockArray',
    method: 'post',
    data: data
  })
}

export function getWarehouseList() {
  return request({
    url: 'api/MobileLocation/GetWareHouseList',
    method: 'get'
  })
}

export function getInactiveStockDictTypeList(data) {
  return request({
    url: 'api/Dictionary/GetDictionaryByType',
    method: 'get',
    params: { type: data }
  })
}

export function getInactiveStockPageRecords(data) {
  return request({
    url: 'api/Stock/GetInactiveStockPageRecords',
    method: 'get',
    params: data
  })
}

export function getMaterialStatusPageRecords(data) {
  return request({
    url: 'api/Stock/GetMaterialStatusPageRecords',
    method: 'get',
    params: data
  })
}

export function getMaterialStatusList(data) {
  return request({
    url: 'api/Stock/GetMaterialStatusList',
    method: 'get',
    params: { MaterialCode: data }
  })
}

export function getInventoryStatusPageRecords(data) {
  return request({
    url: 'api/Stock/GetInventoryStatusPageRecords',
    method: 'get',
    params: data
  })
}

export function getLocationStockByLayoutId(data) {
  return request({
    url: 'api/Stock/GetLocationStockByLayoutId',
    method: 'get',
    params: { layoutId: data }
  })
}

export function DoDownLoadLabelStock(data) {
  return request({
    url: 'api/Stock/DoDownLoadLabelStock',
    method: 'get',
    params: data,
    responseType: 'blob'
  })
}
export function DoDownLoadMaterialStock(data) {
  return request({
    url: 'api/Stock/DoDownLoadMaterialStock',
    method: 'get',
    params: data,
    responseType: 'blob'
  })
}
