import request from '@/utils/request'
export function getInMaterialList(query) {
  return request({
    url: 'api/In/GetNeedShelfPageRecords',
    method: 'get',
    params: query
  })
}

export function getLocationList(data, WareHouseCode) {
  return request({
    url: 'api/In/GetLocationList',
    method: 'get',
    params: { KeyValue: data, WareHouseCode: WareHouseCode }
  })
}

export function shelfInMaterial(data) {
  return request({
    url: 'api/In/PostDoHandShelf',
    method: 'post',
    data: data
  })
}

export function getInMaterialByLabel(data) {
  return request({
    url: 'api/In/GetInMaterialByLabel',
    method: 'get',
    params: { MaterialLabel: data }
  })
}

